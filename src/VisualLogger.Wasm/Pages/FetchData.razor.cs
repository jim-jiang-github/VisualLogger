using BootstrapBlazor.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VisualLogger.Wasm.Pages
{
    public sealed partial class FetchData
    {
        private IEnumerable<TestData> Lines { get; } = Enumerable.Range(0, 100)
        .Select(i => $"{i}------{Guid.NewGuid()}")
        .Select(s => new TestData()
        {
            Name = s
        }).ToArray();

        public FetchData() 
        {
        }

        public class TestData
        {
            public string Name { get; set; }
        }
    }
}
