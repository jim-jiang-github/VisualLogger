using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    public class WordsCollection
    {
        private const string DELIMITER_CHARS = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        private const string EXCLUDE_CHARS = "1234567890";
        private readonly List<string> _words = new();

        public int MinimumStorable { get; set; } = 5;

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
                if (stringBuilder.Length > MinimumStorable && (i == text.Length - 1 || !isDelimiterChar && stringBuilder.Length > 0))
                {
                    var word = stringBuilder.ToString();
                    stringBuilder.Clear();
                    if (_words.Contains(word))
                    {
                        continue;
                    }
                    else
                    {
                        _words.Add(word);
                    }
                }
            }
        }
    }
}
