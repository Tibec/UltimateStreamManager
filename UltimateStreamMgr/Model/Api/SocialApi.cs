using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UltimateStreamMgr.Model.Api
{
    abstract public class SocialApi : WebApi
    {
        public SocialApi(SocialSettings settings)
        {

        }

        public abstract List<SocialMessage> GetMessagesByHashtag(string hashtag, int limit = -1);
        public abstract List<SocialMessage> GetMessagesByAuthor(string authorName, int limit = -1);
        public abstract void PublishMessage(string message);

    }


    public class SocialSettings
    {
        [XmlIgnore]
        public Type Api { get; set; }

        public override string ToString()
        {
            return "Aucun";
        }
    }

}
