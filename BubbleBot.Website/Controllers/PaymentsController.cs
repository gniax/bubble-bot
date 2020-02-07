using System;
using System.Collections.Generic;
using System.Linq;
using BubbleBot.Website.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayPal.Api;
using Microsoft.AspNetCore.Http;
using PayPal;

namespace BubbleBot.Website.Controllers
{
    public class PaymentsController : Controller
    {


        // Fields
        private readonly PanelDbContext _panelDbContext;


        // Constructor
        public PaymentsController(PanelDbContext panelDbContext)
        {
            _panelDbContext = panelDbContext;
        }

        [HttpPost]
        [Authorize]
        public IActionResult BuyPoints(int id)
        {
            var plan = _panelDbContext.GetPointsPlan(id);
            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);

            if (plan == null | user == null)
                return StatusCode(404);

            return ProceedBuyingPoints(plan);
        }

        [Authorize]
        public IActionResult CompleteBuyingPoints(string guid, byte result, string paymentId, string payerId)
        {
            string storedPaymentId = HttpContext.Session.GetString(guid);
            int planId = 0;

            if (storedPaymentId != null)
            {
                planId = HttpContext.Session.GetInt32(storedPaymentId) ?? 0;
                HttpContext.Session.Remove(storedPaymentId);
                HttpContext.Session.Remove(guid);
            }

            if (result != 1)
                return RedirectToAction("Points", "Panel");

            var user = _panelDbContext.GetUser(HttpContext.User.Identity.Name);
            if (user == null)
                return StatusCode(404);

            if (!string.IsNullOrEmpty(guid) && !string.IsNullOrEmpty(paymentId) && !string.IsNullOrEmpty(payerId))
            {
                var plan = _panelDbContext.GetPointsPlan(planId);

                // Check if this is a real payment
                if (plan != null && storedPaymentId != null && storedPaymentId == paymentId)
                {
                    try
                    {
                        // Execute the paiement
                        var paymentExecution = new PaymentExecution { payer_id = payerId };
                        var payment = new Payment { id = paymentId };
                        var apiContext = CreateApiContext();
                        payment = payment.Execute(apiContext, paymentExecution);

                        // Only continue if the payment is approved
                        if (payment.state == "approved")
                        {
                            var transaction = payment.transactions.SingleOrDefault();

                            // Check if the payment is really valid
                            if (transaction != null && transaction.amount.currency == "EUR" && transaction.amount.total == plan.FormattedPrice)
                            {
                                _panelDbContext.PaypalTransactions.Add(new PaypalTransaction(payment.id, payment.payer.payer_info.email, payment.payer.payer_info.payer_id, plan.Id, user.Id));
                                user.Points += plan.Points;
                                _panelDbContext.SaveChanges();
                            }
                        }
                    }
                    catch (PayPalException)
                    {
                        return RedirectToAction("Points", "Panel", new { error = 1 });
                    }
                }
            }

            return RedirectToAction("Points", "Panel");
        }

        private IActionResult ProceedBuyingPoints(PointsPlan plan)
        {
            try
            {
                var apiContext = CreateApiContext();

                var guid = Convert.ToString(new Random().Next(100000));
                //var redirectUrl = $"https://BubbleBot.net/Payments/CompleteBuyingPoints?guid={guid}&result=";
                var redirectUrl = Program.Constants.VpsIpAddress + $"/Payments/CompleteBuyingPoints?guid={guid}&result=";

                var payment = Payment.Create(apiContext, new Payment
                {
                    intent = "sale",
                    payer = new Payer
                    {
                        payment_method = "paypal"
                    },
                    transactions = new List<Transaction>
                    {
                        new Transaction
                        {
                            description = $"{plan.Name} - {plan.Points} Points",
                            amount = new Amount { currency = "EUR", total = plan.FormattedPrice },
                            item_list = new ItemList { items = new List<Item>{ new Item { name = plan.Name, currency = "EUR", price = plan.FormattedPrice, quantity = "1" } } }
                        }
                    },
                    redirect_urls = new RedirectUrls
                    {
                        return_url = $"{redirectUrl}1",
                        cancel_url = $"{redirectUrl}2"
                    }
                });

                if (payment.state == "created")
                {
                    HttpContext.Session.SetString(guid, payment.id);
                    HttpContext.Session.SetInt32(payment.id, plan.Id);

                    return Redirect(payment.GetHateoasLink("approval_url").href);
                }
            }
            catch { }

            TempData["error"] = "Erreur lors de la création du paiement.";
            return RedirectToAction("Points", "Panel");
        }

        private static APIContext CreateApiContext()
        {
            var config = new Dictionary<string, string>
            {
                { "clientId", "AfwuhHy9RzbPjNQTaGwOQK9jYJioSVapOFu7RizYaCHc1Cu7D5FdJt-XKIz5cuBbkNG62r_ohWMk-amI" },
                { "clientSecret", "EP-0aLQQVe70jfVZzbB1PEGMB65tubz4q_OkqqWkADYLwQJoJ70FMDUKR5vG2_bME5nOtiBOC3ijDZGf" },
                { "mode", "live" }
            };

            var accessToken = new OAuthTokenCredential(config);
            return new APIContext(accessToken.GetAccessToken()) { Config = config };
        }

    }
}
