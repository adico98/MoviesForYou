using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class RentedMovieModel
    {
        public int MovieId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Title { get; set; }
        public DateTime DateRented { get; set; }
        public DateTime DateReturned { get; set; }
        public bool CanReview { get; set; }
            
        public RentedMovieModel(int movieId, string username, string fName, string lName, string title, DateTime dateRented, DateTime dateReturned, bool canReview)
        {
            MovieId = movieId;
            Username = username;
            Firstname = fName;
            Lastname = lName;
            Title = title;
            DateRented = dateRented;
            DateReturned = dateReturned;
            CanReview = canReview;
            }

        public RentedMovieModel(int movieId, string username, string fName, string lName, string title, DateTime dateRented, bool canReview)
        {
            MovieId = movieId;
            Username = username;
            Firstname = fName;
            Lastname = lName;
            Title = title;
            DateRented = dateRented;
            CanReview = canReview;
        }
    }
    /*
    public class CurrRentedMovieModel
    {
        public int MovieId { get; set; }
        public string Username { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Name { get; set; }
        public DateTime DateRented { get; set; }

        public bool IsReviewed { get; set; }

        public CurrRentedMovieModel(int movieId, string username, string fName, string lName, string name,  DateTime dateRented, bool isReviewed)
        {
            MovieId = movieId;
            Username = username;
            Firstname = fName;
            Lastname = lName;
            Name = name;
            DateRented = dateRented;
            IsReviewed = isReviewed;
        }
    }
    */
}
