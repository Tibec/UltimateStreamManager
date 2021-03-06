﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using UltimateStreamMgr.Model;
using System.ComponentModel;
using UltimateStreamMgr.Helpers;
using System.IO;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows;
using System.Reflection;
using System.Windows.Data;

namespace UltimateStreamMgr.ViewModel
{
    public enum SetMode { Singles, Doubles, Crewbattles };

    public class RunningSetViewModel : DockWindowViewModel
    {
        private Timer _timer;

        public RunningSetViewModel()
        {
            Log.Info("Initializing RunningSet module...");
            Title = "Running Set";
            Reset();

            _timer = new Timer(500);
            _timer.Elapsed += DoLiveUpdate;

            ResetCommand = new RelayCommand(() => Reset());
            ReportCommand = new RelayCommand(() => Report());
            UpdateCommand = new RelayCommand(() => Update());
            SwapPlayerCommand = new RelayCommand(() => SwapPlayer());

            IncrementEntrant1Command = new RelayCommand(IncrementEntrant1);
            DecrementEntrant1Command = new RelayCommand(DecrementEntrant1);
            IncrementEntrant2Command = new RelayCommand(IncrementEntrant2);
            DecrementEntrant2Command = new RelayCommand(DecrementEntrant2);

            MessengerInstance.Register<Set>(this, StartPendingSet);

            ReportEnabled = BracketData.Instance.IsReportingAvailable;

            PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers());
            PlayerDatabase.DatabaseContentModified += RefreshPlayers;

            LoadCharacters();
            LoadTeams();
            Log.Info("RunningSet module initialized !");
        }

