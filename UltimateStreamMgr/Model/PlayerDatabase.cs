using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Globalization;
using System.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using NLog.Fluent;

namespace UltimateStreamMgr.Model
{
    static class PlayerDatabase
    {
        static SQLiteConnection _conn;

        static private ObservableCollection<Player> _players;
        static private ObservableCollection<Team> _teams;
        static private List<Country> _countries;

        public static void Init(string path = "./Players.db")
        {
            bool schemaMustBeCreated = false;
            if (!File.Exists(path))
            {
                SQLiteConnection.CreateFile("Players.db");
                schemaMustBeCreated = true;
            }

            _conn = new SQLiteConnection("Data Source="+path+";Version=3;");
            _conn.Open();

            // Enable foreign keys
            SQLiteCommand cmd = new SQLiteCommand("PRAGMA foreign_keys = ON", _conn);
            cmd.ExecuteNonQuery();

            if(!VerifySchema())
            {
                schemaMustBeCreated = true;
            }

            if (schemaMustBeCreated)
                CreateSchema();

            LoadCountries();
            LoadTeams();
            LoadPlayers();
            _players.CollectionChanged += UpdatePlayerDatabase;
            _teams.CollectionChanged += UpdateTeamDatabase;
        }

        private static void LoadCountries()
        {
            _countries = new List<Country>();
            var countrylist = (CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                            .Select((c) => new RegionInfo(c.LCID)).ToList());
            foreach (var country in countrylist)
                _countries.Add(new Country(country));
        }

