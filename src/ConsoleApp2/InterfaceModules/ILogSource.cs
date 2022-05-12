using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceModules
{
    public interface ILogSource
    {
        public StreamCellConvertor? GetConvertor(string? convertorName);
        StreamCell? GetCell(string recursivePath);
        string[] GetBodyTemplate();
        IEnumerable<StreamCell[]> GetBodyItems();
    }
}
