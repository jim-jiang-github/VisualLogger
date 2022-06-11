using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Sources
{
    public interface ILogSource
    {
        long TotalRowsCount { get; }
        StreamCell? GetCell(string recursivePath);
        string[] GetColumnHead();
        IEnumerable<StreamCell[]> GetRows();
    }
}
