using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using VisualLogger.Datas.LogContents;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Binary
{
    public class BinaryObject
    {
        public class BinaryProperty
        {
            private readonly BinaryObject _rootObject;
            private readonly BinaryParser.PropertyParser _propertyParser;
            private BinaryType _type;
            private string _parameter;

            public string Name { get; private set; }
            public LogCell Value { get; private set; }

            public BinaryProperty(BinaryObject rootObject, BinaryParser.PropertyParser propertyParser)
            {
                _rootObject = rootObject;
                _propertyParser = propertyParser;
            }

            internal void Load(BinaryReader binaryReader)
            {
                Name = _propertyParser.Name;
                _type = _propertyParser.Type;
                _parameter = _propertyParser.Parameter;
                switch (_type)
                {
                    case BinaryType.Skip:
                        if (int.TryParse(_parameter, out int count))
                        {
                            binaryReader.BaseStream.Position += count;
                            Value = null;
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.Boolean:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 1, LogCellType.Boolean);
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Byte:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 1, LogCellType.Byte);
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Char:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 1, LogCellType.Char);
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Decimal:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 16, LogCellType.Decimal);
                        binaryReader.BaseStream.Position += 16;
                        break;
                    case BinaryType.Double:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 8, LogCellType.Double);
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.Float:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 4, LogCellType.Float);
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.Int:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 4, LogCellType.Int);
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.Long:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 8, LogCellType.Long);
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.Short:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 2, LogCellType.Short);
                        binaryReader.BaseStream.Position += 2;
                        break;
                    case BinaryType.String:
                        if (int.TryParse(_parameter, out int stringUTF8Length))
                        {
                            Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, stringUTF8Length, LogCellType.String);
                            binaryReader.BaseStream.Position += stringUTF8Length;
                        }
                        else if (_parameter == "HeadInt")
                        {
                            stringUTF8Length = binaryReader.ReadInt32();
                            Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, stringUTF8Length, LogCellType.String);
                            binaryReader.BaseStream.Position += stringUTF8Length;
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.UInt:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 4, LogCellType.UInt);
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.ULong:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 8, LogCellType.ULong);
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.UShort:
                        Value = new LogCell(binaryReader, binaryReader.BaseStream.Position, 2, LogCellType.UShort);
                        binaryReader.BaseStream.Position += 2;
                        break;
                    default:
                        Debug.Assert(false, "Can not match any type.");
                        break;
                }
            }

            public override string ToString()
            {
                return $"{Name ?? "None"}-{Value}";
            }
        }

        private readonly BinaryObject _rootObject;
        private readonly BinaryParser.ObjectParser _objectParser;
        private readonly List<BinaryProperty> _properties = new List<BinaryProperty>();
        private readonly List<BinaryObject> _subObjects = new List<BinaryObject>();

        private const string ROOT_NAME = "Root";

        public string Name { get; private set; }
        public IReadOnlyList<BinaryProperty> Properties => _properties;
        public IReadOnlyList<BinaryObject> SubObjects => _subObjects;

        public static BinaryObject LoadFromBinaryParser(BinaryParser binaryParser)
        {
            var binaryObject = new BinaryObject(binaryParser);
            return binaryObject;
        }

        private BinaryObject(BinaryParser binaryParser)
        {
            _rootObject = this;
            foreach (var objectParser in binaryParser.Objects)
            {
                var binaryObject = new BinaryObject(this, objectParser);
                _subObjects.Add(binaryObject);
            }
        }
        private BinaryObject(BinaryObject rootObject, BinaryParser.ObjectParser objectParser)
        {
            _rootObject = rootObject;
            _objectParser = objectParser;
        }

        public void LoadFromStream(Stream stream)
        {
            Name = ROOT_NAME;
            stream.Position = 0;
            BinaryReader binaryReader = new BinaryReader(stream);
            foreach (var subObject in _subObjects)
            {
                subObject.Load(binaryReader);
            }
        }
        internal void Load(BinaryReader binaryReader)
        {
            Name = _objectParser.Name;
            if (_objectParser.Properties != null)
            {
                foreach (var properties in _objectParser.Properties)
                {
                    var binaryProperty = new BinaryProperty(_rootObject, properties);
                    binaryProperty.Load(binaryReader);
                    if (binaryProperty.Value != null)
                    {
                        _properties.Add(binaryProperty);
                    }
                }
            }
            if (_objectParser.SubObjects != null)
            {
                int arrayLength = 0;
                var lengthParser = _objectParser.SubObjects.LengthParser;
                if (int.TryParse(lengthParser, out int length))
                {
                    arrayLength = length;
                }
                else
                {
                    var value = GetValueFromRecursivePath(lengthParser.ToString());
                    if (value is LogCell[] streamDataBlocks && streamDataBlocks.Length > 0 && streamDataBlocks[0].GetLogCellValue() is int lengthFromPath)
                    {
                        arrayLength = lengthFromPath;
                    }
                }
                for (int i = 0; i < arrayLength; i++)
                {
                    var subBinaryObject = new BinaryObject(this, _objectParser.SubObjects.Object);
                    subBinaryObject.Load(binaryReader);
                    _subObjects.Add(subBinaryObject);
                }
            }
        }

        public object GetValueFromRecursivePath(string recursivePath)
        {
            var paths = recursivePath.Split(".");
            return GetValueFromRecursivePath(_rootObject, paths);
        }
        private object GetValueFromRecursivePath(BinaryObject parentObject, IEnumerable<string> paths)
        {
            var path = paths.FirstOrDefault();
            if (path == null)
            {
                if (parentObject == null)
                {
                    return null;
                }
                else
                {
                    if (parentObject.Properties.Count == 0)
                    {
                        return parentObject.SubObjects.Select(o => o.Properties.Select(x => x.Value).ToArray());
                    }
                    else
                    {
                        return parentObject.Properties.Select(x => x.Value).ToArray();
                    }
                }
            }
            if (parentObject.Name == path)
            {
                return GetValueFromRecursivePath(parentObject, paths.Skip(1));
            }
            else
            {
                var subObject = parentObject._subObjects.FirstOrDefault(x => x.Name == path);
                if (subObject == null)
                {
                    return new LogCell[] { parentObject._properties.FirstOrDefault(x => x.Name == path)?.Value };
                }
                else
                {
                    return GetValueFromRecursivePath(subObject, paths.Skip(1));
                }
            }
        }
    }
}
