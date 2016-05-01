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
    /// Interaktionslogik für MeasuringPage.xaml
    /// </summary>
    public partial class MeasuringPage : Page, IMainSwitch
    {

        private IMainSwitch context;

        public MeasuringPage()
        {
            InitializeComponent();
        }

        public MeasuringPage(IMainSwitch context) : this()
        {
            this.context = context;
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            this.SwitchToOptionPage();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.CloseApplication();
        }

        public void CloseApplication()
        {
            context.CloseApplication();
        }

        public void SwitchToOptionPage()
        {
            context.SwitchToOptionPage();
        }

        public void SwitchToMeasuringPage()
        {
            // Useless
        }
    }
}
