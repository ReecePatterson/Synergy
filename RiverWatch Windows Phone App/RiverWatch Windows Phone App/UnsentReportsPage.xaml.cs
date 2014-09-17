using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Windows.UI.Xaml.Media.Imaging;
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
        private BitmapImage deleteImageSource = new BitmapImage();
        private Color deleteBackground = Color.FromArgb(0xFF, 0x5F, 0x9F, 0x9F);//#5F9F9F
        private Color itemBackground = Color.FromArgb(0xFF, 0xAD, 0xD8, 0xE6);//"#ADD8E6"


        public UnsentReportsPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            try
            {
                deleteImageSource = new BitmapImage();
                deleteImageSource.UriSource = (new Uri(@"ms-appx:///Assets/deleteIcon.png"));
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine("DELETE IMAGE FAILED TO LOAD");//image not found!
            }
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
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(HubPage));
            }
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
            reportItems.Clear();
            
            StorageFolder unsentReportFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Unsent_Reports", CreationCollisionOption.OpenIfExists);
            reports = await unsentReportFolder.GetFilesAsync();
            for (int i = 0; i < reports.Count;i++ )
            {
                //Read file, split itno info bits.
                //create preview
                //Delete Button
                Image deleteImage = new Image();
                deleteImage.VerticalAlignment = VerticalAlignment.Center;
                deleteImage.HorizontalAlignment = HorizontalAlignment.Center;
                deleteImage.Height = 60;
                deleteImage.Width = 60;
                deleteImage.Source = deleteImageSource;

                Grid deleteImageBound = new Grid();
                //Set a unique name linking to report
                deleteImageBound.HorizontalAlignment = HorizontalAlignment.Right;
                deleteImageBound.Width = 100;
                deleteImageBound.Background = new SolidColorBrush(deleteBackground);
                deleteImageBound.Tapped += DeleteReport_Click;
                deleteImageBound.Children.Add(deleteImage);

                //image block preview
                Image previewImage = new Image();
                previewImage.HorizontalAlignment = HorizontalAlignment.Left;
                previewImage.Stretch = Stretch.UniformToFill;
                previewImage.Width = 220;
                previewImage.Margin = new Thickness(5,5,0,5);
                //get image source from file


                //Surrounding grid
                Grid currItemGrid = new Grid();
                currItemGrid.Background = new SolidColorBrush(itemBackground);
                currItemGrid.VerticalAlignment = VerticalAlignment.Top;
                currItemGrid.Margin = new Thickness(0, 0, 0, 0);
                currItemGrid.Height = 90;
                currItemGrid.Width = deleteImageBound.Width + previewImage.Width;
                currItemGrid.Children.Add(deleteImageBound);
                currItemGrid.Children.Add(previewImage);

                //List View Item (outer wrapper)
                ListViewItem currFileItem = new ListViewItem();
                currFileItem.Content = currItemGrid;

                reportItems.Add(currFileItem);
            }
            UnsentRportList.ItemsSource = reportItems;
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
