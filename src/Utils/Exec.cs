using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Framework.Runtime;

namespace Utils
{
    public class Exec
    {
        public static int Run(
            string program,
            string commandLine,
            out string stdOut,
            out string stdErr,
            Action<Dictionary<string, string>> envSetup = null,
            string workingDir = null)
        {
            var env = new Dictionary<string, string>();
            envSetup?.Invoke(env);

            var processStartInfo = new ProcessStartInfo()
            {
                UseShellExecute = false,
                WorkingDirectory = workingDir,
                FileName = program,
                Arguments = commandLine,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (var pair in env)
            {
#if DNX451
                processStartInfo.EnvironmentVariables[pair.Key] = pair.Value;
#else
                processStartInfo.Environment.Add(pair);
#endif
            }

            var process = Process.Start(processStartInfo);
            process.EnableRaisingEvents = true;

            var stdoutBuilder = new StringBuilder();
            var stderrBuilder = new StringBuilder();

            process.OutputDataReceived += (sender, args) =>
            {
                // If it is not EOF, we always write out a line
                // This should preserve blank lines
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data);
                    stdoutBuilder.AppendLine(args.Data);
                }
            };

            process.ErrorDataReceived += (sender, args) =>
            {
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data);
                    stderrBuilder.AppendLine(args.Data);
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
            stdOut = stdoutBuilder.ToString();
            stdErr = stderrBuilder.ToString();

            return process.ExitCode;
        }
    }
}
