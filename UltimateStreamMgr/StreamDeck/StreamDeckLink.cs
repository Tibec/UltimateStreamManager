using System.Net;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;


namespace UltimateStreamMgr.StreamDeck
{

    public class StreamDeckLink : WebSocketBehavior
    {
        private const int _port = 28200;
        private WebSocketServer _server;
        public StreamDeckLink()
        {
            var _server = new WebSocketServer(IPAddress.Loopback, _port);
            _server.AddWebSocketService<StreamDeckService>("/");
            _server.Start();
        }

        ~StreamDeckLink()
        {
            _server.Stop();
        }
    }

    public class StreamDeckService : WebSocketBehavior
    {
        protected override Task OnMessage(MessageEventArgs e)
        {
            
        }
    }
}