        private void RefreshPlayers()
        {
            Dispatcher.CurrentDispatcher.Invoke(() => PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers()));
        }

        private void LoadCharacters()
        {
            try
            {
                ObservableCollection<Character> characters = new ObservableCollection<Character>();
                ObservableCollection<string> categories = new ObservableCollection<string>();
                string rundir = Utils.RunDirectory();
                var subfiles = Directory.GetFiles(Path.Combine(rundir, "characters")).ToList();

                foreach (var subfolder in Directory.GetDirectories(Path.Combine(rundir, "characters")))
                {
                    subfiles.AddRange(Directory.GetFiles(subfolder));
                }

                foreach (string file in subfiles)
                {
                    FileInfo f = new FileInfo(file);
                    if (f.Extension == ".png" || f.Extension == ".jpg" || f.Extension == ".gif")
                    {
                        string parentDirectory = Path.GetFileName(f.DirectoryName);
                        string categoryName = parentDirectory == "characters" ? "Unknown" : parentDirectory;
                        if (!categories.Contains(categoryName))
                            categories.Add(categoryName);
                        characters.Add(new Character
                        {
                            Name = Path.GetFileNameWithoutExtension(f.Name),
                            Category = categoryName,
                            FilePath = file
                        });
                    }
                }

                string presetCategory = categories.First();

                if (!string.IsNullOrEmpty(Configuration.Instance.SelectedGame) && categories.Contains(Configuration.Instance.SelectedGame))
                    presetCategory = Configuration.Instance.SelectedGame;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    CharacterList = characters;
                    CharacterCategories = categories;
                    SelectedCharacterCategory = presetCategory;
                    CharacterListView = CollectionViewSource.GetDefaultView(CharacterList);
                    CharacterListView.Filter = FilterCharacter;

                });
            }
            catch (Exception) { }
        }


        private void LoadTeams()
        {
            try
            {
                ObservableCollection<Character> teams = new ObservableCollection<Character>();
                string rundir = Utils.RunDirectory();
                string[] subfiles = Directory.GetFiles(Path.Combine(rundir, "teams"));
                foreach (string file in subfiles)
                {
                    FileInfo f = new FileInfo(file);
                    if (f.Extension == ".png" || f.Extension == ".jpg" || f.Extension == ".gif")
                    {
                        teams.Add(new Character
                        {
                            Name = f.Name.Split('.')[0],
                            FilePath = file
                        });
                    }
                }

                Application.Current.Dispatcher.Invoke(() => TeamList = teams);
            }
            catch (Exception) { }
        }

        private void StartPendingSet(Set obj)
        {
            Reset();

            Opponent1.LinkedPlayer = obj.Opponent1;
            Opponent2.LinkedPlayer = obj.Opponent2;

            if (obj.isDouble)
            {
                Opponent3.LinkedPlayer = obj.Opponent3;
                Opponent4.LinkedPlayer = obj.Opponent4;

                SetMode = SetMode.Doubles;
            }
            else
            {
                SetMode = SetMode.Singles;
            }

            Round = obj.RoundName;
        }

        private void DoLiveUpdate(object sender, ElapsedEventArgs e)
        {
            Update();
        }

        private bool FilterCharacter(object item)
        {
            if (item is Character character)
            {
                return character.Category == SelectedCharacterCategory;
            }

            return false;
        }


        private bool _liveUpdate = false;
        public bool LiveUpdate
        {
            get { return _liveUpdate; }
            set { Set("LiveUpdate", ref _liveUpdate, value); if (value) _timer.Start(); else _timer.Stop(); }
        }

        private ObservableCollection<Character> _characterList;
        public ObservableCollection<Character> CharacterList
        {
            get => _characterList;
            set => Set("CharacterList", ref _characterList, value);
        }

        private ICollectionView _characterListView;
        public ICollectionView CharacterListView
        {
            get => _characterListView;
            set => Set("CharacterListView", ref _characterListView, value);
        }

        private ObservableCollection<Character> _teamList;
        public ObservableCollection<Character> TeamList
        {
            get { return _teamList; }
            set { Set("TeamList", ref _teamList, value); }
        }

        private ObservableCollection<Player> _playerList;
        public ObservableCollection<Player> PlayerList
        {
            get => _playerList;
            set => Set("PlayerList", ref _playerList, value);
        }

        private Opponent _opponent1 = null;
        public Opponent Opponent1
        {
            get => _opponent1;
            set => Set("Opponent1", ref _opponent1, value);
        }

        private Opponent _opponent2 = null;
        public Opponent Opponent2
        {
            get => _opponent2;
            set => Set("Opponent2", ref _opponent2, value);
        }

        private SetMode _setMode = SetMode.Singles;
        public SetMode SetMode
        {
            get => _setMode;
            set => Set("SetMode", ref _setMode, value);
        }

        private Opponent _opponent3 = null;
        public Opponent Opponent3
        {
            get => _opponent3;
            set => Set("Opponent3", ref _opponent3, value);
        }

        private Opponent _opponent4 = null;
        public Opponent Opponent4
        {
            get => _opponent4;
            set => Set("Opponent4", ref _opponent4, value);
        }

        private Character _teamA = null;
        public Character TeamA
        {
            get => _teamA;
            set => Set("TeamA", ref _teamA, value);
        }

        private Character _teamB = null;
        public Character TeamB
        {
            get => _teamB;
            set => Set("TeamB", ref _teamB, value);
        }


        private string _round;
        public string Round
        {
            get => _round;
            set => Set("Round", ref _round, value);
        }

        private string _selectedCharacterCategory;
        public string SelectedCharacterCategory
        {
            get => _selectedCharacterCategory;
            set
            {
                Set("SelectedCharacterCategory", ref _selectedCharacterCategory, value);
                Configuration.Instance.SelectedGame = value;
                CharacterListView?.Refresh();
            } 
        }

        private ObservableCollection<string> _characterCategories;
        public ObservableCollection<string> CharacterCategories
        {
            get => _characterCategories;
            set => Set("CharacterCategories", ref _characterCategories, value);
        }

        public RelayCommand ResetCommand { get; set; }
        private void Reset()
        {
            Opponent1 = new Opponent();
            Opponent2 = new Opponent();
            Opponent3 = new Opponent();
            Opponent4 = new Opponent();
            TeamA = new Character();
            TeamB = new Character();
            Round = "";
        }

        private bool _reportEnabled = false;
        public bool ReportEnabled
        {
            get => _reportEnabled;
            set => Set("ReportEnabled", ref _reportEnabled, value);
        }

        private bool _addP1L;
        public bool AddP1L
        {
            get => _addP1L;
            set
            {
                Set("AddP1L", ref _addP1L, value);
                if (value && AddP1W)
                    AddP1W = false;
            }
        }

        private bool _addP1W;
        public bool AddP1W
        {
            get => _addP1W;
            set
            {
                Set("AddP1W", ref _addP1W, value);
                if (value && AddP1L)
                    AddP1L = false;
            }
        }

        private bool _addP2L;
        public bool AddP2L
        {
            get => _addP2L;
            set
            {
                Set("AddP2L", ref _addP2L, value);
                if (value && AddP2W)
                    AddP2W = false;
            }
        }

        private bool _addP2W;
        public bool AddP2W
        {
            get => _addP2W;
            set
            {
                Set("AddP2W", ref _addP2W, value);
                if (value && AddP2L)
                    AddP2L = false;
            }
        }

        public RelayCommand SwapPlayerCommand { get; set; }
        private void SwapPlayer()
        {
            Opponent o = Opponent1;
            Opponent1 = Opponent2;
            Opponent2 = o;
            o = Opponent3;
            Opponent3 = Opponent4;
            Opponent4 = o;
        }

        public RelayCommand IncrementEntrant1Command { get; set; }
        private void IncrementEntrant1()
        {
            ++Opponent1.Score;
        }


        public RelayCommand DecrementEntrant1Command { get; set; }
        private void DecrementEntrant1()
        {
            --Opponent1.Score;
        }


        public RelayCommand IncrementEntrant2Command { get; set; }
        private void IncrementEntrant2()
        {
            ++Opponent2.Score;
        }


        public RelayCommand DecrementEntrant2Command { get; set; }
        private void DecrementEntrant2()
        {
            --Opponent2.Score;
        }

        public RelayCommand UpdateCommand { get; set; }
        private void Update()
        {
            Output.Data.Player1 = new Opponent(Opponent1);
            if (AddP1L)
                Output.Data.Player1.Name += " [L]";
            if (AddP1W)
                Output.Data.Player1.Name += " [W]";
            Output.Data.Player2 = new Opponent(Opponent2);
            if (AddP2L)
                Output.Data.Player2.Name += " [L]";
            if (AddP2W)
                Output.Data.Player2.Name += " [W]";

            if(SetMode == SetMode.Doubles)
            {
                Output.Data.Player3 = new Opponent(Opponent3);
                Output.Data.Player4 = new Opponent(Opponent4);
            }

            Output.Data.TeamA = new Character(TeamA);
            Output.Data.TeamB = new Character(TeamB);

            Output.Data.RoundName = Round;
        }

        public RelayCommand ReportCommand { get; set; }
        private void Report()
        {
            
        }
    }
}
