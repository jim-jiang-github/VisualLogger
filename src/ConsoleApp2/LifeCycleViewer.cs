using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger
{
    public static class LifeCycleViewer
    {
        public static Dictionary<string, int> LifeCycleMap { get; } = new Dictionary<string, int>();

        public static void Create(Type type)
        {
            if (!LifeCycleMap.TryGetValue(type.Name, out int count))
            {
                LifeCycleMap.Add(type.Name, count);
            }
            LifeCycleMap[type.Name]++;
        }
        public static void Release(Type type)
        {
            LifeCycleMap[type.Name]--;
        }

        public static string GetLifeCycleInfo()
        {
            return string.Join("\r\n", LifeCycleMap.Select(d => $"{d.Key}:{d.Value}"));
        }
    }
}
