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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerMonitorClient
{
    /// <summary>
    /// Interaktionslogik für StartWindow.xaml
    /// </summary>
    public partial class StartWindow : Window, IStartSwitch
    {

        public StartWindow()
        {
            InitializeComponent();

//#if DEBUG
//            MainWindow mainWindow = new MainWindow();
//            mainWindow.Show();
//            this.Close();
//            //LoginPage debugLoginPage = new LoginPage();
//            //mainFrame.Navigate(debugLoginPage);
//#else

            if (String.IsNullOrEmpty(Properties.Settings.Default["token"].ToString()))
            {
                LoginPage loginPage = new LoginPage(this);
                mainFrame.Navigate(loginPage);
            }
            else if (Int32.Parse(Properties.Settings.Default["deviceId"].ToString()) < 0)
            {
                DevicePage devicePage = new DevicePage(this);
                mainFrame.Navigate(devicePage);
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }

//#endif
        }

        public void SwitchToMainWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
