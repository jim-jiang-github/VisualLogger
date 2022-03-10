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
using VisualLogger.Sources;

namespace VisualLogger.Convertors
{
    internal abstract class CellConvertor
    {
        internal const string CELL_VALUE = "CellValue";
        protected string Expression { get; set; }
        public CellConvertor? ContinueConvertor { get; set; }
        protected CellConvertor(string expression)
        {
            Expression = expression;
        }
        internal virtual void Init(IBlockCellFinder blockCellFinder)
        {
            var pattern = @"{(.*?)}";
            var matches = Regex.Matches(Expression, pattern);
            var regex = new Regex(pattern);
            foreach (Match match in matches)
            {
                if (match.Success && match.Groups.Count >= 1 &&
                    match.Groups[1].Value != CELL_VALUE &&
                    blockCellFinder.GetBlockCellValue(match.Groups[1].Value) is string replacement)
                {
                    Expression = regex.Replace(Expression, replacement, 1);
                }
            }
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
