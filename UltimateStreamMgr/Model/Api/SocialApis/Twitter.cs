using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UltimateStreamMgr.Helpers;

namespace UltimateStreamMgr.Model.Api.SocialApis
{
    public class Twitter : SocialApi
    {

        private string _consumerKey = "";
        private string _consumerSecret = "";

        private string _accessTokenKey = "";
        private string _accessTokenSecret = "";

        TinyTwitter apiWrapper;

        public Twitter(SocialSettings settings) : base(settings)
        {
            ApiName = "Twitter";

            apiWrapper = new TinyTwitter(
                new OAuthInfo
                {
                    AccessSecret = _accessTokenSecret,
                    AccessToken = _accessTokenKey,
                    ConsumerKey = _consumerKey,
                    ConsumerSecret = _consumerSecret
                }
            );
        }

        public override List<SocialMessage> GetMessagesByHashtag(string hashtag, int limit = -1)
        {
            IEnumerable<Tweet> tweets = apiWrapper.GetTweetsByHashtag(hashtag);
            List<SocialMessage> msgs = new List<SocialMessage>();

            foreach (var tweet in tweets)
            {
                SocialMessage msg = new SocialMessage();
                msg.Date = tweet.CreatedAt;
                msg.Author = tweet.UserName;
                msg.Message = tweet.Text;
                msgs.Add(msg);
            }

            return msgs;
        }


        public override List<SocialMessage> GetMessagesByAuthor(string authorName, int limit = -1)
        {
            IEnumerable<Tweet> tweets =  apiWrapper.GetUserTimeline(null, null, null, authorName);
            List<SocialMessage> msgs = new List<SocialMessage>();

            foreach (var tweet in tweets)
            {
                SocialMessage msg = new SocialMessage();
                msg.Date = tweet.CreatedAt;
                msg.Author = tweet.UserName ;
                msg.Message = tweet.Text;
                msgs.Add(msg);
            }

            return msgs;
        }

        public static string GetAuthentificationURL()
        {
            return "";
        }


        public override void PublishMessage(string message)
        {
            apiWrapper.UpdateStatus(message);
        }
    }

    public class TwitterSettings : SocialSettings
    {
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public string AccessToken { get; set;}
        public string AccessTokenSecret { get; set; }
    }
}
