using BlazorComponent.I18n;
using System.Globalization;
namespace VisualLogger.Viewer.Web.Localization
{
    /// <summary>
    /// This is auto generate by I18nKeys.tt
    /// </summary>
    public static class I18nKeys
    {
        private static I18n? _i18nInternal;
        private static I18n? I18nInternal => _i18nInternal ??= Global.ServiceProvider?.GetService<I18n>();
        /// <summary>
        /// https://github.com/jim-jiang-github/VisualLogger
        /// </summary>
        public static string GithubRepo => "https://github.com/jim-jiang-github/VisualLogger";
        public class MenuBar
        {
            /// <summary>
            /// File…
            /// </summary>
            public static string File => I18nInternal?.T("MenuBar.File", false, true) ?? "File";
            public class FileSub
            {
                /// <summary>
                /// Open
                /// </summary>
                public static string Open => I18nInternal?.T("MenuBar.FileSub.Open", false, true) ?? "Open";
                public class OpenSub
                {
                    /// <summary>
                    /// Files…
                    /// </summary>
                    public static string FormFiles => I18nInternal?.T("MenuBar.FileSub.OpenSub.FormFiles", false, true) ?? "FormFiles";
                    /// <summary>
                    /// Folder…
                    /// </summary>
                    public static string FromFolder => I18nInternal?.T("MenuBar.FileSub.OpenSub.FromFolder", false, true) ?? "FromFolder";
                    /// <summary>
                    /// Website…
                    /// </summary>
                    public static string FromWebsite => I18nInternal?.T("MenuBar.FileSub.OpenSub.FromWebsite", false, true) ?? "FromWebsite";
                }
                /// <summary>
                /// Scenario…
                /// </summary>
                public static string Scenario => I18nInternal?.T("MenuBar.FileSub.Scenario", false, true) ?? "Scenario";
                /// <summary>
                /// Exit
                /// </summary>
                public static string Exit => I18nInternal?.T("MenuBar.FileSub.Exit", false, true) ?? "Exit";
            }
            /// <summary>
            /// Tools…
            /// </summary>
            public static string Tools => I18nInternal?.T("MenuBar.Tools", false, true) ?? "Tools";
            public class ToolsSub
            {
                /// <summary>
                /// Options…
                /// </summary>
                public static string Options => I18nInternal?.T("MenuBar.ToolsSub.Options", false, true) ?? "Options";
            }
            /// <summary>
            /// Help…
            /// </summary>
            public static string Help => I18nInternal?.T("MenuBar.Help", false, true) ?? "Help";
        }
        public class Picker
        {
            /// <summary>
            /// Select files
            /// </summary>
            public static string SelectFiles => I18nInternal?.T("Picker.SelectFiles", false, true) ?? "SelectFiles";
        }
        public class Hotkeys
        {
            /// <summary>
            /// Highlight
            /// </summary>
            public static string Highlight => I18nInternal?.T("Hotkeys.Highlight", false, true) ?? "Highlight";
            /// <summary>
            /// Find next highlight
            /// </summary>
            public static string NextHighlight => I18nInternal?.T("Hotkeys.NextHighlight", false, true) ?? "NextHighlight";
        }
        public class Notification
        {
            /// <summary>
            /// An error occurred.
            /// </summary>
            public static string ErrorTitle => I18nInternal?.T("Notification.ErrorTitle", false, true) ?? "ErrorTitle";
            /// <summary>
            /// A warning was found.
            /// </summary>
            public static string WarningTitle => I18nInternal?.T("Notification.WarningTitle", false, true) ?? "WarningTitle";
            /// <summary>
            /// Attention!
            /// </summary>
            public static string InfoTitle => I18nInternal?.T("Notification.InfoTitle", false, true) ?? "InfoTitle";
        }
        public class Scenario
        {
            public class Options
            {
                /// <summary>
                /// Enter a git repo.
                /// </summary>
                public static string Repo => I18nInternal?.T("Scenario.Options.Repo", false, true) ?? "Repo";
            }
        }
        public class Main
        {
            /// <summary>
            /// Visual log viewer
            /// </summary>
            public static string Title => I18nInternal?.T("Main.Title", false, true) ?? "Title";
        }
        public class User
        {
            /// <summary>
            /// Namxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxe
            /// </summary>
            public static string Name => I18nInternal?.T("User.Name", false, true) ?? "Name";
            /// <summary>
            /// Agexxxxxxxxxxxxxxxxx
            /// </summary>
            public static string Age => I18nInternal?.T("User.Age", false, true) ?? "Age";
            /// <summary>
            /// Agexxxxxxxxxxxxx1
            /// </summary>
            public static string Age1 => I18nInternal?.T("User.Age1", false, true) ?? "Age1";
            public class User1
            {
                /// <summary>
                /// Name1xxxxxxxxxxxx
                /// </summary>
                public static string Name1 => I18nInternal?.T("User.User1.Name1", false, true) ?? "Name1";
                /// <summary>
                /// Agxxxxxxxxxxxxxxxxxxxxe1
                /// </summary>
                public static string Age1 => I18nInternal?.T("User.User1.Age1", false, true) ?? "Age1";
            }
        }
        public class Goods
        {
            /// <summary>
            /// Namexxxxxxxxxxxxxxxxxxxxxxxx
            /// </summary>
            public static string Name => I18nInternal?.T("Goods.Name", false, true) ?? "Name";
            /// <summary>
            /// Pricxxxxxxxxxxxxe
            /// </summary>
            public static string Price => I18nInternal?.T("Goods.Price", false, true) ?? "Price";
        }
        /// <summary>
        /// Homexxxxxxxxxxxxxxxxxxxxxxxxxx
        /// </summary>
        public static string Home => I18nInternal?.T("Home", false, true) ?? "Home";
        /// <summary>
        /// Docsxxxxxxxxxxxxxx
        /// </summary>
        public static string Docs => I18nInternal?.T("Docs", false, true) ?? "Docs";
        /// <summary>
        /// Blogxxxxxxxxxxxxxx
        /// </summary>
        public static string Blog => I18nInternal?.T("Blog", false, true) ?? "Blog";
        /// <summary>
        /// Teamxxxxxxxxxxxxxxxxxx
        /// </summary>
        public static string Team => I18nInternal?.T("Team", false, true) ?? "Team";
        /// <summary>
        /// Searchxxxxxxxxxxxxx
        /// </summary>
        public static string Search => I18nInternal?.T("Search", false, true) ?? "Search";
    }
}
