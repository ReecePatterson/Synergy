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

        public NavigationHelper NavigationHelper { get; private set; }

        public PollutionReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            NavigationHelper = new NavigationHelper(this);
            NavigationHelper.LoadState += NavigationHelper_LoadState;
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
            //pollutionImage = i;
            Boolean result = report.setBitmapImage(i);
        }

        public static void setTags(List<String> tagList)
        {
            //tags = tagList;
            Boolean result = report.setTags(tagList);
            Debug.WriteLine("Camera here " + report.getSource());
            Debug.WriteLine("IM HERE "+report.getTags()[0]);
        }

        public static void setDescription(String desc)
        {
            //description = desc;
            Boolean result = report.setDescription(desc);
        }

        // 

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            //String s = e.NavigationParameter as String;

            BitmapImage bi = e.NavigationParameter as BitmapImage;

            Debug.WriteLine("this here " + bi);
            
            // this needs to take the parameter given from another page
            // for example, (on the other page...) Frame.Navigate(typeof(PollutionReportPage),tagList)

            // Employee emp = e.NavigationParameter as Employee; // This casts
            // if (emp != null)
            // {
            //     txtName.Text = emp.Name;
            //     txtID.Text = emp.ID.ToString();
            // }
            Frame.GoBack();
        }

    }
}
