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
        int TotalRowsCount { get; }
        string[] ColumnNames { get; }
        IEnumerable<string> EnumerateWords { get; }
        StreamCell? GetCell(string recursivePath);
        IEnumerable<StreamCell[]> GetRows();
    }
}
