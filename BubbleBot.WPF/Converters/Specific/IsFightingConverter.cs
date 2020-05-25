using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Configurations.Language;
using BubbleBot.Core.Enums;

namespace BubbleBot.Converters.Specific
{
    public class IsFightingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (AccountStates)value;
            return val == AccountStates.FIGHTING ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}