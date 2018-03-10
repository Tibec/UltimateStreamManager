using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Helpers
{
    static public class Utils
    {
        static public string RunDirectory()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        } 
    }
}
