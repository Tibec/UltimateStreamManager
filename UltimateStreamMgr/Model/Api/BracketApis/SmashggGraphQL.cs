using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Newtonsoft.Json;
using UltimateStreamMgr.Model.Api.BracketApis.SmashggModel;

namespace UltimateStreamMgr.Model.Api.BracketApis
{
    public class SmashggGraphQL : BracketApi
    {
        private SmashGgGQLSettings _settings;
        private int _tournamentId;

        public SmashggGraphQL(BracketSettings settings) : base(settings)
        {
            ApiName = "smash.gg (GraphQL)";
            _baseUrl = "https://api.smash.gg/gql/alpha";

            if (settings is SmashGgGQLSettings s)
            {
                _settings = s;
            }
            else
            {
                _settings = new SmashGgGQLSettings();
            }

            _headers.Add("Authorization", "Bearer " + _settings.ApiKey);
        }

        public override List<Player> GetAllEntrants()
        {
            List<Player> players = new List<Player>();
            bool allPlayersRetrieved = false;
            int queryPage = 1;

            do
            {
                var query = @"
                { ""query"" : ""query 
                  {
                      tournament(slug: \""" + _settings.TournamentName + @"\"") {
                        participants(query: { perPage:100, page: "+queryPage+@" }) {
                          nodes {
                             ...PlayerInfo
                          }
                        }
                      }
                  }" + playerInfoFragment + "\" } ";
                
                string json = Request("", HttpMethod.Post, query.Replace("\r\n", "").Replace("\t", ""));
                RequestResult data = JsonConvert.DeserializeObject<RequestResult>(json);
                if (data.Error == null)
                {
                    if (data.Data.Tournament.Participants.List.Count > 0)
                    {
                        foreach (var participant in data.Data.Tournament.Participants.List)
                        {
                            players.Add(ParticipantToPlayer(participant, false));
                        }
                    }
                    else
                    {
                        allPlayersRetrieved = true;
                    }

                }
                else
                {
                    Log.Error("SmashGG error: " + data.Error.Message);
                }
                ++queryPage;

            } while (!allPlayersRetrieved);

            return players;
        }

        public override List<Set> GetPendingSets(int bracketId, bool includeNonStream = false)
        {
            throw new NotImplementedException();
        }

        public override List<Bracket> GetAvailablesBrackets(bool includeSets = false, bool includeParticipants = false)
        {
            throw new NotImplementedException();
        }

        public override List<Top8> GetAvailablesTop8()
        {
            /*
             * WIP : For now just call the legacy API
             *
             
            var query = @"{ ""query"" : ""query {
                  tournament(slug: \""" + _settings.TournamentName + @"\"") {
                    events {
                      name
                      standings (query: {page :1, perPage:8}) {
                        nodes {
                          standing
                          entrant {
                            participants {
                              ...PlayerInfo
                            }
                          }
                        }
                      }
                    }
                  }
                }" + playerInfoFragment + "\" } ";

            string json = Request("", HttpMethod.Post, query.Replace("\r\n", ""));
            RequestResult data = JsonConvert.DeserializeObject<RequestResult>(json);

            List<Player> players = new List<Player>();
            foreach (var participant in data.Data.Events)
            {
                players.Add(ParticipantToPlayer(participant));
            }
            return players;*/

            return new Smashgg(new SmashGgSettings {TournamentName = _settings.TournamentName}).GetAvailablesTop8();
        }

        public override List<Player> GetEntrants(int bracketId)
        {
            throw new NotImplementedException();
        }

        public override List<Set> GetAllPendingSets(bool includeNonStream = false)
        {
            var query = @"{ ""query"" : ""query 
               {
                  tournament(slug: \""" + _settings.TournamentName + @"\"") {
		            streamQueue {
                      sets {
                         fullRoundText,
                         slots(includeByes:true) {
                           entrant {
                             participants {
                               gamerTag
                             }
                           }
                         }
                       }
                     }
                   }
                }" + /* playerInfoFragment + */ "\" } ";

            string json = Request("", HttpMethod.Post, query.Replace("\r\n", "").Replace("\t",""));
            RequestResult data = JsonConvert.DeserializeObject<RequestResult>(json);

            List<Set> sets = new List<Set>();
            if (data.Data?.Tournament?.StreamQueue == null)
                return sets;
            foreach (var stream in data.Data.Tournament.StreamQueue)
            {
                foreach (var set in stream.Sets)
                {
                    Set pendingSet = new Set();

                    pendingSet.RoundName = set.RoundName;
                    if (set.Players == null)
                        continue;

                    pendingSet.isDouble = set.Players?[0].Entrant.Participant.Count == 2;

                    pendingSet.Opponent1 = ParticipantToPlayer(set.Players[0].Entrant.Participant[0]);
                    pendingSet.Opponent2 = ParticipantToPlayer(set.Players[1].Entrant.Participant[0]);

                    if (pendingSet.isDouble)
                    {
                        pendingSet.Opponent3 = ParticipantToPlayer(set.Players[0].Entrant.Participant[1]);
                        pendingSet.Opponent4 = ParticipantToPlayer(set.Players[1].Entrant.Participant[1]);
                    }

                    sets.Add(pendingSet);
                }
            }
            return sets;
        }

        private Player ParticipantToPlayer(Participant participant, bool useDatabaseAsReference = true)
        {
            Player p = null;

            if (useDatabaseAsReference && participant.User != null)
                p = PlayerDatabase.GetBySmashggId(participant.User.Id);

            if (p == null)
            {
                p = new Player();

                // Name
                p.Name = participant.GamerTag;

                // Smash.gg link
                if (participant.User != null)
                    p.SmashggId = participant.User.Id;

                // Prefix / Team
                if (!string.IsNullOrEmpty(participant.Prefix))
                {
                    Team existingTeam = PlayerDatabase.GetTeams().Find(t => t.ShortName == participant.Prefix);
                    if (existingTeam == null)
                        p.Team = new Team {ShortName = participant.Prefix};
                    else
                        p.Team = existingTeam;
                }

                p.Twitter = participant.User?.ExternalAccounts?.Find(a => a.Type == "TWITTER")?.Username;
                p.Twitch = participant.User?.ExternalAccounts?.Find(a => a.Type == "TWITCH")?.Username;

                if (!string.IsNullOrEmpty(participant.User?.Location?.Country))
                {
                    p.Country = PlayerDatabase.GetCountryByName(participant.User.Location.Country);
                }
            }

            return p;
        }

        public override Set GetSet(int id)
        {
            throw new NotImplementedException();
        }

        private const string playerInfoFragment = @"
            fragment PlayerInfo on Participant {
              id,
              prefix,
              gamerTag,
              user {
                id,
                authorizations {
                  type,
                  externalUsername
                }
              }
            }";
    }

    public class SmashGgGQLSettings : BracketSettings
    {
        public SmashGgGQLSettings()
        {
            Api = typeof(SmashggGraphQL);
        }

        public string ApiKey { get; set; }
        public string TournamentName { get; set; }

        public override string ToString()
        {
            return "Smash.gg GQL ( Tournoi : " + (string.IsNullOrEmpty(TournamentName) ? "Aucun" : TournamentName) + " )";
        }
    }

}
