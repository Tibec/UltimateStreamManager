using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class CustomKey : ObservableObject
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { Set("Name", ref _name, value); }
        }
        private string _value;
        public string Value
        {
            get { return _value; }
            set { Set("Value", ref _value, value); }
        }
    }
}
