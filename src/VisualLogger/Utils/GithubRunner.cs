using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Localization;

namespace VisualLogger.Utils
{
    public static class GithubRunner
    {
        //This token can only access public repositories
        private const string TOKEN = "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";

        public static Task CreateIssue(string title, string body)
        {
            var client = new GitHubClient(new ProductHeaderValue(StringKeys.Repo.RepoName))
            {
                Credentials = new Credentials(TOKEN)
            };

            return client.Issue.Create(StringKeys.Repo.UserName, StringKeys.Repo.RepoName, new NewIssue(title)
            {
                Body = body
            });
        }
    }
}
