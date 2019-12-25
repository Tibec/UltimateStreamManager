using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UltimateStreamMgr.Model.Api;
using UltimateStreamMgr.Model.Api.StreamApis;
using UltimateStreamMgr.ViewModel;

namespace UltimateStreamMgr.Model
{
    public class Configuration : ISerializable
    {
        #region Singleton implementation 
        private Configuration()
        {

        }

        static Configuration _instance;
        public static Configuration Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Configuration();
                return _instance;
            }
        }
        #endregion

        private static string _saveFile;

        #region Save/Load

        public void Initialize(string configPath)
        {
            _saveFile = configPath;
        }

        public void Load()
        {
            XmlSerializer xs = new XmlSerializer(typeof(Configuration), 
                Assembly.GetExecutingAssembly().GetTypes().Where(
                    (t) => t.IsSubclassOf(typeof(BracketSettings) )
                    || t.IsSubclassOf(typeof(StreamSettings))
                    || t.IsSubclassOf(typeof(SocialSettings))
                    ).ToArray()) ;
            using (StreamReader rd = new StreamReader(_saveFile))
            {
               _instance = xs.Deserialize(rd) as Configuration;
            }
        }

        public void Save(string saveFile = null)
        {
            if (string.IsNullOrEmpty(saveFile))
                saveFile = _saveFile;

            XmlSerializer xs = new XmlSerializer(typeof(Configuration),
                Assembly.GetExecutingAssembly().GetTypes().Where(
                    (t) => t.IsSubclassOf(typeof(BracketSettings))
                    || t.IsSubclassOf(typeof(StreamSettings))
                    || t.IsSubclassOf(typeof(SocialSettings))
                ).ToArray());

            using (StreamWriter wr = new StreamWriter(saveFile))
            {
                xs.Serialize(wr, this);
            }
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Instead of serializing this object, 
            // serialize a SingletonSerializationHelp instead.
            info.SetType(typeof(ConfigurationSerializationHelper));
            // No other values need to be added.
        }
        #endregion

        private OutputSettings _output = new OutputSettings();
        public OutputSettings Output
        {
            get { return _output; }
            set { _output = value; OnOutputSettingsChanged(); }
        }

        private AdvancedSettings _advanced;
        public AdvancedSettings Advanced
        {
            get { return _advanced; }
            set { _advanced = value; OnAdvancedSettingsChanged(); }
        }

        private BracketSettings _bracket;
        public BracketSettings Bracket
        {
            get { return _bracket; }
            set { _bracket = value; OnBracketSettingsChanged(); }
        }

        private StreamSettings _stream;
        public StreamSettings Stream
        {
            get { return _stream; }
            set { _stream = value; OnStreamSettingsChanged(); }
        }

        private ReplaySettings _replay = new ReplaySettings();
        public ReplaySettings Replay
        {
            get { return _replay; }
            set { _replay = value; OnStreamSettingsChanged(); }
        }
        private SocialSettings _social;
        public SocialSettings Social
        {
            get { return _social; }
            set { _social = value; OnSocialSettingsChanged(); }
        }

        public WindowSettings Window { get; set; } = new WindowSettings();

        public string SelectedGame { get; set; } = "";

        public bool DarkThemeEnabled { get; set; } = false;

        #region Events

        private void OnOutputSettingsChanged()
        {
            if (OutputSettingsChanged != null) OutputSettingsChanged();
        }
        private void OnStreamSettingsChanged()
        {
            if (StreamSettingsChanged != null) StreamSettingsChanged();
        }
        private void OnBracketSettingsChanged()
        {
            if (BracketSettingsChanged != null) BracketSettingsChanged();
        }
        private void OnReplaySettingsChanged()
        {
            if (ReplaySettingsChanged != null) ReplaySettingsChanged();
        }
        private void OnAdvancedSettingsChanged()
        {
            if (AdvancedSettingsChanged != null) AdvancedSettingsChanged();
        }

        private void OnSocialSettingsChanged()
        {
            if (SocialSettingsChanged != null) SocialSettingsChanged();
        }

        public delegate void SettingsChanged();
        public event SettingsChanged OutputSettingsChanged;
        public event SettingsChanged ReplaySettingsChanged;
        public event SettingsChanged StreamSettingsChanged;
        public event SettingsChanged BracketSettingsChanged;
        public event SettingsChanged AdvancedSettingsChanged;
        public event SettingsChanged SocialSettingsChanged;

        #endregion
    }

    #region Serialization Helper 
    internal sealed class ConfigurationSerializationHelper : IObjectReference
    {
        // This object has no fields (although it could).

        // GetRealObject is called after this object is deserialized.
        public object GetRealObject(StreamingContext context)
        {
            // When deserialiing this object, return a reference to 
            // the Singleton object instead.
            return Configuration.Instance;
        }
    }
    #endregion

    public class OutputSettings
    {
        public string Folder { get; set; } = @".\output";
        public OutputFormat OutputFormat { get; set; }
        public string TemplateFolder { get; set; }

        public List<CustomKey> SavedKeys;
    }

    public class WindowSettings
    {
        public string DockDisposition { get; set; } 
        public int AppHeight { get; set; }
        public int AppWidth { get; set; }
    }

    public enum OutputFormat
    {
        Text,
        Xml,
        Template
    }

    public class AdvancedSettings
    {

    }

    public class ReplaySettings
    {
        public bool Enabled { get; set; } 
        public string InputDirectory { get; set; }
        public string OutputDirectory { get; set; }
    }


}
