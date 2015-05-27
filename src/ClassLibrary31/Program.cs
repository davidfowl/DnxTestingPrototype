using System.IO;
using Newtonsoft.Json.Linq;

namespace ClassLibrary31
{
    public class Program
    {
        public void Main()
        {
            //Scenario1("1.0.0-dev", "clr", "win", "x86");
            Scenario2("1.0.0-dev", "clr", "win", "x86");
        }

        public void Scenario1(string version, string flavor, string os, string arch)
        {
            var solution = CreateSolution();

            solution["src"] = new DirectoryTree
            {
                ["P1"] = new DirectoryTree
                {
                    ["Foo.cs"] = "This should be a compilation error",
                    ["project.json"] = new JObject
                    {
                        ["frameworks"] = new JObject
                        {
                            ["dnx451"] = new JObject { }
                        }
                    }
                }
            };

            var rootPath = CreateDirectory("scenario1");
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }

            var solutionPath = Path.Combine(rootPath, "solution");
            solution.Save(solutionPath);

            var projectPath = Path.Combine(solutionPath, "src", "P1");
            var publishOutput = Path.Combine(rootPath, "publishOutput");

            var sdk = DnxSdk.GetRuntime(version, flavor, os, arch);

            sdk.Dnu.Restore(projectPath);
            sdk.Dnu.Publish(projectPath, publishOutput, noSource: true);
        }

        public void Scenario2(string version, string flavor, string os, string arch)
        {
            var solution = CreateSolution();

            solution["src"] = new DirectoryTree
            {
                ["P1"] = new DirectoryTree
                {
                    ["project.json"] = new JObject
                    {
                        ["dependencies"] = new JObject
                        {
                            ["P2"] = "",
                            ["System.Security.Cryptography.Hashing.Algorithms2"] = "4.0.0-beta-*",
                        },
                        ["frameworks"] = new JObject
                        {
                            ["dnx451"] = new JObject { }
                        }
                    }
                },
                ["P2"] = new DirectoryTree
                {
                    ["project.json"] = new JObject
                    {
                        ["frameworks"] = new JObject
                        {
                            ["dnx451"] = new JObject
                            {
                                ["frameworkAssemblies"] = new JObject
                                {
                                    ["System.Runtime"] = ""
                                }
                            }
                        }
                    }
                }
            };

            var rootPath = CreateDirectory("scenario2");
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, recursive: true);
            }

            var solutionPath = Path.Combine(rootPath, "solution");
            solution.Save(solutionPath);

            var projectPath = Path.Combine(solutionPath, "src", "P1");
            var publishOutput = Path.Combine(rootPath, "pub");

            var sdk = DnxSdk.GetRuntime(version, flavor, os, arch);

            sdk.Dnu.Restore(projectPath);
            sdk.Dnu.Publish(projectPath, publishOutput, noSource: true);
        }

        private static void BuildPackages()
        {
            var tree = new DirectoryTree
            {
                ["MyPackage1"] = new DirectoryTree
                {
                    ["project.json"] = new JObject
                    {
                        ["dependencies"] = ""
                    }
                }
            };
        }

        private static DirectoryTree Expected()
        {
            return new DirectoryTree
            {
                ["approot"] = new DirectoryTree
                {
                    ["global.json"] = new JObject
                    {
                        ["packages"] = "packages"
                    },
                    ["src"] = new DirectoryTree
                    {
                        ["P1"] = new DirectoryTree
                        {
                            ["project.json", "project.lock.json"] = ""
                        }
                    }
                }
            };
        }

        private string CreateDirectory(string scenario)
        {
            return Path.Combine(@"C:\published_things", scenario);
        }

        private static DirectoryTree CreateSolution()
        {
            var solution = new DirectoryTree
            {
                ["global.json"] = new JObject
                {
                    ["packages"] = "packages"
                },
                ["NuGet.Config"] = @"<configuration>
  <packageSources>
    <clear />
    <add key=""AspNetVNext"" value=""https://www.myget.org/F/aspnetvnext/api/v2"" />
    <add key=""NuGet"" value=""https://nuget.org/api/v2/"" />
     </packageSources >
   </configuration>"
            };

            return solution;
        }
    }
}
