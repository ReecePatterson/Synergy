using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
using Windows.Networking.Sockets;
using Windows.Phone.UI.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WaterQualityReportPage : Page {
        public WaterQualityReportPage() {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

        }


        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e) {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack) {
                rootFrame.Navigate(typeof(PollutionReportPage));
                e.Handled = true;
            }
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e) {

        }


        private void ReturnButton_Click(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(HubPage));
        }


        private async void ScanDevices() {
            lstDevices.Items.Clear();
            string genericUUID = GattDeviceService.GetDeviceSelectorFromUuid(GattServiceUuids.GenericAccess);

            foreach (DeviceInformation di in await DeviceInformation.FindAllAsync(genericUUID)) {
                BluetoothLEDevice bleDevice = await BluetoothLEDevice.FromIdAsync(di.Id);
                lstDevices.Items.Add(new MyBluetoothLEDevice(bleDevice));
            }

        }

        BluetoothLEDevice currentDevice { get; set; }

        private void lstDevices_Tapped(object sender, TappedRoutedEventArgs e) {
            currentDevice = (lstDevices.SelectedItem as MyBluetoothLEDevice).BluetoothLEDevice;
            LoadGattServices();
        }

        private void LoadGattServices() {
            lstGattServices.Items.Clear();
            string description = string.Empty;

            foreach (var service in currentDevice.GattServices) {
                description = UUIDHelper.GetUUIDDescription(service.Uuid.ToString());
                lstGattServices.Items.Add(new MyGattService(description, service));
            }

        }

        private async void lstGattServices_Tapped(object sender, TappedRoutedEventArgs e) {
            lstGattServices.Items.Clear();
            string description = string.Empty;

            foreach (var service in currentDevice.GattServices) {
                description = UUIDHelper.GetUUIDDescription(service.Uuid.ToString());
                lstGattServices.Items.Add(new MyGattService(description, service));
            }
        }

        private async void lstCharacteristics_Tapped(object sender, TappedRoutedEventArgs e) {
            lstCharacteristics.Items.Clear();
            GattDeviceService selectedService = (lstGattServices.SelectedItem as MyGattService).GattDeviceService;

            foreach (var characteristic in selectedService.GetAllCharacteristics()) {
                lstCharacteristics.Items.Add(characteristic.UserDescription + " " + characteristic.Uuid);
            }
        }

    }

    public class MyBluetoothLEDevice {
        public BluetoothLEDevice BluetoothLEDevice { get; private set; }

        public MyBluetoothLEDevice(BluetoothLEDevice gattDeviceService) {
            this.BluetoothLEDevice = gattDeviceService;
        }

        public override string ToString() {
            return string.Format("{0} ({1})", this.BluetoothLEDevice.Name, this.BluetoothLEDevice.ConnectionStatus.ToString());
        }
    }

    public class MyGattService {
        public GattDeviceService GattDeviceService { get; private set; }
        public string Description { get; private set; }
        public MyGattService(string description, GattDeviceService gattDeviceService) {
            this.Description = description;
            this.GattDeviceService = gattDeviceService;
        }

        public override string ToString() {
            return Description;
        }
    }

    public static class UUIDHelper {
        public static string GetUUIDDescription(string uuid) {
            switch (uuid) {
                case "00001811-0000-1000-8000-00805f9b34fb":
                    return "AlertNotification";
                case "0000180f-0000-1000-8000-00805f9b34fb":
                    return "Battery";
                case "00001810-0000-1000-8000-00805f9b34fb":
                    return "BloodPressure";
                case "00001805-0000-1000-8000-00805f9b34fb":
                    return "CurrentTime";
                case "00001818-0000-1000-8000-00805f9b34fb":
                    return "CyclingPower";
                case "00001816-0000-1000-8000-00805f9b34fb":
                    return "CyclingSpeedAndCadence";
                case "0000180a-0000-1000-8000-00805f9b34fb":
                    return "DeviceInformation";
                case "00001800-0000-1000-8000-00805f9b34fb":
                    return "GenericAccess";
                case "00001801-0000-1000-8000-00805f9b34fb":
                    return "GenericAttribute";
                case "00001808-0000-1000-8000-00805f9b34fb":
                    return "Glucose";
                case "00001809-0000-1000-8000-00805f9b34fb":
                    return "HealthThermometer";
                case "0000180d-0000-1000-8000-00805f9b34fb":
                    return "HeartRate";
                case "00001812-0000-1000-8000-00805f9b34fb":
                    return "HumanInterfaceDevice";
                case "00001802-0000-1000-8000-00805f9b34fb":
                    return "ImmediateAlert";
                case "00001803-0000-1000-8000-00805f9b34fb":
                    return "LinkLoss";
                case "00001819-0000-1000-8000-00805f9b34fb":
                    return "LocationAndNavigation";
                case "00001807-0000-1000-8000-00805f9b34fb":
                    return "NextDstChange";
                case "0000180e-0000-1000-8000-00805f9b34fb":
                    return "PhoneAlertStatus";
                case "00001806-0000-1000-8000-00805f9b34fb":
                    return "ReferenceTimeUpdate";
                case "00001814-0000-1000-8000-00805f9b34fb":
                    return "RunningSpeedAndCadence";
                case "00001813-0000-1000-8000-00805f9b34fb":
                    return "ScanParameters";
                case "00001804-0000-1000-8000-00805f9b34fb":
                    return "TxPower";
                default:
                    return "Unknown " + uuid;
            }
        }
    }
}
