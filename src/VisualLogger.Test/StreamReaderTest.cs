using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Streams;

namespace VisualLogger.Test
{
    public class StreamReaderTest
    {
        LineStreamReader _reader;
        StreamReader _streamReader;
        BinaryReader _binaryReader;
        Stream _stream;
        [SetUp]
        public void Setup()
        {
            //_stream = File.OpenRead(@"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_21262c95-3085-4c8f-9335-6609e5b318b8_20220607-154900 (1)\RoomsHost-20220601_100320-pid_13820.log");
            _stream = File.OpenRead(@"Resources\reader1.txt");
            _reader = new LineStreamReader(_stream);
            _streamReader = new StreamReader(_stream);
            _binaryReader = new BinaryReader(_stream);
        }

        [Test]
        public void Test1()
        {
            List<string> line1s = new List<string>();
            List<string> line2s = new List<string>();
            while (_reader.ReadLine() is string line && !string.IsNullOrEmpty(line))
            {
                line1s.Add(line);
            }
            _stream.Seek(0, SeekOrigin.Begin);
            while (_reader.ReadLine() is string line && !string.IsNullOrEmpty(line))
            {
                line2s.Add(line);
            }

        }

        [Test]
        public void Test2()
        {
            List<string> lines = new List<string>();
            BinaryReader binaryReader = new BinaryReader(_stream);
            var start = 0l;
            while (_reader.ReadLine() is string line)
            {
                if (lines.Count == 17209)
                {

                }
                lines.Add(line);
                var end = _reader.Position;
                var lastPosition = _stream.Position;
                _stream.Seek(start, SeekOrigin.Begin);
                var bytes = binaryReader.ReadBytes((int)(end - start));
                var qqq = _reader.CurrentEncoding.GetByteCount(line);
                var ccc = Encoding.ASCII.GetByteCount(line);
                var str = _reader.CurrentEncoding.GetString(bytes);
                _stream.Position = lastPosition;
                Assert.That(str, Is.EqualTo(line));
                start = end;
            }
        }

        [Test]
        public void Test3()
        {
            var dir = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_21262c95-3085-4c8f-9335-6609e5b318b8_20220607-154900 (1)";
            var files = Directory.GetFiles(dir);
            List<string> lines = new List<string>();
            foreach (var file in files)
            {
                using var stream = File.OpenRead("xxx.txt");
                LineStreamReader sr = new LineStreamReader(stream);
                var ssss1 = sr.ReadLine();
                sr.DiscardBufferedData();
                stream.Seek(0, SeekOrigin.Begin);
                var ssss2 = sr.ReadLine();
                //var ssss2 = sr.ReadString(0, 75);
                var ccs1 = ssss1.ToCharArray();
                var ccs2 = ssss2.ToCharArray();
                LineStreamReader reader = new LineStreamReader(_stream);
                BinaryReader binaryReader = new BinaryReader(_stream, reader.CurrentEncoding);
                var start = 0l;
                while (reader.ReadLine() is string line)
                {
                    lines.Add(line);
                    var end = reader.Position;
                    var lastPosition = _stream.Position;
                    reader.DiscardBufferedData();
                    _stream.Seek(start, SeekOrigin.Begin);
                    var str = reader.ReadLine();
                    char[] aaaa = new char[end - start];
                    var bytes = binaryReader.ReadBytes((int)(end - start));
                    //var str = reader.CurrentEncoding.GetString(bytes);
                    _stream.Position = lastPosition;
                    var cs1 = line.ToCharArray();
                    var cs2 = str.ToCharArray();
                    for (int i = 0; i < line.Length; i++)
                    {
                        char c1 = line[i];
                        char c2 = str[i];
                        if (c1 != c2)
                        {

                        }
                    }
                    //if (str != line)
                    //{
                    //    StringBuilder sb = new StringBuilder();
                    //    sb.AppendLine(line);
                    //    sb.AppendLine(line);
                    //    sb.AppendLine(line);
                    //    File.WriteAllText("xxx.txt", sb.ToString(), reader.CurrentEncoding);
                    //}
                    Assert.That(str, Is.EqualTo(line));
                    start = end;
                }
            }
        }
    }
}