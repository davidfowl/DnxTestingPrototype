﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Framework.Runtime;
using Newtonsoft.Json.Linq;

namespace Utils
{
    public static class TestUtils
    {
        public static string NormalizeJson(string json)
        {
            return JObject.Parse(json).ToString();
        }

        public static string LoadNormalizedJson(string path)
        {
            return NormalizeJson(File.ReadAllText(path));
        }

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

        public static void UpdateJson(string path, Action<JObject> action)
        {
            var json = JObject.Parse(File.ReadAllText(path));
            action(json);
            File.WriteAllText(path, json.ToString());
        }

        public static string CreateLocalFeed(Solution solution)
        {
            var sdk = DnxSdkFunctionalTestBase.DnxSdks.First()[0] as DnxSdk;
            var feed = GetLocalTempFolder();
            var packOutput = GetLocalTempFolder();

            sdk.Dnu.RestoreAndCheckExitCode(solution.RootPath);
            foreach (var project in solution.Projects)
            {
                var output = sdk.Dnu.PackAndCheckExitCode(project.ProjectFilePath, packOutput);
                sdk.Dnu.PackagesAddAndCheckExitCode(output.PackagePath, feed);
            }

            return feed;
        }
    }
}