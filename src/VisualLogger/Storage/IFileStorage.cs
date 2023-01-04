using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Localization;

namespace VisualLogger.Storage
{
    public interface IFileStorage
    {
        private class FileStorageNone : IFileStorage
        {
            public string CacheDirectory => string.Empty;

            public string AppDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }
        public static IFileStorage Default = new FileStorageNone();
        string CacheDirectory { get; }
        string AppDataDirectory { get; }
    }
}
