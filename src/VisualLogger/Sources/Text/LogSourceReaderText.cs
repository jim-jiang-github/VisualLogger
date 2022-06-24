using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Streams;

namespace VisualLogger.Sources.Text
{
    internal class LogSourceReaderText : LogSourceReader
    {
        private readonly StreamBytesLineReader _streamBytesLineReader;
        public LogSourceReaderText(Stream stream, Encoding encoding, CellConvertor?[] cellConvertors) : base(cellConvertors)
        {
            _streamBytesLineReader = new StreamBytesLineReader(stream, encoding);
        }

        protected override object GetRawValue(CellSource cell, int cellIndex)
        {
            string? value = _streamBytesLineReader.ReadString(cell.Position, cell.Data);
            return value ?? string.Empty;
        }

        public override void Dispose()
        {
        }
    }
}
