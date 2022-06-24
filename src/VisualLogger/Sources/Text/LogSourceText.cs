using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
using VisualLogger.Sources.Binary;
using VisualLogger.Sources.Text;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    internal class LogSourceText : LogSource<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText>
    {
        private Encoding _encoding;
        private StreamBytesLineReader _streamBytesLineReader;

#nullable disable
        private LogSourceText(Stream stream, SchemaLog<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText> schemaLog) : base(stream, schemaLog)
#nullable enable
        {
        }

        protected override void Init(Stream stream, SchemaLog<SchemaLogText.SchemaBlockText, SchemaLogText.SchemaColumnHeadText, SchemaLogText.SchemaCellText> schemaLog)
        {
            _encoding = Encoding.GetEncoding(schemaLog.EncodingName);
            _streamBytesLineReader = new StreamBytesLineReader(stream, _encoding);
        }

        protected override BlockSource CreateBlockSource(
            SchemaLogText.SchemaBlockText block,
            ref long streamPosition)
        {
            bool isItemCreating = false;
            var stringBuilder = new StringBuilder();
            var startPosition = streamPosition;

            var blockCells = new BlockCellSource[block.Cells.Length];
            var cellConvertors = new CellConvertor?[block.Cells.Length];
            var logSourceReaderText = new LogSourceReaderText(_stream, _encoding, cellConvertors);
            var linePosition = _streamBytesLineReader.ReadLinePosition();
            string? line = _streamBytesLineReader.ReadString(linePosition);
            while (line != null || isItemCreating)
            {
                if (line != null && !isItemCreating && Regex.IsMatch(line, block.RegexStart, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    isItemCreating = true;
                    startPosition = _streamBytesLineReader.Position;
                    linePosition = _streamBytesLineReader.ReadLinePosition();
                    line = _streamBytesLineReader.ReadString(linePosition);
                }
                else if (line != null && Regex.IsMatch(line, block.RegexEnd, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    startPosition = _streamBytesLineReader.Position;
                    linePosition = _streamBytesLineReader.ReadLinePosition();
                    line = _streamBytesLineReader.ReadString(linePosition);
                }
                else
                {
                    var itemContent = stringBuilder.ToString();
                    stringBuilder.Clear();
                    var match = Regex.Match(itemContent, block.RegexContent, RegexOptions.Singleline);
                    for (int i = 0; i < block.Cells.Length; i++)
                    {
                        var cell = block.Cells[i];
                        cellConvertors[i] = _convertorProvider.GetConvertor(cell.ConvertorName);
                        var capture = match.Groups[cell.RegexGroupIndex];
                        long position = startPosition + _streamBytesLineReader.CurrentEncoding.GetByteCount(itemContent.AsSpan(0, capture.Index));
                        int length = _streamBytesLineReader.CurrentEncoding.GetByteCount(itemContent.AsSpan(capture.Index, capture.Length));
                        var streamCell = CreateCell(position, length);
                        blockCells[i] = new BlockCellSource(cell.Name, streamCell);
                    }
                    break;
                }
            }
            streamPosition = startPosition;
            return new BlockSource(logSourceReaderText, block.Name, blockCells);
        }

        protected override ContentSource CreateContentSource(
            IBlockCellFinder blockCellSearchable,
            ref long streamPosition)
        {
            var rows = new List<RowSource>();
            bool isItemCreating = false;
            var stringBuilder = new StringBuilder();
            var startPosition = streamPosition;
            int index = 0;
            var cellConvertors = new CellConvertor?[_schemaLog.ColumnHeadTemplate.Columns.Length];
            for (int i = 0; i < _schemaLog.ColumnHeadTemplate.Columns.Length; i++)
            {
                cellConvertors[i] = _convertorProvider.GetConvertor(_schemaLog.ColumnHeadTemplate.Columns[i].Cell.ConvertorName);
            }
            var logSourceReaderText = new LogSourceReaderText(_stream, _encoding, cellConvertors);

            var linePosition = _streamBytesLineReader.ReadLinePosition();
            string? line = _streamBytesLineReader.ReadString(linePosition);
            while (line != null || isItemCreating)
            {
                if (line != null && !isItemCreating && Regex.IsMatch(line, _schemaLog.ColumnHeadTemplate.RegexStart, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    isItemCreating = true;
                    linePosition = _streamBytesLineReader.ReadLinePosition();
                    line = _streamBytesLineReader.ReadString(linePosition);
                }
                else if (line != null && Regex.IsMatch(line, _schemaLog.ColumnHeadTemplate.RegexEnd, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    linePosition = _streamBytesLineReader.ReadLinePosition();
                    line = _streamBytesLineReader.ReadString(linePosition);
                }
                else
                {
                    var byteLength = 0;
                    if (linePosition != null)
                    {
                        byteLength = (int)(linePosition.Value.Item1 - startPosition);
                    }
                    var itemContent = stringBuilder.ToString();
                    stringBuilder.Clear();
                    isItemCreating = false;
                    var match = Regex.Match(itemContent, _schemaLog.ColumnHeadTemplate.RegexContent, RegexOptions.Singleline);
                    CellSource[] cells = new CellSource[_schemaLog.ColumnHeadTemplate.Columns.Length];
                    for (int cellIndex = 0; cellIndex < cells.Length; cellIndex++)
                    {
                        var column = _schemaLog.ColumnHeadTemplate.Columns[cellIndex];

                        var capture = match.Groups[column.Cell.RegexGroupIndex];
                        int start = _streamBytesLineReader.CurrentEncoding.GetByteCount(itemContent.AsSpan(0, capture.Index));
                        long position = startPosition + start;
                        int length = _streamBytesLineReader.CurrentEncoding.GetByteCount(itemContent.AsSpan(capture.Index, capture.Length));
                        var cell = CreateCell(position, length);
                        HandleContentCellValue(column, logSourceReaderText, cell, cellIndex);
                        cells[cellIndex] = cell;
                    }
                    rows.Add(new RowSource(index++, cells));
                    startPosition += byteLength;
                }
            }
            TotalRowsCount = rows.Count;
            return new ContentSource(logSourceReaderText, _schemaLog.ColumnHeadTemplate.Columns.Select(t => t.Cell.Name).ToArray(), rows);
        }

        private CellSource CreateCell(
            long position,
            int length)
        {
            var streamCell = new CellSource(position, length);
            return streamCell;
        }

        public override void Dispose()
        {
            base.Dispose();
            _streamBytesLineReader.Close();
        }
    }
}
