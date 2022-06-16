using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisualLogger.Convertors
{
    internal class CellConvertorEnum : CellConvertor
    {
        private readonly Dictionary<int, string> _enumDictionary;
        public CellConvertorEnum(string expression) : base(expression)
        {
            var matches = Regex.Matches(expression, @"(\d:[A-Z|a-z]*)");
            if (!matches.Any(m => m.Success))
            {
                _enumDictionary = new Dictionary<int, string>();
                return;
            }
            _enumDictionary = matches.Select(m => m.Value.Split(':')).
                Where(x => x.Length == 2).
                Select(x => new
                {
                    Key = int.TryParse(x[0], out int key) ? (int?)key : null,
                    Value = x[1]
                }).
                Where(x => x.Key != null).
                ToDictionary(x => x.Key ?? -1, x => x.Value);
        }
        protected override object? ConvertInternal(object? value)
        {
            var input = value?.ToString();
            if (input == null)
            {
                return value;
            }
            if (int.TryParse(input, out int enumValue) && _enumDictionary.TryGetValue(enumValue, out string? enumString))
            {
                return enumString;
            }
            return value;
        }
    }
}
