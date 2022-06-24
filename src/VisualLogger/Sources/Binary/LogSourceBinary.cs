using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
using VisualLogger.Sources.Text;
using VisualLogger.Streams;

namespace VisualLogger.Sources.Binary
{
    internal class LogSourceBinary : LogSource<SchemaLogBinary.SchemaBlockBinary, SchemaLogBinary.SchemaColumnHeadBinary, SchemaLogBinary.SchemaCellBinary>
    {
        private Encoding _encoding;
        private BinaryReader _binaryReader;

#nullable disable
        private LogSourceBinary(Stream stream, SchemaLog<SchemaLogBinary.SchemaBlockBinary, SchemaLogBinary.SchemaColumnHeadBinary, SchemaLogBinary.SchemaCellBinary> schemaLog) : base(stream, schemaLog)
#nullable enable
        {
        }

        protected override void Init(Stream stream, SchemaLog<SchemaLogBinary.SchemaBlockBinary, SchemaLogBinary.SchemaColumnHeadBinary, SchemaLogBinary.SchemaCellBinary> schemaLog)
        {
            _encoding = Encoding.GetEncoding(schemaLog.EncodingName);
            _binaryReader = new BinaryReader(stream, _encoding);
        }

        protected override BlockSource CreateBlockSource(
           SchemaLogBinary.SchemaBlockBinary block,
           ref long streamPosition)
        {
            var blockCells = new BlockCellSource[block.Cells.Count(c => c.Type != SchemaLogBinaryType.Skip)];
            var cellConvertors = new CellConvertor?[blockCells.Length];
            int cellIndex = 0;
            foreach (var blockCell in block.Cells)
            {
                if (blockCell.Type == SchemaLogBinaryType.Skip)
                {
                    if (blockCell.Length is int count)
                    {
                        streamPosition += count;
                    }
                    continue;
                }
                var cell = CreateCell(blockCell, ref streamPosition);
                blockCells[cellIndex] = new BlockCellSource(blockCell.Name, cell);
                cellConvertors[cellIndex] = _convertorProvider.GetConvertor(blockCell.ConvertorName);
                cellIndex++;
            }
            var logSourceReaderBinary = new LogSourceReaderBinary(_stream, _encoding, cellConvertors);
            return new BlockSource(logSourceReaderBinary, block.Name, blockCells);
        }

        protected override ContentSource CreateContentSource(
            IBlockCellFinder blockCellSearchable,
            ref long streamPosition)
        {
            var columnHeadTemplate = _schemaLog.ColumnHeadTemplate;
            int rowCount = 0;
            var rowCountParser = columnHeadTemplate.RowCount;
            if (int.TryParse(rowCountParser, out int count))
            {
                rowCount = count;
            }
            else
            {
                var cellValue = blockCellSearchable.GetBlockCellValue(rowCountParser);
                if (int.TryParse(cellValue, out int rowCountFromPath))
                {
                    rowCount = rowCountFromPath;
                }
            }
            var cellConvertors = new CellConvertor?[columnHeadTemplate.Columns.Length];
            for (int i = 0; i < columnHeadTemplate.Columns.Length; i++)
            {
                cellConvertors[i] = _convertorProvider.GetConvertor(columnHeadTemplate.Columns[i].Cell.ConvertorName);
            }
            var logSourceReaderBinary = new LogSourceReaderBinary(_stream, _encoding, cellConvertors);
            TotalRowsCount = rowCount;
            var rows = new RowSource[rowCount];
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                CellSource[] streamCells = new CellSource[columnHeadTemplate.Columns.Length];
                for (int cellIndex = 0; cellIndex < columnHeadTemplate.Columns.Length; cellIndex++)
                {
                    var column = columnHeadTemplate.Columns[cellIndex];
                    var cell = CreateCell(column.Cell, ref streamPosition);
                    streamCells[cellIndex] = cell;
                    HandleContentCellValue(column, logSourceReaderBinary, cell, cellIndex);
                }
                rows[rowIndex] = new RowSource(rowIndex, streamCells);
            }
            return new ContentSource(logSourceReaderBinary, columnHeadTemplate.Columns.Select(t => t.Cell.Name).ToArray(), rows);
        }

        private CellSource CreateCell(
            SchemaLogBinary.SchemaCellBinary cell,
            ref long position)
        {
            var type = cell.Type;
            var length = cell.Length;
            switch (type)
            {
                case SchemaLogBinaryType.Boolean:
                    position += 1;
                    return new CellSource(position - 1, (int)LogCellBinaryType.Boolean);
                case SchemaLogBinaryType.Byte:
                    position += 1;
                    return new CellSource(position - 1, (int)LogCellBinaryType.Byte);
                case SchemaLogBinaryType.Char:
                    position += 1;
                    return new CellSource(position - 1, (int)LogCellBinaryType.Char);
                case SchemaLogBinaryType.Decimal:
                    position += 16;
                    return new CellSource(position - 16, (int)LogCellBinaryType.Decimal);
                case SchemaLogBinaryType.Double:
                    position += 8;
                    return new CellSource(position - 8, (int)LogCellBinaryType.Double);
                case SchemaLogBinaryType.Float:
                    position += 4;
                    return new CellSource(position - 4, (int)LogCellBinaryType.Float);
                case SchemaLogBinaryType.Int:
                    position += 4;
                    return new CellSource(position - 4, (int)LogCellBinaryType.Int);
                case SchemaLogBinaryType.Long:
                    position += 8;
                    return new CellSource(position - 8, (int)LogCellBinaryType.Long);
                case SchemaLogBinaryType.Short:
                    position += 2;
                    return new CellSource(position - 2, (int)LogCellBinaryType.Short);
                case SchemaLogBinaryType.StringWithLength:
                    if (length is int stringLength)
                    {
                        position += stringLength;
                        return new CellSource(position - stringLength, stringLength);
                    }
                    throw new ArgumentException("LogCellBinaryType.StringWithLength can not read as int.");
                case SchemaLogBinaryType.StringWithIntHead:
                    _binaryReader.BaseStream.Position = position;
                    var stringHeadLength = _binaryReader.ReadInt32();
                    position += 4 + stringHeadLength;
                    return new CellSource(position - stringHeadLength, stringHeadLength);
                case SchemaLogBinaryType.UInt:
                    position += 4;
                    return new CellSource(position - 4, (int)LogCellBinaryType.UInt);
                case SchemaLogBinaryType.ULong:
                    position += 8;
                    return new CellSource(position - 8, (int)LogCellBinaryType.ULong);
                case SchemaLogBinaryType.UShort:
                    position += 2;
                    return new CellSource(position - 2, (int)LogCellBinaryType.UShort);
                default:
                    throw new ArgumentException("Not define LogCellBinaryType.");
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _binaryReader.Close();
        }
    }
}
