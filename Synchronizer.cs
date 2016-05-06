using ComputerMonitorClient.RemoteClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerMonitorClient
{
    public class Synchronizer
    {
        private SynchronizerData newData = new SynchronizerData();
        private SynchronizerData oldData = new SynchronizerData();

        private BackgroundWorker backgroundSynchronizer;

        public Synchronizer()
        {
            LoadFromSettings();

            if (newData.Date == default(DateTime))
            {
                oldData.Synchronized = true;
            }
            else if (newData.Date != DateTime.Today.Date)
            {
                oldData = newData.CopyWithoutSynch();
                oldData.Synchronized = false;

                newData.ResetData();

                SaveInSettings();
            }

            StartSynchronizing();
        }

        public IDictionary<String, Double> TodayData()
        {
            IDictionary<String, Double> result = new Dictionary<String, Double>();
            result.Add(new KeyValuePair<String, Double>("download", newData.Download));
            result.Add(new KeyValuePair<String, Double>("upload", newData.Upload));
            return result;
        }

        public void SaveTodayDate(double download, double upload)
        {
            if (newData.Date == default(DateTime) || newData.Date == DateTime.Today.Date)
            {
                newData.Date = DateTime.Today.Date;
                newData.Download = download;
                newData.Upload = upload;
            }
            else
            {
                oldData = newData.CopyWithoutSynch();
                oldData.Synchronized = false;

                newData.ResetData();
            }
            SaveInSettings();
            StartSynchronizing();
        }

        public IDictionary<String, Double> LoadTotalData()
        {
            string token = Properties.Settings.Default["token"].ToString();
            int deviceId = (int)Properties.Settings.Default["deviceId"];

            double download = newData.Download;
            double upload = newData.Upload;

            List<Usage> usageList = Client.LoadUsage(token, deviceId);
            if (usageList != null)
            {
                download += usageList.Sum(x => x.download);
                upload += usageList.Sum(x => x.upload);
            }

            IDictionary<String, Double> result = new Dictionary<String, Double>();
            result.Add(new KeyValuePair<String, Double>("download", download));
            result.Add(new KeyValuePair<String, Double>("upload", upload));
            return result;
        }

        private void LoadFromSettings()
        {
            newData.LoadAsNewData();
            oldData.LoadAsOldData();
        }

        private void SaveInSettings()
        {
            newData.SaveAsNewData();
            oldData.SaveAsOldData();
        }

        public void StartSynchronizing()
        {
            if (!oldData.Synchronized)
            {
                backgroundSynchronizer = new BackgroundWorker();
                backgroundSynchronizer.DoWork += new DoWorkEventHandler(SynchronizeBackground);
                backgroundSynchronizer.RunWorkerCompleted += new RunWorkerCompletedEventHandler(SynchronizeComplete);
                backgroundSynchronizer.RunWorkerAsync();
            }
        }

        private void SynchronizeBackground(object sender, DoWorkEventArgs e)
        {
            string token = Properties.Settings.Default["token"].ToString();
            int deviceId = (int)Properties.Settings.Default["deviceId"];
            bool error = false;

            do
            {
                try
                {
                    Status status;
                    status = Client.AddUsage(token, deviceId, oldData.Download, oldData.Upload, oldData.Date);
                    oldData.Synchronized = status.status;
                    error = false;
                }
                catch (Exception)
                {
                    error = true;
                    System.Threading.Thread.Sleep(5000);
                }

            } while (error);
        }

        private void SynchronizeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveInSettings();
            if (!oldData.Synchronized)
            {
                // TODO Token or deviceId is invalid
                throw new NotImplementedException("Token or deviceId invalid");
            }
        }
    }

    public class SynchronizerData
    {
        public double Download { get; set; }
        public double Upload { get; set; }
        public DateTime Date { get; set; }
        public bool Synchronized { get; set; }

        public SynchronizerData CopyWithoutSynch()
        {
            return new SynchronizerData()
            {
                Download = Download,
                Upload = Upload,
                Date = new DateTime(Date.Ticks)
            };
        }
        public void ResetData()
        {
            Download = 0d;
            Upload = 0d;
            Date = DateTime.Today.Date;
        }
        public void SaveAsNewData()
        {
            Properties.Settings.Default["newdownload"] = Download;
            Properties.Settings.Default["newupload"] = Upload;
            Properties.Settings.Default["newdate"] = Date.Date;
            Properties.Settings.Default.Save();
        }
        public void SaveAsOldData()
        {
            Properties.Settings.Default["olddownload"] = Download;
            Properties.Settings.Default["oldupload"] = Upload;
            Properties.Settings.Default["olddate"] = Date.Date;
            Properties.Settings.Default["oldsynchronized"] = Synchronized;
            Properties.Settings.Default.Save();
        }
        public void LoadAsNewData()
        {
            Download = (double)Properties.Settings.Default["newdownload"];
            Upload = (double)Properties.Settings.Default["newupload"];
            Date = (DateTime)Properties.Settings.Default["newdate"];
        }

        public void LoadAsOldData()
        {
            Download = (double)Properties.Settings.Default["olddownload"];
            Upload = (double)Properties.Settings.Default["oldupload"];
            Date = (DateTime)Properties.Settings.Default["olddate"];
            Synchronized = (bool)Properties.Settings.Default["oldsynchronized"];
        }
    }
}
