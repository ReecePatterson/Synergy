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

        ObservableCollection<PeerInformation> _pairedDevices;  // a local copy of paired device information
        StreamSocket _socket; // socket object used to communicate with the device

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

        BluetoothLEDevice currentDevice { get; set; }
        string deviceName = "SGS4";
        protected async override void OnNavigatedTo(NavigationEventArgs e) {
            _pairedDevices = new ObservableCollection<PeerInformation>();
            PairedDevicesList.ItemsSource = _pairedDevices;

            RefreshPairedDevicesList();
        }

        private async Task<Boolean> connectToPoos() {
            PeerFinder.AllowBluetooth = true;
            PeerFinder.Start();

            PeerFinder.AlternateIdentities["Bluetooth:PAIRED"] = "";
            var available_devices = await PeerFinder.FindAllPeersAsync();
            PeerInformation pi = null;
            if (available_devices.Count == 0) {
                return false;
            }
            else {
                for (int i = 0; i < available_devices.Count; i++) {
                    pi = available_devices[i];
                    this.thingsFound.Text = "\nPaired Device Name: " + pi.DisplayName;
                    if (pi.DisplayName.Contains("Emmanuel")) {
                        this.thingsFound.Text += "\nEmans device found";
                        StreamSocket socket = new StreamSocket();

                        if (socket != null) {
                            socket.Dispose();
                        }

                        this.thingsFound.Text += "\nSocket Created with hostname: " + pi.HostName + " \nwith service name: " + pi.ServiceName;
                        //await socket.ConnectAsync(new End);
                        await socket.ConnectAsync(pi.HostName, "2");
                        this.thingsFound.Text += "\nDevice Connected";
                        return true;
                    }
                }

            }
            return false;

        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(HubPage));
        }


        private async void ConnectButton_Click(object sender, RoutedEventArgs e) {
            //RefreshPairedDevicesList();

        }

        private async void RefreshPairedDevicesList() {
            try {
                // Search for all paired devices
                PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                var peers = await PeerFinder.FindAllPeersAsync();

                // By clearing the backing data, we are effectively clearing the ListBox
                _pairedDevices.Clear();

                if (peers.Count == 0) {
                    //MessageBox.Show(AppResources.Msg_NoPairedDevices);
                }
                else {
                    // Found paired devices.
                    foreach (var peer in peers) {
                        _pairedDevices.Add(peer);
                    }
                }
            }
            catch (Exception ex) {
                if ((uint)ex.HResult == 0x8007048F) {
                    //var result = MessageBox.Show(AppResources.Msg_BluetoothOff, "Bluetooth Off", MessageBoxButton.OKCancel);
                    //if (result == MessageBoxResult.OK) {
                    //    ShowBluetoothcControlPanel();
                    //}
                }
                else if ((uint)ex.HResult == 0x80070005) {
                    //MessageBox.Show(AppResources.Msg_MissingCaps);
                }
                else {
                    //MessageBox.Show(ex.Message);
                }
            }
        }

        async private void ConnectToPeer(Windows.Networking.Proximity.PeerInformation peerInfo) {
            //WriteMessageText("Peer found. Connecting to " + peerInfo.DisplayName + "\n");
            try {
                Windows.Networking.Sockets.StreamSocket socket =
                    await Windows.Networking.Proximity.PeerFinder.ConnectAsync(peerInfo);

                //WriteMessageText("Connection successful. You may now send messages.\n");
                this.thingsFound.Text += "Connection successful. You may now send messages.";
                //SendMessage(socket);
            }
            catch (Exception err) {
                //WriteMessageText("Connection failed: " + err.Message + "\n");
            }
        }

        private void doSomething_Click(object sender, RoutedEventArgs e) {
            // Because I enable the ConnectToSelected button only if the user has
            // a device selected, I don't need to check here whether that is the case.

            // Connect to the device
            PeerInformation pdi = _pairedDevices[0];

            // Asynchronous call to connect to the device
            ConnectToDevice(pdi);
        }

        private async void ConnectToDevice(PeerInformation peer) {
            if (_socket != null) {
                // Disposing the socket with close it and release all resources associated with the socket
                _socket.Dispose();
            }

            try {
                _socket = new StreamSocket();
                //string serviceName = (String.IsNullOrWhiteSpace(peer.ServiceName)) ? "2" : peer.ServiceName;
                string serviceName = "3";

                Debug.WriteLine("before first await");

                // Note: If either parameter is null or empty, the call will throw an exception
                await _socket.ConnectAsync(peer.HostName, serviceName, SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                MessageDialog messageDialog = new MessageDialog(_socket.Information.RemoteAddress.DisplayName);

                Debug.WriteLine("before");
                await messageDialog.ShowAsync();
                //MessageBox.Show(String.Format(AppResources.Msg_ConnectedTo, _socket.Information.RemoteAddress.DisplayName));
                // If the connection was successful, the RemoteAddress field will be populated
                //MessageBox.Show(String.Format(AppResources.Msg_ConnectedTo, _socket.Information.RemoteAddress.DisplayName));
                Debug.WriteLine("poopies hurray");
            }
            catch (Exception ex) {
                // In a real app, you would want to take action dependent on the type of 
                // exception that occurred.
                //MessageBox.Show(ex.Message);

                _socket.Dispose();
                _socket = null;
            }
        }
    }

    /// <summary>
    ///  Class to hold all paired device information
    /// </summary>
    public class PairedDeviceInfo {
        internal PairedDeviceInfo(PeerInformation peerInformation) {
            this.PeerInfo = peerInformation;
            this.DisplayName = this.PeerInfo.DisplayName;
            this.HostName = this.PeerInfo.HostName.DisplayName;
            this.ServiceName = this.PeerInfo.ServiceName;
        }

        public string DisplayName { get; private set; }
        public string HostName { get; private set; }
        public string ServiceName { get; private set; }
        public PeerInformation PeerInfo { get; private set; }
    }
}
