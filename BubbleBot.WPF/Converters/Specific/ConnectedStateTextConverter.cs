using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Converters.Specific
{
    public class ConnectedStateTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AccountStates) value == AccountStates.DISCONNECTED || (AccountStates) value == AccountStates.BANNED
                ? LanguageManager.Translate("12")
                : LanguageManager.Translate("680");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}