using HIU.Models.Spotify;
using System;
using Windows.UI.Xaml.Data;

namespace HIU.Core.Converters
{
    public class ObjectToTrackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is Track ? (Track)value : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is Track ? (Track)value : null;
        }
    }
}
