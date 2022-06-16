using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Convertors
{
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
}
