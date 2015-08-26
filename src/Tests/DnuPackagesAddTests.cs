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

            sdk.Dnu.Restore(project.ProjectDirectory).EnsureSuccess();

            project.Update(json => json["description"] = "Old");
            var packOutput = sdk.Dnu.Pack(project.ProjectDirectory, project.BinPath, configuration: "Release");

            packOutput.EnsureSuccess();

            var result = sdk.Dnu.PackagesAdd(
                packagePath: packOutput.PackagePath,
                packagesDir: solution.LocalPackagesDir);
            result.EnsureSuccess();
            Assert.Empty(result.StandardError);
            Assert.Contains($"Installing {project.Name}.{project.Version}", result.StandardOutput);

            var lastInstallTime = new FileInfo(nuspecPath).LastWriteTimeUtc;

            // TestUtils.UpdateJson(project.ProjectFilePath, json => json["description"] = "New");
            project.Update(json => json["description"] = "New");

            packOutput = sdk.Dnu.Pack(project.ProjectDirectory, project.BinPath, configuration: "Release");

            packOutput.EnsureSuccess();

            result = sdk.Dnu.PackagesAdd(
                packagePath: packOutput.PackagePath,
                packagesDir: solution.LocalPackagesDir);

            result.EnsureSuccess();
            Assert.Empty(result.StandardError);
            Assert.Contains($"Overwriting {project.Name}.{project.Version}", result.StandardOutput);

            var xDoc = XDocument.Load(packagePathResolver.GetManifestFilePath(project.Name, project.Version));
            var actualDescription = xDoc.Root.Descendants()
                .Single(x => string.Equals(x.Name.LocalName, "description")).Value;
            Assert.Equal("New", actualDescription);
            Assert.NotEqual(lastInstallTime, new FileInfo(nuspecPath).LastWriteTimeUtc);
        }
    }
}
