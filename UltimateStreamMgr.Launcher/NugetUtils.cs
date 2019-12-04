using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const string _nugetAssemblyPath = "UltimateStreamMgr.Launcher.nuget.exe";

        private static string GetNugetPath()
        {
            if (!NugetIsAlreadyExtracted())
            {
                if (!Directory.Exists(_nugetDirectory))
                    Directory.CreateDirectory(_nugetDirectory);
                File.WriteAllBytes(Path.Combine(_nugetDirectory, "nuget.exe"), ExtractNugetFromAssembly());
            }
            return Path.Combine(_nugetDirectory, "nuget.exe");
        }

        private static bool NugetIsAlreadyExtracted()
        {
            return File.Exists(Path.Combine(_nugetDirectory, "nuget.exe"));
        }

        public static void RunCommand(string arguments)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = GetNugetPath(),
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
            };

            var process = new Process();

            DataReceivedEventHandler redirectOutputHandler = (sender, args) =>
            {
                if (string.IsNullOrEmpty(args.Data))
                    return;

                Console.WriteLine(args.Data);
            };

            process.StartInfo = processStartInfo;

            process.OutputDataReceived += redirectOutputHandler;
            process.ErrorDataReceived += redirectOutputHandler;
            
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();
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
