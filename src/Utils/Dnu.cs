using System;
using System.IO;
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

        public ExecResult Publish(
            string projectPath,
            string outputPath,
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

            return Execute(sb.ToString(), envSetup);
        }

        public ExecResult Restore(string projectPath)
        {
            var sb = new StringBuilder();
            sb.Append("restore ");
            sb.Append($@"""{projectPath}""");

            return Execute(sb.ToString());
        }

        public ExecResult PackagesAdd(
            string packagePath,
            string packagesDir,
            Action<Dictionary<string, string>> envSetup = null)
        {
            return Execute($"packages add {packagePath} {packagesDir}", envSetup);
        }

        public ExecResult Wrap(string csprojPath)
        {
            return Execute($"wrap {csprojPath}");
        }

        public DnuPackOutput Pack(string projectPath, string outputPath, string configuration = "Debug")
        {
            var sb = new StringBuilder();
            sb.Append("pack ");
            sb.Append($@"""{projectPath}""");
            sb.Append($@" --out ""{outputPath}""");
            sb.Append($" --configuration {configuration}");

            var result = Execute(sb.ToString());

            var projectName = new DirectoryInfo(projectPath).Name;
            return new DnuPackOutput(outputPath, projectName, configuration)
            {
                ExitCode = result.ExitCode,
                StandardError = result.StandardError,
                StandardOutput = result.StandardOutput
            };
        }

        public ExecResult Execute(string commandLine)
        {
            return Execute(commandLine);
        }

        public ExecResult Execute(string commandLine, Action<Dictionary<string, string>> envSetup = null)
        {
            var dnxPath = Path.Combine(_sdkPath, "bin", "dnu.cmd");
            return Exec.Run(dnxPath, commandLine, envSetup);
        }
    }
}
