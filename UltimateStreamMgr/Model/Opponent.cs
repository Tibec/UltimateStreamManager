using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Opponent : ObservableObject
    {
        // Copy constructor.
        public Opponent(Opponent previous)
        {
            Name = previous.Name;
            Character = previous.Character;
            LinkedPlayer = previous.LinkedPlayer;
            Score = previous.Score;
        }

        public Opponent() {
            // Force events to fire
            Character = new Character();
        }

        private string _name = "";
        public string Name
        {
            get { return _name; }
            set { Set("Name", ref _name, value); }
        }

        private Character _character;
        public Character Character
        {
            get { return _character; }
            set { Set("Character", ref _character, value); }
        }

        private Player _linkedPlayer;
        public Player LinkedPlayer
        {
            get { return _linkedPlayer; }
            set { Set("LinkedPlayer", ref _linkedPlayer, value); }
        }

        private int _score = 0;
        public int Score
        {
            get { return _score; }
            set { Set("Score", ref _score, value); }
        }
    }
}
