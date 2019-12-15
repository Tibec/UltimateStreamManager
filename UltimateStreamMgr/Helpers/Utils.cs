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
            return Directory.GetCurrentDirectory();
        }

        public static byte[] ExtractResource(string filename)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            if (a.GetManifestResourceNames().Contains(filename))
            {
                using (Stream resFilestream = a.GetManifestResourceStream(filename))
                {
                    if (resFilestream == null) return null;
                    byte[] ba = new byte[resFilestream.Length];
                    resFilestream.Read(ba, 0, ba.Length);
                    return ba;
                }
            }
            else
            {
                return new byte[0];
            }
        }
    }
}
