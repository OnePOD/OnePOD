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
    class PodViewModel : INotifyPropertyChanged
    {
        private MainWindow mainWindow;
        private PodModel model;

        public PodViewModel(MainWindow mainWindow, PodModel model)
        {
            this.mainWindow = mainWindow;
            this.model = model;
            updateUI();
        }

#region UI properties
        public string Date { get { return model.CurrentPicture == null ? "" : model.CurrentPicture.Date; } }
        public string Title { get { return model.CurrentPicture == null ? "" : model.CurrentPicture.Title; } }
        public string Credit { get { return model.CurrentPicture == null ? "" : model.CurrentPicture.Credit; } }
        public string Detail { get { return model.CurrentPicture == null ? "" : model.CurrentPicture.Detail; } }
        private void updateUI()
        {
            OnPropertyChanged("Date");
            OnPropertyChanged("Title");
            OnPropertyChanged("Credit");
            OnPropertyChanged("Detail");
            showPictureLocal();
        }
        internal void showPictureLocal()
        {
            string filePath = model.GetPodPicPath();
            if (File.Exists(filePath))
            {
                Uri imagePath = new Uri(filePath);
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

#endregion

        internal void GetPictureToday()
        {
            //mainWindow.PodImage.Source = model.PodPicOnePath; // release the handle on the pic file
            model.GetPortalPage();
            string url = model.GetPicUrl(true);
            string filePath = model.GetPodPicPath();
            if (!File.Exists(filePath))
            {
                using (WebClient myWebClient = new WebClient())
                {
                    myWebClient.DownloadFile(url, filePath);
                }
            }
            updateUI();
        }


        internal void SetPictureToday()
        {
            PodInterop.SetImage(model.CurrentPicture.FilePath);
            MessageBox.Show("Set wallpaper successful.", "OnePOD");
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
                title + ": ~ The One Pic of the Day app to rule them all ~\n" +
                "Version: " + PodUtil.POD_VERSION + PodUtil.POD_BUILD + "\n" +
                "Contact: " + PodUtil.POD_CONTACT,
                title);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }
    }
}
