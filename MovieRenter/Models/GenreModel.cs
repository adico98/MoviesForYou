using System;
using System.Collections.Generic;
using System.Text;

namespace MovieRenter.Models
{
    public class GenreModel
    {
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        
        public GenreModel(int genreId, string genreName)
        {
            GenreId = genreId;
            GenreName = genreName;
        }
    }
}
