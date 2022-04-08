using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas.LogContents
{
    public class LogCell
    {
        private readonly BinaryReader _source;

        private readonly long _position;
        private readonly int _length;
        private readonly LogCellType _type;

        public LogCell(BinaryReader source, long position, int length, LogCellType type)
        {
            _source = source;
            _position = position;
            _length = length;
            _type = type;
        }

        public object GetLogCellValue()
        {
            var lastPosition = _source.BaseStream.Position;
            if (lastPosition != _position)
            {
                _source.BaseStream.Seek(_position, SeekOrigin.Begin);
            }
            try
            {
                return _type switch
                {
                    LogCellType.Boolean => _source.ReadBoolean(),
                    LogCellType.Byte => _source.ReadByte(),
                    LogCellType.Char => _source.ReadChar(),
                    LogCellType.Decimal => _source.ReadDecimal(),
                    LogCellType.Double => _source.ReadDouble(),
                    LogCellType.Float => _source.ReadSingle(),
                    LogCellType.Int => _source.ReadInt32(),
                    LogCellType.Long => _source.ReadInt64(),
                    LogCellType.Short => _source.ReadInt16(),
                    LogCellType.String => Encoding.UTF8.GetString(_source.ReadBytes(_length)),
                    LogCellType.UInt => _source.ReadUInt32(),
                    LogCellType.ULong => _source.ReadUInt64(),
                    LogCellType.UShort => _source.ReadUInt16(),
                    _ => null,
                };
            }
            finally
            {
                if (_position != lastPosition)
                {
                    _source.BaseStream.Seek(lastPosition, SeekOrigin.Begin);
                }
            }
        }

        public override string ToString()
        {
            return GetLogCellValue().ToString();
        }
    }
}
