using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal struct BlockCellSource
    {
        public string Name { get; set; }
        public object Cell { get; set; }
        internal BlockCellSource(string name, object cell)
        {
            Name = name;
            Cell = cell;
        }
    }
}
