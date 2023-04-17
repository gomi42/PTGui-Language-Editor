using System;
using System.Globalization;
using System.Windows.Data;

namespace PTGui_Language_Editor
{
    public class DivedByTwoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val = (double)value;

            return val / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 42;
        }
    }
}
