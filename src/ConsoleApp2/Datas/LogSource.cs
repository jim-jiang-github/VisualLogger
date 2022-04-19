using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.Datas
{
    public class LogSource : LifeCycleTracker<LogSource>, IDisposable
    {
        private readonly IEnumerable<StreamCell[]> _content;
        private readonly Stream _stream;

        public LogColumn[] Columns { get; }
        public int Count => _content.Count();
        public LogSource(Stream stream, string[] columns, IEnumerable<StreamCell[]> content)
        {
            _stream = stream;
            _content = content;
            Columns = columns.Select(c => new LogColumn(c, new WordRetriever())).ToArray();

            int itemIndex = 0;
            foreach (var item in content)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    var column = Columns[i];
                    if (column.WordRetriever == null)
                    {
                        continue;
                    }
                    column.WordRetriever.AppendString(item[i].ToString(), itemIndex);
                }
                itemIndex++;
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
