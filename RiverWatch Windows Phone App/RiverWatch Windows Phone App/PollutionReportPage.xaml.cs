using RiverWatch_Windows_Phone_App.Common;
using RiverWatch_Windows_Phone_App.Data;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Phone.UI.Input;
using System.Diagnostics;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PollutionReportPage : Page
    {
        static Report report = new Report();

        public PollutionReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                report.discardReport();
                rootFrame.Navigate(typeof(HubPage));
                //e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e == null)
            {
                return;
            }
            if (e.Parameter is BitmapImage)
            {
                Debug.WriteLine("photo");
                BitmapImage bi = e.Parameter as BitmapImage;
                report.setBitmapImage(bi);
            }
            else if(e.Parameter is List<String>){
                Debug.WriteLine("tags");
                List<String> li = e.Parameter as List<String>;
                report.setTags(li);
            }
            else if (e.Parameter is String)
            {
                Debug.WriteLine("desc");
                String s = e.Parameter as String;
                report.setDescription(s);
            }
            //water quality
            else
            {

            }
            UpdatePollutionReport();
        }

        public async void UpdatePollutionReport()
        {
            // if report is complete, we need to compact the grids and display submit button
            if (report.isReportReady())
            {
                Debug.WriteLine("Report is ready to send");

                return;
            }

            // display image
            if (report.getSource() == null)
            {
                ImageToolTip.Text = "Take a photo";
            }
            else
            {
                ImageToolTip.Text = "";
                imagePreview.Source = report.getSource();
            }

            // display geolocation
            if (!report.isGeolocationReady())
            {
                if (report.getSource() == null)
                {
                    GeolocationToolTip.FontSize = 20;
                    GeolocationToolTip.Text = "Awaiting photo";
                }
                else
                {
                    GeolocationToolTip.FontSize = 20;
                    GeolocationToolTip.Text = "Finding coordinates...";

                    // if we've got an image, start async method that 
                    // sends request to report class every 10s to see if 
                    // geolocation is ready to be displayed on UI

                    checkGeolocation();
                }
            }
            else
            {
                GeolocationToolTip.FontSize = 15;
                GeolocationToolTip.Text = "Latitude: " + report.getLatitude() + "\n\nLongitude: " + report.getLongitude();
            }

            // display tags
            if (!report.isTagsReady())
            {
                TagsToolTip.FontSize = 20;
                TagsToolTip.Text = "Select tags";
            }
            else
            {
                // display as much selected tags
                String t = "";
                int count = 0;
                int limit = 2;
                foreach (String element in report.getTags())
                {
                    if (!(count < limit))
                    {
                        t += "\n";
                        count = 0;
                    }
                    t += "#"+element+", ";
                    count++;
                }
                t = t.Substring(0, t.Length - 2);
                Debug.WriteLine(t);

                TagsToolTip.FontSize = 15;
                TagsToolTip.Text = t;
            }

            // display description

        }

        private async void checkGeolocation()
        {
            Debug.WriteLine("geolocation search started");
            while (!report.isGeolocationReady())
            {
                await Task.Delay(2000);
            }
            Debug.WriteLine("geolocation ready");
            UpdatePollutionReport();
        }

        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // do the same as hardware back button
            this.HardwareButtons_BackPressed(this, null);
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
            Frame.Navigate(typeof(AddTagsPage));
        }

        private void cameraButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(CameraPage));
        }
        
        private void tagButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTagsPage));
        }

        private void descriptionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddDescriptionPage));
        }

        private void waterQualityButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WaterQualityReportPage));
        }

        // ===== public methods =====

        public static void setImage(BitmapImage i)
        {
            Boolean result = report.setBitmapImage(i);
        }

        public static void setTags(List<String> tagList)
        {
            Boolean result = report.setTags(tagList);
        }

        public static void setDescription(String desc)
        {
            Boolean result = report.setDescription(desc);
        }

    }
}
