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
            if (obj is Team)
            {
                Team team = obj as Team;
                return Id == team.Id && ShortName == team.ShortName;
            }
            else
                return false;
        }

    }
}
