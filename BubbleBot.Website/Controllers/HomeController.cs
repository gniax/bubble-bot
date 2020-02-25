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
using System.Net;
using BubbleBot.Website.Extensions;

namespace BubbleBot.Website.Controllers
{
    public class HomeController : Controller
    {

        // Fields
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly PanelDbContext _panelDbContext;

        // Constructor
        public HomeController(PanelDbContext panelDbContext)
        {
            _panelDbContext = panelDbContext;
        }


        [Route("")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("/Functionalities")]
        public IActionResult Functionalities()
        {
            return View();
        }

        [Route("/Galery")]
        public IActionResult Galery()
        {
            return View();
        }

        [Route("/Offers")]
        public IActionResult Offers()
        {
            ViewBag.SubscriptionsPlans = _panelDbContext.SubscriptionsPlans;
            return View();
        }


        [Route("/Register")]
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        #region VerifyEmail
        [Route("/VerifyEmail")]
        [HttpGet]
        public async Task<IActionResult> VerifyEmail(EmailVerificationViewModel lvm)
        {
            if (!ModelState.IsValid)
                return StatusCode(404);

            if (lvm.ValidationToken != null)
            {
                if(lvm.ValidationToken.Length == 24)
                {
                    bool result = false;
                    var task = Task.Run(() =>
                    {
                        User user = _panelDbContext.Users.FirstOrDefault(u => u.ValidationToken == lvm.ValidationToken);
                        if (user == null)
                            result = false;

                        else if (user.UserGroup != 5)
                            result = false;

                        else
                        {
                            user.UserGroup = 1;
                            _panelDbContext.Update(user);
                            _panelDbContext.SaveChanges();
                            result = true;
                        }
                    });

                    task.Wait();

                    if(!result)
                        return StatusCode(404);

                    return View();
                }
            }
            return StatusCode(404);
        }
        #endregion

        #region RegisterController
        [Route("/Register")]
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel lvm)
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

            if(lvm.Username.Length < 3 || lvm.Username.Length > 15)
            {
                ViewBag.ErrorMessage = "La longueur du nom d'utilisateur doit être comprise entre 3 et 15 caractères.";
                return View(lvm);
            }

            List<string> blackList = new List<string> { "administrateur", "admin", "root", "default", "modo", "moderateur", "pute", "bite", "niquetamere", "chatte", "fdp" };
            if(blackList.IndexOf(lvm.Username.ToLower()) != -1)
            {
                ViewBag.ErrorMessage = "Le nom d'utilisateur " + lvm.Username + " est interdit.";
                return View(lvm);
            }

            if (lvm.Password.Length < 5 || lvm.Username.Length > 20)
            {
                ViewBag.ErrorMessage = "La longueur du mot de passe doit être compris entre 5 et 20 caractères.";
                return View(lvm);
            }

            if(!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(lvm.EmailAddress))
            {
                ViewBag.ErrorMessage = "Le format de l'adresse e-mail est incorrecte...";
                return View(lvm);
            }

            var response = await _httpClient.GetAsync(Program.Constants.ApiIpAddress + $"/api/register?username={lvm.Username}&password={lvm.Password}&email={lvm.EmailAddress}&token=1997");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);

            if (json.Value<bool>("success"))
            {
                ViewBag.SuccessMessage = $"Compte <b>{lvm.Username}</b> crée avec succés. Activez votre compte à l'aide du mail d'activation envoyé à <b>{lvm.EmailAddress}</b>.";
                return View(lvm);
            }

            // success : false
            switch (json.Value<byte>("errorId"))
            {
                case 1:
                    ViewBag.ErrorMessage = "Le nom d'utilisateur existe déjà.";
                    break;
                case 2:
                    ViewBag.ErrorMessage = "L'adresse e-mail existe déjà.";
                    break;
                case 3:
                    ViewBag.ErrorMessage = "Le nom d'utilisateur et l'adresse e-mail existent déjà.";
                    break;
                case 4:
                    ViewBag.ErrorMessage = "Erreur lors de l'envoie du mail de validation => annulation de la création du compte...";
                    break;
            }
            try
            {
                
            }
            catch
            {
                ViewBag.ErrorMessage = "Erreur inconnu.";
            }

            return View(lvm);
        }
        #endregion Controller
    }
}
