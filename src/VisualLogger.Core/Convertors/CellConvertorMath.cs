using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisualLogger.Core.Convertors
{
    internal class CellConvertorMath : CellConvertor
    {
        private readonly CSharpScriptGlobalParameter<long> _parameter = new CSharpScriptGlobalParameter<long>();
        private readonly ScriptRunner<long>? _runner;

        public CellConvertorMath(string expression) : base(expression)
        {
            var pattern = @"{" + CellConvertor.CELL_VALUE + "}";
            Expression = Regex.Replace(Expression, pattern, nameof(CSharpScriptGlobalParameter<long>.Value));
            ScriptOptions.Default.WithEmitDebugInformation(false);
            var script = CSharpScript.Create<long>(Expression, globalsType: typeof(CSharpScriptGlobalParameter<long>));
            try
            {
                _runner = script.CreateDelegate();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "StreamCellConvertorMath CSharpScript error");
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
}
