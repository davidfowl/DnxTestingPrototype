using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Utils
{
    public class Solution
    {
        public Solution(string rootPath)
        {
            RootPath = rootPath;
        }

        public string RootPath { get; private set; }

        public string ArtifactsPath
        {
            get
            {
                return Path.Combine(RootPath, "artifacts");
            }
        }

        public string SourcePath
        {
            get
            {
                return Path.Combine(RootPath, "src");
            }
        }

        public string GetProjectPath(string name)
        {
            return Path.Combine(SourcePath, name);
        }
    }
}
