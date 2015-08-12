using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace OnePOD.Model
{
    class Picture : INotifyPropertyChanged
    {
        private FileInfo fileInfo; 
        private Uri uri;
        private ImageSource imageSource;

        public string Name;      // file name
        public string Directory; // file local path
        public string Uri;       // file remote uri

        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                OnPropertyChanged("Date");
            }
        }

        private string _title;
        public string Title 
        { 
            get { return _title; } 
            set 
            { 
                _title = value;
                OnPropertyChanged("Title");
            } 
        }

        private string _credit;
        public string Credit
        {
            get { return _credit; }
            set
            {
                _credit = value;
                OnPropertyChanged("Credit");
            }
        }

        private string _detail;
        public string Detail 
        { 
            get { return _detail; } 
            set 
            { 
                _detail = value;
                OnPropertyChanged("Detail");
            } 
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }
        }

    }
}
