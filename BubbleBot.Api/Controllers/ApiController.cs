using BubbleBot.Website.Models;
using BubbleBot.Website.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BubbleBot.Api.Controllers
{
    [Route("api")]
    public class ApiController : Controller
    {

        // Fields
        private const string _token = "1997";
        private readonly LoginService _loginService;
        private readonly RegisterService _registerService;
        private readonly BotService _botService;
        private readonly CollectHDVService _hdvService;


        // Constructor
        public ApiController(LoginService loginService, RegisterService registerService, BotService botService, CollectHDVService hdvService) 
        {
            _loginService = loginService;
            _registerService = registerService;
            _botService = botService;
            _hdvService = hdvService;
        }


        [HttpGet("login")]
        public JsonResult Login(string username, string password, string token)
        {
            if (token != _token)
                return Json(new { success = false, errorId = 0 });

            var user = _loginService.LoginUser(username, password, out int error);
            if (user == null)
                return Json(new { success = false, errorId = error });

            return Json(new
            {
                success = true,
                user = new
                {
                    id = user.Id,
                    name = user.Username,
                    avatar = user.AvatarUrl,
                    usergroup = user.UserGroup,
                },
                subscribed = user.IsSubscribedToTouch,
                touchEndDate = user.TouchEndDate,
                extensions = user.GetCurrentExtensions()
            });
        }

        [HttpGet("register")]
        public JsonResult Register(string username, string password, string email, string token)
        {
            if (token != _token)
                return Json(new { success = false, errorId = 0 });

            if (username.Length < 3 || username.Length > 15)
            {
                return Json(new { success = false, errorId = 0 });
            }

            if (password.Length < 5 || password.Length > 20)
            {
                return Json(new { success = false, errorId = 0 });
            }

            if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            {
                return Json(new { success = false, errorId = 0 });
            }

            bool result = _registerService.RegisterUser(username, password, email, out int error);
            if (result == false)
                return Json(new { success = false, errorId = error });

            return Json(new { success = true });
        }

        [HttpPatch("characters/{id}")]
        public void BotUpdateInfos(int id, string token, int user_id, int character_id, string account, string name, string server, string breed, byte level,
                          byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, string group_id, byte group_chief, string script_name)
        {
            if (token != _token)
                return;

            // TODO : Check if infos are corrupted or are a threat
            _botService.UpdateOrAdd(user_id, character_id, account, name, server, breed, level, percent_energy, percent_pods, kamas, map_id, map_pos, state, group_id, group_chief, script_name, id);
        }

        [HttpGet("botsstats")]
        public async Task<JsonResult> BotStatsRequest(string username, string token)
        {
            if (token != _token)
                return Json(new { success = false, errorId = 0 });

            List<Character> charactersList = await _botService.GetBotsInfos(username);
            if (charactersList == null)
                return Json(new { success = false, errorId = 1 });

            return Json(new
            {
                success = true,
                characters = charactersList
            });
        }

        [HttpGet("botinformations")]
        public async Task<JsonResult> BotInformationsRequest(string username, string account, string botname, string token)
        {
            if (token != _token)
                return Json(new { success = false, errorId = 0 });

            List<ArchivedCharacter> archivedCharacterList = await _botService.GetArchivedBotInfos(username, account, botname);
            if (archivedCharacterList == null)
                return Json(new { success = false, errorId = 1 });

            return Json(new
            {
                success = true,
                archivedcharacter = archivedCharacterList
            });
        }

        [HttpPost("characters")]
        public void BotInitialUpdateOrAdd(string token, int user_id, int character_id, string account, string name, string server, string breed, byte level,
                          byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, string group_id, byte group_chief, string script_name)
        {
            if (token != _token)
                return;

            // TODO : Check if infos are corrupted or are a threat
            _botService.UpdateOrAdd(user_id, character_id, account, name, server, breed, level, percent_energy, percent_pods, kamas, map_id, map_pos, state, group_id, group_chief, script_name);
        }

        [HttpPatch("characters/archive")]
        public void BotArchiveInfos(int id, string token, int user_id, int character_id, string account, string name, string server, string breed, byte level,
                          byte percent_energy, byte percent_pods, int kamas, int map_id, string map_pos, string state, string group_id, byte group_chief, string script_name)
        {
            if (token != _token)
                return;

            // TODO : Check if infos are corrupted or are a threat
            _botService.ArchiveAndAdd(user_id, character_id, account, name, server, breed, level, percent_energy, percent_pods, kamas, map_id, map_pos, state, group_id, group_chief, script_name);
        }

        [HttpPost("collected/hdv")]
        public void ReceivedHDVInfos(string token, int object_id, string object_name, string object_server, int object_price_lot_1, int object_price_lot_10, int object_price_lot_100, int object_average_price)
        {
            if (token != _token)
                return;

            // TODO : Check if infos are corrupted or are a threat
            _hdvService.Add(object_id, object_name, object_server, object_price_lot_1, object_price_lot_10, object_price_lot_100, object_average_price);
        }

        [HttpGet("iteminformations")]
        public JsonResult ItemInformationsRequest(string itemname, int itemid, string itemserver, string token)
        {
            if (token != _token)
                return Json(new { success = false, errorId = 0 });

            List<CollectedHDVItem> collectedItemsSelectedList = _hdvService.GetItemInfos(itemname, itemid, itemserver);
            if (collectedItemsSelectedList == null)
                return Json(new { success = false, errorId = 1 });

            return Json(new
            {
                success = true,
                collecteditemsselected = collectedItemsSelectedList
            });
        }
    }
}
