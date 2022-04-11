using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryObject
    {
        private const string ROOT_NAME = "Root";
        private readonly BinaryObject _rootObject;
        private string _name;
        private readonly List<string> _propertyNames = new List<string>();
        private readonly List<StreamCell> _cells = new List<StreamCell>();
        private readonly List<string> _subObjectPropertyNames = new List<string>();
        private readonly List<BinaryObject> _subObjects = new List<BinaryObject>();

        public static BinaryObject Load(Stream stream, BinaryContentParser binaryContentParser)
        {
            if (stream == null || binaryContentParser == null)
            {
                return null;
            }
            var binaryObject = new BinaryObject(stream, binaryContentParser);
            return binaryObject;
        }

        private BinaryObject(Stream stream, BinaryContentParser binaryContentParser)
        {
            _rootObject = this;
            _name = ROOT_NAME;
            long position = 0;
            BinaryReader binaryReader = new BinaryReader(stream);
            foreach (var objectParser in binaryContentParser.Blocks)
            {
                var binaryObject = new BinaryObject(_rootObject, binaryReader, objectParser, ref position);
                _subObjects.Add(binaryObject);
            }
        }
        private BinaryObject(BinaryObject rootObject, BinaryReader binaryReader, BinaryContentParser.Block objectParser, ref long position)
        {
            //_name = objectParser.Name;
            //_rootObject = rootObject;
            //if (objectParser.Item != null)
            //{
            //    if (objectParser.PropertyNames.Length == objectParser.Item.Length)
            //    {
            //        for (int i = 0; i < objectParser.Item.Length; i++)
            //        {
            //            var property = objectParser.Item[i];
            //            var binaryProperty = CreateStreamCell(binaryReader, property, ref position);
            //            if (binaryProperty != null)
            //            {
            //                _propertyNames.Add(objectParser.PropertyNames[i]);
            //                _properties.Add(binaryProperty);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //TODO log
            //    }
            //}
            //if (objectParser.Items != null)
            //{
            //    int subObjectCount = 0;
            //    var countParser = objectParser.Items.Count;
            //    if (int.TryParse(countParser, out int count))
            //    {
            //        subObjectCount = count;
            //    }
            //    else
            //    {
            //        var streamCell = GetStreamCellFromRecursivePath(countParser);
            //        if (streamCell.GetValue() is int countFromPath)
            //        {
            //            subObjectCount = countFromPath;
            //        }
            //    }
            //    if (objectParser.SubObjectPropertyNames.Length == subObjectCount)
            //    {
            //        for (int i = 0; i < subObjectCount; i++)
            //        {
            //            var subBinaryObject = new BinaryObject(rootObject, binaryReader, objectParser.Items.Object, ref position);
            //            subBinaryObject._subObjectPropertyNames
            //            _subObjects.Add(subBinaryObject);
            //        }
            //    }
            //}
        }

        private StreamCell CreateStreamCell(BinaryReader binaryReader, BinaryContentParser.Cell propertyParser, ref long position)
        {
            var type = propertyParser.Type;
            var length = propertyParser.Length;
            switch (type)
            {
                case BinaryType.Skip:
                    if (length is int count)
                    {
                        position += count;
                    }
                    return null;
                case BinaryType.Boolean:
                    position += 1;
                    return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Boolean);
                case BinaryType.Byte:
                    position += 1;
                    return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Byte);
                case BinaryType.Char:
                    position += 1;
                    return new StreamCell(binaryReader, position - 1, 1, StreamCellType.Char);
                case BinaryType.Decimal:
                    position += 16;
                    return new StreamCell(binaryReader, position - 16, 16, StreamCellType.Decimal);
                case BinaryType.Double:
                    position += 8;
                    return new StreamCell(binaryReader, position - 8, 8, StreamCellType.Double);
                case BinaryType.Float:
                    position += 4;
                    return new StreamCell(binaryReader, position - 4, 4, StreamCellType.Float);
                case BinaryType.Int:
                    position += 4;
                    return new StreamCell(binaryReader, position - 4, 4, StreamCellType.Int);
                case BinaryType.Long:
                    position += 8;
                    return new StreamCell(binaryReader, position - 8, 8, StreamCellType.Long);
                case BinaryType.Short:
                    position += 2;
                    return new StreamCell(binaryReader, position - 2, 2, StreamCellType.Short);
                case BinaryType.StringWithLength:
                    if (length is int stringLength)
                    {
                        position += stringLength;
                        return new StreamCell(binaryReader, position - stringLength, stringLength, StreamCellType.String);
                    }
                    else
                    {
                        return null;
                    }
                case BinaryType.StringWithIntHead:
                    binaryReader.BaseStream.Position = position;
                    var stringHeadLength = binaryReader.ReadInt32();
                    position += 4 + stringHeadLength;
                    return new StreamCell(binaryReader, position - stringHeadLength, stringHeadLength, StreamCellType.String);
                case BinaryType.UInt:
                    position += 4;
                    return new StreamCell(binaryReader, position - 4, 4, StreamCellType.UInt);
                case BinaryType.ULong:
                    position += 8;
                    return new StreamCell(binaryReader, position - 8, 8, StreamCellType.ULong);
                case BinaryType.UShort:
                    position += 2;
                    return new StreamCell(binaryReader, position - 2, 2, StreamCellType.UShort);
                default:
                    Debug.Assert(false, "Can not match any type.");
                    return null;
            }
        }

        public StreamCell GetStreamCellFromRecursivePath(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetStreamCellFromRecursivePath(_rootObject, paths);
        }

        private StreamCell GetStreamCellFromRecursivePath(BinaryObject parentObject, IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return null;
            }
            if (parentObject._name == path)
            {
                return GetStreamCellFromRecursivePath(parentObject, paths.Skip(1));
            }
            else
            {
                var index = parentObject._propertyNames.IndexOf(path);
                if (index < 0 || index >= parentObject._cells.Count)
                {
                    var subObject = parentObject._subObjects.FirstOrDefault(x => x._name == path);
                    if (subObject == null)
                    {
                        return null;
                    }
                    else
                    {
                        return GetStreamCellFromRecursivePath(subObject, paths.Skip(1));
                    }
                }
                else
                {
                    var property = parentObject._cells[index];
                    return property;
                }
            }
        }

        public IEnumerable<StreamCell> GetObjectFromRecursivePath(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetObjectFromRecursivePath(_rootObject, paths);
        }

        private IEnumerable<StreamCell> GetObjectFromRecursivePath(BinaryObject parentObject, IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return parentObject._cells.AsEnumerable();
            }
            if (parentObject._name == path)
            {
                return GetObjectFromRecursivePath(parentObject, paths.Skip(1));
            }
            else
            {
                var subObject = parentObject._subObjects.FirstOrDefault(x => x._name == path);
                if (subObject == null)
                {
                    return null;
                }
                else
                {
                    return GetObjectFromRecursivePath(subObject, paths.Skip(1));
                }
            }
        }

        public IEnumerable<IEnumerable<StreamCell>> GetSubObjectsFromRecursivePath(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetSubObjectsFromRecursivePath(_rootObject, paths);
        }

        private IEnumerable<IEnumerable<StreamCell>> GetSubObjectsFromRecursivePath(BinaryObject parentObject, IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                return parentObject._subObjects.Select(s => s._cells.AsEnumerable());
            }
            if (parentObject._name == path)
            {
                return GetSubObjectsFromRecursivePath(parentObject, paths.Skip(1));
            }
            else
            {
                var subObject = parentObject._subObjects.FirstOrDefault(x => x._name == path);
                if (subObject == null)
                {
                    return null;
                }
                else
                {
                    return GetSubObjectsFromRecursivePath(subObject, paths.Skip(1));
                }
            }
        }
    }
}
