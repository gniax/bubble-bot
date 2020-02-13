using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BubbleBot.Website.Models
{
    [Table("characters_archive")]
    public class ArchivedCharacter
    {
        // Properties
        [Key]
        public int Id { get; set; }
        public int User_id { get; set; }
        public int Character_id { get; set; }
        public string Account { get; set; }
        public string Name { get; set; }
        public string Server { get; set; }
        public byte Level { get; set; }
        public byte Percent_energy { get; set; }
        public byte Percent_pods { get; set; }
        public int Kamas { get; set; }
        public int Map_id { get; set; }
        public string Map_pos { get; set; }
        public string State { get; set; }
        public DateTime Updated_at { get; set; }

        public ArchivedCharacter() { }

        public ArchivedCharacter(int user_id, int character_id, string account, string name, string server, byte level,
                          byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, DateTime updated_at)
        {
            User_id = user_id;
            Character_id = character_id;
            Account = account;
            Name = name;
            Server = server;
            Level = level;
            Percent_energy = percent_energy;
            Percent_pods = percent_pods;
            Kamas = kamas;
            Map_id = map_id;
            Map_pos = map_pos;
            State = state;
            Updated_at = updated_at;
        }
    }
}
