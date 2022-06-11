using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Core.Convertors;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Streams;

namespace VisualLogger.Core.Sources
{
    internal abstract class LogSource<TBlockSchema, TColumnHeadSchema, TCellSchema> :
        ILogSource,
        IDisposable
        where TBlockSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaBlock, new()
        where TColumnHeadSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaColumnHead, new()
        where TCellSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaCell, new()
    {
        #region Internal Class
        protected struct CellSource
        {
            public string Name { get; }
            public StreamCell Cell { get; }
            public CellSource(string name, StreamCell cell)
            {
                Name = name;
                Cell = cell;
            }
        }
        protected struct BlockSource
        {
            public string Name { get; }
            public CellSource[] Cells { get; }
            public BlockSource(string name, CellSource[] cells)
            {
                Name = name;
                Cells = cells;
            }
        }
        protected struct ColumnHeadSource
        {
            public string[] ColumnHeadTemplate { get; }
            public IEnumerable<StreamCell[]> Cells { get; }
            public ColumnHeadSource(string[] columnHeadTemplate, IEnumerable<StreamCell[]> cells)
            {
                ColumnHeadTemplate = columnHeadTemplate;
                Cells = cells;
            }
        }
        #endregion

        private readonly CellConvertorProvider _cellConvertorProvider;
        private readonly List<BlockSource> _blockSources;
        private readonly ColumnHeadSource _bodySource;

        protected LogSource(Stream stream, SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> schemaLog)
        {
            _cellConvertorProvider = new(this, schemaLog);
            _blockSources = new();
            var mixStreamReader = new MixStreamReader(stream);
            long streamPosition = 0;
            foreach (var block in schemaLog.Blocks)
            {
                var blockSource = CreateBlockSource(mixStreamReader, this, block, _cellConvertorProvider, ref streamPosition);
                _blockSources.Add(blockSource);
            }
            _bodySource = CreateContentSource(mixStreamReader, this, schemaLog.ColumnHeadTemplate, _cellConvertorProvider, ref streamPosition);
        }
        protected abstract BlockSource CreateBlockSource(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            TBlockSchema block,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition);
        protected abstract ColumnHeadSource CreateContentSource(
            MixStreamReader mixStreamReader,
            ILogSource logSource,
            TColumnHeadSchema columnHead,
            CellConvertorProvider cellConvertorProvider,
            ref long streamPosition);
        #region ILogSource
        public long TotalRowsCount { get; protected set; }
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
        public string[] GetColumnHead()
        {
            return _bodySource.ColumnHeadTemplate;
        }
        public IEnumerable<StreamCell[]> GetRows()
        {
            return _bodySource.Cells;
        }
        #endregion
        #region IDisposable
        public void Dispose()
        {
        }
        #endregion
    }
}
