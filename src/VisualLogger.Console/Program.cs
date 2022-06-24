// See https://aka.ms/new-console-template for more information
using Microsoft.CodeAnalysis.FlowAnalysis;
using Serilog;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using VisualLogger.Console;
using VisualLogger.Scenarios;
using VisualLogger.Schemas;
using VisualLogger.Schemas.Logs;
using VisualLogger.Schemas.Scenarios;
using VisualLogger.Sources;
using VisualLogger.Streams;
using VisualLogger.Utils;

//GitRunner gitRunner = new GitRunner("git@git.ringcentral.com:CoreLib/rcvrooms-windows.git");
//GitRunner gitRunner = new GitRunner("https://github.com/jim-jiang-github/VisualLogger.Scenarios.git");
//await gitRunner.Fetch();
//await gitRunner.CloneTo("rcv/windows/22.2.20", "ScenariosFolder");

Log.Logger = new LoggerConfiguration()
#if DEBUG
           .WriteTo.Console()
           .MinimumLevel.Debug()
#else
           .MinimumLevel.Information()
#endif
           .WriteTo.File("log.txt",
               rollingInterval: RollingInterval.Day,
               rollOnFileSizeLimit: true)
           .CreateLogger();
//new LineStreamReader(File.OpenRead("xxxx3.txt")).ReadLine();
//var eee = new UnicodeEncoding(true, true);
//var eee1 = new UnicodeEncoding(false, true);
//var eee2 = Encoding.UTF8;
//var eees = eee.ToString();
//var eees1 = eee1.ToString();
//var arn = "a\rb\rccccc\nddddddddddd\r\n";
//File.WriteAllText("xxxx1.txt", arn, Encoding.UTF8);
//File.WriteAllText("xxxx2.txt", arn, Encoding.ASCII);
//File.WriteAllText("xxxx3.txt", arn, Encoding.Unicode);
//File.WriteAllText("xxxx4.txt", arn, new UnicodeEncoding(true, true));
//var rrrr = new StreamBytesLineReader(File.OpenRead("xxxx4.txt"));
//while (true)
//{
//    var tp = rrrr.ReadLinePosition();
//    var aaa = rrrr.ReadString(tp.Value.Item1, tp.Value.Item2);
//}
//var p1 = Encoding.UTF8.GetPreamble();
//var p2 = Encoding.ASCII.GetPreamble();
//var p3 = Encoding.Unicode.GetPreamble();
//var p4 = new UnicodeEncoding(true, true).GetPreamble();
//var stream1 = File.OpenRead("xxxx2.txt");
//byte[] byteBuffers = new byte[1024];
//int bLen = stream1.Read(byteBuffers, 0, byteBuffers.Length);
//var x = string.Join(",", byteBuffers.Take(bLen));
////"239,187,191,13,10"
////"239,187,191,97,115,100,97,115,100,13,10,113,119,101,120,97"

////"13,10"
////"97,115,100,97,115,100,13,10,113,119,101,120,97"

////"255,254,13,0,10,0"
////"255,254,97,0,115,0,100,0,97,0,115,0,100,0,13,0,10,0,113,0,119,0,101,0,120,0,97,0"

////"254,255,0,97,0,115,0,100,0,97,0,115,0,100,0,13,0,10,0,113,0,119,0,101,0,120,0,97"
////"254,255,0,97,0,115,0,100,0,97,0,115,0,100,0,13,0,113,0,119,0,101,0,120,0,97"
//void ss(string path, long position)
//{
//    var byteLength = 64;
//    var maxCharsPerBuffer = Encoding.UTF8.GetMaxCharCount(byteLength);
//    byte[] byteBuffer = new byte[byteLength];
//    char[] charBuffer = new char[maxCharsPerBuffer];
//    var stream = File.OpenRead(path);
//    stream.Seek(position, SeekOrigin.Begin);
//    int bLen = stream.Read(byteBuffer, 0, byteLength);
//    var cLen = Encoding.UTF8.GetDecoder().GetChars(byteBuffer, 0, bLen, charBuffer, 0);
//    var x = string.Join(",", byteBuffer);
//    string s = new string(charBuffer, 0, cLen);
//    var c = Encoding.UTF8.GetByteCount(s);
//}
//byte[] b1 = new byte[] { 60, 239, 191, 189, 14, 239, 191, 189, 2, 14, 62, 32, 23, 37, 239, 191, 189, 239, 191, 189, 127, 58, 50, 52, 56, 32, 95, 112, 117, 98, 95, 110, 117, 98, 95, 99, 108, 105, 101, 110, 116, 46, 99, 112, 112, 32, 58, 111, 110, 82, 101, 99, 101, 105, 118, 101, 84, 97, 112, 78, 111, 116, 105, 102, 121, 68, 97, 116, 97, 13, 10 };

//byte[] b2 = new byte[] { 60, 215, 14, 235, 164, 2, 14, 62, 32, 23, 37, 207, 255, 127, 58, 50, 52, 56, 32, 95, 112, 117, 98, 95, 110, 117, 98, 95, 99, 108, 105, 101, 110, 116, 46, 99, 112, 112, 32, 58, 111, 110, 82, 101, 99, 101, 105, 118, 101, 84, 97, 112, 78, 111, 116, 105, 102, 121, 68, 97, 116, 97, 13, 10 };

