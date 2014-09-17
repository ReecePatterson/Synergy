﻿using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RiverWatch_Windows_Phone_App;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;
using Windows.Storage;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace RiverWatchTest
{
    [TestClass]
    public class ReportTest
    {
        // Date Now
        DateTime dt = System.DateTime.Now;

        [TestMethod]
        public async Task GeoAfterImageTest()
        {            
            String date = dt.ToString("dd_MM_yyyy_H_mm_ss");
            Report r = new Report();
            var localStorage = ApplicationData.Current.LocalFolder;
            var file = await localStorage.CreateFileAsync("RiverWatchImage_" + date + ".jpg");
            r.setImage(file);
            while (!r.isGeolocationReady())
            {
                await Task.Delay(2000);
            }
            Assert.AreNotEqual(r.getLatitude(), "");
            Assert.AreNotEqual(r.getLongitude(), "");
        }

        [TestMethod]
        public async Task MinimalReportTest()
        {
            String date = dt.ToString("dd_MM_yyyy_H_mm_ss");
            Report r = new Report();
            var localStorage = ApplicationData.Current.LocalFolder;
            var file = await localStorage.CreateFileAsync("RiverWatchImage_" + date + ".jpg");
            r.setImage(file);            
           while (!r.isGeolocationReady())
           {
               await Task.Delay(2000);
           }
            Assert.AreEqual(r.isReportReady(), true);
        }

        [TestMethod]
        public void SetDescriptionTest()
        {
            Report r = new Report();
            r.setDescription("A Description");
            Assert.AreEqual(r.isDescriptionReady(), true);
        }

        [TestMethod]
        public void EmptyListSetTagsTest()
        {
            Report r = new Report();
            List<String> tags = new List<String>();
            r.setTags(tags);
            Assert.AreNotEqual(r.isTagsReady(), true);
        }

        [TestMethod]
        public void NonEmptyListSetTagsTest()
        {
            Report r = new Report();
            List<String> tags = new List<String>();
            tags.Add("Vandalism");
            r.setTags(tags);
            Assert.AreEqual(r.isTagsReady(), true);
        }

        [TestMethod]
        public async Task DiscardReportTest()
        {
            String date = dt.ToString("dd_MM_yyyy_H_mm_ss");
            Report r = new Report();
            var localStorage = ApplicationData.Current.LocalFolder;
            var file = await localStorage.CreateFileAsync("RiverWatchImage_" + date + ".jpg");
            r.setImage(file);
            while (!r.isGeolocationReady())
            {
                await Task.Delay(2000);
            }
            r.discardReport(true);
            try
            {
                StorageFile getfile = await (ApplicationData.Current.LocalFolder.GetFileAsync("RiverWatchImage_" + date + ".jpg"));
                await getfile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                Assert.Fail();
            }
            catch (FileNotFoundException e)
            {
                Assert.AreEqual(0,0);
            }
            
        }
    }
}
