using BlazorComponent;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace VisualLogger.Viewer.Web.Pages
{
    public partial class FilterDialog : IDisposable
    {
        [Parameter]
        public bool _dialog { get; set; }
        private bool _loading;
        private IList<string> _items = Enumerable.Range(0, 50000).Select(i => $"long long data:{i}").ToArray();
        private IList<string> _itemsFilter = new string[0];

        private string? _select;

        private async Task Show(MouseEventArgs args)
        {
            showMenu = false;

            X = args.ClientX;
            Y = args.ClientY;
            showMenu = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            SearchService.ShowFilterDialog += (s, e) =>
            {
                _dialog = e;
                StateHasChanged();
            };
        }
        private void DialogChanged(bool isShowDialog)
        {
            _dialog = isShowDialog;
            SearchService.IsShow = isShowDialog;
        }

        private async Task QuerySelections(string v)
        {
            if (string.IsNullOrEmpty(v) || v == _select)
            {
                return;
            }

            _loading = true;

            //_itemsFilter = _items.AsParallel()
            //.Where(x => x.Contains(v))
            //.Take(10)
            //.ToList();

            _loading = false;
        }

        public void Dispose()
        {
        }

        private StringNumber _selectedItem = 1;
        private Item[] _items1 = new Item[]
        {
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Real-Time", Icon= "mdi-clock" },
       new Item { Text= "Audience", Icon= "mdi-account" },
       new Item { Text= "ConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversionsConversions", Icon= "mdi-flag" }
        };

        class Item
        {
            public bool Enbale { get; set; }
            public string? Text { get; set; }
            public string? Icon { get; set; }

            public bool IsMatchCase { get; set; } = false;
            public bool IsMatchWholeWord { get; set; } = false;
            public bool IsUseRegularExpression { get; set; } = false;

            public void MatchCaseClick()
            {
                IsMatchCase = !IsMatchCase;
            }

            public void MatchWholeWordClick()
            {
                IsMatchWholeWord = !IsMatchWholeWord;
            }

            public void UseRegularExpressionClick()
            {
                IsUseRegularExpression = !IsUseRegularExpression;
            }
        }
        bool showMenu = false;
        double X = 0;
        double Y = 0;
    }
}
