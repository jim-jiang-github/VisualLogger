using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public class LogContent
    {
        private readonly StreamCell[][] _content;

        public string[] ColumnsName { get; }
        public int Count => _content.Length;

        public IEnumerable<StreamCell> this[int index]
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
        public LogContent(string[] columnsName, StreamCell[][] content)
        {
            _content = content;
            ColumnsName = columnsName;
        }

        public IEnumerable<StreamCell[]> GetItems(int startIndex,int length)
        {
            return _content.Skip(startIndex).Take(length);
        }
    }
}
