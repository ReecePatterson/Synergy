using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Capture;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PollutionReportPage : Page
    {
        // fields

        // image information
        public Boolean imageReady = false;

        // location information
        public String longi = "";
        public String latit = "";
        public Boolean geolocationReady = false;

        // textual information
        public String title = "";
        public String description = "";
        public String tags = "";
        public String date = "";



        public PollutionReportPage()
        {
            this.InitializeComponent();
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // init datetime
            this.date = string.Format("{0 : dd/MM/yyyy}", DateTime.Now);
            
            // init geolocation
            geolocationReady = false;
            this.GeolocationText.Text = "Loading Coordinates ...";
            getGeoPosition();
        }

        private async Task getGeoPosition()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 100;
            Geoposition position = await geolocator.GetGeopositionAsync();
            this.latit = "" + position.Coordinate.Latitude;
            this.longi = "" + position.Coordinate.Longitude;
            this.GeolocationText.Text = "Latitude: " + this.latit + "\n\nLongitude: " + this.longi;
            geolocationReady = true;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void PhotoGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            
        }

        private void SubmitReport_Click(object sender, TappedRoutedEventArgs e)
        {
            // Gotta collate all the information we need to send to the server...

            // Save information locally on the phone
            
            // Send information to WaiNZ server

            // If send was successful, delete report that was saved locally
        }

    }
}
