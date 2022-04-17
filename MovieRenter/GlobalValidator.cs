using MovieRenter.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace MovieRenter
{
    public static class GlobalValidator
    {
        public static bool IsReviewValid(ReviewModel review)
        {
            if (review.Rating < 1 || review.Rating > 5)
            {
                MessageBox.Show("You need to rate the movie", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (review.Review == string.Empty || review.Review.Length > 2000)
            {
                MessageBox.Show("Review length need to be between 1-2000 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        public static bool IsNewUserValid(UserModel newUser)
        {
            newUser.Username = newUser.Username.ToLower();

            if (IsValidNameOrLastName(newUser.Firstname) && IsValidNameOrLastName(newUser.LastName) && IsPasswordValid(newUser.Password) && IsDateofbirth(newUser.DateOfBirth) &&
                IsEmailValid(newUser.Email) && IsUsernameValid(newUser.Username))
            {
                if (DBOperations.CheckIfUsernameExist(newUser.Username).Result)
                {
                    MessageBox.Show("The username already exist in the system", "Error", MessageBoxButton.OK);
                    return false;
                }
                if (DBOperations.CheckIfEmailExist(newUser.Email).Result)
                {
                    MessageBox.Show("The email address already exist in the system", "Error", MessageBoxButton.OK);
                    return false;
                }
                return true;
            }
            return false;


        }

        public static bool IsUpdatedUserValid(UserModel newUserDetails, string oldUsername, string oldEmail)
        {
            newUserDetails.Username = newUserDetails.Username.ToLower();

            if (IsValidNameOrLastName(newUserDetails.Firstname) && IsValidNameOrLastName(newUserDetails.LastName) && IsPasswordValid(newUserDetails.Password) && IsDateofbirth(newUserDetails.DateOfBirth))
            {
                if (oldUsername != newUserDetails.Username.ToLower())
                {
                    if (!IsUsernameValid(newUserDetails.Username))
                        return false;

                    if (DBOperations.CheckIfUsernameExist(newUserDetails.Username).Result)
                    {
                        MessageBox.Show("The username already exist in the system", "Error", MessageBoxButton.OK);
                        return false;
                    }


                }

                if (oldEmail != newUserDetails.Email)
                {
                    if (!IsEmailValid(newUserDetails.Email))
                        return false;

                    if (DBOperations.CheckIfEmailExist(newUserDetails.Email).Result)
                    {
                        MessageBox.Show("The email address already exist in the system", "Error", MessageBoxButton.OK);
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        public static bool IsLoginValid(string username, string password)
        {
            if (IsUsernameValid(username) && IsPasswordValid(password))
            {
                return true;
            }
            return false;

        }

        public static bool IsMovieValid(MovieModel movie)
        {
            if (string.IsNullOrEmpty(movie.Title) || string.IsNullOrEmpty(movie.Actors) || string.IsNullOrEmpty(movie.Plot) ||
     movie.Image == null || movie.MovieId <= 0)
            {
                MessageBox.Show("You need to fill all the movie details", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;

            }
            else if (movie.ReleaseYear > DateTime.Now.Year || movie.ReleaseYear < 1888)
            {
                MessageBox.Show("The release date of the movie need to be between 1888 (first movie ever made!) and this year", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        #region UserValidator

        private static bool IsValidNameOrLastName(string a_name)
        {
            if (a_name.Length < 2 || a_name.Length > 20)
            {
                MessageBox.Show("The name / last name need to be between 2-20 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!Regex.IsMatch(a_name, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("The name / last name need to contain only letters", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static bool IsUsernameValid(string a_username)
        {
            if (a_username.Length > 10 || a_username.Length < 2)
            {
                MessageBox.Show("The username need to be between 2-10 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static bool IsPasswordValid(string a_password)
        {
            if (!(Regex.IsMatch(a_password, @"[a-z]") && Regex.IsMatch(a_password, @"[A-Z]") && Regex.IsMatch(a_password, @"[0-9]") && a_password.Length >= 8))
            {
                MessageBox.Show("The password need to contain at least 1 lower case letter, 1 uppercase letter, 1 number and no less than 8 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static bool IsEmailValid(string email)
        {
            if (email.Length < 5 || email.Length > 50)
            {
                MessageBox.Show("The Email need to be between 2-10 chars", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (!(Regex.IsMatch(email, @"^[\w!#$%&'*+\-/=?\^_^`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`^{|}~]+)*"
+ "@"
+ @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$")))
            {
                MessageBox.Show("The Email is in invalid format ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static bool IsDateofbirth(DateTime date)
        {
            if (date > DateTime.Now)
            {
                MessageBox.Show("Birth date must be smaller than today's date", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else if (date.Year < DateTime.Now.Year - 125)
            {
                MessageBox.Show("You cannot be older than 125", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }
        #endregion
    }
}
