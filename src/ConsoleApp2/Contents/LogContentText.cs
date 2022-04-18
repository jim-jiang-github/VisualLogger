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

namespace VisualLogger.Contents
{
    public class LogContentText : LogContent<LogSchemaText, LogSchemaText.BlockText, LogSchemaText.BodyText, LogSchemaText.CellText>
    {
        public LogContentText(Stream stream, LogSchema<LogSchemaText, LogSchemaText.BlockText, LogSchemaText.BodyText, LogSchemaText.CellText> logSchema) : base(stream, logSchema)
        {
        }
        #region Internal Class
        private class BlockContentText : BlockContent
        {
            public BlockContentText(
                ILogContent logContent,
                MixStreamReader logStreamReader,
                LogSchemaText.BlockText block,
                ref long streamPosition) :
                base(logContent, logStreamReader, block, ref streamPosition)
            {
            }

            protected override BodyContent? CreateBodyContent(
                ILogContent logContent,
                MixStreamReader logStreamReader,
                LogSchemaText.BlockText block,
                LogSchemaText.BodyText body,
                ref long streamPosition)
            {
                if (body.BodyTemplate == null)
                {
                    //TODO log body.BodyTemplate can not be null here.
                    return null;
                }
                if (block.RegexPatternItemStart == null)
                {
                    //TODO log block.RegexPatternItemStart can not be null here.
                    return null;
                }
                if (block.RegexPatternItemContent == null)
                {
                    //TODO log block.RegexPatternItemContent can not be null here.
                    return null;
                }
                if (block.RegexPattern == null)
                {
                    //TODO log block.RegexPattern can not be null here.
                    return null;
                }
                var bodyTemplateNames = body.BodyTemplate.Select(t => t.Name).ToArray();
                var items = new List<StreamCell[]>();
                bool isItemCreating = false;
                var stringBuilder = new StringBuilder();
                var startPosition = streamPosition;
                string? line = logStreamReader.ReadLine(true);

                while (line != null || isItemCreating)
                {
                    if (line != null && !isItemCreating && Regex.IsMatch(line, block.RegexPatternItemStart, RegexOptions.Singleline))
                    {
                        stringBuilder.Append(line);
                        isItemCreating = true;
                        startPosition = logStreamReader.BufferPosition;
                        line = logStreamReader.ReadLine(true);
                    }
                    else if (line != null && Regex.IsMatch(line, block.RegexPatternItemContent, RegexOptions.Singleline))
                    {
                        stringBuilder.Append(line);
                        startPosition = logStreamReader.BufferPosition;
                        line = logStreamReader.ReadLine(true);
                    }
                    else
                    {
                        var itemContent = stringBuilder.ToString();
                        stringBuilder.Clear();
                        isItemCreating = false;
                        var match = Regex.Match(itemContent, block.RegexPattern, RegexOptions.Singleline);

                        var item = new StreamCell[body.BodyTemplate.Length];
                        for (int i = 0; i < item.Length; i++)
                        {
                            var template = body.BodyTemplate[i];
                            var streamCell = CreateStreamCell(logContent, logStreamReader, match, template, ref streamPosition);
                            item[i] = streamCell;
                        }
                        items.Add(item);
                        streamPosition = startPosition;
                    }
                }
                return new BodyContent(bodyTemplateNames.Cast<string>().ToArray(), items.ToArray());
            }

            protected override CellsContent? CreateCellsContent(
                ILogContent logContent,
                MixStreamReader logStreamReader,
                LogSchemaText.BlockText block,
                LogSchemaText.CellText[] cells,
                ref long streamPosition)
            {
                if (block.RegexPatternItemStart == null)
                {
                    //TODO log block.RegexPatternItemStart can not be null here.
                    return null;
                }
                if (block.RegexPatternItemContent == null)
                {
                    //TODO log block.RegexPatternItemContent can not be null here.
                    return null;
                }
                if (block.RegexPattern == null)
                {
                    //TODO log block.RegexPattern can not be null here.
                    return null;
                }
                var cellNames = new List<string>();
                var item = new List<StreamCell>();
                bool isItemCreating = false;
                var stringBuilder = new StringBuilder();
                var startPosition = streamPosition;
                string? line = logStreamReader.ReadLine(true);

                while (line != null || isItemCreating)
                {
                    if (line != null && !isItemCreating && Regex.IsMatch(line, block.RegexPatternItemStart, RegexOptions.Singleline))
                    {
                        stringBuilder.Append(line);
                        isItemCreating = true;
                        startPosition = logStreamReader.BufferPosition;
                        line = logStreamReader.ReadLine(true);
                    }
                    else if (line != null && Regex.IsMatch(line, block.RegexPatternItemContent, RegexOptions.Singleline))
                    {
                        stringBuilder.Append(line);
                        startPosition = logStreamReader.BufferPosition;
                        line = logStreamReader.ReadLine(true);
                    }
                    else
                    {
                        var itemContent = stringBuilder.ToString();
                        stringBuilder.Clear();
                        isItemCreating = false;
                        var match = Regex.Match(itemContent, block.RegexPattern, RegexOptions.Singleline);

                        foreach (var cell in cells)
                        {
                            var streamCell = CreateStreamCell(logContent, logStreamReader, match, cell, ref streamPosition);
                            if (cell.Name != null)
                            {
                                cellNames.Add(cell.Name);
                                item.Add(streamCell);
                            }
                        }
                        break;
                    }
                }
                streamPosition = startPosition;
                return new CellsContent(cellNames.ToArray(), item.ToArray());
            }
            private StreamCell CreateStreamCell(
                ILogContent logContent,
                MixStreamReader logStreamReader,
                Match match,
                LogSchemaText.CellText cell,
                ref long streamPosition)
            {
                var capture = match.Groups[cell.RegexGroupIndex];
                var startPosition = streamPosition + capture.Index;
                var streamCell = new StreamCell(logStreamReader, startPosition, capture.Length, StreamCellType.String, logContent.GetConvertor(cell.ConvertorName));
                return streamCell;
            }
        }
        #endregion
        protected override BlockContent CreateBlockContent(
            ILogContent logContent,
            MixStreamReader mixStreamReader,
            LogSchemaText.BlockText block,
            ref long streamPosition)
        {
            return new BlockContentText(logContent, mixStreamReader, block, ref streamPosition);
        }
    }
}
