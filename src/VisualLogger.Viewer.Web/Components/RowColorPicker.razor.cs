using System.Drawing;

namespace VisualLogger.Viewer.Web.Components
{
    public partial class RowColorPicker
    {
        public IEnumerable<RowColor> RowColors
        {
            get
            {
                yield return new("#ff2800");
                yield return new("#fe9300");
                yield return new("#fefb00");
                yield return new("#02f900");
                yield return new("#107C10");

                yield return new("#00fdff");
                yield return new("#0433ff");
                yield return new("#ff40ff");
                yield return new("#942292");
                yield return new("#aa7942");

                yield return new("#234990");
                yield return new("#53d5fd");
                yield return new("#A80000");
                yield return new("#118DFF");
                yield return new("#73A7FE");

                yield return new("#874EFE");
                yield return new("#D357FE");
                yield return new("#ED719E");
                yield return new("#FF8C82");
                yield return new("#FFA57D");

                yield return new("#FFC677");
                yield return new("#FFF995");
                yield return new("#EBF38F");
                yield return new("#B1DD8C");
            }
        }

        public class RowColor
        {
            public string BackgroundColor { get; }
            public string ForeColor { get; }

            public RowColor(string backgroundColor)
            {
                BackgroundColor = backgroundColor;
                //var invertColorArgb = ColorTranslator.FromHtml(backgroundColor).ToArgb() ^ 0xffffff;
                //var invertColor = Color.FromArgb(invertColorArgb);
                //var invertColorHex = ColorTranslator.ToHtml(invertColor);
                var color = ColorTranslator.FromHtml(backgroundColor);
                var y = color.R * 0.299 + color.G * 0.587 + color.B * 0.114; //Get yuv Y
                if (y >= 128)
                {
                    ForeColor = "#000000";
                }
                else
                {
                    ForeColor = "#FFFFFF";
                }
            }
        }
    }
}
