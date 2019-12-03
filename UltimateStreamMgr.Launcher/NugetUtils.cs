using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Launcher
{
    public static class NugetUtils
    {
        private static readonly string _nugetDirectory = Environment.ExpandEnvironmentVariables(@"%USERPROFILE%\.nuget\tools\5.3.1");
        private const string _nugetAssemblyPath = "UltimateStreamMgr.Launcher.nuget";

        private static string GetNugetPath()
        {
            if (!NugetIsAlreadyExtracted())
            {
                File.WriteAllBytes(_nugetDirectory, ExtractNugetFromAssembly());
            }
            return Path.Combine(_nugetDirectory, "nuget.exe");
        }

        private static bool NugetIsAlreadyExtracted()
        {
            return File.Exists(Path.Combine(_nugetDirectory, "nuget.exe"));
        }

        public static string RunCommand(string arguments)
        {
            // TODO
            return "fuck";
        }

        private static byte[] ExtractNugetFromAssembly()
        {
            Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(_nugetAssemblyPath))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }

        }

    }
}
