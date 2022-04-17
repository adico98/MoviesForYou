using MovieRenter.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.ViewModels
{
    public class MovieReviewsViewModel : ViewModelBase
    {
        private List<ReviewModel> allMovieReviews;

        // get all the reviews from the movieId
        public MovieReviewsViewModel(int movieId)
        {
            allMovieReviews = DBOperations.GetAllReviewsData(false).Result.FindAll(x => x.MovieId == movieId);
        }

        // empty ctor fro view
        public MovieReviewsViewModel()
        {

        }

        public List<ReviewModel> AllMovieReviews
        {
            get => allMovieReviews;
        }
    }
}
