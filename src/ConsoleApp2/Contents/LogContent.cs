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
using VisualLogger.Schemas.Log;

namespace VisualLogger.Contents
{
    public abstract class LogContent<TLogSchema, TBlock, TBody, TCell> : ILogContent
        where TLogSchema : LogSchema<TLogSchema, TBlock, TBody, TCell>
        where TBlock : LogSchema<TLogSchema, TBlock, TBody, TCell>.BlockSchema
        where TBody : LogSchema<TLogSchema, TBlock, TBody, TCell>.BodySchema
        where TCell : LogSchema<TLogSchema, TBlock, TBody, TCell>.CellSchema
    {
        #region Internal Class
        protected struct CellsContent
        {
            public string[] CellNames { get; }
            public StreamCell[] Cells { get; }
            public CellsContent(string[] cellNames, StreamCell[] cells)
            {
                CellNames = cellNames;
                Cells = cells;
            }
        }
        protected struct BodyContent
        {
            public string[] BodyTemplate { get; }
            public StreamCell[][] Body { get; }
            public BodyContent(string[] bodyTemplate, StreamCell[][] body)
            {
                BodyTemplate = bodyTemplate;
                Body = body;
            }
        }
        protected abstract class BlockContent
        {
            public string? Name { get; }
            public CellsContent CellsContent { get; }
            public BodyContent BodyContent { get; }

            public BlockContent(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                TBlock block,
                ref long streamPosition)
            {
                Name = block.Name;
                if (block.Cells is TCell[] cells)
                {
                    var cellsContent = CreateCellsContent(logContent, mixStreamReader, block, cells, ref streamPosition);
                    if (cellsContent != null)
                    {
                        CellsContent = cellsContent.Value;
                    }
                }
                if (block.Body is TBody body)
                {
                    var bodyContent = CreateBodyContent(logContent, mixStreamReader, block, body, ref streamPosition);
                    if (bodyContent != null)
                    {
                        BodyContent = bodyContent.Value;
                    }
                }
            }
            protected abstract CellsContent? CreateCellsContent(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                TBlock block,
                TCell[] cells,
                ref long streamPosition);
            protected abstract BodyContent? CreateBodyContent(
                ILogContent logContent,
                MixStreamReader mixStreamReader,
                TBlock block,
                TBody body,
                ref long streamPosition);
        }
        #endregion
        private readonly Dictionary<string, StreamCellConvertor> _convertors = new();
        private readonly List<BlockContent> _blockContents = new();
        private readonly LogSchema<TLogSchema, TBlock, TBody, TCell> _logSchema;

        protected LogContent(Stream stream, LogSchema<TLogSchema, TBlock, TBody, TCell> logSchema)
        {
            _logSchema = logSchema;
            var mixStreamReader = new MixStreamReader(stream);
            long streamPosition = 0;
            foreach (var block in logSchema.Blocks)
            {
                var blockContent = CreateBlockContent(this, mixStreamReader, block, ref streamPosition);
                _blockContents.Add(blockContent);
            }
        }
        protected abstract BlockContent CreateBlockContent(
            ILogContent logContent,
            MixStreamReader mixStreamReader,
            TBlock block,
            ref long streamPosition);
        public StreamCellConvertor? GetConvertor(string? convertorName)
        {
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
                var convertorSchema = _logSchema.Convertors.FirstOrDefault(c => c.Name == convertorName);
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
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockContents.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            path = paths.Skip(1).FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var index = Array.IndexOf(block.CellsContent.CellNames, path);
            if (index < 0 || index >= block.CellsContent.CellNames.Length)
            {
                return null;
            }
            else
            {
                return block.CellsContent.Cells[index];
            }
        }
        public StreamCell[]? GetCells(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetCells(paths);
        }
        private StreamCell[]? GetCells(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockContents.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            if (block.CellsContent.Cells == null)
            {
                return null;
            }
            return block.CellsContent.Cells;
        }
        public string[]? GetItemsTemplate(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetItemsTemplate(paths);
        }
        private string[]? GetItemsTemplate(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockContents.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            if (block.BodyContent.BodyTemplate == null)
            {
                return null;
            }
            return block.BodyContent.BodyTemplate;
        }
        public StreamCell[][]? GetBodyItems(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetBodyItems(paths);
        }
        private StreamCell[][]? GetBodyItems(IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            var block = _blockContents.FirstOrDefault(b => b.Name == path);
            if (block == null)
            {
                return null;
            }
            if (block.BodyContent.Body == null)
            {
                return null;
            }
            return block.BodyContent.Body;
        }
    }
}
