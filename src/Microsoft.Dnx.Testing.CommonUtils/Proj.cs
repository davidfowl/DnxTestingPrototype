using System;
using System.IO;
using Newtonsoft.Json.Linq;
using NuGet;

namespace Microsoft.Dnx.Testing
{
    public class Proj
    {
        private Runtime.Project _project;

        public Proj(Runtime.Project project)
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
            Runtime.Project.TryGetProject(ProjectFilePath, out _project);
        }
    }
}
