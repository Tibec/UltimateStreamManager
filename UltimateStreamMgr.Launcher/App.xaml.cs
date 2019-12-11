using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UltimateStreamMgr.Launcher
{

    public enum LauncherUpdateSteps
    {
        Downloading = 0,
        Copying = 1
    }

    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string _currentVersion = "";
        private string _lastVersion = "";
        private string _targetVersion = "latest";
        private string _launcherNewVersion = "";
        private bool _launcherRequireStop = false;

        private string _installPackage = _releasePackage;

        private const string _targetVersionFile = "version.txt";
        private const string _releasePackage = "UltimateStreamManager";
        private const string _betaPackage = "UltimateStreamManager-Beta";

        private readonly string _packageDirectory = Path.Combine(
            Environment.ExpandEnvironmentVariables("%USERPROFILE%"),
            ".nuget",
            "packages"
        );

        private string GetPackageDirectory()
        {
            return Path.Combine(_packageDirectory, _installPackage);
        }

        private const string nugetTokenP1 = "a3659f3501ee28d03eae";
        private const string nugetTokenP2 = "d00b887e4b376f8e6803";

        protected override void OnStartup(StartupEventArgs e)
        {
            HandleLauncherUpdate(e);

            if (_launcherRequireStop)
            {
                Shutdown(0);
                return;
            }

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
            try
            {

                using (var client = new HttpClient(new HttpClientHandler
                    {AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate}))
                {
                    client.BaseAddress = new Uri("https://api.github.com/");
                    HttpRequestMessage request =
                        new HttpRequestMessage(HttpMethod.Post, "/graphql");
                    request.Headers.Add("User-Agent", "USM.Launcher");
                    request.Headers.Add("Authorization", "bearer " + nugetTokenP1 + nugetTokenP2);
                    request.Headers.Add("Accept", "application/vnd.github.packages-preview+json");
                    string graphqlRequest = @"
                     { ""query"": 
                       ""query
                        {
                          repository(name:\""UltimateStreamManager\"",owner:\""Tibec\"" ) {
		                    packages(names:\""UltimateStreamManager.Launcher\"", first:1) {
                              nodes {
                                versions(first:1) {
                                  nodes {
                                    version
                                  }
                                }
                              }
                            }    
                          }
                        }""}";
                    request.Content = new StringContent(graphqlRequest.Replace("\r\n", "").Replace("\t", ""));

                    HttpResponseMessage response = client.SendAsync(request).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(result);
                    JArray repo = json.data.repository.packages.nodes;
                        List<string> releases = new List<string>();
                    if (repo.Count > 0)
                    {
                        JObject repoVersion = repo.First().ToObject<JObject>();

                        foreach (var release in repoVersion["versions"]["nodes"])
                        {
                            string n = release["version"].ToString();
                            releases.Add(n);
                        }

                        releases.Sort();
                    }

                    string latestLauncherVersion = releases.Count == 0 ? "" : releases.Last();
                    string currentLauncherVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
                    Console.WriteLine($"Current Launcher : {currentLauncherVersion} | Latest : {latestLauncherVersion}");

                    _launcherNewVersion = latestLauncherVersion;

                    return new[] {_lastVersion, _currentVersion}.OrderBy(v => v).Last() != _currentVersion;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void PerformUpdate(LauncherUpdateSteps currentSteps)
        {
            switch (currentSteps)
            {
                case LauncherUpdateSteps.Downloading:
                    DownloadUpdate();
                    break;

                case LauncherUpdateSteps.Copying: // Where currently being executed from somewhere
                    ApplyUpdate();
                    break;
            }
        }

        private void ApplyUpdate()
        {
            File.Copy(Assembly.GetExecutingAssembly().Location, "UltimateStreamMgr.Launcher.exe", true);
            Process process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    FileName = "UltimateStreamMgr.Launcher.exe",
                    UseShellExecute = false
                }
            };

            process.Start();
            _launcherRequireStop = true;
        }

        #endregion


        private void Start()
        {
            string exe = Path.Combine(GetPackageDirectory(), _targetVersion == "latest" ? _lastVersion : _targetVersion, "UltimateStreamMgr.exe");
            Process process = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    FileName = exe,
                    UseShellExecute = false
                }
            };

            process.Start();
        }


        private bool AlreadyInstalled()
        {
            if (!Directory.Exists(GetPackageDirectory()) || Directory.GetDirectories(GetPackageDirectory()).Length == 0)
                return false;

            var directories = Directory.EnumerateDirectories(GetPackageDirectory());
            var selected = directories.OrderBy(d => d).Last();

            Console.WriteLine("Found currentVersion : " + selected);

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
                        new HttpRequestMessage(HttpMethod.Post, "/graphql");
                    request.Headers.Add("User-Agent", "USM.Launcher");
                    request.Headers.Add("Authorization", "bearer " + nugetTokenP1+nugetTokenP2);
                    request.Headers.Add("Accept", "application/vnd.github.packages-preview+json");
                    string graphqlRequest = @"
                     { ""query"": 
                       ""query {
                          repository(owner:\""Tibec\"", name:\""UltimateStreamManager\"") {
                            name
                            packages(last: 3)
                            {
                              nodes {
                                name
                                versions(last:100) {
                                  nodes {
                                    version
                                  }
                                }
                              }
                            }
                           }
                        }""
                     }";
                    request.Content = new StringContent(graphqlRequest.Replace("\r\n", "").Replace("\t", ""));
                         
                    HttpResponseMessage response = client.SendAsync(request).Result;
                    string result = response.Content.ReadAsStringAsync().Result;
                    dynamic json = JsonConvert.DeserializeObject(result);
                    JArray repo = json.data.repository.packages.nodes;
                    JObject repoVersion = repo.First(e => (e as JObject)["name"].ToString() == _installPackage).ToObject<JObject>();
                    List<string> releases = new List<string>();

                    foreach (var release in repoVersion["versions"]["nodes"])
                    {
                        string n = release["version"].ToString();
                        releases.Add(n);
                    }

                    releases.Sort();

                    _lastVersion = releases.Last();

                    Console.WriteLine($"Current : {_currentVersion} | Latest : {_lastVersion}"); 

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
            using (new Notification("UltimateStreamManager", "Updating to v" + (_targetVersion == "latest" ? _lastVersion : _targetVersion) + " ...",
                NotificationType.Info))
            {

                string outputDirectory = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Path.GetRandomFileName());
                Directory.CreateDirectory(outputDirectory);

                string versionToInstall = _targetVersion == "latest" ? _lastVersion : _targetVersion;

                InstallNugetPackage(_installPackage, versionToInstall, outputDirectory);

                Directory.Delete(outputDirectory, true);
            }

        }

        private void DownloadUpdate()
        {
            using (new Notification("UltimateStreamManager", "Updating launcher ...",
                NotificationType.Info))
            {

                string outputDirectory = Path.Combine(Environment.ExpandEnvironmentVariables("%TEMP%"), Path.GetRandomFileName());
                Directory.CreateDirectory(outputDirectory);

                InstallNugetPackage("UltimateStreamManager.Launcher", _launcherNewVersion, outputDirectory);

                Process process = new Process
                {
                    StartInfo =
                    {
                        WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                        FileName = Path.Combine(outputDirectory,"UltimateStreamMgr.Launcher.exe"),
                        UseShellExecute = false,
                        Arguments = "update 2"
                    }
                };

                process.Start();
                _launcherRequireStop = true;
            }
        }

        private void InstallNugetPackage(string packageName, string packageVersion, string outputDirectory)
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

            string installPackageCommand =
                $"install {packageName} -Version {packageVersion} -OutputDirectory {outputDirectory} -NonInteractive -source GPR_USM ";

            NugetUtils.RunCommand(installPackageCommand);
        }
    }
}
