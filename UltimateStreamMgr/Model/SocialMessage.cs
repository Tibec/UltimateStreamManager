using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model
{
    public class SocialMessage
    {
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SocialMessage)
            {
                SocialMessage msg = obj as SocialMessage;
                return Author == msg.Author && Message == msg.Message && Date == msg.Date;
            }
            else
                return false; 
        }
    }
}
