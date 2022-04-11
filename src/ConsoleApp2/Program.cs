using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using VisualLogger.Datas;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Text;
using VisualLogger.InterfaceImplModules.LogStreamLoaders;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var parserFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var binaryContentParser = BinaryContentParser.LoadFromJsonFile(parserFile);
            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_88cd1cff-de9c-4a14-9769-d4d512e077e0_20220210-155757\RoomsHost-20220210155123.rcvlog";
            BinaryLogLoader binaryLogLoader = BinaryLogLoader.Load(binaryContentParser);
            var logContent = binaryLogLoader.LoadLogContent(logFile);
            var items = logContent.GetItems(22, 6);


            //var textParser = new TextParser();
            //var logFile = @"C:\Users\Jim.Jiang\Downloads\IA9D4WBN-C\private\var\mobile\Containers\Data\Application\1102348A-D82A-4DD6-9028-5554338F31F2\Documents\log\2022-04-01-213241.765-network-uploaded.log\2022-04-01-213241.765-network-uploaded.log";
            //TextLogLoader textLogLoader = TextLogLoader.Load(textParser);
            //var logContent = textLogLoader.LoadLogContent(logFile);

        }
    }
}
