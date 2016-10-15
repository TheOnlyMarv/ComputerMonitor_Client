using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerMonitorClient
{
    public static class Settings
    {
        private static string SettingsSaveFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\.ComputerMonitor";
        private static string SettingsSaveFileName = "settings.ini";

        public static string Token { get; set; }
        public static int DeviceId { get; set; }
        public static string DeviceName { get; set; }
        public static int Unit { get; set; }
        public static string Adapter { get; set; }

        public static double NewUpload { get; set; }
        public static double NewDownload { get; set; }
        public static DateTime NewDate { get; set; }

        public static double OldUpload { get; set; }
        public static double OldDownload { get; set; }
        public static DateTime? OldDate { get; set; }
        public static bool Synchronized { get; set; }

        private static Dictionary<string, object> SettingsMap = new Dictionary<string, object>();

        public static void SaveSettings()
        {
            using (RegistryKey key = Registry.CurrentUser.CreateSubKey("Software\\ComputerMonitor"))
            {
                SaveKeyValues(key);
            }

        }

        public static bool LoadSettings()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\ComputerMonitor", false))
            {
                if (key != null)
                {
                    ReadKeyValues(key);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private static void SaveKeyValues(RegistryKey key)
        {
            key.SetValue("Token", Token ?? "");
            key.SetValue("DeviceId", DeviceId);
            key.SetValue("DeviceName", DeviceName ?? "");
            key.SetValue("Unit", Unit);
            key.SetValue("Adapter", Adapter ?? "");

            key.SetValue("NewUpload", NewUpload);
            key.SetValue("NewDownload", NewDownload);
            key.SetValue("NewDate", NewDate);

            key.SetValue("OldUpload", OldUpload );
            key.SetValue("OldDownload", OldDownload);
            key.SetValue("OldDate", OldDate ?? DateTime.Now.AddDays(-1));
            key.SetValue("Synchronized", Synchronized);
        }

        private static void ReadKeyValues(RegistryKey key)
        {
            foreach (var valueName in key.GetValueNames())
            {
                object o = key.GetValue(valueName);
                if (o != null)
                {
                    switch (valueName)
                    {
                        case "Token":
                            Token = o.ToString();
                            break;
                        case "DeviceId":
                            DeviceId = int.Parse(o.ToString());
                            break;
                        case "DeviceName":
                            DeviceName = o.ToString();
                            break;
                        case "Unit":
                            Unit = int.Parse(o.ToString());
                            break;
                        case "Adapter":
                            Adapter = o.ToString();
                            break;

                        case "NewUpload":
                            NewUpload = double.Parse(o.ToString());
                            break;
                        case "NewDownload":
                            NewDownload = double.Parse(o.ToString());
                            break;
                        case "NewDate":
                            NewDate = DateTime.Parse(o.ToString());
                            break;

                        case "OldUpload":
                            OldUpload = double.Parse(o.ToString());
                            break;
                        case "OldDownload":
                            OldDownload = double.Parse(o.ToString());
                            break;
                        case "OldDate":
                            OldDate = DateTime.Parse(o.ToString());
                            break;
                        case "Synchronized":
                            Synchronized = bool.Parse(o.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }

    }

}
