using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;

namespace VisualLogger.Sources
{
    internal struct BlockSource
    {
        private readonly BlockCellSource[] _blockCells;

        public string Name { get; }
        public int Count => _blockCells.Length;

        public object? this[int index]
        {
            get
            {
                return _blockCells[index].Cell;
            }
        }

        public string GetCellName(int index)
        {
            return _blockCells[index].Name;
        }

        internal BlockSource(string name, BlockCellSource[] blockCells)
        {
            Name = name;
            _blockCells = blockCells;
        }
    }
}
