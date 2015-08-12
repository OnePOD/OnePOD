using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Windows;
using System.ComponentModel;
using OnePOD.Model;

namespace OnePOD
{
    class PodViewModel 
    {
        private MainWindow mainWindow;
        private PodModel model;

        public Picture CurrentPicture
        {
            get { return model.CurrentPicture; }
        }

        public PodViewModel(MainWindow mainWindow, PodModel model)
        {
            this.mainWindow = mainWindow;
            this.model = model;
            showPictureLocal();
        }

#region UI properties
        //private string _date;
        //public string Date { 
        //    get { return model.Date; }
        //    set
        //    {
        //        _date = value;
        //        OnPropertyChanged("Date");
        //    }
        //}
        //public string Title { get { return model.Title; } }
        //public string Detail { get { return model.Detail; } }
#endregion

        internal void GetPictureToday()
        {
            //mainWindow.PodImage.Source = model.PodPicOnePath; // release the handle on the pic file
            model.GetPortalPage();
            string url = model.GetPicUrl(true);
            string filePath = model.GenerateNewPodPicPath();
            using (WebClient myWebClient = new WebClient())
            {
                myWebClient.DownloadFile(url, filePath);
            }
            showPictureLocal();
        }

        internal void showPictureLocal()
        {
            if (File.Exists(model.PodPicPath))
            {
                Uri imagePath = new Uri(model.PodPicPath);
                BitmapImage bitmapImage = null;
                try
                {
                    bitmapImage = new BitmapImage(imagePath);
                }
                catch (FileNotFoundException e)
                {
                }
                catch (NotSupportedException e)
                { 
                }
                if (bitmapImage != null)
                    mainWindow.PodImage.Source = bitmapImage;
            }
        }

        internal void SetPictureToday()
        {
            PodInterop.SetImage(model.PodPicPath);
        }

        internal void OpenOnWeb()
        {
            var portalUrl = model.DefaultPortalUrl;
            System.Diagnostics.Process.Start(portalUrl);
        }

        internal void ShowAbout()
        {
            string title = PodUtil.POD_TITLE;
            MessageBox.Show(
                title + ": ~ The One Pic Of Day app to rule them all ~\n" +
                "Version: " + PodUtil.POD_VERSION + "\n" +
                "Contact: " + PodUtil.POD_CONTACT,
                title);
        }

    }
}
