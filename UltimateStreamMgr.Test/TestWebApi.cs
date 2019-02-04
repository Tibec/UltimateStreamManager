using System;
using UltimateStreamMgr.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimateStreamMgr.Model.Api;
using System.Net.Http;
using UltimateStreamMgr.Model.Api.StreamApis;
using UltimateStreamMgr.Model.Api.BracketApis;
using UltimateStreamMgr.Model.Api.SocialApis;

namespace UltimateStreamMgr.Test
{
    [TestClass]
    public class TestWebApi
    {
        [TestMethod]
        public void TestRequest()
        {

        }

        [TestMethod]
        public void TestTwitchChannelExist()
        {
            Twitch api = new Twitch(new TwitchSettings());
            var a = api.GetSupportedActions();
            var info = api.ChannelExist("smashgrenoble");
        }

        [TestMethod]
        public void TestTwitchChannelInfo()
        {
            Twitch api = new Twitch(new TwitchSettings { ChannelName = "smashgrenoble" });
            var a = api.GetChannelInfo();
        }

        [TestMethod]
        public void TestTwitchUpdateChannelInfo()
        {
            Twitch api = new Twitch(new TwitchSettings { ChannelName = "smashgrenoble", OAuthToken = "008iczk72jrmqyt1piaw5591u5scj6" });
            api.UpdateChannelInfo("GRE #3 - Tests ... (" + new Random().Next() + ")");
        }

        [TestMethod]
        public void TestSmashGgPendingSet()
        {
            Smashgg api = new Smashgg(new SmashGgSettings { TournamentName = "gre-3" });
            var a = api.GetAllPendingSets();
        }

        [TestMethod]
        public void TestSmashGgEntrantList()
        {
            Smashgg api = new Smashgg(new SmashGgSettings { TournamentName = "gre-3" });
            var a = api.GetAllEntrants();
        }

        [TestMethod]
        public void TestSmashGgTop8()
        {
            Smashgg api = new Smashgg(new SmashGgSettings { TournamentName = "gre-3" });
            var a = api.GetAvailablesTop8();
        }
        [TestMethod]
        public void TestTwitter()
        {
            Twitter api = new Twitter(null);
           // api.PublishMessage("Ceci est un message con posté depuis une application génial ! Et il contient un lien !!!! https://smash.gg/tournament/gre-1/events");
            var a = api.GetMessagesByHashtag("hashtagdecon");
        }
    }
}
