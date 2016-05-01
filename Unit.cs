using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerMonitorClient
{
    public enum Unit
    {
        [Description("KByte.")]
        KB = 0,
        [Description("MByte.")]
        MB = 1,
        [Description("GByte.")]
        GB = 2
    }
}
