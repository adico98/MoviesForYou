using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using MovieRenter.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class SelectedMovieViewModel :  ViewModelBase
    {
        public MovieModel selectedMovie;
        private List<String> usersList;
        private string selectedUsername;
        private bool canRentMovie;
        private string genres;

        // ctor 
        public SelectedMovieViewModel(MovieModel a_selectedMovie)
        {
            SelectedMovie = a_selectedMovie;

            // if the user is admin, get all the users username so he can rent the movie to a different user 
            if (CurrentUser.UserType == Users.Admin)
            {
                usersList = DBOperations.GetAllUsersData().Result.Select(e => new string( e.Username)).Where(e => e != CurrentUser.User.Username).ToList();
            }

            
            // check if the user is guest or if he is younger than the age rating. 
            // if so don't let him rent the movie 
            if (CurrentUser.UserType == Users.Guest)
                canRentMovie = false;
            else
                canRentMovie = DateTime.Now.AddYears(-selectedMovie.AgeRating) >= CurrentUser.User.DateOfBirth;

            // Get the movies genres
            var GenresList = DBOperations.GetAllMoviesGenresData().Result.FindAll(x => x.MovieId.Equals(selectedMovie.MovieId));
            List<GenreModel> GenreData = DBOperations.GetAllGenresData().Result;

            if (GenresList.Count == 0)
            {
                genres = "This Movie Don't have Genres Yet";
            }
            else
            {
                foreach (var genre in GenresList)
                {
                    genres += GenreData.Find(x => x.GenreId == genre.GenreId).GenreName + "\n";
                }
            }

        }

        public MovieModel SelectedMovie
        {
            get { return selectedMovie; }
            set
            {
                selectedMovie = value;
                OnPropertyChanged("SelectedMovie");
            }
        }

        
        public List<string> UsersList
        {
            get { return usersList; }
        }

        public string SelectedUser
        {
            get => selectedUsername;
            set
            {
                selectedUsername = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public bool CanRentMovie { get => canRentMovie; }

        // If the user is younger than the age rating or is a guest show him a messaged telling him the age restrictions
        public Visibility MinAgeVis 
        {
            get
            {
                if (canRentMovie || CurrentUser.UserType == Users.Guest)
                    return Visibility.Hidden;
                return Visibility.Visible;
            }
        }

        public string Genres
        {
            get => genres;
        }

       

        private ICommand rentMovieCommand;
        public ICommand RentMovieCommand
        {
            get
            {
                return rentMovieCommand ?? (rentMovieCommand = new CommandHandler(() => RentMovie(), true));
            }
        }

        private ICommand updateMovieCommand;
        public ICommand UpdateMovieCommand
        {
            get
            {
                return updateMovieCommand ?? (updateMovieCommand = new CommandHandler(() => UpdateMovie(), true));
            }
        }      
        
        private ICommand deleteMovieCommand;
        public ICommand DeleteMovieCommand
        {
            get
            {
                return deleteMovieCommand ?? (deleteMovieCommand = new CommandHandler(() => DeleteMovie(), true));
            }
        }

        private ICommand reviewsCommand;
        public ICommand ReviewsCommand
        {
            get
            {
                return reviewsCommand ?? (reviewsCommand = new CommandHandler(() => MovieReviews(), true));
            }
        }

        // check if the movies has been rated, or if he has no reviews
        public bool IsRated
        {
            get
            {
                return selectedMovie.Rating == 0;
            }
        }

        // if the user is admin, he can update the movie details (beside the movieId)
        // if the admin update the movie successfully then reload the selected movie details 
        private void UpdateMovie()
        {
            
            var addDialog = new AddOrUpdateMovieView(SelectedMovie);
            ((AddOrUpdateMovieViewModel)addDialog.DataContext).ReloadMoviesData += UpdateMovieEventHandler;
            addDialog.ShowDialog();

        }

        private void UpdateMovieEventHandler(Object sender, MovieModel movie)
        {
            SelectedMovie = movie;
        }

        // if user is admin, he can delete the movie. 
        // if the movie get deleted successfully then the user is transfer to the movies view
        private void DeleteMovie()
        {
            if (DBOperations.DeleteMovie(selectedMovie.MovieId))
                UpdateViewCommand.GetUpdateViewInstance().Execute("Movies");
        }

        // Rent the movie for the user and change the view to rented movies
        private void RentMovie()
        {
            var username = CurrentUser.User.Username;

            // if user is admin and he chose a different user to rent the movie to , change the username to the user the admin chose 
            if (CurrentUser.UserType == Users.Admin)
                if (SelectedUser != null)
                    username = selectedUsername;
                else
                    return;
            
                List<RentedMovieModel> currRentedMovies = DBOperations.GetAllCurrRentedMoviesData(CurrentUser.UserType == Users.Admin, CurrentUser.User.Username).Result;

                if (currRentedMovies.Find(x => x.Username == username && x.MovieId == selectedMovie.MovieId) == null ? false : true)
                {
                    MessageBox.Show("You're already renting this movie", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                int currRentedMoviesAmount = currRentedMovies.FindAll(x => x.Username == username).Count;
                int returnedMoviesAmount = DBOperations.GetAllRentedMoviesData(CurrentUser.UserType == Users.Admin, CurrentUser.User.Username).Result.
                                                FindAll(x => ((x.DateRented - DateTime.Now).TotalDays < 7 || (x.DateReturned - DateTime.Now).TotalDays < 7)
                                                                              && x.Username == username).Count;

                if (currRentedMoviesAmount >= 5)
                {
                    MessageBox.Show("You Can't Rent More Than 5 movies at the same time", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (currRentedMoviesAmount + returnedMoviesAmount >= 5)
                {
                    MessageBox.Show("You Can't Rent More Than 5 movies a week ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            

            if (DBOperations.RentMovie(selectedMovie.MovieId, username))
                 UpdateViewCommand.GetUpdateViewInstance().Execute("RentedMovies");
        }

        // open dialog where user can see the movie reviews 
        private void MovieReviews()
        {
            var addDialog = new MovieReviewsList(selectedMovie.MovieId);
            addDialog.ShowDialog();
        }
    }
}
