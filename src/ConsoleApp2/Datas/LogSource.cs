using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.Datas
{
    public class LogSource : LifeCycleable<LogSource>, IDisposable
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
            string delimiterChars = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
            char[] breakChars = { '\r', '\n' };

            Dictionary<string, List<int>> wordMap = new Dictionary<string, List<int>>();
            StringBuilder stringBuilder = new StringBuilder();
            List<string> words = new List<string>();
            int index = 0;

            foreach (var item in content)
            {
                var str = item[5].ToString();
                foreach (var c in str)
                {
                    if (delimiterChars.Contains(c))
                    {
                        stringBuilder.Append(c);
                    }
                    else
                    {
                        if (stringBuilder.Length > 0)
                        {
                            var word = stringBuilder.ToString();
                            if (wordMap.TryGetValue(word, out List<int>? indexs))
                            {
                                indexs.Add(index);
                            }
                            else
                            {
                                wordMap.Add(word, new List<int>() { index });
                            }
                            index++;
                            stringBuilder.Clear();
                        }
                    }
                }
            }
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
