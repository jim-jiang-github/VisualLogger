using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Datas
{
    public class WordRetriever
    {
        private const string DELIMITER_CHARS = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private const string EXCLUDE_CHARS = "1234567890";
        private readonly Dictionary<string, List<int>> _wordRetrieveMap = new();
        public IEnumerable<string> Words
        {
            get
            {
                var words = _wordRetrieveMap.Select(d => d.Key);
                return words;
            }
        }
        public void AppendString(string str, int lineIndex)
        {
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                var c = str[i];
                var isDelimiterChar = DELIMITER_CHARS.Contains(c);
                if (isDelimiterChar && !EXCLUDE_CHARS.Contains(c))
                {
                    stringBuilder.Append(c);
                }
                if (stringBuilder.Length > 0 && (i == str.Length - 1 || (!isDelimiterChar && stringBuilder.Length > 0)))
                {
                    var word = stringBuilder.ToString();
                    if (_wordRetrieveMap.TryGetValue(word, out var indexs))
                    {
                        if (!indexs.Contains(lineIndex))
                        {
                            indexs.Add(lineIndex);
                        }
                    }
                    else
                    {
                        _wordRetrieveMap.Add(word, new List<int>() { lineIndex });
                    }
                    stringBuilder.Clear();
                }
            }
        }
    }
}
