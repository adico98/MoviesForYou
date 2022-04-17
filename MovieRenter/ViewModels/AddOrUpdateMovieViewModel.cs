using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MovieRenter.ViewModels
{
    class AddOrUpdateMovieViewModel : ViewModelBase
    {
        private MovieModel selectedMovie;
        private List<GenreModel> allGenresData;
        public event EventHandler<MovieModel> ReloadMoviesData;
        private int currGenre1;
        private int currGenre2;

        private bool isAdd = false;

        public int CurrGenre1
        {
            get { return currGenre1; }
            set
            {
                currGenre1 = value;
                OnPropertyChanged("currGenre1");
            }
        }

        public int CurrGenre2
        {
            get { return currGenre2; }
            set
            {
                currGenre2 = value;
                OnPropertyChanged("CurrGenre");
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

        public bool IsAdd { get => isAdd; }

        public Dictionary<int, string> GenreList { get; set; }

        // ctor
        public AddOrUpdateMovieViewModel()
        {
            selectedMovie = new MovieModel();
            LoadGenres();
            isAdd = true;
            
        }

        // load the genres data, set the current genres to not chosen and update the genres list
        private void LoadGenres()
        {
            allGenresData = DBOperations.GetAllGenresData().Result;

            GenreList = new Dictionary<int, string>();
            GenreList.Add(0, "No Genres");

            CurrGenre1 = 0;
            CurrGenre2 = 0;

            foreach (var genre in allGenresData)
            {
                GenreList.Add(genre.GenreId, genre.GenreName);
            }
        }

        // ctor
        public AddOrUpdateMovieViewModel(MovieModel a_selectedMovie)
        {
            selectedMovie = new MovieModel(a_selectedMovie);
            // load the genre list
            LoadGenres();

            // Get the movie genres, if he have some
            var genreList = DBOperations.GetAllMoviesGenresData().Result.FindAll(x => x.MovieId.Equals(selectedMovie.MovieId));

            // if the movie has genre/genres, show it in the window 
            if (genreList.Count == 1)
                currGenre1 = genreList[0].GenreId;
            else if (genreList.Count == 2)
            {
                currGenre1 = genreList[0].GenreId;
                currGenre2 = genreList[1].GenreId;
            }
        }

        private ICommand addImageCommand;
        public ICommand AddImageCommand
        {
            get
            {
                return addImageCommand ?? (addImageCommand = new CommandHandler(() => LoadImage(), true));
            }
        }

        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                return saveCommand ?? (saveCommand = new CommandHandler(windowParam => SaveMovie(windowParam), true));
            }
        }

        private void SaveMovie(object window)
        {
            // check that all the fileds are not null or empty and that the release date is not in the future
            if (GlobalValidator.IsMovieValid(selectedMovie))
            {
                if (!isAdd)
                {
                    // update the movie in the databse, if false, then the movie was not updateed or there was an error
                    if (!DBOperations.UpdateMovie(selectedMovie, CurrGenre1, CurrGenre2))
                        return;
                }
                else
                {
                    // Add the movie to the database, if false, then the movie was not added or there was an error
                    if (!DBOperations.AddMovie(selectedMovie, CurrGenre1, CurrGenre2))
                        return;
                }

                ReloadMoviesData?.Invoke(this, selectedMovie);

                // close the window
                ((Window)window).DialogResult = true;
                ((Window)window).Close();
            }
        }

        // Load image from the computer for the movie image 
        private void LoadImage()
        {
            
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPG Files (*.jpg)|*.jpg|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                // Read the image and upload it to the image var in selectedMovie
                SelectedMovie.Image = File.ReadAllBytes(filename);
                OnPropertyChanged("SelectedMovie");
            }

            
        }
    }
}
