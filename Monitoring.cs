using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using LiveCharts;

namespace ComputerMonitorClient
{
    public class Monitoring
    {
        private IEnumerable<ModelHolder> modelHolders;
        private Echevil.NetworkAdapter networkAdapter;
        private Echevil.NetworkMonitor networkMonitor;
        private Synchronizer synchronizer;
        private BackgroundWorker backgroudWorker;
        private SeriesCollection series;

        public Monitoring(IEnumerable<ModelHolder> modelHolders)
        {
            this.modelHolders = modelHolders;
            this.networkMonitor = new Echevil.NetworkMonitor();
            this.backgroudWorker = new BackgroundWorker();
            this.synchronizer = new Synchronizer();

            var todayData = synchronizer.TodayData();
            this.modelHolders.First(x => x.Typ == ModelUiTyp.TodayDownload).Value = todayData.First(x => x.Key == "download").Value;
            this.modelHolders.First(x => x.Typ == ModelUiTyp.TodayUpload).Value = todayData.First(x => x.Key == "upload").Value;
        }

        public Monitoring(IEnumerable<ModelHolder> modelHolders, SeriesCollection series) : this(modelHolders)
        {
            this.series = series;
        }

        public void StartMonitoring()
        {
            string adapter = Properties.Settings.Default["adapter"].ToString();
            if (!string.IsNullOrEmpty(adapter))
            {
                networkAdapter = this.networkMonitor.Adapters.FirstOrDefault(x => x.Name == adapter);
            }
            if (networkAdapter != null)
            {
                networkMonitor.StartMonitoring(networkAdapter);
                backgroudWorker.DoWork += new DoWorkEventHandler(MonitoringInBackgroud);
                backgroudWorker.ProgressChanged += new ProgressChangedEventHandler(MonitoringReport);
                backgroudWorker.WorkerReportsProgress = true;
                backgroudWorker.RunWorkerAsync();
            }
            else
            {
                throw new NotImplementedException("No adapter");
            }
        }

        private void MonitoringInBackgroud(object sender, DoWorkEventArgs e)
        {
            bool connectionProblem = true;
            while (connectionProblem)
            {
                try
                {
                    var totalData = synchronizer.LoadTotalData();
                    modelHolders.First(x => x.Typ == ModelUiTyp.TotalDownload).Value = totalData.First(x => x.Key == "download").Value;
                    modelHolders.First(x => x.Typ == ModelUiTyp.TotalUpload).Value = totalData.First(x => x.Key == "upload").Value;
                    connectionProblem = false;
                }
                catch (Exception)
                {
                    System.Threading.Thread.Sleep(3000);
                }
            }

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                double download = networkAdapter.DownloadSpeedKbps;
                double upload = networkAdapter.UploadSpeedKbps;

                foreach (ModelHolder model in modelHolders)
                {
                    switch (model.Typ)
                    {
                        case ModelUiTyp.CurrentDownload:
                            model.Value = download;
                            break;
                        case ModelUiTyp.CurrentUpload:
                            model.Value = upload;
                            break;
                        case ModelUiTyp.TodayDownload:
                        case ModelUiTyp.TotalDownload:
                            model.Value += download;
                            break;
                        case ModelUiTyp.TodayUpload:
                        case ModelUiTyp.TotalUpload:
                            model.Value += upload;
                            break;
                        default:
                            break;
                    }
                }
                if (DateTime.Today.Second % 5 == 0)
                {
                    synchronizer.SaveTodayDate(modelHolders.First(x => x.Typ == ModelUiTyp.TodayDownload).Value, modelHolders.First(x => x.Typ == ModelUiTyp.TodayUpload).Value);
                    var todayData = synchronizer.TodayData();
                    modelHolders.First(x => x.Typ == ModelUiTyp.TodayDownload).Value = todayData.First(x => x.Key == "download").Value;
                    modelHolders.First(x => x.Typ == ModelUiTyp.TodayUpload).Value = todayData.First(x => x.Key == "upload").Value;
                }
                backgroudWorker.ReportProgress(0);
            }
        }

        private void MonitoringReport(object sender, ProgressChangedEventArgs e)
        {
            Unit unit = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default["unit"].ToString());

            var models = modelHolders;
            var currentDownUp = models.Where(x => x.Typ == ModelUiTyp.CurrentDownload || x.Typ == ModelUiTyp.CurrentUpload);
            var otherDownUP = models.Where(x => x.Typ != ModelUiTyp.CurrentDownload && x.Typ != ModelUiTyp.CurrentUpload);

            DateTime now = DateTime.Now;
            foreach (var item in currentDownUp)
            {
                item.Label.Content = string.Format("{0} KB/s", item.Value.ToString("n"));
                if (item.Typ == ModelUiTyp.CurrentDownload)
                {
                    series[0].Values.Add(new UsageViewModel
                    {
                        Usage = item.Value,
                        Time = now
                    });
                }
                else
                {
                    series[1].Values.Add(new UsageViewModel
                    {
                        Usage = item.Value,
                        Time = now
                    });
                }
            }

            foreach (var item in otherDownUP)
            {
                item.Label.Content = string.Format("{0} {1}", (item.Value / Math.Pow(1024d, (byte)unit)).ToString("n"), unit.ToString());
            }

            if (series[0].Values.Count > 30 && series[1].Values.Count > 30)
            {
                series[0].Values.RemoveAt(0);
                series[1].Values.RemoveAt(0);
            }
        }
    }

    public class ModelHolder
    {
        private Label label;
        private ModelUiTyp typ;
        private double value;

        public double Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = Math.Abs(value);
            }
        }

        public Label Label
        {
            get
            {
                return label;
            }
        }

        public ModelUiTyp Typ
        {
            get
            {
                return typ;
            }
        }

        public ModelHolder(Label label, ModelUiTyp typ)
        {
            this.label = label;
            this.typ = typ;
        }
    }

    public enum ModelUiTyp
    {
        CurrentDownload,
        CurrentUpload,
        TodayDownload,
        TodayUpload,
        TotalDownload,
        TotalUpload
    }
}
