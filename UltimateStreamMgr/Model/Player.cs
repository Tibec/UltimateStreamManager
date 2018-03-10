using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UltimateStreamMgr.Model
{
    public class Player : ObservableObject
    {
        private int _id = -1;
        public int Id
        {
            get { return _id; }
            set { Set("Id", ref _id, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set("Name", ref _name, value); }
        }

        private Team _team;
        public Team Team
        {
            get { return _team; }
            set { Set("Team", ref _team, value); }
        }

        private Country _country;
        public Country Country
        {
            get { return _country; }
            set { Set("Country", ref _country, value); }
        }

        private string _twitter;
        public string Twitter
        {
            get { return _twitter; }
            set { Set("Twitter", ref _twitter, value); }
        }

        private string _twitch;
        public string Twitch
        {
            get { return _twitch; }
            set { Set("Twitch", ref _twitch, value); }
        }

        private int _smashggId = -1;
        public int SmashggId
        {
            get { return _smashggId; }
            set { Set("SmashggId", ref _smashggId, value); }
        }
    }
}
