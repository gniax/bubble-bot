using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BubbleBot.Website.Models;
using BubbleBot.Website.ViewModels;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BubbleBot.Website.Extensions;
using System.Text.RegularExpressions;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace BubbleBot.Website.Controllers
{
    public class PanelController : Controller
    {

        // Fields
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly PanelDbContext _panelDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

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

        [Authorize]
        public async Task<IActionResult> BotsStats()
        {
            try
            {

                var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
                if (user == null)
                    return StatusCode(404);
                var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/botsstats?username={user.Username}&token=1997");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(content);
                if (json.Value<bool>("success"))
                {
                    dynamic jo = JObject.Parse(content);
                    var characters = jo.characters;
                    List<Character> charactersList = new List<Character>();
                    foreach (var character in characters)
                    {
                        JObject Jcharacter = character as JObject;
                        Character charac = Jcharacter.ToObject<Character>();
                        charactersList.Add(charac);
                    }

                    var archivedcharacters = jo.archivedcharacters;
                    List<ArchivedCharacter> archivedCharactersList = new List<ArchivedCharacter>();
                    foreach (var character in archivedcharacters)
                    {
                        JObject Jcharacter = character as JObject;
                        ArchivedCharacter archived_character = Jcharacter.ToObject<ArchivedCharacter>();
                        archivedCharactersList.Add(archived_character);
                    }

                    if (charactersList.Count() > 0 && archivedCharactersList.Count() > 0)
                    {
                        ViewBag.Characters = charactersList;
                        ViewBag.ArchivedCharacters = archivedCharactersList;
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
            catch(Exception ex)
            {
                Console.WriteLine("EXCEPTION : " + ex);
                return StatusCode(404);
            }
        }

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

            if(Surname != null)
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

            if(TempData["error"] != null)
            {
                return RedirectToAction("UserProfile");
            }

            bool userChange = false;
            if(user.Surname != Surname)
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

            if(Avatar != null)
            {
                string fileExt = Path.GetExtension(Avatar.FileName).ToLower();
                string fileName = Path.GetFileName(Avatar.FileName);
                if (fileName != "")
                {
                    if (fileExt == ".jpg" || fileExt == ".gif" || fileExt == ".png")
                    {
                        if (Avatar.Length <= 2e+6)
                        {
                            if(user.Avatar != "default.jpg")
                            {
                                if(System.IO.File.Exists(_webHostEnvironment.WebRootPath + "/uploads/avatars/" + user.Avatar))
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

            if(userChange)
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