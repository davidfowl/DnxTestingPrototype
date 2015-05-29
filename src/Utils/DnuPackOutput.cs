﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Microsoft.Framework.Runtime;
using NuGet;

namespace Utils
{
    public class DnuPackOutput
    {
        public DnuPackOutput(string outputPath, string packageName, string configuration)
        {
            RootPath = outputPath;
            PackageName = packageName;
            Configuration = configuration;
            var basePath = Path.Combine(RootPath, Configuration);
            PackagePath = Directory.EnumerateFiles(basePath, $"*{NuGet.Constants.PackageExtension}")
                .FirstOrDefault(x => !x.EndsWith($"*.symbols{NuGet.Constants.PackageExtension}"));
            if (string.IsNullOrEmpty(PackagePath))
            {
                throw new InvalidOperationException($"Could not find NuGet package in '{outputPath}'");
            }
        }

        public string RootPath { get; private set; }

        public string Configuration { get; private set; }

        public string PackageName { get; private set; }

        public string PackagePath { get; private set; }

        public string GetAssemblyPath(FrameworkName framework)
        {
            var shortName = VersionUtility.GetShortFrameworkName(framework);
            return Path.Combine(RootPath, Configuration, shortName, $"{PackageName}.dll");
        }
    }
}