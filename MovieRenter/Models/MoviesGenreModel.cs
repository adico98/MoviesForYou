using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class MoviesGenreModel
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }

        public MoviesGenreModel (int movieId, int genreId)
        {
            MovieId = movieId;
            GenreId = genreId;
        }
    }
}
