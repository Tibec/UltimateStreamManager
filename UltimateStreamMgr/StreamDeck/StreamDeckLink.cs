using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CefSharp.Internals;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Entities;
using UltimateStreamMgr.Api.Messages;
using UltimateStreamMgr.Api.Messages.Client;
using UltimateStreamMgr.Api.Messages.Server;
using UltimateStreamMgr.Model;
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

        private RunningSetViewModel _runningSetVM;

        public StreamDeckLink(RunningSetViewModel runningSetVM)
        {
            _runningSetVM = runningSetVM;

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

            RegisterEvents();
        }

        private void RegisterEvents()
        {
            _runningSetVM.PropertyChanged += RefreshRegisteredEvent;

            _runningSetVM.Opponent1.PropertyChanged += OnOpponentChanged;
            _runningSetVM.Opponent2.PropertyChanged += OnOpponentChanged;
        }

        private void RefreshRegisteredEvent(object sender, PropertyChangedEventArgs e)
        {
            _runningSetVM.Opponent1.PropertyChanged += OnOpponentChanged;
            _runningSetVM.Opponent2.PropertyChanged += OnOpponentChanged;

            OnOpponentChanged(null, null);
        }

        private void OnOpponentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null || e.PropertyName == "Character")
            {
                SendMessageToAllClients(new CurrentCharactersMessage
                {
                    Player1CharacterIconPath = _runningSetVM.Opponent1.Character.FilePath,
                    Player2CharacterIconPath =  _runningSetVM.Opponent2.Character.FilePath
                });
            }
            if (sender == null || e.PropertyName == "Score")
            {
                SendMessageToAllClients(new CurrentScoreMessage
                {
                    ScoreP1 = _runningSetVM.Opponent1.Score,
                    ScoreP2 = _runningSetVM.Opponent2.Score
                });
            }

        }

        public void SendMessageToAllClients(BaseMessage message)
        {
            foreach (var client in _clients)
            {
                client.Send(message);
            }
        }

        private void MessageReceived(StreamDeckService conn, BaseMessage mess)
        {
            if (mess is IncrementPlayerScoreMessage incrementMessage)
            {
                if (incrementMessage.Player == 1)
                {
                    _runningSetVM.IncrementEntrant1Command?.Execute(null);
                }
                else if (incrementMessage.Player == 2)
                {
                    _runningSetVM.IncrementEntrant2Command?.Execute(null);
                }
            }
            else if (mess is DecrementPlayerScoreMessage decrementMessage)
            {
                if (decrementMessage.Player == 1)
                {
                    _runningSetVM.DecrementEntrant1Command?.Execute(null);
                }
                else if (decrementMessage.Player == 2)
                {
                    _runningSetVM.DecrementEntrant2Command?.Execute(null);
                }
            }
            else if (mess is ChangeCharacterMessage changeCharacter)
            {
                var availableCharacter = _runningSetVM.CharacterList.Where(c => c.Category == _runningSetVM.SelectedCharacterCategory).ToList();
                if (changeCharacter.PlayerId == 1)
                {
                    _runningSetVM.Opponent1.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 2)
                {
                    _runningSetVM.Opponent2.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 3)
                {
                    _runningSetVM.Opponent3.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
                else if (changeCharacter.PlayerId == 4)
                {
                    _runningSetVM.Opponent4.Character = availableCharacter.Find(c => c.Name == changeCharacter.CharacterName);
                }
            }
            else if (mess is GetCharacterListMessage)
            {
                var availableCharacter = _runningSetVM.CharacterList.ToList().Where(c=>c.Category==_runningSetVM.SelectedCharacterCategory).ToList();
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
            else if (mess is SwapPlayerMessage)
            {
                _runningSetVM.SwapPlayerCommand?.Execute(null);
            }
            else if (mess is ResetScoreMessage)
            {
                _runningSetVM.ResetCommand?.Execute(null);
            }
            else if (mess is GetCurrentCharactersMessage)
            {
                conn.Send(new CurrentCharactersMessage
                {
                    Player1CharacterIconPath = _runningSetVM.Opponent1.Character.FilePath,
                    Player2CharacterIconPath = _runningSetVM.Opponent2.Character.FilePath
                });
            }
            else if (mess is GetCurrentScoreMessage)
            {
                conn.Send(new CurrentScoreMessage
                {
                    ScoreP1 = _runningSetVM.Opponent1.Score,
                    ScoreP2 = _runningSetVM.Opponent2.Score
                });
            }
        }

        private void ConnectionClosed(StreamDeckService conn)
        {
            _clients.Remove(conn);
        }

        private void ConnectionOpened(StreamDeckService conn)
        {
            _clients.Add(conn);
            conn.Send(new CurrentCharactersMessage
            {
                Player1CharacterIconPath = _runningSetVM.Opponent1.Character.FilePath,
                Player2CharacterIconPath = _runningSetVM.Opponent2.Character.FilePath
            });
            conn.Send(new CurrentScoreMessage
            {
                ScoreP1 = _runningSetVM.Opponent1.Score,
                ScoreP2 = _runningSetVM.Opponent2.Score
            });
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
