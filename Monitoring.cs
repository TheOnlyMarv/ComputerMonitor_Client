using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ComputerMonitorClient
{
    public class Monitoring
    {
        private IEnumerable<ModelHolder> modelHolders;
        private Echevil.NetworkAdapter networkAdapter;
        private Echevil.NetworkMonitor networkMonitor;
        private Synchronizer synchronizer;
        private BackgroundWorker backgroudWorker;

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
                    this.modelHolders.First(x => x.Typ == ModelUiTyp.TodayDownload).Value = todayData.First(x => x.Key == "download").Value;
                    this.modelHolders.First(x => x.Typ == ModelUiTyp.TodayUpload).Value = todayData.First(x => x.Key == "upload").Value;
                }
                backgroudWorker.ReportProgress(0);
            }
        }

        private void MonitoringReport(object sender, ProgressChangedEventArgs e)
        {
            Unit unit = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default["unit"].ToString());
            foreach (ModelHolder model in modelHolders)
            {
                if (model.Typ == ModelUiTyp.CurrentDownload || model.Typ == ModelUiTyp.CurrentUpload)
                {
                    model.Label.Content = string.Format("{0} KB/s", model.Value.ToString("n"));
                }
                else
                {
                    switch (unit)
                    {
                        case Unit.KB:
                            model.Label.Content = string.Format("{0} KB", (model.Value).ToString("n"));
                            break;
                        case Unit.MB:
                            model.Label.Content = string.Format("{0} MB", (model.Value / 1024d).ToString("n"));
                            break;
                        case Unit.GB:
                            model.Label.Content = string.Format("{0} GB", (model.Value / 1024d / 1024d).ToString("n"));
                            break;
                        default:
                            break;
                    }
                }
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
                return this.value;
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
                return this.label;
            }
        }

        public ModelUiTyp Typ
        {
            get
            {
                return this.typ;
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
