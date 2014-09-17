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
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RiverWatch_Windows_Phone_App
{
    public class Report
    {
        // partial checks
        private Boolean imageReady = false;
        private Boolean geolocationReady = false;
        private Boolean tagsReady = false;
        private Boolean descriptionReady = false;

        // image information
        private StorageFile pollutionImage = null;

        // location information
        private String longi = "";
        private String latit = "";

        // textual information
        private String description = "";
        private List<String> tags = null;
        private String date = "";

        //byte info
        byte[] imageByte;

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

        //public StorageFile saveReportToFile() {
            //StorageFile sf = ApplicationData.Current.LocalFolder.CreateFileAsync("");
        //}

        public byte[] convertToSave()
        {
            String returnString = "";
            
            // write path
            returnString += this.pollutionImage.Path + ":~:";

            // write geo
            returnString += this.latit + ":~:";
            returnString += this.longi + ":~:";

            // write tags if any
            if (this.isTagsReady()) {
                returnString += this.tags.Count + ":~:";

                foreach (String t in tags) {
                    returnString += t + ":~:";
                }
            }

            // write desc if any
            if (this.isDescriptionReady()) {
                returnString += this.description;
            }

            // write water quality report

            return GetBytes(returnString);
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

        public async void discardReport(Boolean deleteImage)
        {
            // partial checks
            imageReady = false;
            geolocationReady = false;
            tagsReady = false;
            descriptionReady = false;

            // image information
            // delete actual image related to this report
            if (deleteImage) { 
                if (this.pollutionImage != null)
                {
                    try { 
                        Debug.WriteLine("Deleting file: " + "RiverWatchImage_" + this.date + ".jpg");
                        StorageFile file = await (ApplicationData.Current.LocalFolder.GetFileAsync("RiverWatchImage_" + this.date + ".jpg"));
                        await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                    }
                    catch (FileNotFoundException e) {
                        // if a file is not found, it's already deleted
                    }
                }
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
            this.latit = "" + position.Coordinate.Latitude; //TODO research if this is needs to be changed for updated releases point.position.latitude
            this.longi = "" + position.Coordinate.Longitude;
            this.geolocationReady = true;
        }


        // getters
        public String getReportName()
        {
            return "RiverWatchReport_"+this.date+".txt"; // RiverWatchReport
        }

        public StorageFile getImage()
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

        public Boolean setImage(StorageFile imageFile)
        {
            this.pollutionImage = imageFile;
            this.imageReady = true;

            // sneakily start the geolocation task
            this.getGeoPosition(); //TODO research how to make await methods in non-async methods and to make it shut up :(

            // the date the photo was taken is in the files name
            Debug.WriteLine("source:" + imageFile.Path);

            // get file name
            //String[] sa = imageUri.AbsolutePath.Split(new Char[] {'/','.'});
            //Debug.WriteLine("Cut off absolute: "+sa[sa.Length - 2]);
            String filename = imageFile.Name;

            // get date and time
            this.date = filename.Substring(16, 19); //TODO find a nicer way to do this

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
            if (taglist.Count != 0)
            {
                this.tagsReady = true;
            }
            else
            {
                this.tagsReady = false;
            }
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
