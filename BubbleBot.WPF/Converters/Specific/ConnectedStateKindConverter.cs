using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Core.Enums;
using Kind = MahApps.Metro.IconPacks.PackIconMaterialKind;

namespace BubbleBot.Converters.Specific
{
    public class ConnectedStateKindConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (AccountStates) value == AccountStates.DISCONNECTED || (AccountStates) value == AccountStates.BANNED
                ? Kind.PowerPlug
                : Kind.PowerPlugOff;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}