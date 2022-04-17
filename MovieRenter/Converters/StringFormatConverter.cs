using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace MovieRenter.Converters
{
    public class StringFormatConverter : IMultiValueConverter
    {

        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            // return the movies format
            return string.Format("{0} ({1})", (string)value[0], (int)value[1]);
        }

        public object ConvertBack(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return (object[])DependencyProperty.UnsetValue;
        }
    }
}
