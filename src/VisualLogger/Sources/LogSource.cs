using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Convertors;
using VisualLogger.Schemas.Logs;
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

        protected readonly Stream _stream;
        protected readonly SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> _schemaLog;
        protected readonly Encoding _encoding;
        protected readonly CellConvertorProvider _convertorProvider;

        private readonly List<BlockSource> _blockSources = new();
        private int _totalCount = 0;
        private ContentSource? _logContent;
        private readonly WordsCollection _wordsCollection = new();
        ~LogSource()
        {

        }

        protected LogSource(Stream stream, SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema> schemaLog)
        {
            _stream = stream;
            _schemaLog = schemaLog;
            _encoding = Encoding.GetEncoding(schemaLog.EncodingName);
            _convertorProvider = new CellConvertorProvider(schemaLog);
            Filter = new LogFilter();
        }
        protected void Init()
        {
            foreach (var block in _schemaLog.Blocks)
            {
                var blockSource = CreateBlockSource(block);
                _blockSources.Add(blockSource);
            }
            _totalCount = GetTotalCount(this);
            _logContent = CreateContentSource(this);
            _convertorProvider.Init(this);
        }
        protected abstract BlockSource CreateBlockSource(
            TBlockSchema block);
        protected abstract int GetTotalCount(
            IBlockCellFinder blockCellFinder);
        protected abstract ContentSource CreateContentSource(
            IBlockCellFinder blockCellFinder);
        protected void HandleContentCellValue(SchemaLog<TBlockSchema, TColumnHeadSchema, TCellSchema>.SchemaColumn schemaColumn, object cell)
        {
            //if (schemaColumn.Enumeratable)
            //{
            //    var cellValue = logSourceReader.GetValue(cellSource, cellIndex);
            //    _wordsCollection.AppendFromString(cellValue);
            //}
        }
        #region IBlockCellFinder
        public object? GetBlockCellValue(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetBlockCellValue(paths);
        }
        private object? GetBlockCellValue(IEnumerable<string> paths)
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
        public int TotalRowsCount => _totalCount;
        public string[] ColumnNames => _logContent?.ColumnHeadTemplate ?? Array.Empty<string>();
        public IEnumerable<string> EnumerateWords => _wordsCollection.Words;
        public LogFilter Filter { get; }
        public IEnumerable<LogRow> GetRows(int start, int length)
        {
            if (_logContent == null)
            {
                yield break;
            }
            foreach (var row in _logContent.GetRows(start, length))
            {
                yield return row;
            }
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
