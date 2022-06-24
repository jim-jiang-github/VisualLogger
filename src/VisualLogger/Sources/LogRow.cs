using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    public struct LogRow
    {
        private readonly LogSourceReader _reader;
        private readonly CellSource[] _cells;

        public int Index { get; }

        public object? this[int index]
        {
            get
            {
                return _reader.GetValue(_cells[index], index);
            }
        }

        internal LogRow(LogSourceReader reader, int index, CellSource[] cells)
        {
            _reader = reader;
            Index = index;
            _cells = cells;
        }
    }
}
