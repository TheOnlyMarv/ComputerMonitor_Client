using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaktionslogik für OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page, IMainSwitch
    {

        private Echevil.NetworkAdapter[] adapters = new Echevil.NetworkMonitor().Adapters;
        private IMainSwitch context;

        public OptionsPage()
        {
            InitializeComponent();
            this.DataContext = this;

            comboUnit.ItemsSource = Enum.GetValues(typeof(Unit)).Cast<Unit>();
            comboNetworkAdapter.ItemsSource = adapters;
        }

        public OptionsPage(IMainSwitch context) : this()
        {
            this.context = context;
        }

        private void comboUnit_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            comboUnit.SelectedItem = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default["unit"].ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["unit"] = (byte)(Unit)comboUnit.SelectedItem;
            Properties.Settings.Default["adapter"] = ((Echevil.NetworkAdapter)comboNetworkAdapter.SelectedItem).Name;
            Properties.Settings.Default.Save();
            this.SwitchToMeasuringPage();
        }

        private void comboNetworkAdapter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string adapter = Properties.Settings.Default["adapter"].ToString();
            comboNetworkAdapter.SelectedItem = adapters.FirstOrDefault(x => x.Name == adapter);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.SwitchToMeasuringPage();
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
    }
}
