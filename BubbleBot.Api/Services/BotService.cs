using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BubbleBot.Api.Extensions;
using BubbleBot.Website.Models;
using Microsoft.EntityFrameworkCore;

namespace BubbleBot.Website.Services
{
    public class BotService
    {

        // Fields
        private readonly PanelDb _panelDb;

        // Constructors
        public BotService() { }

        public BotService(PanelDb panelDb)
        {
            _panelDb = panelDb;
        }


        public void UpdateOrAdd(int user_id, int character_id, string account, string name, string server, string breed, byte level,
                                byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, string groupid, byte groupchief, string scriptname, int id = -1)
        {
            if (_panelDb == null) return;

            Character character = null;
            if(id != -1) // Si on update juste
            {
                character = _panelDb.Characters.FirstOrDefault(c => c.Character_id == id);
                if (character == null)
                    return;
            }
            else
            {
                character = _panelDb.Characters.FirstOrDefault(c => c.Character_id == character_id);
            }

            DateTime currDate = DateTime.Now;
            // If the user doesn't exist in the database, add it
            if (character == null)
            {
                character = new Character(user_id, character_id, account, name, server, breed, level, percent_energy, percent_pods, kamas, map_id, map_pos, state, currDate, currDate, groupid, groupchief, scriptname);
                _panelDb.Characters.Add(character);
            }
            // Otherwise just update the informations
            else
            {
                character.Name = name;
                character.Server = server;
                character.Breed = breed;
                character.Level = level;
                character.Percent_energy = percent_energy;
                character.Percent_pods = percent_pods;
                character.Kamas = kamas;
                character.Map_id = map_id;
                character.Map_pos = map_pos;
                character.State = state;
                character.Updated_at = currDate;
                character.Group_Id = groupid;
                character.Group_Chief = groupchief;
                character.Script_Name = scriptname;
                _panelDb.Characters.Update(character);
            }

            // Finally, save the panel's db
            _panelDb.SaveChanges();
        }

        public void ArchiveAndAdd(int user_id, int character_id, string account, string name, string server, string breed, byte level,
                                byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, string groupid, byte groupchief, string scriptname)
        {
            if (_panelDb == null) return;
            if (name != null) // ??
            {
                DateTime currDate = DateTime.Now;

                ArchivedCharacter character = new ArchivedCharacter(user_id, character_id, account, name, server, breed, level, percent_energy, percent_pods, kamas, map_id, map_pos, state, currDate, groupid, groupchief, scriptname);
                _panelDb.ArchivedCharacters.Add(character);

                // Finally, save the panel's db
                _panelDb.SaveChanges();
            }
        }

        public async Task<List<Character>> GetBotsInfos(string username)
        {
            if (_panelDb == null) 
                return null;

            var user = _panelDb.Users.Select(u => u.Username == username);
            if (user == null)
                return null;

            int userid = await _panelDb.GetUserId(username);
            if(userid == (default))
                return null;

            _panelDb.SaveChanges();
            List<Character> characters = new List<Character>();
            characters = _panelDb.Characters.Where(c => c.User_id == userid).ToList();
            
            if(characters.Count > 0)
            {
                return characters;
            }

            return null;
        }
        public async Task<List<ArchivedCharacter>> GetArchivedBotInfos(string username, string account, string botname)
        {
            if (_panelDb == null) 
                return null;

            var user = _panelDb.Users.Select(u => u.Username == username);
            if (user == null)
                return null;

            int userid = await _panelDb.GetUserId(username);
            if (userid == (default))
                return null;

            _panelDb.SaveChanges(); // Debug ?
            List<ArchivedCharacter> archivedcharacter = new List<ArchivedCharacter>();
            archivedcharacter = _panelDb.ArchivedCharacters.Where(c => c.User_id == userid && c.Account == account && c.Name == botname).ToList();
         
            if (archivedcharacter.Count > 0)
            {
                return archivedcharacter;
            }

            return null;
        }
    }
}
