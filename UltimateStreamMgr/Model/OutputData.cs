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
                Dictionary<string, object> values = RetrieveAllProperties();
                if (Configuration.Instance.Output.OutputFormat == OutputFormat.Text)
                {
                    foreach(var value in values)
                    {
                        string filename = Path.Combine(Configuration.Instance.Output.Folder, value.Key +".txt");
                        using (var fileStream = File.Open(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            using (StreamWriter sw = new StreamWriter(fileStream))
                            {
                                sw.Write(value.Value);
                            }
                        }
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

        public static Dictionary<string, object> RetrieveAllProperties()
        {
            Dictionary<string, object> props = new Dictionary<string, object>();
            RetrieveSubPropertiesValue(ref props, Data);
            return props;
        }

        public static void RetrieveSubPropertiesValue(ref Dictionary<string, object> values, object obj, string prefix = "")
        {
            if (obj == null)
            {
                return;
            }
            else if(obj is CustomKey)
            {
                CustomKey customKey = obj as CustomKey;
                values.Add(customKey.Name, customKey.Value);
            }
            else if (obj.GetType().IsPrimitive || obj.GetType() == typeof(string))
            {
                values.Add(prefix, obj);
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

        #region Replay
        
        public List<Replay> Replays { get; set; }

        #endregion


    }
}
