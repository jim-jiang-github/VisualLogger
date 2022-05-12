using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Datas.LogSources
{
    public class LogSourceText : LogSource<LogSchemaText, LogSchemaText.BlockText, LogSchemaText.BodyText, LogSchemaText.CellText>
    {
        public LogSourceText(Stream stream, LogSchema<LogSchemaText, LogSchemaText.BlockText, LogSchemaText.BodyText, LogSchemaText.CellText> logSchema) : base(stream, logSchema)
        {
        }
        protected override BlockSource CreateBlockSource(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            LogSchemaText.BlockText block,
            ref long streamPosition)
        {
            if (block.RegexStart == null)
            {
                throw new ArgumentException("block.RegexStart can not be null.");
            }
            if (block.RegexEnd == null)
            {
                throw new ArgumentException("block.RegexEnd can not be null.");
            }
            if (block.RegexContent == null)
            {
                throw new ArgumentException("block.RegexContent can not be null.");
            }
            BlockCell[] cells = Array.Empty<BlockCell>();
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
                    cells = new BlockCell[block.Cells.Length];
                    for (int i = 0; i < block.Cells.Length; i++)
                    {
                        var cell = block.Cells[i];
                        var streamCell = CreateStreamCell(logSource, mixStreamReader, index++, match, cell, ref streamPosition);
                        cells[i] = new BlockCell(cell.Name, streamCell);
                    }
                    break;
                }
            }
            streamPosition = startPosition;
            return new BlockSource(block.Name, cells);
        }
        private StreamCell CreateStreamCell(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            int index,
            Match match,
            LogSchemaText.CellText cell,
            ref long streamPosition)
        {
            var capture = match.Groups[cell.RegexGroupIndex];
            var startPosition = streamPosition + capture.Index;
            var streamCell = new StreamCell(mixStreamReader, index, startPosition, capture.Length, StreamCellType.String, logSource.GetConvertor(cell.ConvertorName));
            return streamCell;
        }

        protected override BodySource CreateBodySource(ILogSource logSource, MixStreamReader mixStreamReader, LogSchemaText.BodyText body, ref long streamPosition)
        {
            if (body.BodyTemplate == null)
            {
                throw new ArgumentException("body.BodyTemplate can not be null.");
            }
            if (body.RegexStart == null)
            {
                throw new ArgumentException("body.RegexStart can not be null.");
            }
            if (body.RegexEnd == null)
            {
                throw new ArgumentException("body.RegexEnd can not be null.");
            }
            if (body.RegexContent == null)
            {
                throw new ArgumentException("body.RegexContent can not be null.");
            }
            var items = new List<StreamCell[]>();
            bool isItemCreating = false;
            var stringBuilder = new StringBuilder();
            var startPosition = streamPosition;
            string? line = mixStreamReader.ReadLine(true);
            int index = 0;

            while (line != null || isItemCreating)
            {
                if (line != null && !isItemCreating && Regex.IsMatch(line, body.RegexStart, RegexOptions.Singleline))
                {
                    stringBuilder.Append(line);
                    isItemCreating = true;
                    startPosition = mixStreamReader.BufferPosition;
                    line = mixStreamReader.ReadLine(true);
                }
                else if (line != null && Regex.IsMatch(line, body.RegexEnd, RegexOptions.Singleline))
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
                    var match = Regex.Match(itemContent, body.RegexContent, RegexOptions.Singleline);

                    var item = new StreamCell[body.BodyTemplate.Length];
                    for (int i = 0; i < item.Length; i++)
                    {
                        var template = body.BodyTemplate[i];
                        var streamCell = CreateStreamCell(logSource, mixStreamReader, index++, match, template, ref streamPosition);
                        item[i] = streamCell;
                    }
                    items.Add(item);
                    streamPosition = startPosition;
                }
            }
            return new BodySource(body.BodyTemplate.Select(t => t.Name).ToArray(), items);
        }
    }
}
