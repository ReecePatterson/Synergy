using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// A page to review the details of an individual pollution report
    /// </summary>
    public sealed partial class ReviewReportPage : Page
    {
        private Report currentReport;
        private StorageFile currentReportFile;
        public ReviewReportPage()
        {
            this.InitializeComponent();
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            //disable camera button
            HardwareButtons.CameraReleased += HardwareButtons_DisableCameraButton;
        }

        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            //Debug.WriteLine("Hardware Back Pressed");
            this.ReturnButton_Click(this, null);
            e.Handled = true;
        }

        void HardwareButtons_DisableCameraButton(object sender, CameraEventArgs e)
        {
        }

        private void ReturnButton_Click(object sender, RoutedEventArgs e)
        {

            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(UnsentReportsPage));
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null && e.Parameter is String) //presume supplied file is a saved report file
            {
                currentReportFile = await StorageFile.GetFileFromPathAsync((String) e.Parameter); //set field for supplied report file
                await DisplayReportContent();
            }
            else //must send a report
            {
                Frame.Navigate(typeof(UnsentReportsPage));
            }
        }

        //Create report object for display from reportfile field
        private async Task DisplayReportContent()
        {
            string fileContent;
            using (StreamReader s = new StreamReader(await currentReportFile.OpenStreamForReadAsync())) //Read from the file
            {
                fileContent = await s.ReadToEndAsync();
                s.Dispose();
            }
            currentReport = await Report.GenerateFromString(fileContent); //use extracted string to create report
            
            PageTitle.Text = currentReport.getReportName(); //Set the page title to the name of the report
            imagePreview.Source = new BitmapImage(new Uri(currentReport.getImage().Path)); //display report image
            GeolocationToolTip.FontSize = 15; //display geolocation from report
            GeolocationToolTip.Text = "Latitude: " + currentReport.getLatitude() + "\n\nLongitude: " + currentReport.getLongitude();
            if (!currentReport.isTagsReady()) //Check if tags entered
            {
                TagsToolTip.FontSize = 20;
                TagsToolTip.Text = "No tags";
            }
            else
            {
                // display as much selected tags
                String t = "";
                int NoOfTags = currentReport.getTags().Count;

                if (NoOfTags == 0)
                {
                    TagsToolTip.FontSize = 20;
                    TagsToolTip.Text = "No tags";
                    currentReport.setTagsNotReady();
                }
                else
                {
                    int TagCount = 0;
                    int TagLimit = 8;

                    int LineCount = 0;
                    int LineLimit = 3;

                    foreach (String element in currentReport.getTags())
                    {
                        // if we've displayed 8 tags
                        if (TagCount >= TagLimit)
                        {
                            break;
                        }

                        // if we've got three tags on a line
                        if (!(LineCount < LineLimit))
                        {
                            t += "\n";
                            LineCount = 0;
                        }

                        // append tag
                        t += "#" + element + ", ";
                        LineCount++;
                        TagCount++;
                    }

                    // remove extra comma
                    t = t.Substring(0, t.Length - 2);

                    // if user selected more than 8 tags, add a ...
                    if (NoOfTags > TagLimit)
                    {
                        t += "...";
                    }

                    // display tags
                    TagsToolTip.FontSize = 15;
                    TagsToolTip.Text = t;
                }
            }

            // display description
            if (!currentReport.isDescriptionReady())
            {
                DescriptionToolTip.FontSize = 20;
                DescriptionToolTip.Text = "No description";
            }
            else
            {
                String desc = currentReport.getDescription();

                if (desc.Length == 0)
                {
                    DescriptionToolTip.FontSize = 20;
                    DescriptionToolTip.Text = "No description";
                    currentReport.setDescriptionNotReady();
                }
                else
                {
                    // need to parse out trailing whitespace
                    DescriptionToolTip.FontSize = 15;
                    DescriptionToolTip.Text = desc.Trim();
                }
            }
        }

        //Method for when the submit button has been tapped. Displays confirmation that they wish to send.
        private async void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            //Make message dialog to confirm if they want to send the report
            MessageDialog sendConfirm = new MessageDialog("Are you sure you wish to send this report?", "Send?");
            sendConfirm.Commands.Add(new UICommand("Send", new UICommandInvokedHandler(this.SendInvokedHandler)));
            sendConfirm.Commands.Add(new UICommand("No"));
            //Show user the message dialog
            await sendConfirm.ShowAsync();
        }

        //Respond to user confriming send prompt
        private async void SendInvokedHandler(IUICommand command)
        {
            if (command.Label == "Send")
            {
                // hide buttons
                this.SubmitButton.Visibility = Visibility.Collapsed;
                this.DeleteButton.Visibility = Visibility.Collapsed;

                // show progress ring
                this.processing.IsActive = true;

                if (await currentReport.UploadToServer()) //attempt upload
                {
                    await currentReport.discardReport(true); //delete report, image and saved file
                    await currentReportFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

                    this.processing.IsActive = false;
                    //navigate to previous page
                    Frame.Navigate(typeof(UnsentReportsPage));
                }
                else
                {
                    this.processing.IsActive = false;
                    MessageDialog didNotSend = new MessageDialog("Error when attempting to send report, please try again later", "Failed to Send");
                    didNotSend.Commands.Add(new UICommand("OK"));
                    await didNotSend.ShowAsync();
                    //show buttons again
                    this.SubmitButton.Visibility = Visibility.Visible;
                    this.DeleteButton.Visibility = Visibility.Visible;
                }
            }
        }

        //Method that repsonds to user pressing delete report button. Displays confirmation that they want to delete the report
        private async void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            //Create messageDialog
            MessageDialog deleteConfirm = new MessageDialog("Are you sure you wish to delete this report?", "Delete?");
            deleteConfirm.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.DeleteInvokedHandler)));
            deleteConfirm.Commands.Add(new UICommand("No"));
            //Show dialog to user
            await deleteConfirm.ShowAsync();
        }

        //Method that responds to the user confirming the delete of the current report
        private async void DeleteInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                await currentReport.discardReport(true); //delete report, image and saved file
                await currentReportFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Frame.Navigate(typeof(UnsentReportsPage));//navigate back to previous page
            }
        }
    }
}