        private static void UpdateTeamDatabase(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Log.Info("Ajout détecté");
                    foreach (Team t in e.NewItems)
                    {
                        AddTeamToDatabase(t);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Log.Info("Suppression détecté");
                    foreach (Team t in e.OldItems)
                    {
                        RemoveTeamFromDatabase(t);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Log.Info("Suppression massive détecté");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Log.Info("Modification détecté");
                    break;
            }
            RaiseDatabaseContentModified();
        }

        private static void UpdatePlayerDatabase(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    Log.Info("Ajout détecté");
                    foreach(Player p in e.NewItems)
                    {
                        AddPlayerToDatabase(p);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Log.Info("Suppression détecté");
                    foreach(Player p in e.OldItems)
                    {
                        RemovePlayerFromDatabase(p);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Log.Info("Suppression massive détecté");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Log.Info("Modification détecté");
                    foreach(Player p in e.NewItems)
                    {
                        UpdatePlayerToDatabase(p);
                    }
                    break;
            }
            RaiseDatabaseContentModified();
        }

        #region SQL Operations

        private static void LoadPlayers()
        {
            _players = new ObservableCollection<Player>();
            string query = "SELECT * FROM vPlayers";
            SQLiteCommand command = new SQLiteCommand(query, _conn);
            SQLiteDataReader result = command.ExecuteReader();
            while (result.Read())
                _players.Add(CreatePlayerFromResult(result));

        }

        private static void LoadTeams()
        {
            _teams = new ObservableCollection<Team>();
            string query = "SELECT * FROM teams";
            SQLiteCommand command = new SQLiteCommand(query, _conn);
            SQLiteDataReader result = command.ExecuteReader();
            while (result.Read())
                _teams.Add(CreateTeamFromResult(result));
        }

        private static bool VerifySchema()
        {
            return true;
        }

        private static void CreateSchema()
        {
            List<string> queries = new List<string>();
            queries.Add("DROP TABLE IF EXISTS `teams` ; CREATE TABLE `teams` ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `name` TEXT, `shortname` TEXT NOT NULL)");
            queries.Add("DROP TABLE IF EXISTS `players` ; CREATE TABLE `players` ( `id` INTEGER NOT NULL UNIQUE, `name` TEXT NOT NULL, `twitter` TEXT, `twitch` TEXT, `team` INTEGER, `country` TEXT, `smashgg_id` INTEGER, PRIMARY KEY(`id`), FOREIGN KEY(`team`) REFERENCES teams(id) ON DELETE SET NULL )");
            queries.Add("DROP TABLE IF EXISTS `db_version` ; CREATE TABLE `db_version` ( `version` INTEGER DEFAULT 100 )");
            queries.Add("DROP VIEW IF EXISTS vPlayers ; CREATE VIEW vPlayers (playerId, playerName, playerCountry, playerTwitter, playerTwitch,playerSmashggId, teamId, teamName, teamShortName) as SELECT players.id as playerId, players.name as playerName, players.country as playerCountry, players.twitter as playerTwitter, players.twitch as playerTwitch, players.smashgg_id as playerSmashggId, players.team as teamId, teams.name as teamName, teams.shortname as teamShortName FROM players LEFT JOIN teams ON teams.id = players.team");
            SQLiteCommand query = new SQLiteCommand(_conn);
            foreach (var q in queries)
            {
                query.CommandText = q;
                query.ExecuteNonQuery();
            }
        }

        private static void AddPlayerToDatabase(Player p)
        {
            string query = "INSERT INTO players(name, twitch, twitter, team, country, smashgg_id) VALUES (@0, @1, @2, @3, @4, @5)";
            SQLiteCommand command = new SQLiteCommand(_conn);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@0", p.Name);
            command.Parameters.AddWithValue("@1", p.Twitch);
            command.Parameters.AddWithValue("@2", p.Twitter);
            command.Parameters.AddWithValue("@3", (p.Team != null && p.Team.Id != -1) ? p.Team.Id.ToString() : null);
            command.Parameters.AddWithValue("@4", p.Country?.ShortName);
            command.Parameters.AddWithValue("@5", p.SmashggId == -1 ? null : p.SmashggId.ToString());

            command.ExecuteNonQuery();
            p.Id = (int)_conn.LastInsertRowId;
        }

        private static void AddTeamToDatabase(Team t)
        {
            string query = "INSERT INTO teams(name, shortname) VALUES (@0, @1)";
            SQLiteCommand command = new SQLiteCommand(_conn);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@0", t.Name);
            command.Parameters.AddWithValue("@1", t.ShortName);

            command.ExecuteNonQuery();
            t.Id = (int)_conn.LastInsertRowId;
        }

        private static void RemovePlayerFromDatabase(Player p)
        {
            string query = "DELETE FROM players WHERE id = " + p.Id;
            SQLiteCommand command = new SQLiteCommand(query, _conn);
            command.ExecuteNonQuery();
        }

        private static void UpdatePlayerToDatabase(Player p)
        {
            if (p.Team != null && p.Team.Id == -1)
                AddTeam(p.Team);

            string query = "UPDATE players SET name = @0, twitch = @1, team = "+
                ((p.Team != null && p.Team.Id != -1) ? p.Team.Id.ToString() : "NULL")
                +", twitter = @2 WHERE id = " + p.Id;
            SQLiteCommand command = new SQLiteCommand(_conn);
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            command.Parameters.AddWithValue("@0", p.Name);
            command.Parameters.AddWithValue("@1", p.Twitch);
            command.Parameters.AddWithValue("@2", p.Twitter);
            command.ExecuteNonQuery();
        }


        private static void RemoveTeamFromDatabase(Team t)
        {
            string query = "DELETE FROM teams WHERE id = " + t.Id;
            SQLiteCommand command = new SQLiteCommand(query, _conn);
            command.ExecuteNonQuery();
        }



        #endregion

        #region SQL Parsing

        private static Player CreatePlayerFromResult(SQLiteDataReader result)
        {
            Player p = new Player();
            p.Id = (int)(long)result["playerId"];
            p.Name = (string)result["playerName"];

            if (result["playerTwitter"] is DBNull == false)
                p.Twitter = (string)result["playerTwitter"];
            if (result["playerTwitch"] is DBNull == false)
                p.Twitch = (string)result["playerTwitch"];

            if (result["teamId"] is DBNull == false && (long)result["teamId"] > 0)
            {
                int teamId = (int)(long)result["teamId"];
                if (_teams.Count((t) => t.Id == teamId) == 1)
                {
                    Team team = _teams.First((t) => t.Id == teamId);
                    p.Team = team;
                }
                else
                {
                    Team t = new Team();
                    t.Id = teamId;
                    if (result["teamName"] is DBNull == false)
                        t.Name = (string)result["teamName"];
                    t.ShortName = (string)result["teamShortName"];
                    p.Team = t;
                }
            }

            if (result["playerCountry"] is DBNull == false)
                p.Country = _countries.Find((c) => c.ShortName == (string)result["playerCountry"]);
            if (result["playerSmashggId"] is DBNull == false)
                p.SmashggId = (int)(long)result["playerSmashggId"];

            return p;
        }

        private static Team CreateTeamFromResult(SQLiteDataReader result)
        {
            Team t = new Team();
            t.Id = (int)(long)result["id"];
            if (result["name"] is DBNull == false)
                t.Name = (string)result["name"];
            t.ShortName = (string)result["shortname"];
            return t;
        }

        #endregion

        #region Public operations

        public static void AddPlayer(Player p)
        {
            // Do we need to register player's team as well ?
            if(p.Team != null && p.Team.Id == -1 && _teams.Count((t) => t.ShortName == p.Team.ShortName) == 0)
            {
                AddTeam(p.Team);
            }
            else if(_teams.Count((t) => t.ShortName == p.Team?.ShortName) == 1)
            {
                p.Team = _teams.First((t) => t.ShortName == p.Team.ShortName);
            }

            _players.Add(p);
        }

        public static void AddTeam(Team t)
        {
            _teams.Add(t);
        }

        public static void UpdateOrAddPlayer(Player player)
        {
            // Check with smash.gg id
            if (player.SmashggId != -1 && _players.Count((p => p.SmashggId == player.SmashggId)) == 1)
            {
                Player old = _players.First((p => p.SmashggId == player.SmashggId));
                player.Id = old.Id;
                int oldIndex = _players.IndexOf(old);
                _players[oldIndex] = player;
                
            }
            // Check with player id
            else if (player.Id != -1 && _players.Count((p => p.Id == player.Id)) == 1)
            {
                Player old = _players.First((p => p.Id == player.Id));
                int oldIndex = _players.IndexOf(old);
                _players[oldIndex] = player;
            }
            else
            {
                AddPlayer(player);
            }

        }

        public static void DeletePlayer(Player p)
        {
            if (!_players.Remove(p))
            {
                // Handle error: player not in database
            }
        }

        public static Player GetById(int id)
        {
            int result = _players.Count((p) => p.Id == id);
            if (result == 0)
                return null;
            else
                return _players.First((p) => p.Id == id);
        }

        public static Player GetBySmashggId(int id)
        {
            int result = _players.Count((p) => p.SmashggId == id);
            if (result == 0)
                return null;
            else
                return _players.First((p) => p.SmashggId == id);
        }

        public static List<Player> GetAllPlayers()
        {
            return _players.ToList();
        }

        public static List<Player> GetPlayersByName(string pattern)
        {
            return _players.Where((p) => p.Name.StartsWith(pattern)).ToList();
        }

        public static List<Team> GetTeams()
        {
            return _teams.ToList();
        }

        public static Team GetTeam(int id)
        {
            int result = _teams.Count((p) => p.Id == id);
            if (result == 0)
                return null;
            else
                return _teams.First((p) => p.Id == id);
        }

        public static void DeleteTeam(Team team)
        {
            IEnumerable<Player> players = _players.Where((p) => p.Team?.Id == team.Id);
            foreach (var p in players)
            {
                p.Team = null;
            }
            _teams.Remove(team);
        }

        public static List<Country> GetCountries()
        {
            return _countries;
        }

        public static Country GetCountryByName(string englishName)
        {
            return _countries.Find((c) => c.FullName == englishName);
        }

        #endregion






        public delegate void ContentChanged();
        public static event ContentChanged DatabaseContentModified;
        private static void RaiseDatabaseContentModified()
        {
            DatabaseContentModified?.Invoke();
        }
    }
}
