using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal struct CellSource
    {
        public long Position { get;  }
        public int Data { get;  }
        internal CellSource(long position, int data)
        {
            Position = position;
            Data = data;
        }

        public static implicit operator CellSource(ValueTuple<long, int> byteLine)
        {
            return new CellSource(byteLine.Item1, byteLine.Item2);
        }
    }
}
