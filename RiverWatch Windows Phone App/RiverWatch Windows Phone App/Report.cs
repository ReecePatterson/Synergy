using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media.Imaging;

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
            if (this.imageReady && this.geolocationReady && this.tagsReady && this.descriptionReady)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public byte[] reportToByteStream()
        {
            byte[] byteArray = new byte[10];

            // something done here

            return byteArray;
        }

        public void reportToFile()
        {

        }

        public void discardReport()
        {

        }
        // getters
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
        // setters

        public void setBitmapImage(BitmapImage bi)
        {
            this.pollutionImage = bi;
            this.imageReady = true;
        }

        public void setGeolocation(Geoposition geo)
        {
            this.latit = "" + geo.Coordinate.Latitude;
            this.longi = "" + geo.Coordinate.Longitude;
            this.geolocationReady = true;
        }

        public void setTags(List<String> taglist)
        {
            this.tags = taglist;
            this.tagsReady = true;
        }

        public void setDescription(String desc)
        {
            this.description = desc;
            this.descriptionReady = true;
        }
    }
}
