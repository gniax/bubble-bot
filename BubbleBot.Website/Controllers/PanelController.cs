using BubbleBot.Website.Enums;
using BubbleBot.Website.Extensions;
using BubbleBot.Website.Models;
using BubbleBot.Website.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BubbleBot.Website.Controllers
{
    public class PanelController : Controller
    {

        // Fields
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly PanelDbContext _panelDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private static List<Character> _userBots = new List<Character>();

        // Constructor
        public PanelController(PanelDbContext panelDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _panelDbContext = panelDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Login Manager
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel lvm)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(lvm);

            if (!CaptchaExtension.ReCaptchaPassed(Request.Form["reCaptcha"]))
            {
                ModelState.AddModelError(string.Empty, "Le CAPTCHA a été refusé.");
                return View(lvm);
            }

            try
            {
                var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/login?username={lvm.Username}&password={lvm.Password}&token=1997");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                if (json.Value<bool>("success"))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, lvm.Username)
                    };

                    var userIdentity = new ClaimsIdentity(claims, "login");

                    ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                    await HttpContext.SignInAsync(principal, new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                    });

                    return RedirectToAction("Index");
                }

                // success : false
                switch (json.Value<byte>("errorId"))
                {
                    case 1:
                        ViewBag.ErrorMessage = "Compte introuvable.";
                        break;
                    case 2:
                        ViewBag.ErrorMessage = "Compte banni.";
                        break;
                    case 3:
                        ViewBag.ErrorMessage = "Compte en attente d'activation.";
                        break;
                    case 4:
                        ViewBag.ErrorMessage = "Mauvais mot de passe.";
                        break;
                }
            }
            catch
            {
                ViewBag.ErrorMessage = "Erreur inconnu.";
            }

            return View(lvm);
        }
        #endregion

        [Authorize]
        public IActionResult Index()
        {

            ViewBag.SubscribedToTouch = _panelDbContext.Users.Where(user => user.TouchEndDate != null && DateTime.Now < user.TouchEndDate.Value).Count();
            ViewBag.Extensions = _panelDbContext.Extensions.Include(e => e.ExtensionsBought);
            ViewBag.TotalUsers = _panelDbContext.Users.Count();
            return HandleAuthorizedAction();
        }

        [Authorize]
        public IActionResult Points(int error)
        {
            ViewBag.PointsPlans = _panelDbContext.PointsPlans;

            if (TempData["success"] != null)
            {
                ViewBag.Success = TempData["success"];
                TempData.Remove("success");
            }

            if (TempData["error"] != null)
            {
                ViewBag.Error = TempData["error"];
                TempData.Remove("error");
            }
            else if (error == 1)
            {
                ViewBag.PointsBuyingError = "Erreur lors de l'exécution du paiement, vérifiez votre option de paiement dans Paypal.";
            }

            return HandleAuthorizedAction();
        }

        [HttpGet]
        [Authorize]
        public IActionResult Subscriptions()
        {
            ViewBag.SubscriptionsPlans = _panelDbContext.SubscriptionsPlans;

            if (TempData["error"] != null)
            {
                ViewBag.Error = TempData["error"];
                TempData.Remove("error");
            }

            if (TempData["success"] != null)
            {
                ViewBag.Success = TempData["success"];
                TempData.Remove("success");
            }

            return HandleAuthorizedAction();
        }

        [Authorize]
        public IActionResult Extensions()
        {
            ViewBag.Extensions = _panelDbContext.Extensions;

            if (TempData["error"] != null)
            {
                ViewBag.Error = TempData["error"];
                TempData.Remove("error");
            }

            if (TempData["success"] != null)
            {
                ViewBag.Success = TempData["success"];
                TempData.Remove("success");
            }

            return HandleAuthorizedAction();
        }

        [Authorize]
        public IActionResult Downloads()
        {
            return HandleAuthorizedAction();
        }

        [Authorize]
        public IActionResult Bots()
        {
            return HandleAuthorizedAction();
        }


        #region HDV-STATS
              [Authorize]
              [HttpPost]
              public async Task<JsonResult> ItemDataRequest(string ItemName, int ItemId, string ItemServer)
                {
                  string errorMessage = "Erreur inconnue...";
                  User user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
                  if (user == null)
                      return Json(new { success = false, error = "Contexte utilisateur introuvable..." });
                  Console.WriteLine(ItemName);
                  Console.WriteLine(ItemId.ToString());
                  Console.WriteLine(ItemServer);
                  // Retrieve infos from api
                    var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/iteminformations?itemname={ItemName}&itemid={ItemId}&itemserver={ItemServer}&token=1997");
                  response.EnsureSuccessStatusCode();
                  var content = await response.Content.ReadAsStringAsync();
                  var json = JObject.Parse(content);
                  if (json.Value<bool>("success"))
                  {
                      dynamic jo = JObject.Parse(content);
                      var collectedItemsSelected = jo.collecteditemsselected;
                      List<CollectedHDVItem> collectedItemsSelectedList = new List<CollectedHDVItem>();
                      foreach (var item in collectedItemsSelected)
                      {
                          JObject Jitem = item as JObject;
                          CollectedHDVItem selected_item = Jitem.ToObject<CollectedHDVItem>();
                          collectedItemsSelectedList.Add(selected_item);
                      }

                if (collectedItemsSelectedList.Count() > 0)
                      {
                    DateTime now = DateTime.Now,
                                    yesterday = DateTime.Now.AddDays(-1),
                                    aWeekAgo = DateTime.Now.AddDays(-7),
                                    aMonthAgo = DateTime.Now.AddDays(-30);


                    List<KeyValuePair<DateTime, int>> all_prix_list_lot_1 = collectedItemsSelectedList.Select(c => new KeyValuePair<DateTime, int>(c.Object_Time, c.Object_Price_Lot_1))
                                                                                       .OrderBy(c => c.Key)
                                                                                       .ToList();
                    List<KeyValuePair<DateTime, int>> all_prix_list_lot_10 = collectedItemsSelectedList.Select(c => new KeyValuePair<DateTime, int>(c.Object_Time, c.Object_Price_Lot_10))
                                                                                       .OrderBy(c => c.Key)
                                                                                       .ToList();
                    List<KeyValuePair<DateTime, int>> all_prix_list_lot_100 = collectedItemsSelectedList.Select(c => new KeyValuePair<DateTime, int>(c.Object_Time, c.Object_Price_Lot_100))
                                                                                       .OrderBy(c => c.Key)
                                                                                       .ToList();
                    List<KeyValuePair<DateTime, int>> all_prix_list_average_price = collectedItemsSelectedList.Select(c => new KeyValuePair<DateTime, int>(c.Object_Time, c.Object_Average_Price))
                                                                                       .OrderBy(c => c.Key)
                                                                                       .ToList();

                    List<KeyValuePair<DateTime, int>> prix_list_lot_1_day = all_prix_list_lot_1.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_1_week = all_prix_list_lot_1.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_1_month = all_prix_list_lot_1.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();
                    

                    List<KeyValuePair<DateTime, int>> prix_list_lot_10_day = all_prix_list_lot_10.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_10_week = all_prix_list_lot_10.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_10_month = all_prix_list_lot_10.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                    List<KeyValuePair<DateTime, int>> prix_list_lot_100_day = all_prix_list_lot_100.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_100_week = all_prix_list_lot_100.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_lot_100_month = all_prix_list_lot_100.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                    List<KeyValuePair<DateTime, int>> prix_list_average_price_day = all_prix_list_average_price.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_average_price_week = all_prix_list_average_price.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                    List<KeyValuePair<DateTime, int>> prix_list_average_price_month = all_prix_list_average_price.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();



                    return Json(new
                        {
                            success = true,
                            // line infos
                            linegraph = true,
                            linegraph_prix_list_lot_1_day = prix_list_lot_1_day,
                            linegraph_prix_list_lot_1_week = prix_list_lot_1_week,
                            linegraph_prix_list_lot_1_month = prix_list_lot_1_month,
                            linegraph_prix_list_lot_10_day = prix_list_lot_10_day,
                            linegraph_prix_list_lot_10_week = prix_list_lot_10_week,
                            linegraph_prix_list_lot_10_month = prix_list_lot_10_month,
                            linegraph_prix_list_lot_100_day = prix_list_lot_100_day,
                            linegraph_prix_list_lot_100_week = prix_list_lot_100_week,
                            linegraph_prix_list_lot_100_month = prix_list_lot_100_month,
                            linegraph_prix_list_average_price_day = prix_list_average_price_day,
                            linegraph_prix_list_average_price_week = prix_list_average_price_week,
                            linegraph_prix_list_average_price_month = prix_list_average_price_month
                    });
                }
                      errorMessage = "Aucun object de ce type n'a été trouvé.";
                  }
                  else
                  {
                      switch (json.Value<byte>("errorId"))
                      {
                          case 0:
                              errorMessage = "Erreur de jeton de communication ...";
                              break;
                          case 1:
                              errorMessage = "Aucun item correspondant trouvé.";
                              break;
                      }
                  }
                  return Json(new { success = false, error = errorMessage });
              }

        [Authorize]
        public IActionResult HdvStats()
        {

            // Verification 
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return StatusCode(404);




            return View(user);
        }
        #endregion HDV-STATS

        #region Statistics Manager
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> BotDataRequest(string Account, string BotName)
        {
            string errorMessage = "Erreur inconnue...";
            User user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return Json(new { success = false, error = "Contexte utilisateur introuvable..." });

            // Retrieve infos from api
            var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/botinformations?username={user.Username}&account={Account}&botname={BotName}&token=1997");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            if (json.Value<bool>("success"))
            {
                dynamic jo = JObject.Parse(content);
                var archivedcharacter = jo.archivedcharacter;
                List<ArchivedCharacter> archivedCharacterList = new List<ArchivedCharacter>(); // Character archived data
                foreach (var character in archivedcharacter)
                {
                    JObject Jcharacter = character as JObject;
                    ArchivedCharacter archived_character = Jcharacter.ToObject<ArchivedCharacter>();
                    archivedCharacterList.Add(archived_character);
                }

                if (archivedCharacterList.Count() > 0)
                {
                    if (_userBots != null)
                    {
                        Character selectedBot = _userBots.Where(c => c.Account == Account && c.Name == BotName).FirstOrDefault();
                        if (selectedBot == null)
                        {
                            errorMessage = "Le personnage sélectionné est introuvable....";
                            return Json(new { success = false, error = errorMessage });
                        }

                        string breedUrl = Program.Constants.WebsiteIpAddress + $"/images/breeds/default.png";
                        if (selectedBot.Breed != null && selectedBot.Breed != "NULL" && selectedBot.Breed != "-" && selectedBot.Breed != "UNDEFINED")
                            breedUrl = Program.Constants.WebsiteIpAddress + $"/images/breeds/" + selectedBot.Breed + ".png";

                        // Constants var
                        DateTime now = DateTime.Now,
                                 yesterday = DateTime.Now.AddDays(-1),
                                 aWeekAgo = DateTime.Now.AddDays(-7),
                                 aMonthAgo = DateTime.Now.AddDays(-30);


                        List<Tuple<string, DateTime>> stateCharacter = archivedCharacterList.Select(c => new Tuple<string, DateTime>(c.State, c.Updated_at))
                                                                                            .OrderBy(c => c.Item2)
                                                                                            .ToList();

                        ILookup<string, (byte, DateTime)> lvlList = archivedCharacterList.Select(i => new { i.Level, i.Updated_at, i.Script_Name })
                                                                                         .OrderBy(g => g.Updated_at)
                                                                                         .ToLookup(c => c.Script_Name, c => (c.Level, c.Updated_at));

                        Dictionary<string, string[]> lvlAverageList = new Dictionary<string, string[]>();
                        int counter = 0;
                        foreach (var line in lvlList)
                        {
                            if (line.Key != null)
                            {

                                if (counter < 5)
                                {
                                    bool first = true;

                                    DateTime beforeDate = line.First().Item2;
                                    byte beforeLevel = line.First().Item1;

                                    TimeSpan dateSum = (default);
                                    int levelSum = 0;

                                    foreach ((byte, DateTime) items in line)
                                    {
                                        if (!(items.Item2 > beforeDate.AddHours(1)) && !first)
                                        {
                                            dateSum += items.Item2 - beforeDate;
                                            levelSum += (int)items.Item1 - beforeLevel;
                                        }
                                        first = false;
                                        beforeDate = items.Item2;
                                        beforeLevel = items.Item1;
                                    }

                                    double lvlAverage = Math.Round((levelSum / dateSum.TotalHours), 2);
                                    double hourSum = Math.Round((dateSum.TotalHours), 0);
                                    string[] data = { lvlAverage.ToString(), hourSum.ToString(), levelSum.ToString() };

                                    if (lvlAverage != 0 && hourSum != 0)
                                    {
                                        lvlAverageList.Add(line.Key, data);
                                        counter++;
                                    }
                                }
                            }
                        }

                        List<KeyValuePair<DateTime, int>> kamasCharacter = archivedCharacterList.Select(c => new KeyValuePair<DateTime, int>(c.Updated_at, c.Kamas))
                                                                                            .OrderBy(c => c.Key)
                                                                                            .ToList();

                        List<KeyValuePair<DateTime, byte>> levelCharacter = archivedCharacterList.Select(c => new KeyValuePair<DateTime, byte>(c.Updated_at, c.Level))
                                                                                            .OrderBy(c => c.Key)
                                                                                            .ToList();

                        List<KeyValuePair<DateTime, byte>> energyCharacter = archivedCharacterList.Select(c => new KeyValuePair<DateTime, byte>(c.Updated_at, c.Percent_energy))
                                                                                            .OrderBy(c => c.Key)
                                                                                            .ToList();

                        List<KeyValuePair<DateTime, byte>> podsCharacter = archivedCharacterList.Select(c => new KeyValuePair<DateTime, byte>(c.Updated_at, c.Percent_pods))
                                                                                            .OrderBy(c => c.Key)
                                                                                            .ToList();

                        Dictionary<string, int> statesDay = new Dictionary<string, int>();
                        Dictionary<string, int> statesWeek = new Dictionary<string, int>();
                        Dictionary<string, int> statesMonth = new Dictionary<string, int>();

                        List<KeyValuePair<DateTime, int>> kamasDay = kamasCharacter.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, int>> kamasWeek = kamasCharacter.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, int>> kamasMonth = kamasCharacter.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                        List<KeyValuePair<DateTime, byte>> levelDay = levelCharacter.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> levelWeek = levelCharacter.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> levelMonth = levelCharacter.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                        List<KeyValuePair<DateTime, byte>> energyDay = energyCharacter.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> energyWeek = energyCharacter.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> energyMonth = energyCharacter.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                        List<KeyValuePair<DateTime, byte>> podsDay = podsCharacter.Where(c => c.Key > yesterday).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> podsWeek = podsCharacter.Where(c => c.Key > aWeekAgo).OrderBy(c => c.Key).ToList();
                        List<KeyValuePair<DateTime, byte>> podsMonth = podsCharacter.Where(c => c.Key > aMonthAgo).OrderBy(c => c.Key).ToList();

                        foreach (Tuple<string, DateTime> kvp in stateCharacter)
                        {
                            AccountStates state = (AccountStates)System.Enum.Parse(typeof(AccountStates), kvp.Item1);
                            DateTime updatedat = kvp.Item2;
                            string s_state = null;

                            switch (state)
                            {
                                case AccountStates.NONE:
                                    s_state = "Inactif";
                                    break;
                                case AccountStates.MOVING:
                                    s_state = "Déplacement";
                                    break;
                                case AccountStates.FIGHTING:
                                    s_state = "Combat";
                                    break;
                                case AccountStates.RECAPTCHA:
                                    s_state = "Captcha";
                                    break;
                                case AccountStates.REGENERATING:
                                    s_state = "Regénération";
                                    break;
                                case AccountStates.BUYING:
                                    s_state = "HDV";
                                    break;
                                case AccountStates.SELLING:
                                    s_state = "HDV";
                                    break;
                                case AccountStates.GATHERING:
                                    s_state = "Récolte";
                                    break;
                            }
                            if (s_state != null)
                            {
                                // Note : to get all the data (for instance: in a week) we have to do the sum of the day data + week data
                                if (updatedat > yesterday)
                                {
                                    statesDay.TryGetValue(s_state, out int currentCount);
                                    statesDay[s_state] = currentCount + 1;

                                    statesWeek.TryGetValue(s_state, out var secondCount);
                                    statesWeek[s_state] = secondCount + 1;

                                    statesMonth.TryGetValue(s_state, out var thirdCount);
                                    statesMonth[s_state] = thirdCount + 1;
                                }
                                else if (updatedat > aWeekAgo)
                                {
                                    statesWeek.TryGetValue(s_state, out var currentCount);
                                    statesWeek[s_state] = currentCount + 1;

                                    statesMonth.TryGetValue(s_state, out var secondCount);
                                    statesMonth[s_state] = secondCount + 1;
                                }

                                else if (updatedat > aMonthAgo)
                                {
                                    statesMonth.TryGetValue(s_state, out var thirdCount);
                                    statesMonth[s_state] = thirdCount + 1;
                                }
                            }
                        }

                        statesDay.OrderByDescending(v => v.Value);
                        statesWeek.OrderByDescending(v => v.Value);
                        statesMonth.OrderByDescending(v => v.Value);

                        return Json(new
                        {
                            success = true,
                            account = Account,
                            botname = BotName,
                            breedURL = breedUrl,
                            botserver = selectedBot.Server,
                            botlevel = selectedBot.Level,
                            botstate = selectedBot.State,
                            botmappos = selectedBot.Map_pos,
                            botmapid = selectedBot.Map_id,
                            botid = selectedBot.Character_id,
                            botscript = selectedBot.Script_Name,
                            botkamas = selectedBot.Kamas,
                            botenergy = selectedBot.Percent_energy,
                            botpods = selectedBot.Percent_pods,
                            botgroupid = selectedBot.Group_Id,
                            // state infos
                            stategraph = true,
                            stategraph_data_day = statesDay,
                            stategraph_data_week = statesWeek,
                            stategraph_data_month = statesMonth,
                            // line infos
                            linegraph = true,
                            linegraph_kamas_day = kamasDay,
                            linegraph_kamas_week = kamasWeek,
                            linegraph_kamas_month = kamasMonth,
                            linegraph_level_day = levelDay,
                            linegraph_level_week = levelWeek,
                            linegraph_level_month = levelMonth,
                            linegraph_energy_day = energyDay,
                            linegraph_energy_week = energyWeek,
                            linegraph_energy_month = energyMonth,
                            linegraph_pods_day = podsDay,
                            linegraph_pods_week = podsWeek,
                            linegraph_pods_month = podsMonth,
                            // bar infos
                            bargraph = true,
                            bargraph_lvl_average = lvlAverageList
                        });
                    }
                }
                errorMessage = "Aucun historique associé au compte trouvé.";
            }
            else
            {
                switch (json.Value<byte>("errorId"))
                {
                    case 0:
                        errorMessage = "Erreur de jeton de communication ...";
                        break;
                    case 1:
                        errorMessage = "Aucun historique associé au compte trouvé.";
                        break;
                }
            }
            return Json(new { success = false, error = errorMessage });
        }

        [Authorize]
        public async Task<IActionResult> BotsStats()
        {
            try
            {
                // Verification 
                var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
                if (user == null)
                    return StatusCode(404);

                // Retrieve infos from api
                var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/botsstats?username={user.Username}&token=1997");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);

                if (json.Value<bool>("success"))
                {
                    dynamic jo = JObject.Parse(content);
                    var characters = jo.characters;
                    List<Character> charactersList = new List<Character>();

                    // Retrieves characters
                    foreach (var character in characters)
                    {
                        JObject Jcharacter = character as JObject;
                        Character charac = Jcharacter.ToObject<Character>();
                        charactersList.Add(charac);
                    }

                    // If there are no errors
                    if (charactersList.Count() > 0)
                    {
                        // Here we reorganize and sort every bots in order to display them
                        List<Character> onlineBots = new List<Character>();
                        List<Character> offlineBots = new List<Character>();
                        List<Character> bannedBots = new List<Character>();
                        Dictionary<string, List<Character>> onlineAccounts = new Dictionary<string, List<Character>>();
                        Dictionary<string, List<Character>> offlineAccounts = new Dictionary<string, List<Character>>();
                        Dictionary<string, List<Character>> bannedAccounts = new Dictionary<string, List<Character>>();

                        foreach (Character character in charactersList)
                        {
                            if (character.State != AccountStates.DISCONNECTED.ToString() && character.State != AccountStates.BANNED.ToString())
                            {
                                onlineBots.Add(character);

                                List<Character> existing;
                                if (onlineAccounts.TryGetValue(character.Account, out existing))
                                {
                                    existing.Add(character);
                                    onlineAccounts[character.Account] = existing;
                                }
                                else
                                {
                                    existing = new List<Character>();
                                    existing.Add(character);
                                    onlineAccounts.Add(character.Account, existing);
                                }
                            }
                            else if (character.State == AccountStates.DISCONNECTED.ToString())
                            {
                                offlineBots.Add(character);

                                List<Character> existing;
                                if (offlineAccounts.TryGetValue(character.Account, out existing))
                                {
                                    existing.Add(character);
                                    offlineAccounts[character.Account] = existing;
                                }
                                else
                                {
                                    existing = new List<Character>();
                                    existing.Add(character);
                                    offlineAccounts.Add(character.Account, existing);
                                }
                            }
                            else if (character.State == AccountStates.BANNED.ToString())
                            {
                                bannedBots.Add(character);

                                List<Character> existing;
                                if (bannedAccounts.TryGetValue(character.Account, out existing))
                                {
                                    existing.Add(character);
                                    bannedAccounts[character.Account] = existing;
                                }
                                else
                                {
                                    existing = new List<Character>();
                                    existing.Add(character);
                                    bannedAccounts.Add(character.Account, existing);
                                }
                            }
                        }

                        // Here we have to sort every bot => by group (with a chief) then alone bots
                        onlineBots = onlineBots.OrderByDescending(x => x.Group_Id).ThenByDescending(x => x.Group_Chief).ThenBy(x => x.Account).ThenBy(x => x.Name).ToList();
                        offlineBots = offlineBots.OrderBy(x => x.Account).ThenBy(x => x.Name).ToList();
                        bannedBots = bannedBots.OrderBy(x => x.Account).ThenBy(x => x.Name).ToList();

                        // Here we have every group in online bots in a dictionnary with group_id as key
                        Dictionary<string, List<Character>> everyGroup = new Dictionary<string, List<Character>>();

                        foreach (Character bot in onlineBots)
                        {
                            if (bot.Group_Id != null && bot.Group_Id != "-" && !everyGroup.ContainsKey(bot.Group_Id)) // If the bot has a group
                            {
                                List<Character> group = new List<Character>();
                                group.Add(bot);
                                foreach (Character lookingForMember in onlineBots)
                                {
                                    if (lookingForMember.Group_Id == bot.Group_Id && lookingForMember != bot)
                                    {
                                        group.Add(lookingForMember);
                                    }
                                }

                                if (group.Count() > 1 && !everyGroup.ContainsKey(bot.Group_Id))
                                {
                                    everyGroup.Add(bot.Group_Id, group);
                                }
                            }
                        }

                        // Associating colors with groups
                        Dictionary<string, string> groupsColors = new Dictionary<string, string>();
                        List<string> availableColors = new List<string>() { "blueviolet" , "brown" , "cadetblue" , "coral" , "cornflowerblue" , "hotpink" , "red" , "palevioletred" , "darkkhaki" ,
                                                        "darkgoldenrod" , "darkgrey" , "darkmagenta" , "darkorange", "fuchsia", "turquoise", "springgreen" , "olivedrap" ,
                                                        "forestgreen" , "moccassin" };

                        // EveryGroup get a color in groupsColors dictionnary , if all colors are taken, it starts again at 0
                        int i = 0;
                        foreach (string key in everyGroup.Keys)
                        {
                            groupsColors.Add(key, availableColors[i]);
                            if (i++ > availableColors.Count() - 1)
                                i = 0;
                            else
                                i++;
                        }

                        ViewBag.Characters = charactersList;
                        ViewBag.OnlineBots = onlineBots;
                        ViewBag.OfflineBots = offlineBots;
                        ViewBag.BannedBots = bannedBots;
                        ViewBag.EveryGroup = everyGroup;
                        ViewBag.GroupsColors = groupsColors;
                        _userBots = charactersList;
                        
                        return View(user);
                    }
                }

                switch (json.Value<byte>("errorId"))
                {
                    case 1:
                        ViewBag.ErrorMessage = "Aucun personnage associé au compte trouvé.";
                        break;
                }

                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION : " + ex);
                return StatusCode(404);
            }
        }
        #endregion

        #region UserProfile Manager
        [HttpGet]
        [Authorize]
        public IActionResult UserProfile()
        {
            if (TempData["error"] != null)
            {
                Console.WriteLine("help");
                ViewBag.Error = TempData["error"];
                TempData.Remove("error");
            }

            if (TempData["success"] != null)
            {
                ViewBag.Success = TempData["success"];
                TempData.Remove("success");
            }

            return HandleAuthorizedAction();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult UserProfile(string FirstName, string Surname, string Discord, string Description, IFormFile Avatar)
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index");

            if (!CaptchaExtension.ReCaptchaPassed(Request.Form["reCaptcha"]))
            {
                ModelState.AddModelError(string.Empty, "Le CAPTCHA a été refusé.");
                return HandleAuthorizedAction();
            }

            User user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return StatusCode(404);

            if (FirstName != null)
                if (FirstName.Length < 2 || FirstName.Length > 15 || !Regex.IsMatch(FirstName, @"^[a-zA-Z]+$"))
                {
                    TempData["error"] += "La longueur du prénom doit être compris entre 2 et 15 caractères alphabétiques.<br/>";
                }

            if (Surname != null)
                if (Surname.Length < 2 || Surname.Length > 15 || !Regex.IsMatch(FirstName, @"^[a-zA-Z]+$"))
                {
                    TempData["error"] += "La longueur du nom doit être compris entre 2 et 15 caractères alphabétiques.<br/>";
                }

            if (Discord != null)
                if (Discord.Length < 2 || Surname.Length > 18)
                {
                    TempData["error"] += "La longueur du discord doit être compris entre 2 et 18 caractères.<br/>";
                }

            if (Description != null)
                if (Description.Length > 255)
                {
                    TempData["error"] += "La longueur de la description doit être inférieur à 255 caractères.<br/>";
                }

            if (TempData["error"] != null)
            {
                return RedirectToAction("UserProfile");
            }

            bool userChange = false;
            if (user.Surname != Surname)
            {
                user.Surname = Surname;
                userChange = true;
            }

            if (user.FirstName != FirstName)
            {
                user.FirstName = FirstName;
                userChange = true;
            }

            if (user.Discord != Discord)
            {
                user.Discord = Discord;
                userChange = true;
            }

            if (user.Description != Description)
            {
                user.Description = Description;
                userChange = true;
            }

            if (Avatar != null)
            {
                string fileExt = Path.GetExtension(Avatar.FileName).ToLower();
                string fileName = Path.GetFileName(Avatar.FileName);
                if (fileName != "")
                {
                    if (fileExt == ".jpg" || fileExt == ".gif" || fileExt == ".png")
                    {
                        if (Avatar.Length <= 2e+6)
                        {
                            if (user.Avatar != "default.jpg")
                            {
                                if (System.IO.File.Exists(_webHostEnvironment.WebRootPath + "/uploads/avatars/" + user.Avatar))
                                    System.IO.File.Delete(_webHostEnvironment.WebRootPath + "/uploads/avatars/" + user.Avatar);
                            }

                            string newFileName = user.Username + fileExt;
                            string filePath = _webHostEnvironment.WebRootPath + "/uploads/avatars/" + newFileName;

                            FileStream stream;
                            Avatar.CopyTo(stream = new FileStream(filePath, FileMode.Create));
                            stream.Close();

                            user.Avatar = newFileName;
                            userChange = true;
                        }
                        else
                        {
                            TempData["error"] += "La taille de l'image doit être inférieur à 2 Mo.<br/>";
                        }
                    }
                    else
                    {
                        TempData["error"] += "L'extension de l'image doit être .jpg, .gif ou .png.<br/>";
                    }
                }
                else
                {
                    TempData["error"] += "Le nom de l'image ne peut pas être nul.<br/>";
                }
            }

            if (TempData["error"] != null)
            {
                return RedirectToAction("UserProfile");
            }

            if (userChange)
            {
                _panelDbContext.Update(user);
                _panelDbContext.SaveChanges();
            }

            return HandleAuthorizedAction();
        }
        #endregion

        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }

        private IActionResult HandleAuthorizedAction()
        {
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return StatusCode(404);

            return View(user);
        }

    }
}