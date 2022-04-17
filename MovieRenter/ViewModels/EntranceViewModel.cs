using MovieRenter.Command;
using MovieRenter.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class EntranceViewModel : ViewModelBase
    {
        public ICommand UpdateCurrViewCommand { get; set; }

        public EntranceViewModel()
        {
            UpdateCurrViewCommand = UpdateViewCommand.GetUpdateViewInstance();
        }

        private ICommand signupCommand;
        public ICommand SignupCommand
        {
            get
            {
                return signupCommand ?? (signupCommand = new CommandHandler(() => Signup(), true));
            }
        }

        // sign up for the system
        // open the signup window and open a message to the user to know if it was successful.
        public void Signup()
        {
            var addDialog = new AddOrUpdateUserView();
            if ((bool)addDialog.ShowDialog())
            {
                MessageBox.Show("You Have Signed up Successfully, You can now login to the system!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

        // login to the system 
        // open the login window
        private void Login()
        {
            var addDialog = new LoginView();
            addDialog.ShowDialog();
        }
    }
}
