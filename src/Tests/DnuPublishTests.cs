﻿using System.Collections.Generic;
using System.IO;
using Utils;
using Xunit;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.PackageManager;
using Microsoft.Framework.PackageManager.Publish;
using Newtonsoft.Json.Linq;
using XunitExt;

namespace Tests
{
    public class DnuPublishTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuPublishWebApp_SubfolderAsPublicFolder_PureDir(DnxSdk sdk)
        {
            const string projectName = "ProjectForTesting";
            var projectJson = new JObject
            {
                ["publishExclude"] = "**.useless",
                ["webroot"] = "public",
                ["frameworks"] = new JObject
                {
                    ["dnx451"] = new JObject { }
                }
            };

            var projectStructure = new Dir
            {
                ["project.json"] = projectJson,
                ["Config.json", "Program.cs"] = Dir.EmptyFile,
                ["public"] = new Dir
                {
                    ["Scripts"] = new Dir
                    {
                        ["bootstrap.js", "jquery.js"] = Dir.EmptyFile
                    },
                    ["Images"] = new Dir
                    {
                        ["logo.png"] = Dir.EmptyFile
                    },
                    ["UselessFolder"] = new Dir
                    {
                        ["file.useless"] = Dir.EmptyFile
                    }
                },
                ["Views"] = new Dir
                {
                    ["Home"] = new Dir
                    {
                        ["index.cshtml"] = Dir.EmptyFile
                    },
                    ["Shared"] = new Dir
                    {
                        ["_Layout.cshtml"] = Dir.EmptyFile
                    }
                },
                ["Controllers"] = new Dir
                {
                    ["HomeController.cs"] = Dir.EmptyFile
                },
                ["UselessFolder"] = new Dir
                {
                    ["file.useless"] = Dir.EmptyFile
                },
                ["packages"] = new Dir { }
            };

            var expectedOutputProjectJson = new JObject
            {
                ["publishExclude"] = "**.useless",
                ["webroot"] = "../../../wwwroot",
                ["frameworks"] = new JObject
                {
                    ["dnx451"] = new JObject { }
                }
            };

            var expectedOutputGlobalJson = new JObject
            {
                ["projects"] = new JArray("src", "test"),
                ["packages"] = "packages",
            };

            var expectedOutputLockFile = new JObject
            {
                ["locked"] = false,
                ["version"] = LockFileFormat.Version,
                ["targets"] = new JObject
                {
                    ["DNX,Version=v4.5.1"] = new JObject { }
                },
                ["libraries"] = new JObject { },
                ["projectFileDependencyGroups"] = new JObject
                {
                    [""] = new JArray(),
                    ["DNX,Version=v4.5.1"] = new JArray()
                }
            };

