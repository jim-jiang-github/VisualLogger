using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceModule
{
    public interface ILogGetter
    {
        string RegexPattern { get; }
        /// <summary>
        /// Log paths
        /// </summary>
        /// <param name="logSource">Log source. It could from the URL or local file and folder</param>
        /// <returns></returns>
        string[] GetLogs(string logSource);
    }
}
