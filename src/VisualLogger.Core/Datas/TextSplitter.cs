using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Datas
{
    public class TextSplitter
    {
        private const string DELIMITER_CHARS = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private const string EXCLUDE_CHARS = "1234567890";
        private readonly Dictionary<string, List<int>> _wordsMap = new();
        public IEnumerable<string> Words
        {
            get
            {
                var words = _wordsMap.Select(d => d.Key.ToString());
                return words;
            }
        }
        public void AppendStreamCell(StreamCell streamCell)
        {
            var text = streamCell.ToString();
            //var matches = Regex.Matches(text, SPLIT_PATTERN);
            //foreach (Match match in matches)
            //{
            //    if (_wordRetrieveMap.TryGetValue(match.Value, out var indexs))
            //    {
            //        if (!indexs.Contains(streamCell.Index))
            //        {
            //            indexs.Add(streamCell.Index);
            //        }
            //    }
            //    else
            //    {
            //        _wordRetrieveMap.Add(match.Value, new List<int>() { streamCell.Index });
            //    }
            //}
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                var c = text[i];
                var isDelimiterChar = DELIMITER_CHARS.Contains(c);
                if (isDelimiterChar && !EXCLUDE_CHARS.Contains(c))
                {
                    stringBuilder.Append(c);
                }
                if (stringBuilder.Length > 0 && (i == text.Length - 1 || (!isDelimiterChar && stringBuilder.Length > 0)))
                {
                    var word = stringBuilder.ToString();
                    if (_wordsMap.TryGetValue(word, out var indexs))
                    {
                        if (!indexs.Contains(streamCell.Index))
                        {
                            indexs.Add(streamCell.Index);
                        }
                    }
                    else
                    {
                        _wordsMap.Add(word, new List<int>() { streamCell.Index });
                    }
                    stringBuilder.Clear();
                }
            }
        }
    }
}
