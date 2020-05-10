using System.IO;

namespace BubbleBot.Core.Accounts.Extensions.PetBreeder
{
    public class PetsToBreedEntry
    {
        // Constructor
        public PetsToBreedEntry(string petname, uint petid, string foodname, uint foodid, uint foodquantity, int petlastmeal, int petintervalmeal,int feddercount)
        {
            NamePet = petname;
            PetID = petid;
            NameFood = foodname;
            FoodID = foodid;
            FoodQuantity = foodquantity;
            PetLastMeal = petlastmeal;
            PetIntervalMeal = petintervalmeal;
            FedderCount = feddercount;
        }

        // Properties
        public string NamePet { get; }
        public uint PetID { get; }
        public string NameFood { get; }
        public uint FoodID { get; }
        public uint FoodQuantity { get; }
        public int PetLastMeal { get; set; }
        public int PetIntervalMeal { get; }
        public int FedderCount { get; set; }

        public void Save(BinaryWriter bw)
        {
            bw.Write(NamePet);
            bw.Write(PetID);
            bw.Write(NameFood);
            bw.Write(FoodID);
            bw.Write(FoodQuantity);
            bw.Write(PetLastMeal);
            bw.Write(PetIntervalMeal);
            bw.Write(FedderCount);
        }


        public static PetsToBreedEntry Load(BinaryReader br)
        {
            return new PetsToBreedEntry(br.ReadString(), br.ReadUInt32(), br.ReadString(), br.ReadUInt32(), br.ReadUInt32(),
                br.ReadInt32(), br.ReadInt32(), br.ReadInt32());
        }


    }
}

