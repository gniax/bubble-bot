using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("collected_hdv")]
    public class CollectedHDVItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        // Properties
        [Key]
        public int Object_Id { get; set; }
        public string Object_Name { get; set; }
        public string Object_Server { get; set; }
        public int Object_Price_Lot_1 { get; set; }
        public int Object_Price_Lot_10 { get; set; }
        public int Object_Price_Lot_100 { get; set; }
        public int Object_Average_Price { get; set; }
        public DateTime Object_Time { get; set; }

        public CollectedHDVItem() { }

        public CollectedHDVItem(int object_id, string object_name, string object_server, int pricelot1, int pricelot10, int pricelot100, int object_average_price, DateTime object_time)
        {
            Object_Id = object_id;
            Object_Name = object_name;
            Object_Server = object_server;
            Object_Price_Lot_1 = pricelot1;
            Object_Price_Lot_10 = pricelot10;
            Object_Price_Lot_100 = pricelot100;
            Object_Average_Price = object_average_price;
            Object_Time = object_time;
        }
    }
}