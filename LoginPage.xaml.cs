using ComputerMonitorClient.RemoteClasses;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerMonitorClient
{
    /// <summary>
    /// Interaktionslogik für LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Status status = Client.Login(textUsername.Text, boxPassword.Password);
            if (status.status)
            {
                Properties.Settings.Default["token"] = status.token;
                Properties.Settings.Default.Save();

                DevicePage devicePage = new DevicePage();
                this.NavigationService.Navigate(devicePage);
            }
            else
            {
                MessageBox.Show(status.message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
            

        private void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            Status status = Client.Register(textUsername.Text, boxPassword.Password);
            if (status.status)
            {
                btnLogin_Click(sender, e);
            }
            else
            {
                MessageBox.Show(status.message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
