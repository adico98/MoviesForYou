using MovieRenter;
using MovieRenter.Models;
using MovieRenter.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    class AddOrUpdateUserViewModel : ViewModelBase
    {
        private UserModel currMember;
        public event EventHandler<UserModel> AddUser;
        private bool isAdd = false;
        
        private string oldUsername;
        private string oldEmail;

        public UserModel CurrMember
        {
            get { return currMember; }
            set
            {


                currMember = value;
                OnPropertyChanged("CurrMember");
            }
        }

        public bool IsAdd { get => isAdd; }

        public string SaveButtonText { get; set; }

        public bool CanChangeField
        {
            get
            {
                // if the user is admin or adding a new user then it can change username. otherwise it can't 
                if (isAdd)
                    return true;
                else if (CurrentUser.User.IsAdmin)
                    return true;
                return false;
            }
        }

        public bool ChangeIsAdmin
        {
            get
            {
                if (CurrentUser.UserType == Users.Admin)
                    if(currMember.Username != CurrentUser.User.Username)
                        return true;
                return false;
            }
        }


        //ctor
        public AddOrUpdateUserViewModel()
        {
            currMember = new UserModel();
            isAdd = true;

            // if the user is a guest and he wants to sign up change the button to sign up instead of save 
            if (isAdd && CurrentUser.UserType == Users.Guest)
                SaveButtonText = "Sign up!";
            else
                SaveButtonText = "Save";

        }

        //ctor
        public AddOrUpdateUserViewModel(UserModel a_currMember)
        {
            currMember = new UserModel(a_currMember);
            oldUsername = a_currMember.Username;
            oldEmail = a_currMember.Email;
            SaveButtonText = "Save";
        }


        private ICommand saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                // saveCommand with this window object
                return saveCommand ?? (saveCommand = new CommandHandler(windowParam => SaveUser(windowParam), true));
            }
        }

        // save or update user 
        private void SaveUser(object window)
        {
            if (IsAdd)
            {
                // save user in data base, close the window and update the dialog result that the user didn't close the window 
                if (GlobalValidator.IsNewUserValid(currMember) && DBOperations.AddUser(currMember))
                {
                    ((Window)window).DialogResult = true;
                    ((Window)window).Close();
                    AddUser?.Invoke(this, currMember);
                    // need to change to login
                }
            } 
            else
            {
                // update the user in the database, close the window and update the dialog results that the user didn't close the window 
                if (GlobalValidator.IsUpdatedUserValid(currMember, oldUsername, oldEmail) && DBOperations.UpdateUserInfo(currMember, oldUsername))
                {
                    if (currMember.Username == CurrentUser.User.Username)
                    {
                        CurrentUser.User = currMember;
                    }
                    ((Window)window).DialogResult = true;
                    ((Window)window).Close();
                }
            }

        }
    }
}
