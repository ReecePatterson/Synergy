using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556


/**
 * Author: Emmanuel Godinez
 * Date: 24/08/14
 * 
 * For the purposes of testing things on Windows Phone 8.1
 * */

namespace RiverWatch_Windows_Phone_App
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CameraPage : Page
    {
        public CameraPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<object>(AppResume);
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //this.thing.Text = "On Navigated To";
            photoCapture();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //this.thing.Text = "On Navigated From";
            stopCamera();
            
        }
        
        void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if(rootFrame != null && rootFrame.CanGoBack)
            {
                rootFrame.Navigate(typeof(PollutionReportPage));
                e.Handled = true;
            }

        }

        private void AppResume(object sender, object e)
        {
            stopCamera();
            
        }

        private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel desired)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

            if (deviceID != null) return deviceID;
            else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
        }

        async void stopCamera()
        {
            await mediaCapture.StopPreviewAsync();
            mediaCapture.Dispose();
        }

        MediaCapture mediaCapture;

        async void photoCapture()
        {
            var cameraID = await GetCameraID(Windows.Devices.Enumeration.Panel.Back);
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = StreamingCaptureMode.Video,
                PhotoCaptureSource = PhotoCaptureSource.VideoPreview,
                AudioDeviceId = string.Empty,
                VideoDeviceId = cameraID.Id

            });
            //var maxResolution = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Aggregate((i1, i2) => (i1 as VideoEncodingProperties).Width > (i2 as VideoEncodingProperties).Width ? i1 : i2);
            //await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxResolution);


            mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            CameraPreview.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
        }

        async void CaptureImage_Click(object sender, RoutedEventArgs e)
        {
            Image i = new Image();
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // create storage file in local app storage
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "TestPhoto.jpg",
                CreationCollisionOption.GenerateUniqueName);

            // take photo
            await mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

            // Get photo as a BitmapImage
            BitmapImage bmpImage = new BitmapImage(new Uri(file.Path));

            // imagePreivew is a <Image> object defined in XAML
            preview.Source = bmpImage;
            i.Source = bmpImage;

            PollutionReportPage.setImage(bmpImage);
            Frame.GoBack();
        }

        async void ReturnButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
