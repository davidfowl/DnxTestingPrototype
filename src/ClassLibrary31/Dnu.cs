using System;
using System.IO;
using System.Text;

namespace ClassLibrary31
{
    public class Dnu
    {
        private readonly string _sdkPath;

        public Dnu(string sdkPath)
        {
            _sdkPath = sdkPath;
        }

        public void Publish(string projectPath, string outputPath, bool noSource = false)
        {
            var sb = new StringBuilder();
            sb.Append("publish ");
            sb.Append($@"""{projectPath}""");
            if (noSource)
            {
                sb.Append(" --no-source");
            }

            sb.Append($@" --out ""{outputPath}""");

            int exitCode = Execute(sb.ToString());

            if (exitCode != 0)
            {
                throw new InvalidOperationException($"Publish failed! Exit code was {exitCode}");
            }
        }

        public void Restore(string projectPath)
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

        public string Build(string projectPath, string outputPath, string configuration = "Debug")
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

            return Path.Combine(outputPath, configuration);
        }

        public int Execute(string commandLine)
        {
            string stdOut;
            string stdErr;
            return Execute(commandLine, out stdOut, out stdErr);
        }

        public int Execute(string commandLine, out string stdOut, out string stdErr)
        {
            var dnxPath = Path.Combine(_sdkPath, "bin", "dnu.cmd");
            return Exec.Run(dnxPath, commandLine, out stdOut, out stdErr);
        }
    }
}
