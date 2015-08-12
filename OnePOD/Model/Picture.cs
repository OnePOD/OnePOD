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
        private Uri _uri;
        public string Uri       // file remote uri
        {
            get 
            {
                if (_uri == null)
                    return null;
                return _uri.ToString(); 
            }
            set
            { 
                _uri = new Uri(value);
                SourceDomain = _uri.Host;
                string[] segments = _uri.Segments;
                FileName = segments[segments.Length - 1];
            }
        }

        public string SourceDomain; // domain name of the source website
        public string DateShort; // e.g. 20150812
        
        private FileInfo fileInfo; 
        private ImageSource imageSource;

        public string FileName;      // file name
        public string FilePath; // file local path

        private string _date;
        public string Date
        {
            get { return _date; }
            set
            {
                _date = value;
                DateTime date;
                if (DateTime.TryParse(_date, out date))
                {
                    DateShort = PodUtil.GetDateShort(date);
                }
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
