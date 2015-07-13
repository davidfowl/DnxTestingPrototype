using System.IO;
using System.Xml.Linq;
using System.Linq;
using Utils;
using Xunit;
using NuGet;

namespace Tests
{
    public class DnuRestoreTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuRestoreInstallsIndirectDependency(DnxSdk sdk)
        {
            // SimpleChain -> DependencyA -> DependencyB
            const string appName = "SimpleChain";
            const string solutionName = "DependencyGraphs";
            var solution = TestUtils.GetSolution(solutionName, shared: true);
            var project = solution.GetProject(appName);
            var localFeed = TestUtils.CreateLocalFeed(solution);
            var tempDir = TestUtils.GetLocalTempFolder();
            var packagesDir = Path.Combine(tempDir, "packages");
            var projectDir = Path.Combine(tempDir, project.Name);
            TestUtils.CopyFolder(project.ProjectDirectory, projectDir);

            string stdOut, stdErr;
            var exitCode = sdk.Dnu.Restore(
                projectDir,
                packagesDir,
                feeds: new string[] { localFeed },
                stdOut: out stdOut,
                stdErr: out stdErr);

            Assert.Equal(0, exitCode);
            Assert.Empty(stdErr);
            Assert.Contains($"Installing DependencyA.1.0.0", stdOut);
            Assert.Contains($"Installing DependencyB.2.0.0", stdOut);
            Assert.Equal(2, Directory.EnumerateFileSystemEntries(packagesDir).Count());
            Assert.True(Directory.Exists(Path.Combine(packagesDir, "DependencyA", "1.0.0")));
            Assert.True(Directory.Exists(Path.Combine(packagesDir, "DependencyB", "2.0.0")));
        }
    }
}
