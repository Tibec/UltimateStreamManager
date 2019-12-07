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
    }
}