            var expectedOutputWebConfig = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""{Constants.WebConfigBootstrapperVersion}"" value="""" />
    <add key=""{Constants.WebConfigRuntimePath}"" value=""..\approot\runtimes"" />
    <add key=""{Constants.WebConfigRuntimeVersion}"" value="""" />
    <add key=""{Constants.WebConfigRuntimeFlavor}"" value="""" />
    <add key=""{Constants.WebConfigRuntimeAppBase}"" value=""..\approot\src\{projectName}"" />
  </appSettings>
</configuration>";

            var expectedOutputStructure = new Dir
            {
                ["wwwroot"] = new Dir
                {
                    ["web.config"] = expectedOutputWebConfig,
                    ["Scripts"] = new Dir
                    {
                        ["bootstrap.js", "jquery.js"] = Dir.EmptyFile,
                    },
                    ["Images"] = new Dir
                    {
                        ["logo.png"] = Dir.EmptyFile
                    },
                    ["UselessFolder"] = new Dir
                    {
                        ["file.useless"] = Dir.EmptyFile
                    }
                },
                ["approot"] = new Dir
                {
                    ["global.json"] = expectedOutputGlobalJson,
                    ["src"] = new Dir
                    {
                        [projectName] = new Dir
                        {
                            ["project.json"] = expectedOutputProjectJson,
                            ["project.lock.json"] = expectedOutputLockFile,
                            ["Config.json", "Program.cs"] = Dir.EmptyFile,
                            ["Views"] = new Dir
                            {
                                ["Home"] = new Dir
                                {
                                    ["index.cshtml"] = Dir.EmptyFile
                                },
                                ["Shared"] = new Dir
                                {
                                    ["_Layout.cshtml"] = Dir.EmptyFile
                                }
                            },
                            ["Controllers"] = new Dir
                            {
                                ["HomeController.cs"] = Dir.EmptyFile
                            }
                        }
                    }
                }
            };

            var basePath = TestUtils.GetLocalTempFolder();
            var projectPath = Path.Combine(basePath, projectName);
            var outputPath = Path.Combine(basePath, "output");
            projectStructure.Save(projectPath);

            var result = sdk.Dnu.Execute(
                $"publish {projectPath} --out {outputPath} --wwwroot-out wwwroot",
                env => env[EnvironmentNames.Packages] = "packages");

            var actualOutputStructure = new Dir(outputPath);

            result.EnsureSuccess();
            Assert.Empty(result.StandardError);
            DirAssert.Equal(expectedOutputStructure, actualOutputStructure);
        }

        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuPublishWebApp_SubfolderAsPublicFolder_WithRuntime_DirPlusFlatList(DnxSdk sdk)
        {
            const string projectName = "ProjectForTesting";
            var targetFramework = DependencyContext.GetFrameworkNameForRuntime(sdk.FullName).FullName;

            var projectJson = new JObject
            {
                ["publishExclude"] = "**.useless",
                ["webroot"] = "public",
                ["frameworks"] = new JObject
                {
                    ["dnx451"] = new JObject { },
                    ["dnxcore50"] = new JObject { }
                }
            };

            var projectStructure = new Dir
            {
                ["project.json"] = projectJson,
                ["Config.json", "Program.cs"] = Dir.EmptyFile,
                ["public"] = new Dir
                {
                    ["Scripts/bootstrap.js", "Scripts/jquery.js", "Images/logo.png", "UselessFolder/file.useless"] = Dir.EmptyFile
                },
                ["Views"] = new Dir
                {
                    ["Home/index.cshtml", "Shared/_Layout.cshtml"] = Dir.EmptyFile
                },
                ["Controllers"] = new Dir
                {
                    ["HomeController.cs"] = Dir.EmptyFile
                },
                ["UselessFolder"] = new Dir
                {
                    ["file.useless"] = Dir.EmptyFile
                },
                ["packages"] = new Dir { }
            };

            var expectedOutputProjectJson = new JObject
            {
                ["publishExclude"] = "**.useless",
                ["webroot"] = "../../../wwwroot",
                ["frameworks"] = new JObject
                {
                    ["dnx451"] = new JObject { },
                    ["dnxcore50"] = new JObject { }
                }
            };

            var expectedOutputGlobalJson = new JObject
            {
                ["projects"] = new JArray("src", "test"),
                ["packages"] = "packages",
            };

            var expectedOutputLockFile = new JObject
            {
                ["locked"] = false,
                ["version"] = LockFileFormat.Version,
                ["targets"] = new JObject
                {
                    [targetFramework] = new JObject { },
                },
                ["libraries"] = new JObject { },
                ["projectFileDependencyGroups"] = new JObject
                {
                    [""] = new JArray(),
                    ["DNX,Version=v4.5.1"] = new JArray(),
                    ["DNXCore,Version=v5.0"] = new JArray()
                }
            };

            var expectedOutputWebConfig = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <appSettings>
    <add key=""{Constants.WebConfigBootstrapperVersion}"" value="""" />
    <add key=""{Constants.WebConfigRuntimePath}"" value=""..\approot\runtimes"" />
    <add key=""{Constants.WebConfigRuntimeVersion}"" value=""{sdk.Version}"" />
    <add key=""{Constants.WebConfigRuntimeFlavor}"" value=""{sdk.Flavor}"" />
    <add key=""{Constants.WebConfigRuntimeAppBase}"" value=""..\approot\src\{projectName}"" />
  </appSettings>
</configuration>";

            var expectedOutputStructure = new Dir
            {
                ["wwwroot"] = new Dir
                {
                    ["web.config"] = expectedOutputWebConfig,
                    ["Scripts/bootstrap.js", "Scripts/jquery.js"] = Dir.EmptyFile,
                    ["Images/logo.png"] = Dir.EmptyFile,
                    ["UselessFolder/file.useless"] = Dir.EmptyFile
                },
                ["approot"] = new Dir
                {
                    ["global.json"] = expectedOutputGlobalJson,
                    ["runtimes"] = new Dir
                    {
                        // We don't want to construct folder structure of a bundled runtime manually
                        [sdk.FullName] = new Dir(sdk.Path)
                    },
                    [$"src/{projectName}"] = new Dir
                    {
                        ["project.json"] = expectedOutputProjectJson,
                        ["project.lock.json"] = expectedOutputLockFile,
                        ["Config.json", "Program.cs"] = Dir.EmptyFile,
                        ["Views"] = new Dir
                        {
                            ["Home/index.cshtml"] = Dir.EmptyFile,
                            ["Shared/_Layout.cshtml"] = Dir.EmptyFile
                        },
                        ["Controllers"] = new Dir
                        {
                            ["HomeController.cs"] = Dir.EmptyFile
                        }
                    }
                }
            };

            var basePath = TestUtils.GetLocalTempFolder();
            var projectPath = Path.Combine(basePath, projectName);
            var outputPath = Path.Combine(basePath, "output");
            projectStructure.Save(projectPath);

            var result = sdk.Dnu.Publish(
                projectPath,
                outputPath,
                additionalArguments: $"--wwwroot-out wwwroot --runtime {sdk.FullName}",
                envSetup: env => env[EnvironmentNames.Packages] = "packages");

            var actualOutputStructure = new Dir(outputPath);

            result.EnsureSuccess();
            Assert.Empty(result.StandardError);
            DirAssert.Equal(expectedOutputStructure, actualOutputStructure);
        }
    }
}
