using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Text
{
    public class TextContent
    {
        private class TextBlock
        {
            private readonly TextContent _textContent;

            public string Name { get; }
            public string[] CellNames { get; } = Array.Empty<string>();
            public StreamCell[] Item { get; } = Array.Empty<StreamCell>();
            public string[] ItemsTemplate { get; } = Array.Empty<string>();
            public StreamCell[][] Items { get; } = Array.Empty<StreamCell[]>();

            public TextBlock(TextContent textContent, LogStreamReader logStreamReader, TextContentParser.Block block, ref string line, ref long position)
            {
                _textContent = textContent;
                Name = block.Name;
                if (block.Cells is TextContentParser.Cell[] cellsParser)
                {
                    var cellNames = new List<string>();
                    var item = new List<StreamCell>();
                    bool isItemCreating = false;
                    var stringBuilder = new StringBuilder();
                    var startPosition = position;

                    while (line != null)
                    {
                        if (!isItemCreating && Regex.IsMatch(line, block.RegexPatternItemStart, RegexOptions.Singleline))
                        {
                            stringBuilder.Append(line);
                            isItemCreating = true;
                            startPosition = logStreamReader.BufferPosition;
                            line = logStreamReader.ReadLine(true);
                        }
                        else if (Regex.IsMatch(line, block.RegexPatternItemContent, RegexOptions.Singleline))
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

                            foreach (var cell in cellsParser)
                            {
                                var streamCell = CreateStreamCell(logStreamReader, match, cell, ref position);
                                if (streamCell != null)
                                {
                                    cellNames.Add(cell.Name);
                                    item.Add(streamCell);
                                }
                            }
                            break;
                        }
                    }
                    position = startPosition;
                    CellNames = cellNames.ToArray();
                    Item = item.ToArray();
                }
                if (block.Items is TextContentParser.Items itemsParser)
                {
                    ItemsTemplate = block.Items.CellsTemplate.Select(t => t.Name).ToArray();
                    var items = new List<StreamCell[]>();
                    bool isItemCreating = false;
                    var stringBuilder = new StringBuilder();
                    var startPosition = position;

                    while (line != null || isItemCreating)
                    {
                        if (!isItemCreating && Regex.IsMatch(line, block.RegexPatternItemStart, RegexOptions.Singleline))
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

                            var item = new StreamCell[ItemsTemplate.Length];
                            for (int i = 0; i < item.Length; i++)
                            {
                                var template = block.Items.CellsTemplate[i];
                                var streamCell = CreateStreamCell(logStreamReader, match, template, ref position);
                                item[i] = streamCell;
                            }
                            items.Add(item);
                            position = startPosition;
                        }
                    }
                    Items = items.ToArray();
                }
            }
            private StreamCell CreateStreamCell(LogStreamReader logStreamReader, Match match, TextContentParser.Cell cell, ref long position)
            {
                var capture = match.Groups[cell.RegexGroupIndex];
                var startPosition = position + capture.Index;
                var streamCell = new StreamCell(logStreamReader, startPosition, capture.Length, StreamCellType.String);
                return streamCell;
            }
        }

        private readonly List<TextBlock> _blocks = new();

        public static TextContent Load(Stream stream, TextContentParser textContentParser)
        {
            if (stream == null || textContentParser == null)
            {
                return null;
            }
            var textContent = new TextContent(stream, textContentParser);
            return textContent;
        }
        private TextContent(Stream stream, TextContentParser textContentParser)
        {
            var logStreamReader = new LogStreamReader(stream);
            long position = 0;
            string line = logStreamReader.ReadLine(true);
            foreach (var block in textContentParser.Blocks)
            {
                var textBlock = new TextBlock(this, logStreamReader, block, ref line, ref position);
                _blocks.Add(textBlock);
            }
        }

        public StreamCell GetCell(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetCell(paths);
        }

        private StreamCell GetCell(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blocks.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            path = paths.Skip(1).FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var index = Array.IndexOf(block.CellNames, path);
            if (index < 0 || index >= block.CellNames.Length)
            {
                return null;
            }
            else
            {
                return block.Item[index];
            }
        }

        public StreamCell[] GetItem(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetItem(paths);
        }

        private StreamCell[] GetItem(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blocks.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            return block.Item.ToArray();
        }

        public string[] GetItemsTemplate(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetItemsTemplate(paths);
        }

        private string[] GetItemsTemplate(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blocks.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            return block.ItemsTemplate;
        }

        public StreamCell[][] GetItems(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetItems(paths);
        }

        private StreamCell[][] GetItems(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blocks.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            return block.Items;
        }

        //Regex regexHead = new Regex(@"app: (RoomsController)/([^\s]+)/(SHA)\(\)");
        //Regex regexOS = new Regex(@"os: iOS/([^\s]+)");
        //Regex regexItemTime = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}");
        //Regex regexLevel = new Regex(@" : ([^\s]+) : ");
        //Regex regexModule = new Regex(@"\[([^\s]+)\]");
        //Regex regexItemInfo = new Regex(@"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}) : ([^\s]+) : (\[.*?\]) (\[.*?\]) (.*)");
        //while (streamReader.ReadLine() is string line && line != null)
        //{
        //    var a = line;
        //    var m = regexHead.Match(line);
        //    var m1 = regexOS.Match(line);
        //    var m2 = regexItemInfo.Match(line);


        //    //int position = 0;
        //    //var m2 = regexItemTime.Matches(line);
        //    //var time = m2[0].Groups[0].Value;
        //    //position += m2[0].Index + m2[0].Length;
        //    //var m3 = regexLevel.Matches(line);
        //    //var level = m3[0].Groups[1].Value;
        //    //position += m3[0].Index + m3[0].Length;
        //    //var m4 = regexModule.Matches(line);
        //    //var m5 = regexModule.Matches(line);
        //}
        //private TextContent(TextContent rootObject, StreamReader streamReader, TextContentParser.Block objectParser, ref long position)
        //{
        //    _name = objectParser.Name;
        //    _rootObject = rootObject;
        //    if (objectParser.Properties != null)
        //    {
        //        foreach (var property in objectParser.Properties)
        //        {
        //            StringBuilder stringBuilder = new StringBuilder();
        //            bool isItemCreating = false;
        //            while (streamReader.ReadLine() is string line)
        //            {
        //                if (!isItemCreating && Regex.IsMatch(line, objectParser.RegexPatternItemStart))
        //                {
        //                    stringBuilder.AppendLine(line);
        //                    isItemCreating = true;
        //                }
        //                else if (Regex.IsMatch(line, objectParser.RegexPatternItemContent))
        //                {
        //                    stringBuilder.AppendLine(line);
        //                }
        //                else
        //                {
        //                    var item = stringBuilder.ToString();
        //                    var qwe = Regex.Match(item, objectParser.RegexPattern);
        //                    //var fileProperty = FileProperty.Load(streamReader, objectParser.RegexPattern, property, ref position);
        //                    //var textProperty = TextProperty.Load(textReader, properties, ref position);
        //                    //if (textProperty != null)
        //                    //{
        //                    //    _properties.Add(textProperty);
        //                    //}
        //                }
        //            }
        //            //var fileProperty = FileProperty.Load(streamReader, objectParser.RegexPattern, property, ref position);
        //            //var textProperty = TextProperty.Load(textReader, properties, ref position);
        //            //if (textProperty != null)
        //            //{
        //            //    _properties.Add(textProperty);
        //            //}
        //        }
        //    }


        //private FileObject(StreamReader streamReader)
        //{
        //    string delimiterChars = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        //    char[] breakChars = { '\r', '\n' };

        //    var a = "x_platform_application.cpp:157 \ttyuty";
        //    Dictionary<string, List<int>> wordMap = new Dictionary<string, List<int>>();
        //    StringBuilder stringBuilder = new StringBuilder();
        //    List<string> words = new List<string>();
        //    int index = 0;

        //    var asd = streamReader.ReadChars((int)(streamReader.BaseStream.Length / 2));
        //    foreach (var c in asd)
        //    {
        //        if (delimiterChars.Contains(c))
        //        {
        //            stringBuilder.Append(c);
        //        }
        //        else
        //        {
        //            if (stringBuilder.Length > 0)
        //            {
        //                var word = stringBuilder.ToString();
        //                if (wordMap.TryGetValue(word, out List<int> indexs))
        //                {
        //                    indexs.Add(index);
        //                }
        //                else
        //                {
        //                    wordMap.Add(word, new List<int>() { index });
        //                }
        //                index++;
        //                stringBuilder.Clear();
        //            }
        //        }
        //    }
        //    while (streamReader.BaseStream.Position != streamReader.BaseStream.Length - 1)
        //    {
        //        var c = streamReader.ReadByte();
        //        //if (delimiterChars.Contains(c))
        //        //{
        //        //    stringBuilder.Append(c);
        //        //}
        //        //else
        //        //{
        //        //    if (stringBuilder.Length > 0)
        //        //    {
        //        //        var word = stringBuilder.ToString();
        //        //        if (wordMap.TryGetValue(word, out List<int> indexs))
        //        //        {
        //        //            indexs.Add(index);
        //        //        }
        //        //        else
        //        //        {
        //        //            wordMap.Add(word, new List<int>() { index });
        //        //        }
        //        //        index++;
        //        //        stringBuilder.Clear();
        //        //    }
        //        //}
        //    }
        //}
    }
}
