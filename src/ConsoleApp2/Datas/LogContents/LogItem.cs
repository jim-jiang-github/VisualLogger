using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas.LogContents
{
    public class LogItem
    {
        public LogCell[] Cells { get; }

        public LogItem(LogCell[] cells)
        {
            Cells = cells;
        }
    }
}
