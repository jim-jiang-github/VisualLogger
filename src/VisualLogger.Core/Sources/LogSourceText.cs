using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Core.Convertors;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Sources
{
    internal class LogSourceText : LogSource<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText>
    {
        private LogSourceText(Stream stream, SchemaLog<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText> schemaLog) : base(stream, schemaLog)
        {
        }
        protected override BlockSource CreateBlockSource(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            SchemaLogText.SchemaBlockText block,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition)
        {
            CellSource[] cells = Array.Empty<CellSource>();
            bool isItemCreating = false;
            var stringBuilder = new StringBuilder();
            var startPosition = streamPosition;
            string? line = mixStreamReader.ReadLine(true);
            int index = 0;

            while (line != null || isItemCreating)
            {
                if (line != null && !isItemCreating && Regex.IsMatch(line, block.RegexStart, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    isItemCreating = true;
                    startPosition = mixStreamReader.BufferPosition;
                    line = mixStreamReader.ReadLine(true);
                }
                else if (line != null && Regex.IsMatch(line, block.RegexEnd, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    startPosition = mixStreamReader.BufferPosition;
                    line = mixStreamReader.ReadLine(true);
                }
                else
                {
                    var itemContent = stringBuilder.ToString();
                    stringBuilder.Clear();
                    var match = Regex.Match(itemContent, block.RegexContent, RegexOptions.Singleline);
                    cells = new CellSource[block.Cells.Length];
                    for (int i = 0; i < block.Cells.Length; i++)
                    {
                        var cell = block.Cells[i];
                        var streamCell = CreateStreamCell(mixStreamReader, logSource, index++, match, cell, cellConvertorProvider, ref streamPosition);
                        cells[i] = new CellSource(cell.Name, streamCell);
                    }
                    break;
                }
            }
            streamPosition = startPosition;
            return new BlockSource(block.Name, cells);
        }
        private StreamCell CreateStreamCell(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            int index,
            Match match,
            SchemaLogText.SchemaCellText cell,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition)
        {
            var capture = match.Groups[cell.RegexGroupIndex];
            var startPosition = streamPosition + capture.Index;
            var streamCell = new StreamCell(mixStreamReader, index, startPosition, capture.Length, StreamCellType.String, cellConvertorProvider.GetConvertor(cell.ConvertorName));
            return streamCell;
        }

        protected override ColumnHeadSource CreateContentSource(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            SchemaLogText.SchemaColumnHeadText columnHead,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition)
        {
            var items = new List<StreamCell[]>();
            bool isItemCreating = false;
            var stringBuilder = new StringBuilder();
            var startPosition = streamPosition;
            string? line = mixStreamReader.ReadLine(true);
            int index = 0;

            while (line != null || isItemCreating)
            {
                if (line != null && !isItemCreating && Regex.IsMatch(line, columnHead.RegexStart, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    isItemCreating = true;
                    startPosition = mixStreamReader.BufferPosition;
                    line = mixStreamReader.ReadLine(true);
                }
                else if (line != null && Regex.IsMatch(line, columnHead.RegexEnd, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    startPosition = mixStreamReader.BufferPosition;
                    line = mixStreamReader.ReadLine(true);
                }
                else
                {
                    var itemContent = stringBuilder.ToString();
                    stringBuilder.Clear();
                    isItemCreating = false;
                    var match = Regex.Match(itemContent, columnHead.RegexContent, RegexOptions.Singleline);

                    var item = new StreamCell[columnHead.Columns.Length];
                    for (int i = 0; i < item.Length; i++)
                    {
                        var column = columnHead.Columns[i];
                        var streamCell = CreateStreamCell(mixStreamReader, logSource, index++, match, column.Cell, cellConvertorProvider, ref streamPosition);
                        HandleContentCell(column, streamCell);
                        item[i] = streamCell;
                    }
                    items.Add(item);
                    streamPosition = startPosition;
                }
            }
            TotalRowsCount = items.Count;
            return new ColumnHeadSource(columnHead.Columns.Select(t => t.Cell.Name).ToArray(), items);
        }
    }
}
