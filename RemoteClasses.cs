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

    public partial class Device
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime? last_used { get; set; }
    }

    public class Usage
    {
        public double upload { get; set; }
        public double download { get; set; }
        public DateTime date { get; set; }
    }

    public partial class Device
    {
        public IList<Usage> usage { get; set; }
    }
}
