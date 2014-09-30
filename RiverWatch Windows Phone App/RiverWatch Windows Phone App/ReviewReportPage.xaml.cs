using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Animation;
using System.Threading.Tasks;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e != null && e.Parameter is StorageFile)
            {
                currentReportFile = (StorageFile) e.Parameter;
                DisplayReportContent();
            }
            else //must send information
            {
                Frame.Navigate(typeof(UnsentReportsPage));
            }
        }

        private async Task DisplayReportContent()
        {
            string fileContent;
            using (StreamReader s = new StreamReader(await currentReportFile.OpenStreamForReadAsync()))
            {
                fileContent = await s.ReadToEndAsync();
                s.Dispose();
            }
            //Report currReport = new Report(fileContent);
            currentReport = await Report.GenerateFromString(fileContent);
            
            PageTitle.Text = currentReport.getReportName();
            imagePreview.Source = new BitmapImage(new Uri(currentReport.getImage().Path));
            GeolocationToolTip.FontSize = 15;
            GeolocationToolTip.Text = "Latitude: " + currentReport.getLatitude() + "\n\nLongitude: " + currentReport.getLongitude();
            if (!currentReport.isTagsReady())
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
                        t += "#" + element + ", ";
                        LineCount++;
                        TagCount++;
                    }

                    // remove extra comma
                    t = t.Substring(0, t.Length - 2);

                    // if user selected more than 6 tags, add a ...
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

        private async void SubmitReport_Click(object sender, RoutedEventArgs e)
        {
            // hide buttons
            this.SubmitButton.Visibility = Visibility.Collapsed;
            this.DeleteButton.Visibility = Visibility.Collapsed;

            // show progress ring
            this.processing.IsActive = true;

            if (await currentReport.UploadToServer())
            {
                await currentReport.discardReport(true);
                await currentReportFile.DeleteAsync(StorageDeleteOption.PermanentDelete);

                this.processing.IsActive = false;
                //submit report
                Frame.Navigate(typeof(UnsentReportsPage));
            }
            else
            {
                this.processing.IsActive = false;
                MessageDialog didNotSend = new MessageDialog("Error when attempting to send report, please try again later", "Failed to Send");
                didNotSend.Commands.Add(new UICommand("OK"));
                await didNotSend.ShowAsync();
            }
        }

        private async void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            await currentReport.discardReport(true);
            await currentReportFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
            Frame.Navigate(typeof(UnsentReportsPage));
        }
    }
}
