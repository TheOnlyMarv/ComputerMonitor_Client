using ComputerMonitorClient.RemoteClasses;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerMonitorClient
{
    public static class Client
    {

        private static WebClient webClient = new WebClient();
        private static string baseUrl = @"https://theonlymarv.de/cm/";

        private static string Downloader(string extendetUrl)
        {
            return webClient.DownloadString(baseUrl + extendetUrl);
        }

        public static Status Login(string username, string password)
        {
            return JsonConvert.DeserializeObject<Status>(Downloader(String.Format("login.php?username={0}&password={1}", username, password)));
        }

        public static Status Register(string username, string password)
        {
            return JsonConvert.DeserializeObject<Status>(Downloader(String.Format("register.php?username={0}&password={1}", username, password)));
        }

        public static List<Device> LoadDevices(string token)
        {
            return JsonConvert.DeserializeObject<List<Device>>(Downloader(String.Format("devicelist.php?token={0}", token)));
        }

        public static Status AddDevice(string token, string name)
        {
            return JsonConvert.DeserializeObject<Status>(Downloader(String.Format("deviceadd.php?token={0}&name={1}", token, name)));
        }
    }
}
