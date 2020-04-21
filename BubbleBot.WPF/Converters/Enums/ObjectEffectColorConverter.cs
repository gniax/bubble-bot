using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace BubbleBot.Converters.Enums
{
    public class ObjectEffectColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var effect = (string) value;
                if (effect == null)
                    return Brushes.White;

                // Note: If it contains - => Red
                // Else if it contains numbers => Green
                // Else => White
                var maxIndex = effect.Length < 5 ? effect.Length : 5;
                if (effect.Substring(0, maxIndex).Contains("-"))
                    return Brushes.PaleVioletRed;
                if (effect.Any(char.IsDigit))
                    return Brushes.LimeGreen;
                return Brushes.White;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception : {0}", ex.Message);
                return Brushes.White;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}