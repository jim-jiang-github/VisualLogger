using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisualLogger.Utils
{
    public static class GitRunner
    {
        private const string BRANCH_NAME_HEAD = "refs/heads/";
        private const string GIT_TEMP_FOLDER = "GitTemp";

        public static async Task<bool> CloneTo(string gitRepo, string branch, CancellationToken cancellationToken = default)
        {
            var gitTempDirectory = Path.Combine(Global.CurrentAppDataDirectory, GIT_TEMP_FOLDER);
            if (!DirectoryHelper.ResetDirectory(gitTempDirectory))
            {
                return false;
            }
            StringBuilder stringBuilder = new StringBuilder();
            //try
            {
                var cmd = await Cli.Wrap("git")
                     .WithWorkingDirectory(gitTempDirectory)
                     .WithArguments(args => args
                     .Add("clone")
                     .Add(gitRepo)
                     .Add("--depth=1")
                     .Add("-b")
                     .Add(branch)
                     )
                     .WithStandardErrorPipe(PipeTarget.ToDelegate((msg) =>
                     {
                         stringBuilder.AppendLine(msg);
                     }))
                    //.WithValidation(CommandResultValidation.None)
                    .ExecuteAsync(cancellationToken);
                Log.Information("Execute result: {StartTime} {RunTime} {ExitTime} {ExitCode}",
                    cmd.StartTime,
                    cmd.RunTime,
                    cmd.ExitTime,
                    cmd.ExitCode);
                if (cmd.ExitCode != 0)
                {
                    var errorMsg = stringBuilder.ToString();
                    Notification.Error(errorMsg);
                    Log.Error(errorMsg);
                }
                else
                {
                    Log.Information("Branch: {branch} cloned!", branch);
                }
            }
            //catch (Exception ex)
            //{
            //    if (cancellationToken.IsCancellationRequested)
            //    {
            //        Log.Information("User canceled!");
            //    }
            //    else
            //    {
            //        stringBuilder.AppendLine(ex.Message);
            //        var errorMsg = stringBuilder.ToString();
            //        Notification.Error(errorMsg);
            //        Log.Error(errorMsg);
            //    }
            //}
            return true;
        }

        public static async Task<IEnumerable<string>> GetAllOriginBranches(string gitRepo, bool isSimplify = true, CancellationToken cancellationToken = default)
        {
            List<string> branches = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            try
            {
                var cmd = await Cli.Wrap("git")
                    .WithArguments(args => args
                    .Add("ls-remote")
                    .Add("--head")
                    .Add(gitRepo))
                    .WithStandardOutputPipe(PipeTarget.ToDelegate((branch) =>
                    {
                        if (isSimplify)
                        {
                            var match = Regex.Match(branch, "(.*)\\t(.*)");
                            if (match.Success && match.Groups.Count == 3)
                            {
                                var id = match.Groups[1];
                                var name = match.Groups[2];
                                var branchName = name.Value.Replace(BRANCH_NAME_HEAD, "");
                                branches.Add(branchName);
                            }
                        }
                        else
                        {
                            branches.Add(branch);
                        }
                    }))
                    .WithStandardErrorPipe(PipeTarget.ToDelegate((err) =>
                    {
                        stringBuilder.AppendLine(err);
                    }))
                    .WithValidation(CommandResultValidation.None)
                    .ExecuteAsync(cancellationToken);
                Log.Information("Execute result: {StartTime} {RunTime} {ExitTime} {ExitCode}",
                    cmd.StartTime,
                    cmd.RunTime,
                    cmd.ExitTime,
                    cmd.ExitCode);
                if (cmd.ExitCode != 0)
                {
                    var errorMsg = stringBuilder.ToString();
                    Notification.Error(errorMsg);
                    Log.Error(errorMsg);
                }
                else
                {
                    Log.Information(string.Join("\r\n", branches));
                }
            }
            catch (Exception ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    Log.Information("User canceled!");
                }
                else
                {
                    stringBuilder.AppendLine(ex.Message);
                    var errorMsg = stringBuilder.ToString();
                    Notification.Error(errorMsg);
                    Log.Error(errorMsg);
                }
            }
            return branches;
        }
    }
}
