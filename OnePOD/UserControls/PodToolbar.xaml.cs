using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OnePOD
{
    /// <summary>
    /// Interaction logic for PodToolbar.xaml
    /// </summary>
    public partial class PodToolbar : UserControl
    {
        internal PodViewModel ViewModel 
        { 
            get { return this.DataContext as PodViewModel; } 
            //set
            //{
            //    _viewModel = value;
            //    this.DataContext = _viewModel;
            //}
        }
        public PodToolbar()
        {
            InitializeComponent();
        }

        private void buttonGet_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.GetPictureToday();
        }

        private void buttonSet_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SetPictureToday();
        }

        private void buttonWeb_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.OpenOnWeb();
        }

        private void buttonAbout_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.ShowAbout();
        }

    }
}
