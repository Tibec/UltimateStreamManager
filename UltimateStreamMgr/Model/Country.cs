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
            ShortName = regioninfo.TwoLetterISORegionName;
            FullName = regioninfo.EnglishName;
        }
        public string ShortName { get; }
        public string FullName { get; }

        public override bool Equals(object obj)
        {
            if (obj is Country c)
            {
                return Equals(c);
            }

            return false;
        }

        protected bool Equals(Country other)
        {
            return ShortName == other.ShortName && FullName == other.FullName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ShortName != null ? ShortName.GetHashCode() : 0) * 397) ^ (FullName != null ? FullName.GetHashCode() : 0);
            }
        }
    }
}
