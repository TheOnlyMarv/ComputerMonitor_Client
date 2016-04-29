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
    public partial class StartWindow : Window
    {

        public StartWindow()
        {
            InitializeComponent();

#if DEBUG
            LoginPage debugLoginPage = new LoginPage();
            mainFrame.Navigate(debugLoginPage);
#else

            if (String.IsNullOrEmpty(Properties.Settings.Default["token"].ToString()))
            {
                LoginPage loginPage = new LoginPage();
                mainFrame.Navigate(loginPage);
            }
            else if (Int32.Parse(Properties.Settings.Default["deviceId"].ToString()) < 0)
            {
                DevicePage devicePage = new DevicePage();
                mainFrame.Navigate(devicePage);
            }
            else
            {
                this.Close();
            }

#endif
        }
    }
}
