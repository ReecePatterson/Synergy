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
        private static Image pollutionImage = null;
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
        public Boolean textReady = false;



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
            // init geolocation
            geolocationReady = false;
            
            // check if an image is saved, if so, its ready
            if (pollutionImage != null)
            {
                // start finding geolocation
                this.GeolocationToolTip.Text = "Loading Coordinates ...";
                getGeoPosition();

                // remove tool tip for image
                this.ImageToolTip.Text = "";
                // resize the photo tile and put the image taken on it
                //
            }
            else
            {
                // keep tool tip for image
                this.ImageToolTip.Text = "Take a photo";
                // keep tool tip for geolocation
                this.GeolocationToolTip.Text = "Find location";
            }

            // check if textual information is filled out, if so, its ready

            // init datetime
            this.date = string.Format("{0 : dd/MM/yyyy}", DateTime.Now);
        }

        private async Task getGeoPosition()
        {
            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 100;
            Geoposition position = await geolocator.GetGeopositionAsync();
            this.latit = "" + position.Coordinate.Latitude;
            this.longi = "" + position.Coordinate.Longitude;

            // resize the geolocation tile and font, then display the coordinates
            // 
            this.GeolocationToolTip.FontSize = 15;
            this.GeolocationToolTip.Text = "Latitude: " + this.latit + "\n\nLongitude: " + this.longi;
            geolocationReady = true;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            pollutionImage = null;
            Frame.GoBack();
        }

        private void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            // Gotta collate all the information we need to send to the server...

            // Save information locally on the phone
            
            // Send information to WaiNZ server

            // If send was successful, delete report that was saved locally
        }

        private void AddTags_Click(object sender, RoutedEventArgs e)
        {
            //Frame.Navigate(typeof(AddTagsPage));
        }

        private void cameraButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(TestPage));
        }

        public static void setImage(Image i)
        {
            pollutionImage = i;
        }

    }
}
