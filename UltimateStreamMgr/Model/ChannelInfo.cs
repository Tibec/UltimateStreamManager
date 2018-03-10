using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class ChannelInfo
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public int Viewers { get; set; }
        public ChannelStatus Status { get; set; }
        public Game Game { get; set; }
    }

    public enum ChannelStatus
    {
        Online, 
        Offline
    }
}
