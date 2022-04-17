using MovieRenter.Models;
using MovieRenter.ViewModels;
using System.Windows;
namespace MovieRenter.Views
{
    /// <summary>
    /// Interaction logic for AddReviewView.xaml
    /// </summary>
    public partial class AddReviewView : Window
    {
        public AddReviewView()
        {
            InitializeComponent();
        }

        public AddReviewView(RentedMovieModel rentedMovie)
        {
            InitializeComponent();
            this.DataContext = new AddReviewViewModel(rentedMovie);
        }
    }
}
