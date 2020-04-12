using BubbleBot.Configurations.Language;
using BubbleBot.Core.Accounts.InGame.Character.Inventory;
using BubbleBot.Protocol.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace BubbleBot.Converters.Specific
{
    public class EquipementActionConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ObjectEntry obj))
                return "-";

            return obj.Position == CharacterInventoryPositionEnum.INVENTORY_POSITION_NOT_EQUIPED ? LanguageManager.Translate("29") : LanguageManager.Translate("28");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
