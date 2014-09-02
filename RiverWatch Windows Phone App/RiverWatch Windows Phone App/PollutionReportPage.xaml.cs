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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PollutionReportPage : Page
    {
        // fields
        public String longi = "";
        public String latit = "";
        public Boolean geoFound = false;

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
            geoFound = false;
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
            geoFound = true;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

    }
}
