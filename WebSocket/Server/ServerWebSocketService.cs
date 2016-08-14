using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WebSockets.Common;
using WebSockets.Server.WebSocket;

namespace ComputerMonitorClient.WebSocket.Server
{
    public class ServerWebSocketService : WebSocketService
    {
        private readonly IWebSocketLogger _logger;

        public ServerWebSocketService(Stream stream, TcpClient tcpClient, string header, IWebSocketLogger logger)
            : base(stream, tcpClient, header, true, logger)
        {
            _logger = logger;
        }

        protected override void OnTextFrame(string text)
        {
            ServerTaskManager.GetInstance().OnMessageReceived(text);
            //string response = "ServerABC: " + text;
            //base.Send(response);
            //_logger.Information(this.GetType(), response);
        }

        public void SendMessage(string text)
        {
            base.Send(text);
            //_logger.Information(this.GetType(), text);
        }
    }
}
