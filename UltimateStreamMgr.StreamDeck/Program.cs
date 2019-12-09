using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BarRaider.SdTools;
using streamdeck_client_csharp;

namespace UltimateStreamMgr.StreamDeck
{
    class Program
    {
        private const int _port = 28200;

        static void Main(string[] args)
        {
            USM.Run();
            SDWrapper.Run(args);

        }
    }
}
