using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Datas.LogSources
{
    public class LogSourceBinary : LogSource<LogSchemaBinary, LogSchemaBinary.BlockBinary, LogSchemaBinary.BodyBinary, LogSchemaBinary.CellBinary>
    {
        public LogSourceBinary(Stream stream, LogSchema<LogSchemaBinary, LogSchemaBinary.BlockBinary, LogSchemaBinary.BodyBinary, LogSchemaBinary.CellBinary> logSchema) : base(stream, logSchema)
        {
        }
        protected override BlockSource CreateBlockSource(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            LogSchemaBinary.BlockBinary block,
            ref long streamPosition)
        {
            var blockCells = new BlockCell[block.Cells.Count(c => c.Type != LogSchemaBinaryType.Skip)];
            int cellIndex = 0;
            foreach (var cell in block.Cells)
            {
                if (cell.Type == LogSchemaBinaryType.Skip)
                {
                    if (cell.Length is int count)
                    {
                        streamPosition += count;
                    }
                    continue;
                }

                var streamCell = CreateStreamCell(logSource, mixStreamReader, cellIndex, cell, ref streamPosition);
                blockCells[cellIndex] = new BlockCell(cell.Name, streamCell);
                cellIndex++;
            }
            return new BlockSource(block.Name, blockCells);
        }

        protected override BodySource CreateBodySource(ILogSource logSource, MixStreamReader mixStreamReader, LogSchemaBinary.BodyBinary body, ref long streamPosition)
        {
            if (body.Count == null)
            {
                throw new ArgumentException("body.Count can not be null here.");
            }
            int itemCount = 0;
            var countParser = body.Count;
            if (int.TryParse(countParser, out int count))
            {
                itemCount = count;
            }
            else
            {
                var streamCell = logSource.GetCell(countParser);
                if (streamCell?.GetValue() is int countFromPath)
                {
                    itemCount = countFromPath;
                }
            }
            var sourceItems = new StreamCell[itemCount][];
            for (int i = 0; i < itemCount; i++)
            {
                StreamCell[] streamCells = new StreamCell[body.BodyTemplate.Length];
                for (int j = 0; j < body.BodyTemplate.Length; j++)
                {
                    var template = body.BodyTemplate[j];
                    var streamCell = CreateStreamCell(logSource, mixStreamReader, i, template, ref streamPosition);
                    streamCells[j] = streamCell;
                }
                sourceItems[i] = streamCells;
            }
            return new BodySource(body.BodyTemplate.Select(t => t.Name).ToArray(), sourceItems);
        }

        private StreamCell CreateStreamCell(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            int index,
            LogSchemaBinary.CellBinary cell,
            ref long position)
        {
            var type = cell.Type;
            var length = cell.Length;
            switch (type)
            {
                case LogSchemaBinaryType.Boolean:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Boolean, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Byte:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Byte, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Char:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Char, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Decimal:
                    position += 16;
                    return new StreamCell(mixStreamReader, index, position - 16, 16, StreamCellType.Decimal, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Double:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.Double, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Float:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.Float, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Int:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.Int, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Long:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.Long, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.Short:
                    position += 2;
                    return new StreamCell(mixStreamReader, index, position - 2, 2, StreamCellType.Short, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.StringWithLength:
                    if (length is int stringLength)
                    {
                        position += stringLength;
                        return new StreamCell(mixStreamReader, index, position - stringLength, stringLength, StreamCellType.String, logSource.GetConvertor(cell.ConvertorName));
                    }
                    throw new ArgumentException("LogSchemaBinaryType.StringWithLength can not read as int.");
                case LogSchemaBinaryType.StringWithIntHead:
                    mixStreamReader.BaseStream.Position = position;
                    var stringHeadLength = mixStreamReader.ReadInt32();
                    position += 4 + stringHeadLength;
                    return new StreamCell(mixStreamReader, index, position - stringHeadLength, stringHeadLength, StreamCellType.String, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.UInt:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.UInt, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.ULong:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.ULong, logSource.GetConvertor(cell.ConvertorName));
                case LogSchemaBinaryType.UShort:
                    position += 2;
                    return new StreamCell(mixStreamReader, index, position - 2, 2, StreamCellType.UShort, logSource.GetConvertor(cell.ConvertorName));
                default:
                    throw new ArgumentException("Not define LogSchemaBinaryType.");
            }
        }
    }
}
