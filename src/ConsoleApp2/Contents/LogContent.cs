using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Datas;
using VisualLogger.InterfaceModules;
using VisualLogger.Schemas.Convertors;
using VisualLogger.Schemas.LogElements;

namespace VisualLogger.Contents
{
    public abstract class LogContent<TLogSchema, TBlock, TBody, TCell> :
        LifeCycleTracker<LogContent<TLogSchema, TBlock, TBody, TCell>>,
        ILogContent, 
        IDisposable
        where TLogSchema : LogSchema<TLogSchema, TBlock, TBody, TCell>, new()
        where TBlock : LogSchema<TLogSchema, TBlock, TBody, TCell>.BlockSchema, new()
        where TBody : LogSchema<TLogSchema, TBlock, TBody, TCell>.BodySchema, new()
        where TCell : LogSchema<TLogSchema, TBlock, TBody, TCell>.CellSchema, new()
    {
        #region Internal Class
        protected struct BlockCell
        {
            public string Name { get; }
            public StreamCell Cell { get; }
            public BlockCell(string name, StreamCell cell)
            {
                Name = name;
                Cell = cell;
            }
        }
        protected struct BlockContent
        {
            public string Name { get; }
            public BlockCell[] Cells { get; }
            public BlockContent(string name, BlockCell[] cells)
            {
                Name = name;
                Cells = cells;
            }
        }
        protected struct BodyContent
        {
            public string[] BodyTemplate { get; }
            public IEnumerable<StreamCell[]> Body { get; }
            public BodyContent(string[] bodyTemplate, IEnumerable<StreamCell[]> body)
            {
                BodyTemplate = bodyTemplate;
                Body = body;
            }
        }
        #endregion

        private readonly Dictionary<string, StreamCellConvertor> _convertors;
        private readonly List<BlockContent> _blockContents;
        private readonly BodyContent _bodyContent;
        private readonly LogSchema<TLogSchema, TBlock, TBody, TCell> _logSchema;

        protected LogContent(Stream stream, LogSchema<TLogSchema, TBlock, TBody, TCell> logSchema)
        {
            _convertors = new();
            _blockContents = new();
            _logSchema = logSchema;
            var mixStreamReader = new MixStreamReader(stream);
            long streamPosition = 0;
            foreach (var block in logSchema.Blocks)
            {
                var blockContent = CreateBlockContent(this, mixStreamReader, block, ref streamPosition);
                _blockContents.Add(blockContent);
            }
            _bodyContent = CreateBodyContent(this, mixStreamReader, logSchema.Body, ref streamPosition);
        }
        protected abstract BlockContent CreateBlockContent(
            ILogContent logContent,
            MixStreamReader mixStreamReader,
            TBlock block,
            ref long streamPosition);
        protected abstract BodyContent CreateBodyContent(
            ILogContent logContent,
            MixStreamReader mixStreamReader,
            TBody body,
            ref long streamPosition);
        #region ILogContent
        public StreamCellConvertor? GetConvertor(string? convertorName)
        {
            if (_convertors == null)
            {
                return null;
            }
            if (convertorName == null)
            {
                return null;
            }
            if (_convertors.TryGetValue(convertorName, out StreamCellConvertor? streamCellConvertor))
            {
                return streamCellConvertor;
            }
            else
            {
                var convertorSchema = _logSchema?.Convertors.FirstOrDefault(c => c.Name == convertorName);
                if (convertorSchema == null)
                {
                    return null;
                }
                streamCellConvertor = StreamCellConvertor.CreateConvertor(this, convertorSchema);
                if (streamCellConvertor == null)
                {
                    return null;
                }
                _convertors.Add(convertorName, streamCellConvertor);
                return streamCellConvertor;
            }
        }
        public StreamCell? GetCell(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetCell(paths);
        }
        private StreamCell? GetCell(IEnumerable<string> paths)
        {
            if (_blockContents == null)
            {
                return null;
            }
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockContents.FirstOrDefault(b => b.Name == path);

            path = paths.Skip(1).FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var index = -1;
            for (int i = 0; i < block.Cells.Length; i++)
            {
                if (block.Cells[i].Name == path)
                {
                    index = i;
                    break;
                }
            }
            if (index < 0)
            {
                return null;
            }
            else
            {
                return block.Cells[index].Cell;
            }
        }
        public string[] GetBodyTemplate()
        {
            return _bodyContent.BodyTemplate;
        }
        public IEnumerable<StreamCell[]> GetBodyItems()
        {
            return _bodyContent.Body;
        }
        #endregion
        public void Dispose()
        {
        }
    }
}
