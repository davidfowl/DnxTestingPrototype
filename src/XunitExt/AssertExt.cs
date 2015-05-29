using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit.Sdk;
using Utils;

namespace XunitExt
{
    public class DirAssert
    {
        public static void Equal(Dir expected, Dir actual)
        {
            var diff = actual.Diff(expected);
            if (diff.IsEmpty)
            {
                return;
            }
            throw new DirMismatchException(expected.LoadPath, actual.LoadPath, diff);
        }
    }
}
