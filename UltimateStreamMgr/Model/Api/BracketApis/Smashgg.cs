using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UltimateStreamMgr.Model.Api.BracketApis
{
    public class Smashgg : BracketApi
    {
        private SmashGgSettings _settings;
        private int _tournamentId;

        //private List<Participant> _participants 

        public Smashgg(BracketSettings settings) : base(settings)
        {
            ApiName = "Smash GG";
            _baseUrl = "https://api.smash.gg/";

            if (settings is SmashGgSettings)
            {
                _settings = settings as SmashGgSettings;
                RetrieveTournamentId();
            }
        }

        private void RetrieveTournamentId()
        {
            var result = Request("tournament/" + _settings.TournamentName);
            if (string.IsNullOrEmpty(result))
                return;
            dynamic data = JsonConvert.DeserializeObject(result);

            _tournamentId = data.entities.tournament.id;
        }

        private Player GetPlayerFromEntrantId(dynamic playerList, dynamic entrantList, int entrantId)
        {
            List<Player> players = ParsePlayerData(playerList, entrantList);
            int playerId = -1;
            foreach(dynamic entrant in entrantList)
            {
                if(entrant.id == entrantId)
                {
                    playerId = (int)(entrant.playerIds as JArray).First;
                }
            }
            if(playerId == -1)
                throw new KeyNotFoundException("Entrant without player id");

            return players.First((p) => p.SmashggId == playerId);
        }

        private List<Player> GetPlayersFromEntrantId(dynamic playerList, dynamic entrantList, int entrantId)
        {
            List<Player> players = ParsePlayerData(playerList, entrantList);
            List<Player> r = new List<Player>();

            foreach (dynamic entrant in entrantList)
            {
                if (entrant.id == entrantId)
                {
                    foreach (var token in (entrant.playerIds as JArray).Children())
                    {
                        int pId = (int)token;
                        r.Add(players.First((p) => p.SmashggId == pId));
                    }
                }
            }

            if (r.Count == 0)
                throw new KeyNotFoundException("Entrant without player id");
            else
                return r;
        }

        private bool IsTeamEntrant(dynamic entrantList, int entrantId)
        {
            foreach(dynamic entrant in entrantList)
            {
                if(entrant.id == entrantId)
                {
                    return (entrant.playerIds as JArray).Children().Count() > 1;
                }
            }
            return true;
        }

        public override List<Bracket> GetAvailablesBrackets(bool includeSets = false, bool includeParticipants = false)
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

        public override List<Set> GetAllPendingSets(bool includeNonStream = false)
        {
            List<Set> sets = new List<Set>();
            try
            {
                if (!includeNonStream)
                {
                    var result = Request("station_queue/" + _tournamentId);
                    if (string.IsNullOrEmpty(result))
                        return sets;
                    dynamic data = JsonConvert.DeserializeObject(result);
                    var setlist = data.data.entities.sets;
                    if(setlist == null)
                    { }
                    else if (setlist.Type == JTokenType.Array)
                    {
                        foreach (dynamic set in setlist)
                        {
                            Set s = ParseSetData(set, data.data.entities.player, data.data.entities.entrants, data.data.entities.groups);
                            sets.Add(s);
                        }
                    }
                    else // if (setlist.Type == JTokenType.Object)
                    {
                        Set s = ParseSetData(setlist, data.data.entities.player, data.data.entities.entrants, data.data.entities.groups);
                        sets.Add(s);
                    }
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch(Exception e)
            { // No pending set  
            }
            return sets;
        }

        private Set ParseSetData(dynamic set, dynamic playerList, dynamic entrantList, dynamic groupData)
        {
            Set s = new Set();
            if(set.id as int? != null)
                s.Id = set.id;
            s.BracketId = set.phaseGroupId;
            int? e1 = set.entrant1Id, e2 = set.entrant2Id;
            if (e1 != null && IsTeamEntrant(entrantList, (int)e1) || e2 != null && IsTeamEntrant(entrantList, (int)e2))
            {
                if (e2 != null)
                {
                    List<Player> team2 = GetPlayersFromEntrantId(playerList, entrantList, (int)e2);
                    s.isDouble = true;
                    s.Opponent2 = team2[0];
                    s.Opponent4 = team2[1];
                }
                if (e1 != null)
                {
                    List<Player> team1 = GetPlayersFromEntrantId(playerList, entrantList, (int)e1);
                    s.isDouble = true;
                    s.Opponent1 = team1[0];
                    s.Opponent3 = team1[1];
                }
            }
            else
            {
                if (e1 != null)
                    s.Opponent1 = GetPlayerFromEntrantId(playerList, entrantList, (int)e1);
                if (e2 != null)
                    s.Opponent2 = GetPlayerFromEntrantId(playerList, entrantList, (int)e2);
            }
            if(groupData is JObject)
            {
                if (groupData.groupTypeId == 3) // RR
                {
                    s.RoundName = "Pool " + groupData.displayIdentifier;
                }
                else // DE or SE
                {
                    s.RoundName = set.fullRoundText;
                }
            }
            else if(groupData is JArray)
            {
                foreach (dynamic group in groupData)
                {
                    if (group.id == set.phaseGroupId)
                    {
                        if (group.groupTypeId == 3) // RR
                        {
                            s.RoundName = "Pool " + group.displayIdentifier;
                        }
                        else // DE or SE
                        {
                            s.RoundName = set.fullRoundText;
                        }
                    }
                }
            }
            switch ((int)set.state)
            {
                case 1:
                    s.State = SetState.Open;
                    break;
                case 2:
                    s.State = SetState.InProgress;
                    break;
                case 3:
                    s.State = SetState.Completed;
                    break;
            }

            if(s.State == SetState.Completed)
            {
                s.Opponent1Score = set.entrant1Score;
                s.Opponent2Score = set.entrant2Score;
            }
            return s;
        }

        public override Set GetSet(int id)
        {
            throw new NotImplementedException();
        }

        private List<Player> ParsePlayerData(dynamic playerList, dynamic entrantList, bool useDatabaseAsReference = true)
        {
            List<Player> players = new List<Player>();
            foreach (dynamic player in playerList)
            {
                Player p = null;

                if(useDatabaseAsReference)
                    p = PlayerDatabase.GetBySmashggId((int)player.id);

                if (p == null)
                {
                    p = new Player();
                    p.SmashggId = player.id;
                    p.Name = player.gamerTag;

                    string playerTeamTag = RetrievePlayerTeam((int)player.id, entrantList);
                    if (!string.IsNullOrEmpty(playerTeamTag))
                    {

                        Team existingTeam = PlayerDatabase.GetTeams().Find( t => t.ShortName == playerTeamTag);
                        if (existingTeam == null)
                            p.Team = new Team { ShortName = playerTeamTag };
                        else
                            p.Team = existingTeam;
                    }

                    p.Twitter = player.twitterHandle;
                    p.Twitch = player.twitchStream;

                    if (!string.IsNullOrEmpty((string)player.country))
                    {
                        p.Country = PlayerDatabase.GetCountryByName((string)player.country);
                    }
                }
                players.Add(p);
            }
            return players;
        }

        private string RetrievePlayerTeam(int playerId, dynamic entrantList)
        {
            foreach(dynamic entrant in entrantList)
            {
                // ignore every 'team' entrant
                if ((entrant.playerIds as JArray).Children().Count() > 1)
                    continue;
                int entrantPlayerId = (int)(entrant.playerIds as JArray).First;
                
                if(entrantPlayerId == playerId)
                {
                    return (string)(entrant.prefixes as JArray).First;
                }
            }
            return "";
        }

        public override List<Player> GetAllEntrants()
        {
            string result = Request("tournament/" + _settings.TournamentName + "?expand[]=entrants");
            if (string.IsNullOrEmpty(result))
                return new List<Player>();
            dynamic data = JsonConvert.DeserializeObject(result);
            return ParsePlayerData(data.entities.player, data.entities.entrants, false);
        }

        public override List<Top8> GetAvailablesTop8()
        {
            List<Top8> top8list = new List<Top8>();
            string result = Request("tournament/" + _settings.TournamentName + "?expand[]=event&expand[]=phase&expand[]=phase&expand[]=groups");

            if (string.IsNullOrEmpty(result))
                return top8list;

            dynamic data = JsonConvert.DeserializeObject(result);

            foreach (dynamic evt in (data.entities as JObject).GetValue("event"))
            {
                Top8 top8 = new Top8();
                top8.Name = evt.name;
                int finalPhaseId = 0, finalGroupId = 0, phaseCountForThisEvent = 0;
                foreach(var phase in data.entities.phase)
                {
                    if (phase.eventId == evt.id)
                        ++phaseCountForThisEvent;
                }
                foreach (var phase in data.entities.phase)
                {
                    if (phase.phaseOrder == phaseCountForThisEvent && phase.eventId == evt.id)
                        finalPhaseId = phase.id;
                }
                foreach(var group in data.entities.groups)
                {
                    if (group.phaseId == finalPhaseId)
                        if (group.groupTypeId == 2)
                            finalGroupId = group.id;
                }

                if (finalPhaseId == 0 || finalGroupId == 0)
                    continue;

                string result2 = Request("phase_group/" + finalGroupId + "?expand[]=sets&expand[]=entrants");

                if (string.IsNullOrEmpty(result))
                    continue;

                dynamic setdata = JsonConvert.DeserializeObject(result2);
                if (setdata.entities.sets==null)
                    continue;
                foreach (var set in setdata.entities.sets)
                {
                    if (set.wPlacement == 1 && set.lPlacement == 2 
                        && set.entrant2PrereqCondition == "winner") // GF1
                    {
                        top8.GrandFinal = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 3 && set.lPlacement == 5) // Winner semi
                    {
                        if (top8.WinnerSemi1 == null)
                            top8.WinnerSemi1 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                        else
                            top8.WinnerSemi2 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 2 && set.lPlacement == 3 && set.round > 0) // WF
                    {
                        top8.WinnerFinal = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 5 && set.lPlacement == 7) // Loser Top 8
                    {
                        if (top8.Loser7th1 == null)
                            top8.Loser7th1 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                        else
                            top8.Loser7th2 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 4 && set.lPlacement == 5) // Loser Quarter
                    {
                        if (top8.LoserQuarter1 == null)
                            top8.LoserQuarter1 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                        else
                            top8.LoserQuarter2 = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 3 && set.lPlacement == 4) // Loser Semi
                    {
                        top8.LoserSemi = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                    else if (set.wPlacement == 2 && set.lPlacement == 3 && set.round < 0) // Loser Final
                    {
                        top8.LoserFinal = ParseSetData(set, setdata.entities.player, setdata.entities.entrants, setdata.entities.groups);
                    }
                }
                top8list.Add(top8);
            }

            return top8list;
        }
    }

    public class SmashGgSettings : BracketSettings
    {
        public SmashGgSettings()
        {
            Api = typeof(Smashgg);
        }

        public string TournamentName { get; set; }

        public override string ToString()
        {
            return "Smash.gg ( Tournoi : " + (string.IsNullOrEmpty(TournamentName) ? "Aucun" : TournamentName) + " )";
        }
    }
}
