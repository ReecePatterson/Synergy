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
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame. TODO CHANGE THIS
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
            // show command bar
            this.cmdBar.Visibility = Visibility.Visible;

            // hide progress bar
            this.processing.IsActive = false;

            await refreshReportList();
        }

        //Method that reads all files in the saved reports folder and creates a list of report objects 
        //and displays ListViewItems to the user for each one
        private async Task refreshReportList()
        {
            //Clear previous lists
            reportItems.Clear();
            reports.Clear();
            //Open the folder (create an empty one if it doesnt exist yet)
            StorageFolder unsentReportFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Unsent_Reports", CreationCollisionOption.OpenIfExists);
            reportFiles = await unsentReportFolder.GetFilesAsync(); //Get all files from the folder
            for (int i = 0; i < reportFiles.Count;i++ ) //For each file
            {
                //Read file, split into info bits.
                StorageFile currFile = reportFiles.ElementAt(i);
                string fileContent;
                using (StreamReader s = new StreamReader(await currFile.OpenStreamForReadAsync()))
                {
                    fileContent = await s.ReadToEndAsync();
                    s.Dispose();
                }
                Report currReport = await Report.GenerateFromString(fileContent); //Generate the report object from read string
                //create preview
                //image block preview
                Image previewImage = new Image();
                previewImage.Source = new BitmapImage(new Uri(currReport.getImage().Path));
                previewImage.HorizontalAlignment = HorizontalAlignment.Left;
                previewImage.Stretch = Stretch.UniformToFill;
                previewImage.Margin = new Thickness(5,5,5,5);

                //List View Item (outer wrapper)
                ListViewItem currFileItem = new ListViewItem();
                currFileItem.Name = currFile.Name;
                currFileItem.Background = new SolidColorBrush(itemBackground);
                currFileItem.Content = previewImage;
                currFileItem.Height = 100;
                currFileItem.Margin = new Thickness(0, 5, 0, 5);
                currFileItem.Tapped +=currFileItem_Tapped;

                reports.Add(currReport); //Add report to list of reports
                reportItems.Add(currFileItem); //add listviewitem so the user can see the report preview
            }
            UnsentReportList.ItemsSource = reportItems; //Set the list of listViewItems to be the source for the list view on the page
            if (reportItems.Count > 0) //if there are unsent reports, hide the text saying there are none and make the command bar visible
            {
                noUnsentText.Text = "";
                cmdBar.Visibility = Visibility.Visible;
            }
            else //if no reports, display text to user and hid the command bar.
            {
                noUnsentText.Text = "No Unsent Reports";
                cmdBar.Visibility = Visibility.Collapsed;
            }
        }

        //Method to respond to tapping a report preview
        private void currFileItem_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!(sender is ListViewItem)) //send must be a listviewItem
            {
                return;
            }
            //goto new page with current report
            ListViewItem senderLVI = (ListViewItem) sender;
            foreach(StorageFile currReportFile in reportFiles){ //Find corresponding file to the item clicked
                if (currReportFile.Name == senderLVI.Name)
                {
                    Frame.Navigate(typeof (ReviewReportPage), currReportFile);
                    return; //report found, break out of the method
                }
            }
        }

        //Respond to the sendall button pressed by user
        private async void SendAllButton_Click(object sender, RoutedEventArgs e)
        {
            //Create confirmation dialog
            MessageDialog sendConfirm = new MessageDialog("Are you sure you wish to send all reports?", "Send All?");
            sendConfirm.Commands.Add(new UICommand("Send", new UICommandInvokedHandler(this.SendInvokedHandler)));
            sendConfirm.Commands.Add(new UICommand("No"));
            //Show dialog to user
            await sendConfirm.ShowAsync();
        }

        //Respond to user confirming they wish to send
        private async void SendInvokedHandler(IUICommand command)
        {
            if (command.Label == "Send")
            {
                // hide command bar
                this.cmdBar.Visibility = Visibility.Collapsed;
                
                // show progress bar
                this.processing.IsActive = true;

                //Loop for all the reports
                //foreach (Report r in reports)
                for (int i = 0; i < reports.Count;i++)   
                {
                    Report r = reports.ElementAt(i);
                    if (await r.UploadToServer()) //Attempt to send report to server
                    {
                        //Get file associated with report and delete it
                        StorageFolder unsentReportFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("Unsent_Reports");
                        StorageFile rFile = await unsentReportFolder.GetFileAsync(r.getReportName());
                        await rFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        await r.discardReport(true); //delete report object and image file
                        this.processing.IsActive = false;
                    }
                    else //Error in attempting to send reports. 
                    {
                        this.processing.IsActive = false;
                        MessageDialog didNotSend = new MessageDialog("Error when attempting to send reports, please try again later", "Failed to Send");
                        didNotSend.Commands.Add(new UICommand("OK"));
                        await didNotSend.ShowAsync(); //Display to user error message
                        this.cmdBar.Visibility = Visibility.Visible; //reenable page
                        break;
                    }
                }
                Frame.Navigate(typeof(UnsentReportsPage));
            }
        }

        //Method to respond to user pressing delete all button
        private async void DeleteAllButton_Click(object sender, RoutedEventArgs e)
        {
            //Create confirmation dialog
            MessageDialog deleteConfirm = new MessageDialog("Are you sure you wish to delete all unsent reports?", "Delete All?");
            deleteConfirm.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.DeleteInvokedHandler)));
            deleteConfirm.Commands.Add(new UICommand("No"));
            //display confirmation dialog to user
            await deleteConfirm.ShowAsync();
        }

        //Respond to user responding to confirmation to delete dialog
        private async void DeleteInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                foreach (Report currReport in reports) //loop for each report and delete it and image associated
                {
                    await currReport.discardReport(true);
                }
                foreach (StorageFile currFile in reportFiles) //loop for each file and delete them
                {
                    await currFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }

                Frame.Navigate(typeof(UnsentReportsPage)); //refresh page
            }
        }


    }
}
