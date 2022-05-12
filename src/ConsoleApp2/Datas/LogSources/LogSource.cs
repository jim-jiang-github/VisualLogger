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
using VisualLogger.Schemas.Logs;

namespace VisualLogger.Datas.LogSources
{
    public abstract class LogSource<TLogSchema, TBlock, TBody, TCell> :
        LifeCycleTracker<LogSource<TLogSchema, TBlock, TBody, TCell>>,
        ILogSource, 
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
        protected struct BlockSource
        {
            public string Name { get; }
            public BlockCell[] Cells { get; }
            public BlockSource(string name, BlockCell[] cells)
            {
                Name = name;
                Cells = cells;
            }
        }
        protected struct BodySource
        {
            public string[] BodyTemplate { get; }
            public IEnumerable<StreamCell[]> Body { get; }
            public BodySource(string[] bodyTemplate, IEnumerable<StreamCell[]> body)
            {
                BodyTemplate = bodyTemplate;
                Body = body;
            }
        }
        #endregion

        private readonly Dictionary<string, StreamCellConvertor> _convertors;
        private readonly List<BlockSource> _blockSources;
        private readonly BodySource _bodySource;
        private readonly LogSchema<TLogSchema, TBlock, TBody, TCell> _logSchema;

        protected LogSource(Stream stream, LogSchema<TLogSchema, TBlock, TBody, TCell> logSchema)
        {
            _convertors = new();
            _blockSources = new();
            _logSchema = logSchema;
            var mixStreamReader = new MixStreamReader(stream);
            long streamPosition = 0;
            foreach (var block in logSchema.Blocks)
            {
                var blockSource = CreateBlockSource(this, mixStreamReader, block, ref streamPosition);
                _blockSources.Add(blockSource);
            }
            _bodySource = CreateBodySource(this, mixStreamReader, logSchema.Body, ref streamPosition);
        }
        protected abstract BlockSource CreateBlockSource(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            TBlock block,
            ref long streamPosition);
        protected abstract BodySource CreateBodySource(
            ILogSource logSource,
            MixStreamReader mixStreamReader,
            TBody body,
            ref long streamPosition);
        #region ILogSource
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
            if (_blockSources == null)
            {
                return null;
            }
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockSources.FirstOrDefault(b => b.Name == path);

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
            return _bodySource.BodyTemplate;
        }
        public IEnumerable<StreamCell[]> GetBodyItems()
        {
            return _bodySource.Body;
        }
        #endregion
        public void Dispose()
        {
        }
    }
}
