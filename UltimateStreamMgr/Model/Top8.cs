using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Top8
    {
        public string Name { get; set; }

        public Set WinnerSemi1 { get; set; }
        public Set WinnerSemi2 { get; set; }

        public Set WinnerFinal { get; set; }

        public Set GrandFinal { get; set; }

        public Set Loser7th1 { get; set; }
        public Set Loser7th2 { get; set; }

        public Set LoserQuarter1 { get; set; }
        public Set LoserQuarter2 { get; set; }

        public Set LoserSemi { get; set; }

        public Set LoserFinal { get; set; }

    }
}
