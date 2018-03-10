using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class Set
    {
        public int Id { get; set; }
        public int BracketId { get; set; }
        public bool isDouble { get; set; }
        public Player Opponent1 { get; set; }
        public Player Opponent2 { get; set; }
        public Player Opponent3 { get; set; }
        public Player Opponent4 { get; set; }
        public int Opponent1Score { get; set; }
        public int Opponent2Score { get; set; }
        public SetState State { get; set; }
        public string RoundName { get; set; }
    }

    public enum SetState
    {
        Open, 
        InProgress,
        Completed,
    }
}
