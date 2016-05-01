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

namespace ComputerMonitorClient
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainSwitch
    {
        private OptionsPage optionsPage;
        private MeasuringPage measuringPage;

        public MainWindow()
        {
            InitializeComponent();

            if (String.IsNullOrEmpty(Properties.Settings.Default["adapter"].ToString()))
            {
                this.SwitchToOptionPage();
            }
            else
            {
                this.SwitchToMeasuringPage();
            }
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
    }
}
