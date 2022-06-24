using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;

namespace VisualLogger.Sources.Binary
{
    internal class LogSourceReaderBinary : LogSourceReader
    {
        private readonly BinaryReader _binaryReader;
        private readonly Encoding _encoding;
        public LogSourceReaderBinary(Stream stream, Encoding encoding, CellConvertor?[] cellConvertors) : base(cellConvertors)
        {
            _binaryReader = new BinaryReader(stream, encoding);
            _encoding = encoding;
        }

        protected override object GetRawValue(CellSource logCell, int index)
        {
            var lastPosition = _binaryReader.BaseStream.Position;
            if (lastPosition != logCell.Position)
            {
                _binaryReader.BaseStream.Seek(logCell.Position, SeekOrigin.Begin);
            }
            try
            {
                var type = (LogCellBinaryType)logCell.Data;
                object value = type switch
                {
                    LogCellBinaryType.Boolean => _binaryReader.ReadBoolean(),
                    LogCellBinaryType.Byte => _binaryReader.ReadByte(),
                    LogCellBinaryType.Char => _binaryReader.ReadChar(),
                    LogCellBinaryType.Float => _binaryReader.ReadSingle(),
                    LogCellBinaryType.Double => _binaryReader.ReadDouble(),
                    LogCellBinaryType.Decimal => _binaryReader.ReadDecimal(),
                    LogCellBinaryType.UShort => _binaryReader.ReadUInt16(),
                    LogCellBinaryType.Short => _binaryReader.ReadInt16(),
                    LogCellBinaryType.UInt => _binaryReader.ReadUInt32(),
                    LogCellBinaryType.Int => _binaryReader.ReadInt32(),
                    LogCellBinaryType.ULong => _binaryReader.ReadUInt64(),
                    LogCellBinaryType.Long => _binaryReader.ReadInt64(),
                    _ => _encoding.GetString(_binaryReader.ReadBytes(logCell.Data)),
                };
                return value;
            }
            finally
            {
                if (logCell.Position != lastPosition)
                {
                    _binaryReader.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }
        }
        public override void Dispose()
        {
            _binaryReader?.Close();
        }
    }
}
