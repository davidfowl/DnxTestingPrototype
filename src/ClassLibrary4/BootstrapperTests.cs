using System;
using System.Collections.Generic;
using System.IO;
using ClassLibrary31;
using Xunit;

namespace ClassLibrary4
{
    public class BootstrapperTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void BootstrapperInvokesAssemblyWithInferredAppBaseAndLibPath(DnxSdk sdk)
        {
            // TODO: Solution class: solution.OutputFolder, solution.MainProject
            var outputFolder = sdk.Flavor == "coreclr" ? "dnxcore50" : "dnx451";
            var solution = Utils.GetSolution("SimpleConsoleApp", shared: false);
            var projectPath = solution.GetProjectPath("SimpleConsoleApp");
            var buildOutputPath = solution.ArtifactsPath;

            sdk.Dnu.Restore(projectPath);
            var sdk.Dnu.Build(projectPath, buildOutputPath, configuration: "Release");

            // TODO: output result as param, DNX_TRACE boolean, env less noisy
            var exitCode = sdk.Dnx.Execute(
                Path.Combine(outputFolder, "Release", outputFolder, "HelloWorld.dll"),
                environment: env =>
                {
                    env[EnvironmentNames.Trace] = null;
                });

            Assert.Equal(0, exitCode);
            Assert.Equal(@"Hello World!
Hello, code!
", stdOut);
        }
    }
}
