using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utils
{
    public class DirDiff
    {
        public IEnumerable<string> MissingEntries { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> ExtraEntries { get; set; } = Enumerable.Empty<string>();
        public IEnumerable<string> DifferentEntries { get; set; } = Enumerable.Empty<string>();

        public bool IsEmpty
        {
            get
            {
                return !MissingEntries.Any() && !ExtraEntries.Any() && !DifferentEntries.Any();
            }
        }
    }
}
