using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Team
    {
        public int Id { get; set; } = -1;
        public string Name { get; set; }
        public string ShortName { get; set; }


        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Team) obj);
        }

        protected bool Equals(Team other)
        {
            return Id == other.Id && ShortName == other.ShortName;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id * 397) ^ (ShortName != null ? ShortName.GetHashCode() : 0);
            }
        }
    }
}
