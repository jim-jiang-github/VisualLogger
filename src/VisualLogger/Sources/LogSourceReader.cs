using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Sources.Binary;

namespace VisualLogger.Sources
{
    internal abstract class LogSourceReader : IDisposable
    {
        private readonly CellConvertor?[] _cellConvertors;
        public LogSourceReader(CellConvertor?[] cellConvertors)
        {
            _cellConvertors = cellConvertors;
        }
        protected abstract object GetRawValue(CellSource cell, int cellIndex);
        public string GetValue(CellSource cell, int cellIndex)
        {
            var value = GetRawValue(cell, cellIndex);
            if (_cellConvertors?[cellIndex] is CellConvertor cellConvertor)
            {
                return cellConvertor.Convert(value)?.ToString() ?? string.Empty;
            }
            else
            {
                return value?.ToString() ?? string.Empty;
            }
        }
        public abstract void Dispose();
    }
}
