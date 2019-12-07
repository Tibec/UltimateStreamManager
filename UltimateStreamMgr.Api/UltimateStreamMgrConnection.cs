using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UltimateStreamMgr.Api.Messages;

namespace UltimateStreamMgr.Api
{
    public class UltimateStreamMgrConnection
    {
        private const int BufferSize = 1024 * 1024;

        private ClientWebSocket _webSocket;
        private SemaphoreSlim _sendSocketSemaphore = new SemaphoreSlim(1);
        private CancellationTokenSource _cancelSource = new CancellationTokenSource();

        public int Port { get; private set; }

        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisconnected;

        public delegate void MessageEvent(BaseMessage message);
        public event MessageEvent OnMessageReceived;

        public UltimateStreamMgrConnection(int port)
        {
            this.Port = port;
        }

        public void Run()
        {
            if (_webSocket == null)
            {
                _webSocket = new ClientWebSocket();
                _ = this.RunAsync();
            }
        }

        public void Stop()
        {
            _cancelSource.Cancel();
        }

        public Task SendAsync(BaseMessage message)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
            return SendAsync(JsonConvert.SerializeObject(message, settings));
        }

        private async Task SendAsync(string text)
        {
            try
            {
                if (_webSocket != null)
                {
                    try
                    {
                        await _sendSocketSemaphore.WaitAsync();
                        byte[] buffer = Encoding.UTF8.GetBytes(text);
                        await _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancelSource.Token);
                    }
                    finally
                    {
                        _sendSocketSemaphore.Release();
                    }
                }
            }
            catch
            {
                await DisconnectAsync();
            }
        }

        private async Task RunAsync()
        {
            try
            {
                await _webSocket.ConnectAsync(new Uri($"ws://localhost:{this.Port}"), _cancelSource.Token);
                if (_webSocket.State != WebSocketState.Open)
                {
                    await DisconnectAsync();
                    return;
                }

                OnConnected?.Invoke(this, new EventArgs());
                await ReceiveAsync();
            }
            finally
            { 
                await DisconnectAsync();
            }
        }

        private async Task<WebSocketCloseStatus> ReceiveAsync()
        {
            byte[] buffer = new byte[BufferSize];
            ArraySegment<byte> arrayBuffer = new ArraySegment<byte>(buffer);
            StringBuilder textBuffer = new StringBuilder(BufferSize);

            try
            {
                while (!_cancelSource.IsCancellationRequested && _webSocket != null)
                {
                    WebSocketReceiveResult result = await _webSocket.ReceiveAsync(arrayBuffer, _cancelSource.Token);

                    if (result != null)
                    {
                        if (result.MessageType == WebSocketMessageType.Close ||
                            (result.CloseStatus.HasValue && result.CloseStatus.Value != WebSocketCloseStatus.Empty))
                        {
                            return result.CloseStatus.GetValueOrDefault();
                        }
                        else if (result.MessageType == WebSocketMessageType.Text)
                        {
                            textBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                            if (result.EndOfMessage)
                            {
                                string receivedMessage = textBuffer.ToString();

                                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                                BaseMessage message = JsonConvert.DeserializeObject<BaseMessage>(receivedMessage, settings);

                                OnMessageReceived?.Invoke(message);

                                textBuffer.Clear();
                            }
                        }
                    }
                }
            }
            catch { }

            return WebSocketCloseStatus.NormalClosure;
        }

        private async Task DisconnectAsync()
        {
            if (_webSocket != null)
            {
                ClientWebSocket socket = _webSocket;
                _webSocket = null;

                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", _cancelSource.Token);
                }
                catch { }

                try
                {
                    socket.Dispose();
                }
                catch { }

                OnDisconnected?.Invoke(this, new EventArgs());
            }
        }
    }
}

