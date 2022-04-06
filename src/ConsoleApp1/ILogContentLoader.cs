using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static ConsoleApp1.LogContent;

namespace ConsoleApp1
{
    public interface ILogContentLoader
    {
        LogContent LoadLogContent(string logPath);
    }

    public class LogContent : IEnumerable<LogItem>
    {
        public class StreamDataBlock
        {
            private BinaryReader _source;

            private long _position;
            private int _length;
            private Type _type;

            public StreamDataBlock(BinaryReader source, long position, int length, Type type)
            {
                _source = source;
                _position = position;
                _length = length;
                _type = type;
            }

            public object GetData()
            {
                if (_source.BaseStream.Position != _position)
                {
                    _source.BaseStream.Position = _position;
                }
                switch (_type)
                {
                    case var x when x == typeof(bool):
                        return _source.ReadBoolean();
                    case var x when x == typeof(byte):
                        return _source.ReadByte();
                    case var x when x == typeof(char):
                        return _source.ReadChar();
                    case var x when x == typeof(decimal):
                        return _source.ReadDecimal();
                    case var x when x == typeof(double):
                        return _source.ReadDouble();
                    case var x when x == typeof(float):
                        return _source.ReadSingle();
                    case var x when x == typeof(int):
                        return _source.ReadInt32();
                    case var x when x == typeof(long):
                        return _source.ReadInt64();
                    case var x when x == typeof(short):
                        return _source.ReadInt16();
                    case var x when x == typeof(string):
                        return Encoding.UTF8.GetString(_source.ReadBytes(_length)).Trim();
                    case var x when x == typeof(uint):
                        return _source.ReadUInt32();
                    case var x when x == typeof(ulong):
                        return _source.ReadUInt64();
                    case var x when x == typeof(ushort):
                        return _source.ReadUInt16();
                    default:
                        return null;
                }
            }

            public object PopData()
            {
                var lastPosition = _source.BaseStream.Position;
                try
                {
                    return GetData();
                }
                finally
                {
                    _source.BaseStream.Position = lastPosition;
                }
            }

            public override string ToString()
            {
                return GetData().ToString();
            }
        }
        public class LogItem
        {
            public StreamDataBlock[] Datas { get; }

            public LogItem(StreamDataBlock[] datas)
            {
                Datas = datas;
            }
        }

        private LogItem[] _logItems;

        public string[] ColumnsName { get; }
        public int Count => _logItems.Length;

        public LogItem this[int index]
        {
            get
            {
                if (index < 0)
                {
                    return null;
                }
                if (index >= Count)
                {
                    return null;
                }
                return _logItems[index];
            }
        }
        public LogContent(string[] columnsName, LogItem[] logItems)
        {
            _logItems = logItems;
            ColumnsName = columnsName;
        }

        public IEnumerator<LogItem> GetEnumerator()
        {
            foreach (var item in _logItems)
            {
                yield return item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
