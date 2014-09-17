using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI;
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
    public sealed partial class UnsentReportsPage : Page
    {
        
        private IReadOnlyList<StorageFile> reports;
        private List<ListViewItem> reportItems = new List<ListViewItem>();

        //Page design for dynamically changing it
        private Color itemBackground = Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6);//"#ADD8E6"

        public UnsentReportsPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(HubPage));
                e.Handled = true;
            }
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            // do the same as hardware back button
            this.HardwareButtons_BackPressed(this, null);
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await refreshReportList();
        }

        private async Task refreshReportList()
        {
            StorageFolder unsentReportFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Unsent_Reports", CreationCollisionOption.OpenIfExists);
            reports = await unsentReportFolder.GetFilesAsync();
            for (int i = 0; i < reports.Count;i++ )
            {
                //create preview
                //Delete Button
                Image deleteImage = new Image();
                //deleteImage.n
                //image block preview
                
                //List View Item (outer wrapper)
                ListViewItem currFileItem = new ListViewItem();
                currFileItem.Background = new SolidColorBrush(itemBackground);


                reportItems.Add(currFileItem);
            }
            UnsentRportList.ItemsSource = reportItems;
        }
    }
}
