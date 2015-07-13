using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Utils
{
    public class Dnu
    {
        private readonly string _sdkPath;

        public Dnu(string sdkPath)
        {
            _sdkPath = sdkPath;
        }

        public int Publish(
            string projectPath,
            string outputPath,
            out string stdOut,
            out string stdErr,
            bool noSource = false,
            string additionalArguments = null,
            Action<Dictionary<string, string>> envSetup = null)
        {
            var sb = new StringBuilder();
            sb.Append("publish ");
            sb.Append($@"""{projectPath}""");
            if (noSource)
            {
                sb.Append(" --no-source");
            }

            sb.Append($@" --out ""{outputPath}""");
            sb.Append($" {additionalArguments}");

            return Execute(sb.ToString(), out stdOut, out stdErr, envSetup);
        }

        public int Restore(
            string projectPath,
            string packagesPath,
            IEnumerable<string> feeds,
            out string stdOut,
            out string stdErr,
            string additionalArguments = null,
            Action<Dictionary<string, string>> envSetup = null)
        {
            var sb = new StringBuilder();
            sb.Append("restore");
            sb.Append($" \"{projectPath}\"");

            if (!string.IsNullOrEmpty(packagesPath))
            {
                sb.Append($" --packages \"{packagesPath}\"");
            }

            if (feeds != null && feeds.Any())
            {
                sb.Append($" -s {string.Join(" -s ", feeds)}");
            }

            sb.Append($" {additionalArguments}");

            return Execute(sb.ToString(), out stdOut, out stdErr, envSetup);
        }

        public int PackagesAdd(
            string packagePath,
            string packagesDir,
            out string stdOut,
            out string stdErr,
            Action<Dictionary<string, string>> envSetup = null)
        {
            return Execute($"packages add {packagePath} {packagesDir}", out stdOut, out stdErr, envSetup);
        }

        public int Wrap(string csprojPath)
        {
            return Execute($"wrap {csprojPath}");
        }

        public void PublishAndCheckExitCode(
            string projectPath,
            string outputPath,
            bool noSource = false,
            Action<Dictionary<string, string>> envSetup = null)
        {
            string stdOut, stdErr;
            int exitCode = Publish(projectPath, outputPath, out stdOut, out stdErr, noSource, envSetup: envSetup);

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Publish failed! Exit code was {exitCode}");
            }
        }

        public void RestoreAndCheckExitCode(string projectPath)
        {
            string stdOut, stdErr;
            var exitCode = Restore(
                projectPath,
                packagesPath: null,
                feeds: null,
                stdOut: out stdOut,
                stdErr: out stdErr);

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Restore failed! Exit code was {exitCode}");
            }
        }

        public DnuPackOutput PackAndCheckExitCode(string projectPath, string outputPath, string configuration = "Debug")
        {
            var sb = new StringBuilder();
            sb.Append("pack ");
            sb.Append($@"""{projectPath}""");
            sb.Append($@" --out ""{outputPath}""");
            sb.Append($" --configuration {configuration}");

            int exitCode = Execute(sb.ToString());

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Pack failed! Exit code was {exitCode}");
            }

            var projectDir = new DirectoryInfo(projectPath);
            return new DnuPackOutput(
                outputPath,
                packageName: projectDir.Exists ? projectDir.Name: projectDir.Parent.Name,
                configuration: configuration);
        }

        public void PackagesAddAndCheckExitCode(
            string packagePath,
            string packagesDir)
        {
            string stdOut, stdErr;
            int exitCode = PackagesAdd(packagePath, packagesDir, out stdOut, out stdErr);

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Packages installation failed! Exit code was {exitCode}");
            }
        }

        public int Execute(string commandLine)
        {
            string stdOut;
            string stdErr;
            return Execute(commandLine, out stdOut, out stdErr);
        }

        public int Execute(string commandLine, out string stdOut, out string stdErr, Action<Dictionary<string, string>> envSetup = null)
        {
            var dnxPath = Path.Combine(_sdkPath, "bin", "dnu.cmd");
            return Exec.Run(dnxPath, commandLine, out stdOut, out stdErr, envSetup);
        }
    }
}
