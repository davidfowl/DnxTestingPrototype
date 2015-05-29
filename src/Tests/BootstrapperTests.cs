using System;
using System.Collections.Generic;
using System.IO;
using Utils;
using Xunit;

namespace Tests
{
    public class BootstrapperTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void BootstrapperInvokesAssemblyWithInferredAppBaseAndLibPath(DnxSdk sdk)
        {
            var outputFolder = sdk.Flavor == "coreclr" ? "dnxcore50" : "dnx451";
            var solution = TestUtils.GetSolution("SimpleConsoleApp", shared: false);
            var projectPath = solution.GetProjectPath("SimpleConsoleApp");
            var buildOutputPath = solution.ArtifactsPath;

            sdk.Dnu.Restore(projectPath);
            sdk.Dnu.Build(projectPath, buildOutputPath, configuration: "Release");

            // TODO: output result as param???
            string stdOut, stdErr;
            var exitCode = sdk.Dnx.Execute(
                Path.Combine(buildOutputPath, "Release", outputFolder, "SimpleConsoleApp.dll"),
                out stdOut,
                out stdErr,
                dnxTraceOn: false);

            Assert.Equal(0, exitCode);
            Assert.Equal(@"Hello World!
", stdOut);
            Assert.True(string.IsNullOrEmpty(stdErr));
        }
    }
}
