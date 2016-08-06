﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ComputerMonitorClient
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            
            Window window;
            if (String.IsNullOrEmpty(ComputerMonitorClient.Properties.Settings.Default[SettingFields.TOKEN].ToString()))
            {
                window = new StartWindow(false);
            }
            else if (Int32.Parse(ComputerMonitorClient.Properties.Settings.Default[SettingFields.DEVICE_ID].ToString()) < 0)
            {
                window = new StartWindow(true);
            }
            else
            {
                if (e.Args.Length > 0)
                {
                    window = new MainWindow(true);
                }
                else
                {
                    window = new MainWindow(false);
                }
            }
            Application.Current.MainWindow = window;
            window.Show();
        }
    }
}
