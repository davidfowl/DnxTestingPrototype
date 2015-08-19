using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Runtime;

namespace Utils
{
    public class Dnx
    {
        private readonly string _sdkPath;

        public Dnx(string sdkPath)
        {
            _sdkPath = sdkPath;
        }

        public ExecResult Execute(string commandLine, bool dnxTraceOn = true)
        {
            var dnxPath = Path.Combine(_sdkPath, "bin", "dnx");
            return Exec.Run(
                dnxPath,
                commandLine,
                env => env[EnvironmentNames.Trace] = dnxTraceOn ? "1" : null);
        }
    }
}
