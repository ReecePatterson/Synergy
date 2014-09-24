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
using Windows.UI.Popups;
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
    /// An empty page that can be used on its own or navigated to within a Frame. TDODO CHANGE THIS
    /// </summary>
    public sealed partial class UnsentReportsPage : Page
    {
        private List<Report> reports = new List<Report>();
        private IReadOnlyList<StorageFile> reportFiles;
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
            reportFiles = await unsentReportFolder.GetFilesAsync(); //TODO change this to be actual reports
            for (int i = 0; i < reportFiles.Count;i++ )
            {
                //Read file, split itno info bits.
                
                
                StorageFile currFile = reportFiles.ElementAt(i);
                string fileContent;
                using (StreamReader s = new StreamReader(await currFile.OpenStreamForReadAsync()))
                {
                    fileContent = await s.ReadToEndAsync();
                    s.Dispose();
                }
                Debug.WriteLine(fileContent + "\n");
                //Report currReport = new Report(fileContent);
                Report currReport = await Report.GenerateFromString(fileContent);
                //string[] reportComponents = fileContent.Split(new String[]{":~:"},StringSplitOptions.None);
                //create preview
                //image block preview
                Image previewImage = new Image();
                previewImage.Source = new BitmapImage(new Uri(currReport.getImage().Path));
                previewImage.HorizontalAlignment = HorizontalAlignment.Left;
                previewImage.Stretch = Stretch.UniformToFill;
                previewImage.Margin = new Thickness(5,5,5,5);

                //List View Item (outer wrapper)
                ListViewItem currFileItem = new ListViewItem();
                currFileItem.Name = currReport.getReportName();
                currFileItem.Background = new SolidColorBrush(itemBackground);
                currFileItem.Content = previewImage;
                currFileItem.Height = 100;
                currFileItem.Margin = new Thickness(0, 5, 0, 5);
                currFileItem.Tapped +=currFileItem_Tapped;

                reports.Add(currReport);
                reportItems.Add(currFileItem);
            }
            UnsentRportList.ItemsSource = reportItems;
        }

        private void currFileItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!(sender is ListViewItem))
            {
                return;
            }
            //goto new page with current report
            ListViewItem senderLVI = (ListViewItem) sender;
            foreach(Report currReport in reports){
                Debug.WriteLine("%s compared to %s", currReport.getReportName(), senderLVI.Name);
                if (currReport.getReportName() == senderLVI.Name)
                {
                    Frame.Navigate(typeof (ReviewReportPage), currReport);
                    return; //report found, break out of the mehtod
                }
            }
        }

        private async void SendAllButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog sendConfirm = new MessageDialog("Are you sure you wish to send all reports?", "Send All?");
            sendConfirm.Commands.Add(new UICommand("Send", new UICommandInvokedHandler(this.SendInvokedHandler)));
            sendConfirm.Commands.Add(new UICommand("No"));

            await sendConfirm.ShowAsync();
        }

        private async void SendInvokedHandler(IUICommand command)
        {
            if (command.Label == "Send")
            {
                //SEND ALL REPORTS

                foreach (Report r in reports) {
                   await r.UploadToServer();
                }

                foreach (Report currReport in reports)
                {
                    currReport.discardReport(true);
                }
                foreach (StorageFile currFile in reportFiles)
                {
                    await currFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                await refreshReportList();
            }
        }

        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog deleteConfirm = new MessageDialog("Are you sure you wish to delete all unsaved reports?", "Delete All?");
            deleteConfirm.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.DeleteInvokedHandler)));
            deleteConfirm.Commands.Add(new UICommand("No"));

            await deleteConfirm.ShowAsync();

            
        }

        private async void DeleteInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {

                foreach (Report currReport in reports)
                {
                    currReport.discardReport(true);
                }
                foreach (StorageFile currFile in reportFiles)
                {
                    await currFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                await refreshReportList();
            }
        }


    }
}
