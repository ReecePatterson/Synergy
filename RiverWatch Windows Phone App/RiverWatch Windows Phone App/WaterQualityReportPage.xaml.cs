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
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            _pairedDevices = new ObservableCollection<PeerInformation>();
            PairedDevicesList.ItemsSource = _pairedDevices;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e) {
            Frame.Navigate(typeof(HubPage));
        }


        private void RefreshPairedDevices_Click(object sender, RoutedEventArgs e) {
            RefreshPairedDevicesList();

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

        private void ConnectToSelectedDevice_Click(object sender, RoutedEventArgs e) {
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
                string serviceName = "2";

                // Note: If either parameter is null or empty, the call will throw an exception
                await _socket.ConnectAsync(peer.HostName, serviceName);

                // If the connection was successful, the RemoteAddress field will be populated
                //MessageBox.Show(String.Format(AppResources.Msg_ConnectedTo, _socket.Information.RemoteAddress.DisplayName));
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
}
