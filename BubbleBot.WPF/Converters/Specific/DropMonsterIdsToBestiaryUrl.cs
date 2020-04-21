using System;
using System.Globalization;
using System.Windows.Data;

namespace BubbleBot.Converters.Specific
{
    public class DropMonsterIdsToBestiaryUrl : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string) value;
            if (val != "7777")
                return $"https://www.dofus-touch.com/fr/mmorpg/encyclopedie/monstres/{val}";
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}