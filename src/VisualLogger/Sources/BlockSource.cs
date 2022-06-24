using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal struct BlockSource
    {
        private readonly LogSourceReader _reader;
        private readonly BlockCellSource[] _blockCells;

        public string Name { get; }
        public int Count => _blockCells.Length;

        public string this[int index]
        {
            get
            {
                return _reader.GetValue(_blockCells[index].Cell, index);
            }
        }

        public string GetCellName(int index)
        {
            return _blockCells[index].Name;
        }

        internal BlockSource(LogSourceReader reader, string name, BlockCellSource[] blockCells)
        {
            Name = name;
            _reader = reader;
            _blockCells = blockCells;
        }
    }
}
