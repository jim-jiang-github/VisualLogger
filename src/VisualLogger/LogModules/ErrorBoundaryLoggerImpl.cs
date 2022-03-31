using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.LogModules
{
    internal class ErrorBoundaryLoggerImpl : IErrorBoundaryLogger
    {
        public ValueTask LogErrorAsync(Exception exception)
        {
            return new ValueTask();
        }
    }
}
