using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public class LogColumn : LifeCycleTracker<LogColumn>
    {
        public string Name { get; }
        public WordRetriever? WordRetriever { get; }
        public LogColumn(string name, WordRetriever? wordRetriever)
        {
            Name = name;
            WordRetriever = wordRetriever;
        }
        public LogColumn(string name) : this(name, null)
        {
        }
    }
}
