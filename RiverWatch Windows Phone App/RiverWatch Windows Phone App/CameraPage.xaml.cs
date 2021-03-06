﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Enumeration;
using Windows.Devices.Geolocation;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Phone.UI.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
        private SimpleOrientationSensor _simpleorientation;

        public CameraPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<object>(AppResume);
            HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            HardwareButtons.CameraPressed += HardwareButtons_CameraPressed;
            HardwareButtons.CameraHalfPressed += HardwareButtons_CameraHalfPressed;
            HardwareButtons.CameraReleased += HardwareButtons_CameraHalfPressed;

            _simpleorientation = SimpleOrientationSensor.GetDefault();
            if (_simpleorientation != null) {
                _simpleorientation.OrientationChanged += new TypedEventHandler<SimpleOrientationSensor, SimpleOrientationSensorOrientationChangedEventArgs>(OrientationChanged);
            }
            
            //DisableNotificationsBar();
        }

        //for disabling the notifications bar on this page
        private void DisableNotificationsBar() {
            DisableNotificationsBarHelper();
        }

        //helper function for DisableNotificationsBar for calling async function
        async void DisableNotificationsBarHelper() {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
        }
        
        //field to store current angle that the phone is set so the image taken is encoded with this in mind
        int currentAngle = 0;

        // For orientation Switching
        private async void OrientationChanged(object sender, SimpleOrientationSensorOrientationChangedEventArgs e) {
            // Set priority to Camera in order to avoid pulling from other UI elements at its runtime
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => {
                SimpleOrientation orientation = e.Orientation;
                // Changes Camera Object orientation depending on Phone orientation
                switch (orientation) {
                    case SimpleOrientation.NotRotated:
                        //Portrait Up 
                        cameraButton.RenderTransform = new RotateTransform() { Angle = 0 };
                        currentAngle = 0;
                        break;
                    case SimpleOrientation.Rotated90DegreesCounterclockwise:
                        //LandscapeLeft 
                        cameraButton.RenderTransform = new RotateTransform() { Angle = 90 };
                        currentAngle = 90;
                        break;
                    case SimpleOrientation.Rotated180DegreesCounterclockwise:
                        //PortraitDown 
                        cameraButton.RenderTransform = new RotateTransform() { Angle = 180 };
                        currentAngle = 180;
                        break;
                    case SimpleOrientation.Rotated270DegreesCounterclockwise:
                        //LandscapeRight 
                        cameraButton.RenderTransform = new RotateTransform() { Angle = 270 };
                        currentAngle = 270;
                        break;
                    case SimpleOrientation.Faceup:
                       // txtOrientation.Text = "Faceup";
                        break;
                    case SimpleOrientation.Facedown:
                        //txtOrientation.Text = "Facedown";
                        break;
                    default:
                        //txtOrientation.Text = "Unknown orientation";
                        break;
                }
            });
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // show capture button
            this.cameraButton.Visibility = Visibility.Visible;

            // hide progress bar
            this.processing.IsActive = false;

            // Upon navigation TOWARDS the Camera Page, st
            StartCamera();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Upon navigation AWAY from the Camera Page, either though BackPressed of CameraPressed,
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

        //this field is used to determine if the camera button has been fully pressed. If not then capture an image
        bool pressed = false;

        void HardwareButtons_CameraPressed(object sender, CameraEventArgs e) {
            if (!pressed) {
                CaptureImage_Click(this, null);
                pressed = true;
            }
        }


        void HardwareButtons_CameraHalfPressed(object sender, CameraEventArgs e) {
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

        async void StartCamera()
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

            var maxResolution = mediaCapture.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.Photo).Aggregate(
                    (i1, i2) => (i1 as VideoEncodingProperties).Width > (i2 as VideoEncodingProperties).Width ? i1 : i2);
            await mediaCapture.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.Photo, maxResolution);

            mediaCapture.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
            //mediaCapture.SetRecordRotation(VideoRotation.Clockwise90Degrees);
            CameraPreview.Source = mediaCapture;
            await mediaCapture.StartPreviewAsync();
        }

        async void CaptureImage_Click(object sender, RoutedEventArgs e)
        {
            // hide capture button
            this.cameraButton.Visibility = Visibility.Collapsed;

            // show progress bar
            this.processing.IsActive = true;

            //mediaCapture.SetRecordRotation(VideoRotation.None);
            ImageEncodingProperties imgFormat = ImageEncodingProperties.CreateJpeg();

            // find the time and date now
            DateTime dt = System.DateTime.Now;
            String date = dt.ToString("dd_MM_yyyy_H_mm_ss");

            // create storage file in local app storage, and name file according to date
            StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(
                "RiverWatchImage_"+date+".jpg",
                CreationCollisionOption.ReplaceExisting);

            // take photo
            await mediaCapture.CapturePhotoToStorageFileAsync(imgFormat, file);

            // Get photo as a BitmapImage
            Uri imageURI = new Uri(file.Path);

            //rotate image back to correct orientation if required
            if (currentAngle != 90) {

                IRandomAccessStream originalImage = await file.OpenAsync(FileAccessMode.ReadWrite);
                BitmapDecoder imageDecoder = await BitmapDecoder.CreateAsync(originalImage);
                var imageStream = new InMemoryRandomAccessStream();
                BitmapEncoder imageEncoder = await BitmapEncoder.CreateForTranscodingAsync(imageStream, imageDecoder);

                //set rotation angle
                switch (currentAngle) {
                    case 0:
                        imageEncoder.BitmapTransform.Rotation = BitmapRotation.Clockwise90Degrees;
                        break;
                    case 180:
                        imageEncoder.BitmapTransform.Rotation = BitmapRotation.Clockwise270Degrees;
                        break;
                    case 270:
                        imageEncoder.BitmapTransform.Rotation = BitmapRotation.Clockwise180Degrees;
                        break;
                }

                await imageEncoder.FlushAsync();

                BitmapImage bitmapImage = new BitmapImage();
                await bitmapImage.SetSourceAsync(imageStream);

                using (var outputStream = originalImage.GetOutputStreamAt(0)) {
                    using (var inputStream = imageStream.GetInputStreamAt(0)) {
                        IBuffer storageFileBuffer = new byte[imageStream.Size].AsBuffer();
                        await inputStream.ReadAsync(storageFileBuffer, storageFileBuffer.Length, InputStreamOptions.None);
                        await outputStream.WriteAsync(storageFileBuffer);
                    }
                }

            }

            // once finished, hide progress bar
            this.processing.IsActive = false;

            Frame.Navigate(typeof(PollutionReportPage),file);
        }
    }
}
