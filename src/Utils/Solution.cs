﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Framework.Runtime;

namespace Utils
{
    public class Solution
    {
        public Solution(string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; private set; }

        public string GlobalFilePath
        {
            get
            {
                return Path.Combine(RootPath, GlobalSettings.GlobalFileName);
            }
        }

        public string ArtifactsPath
        {
            get
            {
                return Path.Combine(RootPath, "artifacts");
            }
        }

        public string LocalPackagesDir
        {
            get
            {
                return Path.Combine(RootPath, "packages");
            }
        }

        public string SourcePath
        {
            get
            {
                return Path.Combine(RootPath, "src");
            }
        }

        public string WrapFolderPath
        {
            get
            {
                return Path.Combine(RootPath, "wrap");
            }
        }

        public Proj GetProject(string name)
        {
            Project project;
            var resolver = new ProjectResolver(RootPath);
            if (!resolver.TryResolveProject(name, out project))
            {
                throw new InvalidOperationException($"Unable to resolve project '{name}' from '{RootPath}'");
            }
            return new Proj(project);
        }

        public string GetWrapperProjectPath(string name)
        {
            var path = Path.Combine(WrapFolderPath, name, Project.ProjectFileName);
            if (!Directory.Exists(path))
            {
                throw new InvalidOperationException($"Unable to find wrapper project {path}");
            }
            return path;
        }

        public string GetCsprojPath(string name)
        {
            return Path.Combine(RootPath, name, $"{name}.csproj");
        }
    }
}
