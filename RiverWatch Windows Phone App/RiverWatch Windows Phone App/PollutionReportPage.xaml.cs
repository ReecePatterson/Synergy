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
        private Boolean UIReadyToSend = false;

        public PollutionReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            //disable camera button
            HardwareButtons.CameraReleased += HardwareButtons_DisableCameraButton;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //Debug.WriteLine("Hardware Back Pressed");
            this.ReturnButton_Click(this, null);
            e.Handled = true;
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
            if (e.Parameter is Uri)
            {
                Debug.WriteLine("photo");
                Uri iu = e.Parameter as Uri;
                report.setImageUri(iu);
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
            // else
            
            UpdatePollutionReport();
        }

        public void UpdatePollutionReport()
        {
            // display image
            if (report.getImageUri() == null)
            {
                ImageToolTip.Text = "Take a photo";
            }
            else
            {
                ImageToolTip.Text = "";
                imagePreview.Source = new BitmapImage(report.getImageUri());
            }

            // display geolocation
            if (!report.isGeolocationReady())
            {
                if (report.getImageUri() == null)
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
                int NoOfTags = report.getTags().Count;

                if (NoOfTags == 0)
                {
                    TagsToolTip.FontSize = 20;
                    TagsToolTip.Text = "Select tags";
                    report.setTagsNotReady();
                }
                else { 
                    int TagCount = 0;
                    int TagLimit = 6;

                    int LineCount = 0;
                    int LineLimit = 2;
                
                    foreach (String element in report.getTags())
                    {
                        // if we've displayed 6 tags
                        if (TagCount >= TagLimit)
                        {
                            break;
                        }

                        // if we've got two tags on a line
                        if (!(LineCount < LineLimit))
                        {
                            t += "\n";
                            LineCount = 0;
                        }

                        // append tag
                        t += "#"+element+", ";
                        LineCount++;
                        TagCount++;
                    }

                    // remove extra comma
                    t = t.Substring(0, t.Length - 2);
                    Debug.WriteLine(t);

                    // if user selected more than 6 tags, add a ...
                    if (NoOfTags > 6)
                    {
                        t += "...";
                    }

                    // display tags
                    TagsToolTip.FontSize = 15;
                    TagsToolTip.Text = t;
                }
            }

            // display description
            if (!report.isDescriptionReady())
            {
                DescriptionToolTip.FontSize = 20;
                DescriptionToolTip.Text = "Add description";
            }
            else
            {
                String desc = report.getDescription();

                if (desc.Length == 0)
                {
                    DescriptionToolTip.FontSize = 20;
                    DescriptionToolTip.Text = "Add description";
                    report.setDescriptionNotReady();
                }
                else { 
                    // need to parse out trailing whitespace
                    DescriptionToolTip.FontSize = 15;
                    DescriptionToolTip.Text = desc.Trim();
                }
            }

            // if report is complete, we need to compact the grids and display submit button
            if (report.isReportReady())
            {
                if (UIReadyToSend == false)
                {
                    UIReadyToSend = true;
                    Debug.WriteLine("Report is now ready to send");
                    animateReadyToSend();
                }
            }
            else
            {
                if (UIReadyToSend == true) {
                    UIReadyToSend = false;
                    Debug.WriteLine("Report is now not ready to send");
                    animateNotReadyToSend();
                }
            }
        }

        private async void checkGeolocation()
        {
            Debug.WriteLine("geolocation search started");
            while (!report.isGeolocationReady())
            {
                await Task.Delay(2000);
                //TODO add timeout
            }
            Debug.WriteLine("geolocation ready");
            UpdatePollutionReport();
        }

        private async void animateReadyToSend()
        {
            double top = 0;
            for (int i = 0; i < 10; i++)
            {
                // animate geolocation
                top = GeolocateGrid.Margin.Top;
                GeolocateGrid.Margin = new Thickness(60, top-1, 10, 0);

                // animate tags
                top = TagsGrid.Margin.Top;
                TagsGrid.Margin = new Thickness(60, top - 2, 10, 0);

                // animate description
                top = DescriptionGrid.Margin.Top;
                DescriptionGrid.Margin = new Thickness(60, top - 3, 10, 0);

                // animate water quality
                top = WaterQualityGrid.Margin.Top;
                WaterQualityGrid.Margin = new Thickness(60, top - 4, 10, 0);
                
                await Task.Delay(20);
            }

            SubmitButton.Visibility = Visibility.Visible;
        }
        private async void animateNotReadyToSend()
        {
            // not sure if this will be used

            // animate geolocation
            GeolocateGrid.Margin = new Thickness(60, 160, 10, 0);
            await Task.Delay(500);

            // animate tags
            TagsGrid.Margin = new Thickness(60, 270, 10, 0);
            await Task.Delay(500);

            // animate description
            DescriptionGrid.Margin = new Thickness(60, 380, 10, 0);
            await Task.Delay(500);

            // animate water quality
            WaterQualityGrid.Margin = new Thickness(60, 490, 10, 0);
            await Task.Delay(500);

            SubmitButton.Visibility = Visibility.Collapsed;
        }


        // ====== click events =======

        private void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubmitReportPage), report);
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack) {
                report.discardReport();

                rootFrame.Navigate(typeof(HubPage));
            }
        }

        private void cameraButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(CameraPage));
        }
        
        private void tagButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddTagsPage),report);
        }

        private void descriptionButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(AddDescriptionPage),report);
        }

        private void waterQualityButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Frame.Navigate(typeof(WaterQualityReportPage));
        }

        void HardwareButtons_DisableCameraButton(object sender, CameraEventArgs e) {
        }
    }
}
