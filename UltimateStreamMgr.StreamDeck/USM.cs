using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.StreamDeck
{
    public static class USM
    {
        public static bool IsConnected { get; private set; }

        private static UltimateStreamMgrConnection _connection;
        private const int _port = 28200;

        public delegate void MessageEvent(BaseMessage mess);
        public static event MessageEvent OnMessageReceived;

        public static void Run()
        {
            _connection = new UltimateStreamMgrConnection(_port);
            _connection.OnConnected += OnConnected;
            _connection.OnDisconnected += OnDisconnected;
            _connection.OnMessageReceived += OnMessage;
            _connection.Run();
        }

        private static void OnDisconnected(object sender, EventArgs e)
        {
            IsConnected = false;
            _connection.Run();
        }

        private static void OnConnected(object sender, EventArgs e)
        {
            IsConnected = true;
        }

        private static void OnMessage(BaseMessage e)
        {
            OnMessageReceived?.Invoke(e);
        }

        public static void Send(BaseMessage message)
        {
            _connection.SendAsync(message).RunSynchronously();
        }
    }

}
