﻿using RiverWatch_Windows_Phone_App.Common;
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
            // else
            
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
                    desc = desc.Trim();

                    // Format description
                    // The description will have 23 chars per line,
                    // and 3 lines only. If longer, description will display
                    // "..." at the end of the third line
                    String finalDesc = "";

                    int LineCount = 0;
                    int LineLimit = 3;
                    Boolean bigDesc = false;
                    Boolean smallDesc = true;

                    int CharLimit = 23;
                    char[] delimiterChars = {' '};
                    String[] sa = desc.Split(delimiterChars);

                    String line = "";
                    int NoOfChars = 0;
                    foreach (String str in sa)
                    {
                        // get size of str
                        int len = str.Length;

                        // check if number of characters in word is more than limit
                        if (len > CharLimit)
                        {
                            // we need to split the word and connect using -

                        }

                        // get NoOfChars + len
                        int tempLength = NoOfChars + len;

                        if (tempLength < CharLimit)
                        {
                            // we can add this word to the line
                            line += str + " ";
                            NoOfChars += len + 1;
                        }
                        else
                        {
                            // we need to create a new line for the new word
                            finalDesc += line + "\n";
                            LineCount++;
                            line = "";
                            NoOfChars = 0;
                            smallDesc = false;

                            if (LineCount >= LineLimit)
                            {
                                bigDesc = true;
                                break;
                            }
                        }
                    }

                    if (smallDesc)
                    {
                        finalDesc += line;
                    }

                    else if (bigDesc)
                    {
                        // remove the ending new line char
                        finalDesc = finalDesc.Substring(0, finalDesc.Length - 2);

                        // replace with ...
                        finalDesc += "...";
                    }

                    // need to parse out trailing whitespace
                    finalDesc = finalDesc.Trim();

                    DescriptionToolTip.FontSize = 15;
                    DescriptionToolTip.Text = finalDesc;
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
            }
            Debug.WriteLine("geolocation ready");
            UpdatePollutionReport();
        }

        private async void animateReadyToSend()
        {
            SubmitButton.Visibility = Visibility.Visible;
        }
        private async void animateNotReadyToSend()
        {
            SubmitButton.Visibility = Visibility.Collapsed;
        }


        // ====== click events =======

        private void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            // Gotta collate all the information we need to send to the server...

            // Save information locally on the phone
            
            // Send information to WaiNZ server

            // If send was successful, delete report that was saved locally
        }

        private async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // do the same as hardware back button
            this.HardwareButtons_BackPressed(this, null);
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

    }
}
