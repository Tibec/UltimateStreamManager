using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace UltimateStreamMgr.Model
{
    static public class Output
    {
        private static Timer _timer;

        public static OutputData Data { get; set; } = new OutputData();

        static Output()
        {
            _timer = new Timer(500);
            _timer.Elapsed += Generate;
            _timer.Start();
        }

        public static void Generate(object s, EventArgs e)
        {
            _timer.Stop();
            Directory.CreateDirectory(Configuration.Instance.Output.Folder);

            if (Configuration.Instance.Output.OutputFormat == OutputFormat.Xml)
            {
                XmlSerializer serializer = new XmlSerializer(typeof(OutputData));
                string outputfile = Path.Combine(Configuration.Instance.Output.Folder, "output.xml");
                int tries = 0; bool ok = false;
                do
                {
                    try
                    {
                        // for some reasons FileMode.OpenOrCreate flag does not work so, 
                        // lets create the file manually
                        if (!File.Exists(outputfile))
                            File.Create(outputfile);

                        using (var fileStream = File.Open(outputfile, FileMode.OpenOrCreate | FileMode.Truncate, FileAccess.Write))
                        {
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {   
                                serializer.Serialize(sw, Data);
                            }
                        }
                        ok = true;
                    }
                    catch (Exception ex) { }
                } while (!ok && tries < 5);
            }
            else
            {
                Dictionary<string, object> values = RetrieveAllPropertiesValues();
                if (Configuration.Instance.Output.OutputFormat == OutputFormat.Text)
                {
                    foreach (var value in values)
                    {
                        if (value.Key == null)
                            continue;
                        string filename = Path.Combine(Configuration.Instance.Output.Folder, value.Key + ".txt");
                        if(value.Value == null)
                            File.WriteAllText(filename, string.Empty);
                        else
                            File.WriteAllText(filename, value.Value.ToString()  );
                    }
                    // copy image if possible
                    List<Opponent> listOpponent = new List<Opponent> { Data.Player1, Data.Player2, Data.Player3, Data.Player4 };
                    int i = 1;
                    foreach(var opponent in listOpponent)
                    {
                        if(opponent?.Character != null && !string.IsNullOrEmpty(opponent.Character.FilePath))
                        {
                            string imgExt = Path.GetExtension(opponent.Character.FilePath);
                            string outFile = Path.Combine(Configuration.Instance.Output.Folder, "Player"+i+".Character"+imgExt );
                            File.Copy(opponent.Character.FilePath, outFile, true);
                        }
                        ++i;
                    }
                    
                }
                else if(Configuration.Instance.Output.OutputFormat == OutputFormat.Template)
                {
                    //TODO: foreach file in the specified directory with .tpl extension
                    //         read those files, replace $key.subkey$ by their values
                    //         and save the output in filename.tpl.out
                }

            }
            _timer.Start();
        }

        public static Dictionary<string, object> RetrieveAllPropertiesValues()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            RetrieveSubProperties(ref props, Data.GetType());
            RetrieveSubPropertiesValue(ref props, Data);
            return props;
        }

        public static void RetrieveSubProperties(ref Dictionary<string, object> values, Type obj, string prefix = "")
        {
            if (obj == null)
            {
                return;
            }
            else if (obj == typeof(DateTime))
            {
                values[prefix] = null;
            }
            else if (obj.IsPrimitive || obj == typeof(string))
            {
                values[prefix] = null;
            }
            else
            {
                PropertyInfo[] subproperties = obj.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in subproperties)
                {
                    if (property.GetIndexParameters().Length != 0)
                        continue;
                    string newprefix;
                    if (string.IsNullOrEmpty(prefix))
                        newprefix = property.Name;
                    else
                        newprefix = prefix + "." + property.Name;

                    RetrieveSubProperties(ref values, property.PropertyType, newprefix);
                }
            }
        }



        public static void RetrieveSubPropertiesValue(ref Dictionary<string, object> values, object obj, string prefix = "")
        {
            if (obj == null)
            {
                return;
            }
            else if (obj is CustomKey)
            {
                CustomKey customKey = obj as CustomKey;
                values.Add(customKey.Name, customKey.Value);
            }
            else if (obj is DateTime)
            {
                values[prefix] = ((DateTime)obj).ToString("");
            }
            else if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string))
            {
                values[prefix] = obj;
            }
            else
            {
                PropertyInfo[] subproperties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in subproperties)
                {
                    if (property.GetIndexParameters().Length != 0)
                        continue;
                    string newprefix;
                    if (string.IsNullOrEmpty(prefix))
                        newprefix = property.Name;
                    else
                        newprefix = prefix + "." + property.Name;

                    RetrieveSubPropertiesValue(ref values, property.GetValue(obj), newprefix );
                }
            }
        }
    }

    public class OutputData
    {
        #region Running Set 

        public Opponent Player1 { get; set; }
        public Opponent Player2 { get; set; }
        public Opponent Player3 { get; set; }
        public Opponent Player4 { get; set; }

        public string RoundName { get; set; }

        #endregion

        #region Casters

        public Caster Caster1 { get; set; }
        public Caster Caster2 { get; set; }

        #endregion

        #region Other
        public List<CustomKey> Custom { get; set; }

        #endregion

        #region Top 8

        public List<Top8> Top8List { get; set; }

        #endregion

        #region
        public SocialMessage HighlightedTweet { get; set; }
        #endregion

        #region Replay

        public List<Replay> Replays { get; set; }

        #endregion


    }
}
