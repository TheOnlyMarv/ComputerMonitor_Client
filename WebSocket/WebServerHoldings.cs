using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComputerMonitorClient.WebSocket
{
    public class WebServerHoldings
    {
        private static WebServerHoldings instance;

        private WebSocketServer webSocketServer;
        private Thread webServerThread;
        private string connectionString;

        private WebServerHoldings()
        {
            webSocketServer = new WebSocketServer();
        }

        public static WebServerHoldings getInstance()
        {
            if (instance == null)
            {
                instance = new WebServerHoldings();
            }
            return instance;
        }

        public void StartWebServerOnNewThread()
        {
            if (webServerThread == null)
            {
                connectionString = webSocketServer.Initialize();
                webServerThread = new Thread(new ThreadStart(webSocketServer.StartServer));
                webServerThread.Start();
            }
        }

        public QRCode GetQrCode()
        {
            StartWebServerOnNewThread();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrData = qrGenerator.CreateQrCode(connectionString, QRCodeGenerator.ECCLevel.Q);
            return new QRCode(qrData);
        }

        public void StopWebServerThread()
        {
            if (webServerThread != null)
            {
                webSocketServer.StopServer();
                webServerThread.Abort();
            }
        }
    }
}
