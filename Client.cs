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

        public static string Login(string username, string password)
        {
            return webClient.DownloadString(baseUrl + String.Format("login.php?username={0}&password={1}", username, password));
        }
    }
}
