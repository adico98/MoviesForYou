using MovieRenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MovieRenter.Converters
{
    public class UserToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(parameter == null)
            {
                return Visibility.Visible;
            }
            // return if the menu tab will be presented according to the users premission and what user is connect right now
            if ((Users)parameter == CurrentUser.UserType || ((Users)parameter == Users.Member && CurrentUser.UserType == Users.Admin))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
