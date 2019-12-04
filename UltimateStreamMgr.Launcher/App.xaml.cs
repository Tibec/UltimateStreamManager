using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json;

namespace UltimateStreamMgr.Launcher
{

    public enum LauncherUpdateSteps
    {
        Downloading = 0,
        Copying = 1,
        Deleting = 2,
    }

    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string _currentVersion = "";
        private string _lastVersion = "";
        private string _targetVersion = "latest";

        private string _installPackage = _releasePackage;

        private const string _targetVersionFile = "version.txt";
        private const string _releasePackage = "UltimateStreamManager";
        private const string _betaPackage = "UltimateStreamManager-Beta";

        private readonly string _packageDirectory = Path.Combine(
            Environment.ExpandEnvironmentVariables("%USERPROFILE%"),
            ".nuget",
            "packages",
            "ultimatestreammanager"
        );

        private const string nugetTokenP1 = "a3659f3501ee28d03eae";
        private const string nugetTokenP2 = "d00b887e4b376f8e6803";

        protected override void OnStartup(StartupEventArgs e)
        {
            // HandleLauncherUpdate(e);
            LoadTargetVersion();
            ParseArguments(e);

            bool installed = AlreadyInstalled();
            bool lookForUpdate = _targetVersion != _currentVersion;
            bool updateAvailable = UpdateAvailable();


            if (!installed && !updateAvailable) // User have not a version and doesn't have internet
            {
                using (new Notification("UltimateStreamManager", $"You need to have access to internet for the first launcher !",
                    NotificationType.Error))
                {
                    Shutdown(1);
                    return;
                }
            }
            if (!installed || (installed && lookForUpdate && updateAvailable))
            {
                Install();
            }

            Start();
            Shutdown(0);
        }

        private void LoadTargetVersion()
        {
            if (File.Exists(_targetVersionFile))
            {
                ParseTargetVersion(File.ReadAllText(_targetVersionFile).Trim());
            }
        }

        private void ParseTargetVersion(string targetVersion)
        {
            if (targetVersion.StartsWith("beta-"))
            {
                _targetVersion = targetVersion.Substring(5);
                _installPackage = _betaPackage;
            }
            else
            {
                _targetVersion = targetVersion;
                _installPackage = _releasePackage;
            }
        }


        private void ParseArguments(StartupEventArgs e)
        {
            if (e.Args.Length == 2)
            {
                if (e.Args[0] == "version")
                {
                    ParseTargetVersion(e.Args[1]);
                }
            }
        }

        #region Launcher Update Flow

        private void HandleLauncherUpdate(StartupEventArgs e)
        {
            if (LauncherUpdateAvailable())
            {
                PerformUpdate(LauncherUpdateSteps.Downloading);
            }
            else if (e.Args.Length == 2 && e.Args[0] == "update")
            {
                LauncherUpdateSteps current = (LauncherUpdateSteps) Enum.Parse(typeof(LauncherUpdateSteps), e.Args[1]);
                PerformUpdate(current);
            }
        }

        private bool LauncherUpdateAvailable()
        {
            return false;
        }

        private void PerformUpdate(LauncherUpdateSteps currentSteps)
        {
            switch (currentSteps)
            {
                case LauncherUpdateSteps.Downloading:
                    //Store Launcher.update.exe and launch it
                    break;

                case LauncherUpdateSteps.Copying:
                    // Copy Launcher.update.exe as Launcher.exe and launch it
                    break;

                case LauncherUpdateSteps.Deleting:
                    // Deleting Launcher.update.exe and resume normal startup
                    break;

            }
        }

        #endregion


        private void Start()
        {
            string exe = Path.Combine(_packageDirectory, _lastVersion, "UltimateStreamMgr.exe");
            Process process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    FileName = exe
                }
            };

            process.Start();
        }


        private bool AlreadyInstalled()
        {
            if (!Directory.Exists(_packageDirectory) || Directory.GetDirectories(_packageDirectory).Length == 0)
                return false;

            var directories = Directory.EnumerateDirectories(_packageDirectory);
            var selected = directories.OrderBy(d => d).Last();

            System.Console.WriteLine("Found currentVersion : " + selected);

            _currentVersion = Path.GetFileName(selected);

            return true;
        }

        private bool UpdateAvailable()
        {
            try
            {

                using (var client = new HttpClient(new HttpClientHandler
                    {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}))
                {
                    client.BaseAddress = new Uri("https://api.github.com/");
                    HttpRequestMessage request =
                        new HttpRequestMessage(HttpMethod.Get, "/repos/Tibec/UltimateStreamManager/releases");
                    request.Headers.Add("User-Agent", "USM.Launcher");

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(result);

                    List<string> releases = new List<string>();


                    foreach (var release in json)
                    {
                        string n = release.tag_name;
                        releases.Add(n);
                    }

                    releases.Sort();

                    _lastVersion = releases.Last();

                    System.Console.WriteLine($"Current : {_currentVersion} | Latest : {_lastVersion}");

                    return new[] {_lastVersion, _currentVersion}.OrderBy(v => v).Last() != _currentVersion;
                }
            }
            catch
            {
                return false;
            }
        }

        private void Install()
        {
            using (new Notification("UltimateStreamManager", $"Updating to v{_targetVersion} ...",
                NotificationType.Info))
            {

                // Well i let that in clear, because it's a random account i created specially for this with only repo and package:read rights
                // It should be harmless.. Maybe.
                string removeRepoCommand =
                    "sources Remove -Name GPR_USM";
                string installRepoCommand =
                    "sources Add -Name GPR_USM -Source https://nuget.pkg.github.com/Tibec/index.json -UserName userbidon42 -Password " +
                    nugetTokenP1 + nugetTokenP2;

                NugetUtils.RunCommand(removeRepoCommand);
                NugetUtils.RunCommand(installRepoCommand);

                string outputDirectory = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Path.GetRandomFileName());

                Directory.CreateDirectory(outputDirectory);

                string installPackageCommand =
                    $"install {_installPackage} -Version {_targetVersion} -OutputDirectory {outputDirectory} -NonInteractive -source GPR_USM ";

                NugetUtils.RunCommand(installPackageCommand);

                Directory.Delete(outputDirectory, true);
            }

        }

    }
}