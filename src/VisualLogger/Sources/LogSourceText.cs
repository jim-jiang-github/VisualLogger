using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Commons;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    internal class LogSourceText : LogSource<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText>
    {
        private ReadLinesIterator? _readLinesIterator;
        private string[]? _lines;

        private LogSourceText(Stream stream, SchemaLog<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText> schemaLog) : base(stream, schemaLog)
        {
            var streamReader = new StreamReader(stream, _encoding);
            _readLinesIterator = new ReadLinesIterator(streamReader);
        }

        protected override BlockSource CreateBlockSource(SchemaLogText.SchemaBlockText block)
        {
            if (_readLinesIterator == null)
            {
                throw new ArgumentException("_readLinesIterator is null.");
            }
            string? line = _readLinesIterator.ReadNext();
            while (line != null && !Regex.IsMatch(line, block.RegexStart, RegexOptions.Singleline))
            {
                line = _readLinesIterator.ReadNext();
            }
            if (line == null)
            {
                throw new ArgumentException("block format error.");
            }
            var stringBuilder = new StringBuilder(line);
            line = _readLinesIterator.ReadNext();
            while (line != null && !Regex.IsMatch(line, block.RegexEnd, RegexOptions.Singleline))
            {
                stringBuilder.AppendLine(line);
                line = _readLinesIterator.ReadNext();
            }
            stringBuilder.AppendLine(line);
            var itemContent = stringBuilder.ToString();
            var match = Regex.Match(itemContent, block.RegexContent, RegexOptions.Singleline);
            if (match.Groups.Count != block.Cells.Length + 1)
            {
                throw new ArgumentException("block cell format error.");
            }
            var blockCells = new BlockCellSource[block.Cells.Length];
            for (int i = 0; i < block.Cells.Length; i++)
            {
                var cell = block.Cells[i];
                var cellConvertor = _convertorProvider.GetConvertor(cell.ConvertorName);
                var captureCell = match.Groups[cell.RegexGroupIndex].Value;
                blockCells[i] = new BlockCellSource(cell.Name, captureCell.Convert(cellConvertor) ?? "");
            }
            return new BlockSource(block.Name, blockCells);
        }

        protected override int GetTotalCount(IBlockCellFinder blockCellFinder)
        {
            if (_readLinesIterator == null)
            {
                throw new ArgumentException("_readLinesIterator is null.");
            }
            int index = 0;
            _lines = _readLinesIterator
             .AsParallel()
             .AsOrdered()
             .Select(l =>
             {
                 return (Regex.IsMatch(l, _schemaLog.ColumnHeadTemplate.RegexStart), l);
             })
             .AsSequential()
             .Select(x =>
             {
                 if (x.Item1)
                 {
                     index++;
                 }
                 return (index, x.l);
             })
             .ToLookup(x => x.index, x => x.l)
             .AsParallel()
             .AsOrdered()
             .Select(x => string.Join(Environment.NewLine, x))
             .ToArray();
            return _lines.Length;
        }

        protected override ContentSource CreateContentSource(
            IBlockCellFinder blockCellFinder)
        {
            if (_lines == null)
            {
                throw new ArgumentException("_lines is null.");
            }

            var cellConvertors = new CellConvertor?[_schemaLog.ColumnHeadTemplate.Columns.Length];
            for (int i = 0; i < _schemaLog.ColumnHeadTemplate.Columns.Length; i++)
            {
                cellConvertors[i] = _convertorProvider.GetConvertor(_schemaLog.ColumnHeadTemplate.Columns[i].Cell.ConvertorName);
            }

            var rowSources = _lines
                .AsParallel()
                .AsOrdered()
                .Select((l, i) =>
                {
                    var match = Regex.Match(l, _schemaLog.ColumnHeadTemplate.RegexContent, RegexOptions.Singleline);
                    var captureCells = match.Groups.Values.Skip(1);
                    var cells = cellConvertors.Zip(captureCells, (convertor, cell) =>
                    {
                        if (convertor == null)
                        {
                            return cell.Value;
                        }
                        else
                        {
                            return convertor.Convert(cell.Value);
                        }
                    });
                    return new LogRow(i, cells.ToArray());
                })
                .ToArray();

            return new ContentSource(_schemaLog.ColumnHeadTemplate.Columns.Select(t => t.Cell.Name).ToArray(), rowSources);
        }

        public override void Dispose()
        {
            base.Dispose();
            _readLinesIterator?.Dispose();
            _lines = null;
        }
    }
}
