using MovieRenter.Models;
using MovieRenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MovieRenter.Views
{
    /// <summary>
    /// Interaction logic for AddOrUpdateUserView.xaml
    /// </summary>
    public partial class AddOrUpdateUserView : Window
    {
        public AddOrUpdateUserView()
        {
            InitializeComponent();
        }

        public AddOrUpdateUserView(UserModel selectedUser)
        {
            InitializeComponent();
            this.DataContext = new AddOrUpdateUserViewModel(selectedUser);
            this.PasswordTxt.Password = selectedUser.Password;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            { ((dynamic)this.DataContext).CurrMember.Password = ((PasswordBox)sender).Password; }
        }
    }
}
