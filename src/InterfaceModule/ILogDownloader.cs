using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceModule
{
    public interface ILogDownloader
    {
        string RegexPattern { get; }
        string[] GetLogs(string logUrl);
    }
}
