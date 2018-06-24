using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro;
using Microsoft.Win32;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Navigation;
using UltimateStreamMgr.Model;
using UltimateStreamMgr.View;
using UltimateStreamMgr.ViewModel;

namespace UltimateStreamMgr
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                bool firstLaunch = false;

                GenerateDemoLicenceKey();

                //AppDomain.CurrentDomain.AppendPrivatePath(@"dependencies");

                try
                {
                    if (File.Exists("config.xml"))
                    {
                        Configuration.Instance.Load("config.xml");
                    }
                    else
                        firstLaunch = true;
                }
                catch {
                    LogManager.GetCurrentClassLogger().Error("Couldn't read a valid configuration file. A new one will be created");
                    firstLaunch = true;
                }

                PlayerDatabase.Init();

                InitializeQuickConverter();

                MainWindow = new MainWindow(firstLaunch);

                if (Configuration.Instance.Window.AppHeight != 0
                    && Configuration.Instance.Window.AppWidth != 0)
                {

                    MainWindow.Width = Configuration.Instance.Window.AppWidth;
                    MainWindow.Height = Configuration.Instance.Window.AppHeight;
                }

                MainWindow.Show();

                if (!string.IsNullOrEmpty(Configuration.Instance.Window.DockDisposition) && !firstLaunch)
                {
                    Messenger.Default.Send(Configuration.Instance.Window.DockDisposition);
                }
                else
                {
                    (MainWindow.DataContext as MainViewModel).ResetLayout.Execute(null);
                }
            }
            catch (Exception exception)
            {
                LogManager.GetCurrentClassLogger().Fatal(exception);
                App.Current.Shutdown();
            }

        }
        #region Demo key generation

        private void GenerateDemoLicenceKey()
        {
            // Dirty hack, not really legal but since AvalonDock 3.3 is not working correctly
            // and I don't have that much time to investigate/repair the broken features I need
            // this code create a 'demo key' for 45 days at every startup based on the nugget script

            // It will be removed once WPF Toolkit Community Edition 3.6 will be released.

            string regPath = @"Software\Xceed Software\Licenses\WTK";
            string regKey = "3.6";
            
            BitArray licence = new BitArray(65, false);
            licence[6] = licence[64] = true;
            DateTime date = new DateTime(2000, 11, 17);
            int days = DateTime.Today.Subtract(date).Days;
            string prodId = "WTK36";

            MapBits(ref licence, /* productIndex */ 53, new byte[] { 3, 16, 29, 41, 53, 61, 0xFF });
            MapBits(ref licence, /* version */ 36, new byte[] { 4, 15, 25, 34, 43, 50, 58, 0xFF });
            MapBits(ref licence, days, new byte[] { 2, 7, 12, 17, 22, 26, 31, 37, 42, 47, 51, 55, 59, 62, 0xFF });

            short[] a1 = new short[5] { (short)'W', (short)'T', (short)'K', (short)'3', (short)'6', };
            byte[] a2 = ToByteArray(licence);

            short[] a = new short[a1.Length + a2.Length];
            Array.Copy(a1, 0, a, 0, a1.Length);
            Array.Copy(a2, 0, a, a1.Length, a2.Length);

            byte checksum = CalculateChecksum(a);
          
            MapBits(ref licence, checksum, new byte[] { 0, 9, 18, 27, 36, 45, 54, 63, 0xFF });
          
            string regKeyValue = prodId + GenAlpha(licence);

            RegistryKey root = Registry.CurrentUser;
            foreach(var subkey in regPath.Split('\\'))
            {
                RegistryKey key = root.OpenSubKey(subkey, true);
                if (key == null)
                {
                    root = root.CreateSubKey(subkey);
                }
                else
                {
                    root = key; 
                }
            }
            root.SetValue(regKey, regKeyValue);
        }

        private string GenAlpha(BitArray ba)
        {
            string suffix = "";
            string AlphaNumLookup = "ABJCKTDL4UEMW71FNX52YGP98Z63HRS0";

            int mask = 0x10;
            int value = 0;
            for(int i = 0; i < ba.Length;i++)
            {
                if( mask == 0 )
                {
                    suffix += AlphaNumLookup[value];
                    value = 0;
                    mask = 0x10;
                }

                if(ba[i])
                {
                    value = value | mask;
                }

                mask = mask >> 1;
              }

            suffix += AlphaNumLookup[value];

            return suffix + 'A';
        }

        private byte CalculateChecksum(short[] b)
        {
            short dw1 = 0;
            short dw2 = 0;

            for(int i=0;i < b.Length;i++)
            {
                dw1 += b[i];
                dw2 += dw1;
            }

            // Reduce to 8 bits
            short r1 = (short)(dw2 ^ dw1);
            byte r2 = (byte)((r1 >> 8) ^ (r1 & 0x00FF));

            return r2;
        }

        private void MapBits(ref BitArray barray, int value, byte[] bitPos)
        {
            for ( int i = 0; i < bitPos.Length - 1; ++i )
            {
                int x = 1 << i;
                barray[bitPos[i]] = (value & x) != 0;
            }
        }

        private byte[] ToByteArray(BitArray barray)
        {
            byte[] array = new byte[9];

            for( byte i = 0; i < barray.Length; i++ )
            {
                if(barray[i])
                {
                    int mod = i % 8;
                    int index = (i - mod) / 8;
                    array[index] = (byte)((array[index]) | (128 >> mod));
                }
            }

            return array;
        }

#endregion

        private void InitializeQuickConverter()
        {
            QuickConverter.EquationTokenizer.AddNamespace(typeof(object));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(Visibility));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(string));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(ChannelStatus));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(OutputFormat));
            QuickConverter.EquationTokenizer.AddNamespace(typeof(SetMode));
        }
    }
}
