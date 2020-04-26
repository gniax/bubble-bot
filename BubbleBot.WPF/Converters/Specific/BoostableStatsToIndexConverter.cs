using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Core.Enums;

namespace BubbleBot.Converters.Specific
{
    public class BoostableStatsToIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)(BoostableStats)value == null || (int)(BoostableStats)value == 0 || (int)(BoostableStats)value == 9)
                return 0;

            return (int) (BoostableStats) value - 9;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (BoostableStats) ((int) value + 9);
        }
    }
}