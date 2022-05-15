using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.InterfaceModules
{
    public interface IWebsitePicker
    {
        string Name { get; }
        string RegexPattern { get; }
        Task<string> PickWebsite();
    }
}
