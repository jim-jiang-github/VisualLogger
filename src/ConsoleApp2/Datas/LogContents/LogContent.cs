using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas.LogContents
{
    public class LogContent : IEnumerable<LogItem>
    {

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
