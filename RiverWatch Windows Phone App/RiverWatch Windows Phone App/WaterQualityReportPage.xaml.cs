using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WaterQualityReportPage : Page
    {
        public WaterQualityReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(PollutionReportPage));
                e.Handled = true;
            }
        }

        BluetoothLEDevice currentDevice { get; set; }
        string deviceName = "Philips AEA1000";
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(BluetoothLEDevice.GetDeviceSelector()))
            {
                BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(di.Id);
                if (bleDevice.Name == deviceName)
                {
                    currentDevice = bleDevice;
                    break;
                }
            }

            List<string> serviceList = new List<string>();
            foreach (var service in currentDevice.GattServices)
            {
                switch (service.Uuid.ToString())
                {
                    case "00001811-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("AlertNotification");
                        break;
                    case "0000180f-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("Battery");
                        break;
                    case "00001810-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("BloodPressure");
                        break;
                    case "00001805-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CurrentTime");
                        break;
                    case "00001818-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CyclingPower");
                        break;
                    case "00001816-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("CyclingSpeedAndCadence");
                        break;
                    case "0000180a-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("DeviceInformation");
                        break;
                    case "00001800-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("GenericAccess");
                        break;
                    case "00001801-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("GenericAttribute");
                        break;
                    case "00001808-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("Glucose");
                        break;
                    case "00001809-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HealthThermometer");
                        break;
                    case "0000180d-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HeartRate");
                        break;
                    case "00001812-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("HumanInterfaceDevice");
                        break;
                    case "00001802-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ImmediateAlert");
                        break;
                    case "00001803-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("LinkLoss");
                        break;
                    case "00001819-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("LocationAndNavigation");
                        break;
                    case "00001807-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("NextDstChange");
                        break;
                    case "0000180e-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("PhoneAlertStatus");
                        break;
                    case "00001806-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ReferenceTimeUpdate");
                        break;
                    case "00001814-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("RunningSpeedAndCadence");
                        break;
                    case "00001813-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("ScanParameters");
                        break;
                    case "00001804-0000-1000-8000-00805f9b34fb":
                        serviceList.Add("TxPower");
                        break;
                    default:
                        break;
                }
            }
            MessageDialog md = new MessageDialog(String.Join("\r\n", serviceList));
            md.ShowAsync();

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
        }
    }
}
