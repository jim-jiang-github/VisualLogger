using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    public interface ILogSource : IDisposable
    {
        int TotalRowsCount { get; }
        string[] ColumnNames { get; }
        IEnumerable<string> EnumerateWords { get; }
        LogFilter Filter { get; }
        IEnumerable<LogRow> GetRows(int start, int length);
    }
}
