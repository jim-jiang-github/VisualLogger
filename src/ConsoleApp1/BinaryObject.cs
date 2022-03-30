using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
            public object Value { get; private set; }

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
                            binaryReader.ReadBytes(count);
                            Value = $"Skip {count} byte.";
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.Boolean:
                        Value = binaryReader.ReadBoolean();
                        break;
                    case BinaryType.Byte:
                        Value = binaryReader.ReadByte();
                        break;
                    case BinaryType.Char:
                        Value = binaryReader.ReadChar();
                        break;
                    case BinaryType.Decimal:
                        Value = binaryReader.ReadDecimal();
                        break;
                    case BinaryType.Double:
                        Value = binaryReader.ReadDouble();
                        break;
                    case BinaryType.Float:
                        Value = binaryReader.ReadSingle();
                        break;
                    case BinaryType.Int:
                        Value = binaryReader.ReadInt32();
                        break;
                    case BinaryType.Long:
                        Value = binaryReader.ReadInt64();
                        break;
                    case BinaryType.Short:
                        Value = binaryReader.ReadInt16();
                        break;
                    case BinaryType.StringUnicode:
                        if (_parameter is int stringUnicodeLength)
                        {
                            Value = Encoding.Unicode.GetString(binaryReader.ReadBytes(stringUnicodeLength)).Trim();
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.StringUnicodeWithIntHead:
                        Value = Encoding.Unicode.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32())).Trim();
                        break;
                    case BinaryType.StringUTF8:
                        if (_parameter is int stringUTF8Length)
                        {
                            Value = Encoding.UTF8.GetString(binaryReader.ReadBytes(stringUTF8Length)).Trim();
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.StringUTF8WithIntHead:
                        Value = Encoding.UTF8.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32())).Trim();
                        break;
                    case BinaryType.UInt:
                        Value = binaryReader.ReadUInt32();
                        break;
                    case BinaryType.ULong:
                        Value = binaryReader.ReadUInt64();
                        break;
                    case BinaryType.UShort:
                        Value = binaryReader.ReadUInt16();
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
                    _properties.Add(binaryProperty);
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
                    if (value is int lengthFromPath)
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
                        return parentObject.SubObjects.Select(o => o.Properties.Select(x => x.Value));
                    }
                    else
                    {
                        return parentObject.Properties.Select(x => x.Value);
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
                    return parentObject._properties.FirstOrDefault(x => x.Name == path)?.Value;
                }
                else
                {
                    return GetValueFromRecursivePath(subObject, paths.Skip(1));
                }
            }

        }
    }
}
