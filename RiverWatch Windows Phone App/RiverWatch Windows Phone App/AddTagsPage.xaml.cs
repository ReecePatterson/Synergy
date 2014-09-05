using RiverWatch_Windows_Phone_App.Common;
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
            List<String> tags = new List<String>();

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
            if (tag7.IsChecked == true)
            {
                tags.Add(tag7.Content.ToString());
            }
            if (tag8.IsChecked == true)
            {
                tags.Add(tag8.Content.ToString());
            }

            
            //PollutionReportPage.setTags(tags);

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(PollutionReportPage),tags);
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
            
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            HardwareButtons_BackPressed(this, null);
        }

    }
}
