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
        public CellSource Cell { get; set; }
        internal BlockCellSource(string name, CellSource cell)
        {
            Name = name;
            Cell = cell;
        }
    }
}
