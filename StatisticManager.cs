using System.Linq;
using LiveCharts;
using System.ComponentModel;
using ComputerMonitorClient.RemoteClasses;
using System.Collections.Generic;
using System.Windows.Controls;
using System;

namespace ComputerMonitorClient
{
    public class StatisticManager
    {
        private BackgroundWorker backgroundWorker;
        private IList<Device> deviceList;
        private Device thisDevice;
        private Usage selecetedDate;

        private StatisticComponents statisticComponents;

        public StatisticManager(StatisticComponents statisticComponents)
        {
            this.statisticComponents = statisticComponents;
            this.statisticComponents.BtnNext.Click += BtnNext_Click;
            this.statisticComponents.BtnPrevious.Click += BtnPrevious_Click;
            this.statisticComponents.BtnNext.IsEnabled = false;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BtnPrevious_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int index = thisDevice.usage.IndexOf(selecetedDate);
            if (thisDevice.usage.Count > index + 1)
            {
                selecetedDate = thisDevice.usage[index + 1];
                statisticComponents.BtnNext.IsEnabled = true;
            }
            if (thisDevice.usage.Count -1  == index +1 )
            {
                statisticComponents.BtnPrevious.IsEnabled = false;
            }
            AssignSelectedDate();
        }

        private void BtnNext_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            int index = thisDevice.usage.IndexOf(selecetedDate);
            if (index - 1 > 0)
            {
                selecetedDate = thisDevice.usage[index - 1];
            }
            else if (index - 1 == 0)
            {
                selecetedDate = thisDevice.usage[index - 1];
                statisticComponents.BtnNext.IsEnabled = false;
            }
            statisticComponents.BtnPrevious.IsEnabled = true;
            AssignSelectedDate();
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int deviceId = Settings.DeviceId;
            thisDevice = deviceList.FirstOrDefault(x => x.id == deviceId);
            thisDevice.usage = thisDevice.usage.OrderByDescending(x => x.date).ToList();
            if (thisDevice.usage.Count >= 0)
            {
                selecetedDate = thisDevice.usage[0];
            }
            AssignSelectedDate();
        }

        private void AssignSelectedDate()
        {
            if ((DateTime.Today.Date - selecetedDate.date).Duration() == new TimeSpan(1, 0, 0, 0, 0))
            {
                statisticComponents.GroupBox.Header = "Yesterday";
            }
            else
            {
                statisticComponents.GroupBox.Header = String.Format("{0:dddd, MMMM d, yyyy}", selecetedDate.date);
            }
            Unit unit = (Unit)Enum.Parse(typeof(Unit), Settings.Unit.ToString());
            statisticComponents.TextDownload.Content = string.Format("{0} {1}", (selecetedDate.download / Math.Pow(1024d, (byte)unit)).ToString("n"), unit.ToString());
            statisticComponents.TextUpload.Content = string.Format("{0} {1}", (selecetedDate.upload / Math.Pow(1024d, (byte)unit)).ToString("n"), unit.ToString());
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string token = Settings.Token;
            deviceList = Client.LoadDevices(token);
            foreach (var device in deviceList)
            {
                device.usage = Client.LoadUsage(token, device.id);
            }
        }

        public void Start()
        {
            backgroundWorker.RunWorkerAsync();
        }
    }

    public class StatisticComponents
    {
        public Button BtnPrevious { get; set; }
        public Button BtnNext { get; set; }
        public GroupBox GroupBox { get; set; }
        public Label TextDownload { get; set; }
        public Label TextUpload { get; set; }
    }
}