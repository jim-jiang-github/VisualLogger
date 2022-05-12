using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public struct StreamCell
    {
        private readonly MixStreamReader _source;
        private readonly int _index;
        private readonly long _position;
        private readonly int _length;
        private readonly StreamCellType _type;
        private readonly StreamCellConvertor? _convertor;

        public int Index => _index;

        public StreamCell(MixStreamReader source, int index, long position, int length, StreamCellType type, StreamCellConvertor? convertor)
        {
            Program.Count++;
            _source = source;
            _index = index;
            _position = position;
            _length = length;
            _type = type;
            _convertor = convertor;
        }

        public object? GetValue()
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
            if (_convertor == null)
            {
                return GetValue()?.ToString() ?? string.Empty;
            }
            else
            {
                return _convertor.Convert(GetValue())?.ToString() ?? string.Empty;
            }
        }

        public string ToString(int start, int end)
        {
            var text = ToString();
            return text.AsSpan().Slice(start, end).ToString();
        }
    }
}
