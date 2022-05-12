using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public struct StreamCellPart
    {
        private readonly StreamCell _streamCell;
        private readonly int _start;
        private readonly int _end;

        public StreamCellPart(StreamCell streamCell, int start, int end)
        {
            _streamCell = streamCell;
            _start = start;
            _end = end;
        }

        public override string ToString()
        {
            return _streamCell.ToString(_start, _end);
        }
    }
}
