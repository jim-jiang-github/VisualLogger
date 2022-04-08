using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using VisualLogger.Datas.LogContents;
using VisualLogger.InterfaceImplModules.LogContentLoaders.Binary;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            var jsonFile = @"C:\Users\Jim.Jiang\Documents\VisualLogger\src\ConsoleApp1\RCRooms_Windows_Binary_Parser.json";
            var logFile = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_88cd1cff-de9c-4a14-9769-d4d512e077e0_20220210-155757\RoomsHost-20220210155123.rcvlog";
            BinaryLogLoader binaryLogLoader = BinaryLogLoader.Load(jsonFile);
            var a = binaryLogLoader.LoadLogContent(logFile);
            GC.Collect();
        }
    }
}
