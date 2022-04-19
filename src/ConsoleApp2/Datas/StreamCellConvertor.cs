using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Convertors;

namespace VisualLogger.Datas
{
    public abstract class StreamCellConvertor : LifeCycleTracker<StreamCellConvertor>
    {
        private const string CELL_VALUE = "CellValue";
        #region Internal Class
        /// <summary>
        /// This must use public, Otherwise '_script.RunAsync(_parameter).Result' will cause 'is inaccessible due to its protection level' error.
        /// 
        /// The parameter of CSharpScript expression
        /// 
        /// var script = CSharpScript.Create<int>("Value*Value", globalsType: typeof(CSharpScriptGlobalParameter<int>));
        /// script.Compile();
        /// var qweqwe = script.RunAsync(new CSharpScriptGlobalParameter<int>()
        /// {
        ///     Value = 11111
        /// }).Result.ReturnValue;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class CSharpScriptGlobalParameter<T>
        {
            public T? Value { get; set; }
        }
        private class StreamCellConvertorMath : StreamCellConvertor
        {
            private CSharpScriptGlobalParameter<long> _parameter = new CSharpScriptGlobalParameter<long>();
            private ScriptRunner<long>? _runner;

            public StreamCellConvertorMath(string expression) : base(expression)
            {
                var pattern = @"{" + CELL_VALUE + "}";
                Expression = Regex.Replace(Expression, pattern, nameof(CSharpScriptGlobalParameter<long>.Value));
                ScriptOptions.Default.WithEmitDebugInformation(false);
                var script = CSharpScript.Create<long>(Expression, globalsType: typeof(CSharpScriptGlobalParameter<long>));
                try
                {
                    _runner = script.CreateDelegate();
                }
                catch (Exception ex)
                {
                    //TODO log
                    _runner = null;
                }
            }
            protected override object? ConvertInternal(object? value)
            {
                var input = value?.ToString();
                if (input == null)
                {
                    return value;
                }
                if (_runner == null)
                {
                    return value;
                }
                if (int.TryParse(input, out int tickOffset))
                {
                    _parameter.Value = tickOffset;
                    var result = _runner.Invoke(_parameter).Result;
                    return result;
                }
                return value;
            }
        }
        private class StreamCellConvertorTime : StreamCellConvertor
        {
            public StreamCellConvertorTime(string expression) : base(expression)
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
        private class StreamCellConvertorEnum : StreamCellConvertor
        {
            private readonly Dictionary<int, string> _enumDictionary;
            public StreamCellConvertorEnum(string expression) : base(expression)
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
        #endregion
        protected string Expression { get; set; }
        public StreamCellConvertor? ContinueConvertor { get; set; }
        private StreamCellConvertor(string expression)
        {
            Expression = expression;
        }
        public static StreamCellConvertor? CreateConvertor(ILogContent logContent, ConvertorSchema? convertorSchema)
        {
            if (convertorSchema == null)
            {
                return null;
            }
            if (convertorSchema.Expression == null)
            {
                return null;
            }
            var expression = convertorSchema.Expression;
            var pattern = @"{(.*?)}";
            var matches = Regex.Matches(expression, pattern);
            var regex = new Regex(pattern);
            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count >= 1 &&
                    match.Groups[1].Value != CELL_VALUE &&
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
            StreamCellConvertor? streamCellConvertor = convertorSchema.Type switch
            {
                ConvertorSchemaType.Math => new StreamCellConvertorMath(expression),
                ConvertorSchemaType.Time => new StreamCellConvertorTime(expression),
                ConvertorSchemaType.Enum => new StreamCellConvertorEnum(expression),
                _ => null,
            };
            if (streamCellConvertor != null && convertorSchema.ContinueConvertor != null)
            {
                streamCellConvertor.ContinueConvertor = CreateConvertor(logContent, convertorSchema.ContinueConvertor);
            }
            return streamCellConvertor;
        }

        public object? Convert(object? value)
        {
            value = ConvertInternal(value);
            if (ContinueConvertor != null)
            {
                value = ContinueConvertor.Convert(value);
            }
            return value;
        }
        protected abstract object? ConvertInternal(object? value);
    }
}
