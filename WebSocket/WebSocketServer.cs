using ComputerMonitorClient.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSockets;
using WebSockets.Common;

namespace ComputerMonitorClient.WebSocket
{
    public class WebSocketServer
    {
        private bool interrupt;

        public string Initialize()
        {
            return "ws://" + Utilities.GetLocalIPAddress() + ":" + WebSocketSettings.port + WebSocketSettings.path ;
        }

        public void StartServer()
        {
            IWebSocketLogger logger = new WebSocketLogger();
            interrupt = false;
            try
            {
                ServiceFactory serviceFactory = new ServiceFactory("", logger);

                using (WebServer server = new WebServer(serviceFactory,logger))
                {
                    server.Listen(WebSocketSettings.port);
                    while (true && !interrupt)
                    {
                        Thread.Sleep(5000);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(typeof(WebSocketServer), ex);
            }
        }

        public void StopServer()
        {
            interrupt = true;
        }
    }
}
