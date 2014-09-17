using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Streams;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubmitReportPage : Page
    {
        private Report report;

        public SubmitReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            // do nothing... let the report get sent first
            
            //Frame rootFrame = Window.Current.Content as Frame;
            //if (rootFrame != null && rootFrame.CanGoBack)
            //{
            //    rootFrame.Navigate(typeof(PollutionReportPage));
            //    //e.Handled = true;
            //}
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.report = e.Parameter as Report;
            attemptSendToServer();

            //if we are coming back to this page from the camera page, the notifications bar will have been disabled so re-enable it

        }

        //used to reenable the notifications bar after navigating back from the camera page
        private void EnableNotificationsBar() {
            EnableNotificationsBarHelper();
        }

        private async void EnableNotificationsBarHelper(){
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
        }

        private static HttpClient _client;
        private readonly Uri uploadAddress = new Uri("http://www-test.wainz.org.nz/api/image");

        private async void attemptSendToServer()
        {
            this.SubmitReportText.Text = "Submitting your report\nto WaiNZ";
            Boolean success = false;
            await Task.Delay(2000);

            //convert to json and upload to server
            _client = new HttpClient();
            //Globals.MemberId = 1;
            int memberId = 2;

            byte[] buffer = new byte[1];

            try { 
                //BitmapImage bitmapImage = new BitmapImage(new Uri(report.getImage().Path)); 
                RandomAccessStreamReference rasr = RandomAccessStreamReference.CreateFromFile(report.getImage()); 
                var streamWithContent = await rasr.OpenReadAsync(); 
                buffer = new byte[streamWithContent.Size]; 
                await streamWithContent.ReadAsync(buffer.AsBuffer(), (uint)streamWithContent.Size, InputStreamOptions.None);
            }
            catch (FileNotFoundException e) {
                Debug.WriteLine(e.StackTrace + "");
            }

            foreach (byte b in buffer) {
                this.testText.Text += b + "";
            }

            success = true;

            // attempt to send
            // http://social.msdn.microsoft.com/forums/windowsapps/en-us/3fbf0af7-fe8d-44d8-85b4-11ff5d56becb/httpwebrequest-in-application-metro
            // CallService();

            if (success)
            {
                // tell user the report was successfully sent
                this.SubmitReportText.Text = "Report sent";

                // disable progress ring
                this.SubmitReportProgress.IsActive = false;

                // display tick icon
                
                // wait for a bit, discard report, then go back to hub
                await Task.Delay(2000);
                report.discardReport();
                Frame.Navigate(typeof(HubPage));
            }
            else
            {
                // tell user the report failed to send
                this.SubmitReportText.Text = "Report failed to send\nSave report?";

                // disable progress ring
                this.SubmitReportProgress.IsActive = false;

                // display ! icon

                // show commandbar
                this.commandBar.Visibility = Visibility.Visible;
            }
        }

        public static String CallService(string serviceUrl, string login, string password, string message)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serviceUrl);

            request.Method = "POST";
            return "";
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // check if we can save the report
            Boolean canSave = true;

            // check if we've got space

            // if so, save the report
            if (canSave)
            {
                saveReport();
            }

            // if not, tell the user they cannot save the report
            // and discard it.
            else
            {
                notifyUnableToSave();
            }
        }

        private async void saveReport()
        {
            // tell user that saving is in progress
            this.SubmitReportText.Text = "Saving your report";

            // show progress ring
            this.SubmitReportProgress.IsActive = true;

            // hide command bar
            this.commandBar.Visibility = Visibility.Collapsed;

            // wait for a bit
            await Task.Delay(2000);

            // get byte stream of report
            byte[] fileBytes = report.reportToByteStream();
            
            // find local storage
            var localStorage = ApplicationData.Current.LocalFolder;

            // create a file to save the report in
            var file = await localStorage.CreateFileAsync(report.getReportName(),
            CreationCollisionOption.FailIfExists);

            // write byte stream to file
            using (var s = await file.OpenStreamForWriteAsync())
            {
                s.Write(fileBytes, 0, fileBytes.Length);
            }

            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);

            // tell user that report has been saved
            this.SubmitReportText.Text = "Report Saved\nNot Really. Eman Deleted it sneakily";

            // hide progress ring
            this.SubmitReportProgress.IsActive = false;

            await Task.Delay(2000);

            // discard report and go back to hub
            report.discardReport();
            Frame.Navigate(typeof(HubPage));
        }

        private async void notifyUnableToSave()
        {
            var messageDialog = new MessageDialog("Your phone is in a state where this report cannot be saved. This report will be discarded.");
            
            messageDialog.Commands.Add(new UICommand(
                            "Okay",
                            new UICommandInvokedHandler(this.UnableToSaveInvokedHandler)));

            await messageDialog.ShowAsync();
        }

        private void UnableToSaveInvokedHandler(IUICommand command)
        {
            // discard report and go back to hub
            report.discardReport();
            Frame.Navigate(typeof(HubPage));
            
        }

        private void DiscardButton_Click(object sender, RoutedEventArgs e)
        {
            // ask user if they are sure they want to discard the report
            promptAreYouSure();
        }

        private async void promptAreYouSure()
        {
            // Create the message dialog and set its content
            var messageDialog = new MessageDialog("Are you sure you want to discard this report?");

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            
            messageDialog.Commands.Add(new UICommand(
                "Yes",
                new UICommandInvokedHandler(this.AreYouSureInvokedHandler)));
            messageDialog.Commands.Add(new UICommand(
                "No",
                new UICommandInvokedHandler(this.AreYouSureInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();
        }

        private void AreYouSureInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked
            if (command.Label.Equals("Yes"))
            {
                // discard report and go back to hub
                report.discardReport();
                Frame.Navigate(typeof(HubPage));
            }
            else
            {
                // Do nothing
            }
        }
    }
}
