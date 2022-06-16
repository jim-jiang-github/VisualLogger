using CliWrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VisualLogger.Utils
{
    public class GitRunner
    {
        private const string DEFAULT_BRANCH_NAME = "HEAD";
        private const string BRANCH_NAME_HEAD = "refs/heads/";
        private const string GIT_TEMP_FOLDER = "GitTemp";
        private static readonly string _gitTempDirectory = Path.Combine(Directory.GetCurrentDirectory(), GIT_TEMP_FOLDER);

        private readonly string _gitRepo;

        public int DefaultBranchIndex { get; private set; } = -1;
        public string? DefaultBranch { get; private set; }
        public IEnumerable<string>? Branches { get; private set; }

        public GitRunner(string gitRepo)
        {
            _gitRepo = gitRepo;
        }
        public async Task Fetch()
        {
            var branchInfos = await GetAllOriginalBranches();
            List<string> branches = new();
            var currentId = string.Empty;
            int index = -1;
            foreach (var branch in branchInfos)
            {
                var match = Regex.Match(branch, "(.*)\\t(.*)");
                if (match.Success && match.Groups.Count == 3)
                {
                    var id = match.Groups[1];
                    var name = match.Groups[2];
                    if (name.Value == DEFAULT_BRANCH_NAME)
                    {
                        currentId = id.Value;
                    }
                    else
                    {
                        var branchName = name.Value.Replace(BRANCH_NAME_HEAD, "");
                        if (currentId == id.Value)
                        {
                            DefaultBranchIndex = index;
                            DefaultBranch = branchName;
                        }
                        branches.Add(branchName);
                    }
                }
                index++;
            }
            Branches = branches;
        }
        public async Task<bool> CloneTo(string branch, string folder = "")
        {
            if (!DirectoryHelper.ResetDirectory(_gitTempDirectory))
            {
                return false;
            }
            var cmd = await Cli.Wrap("git")
                .WithWorkingDirectory(_gitTempDirectory)
                .WithArguments(args => args
                .Add("clone")
                .Add(_gitRepo)
                .Add(folder)
                .Add("--depth=1")
                .Add("-b")
                .Add(branch)
                )
                .WithStandardOutputPipe(PipeTarget.ToDelegate((msg) =>
                {

                }))
                .ExecuteAsync();
            return true;

        }
        private async Task<IEnumerable<string>> GetAllOriginalBranches()
        {
            List<string> branches = new List<string>();
            var cmd = await Cli.Wrap("git")
                .WithArguments(args => args
                .Add("ls-remote")
                .Add(_gitRepo))
                .WithStandardOutputPipe(PipeTarget.ToDelegate((msg) =>
                {
                    branches.Add(msg);
                }))
                .ExecuteAsync();
            return branches;
        }
    }
}
