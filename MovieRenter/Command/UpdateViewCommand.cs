using MovieRenter.Models;
using MovieRenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace MovieRenter.Command
{
    public class UpdateViewCommand : ICommand
    {
        private static UpdateViewCommand ldinstance = null;
        private static readonly object padlock = new object();
        private MovieModel selectedMovie;

        // Get class instance and update it's view model if the instance is null
        public static UpdateViewCommand GetUpdateViewInstance(MainWindowViewModel viewModel)
        {
            lock (padlock)
            {
                if (ldinstance == null)
                {
                    ldinstance = new UpdateViewCommand(viewModel);
                }

                return ldinstance;
            }
        }

        // Get class instance 
        public static UpdateViewCommand GetUpdateViewInstance()
        {
            lock (padlock)
            {
                return ldinstance;
            }
        }
        private MainWindowViewModel viewModel;

        // Update the view model of the class 
        private UpdateViewCommand(MainWindowViewModel viewModel)
        {
            this.viewModel = viewModel;
        }

        // Update that the user that use the app has changed 
        public void UpdateCurrentUser()
        {
            this.viewModel.onCurrentUserChange();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        // Set selected movie 
        public void SetSelectedMovie(MovieModel movieModel)
        {
            selectedMovie = movieModel;
        }

        // change the user control that is being presented in the app and it's view model 
        public void Execute(object parameter)
        {
            if (parameter.ToString() == "Reviews")
            {
                viewModel.SelectedViewModel = new UsersReviewsViewModel();
            } else if (parameter.ToString() == "Movies")
            {
                 viewModel.SelectedViewModel = new MoviesViewModel();
            } else if (parameter.ToString() == "SelectedMovie")
            {
                viewModel.SelectedViewModel = new SelectedMovieViewModel(selectedMovie);
            } else if (parameter.ToString() == "Entrance" )
            {
                viewModel.SelectedViewModel = new EntranceViewModel();
            } else if (parameter.ToString() == "Users")
            {
                viewModel.SelectedViewModel = new UsersViewModel();
            } else if (parameter.ToString() == "RentedMovies")
            {
                viewModel.SelectedViewModel = new RentedMoviesViewModel();
                
            } else if (parameter.ToString() == "Reports")
            {
                viewModel.SelectedViewModel = new ReportsViewModel();
            }
        }
    }
}
