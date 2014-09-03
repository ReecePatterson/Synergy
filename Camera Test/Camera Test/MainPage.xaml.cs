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
using Windows.Media.Capture;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.MediaProperties;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace Camera_Test
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
    
    public sealed partial class MainPage : Page
    {
        private MediaCapture mediaCapture;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        //Aquired from http://www.romasz.net/how-to-take-a-photo-in-windows-runtime/
        private static async Task<DeviceInformation> GetCameraID(Windows.Devices.Enumeration.Panel desired)
        {
            DeviceInformation deviceID = (await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture))
                .FirstOrDefault(x => x.EnclosureLocation != null && x.EnclosureLocation.Panel == desired);

            if (deviceID != null) return deviceID;
            else throw new Exception(string.Format("Camera of type {0} doesn't exist.", desired));
        }

        async void photoCapture(object sender, RoutedEventArgs e)
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
    }
}
