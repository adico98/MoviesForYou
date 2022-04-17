using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class ReviewModel
    {
        public int MovieId { get; set; }
        public string MovieTitle { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
        public string Review { get; set; }
        public bool IsApprove { get; set; }

        public ReviewModel(int movieId, string movieTitle, string username, DateTime date, int rating, string review, bool isApprove)
        {
            MovieId = movieId;
            MovieTitle = movieTitle;
            Username = username;
            Date = date;
            Rating = rating;
            Review = review;
            IsApprove = isApprove ;
        }

        public ReviewModel(int movieId, string movieTitle, string username, int rating, string review)
        {
            MovieId = movieId;
            MovieTitle = movieTitle;
            Username = username;
            Rating = rating;
            Review = review;
            IsApprove = false;
        }
    }
}
