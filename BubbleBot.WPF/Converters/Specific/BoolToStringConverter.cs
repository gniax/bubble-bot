using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Configurations.Language;

namespace BubbleBot.Converters.Specific
{
    public class BoolToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (bool) value;
            return val ? LanguageManager.Translate("205") : LanguageManager.Translate("206");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}