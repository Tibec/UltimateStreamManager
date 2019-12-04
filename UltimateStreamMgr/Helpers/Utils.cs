using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Helpers
{
    public static class Utils
    {
        public static string RunDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        } 
    }
}
