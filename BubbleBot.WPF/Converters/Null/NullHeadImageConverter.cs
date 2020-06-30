using BubbleBot.Configurations;
using BubbleBot.Core.Accounts;
using BubbleBot.Utility.DofusTouch;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Collections.Generic;

namespace BubbleBot.Converters.Others
{
    public class NullHeadImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value.ToString() != "")
            {
                AccountConfiguration account = value as AccountConfiguration;
                if (account.CharacterCreation.Create)
                {
                    return $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/cosmetics/{account.CharacterCreation.Breed}{account.CharacterCreation.Sex}_{account.CharacterCreation.Head+1}.png";
                }
            }

            return  DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}