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
        public long Id { get; set; }
        public string AuthorHandle { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Message { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((SocialMessage) obj);
        }

        protected bool Equals(SocialMessage other)
        {
            return Author == other.Author && Date.Equals(other.Date) && Message == other.Message;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Author != null ? Author.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ (Message != null ? Message.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
