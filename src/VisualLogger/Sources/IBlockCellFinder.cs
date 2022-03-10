using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal interface IBlockCellFinder
    {
        object? GetBlockCellValue(string recursivePath);
    }
}
