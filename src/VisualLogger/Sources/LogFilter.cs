using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    public class LogFilter
    {
        internal List<string> Filters { get; } = new List<string>();
        public void AddFilter(string filter)
        {
            ((List<string>)Filters).Add(filter);
        }
    }
}
