using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Streams
{
    public struct BytesLine
    {
        public long Position { get; }
        public int Length { get; }
        //public byte[] Data { get; }

        public BytesLine(long position, int length)
        {
            Position = position;
            Length = length;
            //Data = data;
        }
    }
}
