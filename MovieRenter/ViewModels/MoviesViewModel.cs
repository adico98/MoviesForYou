using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using MovieRenter.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Linq;

namespace MovieRenter.ViewModels
{
    public class MoviesViewModel : ViewModelBase
    {
        
        private List<MovieModel> allMoviesData;
        private List<MoviesGenreModel> allMoviesGenresData;
        private string filter;
        private int currGenreModel;
        private bool ageFilter;

        // ctor
        public MoviesViewModel()
        {
            MovieList = new ObservableCollection<MovieModel>();
            GenreList = new Dictionary<int, string>();

            GenreList.Add(0, "All Genres");
            currGenreModel = 0;

            // adding the movies and genres data to the List
            allMoviesData = DBOperations.GetAllMoviesData().Result;

            MovieList.Clear();
            foreach (var movie in allMoviesData)
            {
                if (CurrentUser.UserType == Users.Guest || DateTime.Now.AddYears(-movie.AgeRating) >= CurrentUser.User.DateOfBirth)
                {
                    MovieList.Add(movie);
                }
            }

            foreach (var genre in DBOperations.GetAllGenresData().Result)
            {
                GenreList.Add(genre.GenreId, genre.GenreName);
            }
        }

        public ObservableCollection<MovieModel> MovieList { get; set; }
        public Dictionary<int, string> GenreList { get; set; }

        // age filter, only shows that movies that can be rented according to the user age 
        public bool RemoveAgeFilter
        {
            get => ageFilter;
            set
            {
                ageFilter = value;
                Filters();
                OnPropertyChanged("AgeFilter");
            }
        }

        // filter the movies by genre, if the genre is 0, then shows all movies
        public int CurrGenre 
        {
            get { return currGenreModel; } 
            set
            {
                if (value == currGenreModel)
                    return;
                currGenreModel = value;
                Filters();
                OnPropertyChanged("CurrGenre");
            } 
        }

        // Search filter 
        public string Filter
        {
            get { return filter; }
            set
            {
                if (value == filter)
                    return;
                filter = value;
                OnPropertyChanged("Filter");
                Filters();
            }
        }

        public bool IsGuest { get => CurrentUser.UserType == Users.Guest; }

        // filter the movies shown by chosen filters - age rating, genre and search 
        private void Filters()
        {
            if (filter == null)
                filter = string.Empty;


            List<MovieModel> moviesList;

            // if the no genre was chosen get all movies, if genre was chosen get only movies that belong to that genre 
            if (currGenreModel == 0)
                moviesList = allMoviesData;
            else
                moviesList = GetMoviesIDForGenre(CurrGenre);

            // clear the movie list and go through each movie 
            MovieList.Clear();
            foreach (var movie in moviesList)
            {
                // check if  the filter is empty or if the movie title contain the search chars 
                if (filter == string.Empty || movie.Title.ToLower().Contains(Filter.ToLower()))
                {
                    // check if the age filter is on
                    if (RemoveAgeFilter)
                    {
                        // add the movie to the list 
                        MovieList.Add(movie);
                    }
                    else
                    {
                        // check if the user is a guest (no age limit) or if the user is old enough for that movie. 
                        if (CurrentUser.UserType == Users.Guest || DateTime.Now.AddYears(-movie.AgeRating) >= CurrentUser.User.DateOfBirth)
                        {
                            MovieList.Add(movie);
                        }
                    }
                }

            }
        }

        private List<MovieModel> GetMoviesIDForGenre(int genreId)
        {
            if (allMoviesGenresData == null)
                allMoviesGenresData = DBOperations.GetAllMoviesGenresData().Result;
            List<int> moviesIdList = new List<int>();
            foreach (var movieGenre in allMoviesGenresData)
            {
                if (movieGenre.GenreId == genreId)
                    moviesIdList.Add(movieGenre.MovieId);
            }
            return allMoviesData.FindAll(x => moviesIdList.Contains(x.MovieId));
        }


        private ICommand _selectMovieCommand;
        public ICommand SelectMovieCommand
        {
            get
            {
                return _selectMovieCommand ?? (_selectMovieCommand = new CommandHandler(param => SelectMovie(param), true));
            }
        }

        private ICommand addMovieCommand;
        public ICommand AddMovieCommand
        {
            get
            {
                return addMovieCommand ?? (addMovieCommand = new CommandHandler(() => AddMovie(), true));
            }
        }

        private void AddMovie()
        {
            // open the add movie dialog, if the movie was added (showDialog is true) then reload that movies data
            var addDialog = new AddOrUpdateMovieView();
            
            // creating new ViewModel, adding relevent data and connecting it to the window. 
            AddOrUpdateMovieViewModel vm = new AddOrUpdateMovieViewModel();
            vm.SelectedMovie.MovieId = allMoviesData.Last().MovieId + 1;
            vm.ReloadMoviesData += AddMovieEventHandler;
            addDialog.DataContext = vm;

            if ((bool)addDialog.ShowDialog())
            {

                MessageBox.Show("Movie Added Successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddMovieEventHandler(object sender, MovieModel movie)
        {
            MovieList.Add(movie);
        }

        private void SelectMovie(object o)
        {
            // when user select movie, change the view to the movie details 
            UpdateViewCommand.GetUpdateViewInstance().SetSelectedMovie((MovieModel)o);
            UpdateViewCommand.GetUpdateViewInstance().Execute("SelectedMovie");
        }
    }
}
