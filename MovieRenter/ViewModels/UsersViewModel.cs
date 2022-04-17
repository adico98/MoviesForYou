using MovieRenter.Models;
using MovieRenter.Command;
using MovieRenter.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MovieRenter.ViewModels
{
    public class UsersViewModel : ViewModelBase
    {
        // ctor 
        public UsersViewModel()
        {
            MembersList = new ObservableCollection<UserModel>();
            UpdateUsersList();
        }

        private ICommand addUserCommand;
        public ICommand AddUserCommand
        {
            get
            {
                return addUserCommand ?? (addUserCommand = new CommandHandler(() => AddUser(), true));
            }
        }

        private ICommand updateUserCommand;
        public ICommand UpdateUserCommand
        {
            get
            {
                return updateUserCommand ?? (updateUserCommand = new CommandHandler(param => UpdateUser(param), true));
            }
        }

        public void AddUser()
        {
            var addDialog = new AddOrUpdateUserView();
            ((AddOrUpdateUserViewModel)addDialog.DataContext).AddUser += AddUserEventHandler;
            if ((bool)addDialog.ShowDialog())
            {
                MessageBox.Show("User Added Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void UpdateUser(object param)
        {
            var addDialog = new AddOrUpdateUserView((UserModel) param);
            if ((bool)addDialog.ShowDialog())
            {
                UpdateUsersList();
                MessageBox.Show("User Updated Successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddUserEventHandler(object sender, UserModel user)
        {
            MembersList.Add(user);
        }

        public void UpdateUsersList()
        {
            MembersList.Clear();
            foreach (var member in DBOperations.GetAllUsersData().Result)
            {
                MembersList.Add(member);
            }
            OnPropertyChanged("MemberList");
        }

        public ObservableCollection<UserModel> MembersList { get; set; }
    }
}
