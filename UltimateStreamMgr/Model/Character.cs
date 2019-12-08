using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Character
    {
        // Copy constructor.
        public Character(Character previous)
        {
            Name = previous.Name;
            Category = previous.Category;
            FilePath = previous.FilePath;
        }

        public Character()
        {

        }

        public string Name { get; set; }
        public string Category { get; set; }
        public string FilePath { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Character character)
            {
                return Equals(character);
            }
            else
                return base.Equals(obj);
        }

        protected bool Equals(Character other)
        {
            return Name == other.Name && Category == other.Category && FilePath == other.FilePath;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Category != null ? Category.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FilePath != null ? FilePath.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