//char[] c1 = new char[b1.Length];
//var s1 = Encoding.UTF8.GetString(b1);
//var s2 = Encoding.UTF8.GetString(b2);
//var sc1 = Encoding.UTF8.GetByteCount(s1);
//var sc2 = Encoding.UTF8.GetByteCount(s2);

////"<�\u000e�\u0002\u000e> \u0017%��\u007f:248 _pub_nub_client.cpp :onReceiveTapNotifyData\r\n"
////"<�\u000e�\u0002\u000e> \u0017%��\u007f:248 _pub_nub_client.cpp :onReceiveTapNotifyData\r\n"
////"60|215|14|235|164|2|14|62|32|23|37|207|255|127|58|50|52|56|32|95|112|117|98|95|110|117|98|95|99|108|105|101|110|116|46|99|112|112|32|58|111|110|82|101|99|101|105|118|101|84|97|112|78|111|116|105|102|121|68|97|116|97|13|10"
////"60|215|14|235|164|2|14|62|32|23|37|207|255|127|58|50|52|56|32|95|112|117|98|95|110|117|98|95|99|108|105|101|110|116|46|99|112|112|32|58|111|110|82|101|99|101|105|118|101|84|97|112|78|111|116|105|102|121|68|97|116|97|13|10|48|54|47|48|49|47|50"
////"60|239|191|189|14|239|191|189|2|14|62|32|23|37|239|191|189|239|191|189|127|58|50|52|56|32|95|112|117|98|95|110|117|98|95|99|108|105|101|110|116|46|99|112|112|32|58|111|110|82|101|99|101|105|118|101|84|97|112|78|111|116|105|102|121|68|97|116|97|13|10"
////ss("xxx.txt", 0);
//ss(@"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_21262c95-3085-4c8f-9335-6609e5b318b8_20220607-154900 (1)\RoomsHost-20220601_095353-pid_10424.log", 1669477);
//var byteLength = 71;
//var maxCharsPerBuffer = Encoding.UTF8.GetMaxCharCount(byteLength);
//byte[] byteBuffer = new byte[71];
//char[] charBuffer = new char[maxCharsPerBuffer];
//var stream = File.OpenRead("xxx.txt");
//int bLen = stream.Read(byteBuffer, 0, byteLength);
//var cLen = Encoding.UTF8.GetDecoder().GetChars(byteBuffer, 0, bLen, charBuffer, 0);
//string s = new string(charBuffer, 0, cLen);
//var stream1 = File.OpenRead(@"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_21262c95-3085-4c8f-9335-6609e5b318b8_20220607-154900 (1)\RoomsHost-20220601_095353-pid_10424.log");
//stream1.Seek(1669477, SeekOrigin.Begin);
//bLen = stream1.Read(byteBuffer, 0, byteLength);
//cLen = Encoding.UTF8.GetDecoder().GetChars(byteBuffer, 0, bLen, charBuffer, 0);
//s = new string(charBuffer, 0, cLen);

//SchemaLogText schemaLogText = new SchemaLogText();
//schemaLogText.SaveAsDefault_22_2_20();
//SchemaLogBinary schemaLogBinary = new SchemaLogBinary();
////schemaLogBinary.SaveAsDefault();
//SchemaScenario schemaScenario = new SchemaScenario();
//schemaScenario.SaveAsDefault();

//var t = File.ReadAllText("xxx.txt");
//LineStreamReader streamReader = new LineStreamReader(File.OpenRead("xxx.txt"));
//var qweqwe = streamReader.ReadLine();
//var asss = qweqwe.ToCharArray();
//var a = streamReader.Position;
//var aaa = streamReader.ReadString(0, 71);
//int length = Encoding.UTF8.GetByteCount(t);

//var dir = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_dd8ae80b-0de0-46ba-b1e5-72b948a2ad01_20220617-112805";
var xx = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_21262c95-3085-4c8f-9335-6609e5b318b8_20220607-154900 (1)\RoomsHost-20220601_095353-pid_10424.log";
var dir = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_75a6889c-ce51-4840-ace1-3ef098034520_20220615-104915";
var files = Directory.GetFiles(dir);
Scenario scenario = new Scenario();
scenario.Init();
Stopwatch stopwatch = new Stopwatch();
foreach (var file in files.Skip(1))
{
    stopwatch.Restart();
    scenario.LoadLogSource(file);
    var arr = scenario.LogSource.GetRows(0, scenario.LogSource.TotalRowsCount).Select(x => x[0].ToString()).ToArray();
    Log.Information("Find {path} total:{total} use:{time}ms", file, scenario.LogSource.TotalRowsCount, stopwatch.ElapsedMilliseconds);
    for (int i = 0; i < arr.Length; i++)
    {
        string item = arr[i];
        if (DateTime.TryParseExact(item, "yyyy-MM-dd HH:mm:ss,fff", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime datetime))
        {
        }
        else
        {
            Log.Error("Find {path} error at line:{line} total:{total}", file, i, arr.Length);
            break;
        }
    }
}