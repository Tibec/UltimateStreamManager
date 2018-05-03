using GalaSoft.MvvmLight;
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

namespace UltimateStreamMgr.ViewModel
{
    public enum SetMode { Singles, Doubles, Crewbattles };

    public class RunningSetViewModel : DockWindowViewModel
    {
        private Timer _timer;

        public RunningSetViewModel()
        {
            Title = "Running Set";
            Reset();

            _timer = new Timer(500);
            _timer.Elapsed += DoLiveUpdate;

            ResetCommand = new RelayCommand(() => Reset());
            ReportCommand = new RelayCommand(() => Report());
            UpdateCommand = new RelayCommand(() => Update());
            SwapPlayerCommand = new RelayCommand(() => SwapPlayer());

            MessengerInstance.Register<Set>(this, StartPendingSet);

            PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers());
            PlayerDatabase.DatabaseContentModified += RefreshPlayers;

            LoadCharacters();
        }

        private void RefreshPlayers()
        {
            Dispatcher.CurrentDispatcher.Invoke(() => PlayerList = new ObservableCollection<Player>(PlayerDatabase.GetAllPlayers()));
        }

        private async void LoadCharacters()
        {
            try
            {

                ObservableCollection<Character> characters = new ObservableCollection<Character>();
                string rundir = Utils.RunDirectory();
                string[] subfiles = Directory.GetFiles(Path.Combine(rundir, "characters"));
                foreach (string file in subfiles)
                {
                    FileInfo f = new FileInfo(file);
                    if (f.Extension == ".png" || f.Extension == ".jpg" || f.Extension == ".gif")
                    {
                        characters.Add(new Character
                        {
                            Name = f.Name.Split('.')[0],
                            FilePath = file
                        });
                    }
                }

                Application.Current.Dispatcher.Invoke(() => CharacterList = characters);
            }
            catch (Exception e) { }
        }

        private void StartPendingSet(Set obj)
        {
            Reset();

            Opponent1.LinkedPlayer = obj.Opponent1;
            Opponent1.Name = obj.Opponent1?.Name;

            Opponent2.LinkedPlayer = obj.Opponent2;
            Opponent2.Name = obj.Opponent2?.Name;
            if(obj.isDouble)
            {
                Opponent3.LinkedPlayer = obj.Opponent3;
                Opponent3.Name = obj.Opponent3?.Name;

                Opponent4.LinkedPlayer = obj.Opponent4;
                Opponent4.Name = obj.Opponent4?.Name;

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

        private bool _liveUpdate = false;
        public bool LiveUpdate
        {
            get { return _liveUpdate; }
            set { Set("LiveUpdate", ref _liveUpdate, value); if (value) _timer.Start(); else _timer.Stop(); }
        }

        private ObservableCollection<Character> _characterList;
        public ObservableCollection<Character> CharacterList
        {
            get { return _characterList; }
            set { Set("CharacterList", ref _characterList, value); }
        }

        private ObservableCollection<Player> _playerList;
        public ObservableCollection<Player> PlayerList
        {
            get { return _playerList; }
            set { Set("PlayerList", ref _playerList, value); }
        }

        private Opponent _opponent1 = null;
        public Opponent Opponent1
        {
            get { return _opponent1; }
            set { Set("Opponent1", ref _opponent1, value); }
        }

        private Opponent _opponent2 = null;
        public Opponent Opponent2
        {
            get { return _opponent2; }
            set { Set("Opponent2", ref _opponent2, value); }
        }

        private SetMode _setMode = SetMode.Singles;
        public SetMode SetMode
        {
            get { return _setMode; }
            set { Set("SetMode", ref _setMode, value); }
        }

        private Opponent _opponent3 = null;
        public Opponent Opponent3
        {
            get { return _opponent3; }
            set { Set("Opponent3", ref _opponent3, value); }
        }

        private Opponent _opponent4 = null;
        public Opponent Opponent4
        {
            get { return _opponent4; }
            set { Set("Opponent4", ref _opponent4, value); }
        }

        private string _round;
        public string Round
        {
            get { return _round; }
            set { Set("Round", ref _round, value); }
        }

        public RelayCommand ResetCommand { get; set; }
        private void Reset()
        {
            Opponent1 = new Opponent();
            Opponent2 = new Opponent();
            Opponent3 = new Opponent();
            Opponent4 = new Opponent();
            Round = "";
        }

        private bool _reportEnabled = false;
        public bool ReportEnabled
        {
            get { return _reportEnabled; }
            set { Set("ReportEnabled", ref _reportEnabled, value); }
        }

        private bool _addP1L;
        public bool AddP1L
        {
            get { return _addP1L; }
            set { Set("AddP1L", ref _addP1L, value); if (value && AddP1W) AddP1W = false; }
        }

        private bool _addP1W;
        public bool AddP1W
        {
            get { return _addP1W; }
            set { Set("AddP1W", ref _addP1W, value); if (value && AddP1L) AddP1L = false; }
        }

        private bool _addP2L;
        public bool AddP2L
        {
            get { return _addP2L; }
            set { Set("AddP2L", ref _addP2L, value); if (value && AddP2W) AddP2W = false; }
        }

        private bool _addP2W;
        public bool AddP2W
        {
            get { return _addP2W; }
            set { Set("AddP2W", ref _addP2W, value); if (value && AddP2L) AddP2L = false; }
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

            Output.Data.RoundName = Round;
        }

        public RelayCommand ReportCommand { get; set; }
        private void Report()
        {
            
        }
    }
}
