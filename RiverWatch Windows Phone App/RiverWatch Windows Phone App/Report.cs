using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using System.IO;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Graphics.Imaging;

namespace RiverWatch_Windows_Phone_App
{
    class Report
    {
        // partial checks
        private Boolean imageReady = false;
        private Boolean geolocationReady = false;
        private Boolean tagsReady = false;
        private Boolean descriptionReady = false;

        // image information
        private BitmapImage pollutionImage = null;

        // location information
        private String longi = "";
        private String latit = "";

        // textual information
        private String description = "";
        private List<String> tags = null;
        private String date = "";

        public Report()
        {

        }

        public Boolean isReportReady()
        {
            if (this.imageReady && this.geolocationReady)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Boolean isImageReady()
        {
            return this.imageReady;
        }

        public Boolean isGeolocationReady()
        {
            return this.geolocationReady;
        }

        public Boolean isTagsReady()
        {
            return this.tagsReady;
        }

        public Boolean isDescriptionReady()
        {
            return this.descriptionReady;
        }

        public byte[] reportToByteStream()
        {
            byte[] finalByteArray = new byte[1];
            String reportString = "";

            // convert image file path
            String uriToImage = this.pollutionImage.UriSource.AbsolutePath;
            reportString += uriToImage+":~:";

            // convert geolocation
            reportString += this.longi+":~:";
            reportString += this.latit+":~:";

            // convert tags (if any)
            if (this.tags != null)
            {
                reportString += this.tags.Count + ":~:";

                // loop through tags

            }

            // convert description (if any)
            if (this.description.Length > 0)
            {
                reportString += this.description + ":~:";
            }

            // convert water quality report (if any)

            // finally, remove last 3 chars, then convert string to byte array
            reportString = reportString.Substring(0,reportString.Length-3);

            Debug.WriteLine("Report String: "+reportString);

            finalByteArray = GetBytes(reportString);

            return finalByteArray;
        }

        public static Report byteStreamToReport()
        {
            return null;
        }

        static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        static string GetString(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public async void discardReport()
        {
            // partial checks
            imageReady = false;
            geolocationReady = false;
            tagsReady = false;
            descriptionReady = false;

            // image information
            // delete actual image related to this report
            if (this.pollutionImage != null)
            {
                Debug.WriteLine("Deleting file: " + "RiverWatchImage_" + this.date + ".jpg");
                StorageFile file = await (ApplicationData.Current.LocalFolder.GetFileAsync("RiverWatchImage_" + this.date + ".jpg"));
                await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
            
            }
            pollutionImage = null;

            // location information
            longi = "";
            latit = "";

            // textual information
            description = "";
            tags = null;
            date = "";
        }

        private async Task getGeoPosition()
        {
            // initialise geolocation values
            this.geolocationReady = false;
            this.longi = "";
            this.latit = "";

            var geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 80;
            Geoposition position = await geolocator.GetGeopositionAsync();
            //TODO check that this falls within new zealand!!!
            this.latit = "" + position.Coordinate.Latitude;
            this.longi = "" + position.Coordinate.Longitude;
            this.geolocationReady = true;
        }


        // getters
        public String getReportName()
        {
            return "RiverWatchReport_"+this.date+".rwr"; // RiverWatchReport
        }

        public BitmapImage getSource()
        {
            return this.pollutionImage;
        }

        public String getLongitude()
        {
            return this.longi;
        }

        public String getLatitude()
        {
            return this.latit;
        }

        public List<String> getTags()
        {
            return this.tags;
        }

        public String getDescription()
        {
            return this.description;
        }

        public String getDate()
        {
            return this.date;
        }

        // setters

        public Boolean setBitmapImage(BitmapImage bi)
        {
            this.pollutionImage = bi;
            this.imageReady = true;

            // sneakily start the geolocation task
            this.getGeoPosition();

            // the date the photo was taken is in the files name
            Debug.WriteLine("source:" + bi.UriSource.AbsolutePath);

            // get file name
            String[] sa = bi.UriSource.AbsolutePath.Split(new Char[] {'/','.'});
            Debug.WriteLine("Cut off absolute: "+sa[sa.Length - 2]);
            String filename = sa[sa.Length - 2];

            // get date and time
            String dateAndTime = filename.Substring(16, 19);
            
            // set the date of the report
            this.date = dateAndTime;

            return true;
        }

        public Boolean setGeolocation(Geoposition geo)
        {
            this.latit = "" + geo.Coordinate.Latitude;
            this.longi = "" + geo.Coordinate.Longitude;
            this.geolocationReady = true;
            return true;
        }

        public Boolean setTags(List<String> taglist)
        {
            this.tags = taglist;
            this.tagsReady = true;
            return true;
        }

        public Boolean setDescription(String desc)
        {
            this.description = desc;
            this.descriptionReady = true;
            return true;
        }

        public void setTagsNotReady()
        {
            this.tagsReady = false;
        }

        public void setDescriptionNotReady()
        {
            this.descriptionReady = false;
        }
    }
}
