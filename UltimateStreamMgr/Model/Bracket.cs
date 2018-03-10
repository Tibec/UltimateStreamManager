using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Bracket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Game Game { get; set; }

        public List<Player> Entrants { get; set; }
        public List<Set> Sets { get; set; }
    }
}
