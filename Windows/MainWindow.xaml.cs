using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Hardcodet.Wpf.TaskbarNotification;
using ComputerMonitorClient.Pages;

namespace ComputerMonitorClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainSwitch
    {
        private OptionsPage optionsPage;
        private MeasuringPage measuringPage;
        private StatisticPage statisticPage;
        private bool statedMinimized = false;

        public MainWindow()
        {
            InitializeComponent();

            trayIcon.TrayMouseDoubleClick += TrayIcon_TrayMouseDoubleClick;

            if (String.IsNullOrEmpty(Properties.Settings.Default[SettingFields.ADAPTER].ToString()))
            {
                this.SwitchToOptionPage();
            }
            else
            {
                this.SwitchToMeasuringPage();
            }
        }

        public MainWindow(bool isMinimized) : this()
        {
            if (isMinimized)
            {
                this.WindowState = WindowState.Minimized;
                this.statedMinimized = isMinimized;
            }
        }

        private void TrayIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        public void CloseApplication()
        {
            this.Close();
        }

        public void SwitchToOptionPage()
        {
            if (optionsPage == null)
            {
                optionsPage = new OptionsPage(this);
            }
            mainFrame.Navigate(optionsPage);
        }

        public void SwitchToMeasuringPage()
        {
            if (measuringPage == null)
            {
                measuringPage = new MeasuringPage(this);
            }
            mainFrame.Navigate(measuringPage);
        }

        public void SwitchToStatisticPage()
        {
            if (statisticPage == null)
            {
                statisticPage = new StatisticPage(this);
            }
            mainFrame.Navigate(statisticPage);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (statedMinimized)
            {
                this.ShowInTaskbar = false;
            }
        }
    }
}
