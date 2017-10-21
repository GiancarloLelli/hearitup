using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace HIU.Core.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var typedBool = value != null && value is bool ? (bool)value : false;
            return typedBool ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is bool ? true : false;
        }
    }
}
