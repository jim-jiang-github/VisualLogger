using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ConsoleApp1
{
    public class BinaryLoader
    {
        public class BinaryObject
        {
            private BinaryLoader _binaryLoader;
            private BinaryDescription.ObjectDescription _objectDescription;
            private List<BinaryProperty> binaryProperties = new List<BinaryProperty>();
            private List<BinaryObject> subBinaryObjects = new List<BinaryObject>();

            public string Name { get; private set; }
            public IReadOnlyList<BinaryProperty> Properties => binaryProperties;
            public IReadOnlyList<BinaryObject> SubObjects => subBinaryObjects;

            public BinaryObject(BinaryLoader binaryLoader, BinaryDescription.ObjectDescription objectDescription)
            {
                _binaryLoader = binaryLoader;
                _objectDescription = objectDescription;
            }

            public void Load(BinaryReader binaryReader)
            {
                Name = _objectDescription.Name;
                if (_objectDescription.Array == null)
                {
                    foreach (var propertyDescription in _objectDescription.Properties)
                    {
                        var binaryProperty = new BinaryProperty(_binaryLoader, propertyDescription);
                        binaryProperty.Load(binaryReader);
                        binaryProperties.Add(binaryProperty);
                    }
                }
                else
                {
                    var arrayLength = _objectDescription.Array.Length;
                    if (arrayLength == null)
                    {
                        var fromObjectName = _objectDescription.Array.FromObjectName;
                        var fromObject = _binaryLoader.binaryObjects.Find(x => x.Name == fromObjectName);
                        if (fromObject == null)
                        {
                            return;
                        }
                        var fromPropertyName = _objectDescription.Array.FromPropertyName;
                        var fromProperty = fromObject.binaryProperties.Find(x => x.Name == fromPropertyName);
                        if (fromProperty.Value is int)
                        {
                            arrayLength = (int)fromProperty.Value;
                        }
                        else
                        {
                            return;
                        }
                    }
                    for (int i = 0; i < arrayLength; i++)
                    {
                        var subBinaryObject = new BinaryObject(_binaryLoader, _objectDescription.Array.ArrayItem);
                        subBinaryObject.Load(binaryReader);
                        subBinaryObjects.Add(subBinaryObject);
                    }
                }
            }
        }
        public class BinaryProperty
        {
            private BinaryLoader _binaryLoader;
            private BinaryDescription.PropertyDescription _propertyDescription;
            private BinaryType _type;
            private int? _parameter;

            public string Name { get; private set; }
            public object Value { get; private set; }

            public BinaryProperty(BinaryLoader binaryLoader, BinaryDescription.PropertyDescription propertyDescription)
            {
                _binaryLoader = binaryLoader;
                _propertyDescription = propertyDescription;
            }

            internal void Load(BinaryReader binaryReader)
            {
                Name = _propertyDescription.Name;
                _type = _propertyDescription.Type;
                _parameter = _propertyDescription.Length;
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
                            Encoding.Unicode.GetString(binaryReader.ReadBytes(stringUnicodeLength));
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.StringUnicodeWithIntHead:
                        Value = Encoding.Unicode.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32()));
                        break;
                    case BinaryType.StringUTF8:
                        if (_parameter is int stringUTF8Length)
                        {
                            Value = Encoding.UTF8.GetString(binaryReader.ReadBytes(stringUTF8Length));
                        }
                        else
                        {
                            Debug.Assert(_parameter != null, "Parameter should not be null.");
                        }
                        break;
                    case BinaryType.StringUTF8WithIntHead:
                        Value = Encoding.UTF8.GetString(binaryReader.ReadBytes(binaryReader.ReadInt32()));
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

        private List<BinaryObject> binaryObjects = new List<BinaryObject>();

        public IReadOnlyList<BinaryObject> Objects => binaryObjects;

        public static BinaryLoader LoadFromBinaryDescription(string jsonFile)
        {
            if (!File.Exists(jsonFile))
            {
                return null;
            }
            var jsonContent = File.ReadAllText(jsonFile);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new LowerCaseNamingPolicy(),
                WriteIndented = true
            };
            try
            {
                var binaryDescription = JsonSerializer.Deserialize<BinaryDescription>(jsonContent, options);
                return new BinaryLoader(binaryDescription);
            }
            catch
            {
                return null;
            }
        }

        private BinaryLoader(BinaryDescription binaryDescription)
        {
            foreach (var objectDescription in binaryDescription.Objects)
            {
                var binaryObject = new BinaryObject(this, objectDescription);
                binaryObjects.Add(binaryObject);
            }
        }

        public void LoadFromStream(Stream stream)
        {
            BinaryReader binaryReader = new BinaryReader(stream);
            foreach (var binaryObject in binaryObjects)
            {
                binaryObject.Load(binaryReader);
            }
        }
    }

    public enum BinaryType
    {
        Skip,
        Boolean,
        Byte,
        Char,
        Short,
        Int,
        Long,
        UShort,
        UInt,
        ULong,
        Float,
        Double,
        Decimal,
        StringUTF8,
        StringUnicode,
        StringUTF8WithIntHead,
        StringUnicodeWithIntHead,
    }

    public class BinaryDescription
    {
        public class ArrayDescription
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string FromObjectName { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string FromPropertyName { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Length { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ObjectDescription ArrayItem { get; set; }
        }
        public class ObjectDescription
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public List<PropertyDescription> Properties { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public ArrayDescription Array { get; set; }
        }
        public class PropertyDescription
        {
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public string Name { get; set; }
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public BinaryType Type { get; set; }
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            public int? Length { get; set; } = null;

            public override string ToString()
            {
                return $"{Name}-{Type}";
            }
        }

        public List<ObjectDescription> Objects { get; set; } = new List<ObjectDescription>();

        public BinaryDescription()
        {
            //var logFileHeader = new ObjectDescription()
            //{
            //    Name = "LogFileHeader"
            //};
            //logFileHeader.Properties = new List<PropertyDescription>();
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "Signature",
            //    Type = BinaryType.StringUTF8,
            //    Length = 3
            //});
            ////self.bom = bytes().join(struct.unpack('2c', buf[3:5]))
            ////self.os,self.logVersion,self.encode = struct.unpack('3c', buf[5:8])
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Type = BinaryType.Skip,
            //    Length = 2 + 3
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "MagicNumber",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "EncryptKey",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "SummarySize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "FileMaxSize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Name = "FileCurSize",
            //    Type = BinaryType.Int
            //});
            //logFileHeader.Properties.Add(new PropertyDescription
            //{
            //    Type = BinaryType.Skip,
            //    Length = 32
            //});

            //Objects.Add(logFileHeader);

            //var logSummary = new ObjectDescription()
            //{
            //    Name = "LogSummary"
            //};
            //logSummary.Properties = new List<PropertyDescription>();
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "TimeZone",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Type = BinaryType.Skip,
            //    Length = 4
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "StartTime",
            //    Type = BinaryType.Long
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "StartTimeMS",
            //    Type = BinaryType.UInt
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "ProcessName",
            //    Type = BinaryType.StringUTF8,
            //    Length = 256
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "ProcessId",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Name = "ItemCount",
            //    Type = BinaryType.Int
            //});
            //logSummary.Properties.Add(new PropertyDescription
            //{
            //    Type = BinaryType.Skip,
            //    Length = 32 + 4
            //});
            //Objects.Add(logSummary);

            //var arrayItem = new ObjectDescription()
            //{
            //    Name = "LogItem"
            //};
            //arrayItem.Properties = new List<PropertyDescription>();
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "TickOffset",
            //    Type = BinaryType.Long
            //});
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "ModuleName",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "ThreadId",
            //    Type = BinaryType.Int
            //});
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "Level",
            //    Type = BinaryType.Int
            //});
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "Hint",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});
            //arrayItem.Properties.Add(new PropertyDescription
            //{
            //    Name = "Msg",
            //    Type = BinaryType.StringUTF8WithIntHead
            //});

            //var array = new ObjectDescription()
            //{
            //    Array = new ArrayDescription()
            //    {
            //        FromObjectName = "LogSummary",
            //        FromPropertyName = "ItemCount",
            //        ArrayItem = arrayItem
            //    }
            //};
            //Objects.Add(array);

            //var options = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = new LowerCaseNamingPolicy(),
            //    WriteIndented = true
            //};
            //var a = System.Text.Json.JsonSerializer.Serialize(this, options);
        }
    }
    public class LowerCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            name.ToLower();
    }
}
