using System.Linq;
using LiveCharts;
using System.ComponentModel;
using ComputerMonitorClient.RemoteClasses;
using System.Collections.Generic;

namespace ComputerMonitorClient
{
    public class StatisticManager
    {
        private SeriesCollection seriesAllDevices;
        private SeriesCollection seriesThisDevice;
        private BackgroundWorker backgroundWorker;
        private IList<Device> deviceList;

        public StatisticManager(SeriesCollection seriesThisDevice, SeriesCollection seriesAllDevices)
        {
            this.seriesThisDevice = seriesThisDevice;
            this.seriesAllDevices = seriesAllDevices;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int deviceId = (int)Properties.Settings.Default[Utilities.DEVICE_ID];
            Device thisDevice = deviceList.FirstOrDefault(x => x.id == deviceId);
            var weekStatistic = thisDevice.usage.OrderByDescending(x => x.date).Take(7);

            seriesThisDevice.Add(new PieSeries()
            {
                Title = "Download",
                Values = new ChartValues<double>() {
                    weekStatistic.Select(x=>x.download).Sum()
                },
                StrokeThickness = 0.3,
            });
            seriesThisDevice.Add(new PieSeries()
            {
                Title = "Upload",
                Values = new ChartValues<double>() {
                    weekStatistic.Select(x=>x.upload).Sum()
                },
                StrokeThickness = 0.3,
                
            });
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string token = Properties.Settings.Default[Utilities.TOKEN].ToString();
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
}