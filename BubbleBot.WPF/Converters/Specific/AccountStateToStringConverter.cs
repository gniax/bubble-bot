using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Converters.Specific
{
    public class AccountStateToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (short)value == 0 
                ? LanguageManager.Translate("753")
                : (short)value == 1 
                ? LanguageManager.Translate("754") 
                : (short)value == 2 
                ? LanguageManager.Translate("755")
                : LanguageManager.Translate("756");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}