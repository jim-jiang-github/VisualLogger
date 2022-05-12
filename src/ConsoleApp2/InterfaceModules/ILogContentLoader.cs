using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceModules
{
    public interface ILogContentLoader
    {
        ILogSource LoadLogContent(Stream stream, ILogSchemaLoader logSchemaLoader);
    }
}
