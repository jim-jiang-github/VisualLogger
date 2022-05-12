using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VisualLogger.Datas;
using VisualLogger.Datas.LogSources;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;
using VisualLogger.Schemas.Logs;

namespace VisualLogger
{
    public class Program
    {
        public static int Count;
        static void Main(string[] args)
        {

            MemoryMappedStreamLoader memoryMappedStreamLoader = new MemoryMappedStreamLoader();
            var stream = memoryMappedStreamLoader.LoadLogStream(@"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_1112e3df-80f9-435d-8b5d-2b7c5a76ee1f_20220407-172316\RoomsHost-20220407165140.rcvlog");
            //12137MB
            //MixStreamReader mixStreamReader = new MixStreamReader(stream);
            //List<StreamCell> sss = new List<StreamCell>();
            //for (int i = 0; i < 111267; i++)
            //{
            //    sss.Add(new StreamCell(mixStreamReader, 0, 0, StreamCellType.Int, null));
            //}
            var a = LogSchemaBinary.LoadFromJsonFile("LogSchemaBinary.json", out string? ssss);
            if (a == null)
            {
                return;
            }
            LogSourceBinary logContentBinary = new LogSourceBinary(stream, a);
            logContentBinary.Dispose();
            stream.Dispose();
            GC.Collect();
            Console.ReadKey();
            var asd = logContentBinary.GetBodyItems();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (asd == null)
            {
                return;
            }
            var t = logContentBinary.GetBodyTemplate();
            if (t == null)
            {
                return;
            }
            LogContent logSource = new LogContent(stream, t, asd);
            foreach (var item in asd)
            {
                var sssasd = item[0].ToString();
            }

            ////LogSchemaText logSchema = new LogSchemaText();
            //FileStreamLoader fileStreamLoader = new FileStreamLoader();
            //var stream = fileStreamLoader.LoadLogStream(@"C:\Users\Jim.Jiang\Downloads\88D77430\log\2022-04-16-154234.906-main.log");
            //var logSchema = LogSchemaText.LoadFromJsonFile("LogSchemaText.json", out string? ssss);
            //if (logSchema==null)
            //{
            //    return;
            //}
            //LogContentText logContentText = new LogContentText(stream, logSchema);
            //var asd = logContentText.GetBodyItems("Content");

            //var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            //var binaryContentParser = BinaryContentParser.LoadFromJsonFile(parserFile);
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_88cd1cff-de9c-4a14-9769-d4d512e077e0_20220210-155757\RoomsHost-20220210155123.rcvlog";
            //BinaryContentLoader binaryContentLoader = BinaryContentLoader.Load(binaryContentParser);
            //var logContent = binaryContentLoader.LoadLogContent(logFile);
            //var items = logContent.GetItems(22, 6);

            //var textContentParser = new TextContentParser();
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\IA9D4WBN-C\private\var\mobile\Containers\Data\Application\1102348A-D82A-4DD6-9028-5554338F31F2\Documents\log\2022-04-01-213241.765-network-uploaded.log\2022-04-01-213241.765-network-uploaded.log";
            //var logFile1 = @"C:\Users\Jim.Jiang\Downloads\IA9D4WBN-C\private\var\mobile\Containers\Data\Application\1102348A-D82A-4DD6-9028-5554338F31F2\Documents\log\2022-04-06-144614.281-main.log";

            //var logFile2 = @"C:\Users\Jim.Jiang\Documents\rcvrooms-windows-feature\ci\versions.txt";
            //var logFile3 = @"1.txt";
            ////File.WriteAllText(logFile3, "aaaaaaaaaa\r\nxxxxxxxxxx\r\ndddddddddd");
            ////var text = File.ReadAllText(logFile);

            ////var a = Regex.Match(text, @"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3})(.*\D*)(?!(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}.\d{3} [A-Z]{3}))", RegexOptions.Singleline);

            ////byte[] buffer = new byte[1024];
            ////var fs = File.OpenRead(logFile2);
            ////while (fs.Read(buffer, 0, buffer.Length) > 0)
            ////{
            ////    var a = Encoding.UTF8.GetString(buffer);
            ////}
            ////int index = 0;
            ////foreach (var c in text)
            ////{
            ////    index++;
            ////}

            //var textContentLoader = TextContentLoader.Load(textContentParser);
            //var logContent = textContentLoader.LoadLogContent(logFile1);

        }
    }
}
