﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary31
{
    public class DnxSdk
    {
        public string Version { get; set; }

        public string Flavor { get; set; }

        public string Architecture { get; set; }

        public string OperationSystem { get; set; }

        public string Path { get; set; }

        public Dnu Dnu => new Dnu(Path);

        public Dnx Dnx => new Dnx(Path);

        public static DnxSdk GetRuntime(string version)
        {
            return GetRuntime(version, "clr", "win", "x86");
        }

        public static DnxSdk GetRuntime(string version, string flavor, string os, string arch)
        {
            var homePath = Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable("DNX_HOME"));

            if (string.IsNullOrEmpty(homePath))
            {
                var basePath = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrEmpty(basePath))
                {
                    basePath = Environment.GetEnvironmentVariable("USERPROFILE");
                }

                homePath = System.IO.Path.Combine(basePath, ".dnx");
            }

            return GetRuntime(homePath, version, flavor, os, arch);
        }

        public static DnxSdk GetRuntime(string basePath, string version, string flavor, string os, string arch)
        {
            return new DnxSdk
            {
                Path = System.IO.Path.Combine(basePath, "runtimes", GetRuntimeName(flavor, os, arch) + $".{version}"),
                Architecture = arch,
                Flavor = flavor,
                OperationSystem = os,
                Version = version
            };
        }

        private static string GetRuntimeName(string flavor, string os, string architecture)
        {
            // Mono ignores os and architecture
            if (string.Equals(flavor, "mono", StringComparison.OrdinalIgnoreCase))
            {
                return "dnx-mono";
            }

            return $"dnx-{flavor}-{os}-{architecture}";
        }
    }
}
