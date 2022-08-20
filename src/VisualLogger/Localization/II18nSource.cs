using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Localization
{
    public interface II18nSource
    {
        private class I18nSourceNone : II18nSource
        {
            public string GetValueByKey(string key)
            {
                return key;
            }
        }
        public static II18nSource Default = new I18nSourceNone();
        string GetValueByKey(string key);
    }
}
