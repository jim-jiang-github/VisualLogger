using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    internal class LogSourceBinary : LogSource<SchemaLogBinary.SchemaBlockBinary, SchemaLogBinary.SchemaColumnHeadBinary, SchemaLogBinary.SchemaCellBinary>
    {
        private LogSourceBinary(Stream stream, SchemaLog<SchemaLogBinary.SchemaBlockBinary, SchemaLogBinary.SchemaColumnHeadBinary, SchemaLogBinary.SchemaCellBinary> schemaLog) : base(stream, schemaLog)
        {
        }
        protected override BlockSource CreateBlockSource(
           MixStreamReader mixStreamReader,
           ILogSource logSource,
           SchemaLogBinary.SchemaBlockBinary block,
           CellConvertorProvider cellConvertorProvider,
           ref long streamPosition)
        {
            var blockCells = new CellSource[block.Cells.Count(c => c.Type != SchemaLogBinaryType.Skip)];
            int cellIndex = 0;
            foreach (var cell in block.Cells)
            {
                if (cell.Type == SchemaLogBinaryType.Skip)
                {
                    if (cell.Length is int count)
                    {
                        streamPosition += count;
                    }
                    continue;
                }
                var streamCell = CreateStreamCell(mixStreamReader, logSource, cellIndex, cell, cellConvertorProvider, ref streamPosition);
                blockCells[cellIndex] = new CellSource(cell.Name, streamCell);
                cellIndex++;
            }
            return new BlockSource(block.Name, blockCells);
        }

        protected override ColumnHeadSource CreateContentSource(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            SchemaLogBinary.SchemaColumnHeadBinary columnHead,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition)
        {
            if (columnHead.Count == null)
            {
                Log.Fatal("body.Count can not be null here.");
                throw new ArgumentException("body.Count can not be null here.");
            }
            int itemCount = 0;
            var countParser = columnHead.Count;
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
            TotalRowsCount = itemCount;
            var sourceItems = new StreamCell[itemCount][];
            for (int i = 0; i < itemCount; i++)
            {
                StreamCell[] streamCells = new StreamCell[columnHead.Columns.Length];
                for (int j = 0; j < columnHead.Columns.Length; j++)
                {
                    var column = columnHead.Columns[j];
                    var streamCell = CreateStreamCell(mixStreamReader, logSource, i, column.Cell, cellConvertorProvider, ref streamPosition);
                    streamCells[j] = streamCell;
                    HandleContentCell(column, streamCell);
                }
                sourceItems[i] = streamCells;
            }
            return new ColumnHeadSource(columnHead.Columns.Select(t => t.Cell.Name).ToArray(), sourceItems);
        }

        private StreamCell CreateStreamCell(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            int index,
            SchemaLogBinary.SchemaCellBinary cell,
            CellConvertorProvider cellConvertorProvider,
            ref long position)
        {
            var type = cell.Type;
            var length = cell.Length;
            switch (type)
            {
                case SchemaLogBinaryType.Boolean:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Boolean, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Byte:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Byte, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Char:
                    position += 1;
                    return new StreamCell(mixStreamReader, index, position - 1, 1, StreamCellType.Char, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Decimal:
                    position += 16;
                    return new StreamCell(mixStreamReader, index, position - 16, 16, StreamCellType.Decimal, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Double:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.Double, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Float:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.Float, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Int:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.Int, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Long:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.Long, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.Short:
                    position += 2;
                    return new StreamCell(mixStreamReader, index, position - 2, 2, StreamCellType.Short, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.StringWithLength:
                    if (length is int stringLength)
                    {
                        position += stringLength;
                        return new StreamCell(mixStreamReader, index, position - stringLength, stringLength, StreamCellType.String, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                    }
                    throw new ArgumentException("SchemaLogBinaryType.StringWithLength can not read as int.");
                case SchemaLogBinaryType.StringWithIntHead:
                    mixStreamReader.BaseStream.Position = position;
                    var stringHeadLength = mixStreamReader.ReadInt32();
                    position += 4 + stringHeadLength;
                    return new StreamCell(mixStreamReader, index, position - stringHeadLength, stringHeadLength, StreamCellType.String, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.UInt:
                    position += 4;
                    return new StreamCell(mixStreamReader, index, position - 4, 4, StreamCellType.UInt, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.ULong:
                    position += 8;
                    return new StreamCell(mixStreamReader, index, position - 8, 8, StreamCellType.ULong, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                case SchemaLogBinaryType.UShort:
                    position += 2;
                    return new StreamCell(mixStreamReader, index, position - 2, 2, StreamCellType.UShort, cellConvertorProvider.GetConvertor(cell.ConvertorName));
                default:
                    throw new ArgumentException("Not define SchemaLogBinaryType.");
            }
        }
    }
}
