using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Storage;

namespace VisualLogger.ViewerApp.Data
{
    internal class MauiFileStorage : IFileStorage
    {
        public string CacheDirectory => FileSystem.CacheDirectory;

        public string AppDataDirectory => FileSystem.AppDataDirectory;
    }
}
