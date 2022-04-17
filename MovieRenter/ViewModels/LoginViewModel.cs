using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MovieRenter;
using MovieRenter.Models;
using System.Windows;
using System.Security;

namespace MovieRenter.ViewModels
{
    class LoginViewModel : ViewModelBase
    {
        private string username;

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged("Username");
            }
        }

        private string password;

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged("Password");
            }
        }


        private ICommand _loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return _loginCommand ?? (_loginCommand = new CommandHandler(param => Login(param), true)) ;
            }
        }

        // ctor
        public LoginViewModel()
        {
            password = string.Empty;
            username = string.Empty;
        }

        // Try to login
        private void Login(object window)
        {
            if (!GlobalValidator.IsLoginValid(username, password))
                return;

            UserModel currMember = DBOperations.LoginUser(Username, Password).Result;

            // if there is a user with that username and password
            if (currMember != null)
            {
                // update if the user is admin or not
                if (currMember.IsAdmin)
                    CurrentUser.UserType = Users.Admin;
                else
                    CurrentUser.UserType = Users.Member;
                CurrentUser.User = currMember;
                // let the user know he is loged in, move him to the movies view and close the window. 
                MessageBox.Show("Welcome " + currMember.Firstname + " :)", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateViewCommand.GetUpdateViewInstance().Execute("Movies");
                ((Window)window).Close();
            }
            else
            {
                // if there wasn't a user with that username or password, let the user know with error message 
                MessageBox.Show("Incorrect Username or Password", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



    }
}
