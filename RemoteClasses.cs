using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;

namespace ComputerMonitorClient.RemoteClasses
{
    public class Status
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string token { get; set; }
    }

    public class Device
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Usage
    {
        public string upload { get; set; }
        public string download { get; set; }
        public string date { get; set; }
    }
}
