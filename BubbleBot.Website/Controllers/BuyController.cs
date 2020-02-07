using System;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using BubbleBot.Website.Models;
using Microsoft.AspNetCore.Authorization;

namespace BubbleBot.Website.Controllers
{
    public class BuyController : Controller
    {

        // Fields
        private readonly PanelDbContext _panelDbContext;


        // Constructor
        public BuyController(PanelDbContext panelDbContext)
        {
            _panelDbContext = panelDbContext;
        }


        [HttpPost]
        [Authorize]
        [Route("/Panel/BuySubscription")]
        public IActionResult BuySubscription(int id)
        {
            var plan = _panelDbContext.GetSubscriptionPlan(id);
            if (plan == null)
                return StatusCode(404);

            if (plan.Id == 1)
                return RedirectToAction("Downloads", "Panel");

            ViewBag.SubscriptionToBuy = plan;
            return HandleAuthorizedAction();
        }

        [HttpPost]
        [Authorize]
        public IActionResult BuySubscriptionConfirm(int id, int offer)
        {
            // The only possible offers are 2 weeks and 4 weeks
            if (offer != 2 && offer != 4)
                return StatusCode(404);

            var plan = _panelDbContext.GetSubscriptionPlan(id);
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);

            // You can't buy the starter pack
            if (plan == null || user == null || plan.Id == 1)
                return StatusCode(404);

            double cost = plan.Price * (offer == 2 ? 1 : 2);

            // Check if the user has enough points to afford this
            if (user.Points < cost)
            {
                TempData["error"] = "Vous n'avez pas assez de points.";
            }
            else
            {
                // Remove the points from the user
                user.Points -= cost;

                // If the user wasn't subscribed, reset the days
                if (plan.Id == 2)
                {
                    user.TouchEndDate = !user.IsSubscribedToTouch ? DateTime.Now.AddDays(offer * 7) : user.TouchEndDate.Value.AddDays(offer * 7);
                }

                _panelDbContext.SubscriptionsBought.Add(new SubscriptionBought(plan.Id, user.Id, offer));
                _panelDbContext.SaveChanges();
            }

            return RedirectToAction("Subscriptions", "Panel");
        }

        [HttpPost]
        [Authorize]
        [Route("/Panel/BuyExtension")]
        public IActionResult BuyExtension(int id)
        {
            var extension = _panelDbContext.GetExtension(id);
            if (extension == null)
                return StatusCode(404);

            ViewBag.ExtensionToBuy = extension;
            return HandleAuthorizedAction();
        }

        [HttpPost]
        [Authorize]
        public IActionResult BuyExtensionConfirm(int id, int weeks)
        {
            var extension = _panelDbContext.GetExtension(id);
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);

            if (extension == null || user == null)
                return StatusCode(404);

            double cost = extension.Price * weeks;

            // Check if the user has enough points to afford this
            if (user.Points < cost)
            {
                TempData["error"] = "Vous n'avez pas assez de points.";
            }
            else
            {
                // Remove the points from the user
                user.Points -= cost;

                _panelDbContext.ExtensionsBought.Add(new ExtensionBought(extension.Id, user.Id, weeks));
                _panelDbContext.SaveChanges();
            }

            return RedirectToAction("Extensions", "Panel");
        }

        private IActionResult HandleAuthorizedAction([CallerMemberName]string caller = "")
        {
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return StatusCode(404);

            return View($"~/Views/Panel/{caller}.cshtml", user);
        }

    }
}
