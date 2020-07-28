using BubbleBot.Configurations.Language;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BubbleBot.Converters.Specific
{
    public class ToMainTitleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return $"{LanguageManager.Translate("751").ToUpper()}{value.ToString().ToUpper()}";
            }
            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
                                  CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
