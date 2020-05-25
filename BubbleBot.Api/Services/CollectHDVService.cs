using BubbleBot.Website.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;


namespace BubbleBot.Website.Services
{
    public class CollectHDVService
    {
        // Fields
        private readonly PanelDb _panelDb;

        // Constructors
        public CollectHDVService() { }

        public CollectHDVService(PanelDb panelDb)
        {
            _panelDb = panelDb;
        }

        public void Add(int object_id, string object_name, string object_server, int object_price_lot_1, int object_price_lot_10, int object_price_lot_100, int object_average_price)
        {
            if (_panelDb == null) return;

            CollectedHDVItem ItemInformations = null;


            DateTime object_time = DateTime.Now;

            // If the user doesn't exist in the database, add it
            if (ItemInformations == null)
            {
                ItemInformations = new CollectedHDVItem( object_id,  object_name,  object_server, object_price_lot_1, object_price_lot_10, object_price_lot_100, object_average_price, object_time);
                _panelDb.CollectedHDVItems.Add(ItemInformations);
            }

            // Finally, save the panel's db
            _panelDb.SaveChanges();
        }

        public List<CollectedHDVItem> GetItemInfos(string object_name, int object_id, string object_server)
        {
            if (_panelDb == null)
                return null;


            List<CollectedHDVItem> allItemsSelected = new List<CollectedHDVItem>();

            if (object_name != "" && object_id == -1)
            {
                Console.WriteLine("CHECK NAME ..." + object_name + "   " + object_server);
                allItemsSelected = _panelDb.CollectedHDVItems.AsNoTracking().Where(i => i.Object_Name == object_name && i.Object_Server == object_server).ToList();
            }
            else if(object_id > 0)
            {
                Console.WriteLine("CHECK ID ...");
                allItemsSelected = _panelDb.CollectedHDVItems.AsNoTracking().Where(i => i.Object_Id == object_id && i.Object_Server == object_server).ToList();
                /*
                var CategoryNoList = list.Where(p => p.CatName == "name 1").Select(p => p.CatNo).ToList();

                foreach (var catno in CategoryNoList)
                {
                    var ProdList = (from ProdRec in list
                                    where ProdRec.CatName == "name 1" && ProdRec.CatNo == catno
                                    select ProdRec).First();
                    prod.Add(ProdList);
                }*/
            }

            foreach(var item in allItemsSelected)
            {
                Console.WriteLine(item.Object_Id.ToString() + "  " + item.Object_Price_Lot_1.ToString() + "  " + item.Object_Price_Lot_10.ToString() + "    " +  item.Object_Price_Lot_100.ToString() + "   " + item.Object_Server);
            }


            if (allItemsSelected.Count > 0)
            {
                Console.WriteLine("Nombe d'item :" + allItemsSelected.Count.ToString());
                return allItemsSelected;
            }

            return null;
        }


    }
}
