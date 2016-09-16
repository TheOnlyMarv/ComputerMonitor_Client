using ComputerMonitorClient.WebSocket;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
            checkStartup.IsChecked = IsOnStartUp();

            createQrCode();
        }

        private void createQrCode()
        {
            QRCode qrCode = WebServerHoldings.getInstance().GetQrCode();

            var bitmap = qrCode.GetGraphic(50);
            qrImage.Source = Utilities.convertBitmapToBitmapSource(bitmap);
        }

        public OptionsPage(IMainSwitch context) : this()
        {
            this.context = context;
        }

        private void comboUnit_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            comboUnit.SelectedItem = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default[SettingFields.UNIT].ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default[SettingFields.UNIT] = (byte)(Unit)comboUnit.SelectedItem;
            Properties.Settings.Default[SettingFields.ADAPTER] = ((Echevil.NetworkAdapter)comboNetworkAdapter.SelectedItem).Name;
            Properties.Settings.Default.Save();
            StartUp((bool)checkStartup.IsChecked);

            this.SwitchToMeasuringPage();
        }


        private void comboNetworkAdapter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string adapter = Properties.Settings.Default[SettingFields.ADAPTER].ToString();
            comboNetworkAdapter.SelectedItem = adapters.FirstOrDefault(x => x.Name == adapter);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            SettingsReset();
            this.SwitchToMeasuringPage();
        }

        private void SettingsReset()
        {
            checkStartup.IsChecked = IsOnStartUp();
            string adapter = Properties.Settings.Default[SettingFields.ADAPTER].ToString();
            comboNetworkAdapter.SelectedItem = adapters.FirstOrDefault(x => x.Name == adapter);
            comboUnit.SelectedItem = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default[SettingFields.UNIT].ToString());
        }

        private void StartUp(bool addOnStartup)
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                if (addOnStartup)
                {
                    key.SetValue(curAssembly.GetName().Name, curAssembly.Location + " start");
                }
                else
                {
                    key.DeleteValue(curAssembly.GetName().Name);
                }
            }
            catch { }
        }

        private bool IsOnStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                return key.GetValue(curAssembly.GetName().Name) != null;
            }
            catch
            {
                return false;
            }
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

        private void btnCancel_Copy_Click(object sender, RoutedEventArgs e)
        {
            WebSocketSettings.WsServer.SendMessage("Testnachricht!");
        }
    }
}
