using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerMonitorClient
{
    public interface IMainSwitch
    {
        void CloseApplication();
        void SwitchToOptionPage();
        void SwitchToMeasuringPage();
    }
}
