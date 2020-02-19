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

namespace BubbleBot.Website.Controllers
{
    public class PanelController : Controller
    {

        // Fields
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly PanelDbContext _panelDbContext;


        // Constructor
        public PanelController(PanelDbContext panelDbContext)
        {
            _panelDbContext = panelDbContext;
        }


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
        public IActionResult BotsStats()
        {
            return HandleAuthorizedAction();
        }

        [Authorize]
        public IActionResult User()
        {
            return HandleAuthorizedAction();
        }

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