using System;
using System.Collections.Generic;
using System.Linq;
using BubbleBot.Converters.Enums;
using BubbleBot.Protocol.Data;
using BubbleBot.Protocol.Enums;
using BubbleBot.Protocol.Messages;
using BubbleBot.Protocol.Types;
using BubbleBot.Utility.DofusTouch;
using BubbleBot.Data;

namespace BubbleBot.Core.Accounts.InGame.Character.Inventory
{
    public class ObjectEntry
    {
        // Constructor
        public ObjectEntry(ObjectItem o, Items item = null)
        {
            if (ObjectEffectsToString == null)
                ObjectEffectsToString = new List<string>();

            if (SortedDropMonsterIds == null)
                SortedDropMonsterIds = new List<string>();

            GID = o.ObjectGID;
            UID = o.ObjectUID;
            Quantity = o.Quantity;
            Position = (CharacterInventoryPositionEnum) o.Position;

            // Check if this item gives hp back (BOOST_HP 110)
            for (var i = 0; i < o.Effects.Count; i++)
            {
                ObjectEffectsToString.Add(ObjectEffectToStringConverter.Convert(o.Effects[i]));

                if (!(o.Effects[i] is ObjectEffectInteger oei))
                    continue;

                if (oei.ActionId == 110)
                    RegenValue = oei.Value;
                else if (oei.ActionId == 158) WeightBoost = oei.Value;
            }

            SetObjectInformations(item);

        }

        // Properties
        public uint GID { get; private set; }
        public uint UID { get; private set; }
        public uint Quantity { get; private set; }
        public int Price { get; private set; }
        public CharacterInventoryPositionEnum Position { get; private set; }
        public ObjectTypes Type { get; private set; }
        public string Name { get; private set; }
        public int IconId { get; private set; }
        public bool Usable { get; private set; }
        public bool Exchangeable { get; private set; }
        public int Range { get; private set; }
        public int Level { get; private set; }
        public string Description { get; private set; }
        public bool IsFishingRod { get; private set; }
        public int RealWeight { get; private set; }
        public int TypeId { get; private set; }
        public int SuperTypeId { get; private set; }
        public uint RegenValue { get; private set; }
        public uint WeightBoost { get; private set; }
        public string StringType { get; set; }
        public List<object> DropMonsterIds { get; private set; }
        public List<string> ObjectEffectsToString { get; set; }

        public string IconUrl =>
            $"https://dofustouch.cdn.ankama.com/assets/{DTConstants.AssetsVersion}/gfx/items/{IconId}.png";

        public List<string> SortedDropMonsterIds { get; private set; } // Used for AccountInventoryView
        public bool SortedDropMonsterIdsIsEmpty => SortedDropMonsterIds.Count > 0; // Used for AccountInventoryView

        private async void SetObjectInformations(Items item)
        {
           
            if(item == null)
                item = await DataManager.Get<Items>((int)GID);
 
            var type = await DataManager.Get<ItemTypes>(item.TypeId);
            DropMonsterIds = item.DropMonsterIds;
            if (DropMonsterIds != null)
            {
                foreach (var id in DropMonsterIds)
                    if ((Convert.ToInt32(id) < 2270 || Convert.ToInt32(id) > 2601) &&
                        SortedDropMonsterIds.Count < 7) // archi-monstres
                        SortedDropMonsterIds.Add(id.ToString());
                    else if ((Convert.ToInt32(id) < 2270 || Convert.ToInt32(id) > 2601) &&
                             SortedDropMonsterIds.Count == 7)
                        SortedDropMonsterIds.Add("7777"); // Fake-id to display too_many picture  
                if (SortedDropMonsterIds.Count > 0)
                    SortedDropMonsterIds = SortedDropMonsterIds.OrderBy(x => Convert.ToInt32(x)).ToList();
            }

            Description = item.DescriptionId;
            Name = item.NameId;
            IconId = item.IconId;
            Price = item.Price;
            Level = item.Level;
            Usable = item.Usable;
            Exchangeable = item.Exchangeable;
            Range = item.Range;
            IsFishingRod = item.TypeId == 20 && item.UseAnimationId == 18;
            RealWeight = item.RealWeight;
            TypeId = item.TypeId;
            StringType = ObjectTypeNameFinder.GetObjectTypeNameById(TypeId);

            if (type != null && type.Id != 0)
            {
                SuperTypeId = type.SuperTypeId;
                Type = InventoryHelper.GetObjectType(SuperTypeId);
            }
        }

        #region Updates

        public void Update(ObjectItem o)
        {
            GID = o.ObjectGID;
            UID = o.ObjectUID;
            Quantity = o.Quantity;
            Position = (CharacterInventoryPositionEnum) o.Position;
        }

        public void UpdateQuantity(uint qty)
        {
            Quantity = qty;
        }

        public void Update(ObjectMovementMessage message)
        {
            Position = (CharacterInventoryPositionEnum) message.Position;
        }

        #endregion
    }
}