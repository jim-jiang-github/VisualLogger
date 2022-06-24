using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal struct RowSource
    {
        public int Index { get; }
        public IEnumerable<CellSource> Cells { get; }

        internal RowSource(int index, IEnumerable<CellSource> cells)
        {
            Index = index;
            Cells = cells;
        }
    }
}
