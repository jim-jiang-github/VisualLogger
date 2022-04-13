using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public enum BinaryType
    {
        Skip,
        Boolean,
        Byte,
        Char,
        Short,
        Int,
        Long,
        UShort,
        UInt,
        ULong,
        Float,
        Double,
        Decimal,
        StringWithLength,
        StringWithIntHead,
    }
}
