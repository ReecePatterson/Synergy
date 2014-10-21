using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
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
    public sealed partial class SettingsPage : Page
    {
        private Geolocator myGeoLocator = new Geolocator();

        public SettingsPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            // Send the user input within the textbox of this page to the Pollution Report Page
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(HubPage));
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Boolean current ;
            try
            {
                current = (Boolean)Application.Current.Resources["locSer"];
            }catch(Exception ex){
                current = false;
            }
            
            Debug.WriteLine("Settings Initially says: " + current);
            Application.Current.Resources["locSer"] = current;
            locationServices.IsOn = current;
        }

        

        private void privacyPolicy_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // go to page that displays privacy policy
            Frame.Navigate(typeof(PrivacyPolicyPage));
        }

        private void locationServices_Toggled(object sender, RoutedEventArgs e)
        {
            // update location services boolean
            // boolean is after toggle is shifted
            Boolean current = locationServices.IsOn;
            Debug.WriteLine("Settings says: " + current);
            Application.Current.Resources["locSer"] = current;
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HubPage));
        }
    }
}
