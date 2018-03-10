using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Country
    {
        public Country()
        { }
        public Country(RegionInfo regioninfo)
        {
            ShortName = regioninfo.Name;
            FullName = regioninfo.EnglishName;
        }
        public string ShortName { get; set; }
        public string FullName { get; set; }
    }
}
