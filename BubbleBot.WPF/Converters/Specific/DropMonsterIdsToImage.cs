using System;
using System.Globalization;
using System.Windows.Data;
using BubbleBot.Utility.DofusTouch;

namespace BubbleBot.Converters.Specific
{
    public class DropMonsterIdsToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string) value;
            if (val != "7777")
                return $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/monsters/{val}.png";
            return "..\\..\\Resources\\too_many.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}