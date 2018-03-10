using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using UltimateStreamMgr.Model;

namespace UltimateStreamMgr.ViewModel
{
    class ReplayViewModel : ViewModelBase
    {
        private string _watchFolder;
        private string _outputFolder;
        private Timer _timer;

        public ReplayViewModel()
        {
            ClearPlaylistCommand = new RelayCommand(() => ClearPlaylist());

            _timer = new Timer(500);
            _timer.Elapsed += ScanFolder;

            ReloadSettings();
            Configuration.Instance.ReplaySettingsChanged += ReloadSettings;


            Playlist.CollectionChanged += UpdateReplayList;
        }

        private void UpdateReplayList(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (Replay item in e.NewItems)
                    item.PropertyChanged += UpdateReplayListItem ;

            if (e.OldItems != null)
                foreach (Replay item in e.OldItems)
                    item.PropertyChanged -= UpdateReplayListItem;
        }

        private void UpdateReplayListItem(object sender, PropertyChangedEventArgs e)
        {
            Output.Data.Replays = Playlist.ToList().FindAll(r => r.Play);
        }

        private void ScanFolder(object sender, ElapsedEventArgs e)
        {
            DirectoryInfo watchDirInfo = new DirectoryInfo(_watchFolder);
            FileInfo[] replays = watchDirInfo.GetFiles("Replay_*");
            foreach(var replay in replays)
            {
                try
                {
                    string newName = GenerateReplayName(replay.Extension);
                    MoveLastReplay(replay, newName);
                    Replay newReplay = new Replay { Name = newName, FilePath = replay.FullName };
                    LastReplay = newReplay;
                    Application.Current.Dispatcher.Invoke(() => Playlist.Insert(0,newReplay));
                }
                catch (Exception ex) {
                    // Happen if the file is not ready to be moved
                }
            }
            _timer.Start();
        }

        private void MoveLastReplay(FileInfo replay, string newName)
        {
            replay.MoveTo(Path.Combine(_outputFolder, "playlist", newName));
        }

        private string GenerateReplayName(string extension)
        {
            string name = "";
            if(!string.IsNullOrEmpty(Output.Data.RoundName))
            {
                name += Output.Data.RoundName.Replace(' ', '-') + "_";
            }
            if(!string.IsNullOrEmpty(Output.Data.Player1?.Name) 
                && !string.IsNullOrEmpty(Output.Data.Player2?.Name))
            {
                name += Output.Data.Player1.Name + "_vs_" + Output.Data.Player2.Name;
            }
            if (File.Exists(Path.Combine(_outputFolder, "playlist", name + extension)))
            {
                int similarity = 1;
                while (File.Exists(Path.Combine(_outputFolder, "playlist", name + "-"+ similarity + extension)))
                {
                    ++similarity;
                }
                name += "-" + similarity;
            } 
            return name + extension;
        }

        private void ReloadSettings()
        {
            ReplayEnabled = Configuration.Instance.Replay.Enabled;
            _watchFolder = Configuration.Instance.Replay.InputDirectory;
            _outputFolder = Configuration.Instance.Replay.OutputDirectory;
            if (ReplayEnabled)
                _timer.Start();
            else
                _timer.Stop();
            CreateOutputStructure();
        }

        private void CreateOutputStructure()
        {
            if (string.IsNullOrEmpty(_outputFolder))
                return;
            Directory.CreateDirectory(Path.Combine(_outputFolder, "playlist"));
            Directory.CreateDirectory(Path.Combine(_outputFolder, "old"));

            string[] existing = Directory.GetFiles(Path.Combine(_outputFolder, "playlist"));
            string backupdir = Path.Combine(_outputFolder, "old", DateTime.Now.ToString("yyyyMMdd-HHmmss"));
            if (existing.Length > 0)
                Directory.CreateDirectory(backupdir);
            foreach(var file in existing)
            {
                FileInfo fi = new FileInfo(file);
                fi.MoveTo(Path.Combine(backupdir, fi.Name));
            }
        }

        private bool _replayEnabled;
        public bool ReplayEnabled
        {
            get { return _replayEnabled; }
            set { Set("ReplayEnabled", ref _replayEnabled, value); }
        }

        private Replay _lastReplay;
        public Replay LastReplay
        {
            get { return _lastReplay; }
            set { Set("LastReplay", ref _lastReplay, value); }
        }

        private ObservableCollection<Replay> _playlist = new ObservableCollection<Replay>();
        public ObservableCollection<Replay> Playlist
        {
            get { return _playlist; }
            set { Set("Playlist", ref _playlist, value); }
        }

        public RelayCommand ClearPlaylistCommand { get; set; }
        private void ClearPlaylist()
        {
            CreateOutputStructure();
            LastReplay = null;
            Playlist.Clear();
        }
    }
}
