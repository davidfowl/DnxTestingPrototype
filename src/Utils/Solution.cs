using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Framework.Runtime;

namespace Utils
{
    public class Solution
    {
        private readonly ProjectResolver _projectResolver;
        public Solution(string rootPath)
        {
            RootPath = rootPath;
            _projectResolver = new ProjectResolver(rootPath);
        }

        public string RootPath { get; private set; }

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

        public Proj GetProject(string name)
        {
            Project project;
            if (!_projectResolver.TryResolveProject(name, out project))
            {
                throw new InvalidOperationException($"Unable to resolve project '{name}' from '{RootPath}'");
            }
            return new Proj(project);
        }
    }
}
