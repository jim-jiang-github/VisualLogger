using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisualLogger.Datas;

namespace VisualLogger.InterfaceImplModules.LogContentLoaders.Text
{
    public class FileObject
    {
        private class FileProperty
        {
            public string Name { get; }
            public StreamCell StreamCell { get; }

            public FileProperty(BinaryReader binaryReader, string name, long position, int length, StreamCellType streamCellType)
            {
                Name = name;
                StreamCell = new StreamCell(binaryReader, position, length, streamCellType);
            }
        }

        private const string ROOT_NAME = "Root";
        private FileObject _rootObject;
        private string _name;
        private readonly List<FileProperty> _properties = new List<FileProperty>();
        private readonly List<FileObject> _subObjects = new List<FileObject>();

        public static FileObject Load(Stream stream, TextParser textParser)
        {
            if (stream == null || textParser == null)
            {
                return null;
            }
            var fileObject = new FileObject(stream, textParser);
            return fileObject;
        }
        private FileObject(Stream stream, TextParser textParser)
        {
            _rootObject = this;
            _name = ROOT_NAME;
            long position = 0;
            BinaryReader binaryReader = new BinaryReader(stream);
            StreamReader streamReader = new StreamReader(stream);
            foreach (var objectParser in textParser.Objects)
            {
                var binaryObject = new FileObject(_rootObject, streamReader, objectParser, ref position);
                _subObjects.Add(binaryObject);
            }

            //Regex regexHead = new Regex(@"app: (RoomsController)/([^\s]+)/(SHA)\(\)");
            //Regex regexOS = new Regex(@"os: iOS/([^\s]+)");
            //Regex regexItemTime = new Regex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}");
            //Regex regexLevel = new Regex(@" : ([^\s]+) : ");
            //Regex regexModule = new Regex(@"\[([^\s]+)\]");
            //Regex regexItemInfo = new Regex(@"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}) : ([^\s]+) : (\[.*?\]) (\[.*?\]) (.*)");
            //while (streamReader.ReadLine() is string line && line != null)
            //{
            //    var a = line;
            //    var m = regexHead.Match(line);
            //    var m1 = regexOS.Match(line);
            //    var m2 = regexItemInfo.Match(line);


            //    //int position = 0;
            //    //var m2 = regexItemTime.Matches(line);
            //    //var time = m2[0].Groups[0].Value;
            //    //position += m2[0].Index + m2[0].Length;
            //    //var m3 = regexLevel.Matches(line);
            //    //var level = m3[0].Groups[1].Value;
            //    //position += m3[0].Index + m3[0].Length;
            //    //var m4 = regexModule.Matches(line);
            //    //var m5 = regexModule.Matches(line);
            //}
        }
        private FileObject(FileObject rootObject, StreamReader streamReader, TextParser.ObjectParser objectParser, ref long position)
        {
            _name = objectParser.Name;
            _rootObject = rootObject;
            if (objectParser.Properties != null)
            {
                foreach (var property in objectParser.Properties)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    bool isItemCreating = false;
                    while (streamReader.ReadLine() is string line)
                    {
                        if (!isItemCreating && Regex.IsMatch(line, objectParser.RegexPatternLineStart))
                        {
                            stringBuilder.AppendLine(line);
                            isItemCreating = true;
                        }
                        else if (Regex.IsMatch(line, objectParser.RegexPatternLineContent))
                        {
                            stringBuilder.AppendLine(line);
                        }
                        else
                        {
                            var item = stringBuilder.ToString();
                            var qwe = Regex.Match(item, objectParser.RegexPattern);
                            //var fileProperty = FileProperty.Load(streamReader, objectParser.RegexPattern, property, ref position);
                            //var binaryProperty = BinaryProperty.Load(binaryReader, properties, ref position);
                            //if (binaryProperty != null)
                            //{
                            //    _properties.Add(binaryProperty);
                            //}
                        }
                    }
                    //var fileProperty = FileProperty.Load(streamReader, objectParser.RegexPattern, property, ref position);
                    //var binaryProperty = BinaryProperty.Load(binaryReader, properties, ref position);
                    //if (binaryProperty != null)
                    //{
                    //    _properties.Add(binaryProperty);
                    //}
                }
            }
            if (objectParser.SubObjects != null)
            {
                //int arrayLength = 0;
                //var lengthParser = objectParser.SubObjects.LengthParser;
                //if (int.TryParse(lengthParser, out int length))
                //{
                //    arrayLength = length;
                //}
                //else
                //{
                //    var value = GetValueFromRecursivePath(binaryReader, lengthParser);
                //    if (value is int lengthFromPath)
                //    {
                //        arrayLength = lengthFromPath;
                //    }
                //}
                //for (int i = 0; i < arrayLength; i++)
                //{
                //    var subBinaryObject = new BinaryObject(rootObject, binaryReader, objectParser.SubObjects.Object, ref position);
                //    _subObjects.Add(subBinaryObject);
                //}
            }
        }
        //private FileObject(StreamReader streamReader)
        //{
        //    string delimiterChars = "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_";
        //    char[] breakChars = { '\r', '\n' };

        //    var a = "x_platform_application.cpp:157 \ttyuty";
        //    Dictionary<string, List<int>> wordMap = new Dictionary<string, List<int>>();
        //    StringBuilder stringBuilder = new StringBuilder();
        //    List<string> words = new List<string>();
        //    int index = 0;

        //    var asd = streamReader.ReadChars((int)(streamReader.BaseStream.Length / 2));
        //    foreach (var c in asd)
        //    {
        //        if (delimiterChars.Contains(c))
        //        {
        //            stringBuilder.Append(c);
        //        }
        //        else
        //        {
        //            if (stringBuilder.Length > 0)
        //            {
        //                var word = stringBuilder.ToString();
        //                if (wordMap.TryGetValue(word, out List<int> indexs))
        //                {
        //                    indexs.Add(index);
        //                }
        //                else
        //                {
        //                    wordMap.Add(word, new List<int>() { index });
        //                }
        //                index++;
        //                stringBuilder.Clear();
        //            }
        //        }
        //    }
        //    while (streamReader.BaseStream.Position != streamReader.BaseStream.Length - 1)
        //    {
        //        var c = streamReader.ReadByte();
        //        //if (delimiterChars.Contains(c))
        //        //{
        //        //    stringBuilder.Append(c);
        //        //}
        //        //else
        //        //{
        //        //    if (stringBuilder.Length > 0)
        //        //    {
        //        //        var word = stringBuilder.ToString();
        //        //        if (wordMap.TryGetValue(word, out List<int> indexs))
        //        //        {
        //        //            indexs.Add(index);
        //        //        }
        //        //        else
        //        //        {
        //        //            wordMap.Add(word, new List<int>() { index });
        //        //        }
        //        //        index++;
        //        //        stringBuilder.Clear();
        //        //    }
        //        //}
        //    }
        //}

        private FileProperty CreateBinaryProperty(BinaryReader binaryReader, TextParser.PropertyParser propertyParser, ref long position)
        {
            return null;
        }

    }
}
