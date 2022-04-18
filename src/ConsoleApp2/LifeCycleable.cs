using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger
{
    public class LifeCycleable<T>
        where T: LifeCycleable<T>
    {
        ~LifeCycleable() 
        {
            LifeCycleViewer.Release(GetType());
        }

        public LifeCycleable()
        {
            LifeCycleViewer.Create(GetType());
        }
    }
}
