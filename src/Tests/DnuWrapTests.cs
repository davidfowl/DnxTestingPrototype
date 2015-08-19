﻿using System.IO;
using System.Linq;
using Microsoft.Framework.Runtime;
using Utils;
using Xunit;
using Newtonsoft.Json.Linq;

namespace Tests
{
    public class DnuWrapTests : DnxSdkFunctionalTestBase
    {
        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuWrapUpdatesExistingProjectJson(DnxSdk sdk)
        {
            if (PlatformHelper.IsMono)
            {
                return;
            }

            var expectedProjectJson = new JObject
            {
                ["version"] = "1.0.0-*",
                ["dependencies"] = new JObject { },
                ["frameworks"] = new JObject
                {
                    ["net45+win+wpa81+wp80"] = new JObject
                    {
                        ["wrappedProject"] = "../../LibraryBeta.PCL/LibraryBeta.PCL.csproj",
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../LibraryBeta.PCL/obj/{configuration}/LibraryBeta.dll",
                            ["pdb"] = "../../LibraryBeta.PCL/obj/{configuration}/LibraryBeta.pdb"
                        }
                    },
                    ["net45"] = new JObject
                    {
                        ["wrappedProject"] = "../../LibraryBeta.PCL.Desktop/LibraryBeta.PCL.Desktop.csproj",
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../LibraryBeta.PCL.Desktop/obj/{configuration}/LibraryBeta.dll",
                            ["pdb"] = "../../LibraryBeta.PCL.Desktop/obj/{configuration}/LibraryBeta.pdb"
                        }
                    },
                    ["wpa81"] = new JObject
                    {
                        ["wrappedProject"] = "../../LibraryBeta.PCL.Phone/LibraryBeta.PCL.Phone.csproj",
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../LibraryBeta.PCL.Phone/obj/{configuration}/LibraryBeta.dll",
                            ["pdb"] = "../../LibraryBeta.PCL.Phone/obj/{configuration}/LibraryBeta.pdb"
                        }
                    }
                }
            };

            var expectedGlobalJson = new JObject
            {
                ["projects"] = new JArray("src", "test")
            };

            const string solutionName = "DnuWrapTestProjects";
            var solution = TestUtils.GetSolution(solutionName, shared: false);
            var libraryBetaProject = solution.GetProject("LibraryBeta");

            sdk.Dnu.Wrap(solution.GetCsprojPath("LibraryBeta.PCL")).EnsureSuccess();

            sdk.Dnu.Wrap(solution.GetCsprojPath("LibraryBeta.PCL.Desktop")).EnsureSuccess();

            sdk.Dnu.Wrap(solution.GetCsprojPath("LibraryBeta.PCL.Phone")).EnsureSuccess();

            // DNX internal JSON writer doesn't follow the indentation convention followed by JSON.NET
            Assert.Equal(expectedGlobalJson.ToString(), TestUtils.LoadNormalizedJson(solution.GlobalFilePath));
            Assert.False(Directory.Exists(solution.WrapFolderPath));
            Assert.Equal(expectedProjectJson.ToString(), TestUtils.LoadNormalizedJson(libraryBetaProject.ProjectFilePath));
        }

        [Theory]
        [MemberData(nameof(DnxSdks))]
        public void DnuWrapMaintainsAllKindsOfReferences(DnxSdk sdk)
        {
            if (PlatformHelper.IsMono)
            {
                return;
            }

            var expectedLibGammaProjectJson = new JObject
            {
                ["version"] = "1.0.0-*",
                ["frameworks"] = new JObject
                {
                    ["net45"] = new JObject
                    {
                        ["wrappedProject"] = "../../LibraryGamma/LibraryGamma.csproj",
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../LibraryGamma/obj/{configuration}/LibraryGamma.dll",
                            ["pdb"] = "../../LibraryGamma/obj/{configuration}/LibraryGamma.pdb"
                        },
                        ["dependencies"] = new JObject
                        {
                            ["EntityFramework"] = "6.1.2-beta1",
                            ["LibraryEpsilon"] = "1.0.0-*",
                            ["LibraryDelta"] = "1.0.0-*",
                        }
                    }
                }
            };

            var expectedLibEpsilonProjectJson = new JObject
            {
                ["version"] = "1.0.0-*",
                ["frameworks"] = new JObject
                {
                    ["net45"] = new JObject
                    {
                        ["wrappedProject"] = "../../LibraryEpsilon/LibraryEpsilon.csproj",
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../LibraryEpsilon/obj/{configuration}/LibraryEpsilon.dll",
                            ["pdb"] = "../../LibraryEpsilon/obj/{configuration}/LibraryEpsilon.pdb"
                        }
                    }
                }
            };

            var expectedLibDeltaProjectJson = new JObject
            {
                ["version"] = "1.0.0-*",
                ["frameworks"] = new JObject
                {
                    ["net45"] = new JObject
                    {
                        ["bin"] = new JObject
                        {
                            ["assembly"] = "../../ExternalAssemblies/LibraryDelta.dll"
                        }
                    }
                }
            };

            var expectedGlobalJson = new JObject
            {
                ["projects"] = new JArray("src", "test", "wrap")
            };

            const string solutionName = "DnuWrapTestProjects";
            var solution = TestUtils.GetSolution(solutionName, shared: false);

            sdk.Dnu.Wrap(solution.GetCsprojPath("LibraryGamma")).EnsureSuccess();

            var libGammaProjectJson = solution.GetProject("LibraryGamma").ProjectFilePath;
            var libEpsilonProjectJson = solution.GetProject("LibraryEpsilon").ProjectFilePath;
            var libDeltaProjectJson = solution.GetProject("LibraryDelta").ProjectFilePath;

            Assert.Equal(expectedGlobalJson.ToString(), TestUtils.LoadNormalizedJson(solution.GlobalFilePath));
            Assert.Equal(3, Directory.EnumerateDirectories(solution.WrapFolderPath).Count());
            Assert.Equal(expectedLibGammaProjectJson.ToString(), TestUtils.LoadNormalizedJson(libGammaProjectJson));
            Assert.Equal(expectedLibEpsilonProjectJson.ToString(), TestUtils.LoadNormalizedJson(libEpsilonProjectJson));
            Assert.Equal(expectedLibGammaProjectJson.ToString(), TestUtils.LoadNormalizedJson(libGammaProjectJson));
        }
    }
}