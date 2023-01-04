using Microsoft.Extensions.DependencyInjection;
using VisualLogger.Localization;
namespace VisualLogger.Localization
{
    /// <summary>
    /// This is auto generate by StringKeys.tt
    /// </summary>
    public static class StringKeys
    {
        private static II18nSource? _i18nSourceInternal;
        private static II18nSource I18nSourceInternal => _i18nSourceInternal ??= (Global.ServiceProvider?.GetService<II18nSource>() ?? II18nSource.Default);
        public class Repo
        {
            /// <summary>
            /// jim-jiang-github
            /// </summary>
            public static string UserName => "jim-jiang-github";
            /// <summary>
            /// VisualLogger
            /// </summary>
            public static string RepoName => "VisualLogger";
            /// <summary>
            /// https://github.com/jim-jiang-github/VisualLogger
            /// </summary>
            public static string RepoUrl => "https://github.com/jim-jiang-github/VisualLogger";
        }
        public class MenuBar
        {
            /// <summary>
            /// File…
            /// </summary>
            public static string File => I18nSourceInternal.GetValueByKey("MenuBar.File");
            public class FileSub
            {
                /// <summary>
                /// Open
                /// </summary>
                public static string Open => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.Open");
                public class OpenSub
                {
                    /// <summary>
                    /// Files…
                    /// </summary>
                    public static string FormFiles => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.OpenSub.FormFiles");
                    /// <summary>
                    /// Folder…
                    /// </summary>
                    public static string FromFolder => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.OpenSub.FromFolder");
                    /// <summary>
                    /// Website…
                    /// </summary>
                    public static string FromWebsite => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.OpenSub.FromWebsite");
                }
                /// <summary>
                /// Scenario…
                /// </summary>
                public static string Scenario => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.Scenario");
                /// <summary>
                /// Exit
                /// </summary>
                public static string Exit => I18nSourceInternal.GetValueByKey("MenuBar.FileSub.Exit");
            }
            /// <summary>
            /// Tools…
            /// </summary>
            public static string Tools => I18nSourceInternal.GetValueByKey("MenuBar.Tools");
            public class ToolsSub
            {
                /// <summary>
                /// Options…
                /// </summary>
                public static string Options => I18nSourceInternal.GetValueByKey("MenuBar.ToolsSub.Options");
            }
            /// <summary>
            /// Help…
            /// </summary>
            public static string Help => I18nSourceInternal.GetValueByKey("MenuBar.Help");
        }
        public class Picker
        {
            /// <summary>
            /// Select files
            /// </summary>
            public static string SelectFiles => I18nSourceInternal.GetValueByKey("Picker.SelectFiles");
        }
        public class Hotkeys
        {
            /// <summary>
            /// Highlight
            /// </summary>
            public static string Highlight => I18nSourceInternal.GetValueByKey("Hotkeys.Highlight");
            /// <summary>
            /// Find next highlight
            /// </summary>
            public static string NextHighlight => I18nSourceInternal.GetValueByKey("Hotkeys.NextHighlight");
        }
        public class Notification
        {
            /// <summary>
            /// An error occurred.
            /// </summary>
            public static string ErrorTitle => I18nSourceInternal.GetValueByKey("Notification.ErrorTitle");
            /// <summary>
            /// A warning was found.
            /// </summary>
            public static string WarningTitle => I18nSourceInternal.GetValueByKey("Notification.WarningTitle");
            /// <summary>
            /// Attention!
            /// </summary>
            public static string InfoTitle => I18nSourceInternal.GetValueByKey("Notification.InfoTitle");
        }
        public class ErrorHandling
        {
            /// <summary>
            /// Oh shit!
            /// </summary>
            public static string Title => I18nSourceInternal.GetValueByKey("ErrorHandling.Title");
            /// <summary>
            /// The app was terminated unexpectedly. To continue, please reload.
            /// </summary>
            public static string Message => I18nSourceInternal.GetValueByKey("ErrorHandling.Message");
            /// <summary>
            /// Reload
            /// </summary>
            public static string Reload => I18nSourceInternal.GetValueByKey("ErrorHandling.Reload");
            /// <summary>
            /// Report
            /// </summary>
            public static string Report => I18nSourceInternal.GetValueByKey("ErrorHandling.Report");
        }
        public class Dialog
        {
            public class Button
            {
                /// <summary>
                /// Close
                /// </summary>
                public static string Close => I18nSourceInternal.GetValueByKey("Dialog.Button.Close");
            }
        }
        public class Scenario
        {
            public class Options
            {
                /// <summary>
                /// Enter a git repo.
                /// </summary>
                public static string Repo => I18nSourceInternal.GetValueByKey("Scenario.Options.Repo");
                /// <summary>
                /// Select a branch.
                /// </summary>
                public static string SelectBranch => I18nSourceInternal.GetValueByKey("Scenario.Options.SelectBranch");
            }
        }
        public class Main
        {
            /// <summary>
            /// Visual log viewer
            /// </summary>
            public static string Title => I18nSourceInternal.GetValueByKey("Main.Title");
        }
        public class User
        {
            /// <summary>
            /// Namxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxe
            /// </summary>
            public static string Name => I18nSourceInternal.GetValueByKey("User.Name");
            /// <summary>
            /// Agexxxxxxxxxxxxxxxxx
            /// </summary>
            public static string Age => I18nSourceInternal.GetValueByKey("User.Age");
            /// <summary>
            /// Agexxxxxxxxxxxxx1
            /// </summary>
            public static string Age1 => I18nSourceInternal.GetValueByKey("User.Age1");
            public class User1
            {
                /// <summary>
                /// Name1xxxxxxxxxxxx
                /// </summary>
                public static string Name1 => I18nSourceInternal.GetValueByKey("User.User1.Name1");
                /// <summary>
                /// Agxxxxxxxxxxxxxxxxxxxxe1
                /// </summary>
                public static string Age1 => I18nSourceInternal.GetValueByKey("User.User1.Age1");
            }
        }
        public class Goods
        {
            /// <summary>
            /// Namexxxxxxxxxxxxxxxxxxxxxxxx
            /// </summary>
            public static string Name => I18nSourceInternal.GetValueByKey("Goods.Name");
            /// <summary>
            /// Pricxxxxxxxxxxxxe
            /// </summary>
            public static string Price => I18nSourceInternal.GetValueByKey("Goods.Price");
        }
        /// <summary>
        /// Homexxxxxxxxxxxxxxxxxxxxxxxxxx
        /// </summary>
        public static string Home => I18nSourceInternal.GetValueByKey("Home");
        /// <summary>
        /// Docsxxxxxxxxxxxxxx
        /// </summary>
        public static string Docs => I18nSourceInternal.GetValueByKey("Docs");
        /// <summary>
        /// Blogxxxxxxxxxxxxxx
        /// </summary>
        public static string Blog => I18nSourceInternal.GetValueByKey("Blog");
        /// <summary>
        /// Teamxxxxxxxxxxxxxxxxxx
        /// </summary>
        public static string Team => I18nSourceInternal.GetValueByKey("Team");
        /// <summary>
        /// Searchxxxxxxxxxxxxx
        /// </summary>
        public static string Search => I18nSourceInternal.GetValueByKey("Search");
    }
}
