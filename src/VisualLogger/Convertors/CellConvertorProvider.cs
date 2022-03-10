using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Schemas.Logs;
using VisualLogger.Sources;

namespace VisualLogger.Convertors
{
    internal class CellConvertorProvider
    {
        private readonly Dictionary<string, CellConvertor?> _convertorMap;

        public CellConvertorProvider(SchemaLog schemaLog)
        {
            _convertorMap = schemaLog.Convertors.ToDictionary(x => x.Name, x => CreateConvertor(x));
        }
        public void Init(IBlockCellFinder blockCellFinder)
        {
            foreach (var convertor in _convertorMap)
            {
                convertor.Value?.Init(blockCellFinder);
            }
        }

        public CellConvertor? GetConvertor(string? convertorName)
        {
            if (convertorName == null)
            {
                return null;
            }
            if (_convertorMap.TryGetValue(convertorName, out CellConvertor? cellConvertor))
            {
                return cellConvertor;
            }
            return null;
        }

        private CellConvertor? CreateConvertor(SchemaLog.SchemaConvertor? schemaConvertor)
        {
            if (schemaConvertor == null)
            {
                return null;
            }
            if (schemaConvertor.Expression == null)
            {
                return null;
            }
            CellConvertor? streamCellConvertor = schemaConvertor.Type switch
            {
                SchemaConvertorType.Math => new CellConvertorMath(schemaConvertor.Expression),
                SchemaConvertorType.Long2Time => new CellConvertorLong2Time(schemaConvertor.Expression),
                SchemaConvertorType.Time2Time => new CellConvertorTime2Time(schemaConvertor.Expression),
                SchemaConvertorType.Enum => new CellConvertorEnum(schemaConvertor.Expression),
                _ => null,
            };
            if (streamCellConvertor != null && schemaConvertor.ContinueWith != null)
            {
                streamCellConvertor.ContinueConvertor = CreateConvertor(schemaConvertor.ContinueWith);
            }
            return streamCellConvertor;
        }
    }
}
