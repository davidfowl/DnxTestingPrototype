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
            const string configuration = "Release";
            const string appName = "SimpleConsoleApp";
            var solution = TestUtils.GetSolution(appName, shared: false);
            var project = solution.GetProject(appName);
            var buildOutputPath = project.BinPath;

            sdk.Dnu.RestoreAndCheckExitCode(project.ProjectDirectory);
            var packOutput = sdk.Dnu.PackAndCheckExitCode(project.ProjectDirectory, buildOutputPath, configuration: configuration);

            string stdOut, stdErr;
            var exitCode = sdk.Dnx.Execute(
                packOutput.GetAssemblyPath(sdk.TargetFramework),
                out stdOut,
                out stdErr,
                dnxTraceOn: false);

            Assert.Equal(0, exitCode);
            Assert.Equal(@"Hello World!
", stdOut);
            Assert.Empty(stdErr);
        }
    }
}
