﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Viewer.Interfaces
{
    public interface IFolderPicker
    {
        Task<string?> PickFolder();
    }
}
