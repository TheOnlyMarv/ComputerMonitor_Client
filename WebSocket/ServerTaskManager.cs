using ComputerMonitorClient.RemoteClasses;
using ComputerMonitorClient.WebSocket.Server;
using NAudio.CoreAudioApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputerMonitorClient.WebSocket
{
    public class ServerTaskManager
    {
        private static ServerTaskManager instance;

        private ServerTaskManager()
        {

        }

        public static ServerTaskManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ServerTaskManager();
            }
            return instance;
        }

        public void OnMessageReceived(string text)
        {
            ServerWebSocketService swss = WebSocketSettings.WsServer;
            try
            {
                RemoteAction remote = JsonConvert.DeserializeObject<RemoteAction>(text);
                if (remote.action == null)
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.MissingMemberHandling = MissingMemberHandling.Error;
                    RemoteResponse response = JsonConvert.DeserializeObject<RemoteResponse>(text, settings);
                    // response ereignis
                }
                else
                {
                    if (AssignRemoteAction(remote.action.Value, remote.value))
                    {
                        if (swss != null)
                        {
                            RemoteResponse rr = new RemoteResponse();
                            rr.status = 200;
                            rr.message = "ok";
                            swss.SendMessage(JsonConvert.SerializeObject(rr));
                        }
                    }
                }
            }
            catch (JsonSerializationException ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            catch(JsonReaderException jre)
            {
                Debug.WriteLine("Received invalid json: " + text);
                RemoteResponse rr = new RemoteResponse();
                rr.status = 406;
                rr.message = "Invalid JSON-Object";
                swss.SendMessage(JsonConvert.SerializeObject(rr));
            }
        }

        private bool AssignRemoteAction(RemoteClasses.Action action, int value)
        {
            bool returnValue = false;
            switch (action)
            {
                case RemoteClasses.Action.Volumn:
                    try
                    {
                        MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
                        MMDevice device = DevEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

                        device.AudioEndpointVolume.MasterVolumeLevelScalar = (float)value / 100.0f;
                        returnValue = true;
                    }
                    catch (Exception)
                    {
                        returnValue = false;
                    }
                    break;
                default:
                    break;
            }
            return returnValue;
        }
    }
}
