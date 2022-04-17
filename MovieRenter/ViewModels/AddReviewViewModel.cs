using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class AddReviewViewModel : ViewModelBase
    {
        private RentedMovieModel currRentedMovie;
        private string movieName;
        private int movieRating;
        private string reviewDesc;

        // ctor 
        public AddReviewViewModel(RentedMovieModel rentedMovie)
        {
            currRentedMovie = rentedMovie;
            movieName = currRentedMovie.Title;
            reviewDesc = string.Empty;
        }

        // need empty ctor for view, will not be used otherwise
        public AddReviewViewModel()
        {

        }

        public string MovieName { get => movieName;}
        public int MovieRating
        {
            get => movieRating;
            set
            {
                movieRating = value;
                OnPropertyChanged("MovieRating");
            }
        }
        public string ReviewDesc
        {
            get => reviewDesc;
            set
            {
                reviewDesc = value;
                OnPropertyChanged("ReviewDesc");
            }
        }

        private ICommand saveReviewCommand;
        public ICommand SaveReviewCommand
        {
            get
            {
                return saveReviewCommand ?? (saveReviewCommand = new CommandHandler(param => SaveReview(param), true));
            }
        }

        private void SaveReview(object param )
        {
            // add the review, and check if it went ok
            ReviewModel review = new ReviewModel(currRentedMovie.MovieId, movieName, currRentedMovie.Username, MovieRating, reviewDesc);
            if (GlobalValidator.IsReviewValid(review)) 
            {
                if(DBOperations.AddReview(CurrentUser.UserType == Users.Admin ,review))
                {
                    // let the user know the review is waiting for admin approval and change it so the user cannot review this movie
                    // close the window 
                    if (CurrentUser.UserType != Users.Admin)
                        MessageBox.Show("Your review is pending for admin approval!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    currRentedMovie.CanReview = false;

                    ((Window)param).DialogResult = true;
                    ((Window)param).Close();
                }
            }
        }
    }
}
