// See https://aka.ms/new-console-template for more information
using Serilog;
using System.Globalization;
using VisualLogger.Core.Scenarios;
using VisualLogger.Core.Schemas;
using VisualLogger.Core.Schemas.Logs;
using VisualLogger.Core.Schemas.Scenarios;
using VisualLogger.Core.Sources;
using VisualLogger.Core.Utils;
using VisualLogger.Data;

//GitRunner gitRunner = new GitRunner("git@git.ringcentral.com:CoreLib/rcvrooms-windows.git");
//GitRunner gitRunner = new GitRunner("https://github.com/jim-jiang-github/VisualLogger.Scenarios.git");
//await gitRunner.Fetch();
//await gitRunner.CloneTo("rcv/windows/22.2.20", "ScenariosFolder");


Log.Logger = new LoggerConfiguration()
#if DEBUG
           .MinimumLevel.Debug()
           .WriteTo.Console()
#else
           .MinimumLevel.Information()
#endif
           .WriteTo.File("log.txt",
               rollingInterval: RollingInterval.Day,
               rollOnFileSizeLimit: true)
           .CreateLogger();

SchemaLogText logSchemaText = new SchemaLogText();
logSchemaText.SaveAsDefault_22_2_20();
SchemaScenario schemaScenario = new SchemaScenario();
schemaScenario.SaveAsDefault();

var logFilePath = @"C:\Users\Jim.Jiang\Downloads\WRoomsFeedBack_HostLog_49ff3833-2153-4dfd-a55d-ea34a30566a5_20220610-140234\RoomsHost-20220607_131631-pid_16104.log";
Scenario scenario = new Scenario();
scenario.Init();
scenario.LoadLogSource(logFilePath);
var logSource = scenario.LogSource;

//for (int i = 0; i < 11; i++)
//{
//    Log.Debug("Hello, Serilog! {Index}", i);
//    Log.Information("Hello, Serilog! {Index}", i);
//    Log.Warning("Hello, Serilog! {Index}", i);
//    Log.Error("Hello, Serilog! {Index}", i);
//}
Log.CloseAndFlush();
Console.WriteLine("Hello, World!");
Console.ReadLine();