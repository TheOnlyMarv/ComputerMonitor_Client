using LiveCharts;
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

namespace ComputerMonitorClient.Pages
{
    /// <summary>
    /// Interaktionslogik für StatisticPage.xaml
    /// </summary>
    public partial class StatisticPage : Page, IMainSwitch
    {
        private IMainSwitch context;
        public SeriesCollection SeriesThisDevice { get; set; }
        public SeriesCollection SeriesAllDevices { get; set; }
        private StatisticManager statisticManager;

        public StatisticPage(IMainSwitch context)
        {
            InitializeComponent();
            this.context = context;

            SeriesThisDevice = new SeriesCollection();
            SeriesAllDevices = new SeriesCollection();
            DataContext = this;
            statisticManager = new StatisticManager(SeriesThisDevice, SeriesAllDevices);
            statisticManager.Start();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SwitchToMeasuringPage();
        }

        public void CloseApplication()
        {
            // Useless
        }

        public void SwitchToOptionPage()
        {
            // Useless
        }

        public void SwitchToMeasuringPage()
        {
            context.SwitchToMeasuringPage();
        }

        public void SwitchToStatisticPage()
        {
            // Useless
        }
    }
}
