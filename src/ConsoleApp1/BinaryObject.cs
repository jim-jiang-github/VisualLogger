using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using static ConsoleApp1.LogContent;

namespace ConsoleApp1
{
    public class BinaryObject
    {
        public class BinaryProperty
        {
            private BinaryObject _rootObject;
            private BinaryParser.PropertyParser _propertyParser;
            private BinaryType _type;
            private int? _parameter;

            public string Name { get; private set; }
            public StreamDataBlock Value { get; private set; }

            public BinaryProperty(BinaryObject rootObject, BinaryParser.PropertyParser propertyParser)
            {
                _rootObject = rootObject;
                _propertyParser = propertyParser;
            }

            internal void Load(BinaryReader binaryReader)
            {
                Name = _propertyParser.Name;
                _type = _propertyParser.Type;
                _parameter = _propertyParser.Length;
                switch (_type)
                {
                    case BinaryType.Skip:
                        if (_parameter is int count)
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
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 1, typeof(bool));
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Byte:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 1, typeof(byte));
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Char:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 1, typeof(char));
                        binaryReader.BaseStream.Position += 1;
                        break;
                    case BinaryType.Decimal:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 16, typeof(decimal));
                        binaryReader.BaseStream.Position += 16;
                        break;
                    case BinaryType.Double:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 8, typeof(double));
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.Float:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 4, typeof(float));
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.Int:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 4, typeof(int));
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.Long:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 8, typeof(long));
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.Short:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 2, typeof(short));
                        binaryReader.BaseStream.Position += 2;
                        break;
                    case BinaryType.StringUTF8:
                        if (_parameter is int stringUTF8Length)
                        {
                            Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, stringUTF8Length, typeof(string));
                            binaryReader.BaseStream.Position += stringUTF8Length;
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.StringUTF8WithIntHead:
                        var length = binaryReader.ReadInt32();
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, length, typeof(string));
                        binaryReader.BaseStream.Position += length;
                        break;
                    case BinaryType.UInt:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 4, typeof(uint));
                        binaryReader.BaseStream.Position += 4;
                        break;
                    case BinaryType.ULong:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 8, typeof(ulong));
                        binaryReader.BaseStream.Position += 8;
                        break;
                    case BinaryType.UShort:
                        Value = new StreamDataBlock(binaryReader, binaryReader.BaseStream.Position, 2, typeof(ushort));
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

        private BinaryObject _rootObject;
        private BinaryParser.ObjectParser _objectDescription;
        private List<BinaryProperty> _properties = new List<BinaryProperty>();
        private List<BinaryObject> _subObjects = new List<BinaryObject>();

        private const string ROOT_NAME = "Root";

        public string Name { get; private set; }
        public IReadOnlyList<BinaryProperty> Properties => _properties;
        public IReadOnlyList<BinaryObject> SubObjects => _subObjects;

        public static BinaryObject LoadFromBinaryDescription(BinaryParser binaryParser)
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
            _objectDescription = objectParser;
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
            Name = _objectDescription.Name;
            if (_objectDescription.Array == null)
            {
                foreach (var propertyDescription in _objectDescription.Properties)
                {
                    var binaryProperty = new BinaryProperty(_rootObject, propertyDescription);
                    binaryProperty.Load(binaryReader);
                    if (binaryProperty.Value != null)
                    {
                        _properties.Add(binaryProperty);
                    }
                }
            }
            else
            {
                int arrayLength = 0;
                var lengthDescription = _objectDescription.Array.LengthParser;
                if (int.TryParse(lengthDescription, out int length))
                {
                    arrayLength = length;
                }
                else
                {
                    var value = GetValueFromRecursivePath(lengthDescription);
                    if (value is StreamDataBlock[] streamDataBlocks && streamDataBlocks.Length > 0 && streamDataBlocks[0].PopData() is int lengthFromPath)
                    {
                        arrayLength = lengthFromPath;
                    }
                }
                for (int i = 0; i < arrayLength; i++)
                {
                    var subBinaryObject = new BinaryObject(this, _objectDescription.Array.ArrayItem);
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
                    return new StreamDataBlock[] { parentObject._properties.FirstOrDefault(x => x.Name == path)?.Value };
                }
                else
                {
                    return GetValueFromRecursivePath(subObject, paths.Skip(1));
                }
            }

        }
    }
}
