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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieRenter.Views
{
    /// <summary>
    /// Interaction logic for ApproveUsersReviews.xaml
    /// </summary>
    public partial class DeleteReviewReason : Window
    {
        public DeleteReviewReason()
        {
            InitializeComponent();
        }

        private void Save_Reason(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        public string Answer
        {
            get { return TxtReason.Text; }
        }
    }
}
