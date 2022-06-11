using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Datas
{
    public class WordsCollection
    {
        private const string DELIMITER_CHARS = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private const string EXCLUDE_CHARS = "1234567890";
        private readonly List<string> _words = new();

        public IEnumerable<string> Words
        {
            get
            {
                foreach (var word in _words)
                {
                    yield return word;
                }
            }
        }
        public void AppendFromString(string text)
        {
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
                    if (_words.Contains(word))
                    {
                        return;
                    }
                    else
                    {
                        _words.Add(word);
                    }
                    stringBuilder.Clear();
                }
            }
        }
    }
}
