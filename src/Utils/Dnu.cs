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
            var sb = new StringBuilder();
            sb.Append("restore ");
            sb.Append($@"""{projectPath}""");

            int exitCode = Execute(sb.ToString());

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Restore failed! Exit code was {exitCode}");
            }
        }

        public void BuildAndCheckExitCode(string projectPath, string outputPath, string configuration = "Debug")
        {
            var sb = new StringBuilder();
            sb.Append("build ");
            sb.Append($@"""{projectPath}""");
            sb.Append($@" --out ""{outputPath}""");
            sb.Append($" --configuration {configuration}");

            int exitCode = Execute(sb.ToString());

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Build failed! Exit code was {exitCode}");
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
