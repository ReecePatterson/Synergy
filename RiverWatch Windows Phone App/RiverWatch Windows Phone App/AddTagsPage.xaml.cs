﻿using RiverWatch_Windows_Phone_App.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
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
    public sealed partial class AddTagsPage : Page
    {
        public AddTagsPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            AppBarButton_Click(this, null);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // When navigated to this Frame, try to fetch an existing list of selected Report Tags.
            Report report = e.Parameter as Report;
            List<String> li = report.getTags();

            // If the list is not empty, match all contents of the list with our predetermined expected Tags.
            if (li != null)
            {
                if (li.Contains("Cow"))
                {
                    tag1.IsChecked = true;
                }
                if (li.Contains("Pollution"))
                {
                    tag2.IsChecked = true;
                }
                if (li.Contains("Runoff"))
                {
                    tag3.IsChecked = true;
                }
                if (li.Contains("Drain"))
                {
                    tag4.IsChecked = true;
                }
                if (li.Contains("Waterway"))
                {
                    tag5.IsChecked = true;
                }
                if (li.Contains("Paint"))
                {
                    tag6.IsChecked = true;
                }

                
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            List<String> tags = new List<String>();

            // If the CheckBox # is checked, add that Checkbox # 's representative String to the List
            // Can be improved to get the list of tags from the Web Server, instead of our hardcoded ones.
            if (tag1.IsChecked == true)
            {
                tags.Add(tag1.Content.ToString());
            }
            if (tag2.IsChecked == true)
            {
                tags.Add(tag2.Content.ToString());
            }
            if (tag3.IsChecked == true)
            {
                tags.Add(tag3.Content.ToString());
            }
            if (tag4.IsChecked == true)
            {
                tags.Add(tag4.Content.ToString());
            }
            if (tag5.IsChecked == true)
            {
                tags.Add(tag5.Content.ToString());
            }
            if (tag6.IsChecked == true)
            {
                tags.Add(tag6.Content.ToString());
            }

            // After the list is completed, navigate back to Pollution Report Page with the list stored in a session
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(PollutionReportPage), tags);
                //e.Handled = true;
            }
        }

    }
}
