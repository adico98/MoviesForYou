using MovieRenter.Models;
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
    public partial class AddOrUpdateMovieView : Window
    {
        public AddOrUpdateMovieView()
        {
            InitializeComponent();
        }

        public AddOrUpdateMovieView(MovieModel selectedMovie)
        {
            InitializeComponent();
            this.DataContext = new AddOrUpdateMovieViewModel(selectedMovie);
        }
    }
}
