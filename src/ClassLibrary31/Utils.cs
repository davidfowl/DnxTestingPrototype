using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Framework.Runtime;

namespace ClassLibrary31
{
    public static class Utils
    {
        public static Solution GetSolution(string solutionName, bool shared = false)
        {
            var rootPath = ProjectResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());
            var originalSolutionPath = Path.Combine(Path.Combine(rootPath, "solutions", solutionName));
            if (shared)
            {
                return new Solution(originalSolutionPath);
            }

            var tempSolutionPath = GetLocalTempFolder();
            CopyFolder(originalSolutionPath, tempSolutionPath);
            return new Solution(tempSolutionPath);
        }

        public static string GetLocalTempFolder()
        {
            // This env var can be set by VS load profile
            var basePath = Environment.GetEnvironmentVariable("DNX_LOCAL_TEMP_FOLDER_FOR_TESTING");
            if (string.IsNullOrEmpty(basePath))
            {
                var rootPath = ProjectResolver.ResolveRootDirectory(Directory.GetCurrentDirectory());
                basePath = Path.Combine(rootPath, "artifacts");
            }

            var tempFolderPath = Path.Combine(basePath, Path.GetRandomFileName());
            Directory.CreateDirectory(tempFolderPath);
            return tempFolderPath;
        }

        public static void CopyFolder(string sourceFolder, string targetFolder)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            foreach (var filePath in Directory.EnumerateFiles(sourceFolder))
            {
                var fileName = Path.GetFileName(filePath);
                File.Copy(filePath, Path.Combine(targetFolder, fileName));
            }

            foreach (var folderPath in Directory.EnumerateDirectories(sourceFolder))
            {
                var folderName = new DirectoryInfo(folderPath).Name;
                CopyFolder(folderPath, Path.Combine(targetFolder, folderName));
            }

        }
    }
}