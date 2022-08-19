using Microsoft.VisualStudio.TestPlatform.PlatformAbstractions.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using VisualLogger.Streams;
using VisualLogger.Utils;

namespace VisualLogger.Test
{
    public class GitRunnerTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase("https://github.com/BlazorComponent/BlazorComponent.git", true)]
        [TestCase("https://github.com/BlazorComponent/BlazorComponent.git", false)]
        [TestCase("https://github.com/BlazorComponent/MASA.Blazor.git", true)]
        [TestCase("https://github.com/BlazorComponent/MASA.Blazor.git", false)]
        [TestCase("https://github.com/jim-jiang-github/VisualLogger.git", true)]
        [TestCase("https://github.com/jim-jiang-github/VisualLogger.git", false)]
        public async Task TestGetAllOriginBranches(string gitRepo, bool isSimplify)
        {
            var branches = await GitRunner.GetAllOriginBranches(gitRepo, isSimplify);
            Assert.True(branches.Count() > 0);
        }

        [Test]
        [TestCase("https://github.com/BlazorComponent/BlazorComponent.git", "main")]
        [TestCase("https://github.com/BlazorComponent/MASA.Blazor.git", "main")]
        [TestCase("https://github.com/jim-jiang-github/VisualLogger.git", "dev")]
        public async Task TestCloneTo(string gitRepo, string branch)
        {
            var result =  GitRunner.CloneTo(gitRepo, branch).Result;
            Assert.True(result);
        }
    }
}
