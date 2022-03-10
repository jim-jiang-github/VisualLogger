using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Convertors
{
    internal class CellConvertorLong2Time : CellConvertor
    {
        public CellConvertorLong2Time(string expression) : base(expression)
        {
        }
        protected override object? ConvertInternal(object? value)
        {
            var input = value?.ToString();
            if (input == null)
            {
                return value;
            }
            if (long.TryParse(input, out long milliseconds))
            {
                var timeSpant = TimeSpan.FromMilliseconds(milliseconds);
                var dateTime = DateTime.UnixEpoch + timeSpant + TimeZoneInfo.Local.BaseUtcOffset;
                var time = dateTime.ToString(Expression);
                return time;
            }
            return value;
        }
    }
}
