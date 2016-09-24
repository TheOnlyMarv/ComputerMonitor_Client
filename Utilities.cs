using ComputerMonitorClient.WebSocket.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ComputerMonitorClient
{
    public static class SettingFields
    {
        public static readonly string TOKEN             = "token";
        public static readonly string DEVICE_ID         = "deviceId";
        public static readonly string DEVICE_NAME       = "devicename";
        public static readonly string UNIT              = "unit";
        public static readonly string ADAPTER           = "adapter";
        public static readonly string NEW_UPLOAD        = "newupload";
        public static readonly string NEW_DOWNLOAD      = "newdownload";
        public static readonly string NEW_DATE          = "newdate";
        public static readonly string OLD_DOWNLOAD      = "olddownload";
        public static readonly string OLD_UPLOAD        = "oldupload";
        public static readonly string OLD_DATE          = "olddate";
        public static readonly string OLD_SYNCHRONIZED  = "oldsynchronized";
    }

    public static class WebSocketSettings
    {
        public static readonly string path = "/cm";
        public static readonly int port = 1337;
        public static ServerWebSocketService WsServer { get; set; }
    }

    public static class Utilities
    {
        public static BitmapSource convertBitmapToBitmapSource(Bitmap bitmap)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("Local IP Address Not Found!");
        }
    }
}
