using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class MovieModel
    {
        public int MovieId { get; set; }
        public string Title { get; set; }
        public string Plot { get; set; }
        public string Actors { get;
            set; }
        public int ReleaseYear { get; set; }
        public byte[] Image { get; set; }
        public int AgeRating { get; set; }

        public int Rating { get; set; }

        public int ReviewCount { get; set; }

        public MovieModel()
        {
            ReleaseYear = DateTime.Now.Year;
        }

        public MovieModel(MovieModel copyMovie)
        {
            MovieId = copyMovie.MovieId;
            Title = copyMovie.Title;
            Plot = copyMovie.Plot;
            Actors = copyMovie.Actors;
            ReleaseYear = copyMovie.ReleaseYear;
            Image = copyMovie.Image;
            AgeRating = copyMovie.AgeRating;
        }

        public MovieModel(int movieId, string title, string plot, string actors, int releaseYear, byte[] image, int ageRating, int rating, int reviewCount)
        {
            MovieId = movieId;
            Title = title;
            Plot = plot;
            Actors = actors;
            ReleaseYear = releaseYear;
            Image = image;
            AgeRating = ageRating;
            Rating = rating;
            ReviewCount = reviewCount;
        }

    }
}
