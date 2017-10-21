using HIU.Models.Repository;
using System;
using Windows.UI.Xaml.Data;

namespace HIU.Core.Converters
{
    public class ObjectToMusicRecordItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is MusicRecordItem ? (MusicRecordItem)value : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null && value is MusicRecordItem ? (MusicRecordItem)value : null;
        }
    }
}
