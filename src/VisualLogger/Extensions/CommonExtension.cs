using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Extensions
{
    internal static class CommonExtension
    {
        public static object? Convert(this object? cell, CellConvertor? cellConvertor)
        {
            if (cell == null)
            {
                return null;
            }
            if (cellConvertor == null)
            {
                return cell;
            }
            else
            {
                return cellConvertor.Convert(cell);
            }
        }
    }
}
