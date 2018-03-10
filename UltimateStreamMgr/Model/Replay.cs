using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Replay : ObservableObject
    {
        private bool _play;
        public bool Play
        {
            get { return _play; }
            set { Set("Play", ref _play, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set("Name", ref _name, value); }
        }

        private string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set { Set("FilePath", ref _filePath, value); }
        }
    }
}
