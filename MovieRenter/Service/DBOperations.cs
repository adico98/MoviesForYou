using MovieRenter.Models;
using MovieRenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;
using System.Windows;


namespace MovieRenter
{
    internal class DBOperations
    {
        static readonly string connectionString = Properties.Settings.Default.connString;

        #region MoviesAndGenres

        // Update movie and it's genres 
        public static bool UpdateMovie(MovieModel movie, int fGenre, int sGenre)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string updateString = "update dbo.Movies set " +
                                           "Title = @title, Plot = @plot, Actors = @actors, ReleaseYear = @releaseYear, Image = @image, AgeRating = @ageRating " +
                                           "where MovieId = @id";

                    SqlCommand updateCommand = new SqlCommand(updateString, connection);
                    updateCommand.Parameters.AddWithValue("@id", movie.MovieId);
                    updateCommand.Parameters.AddWithValue("@title", movie.Title);
                    updateCommand.Parameters.AddWithValue("@plot", movie.Plot);
                    updateCommand.Parameters.AddWithValue("@actors", movie.Actors);
                    updateCommand.Parameters.AddWithValue("@releaseYear", movie.ReleaseYear);
                    updateCommand.Parameters.AddWithValue("@image", movie.Image);
                    updateCommand.Parameters.AddWithValue("@ageRating", movie.AgeRating);


                    SqlCommand cmd = ChangeGenres(fGenre, sGenre, movie.MovieId, connection);

                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    updateCommand.Dispose();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // check how much genres need to add to the movie and create a command accordingly
        private static SqlCommand ChangeGenres(int fGenre, int sGenre, int movieId, SqlConnection connection)
        {
            try
            {
                SqlCommand cmd = connection.CreateCommand();

                if (fGenre == 0 && sGenre == 0)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "RemoveAllGenre";


                }
                else if (fGenre != 0 && sGenre != 0)
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "AddTwoGenres";
                    cmd.Parameters.Add("@fGenre", SqlDbType.Int).Value = fGenre;
                    cmd.Parameters.Add("@sGenre", SqlDbType.Int).Value = sGenre;

                }
                else // fGenre != 0 || sGenre != 0 
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "AddOneGenre";
                    cmd.Parameters.Add("@genre", SqlDbType.Int).Value = (fGenre == 0) ? sGenre : fGenre; ;
                }
                cmd.Parameters.Add("@movieId", SqlDbType.Int).Value = movieId;
                return cmd;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // Get All the Movies Genres From Database 
        public static async Task<List<MoviesGenreModel>> GetAllMoviesGenresData()
        {
            List<MoviesGenreModel> movieGenresList = new List<MoviesGenreModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.MoviesGenre";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        movieGenresList.Add(new MoviesGenreModel(Convert.ToInt32(reader["MovieId"]),
                           Convert.ToInt32(reader["GenreId"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
             
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            

            return movieGenresList;

        }

        // Add new movie and it's genres the to database
        public static bool AddMovie(MovieModel movie,  int fGenre, int sGenre)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string insertString = "insert into dbo.Movies " +
                                          "(MovieId, Title, Plot, Actors, ReleaseYear, Image, AgeRating)" +
                                          "values " +
                                          "(@id, @title, @plot, @actors, @releaseYear, @image, @ageRating)";

                    SqlCommand insertCommand = new SqlCommand(insertString, connection);
                    insertCommand.Parameters.AddWithValue("@id", movie.MovieId);
                    insertCommand.Parameters.AddWithValue("@title", movie.Title);
                    insertCommand.Parameters.AddWithValue("@plot", movie.Plot);
                    insertCommand.Parameters.AddWithValue("@actors", movie.Actors);
                    insertCommand.Parameters.AddWithValue("@releaseYear", movie.ReleaseYear);
                    insertCommand.Parameters.AddWithValue("@image", movie.Image);
                    insertCommand.Parameters.AddWithValue("@ageRating", movie.AgeRating);

                    SqlCommand cmd = ChangeGenres(fGenre, sGenre, movie.MovieId, connection);


                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    cmd.ExecuteNonQuery();
                    insertCommand.Dispose();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // Delete movie from table
        public static bool DeleteMovie(int movieId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "DeleteAMovie";
                    cmd.Parameters.Add("@movieId", SqlDbType.Int).Value = movieId;


                    connection.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // Get all Genres From database
        public static async Task<List<GenreModel>> GetAllGenresData()
        {
            List<GenreModel> genresList = new List<GenreModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Genres";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        genresList.Add(new GenreModel(
                           Convert.ToInt32(reader["GenreId"]),
                           reader["GenreName"].ToString()));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return genresList;
        }

        // Get all movies from database
        public static async Task<List<MovieModel>> GetAllMoviesData()
        {
            List<MovieModel> moviesIn = new List<MovieModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetAllMovies";

                     await connection.OpenAsync().ConfigureAwait(false);
                     SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        moviesIn.Add(new MovieModel(
                           Convert.ToInt32(reader["MovieID"]),
                           reader["Title"].ToString(),
                           reader["Plot"].ToString(),
                           reader["Actors"].ToString(),
                           Convert.ToInt32(reader["ReleaseYear"]),
                           (byte[])reader["Image"],
                           Convert.ToInt32(reader["AgeRating"]),
                           reader["Rating"] as int? ??
                           default(int),
                           reader["ReviewCount"] as int? ??
                           default(int)
                        ));
                    }

                    reader.Close();
                    
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            
            return moviesIn;

        }

