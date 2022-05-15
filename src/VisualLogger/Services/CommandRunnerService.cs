using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static VisualLogger.Services.CommandRunnerService;

namespace VisualLogger.Services
{
    //var git = new CommandRunner("git", @"C:\Users\Jim.Jiang\Documents\git_test");
    ////git.Run("init");
    ////git.Run("config core.sparsecheckout true");
    ////git.Run("remote add origin https://github.com/GitTools/GitVersion.git");
    ////git.Run("fetch --depth = 1 origin support/5.x");
    ////git.Run("echo 'path/docs/' > .git/info/sparse-checkout");
    //////git.Run("checkout support/5.x");
    ////git.Run("pull origin support/5.x");

    //git.Run("clone -b support/5.x https://github.com/GitTools/GitVersion.git --depth 1");

    public class CommandRunnerService
    {
        public class CommandRunner
        {
            private string _executablePath;
            private string _workingDirectory;
            private List<Func<Task<string>>> commands = new List<Func<Task<string>>>();
            public CommandRunner(string executablePath, string workingDirectory)
            {
                _executablePath = executablePath;
                _workingDirectory = workingDirectory;
            }
            public CommandRunner Command(string arguments, bool subscriptResult = false)
            {
                Func<Task<string>> func = new Func<Task<string>>(async () =>
                {
                    var info = new ProcessStartInfo(_executablePath, arguments)
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        WorkingDirectory = _workingDirectory,
                    };
                    var process = new Process
                    {
                        StartInfo = info,
                        EnableRaisingEvents = true
                    };
                    process.Start();
                    await process.WaitForExitAsync();
                    var error = process.StandardError.ReadToEnd();
                    var result = string.Empty;
                    if (subscriptResult)
                    {
                        result = process.StandardOutput.ReadToEnd();
                    }
                    return result;
                });
                commands.Add(func);
                return this;
            }

            public async Task<string> Run()
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var command in commands)
                {
                    var result = await command.Invoke();
                    stringBuilder.AppendLine(result);
                }
                return stringBuilder.ToString();
            }
        }

        public CommandRunner Init(string executablePath, string workingDirectory)
        {
            if (!Directory.Exists(workingDirectory))
            {
                Directory.CreateDirectory(workingDirectory);
            }
            return new CommandRunner(executablePath, workingDirectory);
        }
    }
}
