using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClassLibrary31
{
    public class Dnx
    {
        private readonly string _sdkPath;

        public Dnx(string sdkPath)
        {
            _sdkPath = sdkPath;
        }

        public int Execute(string commandLine, out string stdOut, out string stdErr)
        {
            var dnxPath = Path.Combine(_sdkPath, "bin", "dnx");
            return Exec.Run(dnxPath, commandLine, out stdOut, out stdErr);
        }
    }
}
