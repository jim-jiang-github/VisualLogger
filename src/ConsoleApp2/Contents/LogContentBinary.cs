using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Contents
{
    public class LogContentBinary : LogContent<LogSchemaBinary, LogSchemaBinary.BlockBinary, LogSchemaBinary.BodyBinary, LogSchemaBinary.CellBinary>
    {
        public LogContentBinary(Stream stream, LogSchema<LogSchemaBinary, LogSchemaBinary.BlockBinary, LogSchemaBinary.BodyBinary, LogSchemaBinary.CellBinary> logSchema) : base(stream, logSchema)
        {
        }
        #region Internal Class
        private class BlockContentBinary : BlockContent
        {
            public BlockContentBinary(
                ILogContent logContent,
                MixStreamReader mixStreamReader, LogSchemaBinary.BlockBinary block, ref long streamPosition) :
                base(logContent, mixStreamReader, block, ref streamPosition)
            {
            }

            protected override BodyContent? CreateBodyContent(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                LogSchemaBinary.BlockBinary block,
                LogSchemaBinary.BodyBinary body,
                ref long streamPosition)
            {
                if (body.BodyTemplate == null)
                {
                    //TODO log body.BodyTemplate can not be null here.
                    return null;
                }
                if (body.Count == null)
                {
                    //TODO log body.Count can not be null here.
                    return null;
                }
                var bodyTemplateNames = body.BodyTemplate.Select(t => t.Name).ToArray();
                int itemCount = 0;
                var countParser = body.Count;
                if (int.TryParse(countParser, out int count))
                {
                    itemCount = count;
                }
                else
                {
                    var streamCell = logContent.GetCell(countParser);
                    if (streamCell?.GetValue() is int countFromPath)
                    {
                        itemCount = countFromPath;
                    }
                }
                var contentItems = new StreamCell[itemCount][];
                for (int i = 0; i < itemCount; i++)
                {
                    StreamCell[] streamCells = new StreamCell[bodyTemplateNames.Length];
                    for (int j = 0; j < bodyTemplateNames.Length; j++)
                    {
                        var template = body.BodyTemplate[j];
                        var streamCell = CreateStreamCell(logContent, mixStreamReader, template, ref streamPosition);
                        if (streamCell != null)
                        {
                            streamCells[j] = streamCell;
                        }
                    }
                    contentItems[i] = streamCells;
                }
                return new BodyContent(bodyTemplateNames.Cast<string>().ToArray(), contentItems);
            }

            protected override CellsContent? CreateCellsContent(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                LogSchemaBinary.BlockBinary block,
                LogSchemaBinary.CellBinary[] cells,
                ref long streamPosition)
            {
                var cellNames = new List<string>();
                var item = new List<StreamCell>();
                foreach (var cell in cells)
                {
                    var streamCell = CreateStreamCell(logContent, mixStreamReader, cell, ref streamPosition);
                    if (streamCell != null && cell.Name != null)
                    {
                        cellNames.Add(cell.Name);
                        item.Add(streamCell);
                    }
                }
                return new CellsContent(cellNames.ToArray(), item.ToArray());
            }
            private StreamCell? CreateStreamCell(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                LogSchemaBinary.CellBinary cell,
                ref long position)
            {
                var type = cell.Type;
                var length = cell.Length;
                switch (type)
                {
                    case LogSchemaBinaryType.Skip:
                        if (length is int count)
                        {
                            position += count;
                        }
                        return null;
                    case LogSchemaBinaryType.Boolean:
                        position += 1;
                        return new StreamCell(mixStreamReader, position - 1, 1, StreamCellType.Boolean, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Byte:
                        position += 1;
                        return new StreamCell(mixStreamReader, position - 1, 1, StreamCellType.Byte, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Char:
                        position += 1;
                        return new StreamCell(mixStreamReader, position - 1, 1, StreamCellType.Char, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Decimal:
                        position += 16;
                        return new StreamCell(mixStreamReader, position - 16, 16, StreamCellType.Decimal, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Double:
                        position += 8;
                        return new StreamCell(mixStreamReader, position - 8, 8, StreamCellType.Double, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Float:
                        position += 4;
                        return new StreamCell(mixStreamReader, position - 4, 4, StreamCellType.Float, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Int:
                        position += 4;
                        return new StreamCell(mixStreamReader, position - 4, 4, StreamCellType.Int, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Long:
                        position += 8;
                        return new StreamCell(mixStreamReader, position - 8, 8, StreamCellType.Long, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.Short:
                        position += 2;
                        return new StreamCell(mixStreamReader, position - 2, 2, StreamCellType.Short, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.StringWithLength:
                        if (length is int stringLength)
                        {
                            position += stringLength;
                            return new StreamCell(mixStreamReader, position - stringLength, stringLength, StreamCellType.String, logContent.GetConvertor(cell.ConvertorName));
                        }
                        else
                        {
                            return null;
                        }
                    case LogSchemaBinaryType.StringWithIntHead:
                        mixStreamReader.BaseStream.Position = position;
                        var stringHeadLength = mixStreamReader.ReadInt32();
                        position += 4 + stringHeadLength;
                        return new StreamCell(mixStreamReader, position - stringHeadLength, stringHeadLength, StreamCellType.String, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.UInt:
                        position += 4;
                        return new StreamCell(mixStreamReader, position - 4, 4, StreamCellType.UInt, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.ULong:
                        position += 8;
                        return new StreamCell(mixStreamReader, position - 8, 8, StreamCellType.ULong, logContent.GetConvertor(cell.ConvertorName));
                    case LogSchemaBinaryType.UShort:
                        position += 2;
                        return new StreamCell(mixStreamReader, position - 2, 2, StreamCellType.UShort, logContent.GetConvertor(cell.ConvertorName));
                    default:
                        return null;
                }
            }
        }
        #endregion
        protected override BlockContent CreateBlockContent(
            ILogContent logContent,
            MixStreamReader mixStreamReader,
            LogSchemaBinary.BlockBinary block,
            ref long streamPosition)
        {
            return new BlockContentBinary(logContent, mixStreamReader, block, ref streamPosition);
        }
    }
}
