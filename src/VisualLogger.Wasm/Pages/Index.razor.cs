using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualLogger.Wasm.Pages
{
    public sealed partial class Index
    {
        public VirtualizeTable virtualizeTable = new VirtualizeTable();

        public Index()
        {
            virtualizeTable.ColumnNames = new string[] { "C1", "C2", "C3" };
            virtualizeTable.Rows = Enumerable.Range(0, 100)
                .Select(i => $"{i}------{Guid.NewGuid()}")
                .Select(s => new string[]
                {
                    $"C1:{s}",
                    $"C2:{s}",
                    $"C3:{s}",
                }).ToArray();
        }
        
        public class VirtualizeTable
        {
            public string[] ColumnNames { get; set; }
            public string[][] Rows { get; set; }
        }
    }
}
