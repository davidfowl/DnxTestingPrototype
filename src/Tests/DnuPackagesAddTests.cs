using System.IO;
using System.Xml.Linq;
using System.Linq;
using Utils;
using Xunit;
using NuGet;

namespace Tests
{
    public class DnuPackagesAddTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuPackagesAddOverwritesInstalledPackageWhenShasDoNotMatch(DnxSdk sdk)
        {
            const string appName = "SimpleConsoleApp";
            var solution = TestUtils.GetSolution(appName, shared: false);
            var project = solution.GetProject(appName);
            var packagePathResolver = new DefaultPackagePathResolver(solution.LocalPackagesDir);
            var nuspecPath = packagePathResolver.GetManifestFilePath(project.Name, project.Version);

            sdk.Dnu.RestoreAndCheckExitCode(project.ProjectDirectory);

            TestUtils.UpdateJson(project.ProjectFilePath, json => json["description"] = "Old");
            var packOutput = sdk.Dnu.PackAndCheckExitCode(project.ProjectDirectory, project.BinPath, configuration: "Release");
            string stdOut, stdErr;
            var exitCode = sdk.Dnu.PackagesAdd(
                packagePath: packOutput.PackagePath,
                packagesDir: solution.LocalPackagesDir,
                stdOut: out stdOut,
                stdErr: out stdErr);
            Assert.Equal(0, exitCode);
            Assert.Empty(stdErr);
            Assert.Contains($"Installing {project.Name}.{project.Version}", stdOut);

            var lastInstallTime = new FileInfo(nuspecPath).LastWriteTimeUtc;

            TestUtils.UpdateJson(project.ProjectFilePath, json => json["description"] = "New");
            packOutput = sdk.Dnu.PackAndCheckExitCode(project.ProjectDirectory, project.BinPath, configuration: "Release");
            exitCode = sdk.Dnu.PackagesAdd(
                packagePath: packOutput.PackagePath,
                packagesDir: solution.LocalPackagesDir,
                stdOut: out stdOut,
                stdErr: out stdErr);
            Assert.Equal(0, exitCode);
            Assert.Empty(stdErr);
            Assert.Contains($"Overwriting {project.Name}.{project.Version}", stdOut);

            var xDoc = XDocument.Load(packagePathResolver.GetManifestFilePath(project.Name, project.Version));
            var actualDescription = xDoc.Root.Descendants()
                .Single(x => string.Equals(x.Name.LocalName, "description")).Value;
            Assert.Equal("New", actualDescription);
            Assert.NotEqual(lastInstallTime, new FileInfo(nuspecPath).LastWriteTimeUtc);
        }
    }
}
