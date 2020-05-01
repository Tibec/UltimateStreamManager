using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model.Api.BracketApis
{
    /*
    public class Challonge : BracketApi
    {
        private ChallongeSettings _settings;

        public Challonge(BracketSettings settings) : base(settings)
        {
            ApiName = "Challonge";
            _baseUrl = "https://api.challonge.com/v1/";

            if (settings is ChallongeSettings)
            {
                _settings = (ChallongeSettings) settings;
            }
        }

        public override List<Player> GetAllEntrants()
        {
            throw new NotImplementedException();
        }

        public override List<Set> GetAllPendingSets(bool includeNonStream = false)
        {
            throw new NotImplementedException();
        }

        public override List<Bracket> GetAvailablesBrackets(bool includeSets = false, bool includeParticipants = false)
        {
            throw new NotImplementedException();
        }

        public override List<Top8> GetAvailablesTop8()
        {
            throw new NotImplementedException();
        }

        public override List<Player> GetEntrants(int bracketId)
        {
            throw new NotImplementedException();
        }

        public override List<Set> GetPendingSets(int bracketId, bool includeNonStream = false)
        {
            throw new NotImplementedException();
        }

        public override Set GetSet(int id)
        {
            throw new NotImplementedException();
        }
    }

    public class ChallongeSettings : BracketSettings
    {
        public ChallongeSettings()
        {
            Api = typeof(Challonge);
        }

        public string TournamentName { get; set; }
        public string Username { get; set; }
        public string ApiKey { get; set; }

        public override string ToString()
        {
            return $"Challonge ( Tournoi : {TournamentName}, Username: {Username}, ApiKey : {(string.IsNullOrEmpty(ApiKey) ?  "" : "********")} )";
        }
    }
    */
}
