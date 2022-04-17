using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using MovieRenter.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class RentedMoviesViewModel : ViewModelBase
    {
        // ctor
        public RentedMoviesViewModel()
        {
            // load the user details to the rented movies data and load the rentedmovies 
            CurrRentedMoviesList = new ObservableCollection<RentedMovieModel>();
            RentedMoviesList = new ObservableCollection<RentedMovieModel>();
            GetAllRentedMovies();
        }

        public ObservableCollection<RentedMovieModel> CurrRentedMoviesList { get; set; }
        public ObservableCollection<RentedMovieModel> RentedMoviesList { get; set; }

        // return movie command 
        private ICommand returnCommand;
        public ICommand ReturnCommand
        {
            get
            {
                return returnCommand ?? (returnCommand = new CommandHandler(param => ReturnMovie(param), true));
            }
        }

        // add review to a movie command 
        private ICommand reviewCommand;
        public ICommand ReviewCommand
        {
            get
            {
                return reviewCommand ?? (reviewCommand = new CommandHandler(param => ReviewMovie(param), true));
            }
        }

        // Get all the returned rented movies 
        private void GetAllRentedMovies()
        {
           // allRentedMoviesData = RentedMoviesData.GetRentedMovieDataInstance().GetReturnedRentedMovies();
            RentedMoviesList.Clear();
            foreach (var rentedMovie in DBOperations.GetAllRentedMoviesData(CurrentUser.User.IsAdmin, CurrentUser.User.Username).Result)
            {
                RentedMoviesList.Add(rentedMovie);
            }

           // allCurrRentedMoviesData = RentedMoviesData.GetRentedMovieDataInstance().GetCurrRentedMovies();
            CurrRentedMoviesList.Clear();
            foreach (var rentedMovie in DBOperations.GetAllCurrRentedMoviesData(CurrentUser.User.IsAdmin, CurrentUser.User.Username).Result)
            {
                CurrRentedMoviesList.Add(rentedMovie);
            }
        }

        // return a movie, and update the view 
        private void ReturnMovie(object param)
        {
            if (DBOperations.ReturnMovie((RentedMovieModel)param))
            {
                CurrRentedMoviesList.Remove(((RentedMovieModel)param));
                ((RentedMovieModel)param).DateReturned = DateTime.Now;
                RentedMoviesList.Add(((RentedMovieModel)param));
            }

        }

        // add a review to a movie, if the review was added update the view
        private void ReviewMovie(object param)
        {
            var addDialog = new AddReviewView(((RentedMovieModel)param));
            if ((bool)addDialog.ShowDialog())
            {
                GetAllRentedMovies();
            }
        }
    }
}
