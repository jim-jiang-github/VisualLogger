using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger
{
    public class LifeCycleTracker<T>
        where T: LifeCycleTracker<T>
    {
        ~LifeCycleTracker() 
        {
            LifeCycleViewer.Release(GetType());
        }

        public LifeCycleTracker()
        {
            LifeCycleViewer.Create(GetType());
        }
    }
}
