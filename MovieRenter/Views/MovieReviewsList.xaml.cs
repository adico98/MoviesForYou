using MovieRenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovieRenter.Views
{
    /// <summary>
    /// Interaction logic for MovieReviewsList.xaml
    /// </summary>
    public partial class MovieReviewsList : Window
    {
        public MovieReviewsList()
        {
            InitializeComponent();
        }



        public MovieReviewsList(int movieId)
        {
            InitializeComponent();
            this.DataContext = new MovieReviewsViewModel(movieId);
        }
    }
}
