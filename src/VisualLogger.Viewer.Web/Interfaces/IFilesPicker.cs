﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Viewer.Web.Interfaces
{
    public interface IFilesPicker
    {
        Task<IEnumerable<string>> PickFiles();
    }
}
