using InterfaceModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogDownloaders
{
    internal class LogDownloaderUrl : ILogDownloader
    {
        public string RegexPattern => @"^((https)?:\/\/)cdn.filestackcontent.com/[^\s]+";

        public string[] GetLogs(string logUrl)
        {
            return null;
        }
    }
}