        #endregion Movies

        #region RentedMovies

        // Rent movie to user
        public static bool RentMovie(int movieId, string username)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string insertString = "insert into dbo.RentedMovies " +
                                          "(MovieId, Username, DateRented, CanReview)" +
                                          "values " +
                                          "(@MovieIDFK, @CustIDFK, @DateRented, 1)";

                    SqlCommand insertCommand = new SqlCommand(insertString, connection);
                    insertCommand.Parameters.AddWithValue("@MovieIDFK", movieId);
                    insertCommand.Parameters.AddWithValue("@CustIDFK", username);
                    insertCommand.Parameters.AddWithValue("@DateRented", DateTime.Now);

                    if (insertCommand.ExecuteNonQuery() == -1)
                        return false;

                    insertCommand.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // Get all Currently rented movies from database
        // if the user is admin return the data of all the users, if not return the data of the curr user 
        public static async Task<List<RentedMovieModel>> GetAllCurrRentedMoviesData(bool isAdmin, string username)
        {
            List<RentedMovieModel> currRentedMovies = new List<RentedMovieModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (isAdmin)
                        cmd.CommandText = "GetCurrRentedMovies";
                    else
                    {
                        cmd.CommandText = "GetCurrUserRentedMovies";
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    }

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        currRentedMovies.Add(new RentedMovieModel(
                           Convert.ToInt32(reader["MovieID"]),
                           reader["Username"].ToString(),
                           reader["Firstname"].ToString(),
                           reader["Lastname"].ToString(),
                           reader["Title"].ToString(),
                           Convert.ToDateTime(reader["DateRented"]),
                           Convert.ToBoolean(reader["CanReview"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return currRentedMovies;

        }

        // Get all returned rented movies from database
        // if the user is admin return the data of all the users, if not return the data of the curr user 
        public static async Task<List<RentedMovieModel>> GetAllRentedMoviesData(bool isAdmin, string username)
        {
            List<RentedMovieModel> rentedMovies = new List<RentedMovieModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;

                    if (isAdmin)
                        cmd.CommandText = "GetAllRentedMovies";
                    else
                    {
                        cmd.CommandText = "GetAllUserRentedMovies";
                        cmd.Parameters.Add("@username", SqlDbType.VarChar).Value = username;
                    }

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        rentedMovies.Add(new RentedMovieModel(
                           Convert.ToInt32(reader["MovieId"]),
                           reader["Username"].ToString(),
                           reader["Firstname"].ToString(),
                           reader["Lastname"].ToString(),
                           reader["Title"].ToString(),
                           Convert.ToDateTime(reader["DateRented"]),
                           Convert.ToDateTime(reader["DateReturned"]),
                           Convert.ToBoolean(reader["CanReview"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return rentedMovies;

        }

        // Return movie 
        public static bool ReturnMovie(RentedMovieModel rentedMovieData)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string updateString = "update dbo.RentedMovies set DateReturned = GETDATE() " +
                                                      "where MovieId = @movieId and Username = @username and DateRented = @dateRented";

                    SqlCommand updateCommand = new SqlCommand(updateString, connection);
                    updateCommand.Parameters.AddWithValue("@movieId", rentedMovieData.MovieId);
                    updateCommand.Parameters.AddWithValue("@username", rentedMovieData.Username);
                    updateCommand.Parameters.AddWithValue("@dateRented", rentedMovieData.DateRented);

                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }


                return true;
            }
        }

        // Get List of most rented movies (movieId, movie title, renting amount) order by DESC 
        public static async Task<List<Tuple<int, string, int>>> GetMostRentedMovies()
        {
            List<Tuple<int, string, int>> mostRentedMovies = new List<Tuple<int, string, int>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetMostRentedMovies";


                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        mostRentedMovies.Add(new Tuple<int, string, int>(
                           Convert.ToInt32(reader["MovieId"]),
                           reader["Title"].ToString(),
                            Convert.ToInt32(reader["RentingAmount"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return mostRentedMovies;
        }

        // Get list of most rented genres (genreId, Genre name, renting amount) order by DESC
        public static async Task<List<Tuple<int, string, int>>> GetMostRentedGenres()
        {
            List<Tuple<int, string, int>> mostRentedGenres = new List<Tuple<int, string, int>>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GetMostRentedGenres";


                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        mostRentedGenres.Add(new Tuple<int, string, int>(
                           Convert.ToInt32(reader["GenreId"]),
                           reader["GenreName"].ToString(),
                            Convert.ToInt32(reader["RentingAmount"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return mostRentedGenres;
        }

        #endregion

        #region Users

        public static async Task<List<UserModel>> GetAllUsersData()
        {
            List<UserModel> usersList = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Users";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        usersList.Add(new UserModel(
                           reader["Username"].ToString(),
                           reader["Firstname"].ToString(),
                           reader["Lastname"].ToString(),
                           reader["Password"].ToString(),
                           reader["Email"].ToString(),
                           Convert.ToDateTime(reader["DateOfBirth"]),
                           Convert.ToBoolean(reader["IsAdmin"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return usersList;

        }

        public static bool AddUser(UserModel newUser)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    string insertString = "insert into dbo.Users " +
                                          "(Username, Firstname, Lastname, Password, Email, DateOfBirth, IsAdmin)" +
                                          "values " +
                                          "(@username, @Firstname, @lastname, @password, @email, @dateOfBirth, @isAdmin)";

                    SqlCommand insertCommand = new SqlCommand(insertString, connection);
                    insertCommand.Parameters.AddWithValue("@username", newUser.Username);
                    insertCommand.Parameters.AddWithValue("@Firstname", newUser.Firstname);
                    insertCommand.Parameters.AddWithValue("@lastname", newUser.LastName);
                    insertCommand.Parameters.AddWithValue("@password", newUser.Password);
                    insertCommand.Parameters.AddWithValue("@email", newUser.Email);
                    insertCommand.Parameters.AddWithValue("@dateOfBirth", newUser.DateOfBirth);
                    insertCommand.Parameters.AddWithValue("@isAdmin", newUser.IsAdmin);

                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Dispose();

                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                return true;
            }
        }

        public static bool UpdateUserInfo(UserModel user, string oldUsername)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {

                    string updateString = "update dbo.Users set " +
                                          "Username = @username, Firstname = @firstname, Lastname = @lastname, Password = @password, Email = @email, DateOfBirth = @dateOfBirth, IsAdmin = @isAdmin" +
                                          " Where Username = @oldUsername";

                    SqlCommand updateCommand = new SqlCommand(updateString, connection);
                    updateCommand.Parameters.AddWithValue("@username", user.Username);
                    updateCommand.Parameters.AddWithValue("@firstname", user.Firstname);
                    updateCommand.Parameters.AddWithValue("@lastname", user.LastName);
                    updateCommand.Parameters.AddWithValue("@password", user.Password);
                    updateCommand.Parameters.AddWithValue("@email", user.Email);
                    updateCommand.Parameters.AddWithValue("@dateOfBirth", user.DateOfBirth);
                    updateCommand.Parameters.AddWithValue("@isAdmin", user.IsAdmin);
                    updateCommand.Parameters.AddWithValue("@oldUsername", oldUsername);

                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            return true;

        }

        public static async Task<bool> CheckIfUsernameExist(string username)
        {
            bool isUsernameExist = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Users WHERE Username = '" + username + "'";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    if (reader.HasRows)
                        isUsernameExist = true;

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return isUsernameExist;

        }

        public static async Task<bool> CheckIfEmailExist(string email)
        {
            bool isEmailExist = false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Users WHERE Email = '" + email + "'";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    if (reader.HasRows)
                        isEmailExist = true;

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return isEmailExist;

        }

        public static async Task<UserModel> LoginUser(string username, string password)
        {
            UserModel currMember = null;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Users where Username = '" + username + "' and Password = '" + password + "'";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    if (reader.HasRows)
                    {

                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            currMember = new UserModel(
                              reader["Username"].ToString(),
                              reader["Firstname"].ToString(),
                              reader["Lastname"].ToString(),
                              reader["Password"].ToString(),
                              reader["Email"].ToString(),
                              Convert.ToDateTime(reader["DateOfBirth"]),
                              Convert.ToBoolean(reader["IsAdmin"]));
                        }
                    }
                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return currMember;
                
        }


        #endregion


        #region Reviews

        // Add revire. if the user adding the review is admin approve the review, otherwise don't 
        public static bool AddReview(bool isAdmin, ReviewModel review)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string insertString = "insert into dbo.Reviews " +
                                          "(MovieId, Username, MovieTitle, Date, Rating, Review, IsApprove)" +
                                          "values " +
                                          "(@movieId, @username, @movieTitle, @date, @rating, @review, @isAdmin)";

                    SqlCommand insertCommand = new SqlCommand(insertString, connection);
                    insertCommand.Parameters.AddWithValue("@movieId", review.MovieId);
                    insertCommand.Parameters.AddWithValue("@username", review.Username);
                    insertCommand.Parameters.AddWithValue("@movieTitle", review.MovieTitle);
                    insertCommand.Parameters.AddWithValue("@date", DateTime.Now);
                    insertCommand.Parameters.AddWithValue("@rating", review.Rating);
                    insertCommand.Parameters.AddWithValue("@review", review.Review);
                    insertCommand.Parameters.AddWithValue("@isAdmin", isAdmin == true ? 1 : 0)  ;

                    connection.Open();
                    insertCommand.ExecuteNonQuery();
                    insertCommand.Dispose();



                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // Get all reviews from database. 
        // if the param is true return all the reviews pending for approval, if its false return all the approve reviews 
        public static async Task<List<ReviewModel>> GetAllReviewsData(bool isPendingApproval)
        {
            List<ReviewModel> reviewsList = new List<ReviewModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT * FROM dbo.Reviews WHERE IsApprove= 0";
                    if (!isPendingApproval)
                        query = "SELECT * FROM dbo.Reviews WHERE IsApprove= 1";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };

                    await connection.OpenAsync().ConfigureAwait(false);
                    SqlDataReader reader = await cmd.ExecuteReaderAsync().ConfigureAwait(false);

                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        reviewsList.Add(new ReviewModel(
                           Convert.ToInt32(reader["MovieId"]),
                           reader["MovieTitle"].ToString(),
                           reader["Username"].ToString(),
                           Convert.ToDateTime(reader["Date"]),
                           Convert.ToInt32(reader["Rating"]),
                           reader["Review"].ToString(),
                           Convert.ToBoolean(reader["IsApprove"])));
                    }

                    reader.Close();
                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            return reviewsList;

        }

        // Change the review to approve 
        public static bool ApproveReview(ReviewModel review)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string updateString = "update dbo.Reviews set " +
                                          "IsApprove= 1" +
                                          " Where Username = @username and MovieId = @movieId";

                    SqlCommand updateCommand = new SqlCommand(updateString, connection);
                    updateCommand.Parameters.AddWithValue("@username", review.Username);
                    updateCommand.Parameters.AddWithValue("@movieId", review.MovieId);

                    connection.Open();
                    updateCommand.ExecuteNonQuery();
                    updateCommand.Dispose();

                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }
        }

        // Delete review from database 
        public static bool DeleteReview(ReviewModel review)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string deleteString = "delete from dbo.Reviews Where Username = @username and MovieId = @movieId";

                    SqlCommand deleteCommand = new SqlCommand(deleteString, connection);
                    deleteCommand.Parameters.AddWithValue("@username", review.Username);
                    deleteCommand.Parameters.AddWithValue("@movieId", review.MovieId);

                    connection.Open();
                    deleteCommand.ExecuteNonQuery();
                    deleteCommand.Dispose();

                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

                return true;
            }

        }

        #endregion

        #region Reports

        // Get the number of genres in the system 
        public static int GetAllGenresDataCount()
        {
            int genreCount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT count(*) as GenreCount FROM dbo.Genres";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    connection.Open();

                    genreCount = (int)cmd.ExecuteScalar();

                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return genreCount;
        }

        // Get the number of currently Rented movies in the system 
        public static int GetAllCurrRentedMoviesDataCount()
        {
            int currRentedMoviesCount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT count(*) FROM dbo.RentedMovies Where DateReturned is null";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    connection.Open();

                    currRentedMoviesCount = (int)cmd.ExecuteScalar();

                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return currRentedMoviesCount;
        }

        // Get the number of returned rented movies in the system 
        public static int GetAllRentedMoviesDataCount()
        {
            int rentedMoviesCount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT count(*) FROM dbo.RentedMovies Where DateReturned is not null";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    connection.Open();

                    rentedMoviesCount = (int)cmd.ExecuteScalar();

                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return rentedMoviesCount;
        }

        // Get the number of users in the system 
        public static int GetAllUsersDataCount()
        {
            int usersCount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT count(*) FROM dbo.Users";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    connection.Open();

                    usersCount = (int)cmd.ExecuteScalar();

                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return usersCount;
        }

        // Get the number of approve reviews in the system 
        public static int GetAllReviewsDataCount()
        {
            int reviewsCount = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    String query = "SELECT count(*) FROM dbo.Reviews Where IsApprove = 1";
                    SqlCommand cmd = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.Text
                    };
                    connection.Open();

                    reviewsCount = (int)cmd.ExecuteScalar();

                    cmd.Dispose();
                }
                catch (SqlException sqlException)
                {
                    CatchSqlConnectionExceptions(sqlException);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return reviewsCount;
        }

        #endregion

        private static void CatchSqlConnectionExceptions(SqlException sqlException)
        {
            var retry = MessageBox.Show($"An error occured during connection:\n{sqlException.Message}", "SQL Connection Error", MessageBoxButton.OK);
            
        }
    }
}
