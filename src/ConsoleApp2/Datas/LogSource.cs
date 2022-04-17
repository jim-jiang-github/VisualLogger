using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace ConsoleApp2.Datas
{
    public class LogSource : IDisposable
    {
        private readonly StreamCell[][] _content;
        private readonly Stream _stream;

        public string[] ColumnsName { get; }
        public int Count => _content.Length;
        public IEnumerable<StreamCell>? this[int index]
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
                return _content[index];
            }
        }
        public LogSource(Stream stream, string[] columnsName, StreamCell[][] content)
        {
            _stream = stream;
            _content = content;
            ColumnsName = columnsName;
        }
        public IEnumerable<StreamCell[]> GetItems(int startIndex, int length)
        {
            return _content.Skip(startIndex).Take(length);
        }
        public void Dispose()
        {
            _stream?.Dispose();
        }
    }
}
