using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Core.Schemas
{
    [DefaultValue(Unknow)]
    public enum SchemaType
    {
        LogBinary,
        LogText,
        Scenario,
        Unknow
    }
}