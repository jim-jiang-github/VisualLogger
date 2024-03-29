﻿using BlazorComponent.I18n;
using VisualLogger.Localization;

namespace VisualLogger.Viewer.Data
{
    public class I18nSource : II18nSource
    {
        private readonly I18n _i18n;

        public I18nSource(I18n i18n)
        {
            _i18n = i18n;
        }
        public string GetValueByKey(string key)
        {
            var value = _i18n?.T(key, false, true) ?? key;
            return value;
        }
    }
}
