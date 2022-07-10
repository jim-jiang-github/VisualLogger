using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Console
{
    public class ScriptManager
    {
        public void ExecuteScript(string scriptId)
        {
            try
            {
                string inputSript = GetStriptById(scriptId);
                var scriptOptions = ScriptOptions.Default;

                Execute(inputSript, scriptOptions);
                var result = Execute("new ScriptedClass().input", scriptOptions);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        private string GetStriptById(string id)
        {

            string csSript = @" public class ScriptedClass
            {
                public string Input {get;set;}
            }";

            return csSript;
        }

        private static ScriptState<object> scriptState = null;
        public static object Execute(string code, dynamic scriptOptions)
        {
            scriptState = scriptState == null ? CSharpScript.RunAsync(code).Result : scriptState.ContinueWithAsync(code).Result;

            if (scriptState.ReturnValue != null && !string.IsNullOrEmpty(scriptState.ReturnValue.ToString()))
                return scriptState.ReturnValue;
            return null;
        }
    }
}
