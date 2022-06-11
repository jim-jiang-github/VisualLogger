using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Sources;

namespace VisualLogger.Core.Convertors
{
    internal class CellConvertorProvider
    {
        private readonly ILogSource _logSource;
        private readonly SchemaLog _schemaLog;
        private readonly Dictionary<string, CellConvertor> _convertors = new();

        public CellConvertorProvider(ILogSource logSource, SchemaLog schemaLog)
        {
            _logSource = logSource;
            _schemaLog = schemaLog;
        }
        public CellConvertor? GetConvertor(string? convertorName)
        {
            if (_convertors == null)
            {
                return null;
            }
            if (convertorName == null)
            {
                return null;
            }
            if (_convertors.TryGetValue(convertorName, out CellConvertor? streamCellConvertor))
            {
                return streamCellConvertor;
            }
            else
            {
                var schemaConvertor = _schemaLog?.Convertors.FirstOrDefault(c => c.Name == convertorName);
                if (schemaConvertor == null)
                {
                    return null;
                }
                streamCellConvertor = CreateConvertor(_logSource, schemaConvertor);
                if (streamCellConvertor == null)
                {
                    return null;
                }
                _convertors.Add(convertorName, streamCellConvertor);
                return streamCellConvertor;
            }
        }

        private CellConvertor? CreateConvertor(ILogSource logContent, SchemaLog.SchemaConvertor? schemaConvertor)
        {
            if (schemaConvertor == null)
            {
                return null;
            }
            if (schemaConvertor.Expression == null)
            {
                return null;
            }
            var expression = schemaConvertor.Expression;
            var pattern = @"{(.*?)}";
            var matches = Regex.Matches(expression, pattern);
            var regex = new Regex(pattern);
            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count >= 1 &&
                    match.Groups[1].Value != CellConvertor.CELL_VALUE &&
                    logContent.GetCell(match.Groups[1].Value) is object value)
                {
                    var replacement = value.ToString();
                    if (replacement == null)
                    {
                        continue;
                    }
                    expression = regex.Replace(expression, replacement, 1);
                }
            }
            CellConvertor? streamCellConvertor = schemaConvertor.Type switch
            {
                SchemaConvertorType.Math => new CellConvertorMath(expression),
                SchemaConvertorType.Long2Time => new CellConvertorLong2Time(expression),
                SchemaConvertorType.Time2Time => new CellConvertorTime2Time(expression),
                SchemaConvertorType.Enum => new CellConvertorEnum(expression),
                _ => null,
            };
            if (streamCellConvertor != null && schemaConvertor.ContinueWith != null)
            {
                streamCellConvertor.ContinueConvertor = CreateConvertor(logContent, schemaConvertor.ContinueWith);
            }
            return streamCellConvertor;
        }
    }
}
