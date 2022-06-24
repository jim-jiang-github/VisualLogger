using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
using VisualLogger.Sources.Text;
using VisualLogger.Streams;

namespace VisualLogger.Sources
{
    internal abstract class LogSource<TBlockSchema, TColumnHeadSchema, TCellSchema> :
        IBlockCellFinder,
        ILogSource
        where TBlockSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaBlock, new()
        where TColumnHeadSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaColumnHead, new()
        where TCellSchema : SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaCell, new()
    {
        private readonly List<BlockSource> _blockSources;
        private readonly WordsCollection _wordsCollection;
        protected readonly Stream _stream;
        protected readonly SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> _schemaLog;
        protected readonly CellConvertorProvider _convertorProvider;
        protected readonly ContentSource _logContent;
        protected LogSource(Stream stream, SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> schemaLog)
        {
            _schemaLog = schemaLog;
            _blockSources = new();
            _stream = stream;
            _wordsCollection = new WordsCollection();
            _convertorProvider = new CellConvertorProvider(schemaLog);
            Filter = new LogFilter();
            Init(stream, schemaLog);
            long streamPosition = 0;
            foreach (var block in _schemaLog.Blocks)
            {
                var blockSource = CreateBlockSource(block, ref streamPosition);
                _blockSources.Add(blockSource);
            }
            _convertorProvider.Init(this);
            _logContent = CreateContentSource(this, ref streamPosition);
        }
        protected abstract void Init(Stream stream, SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> schemaLog);
        protected abstract BlockSource CreateBlockSource(
            TBlockSchema block,
            ref long streamPosition);
        protected abstract ContentSource CreateContentSource(
            IBlockCellFinder blockCellSearchable,
            ref long streamPosition);
        protected void HandleContentCellValue(SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaColumn schemaColumn, LogSourceReader logSourceReader, CellSource cellSource, int cellIndex)
        {
            //if (schemaColumn.Enumeratable)
            //{
            //    var cellValue = logSourceReader.GetValue(cellSource, cellIndex);
            //    _wordsCollection.AppendFromString(cellValue);
            //}
        }
        #region IBlockCellFinder
        public string? GetBlockCellValue(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetBlockCellValue(paths);
        }
        private string? GetBlockCellValue(IEnumerable<string> paths)
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
            for (int i = 0; i < block.Count; i++)
            {
                if (block.GetCellName(i) == path)
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
                return block[index];
            }
        }
        #endregion
        #region ILogSource
        public int TotalRowsCount { get; protected set; }
        public string[] ColumnNames => _logContent.ColumnHeadTemplate;
        public IEnumerable<string> EnumerateWords => _wordsCollection.Words;
        public LogFilter Filter { get; }
        public IEnumerable<LogRow> GetRows(int start, int length)
        {
            var rows = _logContent.GetRows(start, length);
            return rows;
        }
        #endregion
        #region IDisposable
        public virtual void Dispose()
        {
            _stream.Close();
        }
        #endregion
    }
}
