using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceModules
{
    public interface ILogContent
    {
        public StreamCellConvertor? GetConvertor(string? convertorName);
        StreamCell? GetCell(string recursivePath);
        StreamCell[]? GetCells(string recursivePath);
        string[]? GetItemsTemplate(string recursivePath);
        StreamCell[][]? GetBodyItems(string recursivePath);
    }
}
