using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.Datas
{
    public class LogContent : LifeCycleTracker<LogContent>, IDisposable
    {
        private readonly IEnumerable<StreamCell[]> _content;
        private readonly Stream _stream;

        public LogColumn[] Columns { get; }
        public int Count => _content.Count();
        public LogContent(Stream stream, string[] columns, IEnumerable<StreamCell[]> content)
        {
            _stream = stream;
            _content = content;
            Columns = columns.Select((c, i) => new LogColumn(c, i == 5 ? new TextSplitter() : null)).ToArray();

            foreach (var item in content)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    var column = Columns[i];
                    if (column.Splitter == null)
                    {
                        continue;
                    }
                    column.Splitter.AppendStreamCell(item[i]);
                }
            }
        }
        public void AddIncludeKey()
        {

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
