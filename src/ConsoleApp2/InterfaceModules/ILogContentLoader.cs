using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using VisualLogger.Datas.LogContents;

namespace VisualLogger.InterfaceModules
{
    public interface ILogContentLoader
    {
        LogContent LoadLogContent(string logPath);
    }
}
