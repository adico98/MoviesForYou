using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Views;
using System.Windows;

namespace MovieRenter.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _selectedViewModel;
        public ViewModelBase SelectedViewModel
        {
            get { return _selectedViewModel; }
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged(nameof(SelectedViewModel));
            }
        }

        // when the user change, update the menu
        public void onCurrentUserChange()
        {
            OnPropertyChanged(nameof(CurrUser));
        }

        public Users CurrUser
        {
            get => CurrentUser.UserType;
        }

        public ICommand UpdateCurrViewCommand { get; set; }


        private ICommand _logoutCommand;
        public ICommand LogoutCommand
        {
            get
            {
                return _logoutCommand ?? (_logoutCommand = new CommandHandler(() => Logout(), true));
            }
        }        
        
        private ICommand viewProfileCommand
;
        public ICommand ViewProfileCommand
        {
            get
            {
                return viewProfileCommand ?? (viewProfileCommand = new CommandHandler(() => ViewProfile(), true));
            }
        }
        
        private ICommand signupCommand;
        public ICommand SignupCommand
        {
            get
            {
                return signupCommand ?? (signupCommand = new CommandHandler(() => Signup(), true));
            }
        }

        private ICommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                return loginCommand ?? (loginCommand = new CommandHandler(() => Login(), true));
            }
        }

        // ctor 
        public MainWindowViewModel()
        {
            // create a new updateCurrViewCommand that change the views and set the first view to be the entrance 
            UpdateCurrViewCommand = UpdateViewCommand.GetUpdateViewInstance(this);
            SelectedViewModel = new EntranceViewModel();        
        }

        private void Logout()
        {
            // log out of the system and move back to the entrance view
            CurrentUser.UserType = Users.Guest;
            CurrentUser.User = null;
            UpdateViewCommand.GetUpdateViewInstance().Execute("Entrance");

        }

        public void Signup()
        {
            // open the sign up window and let the user know if he signed up successfuly 
            var addDialog = new AddOrUpdateUserView();
            if ((bool)addDialog.ShowDialog())
            {
                MessageBox.Show("You Have Signed up Successfully, You can now login to the system!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            } 
        }   
        
        private void Login()
        {
            // open the login window 
            var addDialog = new LoginView();
            addDialog.ShowDialog();
        }
        public void ViewProfile()
        {
            // open the profile window, if the any user details changed go to the movies view to load everything 
            var addDialog = new AddOrUpdateUserView(CurrentUser.User);
            if ((bool)addDialog.ShowDialog())
            {
                MessageBox.Show("Your profile updated successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                if (CurrentUser.UserType == Users.Admin)
                {
                    if (SelectedViewModel is UsersViewModel)
                    {
                        ((UsersViewModel)SelectedViewModel).UpdateUsersList();
                    }
                }
            }
        }
    }

    public static class CurrentUser
    {
        private static Users userType = Users.Guest;
        public static Users UserType {
            get => userType;
            set
            {
                userType = value;
                UpdateViewCommand.GetUpdateViewInstance().UpdateCurrentUser();
            }
        }
        private static UserModel user;

        public static UserModel User
        {
            get => user;
            set
            {
                user = value;
            }
        }
    }

    public enum Users
    {
        Admin,
        Member,
        Guest
    }
}
