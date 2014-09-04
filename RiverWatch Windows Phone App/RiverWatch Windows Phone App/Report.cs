using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace RiverWatch_Windows_Phone_App
{
    class Report
    {
        private Boolean reportReady = false;

        // image information
        private BitmapImage pollutionImage = null;
        private Boolean imageReady = false;

        // location information
        private String longi = "";
        private String latit = "";
        private static Boolean geolocationReady = false;

        // textual information
        private String description = "";
        private List<String> tags = null;
        private String date = "";
        private Boolean textReady = false;

        public Report()
        {

        }

        public void setBitmapImage()
        {

        }

    }
}
