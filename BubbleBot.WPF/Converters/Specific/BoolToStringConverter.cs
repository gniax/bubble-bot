using BubbleBot.Configurations.Language;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BubbleBot.Converters.Specific
{
    public class BoolToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = (bool)value;
            return val ? LanguageManager.Translate("205") : LanguageManager.Translate("206");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}