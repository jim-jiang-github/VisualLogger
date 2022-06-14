using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Sources
{
    public interface ILogSource : IDisposable
    {
        long TotalRowsCount { get; }
        IEnumerable<string> ColumnHeads { get; }
        IEnumerable<string> EnumerateWords { get; }
        StreamCell? GetCell(string recursivePath);
        IEnumerable<StreamCell[]> GetRows();
    }
}
