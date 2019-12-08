using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Entities;
using UltimateStreamMgr.Api.Messages;
using UltimateStreamMgr.StreamDeck;
using UltimateStreamMgr.ViewModel;
using WebSocketSharp;
using WebSocketSharp.Server;


namespace UltimateStreamMgr.StreamDeck
{

    public class StreamDeckLink : WebSocketBehavior
    {
        private const int _port = 28200;
        private WebSocketServer _server;

        private List<StreamDeckService> _clients = new List<StreamDeckService>();

        private List<DockWindowViewModel> _vms;

        public StreamDeckLink(List<DockWindowViewModel> vms)
        {
            _vms = vms;

            _server = new WebSocketServer(IPAddress.Loopback, _port);
            _server.AddWebSocketService("/", () =>
            {
                StreamDeckService newService = new StreamDeckService(this);
                newService.OnConnectionOpened += ConnectionOpened;
                newService.OnConnectionClosed += ConnectionClosed;
                newService.OnMessageReceived += MessageReceived;
                return newService;
            });
            _server.Start();
        }

        private void MessageReceived(StreamDeckService conn, BaseMessage mess)
        {
            if (mess is IncrementPlayerScoreMessage incrementMessage)
            {
                RunningSetViewModel currentSet = _vms.Find((v) => v is RunningSetViewModel) as RunningSetViewModel;
                if (incrementMessage.Player == 1)
                {
                    currentSet.IncrementEntrant1Command?.Execute(null);
                }
                else if (incrementMessage.Player == 2)
                {
                    currentSet.IncrementEntrant1Command?.Execute(null);
                }
            }
            else if (mess is ChangeCharacterMessage changeCharacter)
            {
                RunningSetViewModel currentSet = _vms.Find((v) => v is RunningSetViewModel) as RunningSetViewModel;
                var availableCharacter = currentSet.CharacterList.Where(c => c.Category == "ultimate").ToList();
                if (changeCharacter.PlayerId == 1)
                {
                    currentSet.Opponent1.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 2)
                {
                    currentSet.Opponent2.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 3)
                {               
                    currentSet.Opponent3.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 4)
                {
                    currentSet.Opponent4.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
            }
            else if (mess is GetCharacterListMessage)
            {
                RunningSetViewModel currentSet = _vms.Find((v) => v is RunningSetViewModel) as RunningSetViewModel;
                var availableCharacter = currentSet.CharacterList.ToList().Where(c=>c.Category=="ultimate").ToList();
                List<CharacterInfo> charaList = new List<CharacterInfo>();
                foreach (var character in availableCharacter)
                {
                    string[] info = character.Name.Split('_');
                    string charaName = string.Join("_",info.SubArray(0, info.Length - 1));
                    string charaAlt = info.Last();
                    if (info.Length == 1) // No alt
                    {
                        charaList.Add(new CharacterInfo{Name = charaName, IconPath = character.FilePath});
                    }
                    else // There is alt
                    {
                        var sourceChara = charaList.Find(c => c.Name == charaName);
                        if (sourceChara == null) // But we have to create the first entry
                        {
                            var characterEntry = new CharacterInfo {Name = charaName, IconPath = character.FilePath};
                            var altEntry = new CharacterAltInfo() { Name = character.Name, IconPath = character.FilePath };
                            characterEntry.Alts.Add(altEntry);
                            charaList.Add(characterEntry);
                        }
                        else
                        {
                            if (int.Parse(charaAlt) == 0) // In the case of the first entry was not created with the good alt, replace it by the good first one.
                            {
                                sourceChara.IconPath = character.FilePath;
                            }
                            var altEntry = new CharacterAltInfo() { Name = character.Name, IconPath = character.FilePath };
                            sourceChara.Alts.Add(altEntry);
                        }
                    }
                }
                conn.Send(new CharacterListMessage{Characters = charaList});
            }
        }

        private void ConnectionClosed(StreamDeckService conn)
        {
            _clients.Remove(conn);
        }

        private void ConnectionOpened(StreamDeckService conn)
        {
            _clients.Add(conn);
        }

        ~StreamDeckLink()
        {
            _server.Stop();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }

    public class StreamDeckService : WebSocketBehavior
    {
        private StreamDeckLink _server;

        public StreamDeckService(StreamDeckLink server)
        {
            IgnoreExtensions = true;
            _server = server;
        }

        protected override void OnOpen()
        {
            OnConnectionOpened?.Invoke(this);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            OnConnectionClosed?.Invoke(this);
        }

        protected override void OnError(ErrorEventArgs e)
        {
            // ???
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            BaseMessage message = JsonConvert.DeserializeObject<BaseMessage>(e.Data, settings);

            OnMessageReceived?.Invoke(this, message);
        }

        public void Send(BaseMessage message)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            this.Send(JsonConvert.SerializeObject(message, settings));
        }

        public delegate void ConnectionEvent(StreamDeckService conn);
        public event ConnectionEvent OnConnectionOpened;
        public event ConnectionEvent OnConnectionClosed;

        public delegate void MessageEvent(StreamDeckService conn, BaseMessage mess);
        public event MessageEvent OnMessageReceived;

    }
}
