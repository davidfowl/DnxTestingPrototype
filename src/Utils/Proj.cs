using System;
using System.IO;
using Microsoft.Framework.Runtime;
using Newtonsoft.Json.Linq;
using NuGet;

namespace Utils
{
    public class Proj
    {
        private Project _project;

        public Proj(Project project)
        {
            _project = project;
        }

        public string Name
        {
            get
            {
                return _project.Name;
            }
        }

        public SemanticVersion Version
        {
            get
            {
                return _project.Version;
            }
        }

        public string ProjectDirectory
        {
            get
            {
                return _project.ProjectDirectory;
            }
        }

        public string ProjectFilePath
        {
            get
            {
                return _project.ProjectFilePath;
            }
        }

        public string BinPath
        {
            get
            {
                return Path.Combine(ProjectDirectory, "bin");
            }
        }

        public string LocalPackagesDir
        {
            get
            {
                return Path.Combine(ProjectDirectory, "packages");
            }
        }

        public void Update(Action<JObject> updateContents)
        {
            TestUtils.UpdateJson(ProjectFilePath, updateContents);

            // Reparse
            Project.TryGetProject(ProjectFilePath, out _project);
        }
    }
}
