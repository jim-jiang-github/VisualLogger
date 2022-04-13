using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public class StreamCell
    {
        private readonly LogStreamReader _source;
        private readonly long _position;
        private readonly int _length;
        private readonly StreamCellType _type;

        public StreamCell(LogStreamReader source, long position, int length, StreamCellType type)
        {
            _source = source;
            _position = position;
            _length = length;
            _type = type;
        }

        public object GetValue()
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
                    StreamCellType.Boolean => _source.ReadBoolean(),
                    StreamCellType.Byte => _source.ReadByte(),
                    StreamCellType.Char => _source.ReadChar(),
                    StreamCellType.Decimal => _source.ReadDecimal(),
                    StreamCellType.Double => _source.ReadDouble(),
                    StreamCellType.Float => _source.ReadSingle(),
                    StreamCellType.Int => _source.ReadInt32(),
                    StreamCellType.Long => _source.ReadInt64(),
                    StreamCellType.Short => _source.ReadInt16(),
                    StreamCellType.String => Encoding.UTF8.GetString(_source.ReadBytes(_length)),
                    StreamCellType.UInt => _source.ReadUInt32(),
                    StreamCellType.ULong => _source.ReadUInt64(),
                    StreamCellType.UShort => _source.ReadUInt16(),
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
            return GetValue().ToString();
        }
    }
}
