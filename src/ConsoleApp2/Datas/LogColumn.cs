using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public class LogColumn
    {
        public string Name { get; }
        public TextSplitter? Splitter { get; }
        public LogColumn(string name, TextSplitter? splitter)
        {
            Name = name;
            Splitter = splitter;
        }
        public LogColumn(string name) : this(name, null)
        {
        }
    }
}
