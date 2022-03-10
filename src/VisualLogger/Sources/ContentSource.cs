using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;

namespace VisualLogger.Sources
{
    internal class ContentSource
    {
        public string[] ColumnHeadTemplate { get; }
        public IEnumerable<LogRow> Rows { get; }
        public ContentSource(string[] columnHeadTemplate, IEnumerable<LogRow> rows)
        {
            ColumnHeadTemplate = columnHeadTemplate;
            Rows = rows;
        }

        public IEnumerable<LogRow> GetRows(int start, int length)
        {
            var rows = Rows.Skip(start).Take(length);
            return rows;
        }
    }
}
