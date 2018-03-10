using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UltimateStreamMgr.Model.Api
{
    abstract public class BracketApi : WebApi
    {
        protected Dictionary<BracketCapabilities, bool> _allowedActions = new Dictionary<BracketCapabilities, bool>();

        public BracketApi(BracketSettings settings) : base()
        {
            _allowedActions[BracketCapabilities.PendingSetListing] = false;
            _allowedActions[BracketCapabilities.SetReporting] = false;
            _allowedActions[BracketCapabilities.UserSynchronization] = false;
        }

        public List<BracketCapabilities> GetSupportedActions()
        {
            List<BracketCapabilities> list = new List<BracketCapabilities>();
            foreach (var action in _allowedActions)
                if (action.Value)
                    list.Add(action.Key);
            return list;
        }

        public abstract Set GetSet(int id);

        public abstract List<Set> GetPendingSets(int bracketId, bool includeNonStream = false);

        public abstract List<Set> GetAllPendingSets(bool includeNonStream = false);

        public abstract List<Bracket> GetAvailablesBrackets(bool includeSets = false, bool includeParticipants = false);

        public abstract List<Player> GetEntrants(int bracketId);

        public abstract List<Player> GetAllEntrants();

        public abstract List<Top8> GetAvailablesTop8();



       // public virtual
    }

    public class BracketSettings
    {
        [XmlIgnore]
        public Type Api { get; set; }
    }

    public enum BracketCapabilities
    {
        PendingSetListing,
        SetReporting,
        UserSynchronization,
    }
}
