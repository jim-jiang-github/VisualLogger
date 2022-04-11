using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryContent
    {
        private class BinaryBlock
        {
            private readonly BinaryContent _binaryContent;

            public string Name { get; }
            public string[] CellNames { get; } = Array.Empty<string>();
            public StreamCell[] Item { get; } = Array.Empty<StreamCell>();
            public string[] ItemsTemplate { get; } = Array.Empty<string>();
            public StreamCell[][] Items { get; } = Array.Empty<StreamCell[]>();

            public BinaryBlock(BinaryContent binaryContent, BinaryReader binaryReader, BinaryContentParser.Block block, ref long position)
            {
                _binaryContent = binaryContent;
                Name = block.Name;
                if (block.Cells is BinaryContentParser.Cell[] cells)
                {
                    var cellNames = new List<string>();
                    var item = new List<StreamCell>();
                    foreach (var cell in cells)
                    {
                        var streamCell = CreateStreamCell(binaryReader, cell, ref position);
                        if (streamCell != null)
                        {
                            cellNames.Add(cell.Name);
                            item.Add(streamCell);
                        }
                    }
                    CellNames = cellNames.ToArray();
                    Item = item.ToArray();
                }
                if (block.Items is BinaryContentParser.Items items)
                {
                    ItemsTemplate = block.Items.CellsTemplate.Select(t => t.Name).ToArray();
                    int itemCount = 0;
                    var countParser = items.Count;
                    if (int.TryParse(countParser, out int count))
                    {
                        itemCount = count;
                    }
                    else
                    {
                        var streamCell = binaryContent.GetCell(countParser);
                        if (streamCell.GetValue() is int countFromPath)
                        {
                            itemCount = countFromPath;
                        }
                    }
                    Items = new StreamCell[itemCount][];
                    for (int i = 0; i < itemCount; i++)
                    {
                        StreamCell[] streamCells = new StreamCell[ItemsTemplate.Length];
                        for (int j = 0; j < ItemsTemplate.Length; j++)
                        {
                            var template = block.Items.CellsTemplate[j];
                            var streamCell = CreateStreamCell(binaryReader, template, ref position);
                            streamCells[j] = streamCell;
                        }
                        Items[i] = streamCells;
                    }
                }
            }
            private StreamCell CreateStreamCell(BinaryReader binaryReader, BinaryContentParser.Cell cell, ref long position)
            {
                var type = cell.Type;
                var length = cell.Length;
                switch (type)
                {
                    case BinaryType.Skip:
                        if (length is int count)
                        {
                            position += count;
                        }
                        return null;
                    case BinaryType.Boolean:
                        position += 1;
                        return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Boolean);
                    case BinaryType.Byte:
                        position += 1;
                        return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Byte);
                    case BinaryType.Char:
                        position += 1;
                        return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Char);
                    case BinaryType.Decimal:
                        position += 16;
                        return new StreamCell(binaryReader, position - 16, 16, StreamCellType.Decimal);
                    case BinaryType.Double:
                        position += 8;
                        return new StreamCell(binaryReader, position - 8, 8, StreamCellType.Double);
                    case BinaryType.Float:
                        position += 4;
                        return new StreamCell(binaryReader, position - 4, 4, StreamCellType.Float);
                    case BinaryType.Int:
                        position += 4;
                        return new StreamCell(binaryReader, position - 4, 4, StreamCellType.Int);
                    case BinaryType.Long:
                        position += 8;
                        return new StreamCell(binaryReader, position - 8, 8, StreamCellType.Long);
                    case BinaryType.Short:
                        position += 2;
                        return new StreamCell(binaryReader, position - 2, 2, StreamCellType.Short);
                    case BinaryType.StringWithLength:
                        if (length is int stringLength)
                        {
                            position += stringLength;
                            return new StreamCell(binaryReader, position - stringLength, stringLength, StreamCellType.String);
                        }
                        else
                        {
                            return null;
                        }
                    case BinaryType.StringWithIntHead:
                        binaryReader.BaseStream.Position = position;
                        var stringHeadLength = binaryReader.ReadInt32();
                        position += 4 + stringHeadLength;
                        return new StreamCell(binaryReader, position - stringHeadLength, stringHeadLength, StreamCellType.String);
                    case BinaryType.UInt:
                        position += 4;
                        return new StreamCell(binaryReader, position - 4, 4, StreamCellType.UInt);
                    case BinaryType.ULong:
                        position += 8;
                        return new StreamCell(binaryReader, position - 8, 8, StreamCellType.ULong);
                    case BinaryType.UShort:
                        position += 2;
                        return new StreamCell(binaryReader, position - 2, 2, StreamCellType.UShort);
                    default:
                        Debug.Assert(false, "Can not match any type.");
                        return null;
                }
            }
        }

        private readonly List<BinaryBlock> _blocks = new();

        public static BinaryContent Load(Stream stream, BinaryContentParser binaryContentParser)
        {
            if (stream == null || binaryContentParser == null)
            {
                return null;
            }
            var binaryContent = new BinaryContent(stream, binaryContentParser);
            return binaryContent;
        }
        private BinaryContent(Stream stream, BinaryContentParser binaryContentParser)
        {
            var binaryReader = new BinaryReader(stream);
            long position = 0;
            foreach (var block in binaryContentParser.Blocks)
            {
                var binaryBlock = new BinaryBlock(this, binaryReader, block, ref position);
                _blocks.Add(binaryBlock);
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
    }
}
