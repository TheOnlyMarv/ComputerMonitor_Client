using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
            checkStartup.IsChecked = IsOnStartUp();

            createQrCode();
        }

        private void createQrCode()
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrData = qrGenerator.CreateQrCode("HIER STEHT EINE IP", QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrData);

            var bitmap = qrCode.GetGraphic(20);
            qrImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions()
                );
            //qrImage.Source = ;//BitmapSource.Create(bitmap.Width, bitmap.Height, bitmap.VerticalResolution, bitmap.HorizontalResolution, PixelFormats.Bgr32, null,)
        }

        public OptionsPage(IMainSwitch context) : this()
        {
            this.context = context;
        }

        private void comboUnit_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            comboUnit.SelectedItem = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default[Utilities.UNIT].ToString());
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default[Utilities.UNIT] = (byte)(Unit)comboUnit.SelectedItem;
            Properties.Settings.Default[Utilities.ADAPTER] = ((Echevil.NetworkAdapter)comboNetworkAdapter.SelectedItem).Name;
            Properties.Settings.Default.Save();
            StartUp((bool)checkStartup.IsChecked);

            this.SwitchToMeasuringPage();
        }


        private void comboNetworkAdapter_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            string adapter = Properties.Settings.Default[Utilities.ADAPTER].ToString();
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
            string adapter = Properties.Settings.Default[Utilities.ADAPTER].ToString();
            comboNetworkAdapter.SelectedItem = adapters.FirstOrDefault(x => x.Name == adapter);
            comboUnit.SelectedItem = (Unit)Enum.Parse(typeof(Unit), Properties.Settings.Default[Utilities.UNIT].ToString());
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
    }
}
