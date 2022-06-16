using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisualLogger.Convertors
{
    internal class CellConvertorTime2Time : CellConvertor
    {
        public CellConvertorTime2Time(string expression) : base(expression)
        {
        }
        protected override object? ConvertInternal(object? value)
        {
            var input = value?.ToString();
            if (input == null)
            {
                return value;
            }
            var matches = Regex.Matches(Expression, @"(?<=\[).*?(?=\])");
            if (matches.Count == 2)
            {
                var fromTimeFormat = matches[0].Value.ToString();
                var toTimeFormat = matches[1].Value.ToString();
                if (DateTime.TryParseExact(input, fromTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime))
                {
                    var formatedTime = datetime.ToString(toTimeFormat);
                    return formatedTime;
                }
            }
            return value;
        }
    }
}
