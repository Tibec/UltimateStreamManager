using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Caster : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set("Name", ref _name, value); }
        }

        private Player _linkedPlayer;
        public Player LinkedPlayer
        {
            get { return _linkedPlayer; }
            set { Set("LinkedPlayer", ref _linkedPlayer, value); }
        }


    }
}
