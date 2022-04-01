using System;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var git = new CommandRunner("git", @"C:\Users\Jim.Jiang\Documents\git_test");
            //git.Run("init");
            //git.Run("config core.sparsecheckout true");
            //git.Run("remote add origin https://github.com/GitTools/GitVersion.git");
            //git.Run("fetch --depth = 1 origin support/5.x");
            //git.Run("echo 'path/docs/' > .git/info/sparse-checkout");
            ////git.Run("checkout support/5.x");
            //git.Run("pull origin support/5.x");

            git.Run("clone -b support/5.x https://github.com/GitTools/GitVersion.git --depth 1");
        }
    }
    public class CommandRunner
    {
        public string ExecutablePath { get; }
        public string WorkingDirectory { get; }

        public CommandRunner(string executablePath, string workingDirectory = null)
        {
            ExecutablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
            WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(executablePath);
        }

        public void Run(string arguments)
        {
            var info = new ProcessStartInfo(ExecutablePath, arguments)
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory,
            };
            var process = new Process
            {
                StartInfo = info,
                EnableRaisingEvents = true
            };
            process.OutputDataReceived += (sender, args) =>
            {
                Console.WriteLine("received output: {0}", args.Data);
            };
            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.CancelOutputRead();
        }
    }
}
