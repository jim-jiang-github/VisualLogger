using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace VisualLogger.Sources
{
    internal struct ContentSource
    {
        private readonly LogSourceReader _reader;

        public string[] ColumnHeadTemplate { get; }
        public IEnumerable<RowSource> Rows { get; }
        public ContentSource(LogSourceReader reader, string[] columnHeadTemplate, IEnumerable<RowSource> rows)
        {
            _reader = reader;
            ColumnHeadTemplate = columnHeadTemplate;
            Rows = rows;
        }

        public IEnumerable<LogRow> GetRows(int start, int length)
        {
            var reader = _reader;
            var rows = Rows.Skip(start).Take(length).Select(r => new LogRow(reader, r.Index, r.Cells.ToArray()));
            return rows;
        }
    }
}
