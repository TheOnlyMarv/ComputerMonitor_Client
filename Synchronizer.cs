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
        private double newDownload;
        private double newUpload;
        private DateTime newDate;

        private double oldDownload;
        private double oldUpload;
        private DateTime oldDate;
        private bool oldSynchronized;

        private BackgroundWorker backgroundSynchronizer;

        public Synchronizer()
        {
            LoadFromSettings();

            if (newDate == default(DateTime))
            {
                oldSynchronized = true;
            }
            else if (newDate.Date != DateTime.Today.Date)
            {
                oldDownload = newDownload;
                oldUpload = newUpload;
                oldDate = newDate;
                oldSynchronized = false;

                newDownload = 0d;
                newUpload = 0d;
                newDate = DateTime.Today.Date;

                SaveInSettings();
            }

            StartSynchronizing();
        }

        public IDictionary<String, Double> TodayData()
        {
            IDictionary<String, Double> result = new Dictionary<String, Double>();
            result.Add(new KeyValuePair<String, Double>("download", newDownload));
            result.Add(new KeyValuePair<String, Double>("upload", newUpload));
            return result;
        }

        public void SaveTodayDate(double download, double upload)
        {
            if (newDate == default(DateTime) || newDate.Date == DateTime.Today.Date)
            {
                newDate = DateTime.Today.Date;
                newDownload = download;
                newUpload = upload;
            }
            else
            {
                oldDate = newDate;
                oldDownload = newDownload;
                oldUpload = newUpload;
                oldSynchronized = false;

                newDate = DateTime.Today.Date;
                newDownload = 0;
                newUpload = 0;
            }
            SaveInSettings();
            StartSynchronizing();
        }

        private void LoadFromSettings()
        {
            this.newDownload = (double)Properties.Settings.Default["newdownload"];
            this.newUpload = (double)Properties.Settings.Default["newupload"];
            this.newDate = (DateTime)Properties.Settings.Default["newdate"];

            this.oldDownload = (double)Properties.Settings.Default["olddownload"];
            this.oldUpload = (double)Properties.Settings.Default["oldupload"];
            this.oldDate = (DateTime)Properties.Settings.Default["olddate"];
            this.oldSynchronized = (bool)Properties.Settings.Default["oldsynchronized"];
        }

        private void SaveInSettings()
        {
            Properties.Settings.Default["newdownload"] = this.newDownload;
            Properties.Settings.Default["newupload"] = this.newUpload;
            Properties.Settings.Default["newdate"] = this.newDate.Date;

            Properties.Settings.Default["olddownload"] = this.oldDownload;
            Properties.Settings.Default["oldupload"] = this.oldUpload;
            Properties.Settings.Default["olddate"] = this.oldDate.Date;
            Properties.Settings.Default["oldsynchronized"] = this.oldSynchronized;
            Properties.Settings.Default.Save();
        }

        public void StartSynchronizing()
        {
            if (!oldSynchronized)
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
                    status = Client.AddUsage(token, deviceId, oldDownload, oldUpload, oldDate);
                    oldSynchronized = status.status;
                    error = false;
                }
                catch (Exception ex )
                {
                    error = true;
                    System.Threading.Thread.Sleep(5000);
                }

            } while (error);
        }

        private void SynchronizeComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            SaveInSettings();
            if (!oldSynchronized)
            {
                // TODO Token or deviceId is invalid
                throw new NotImplementedException("Token or deviceId invalid");
            }
        }
    }
}
