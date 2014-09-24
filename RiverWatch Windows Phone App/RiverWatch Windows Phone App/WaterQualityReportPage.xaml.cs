using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Proximity;
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
            
        }

        

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
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


        //Windows.Devices.Bluetooth.RfcommDeviceService _service;
        //Windows.Networking.Sockets.StreamSocket _socket;

        async void Initialize()
        {
            // Enumerate devices with the object push service
            var services =
                await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync();
        }

        bool _started = false;

        private async void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            this.thingsFound.Text = ">>> Finding bluetooth people\n";

            Windows.Networking.Proximity.PeerFinder.AllowBluetooth = true;

            Windows.Networking.Proximity.PeerFinder.Start();
            _started = true;

            var peers = await PeerFinder.FindAllPeersAsync();
           
            for (int i = 0; i < peers.Count; i++)
            {
                ConnectToPeer(peers.ElementAt(i));
            }
            

            /*
            String selector = BluetoothDevice.GetDeviceSelector();
            var devices = await DeviceInformation.FindAllAsync(selector);

            BluetoothDevice dev;

            for (int i = 0; i < devices.Count; i++)
            {
                dev = await BluetoothDevice.FromIdAsync(devices.ElementAt(i).Id);
                this.thingsFound.Text += dev.Name + "\n";
            }
             * */

            this.thingsFound.Text += "Done";
        }

        async private void ConnectToPeer(Windows.Networking.Proximity.PeerInformation peerInfo)
        {
            //WriteMessageText("Peer found. Connecting to " + peerInfo.DisplayName + "\n");
            try
            {
                Windows.Networking.Sockets.StreamSocket socket =
                    await Windows.Networking.Proximity.PeerFinder.ConnectAsync(peerInfo);

                //WriteMessageText("Connection successful. You may now send messages.\n");
                this.thingsFound.Text += "Connection successful. You may now send messages.";
                //SendMessage(socket);
            }
            catch (Exception err)
            {
                //WriteMessageText("Connection failed: " + err.Message + "\n");
            }

            //requestingPeer = null;
        }
    }
}
