using System;
using System.Net.Mail;
using System.Threading.Tasks;
using BubbleBot.Api;
using BubbleBot.Api.Extensions;
using BubbleBot.Website.Models;

namespace BubbleBot.Website.Services
{
    public class RegisterService
    {

        // Fields
        private readonly PanelDb _panelDb;


        // Constructors
        public RegisterService() { }

        public RegisterService(PanelDb panelDb)
        {
            _panelDb = panelDb;
        }


        public bool RegisterUser(string username, string password, string email, out int error)
        {
            error = -1;
            if (_panelDb == null)
                return false;

            int res = _panelDb.UserExists(username, email);
            switch(res)
            {
                case 1: // Username exists
                    error = 1;
                    break;
                case 2: // Email exists
                    error = 2;
                    break;
                case 3: // Both exist
                    error = 3;
                    break;
            }
            if (res > 0) return false;

            // User does not exist so I add it
            string salt = CryptographyExtension.GenerateString(8);
            string validationToken = CryptographyExtension.GenerateString(24);
            string newPassword = $"{salt.GetMD5()}{password.GetMD5()}".GetMD5();
            User user = new User(username, email, newPassword, salt, "default.jpg", 0, 0, 5, DateTime.Now, default(DateTime), validationToken);

            string subject = "Instructions d'activation";
            string body = "Bonjour <b>" + username + "</b> !</br>Vous allez enfin pouvoir profiter de toutes nos fonctionnalitées :) !</br></br>" +
                          "Pour confirmer et activer votre compte, veuillez cliquer sur ce lien :" +
                          "</br></br><a href=\"" + Program.Constants.VpsIpAddress + "/verifyEmail?validationToken=" + validationToken + "\"><b>Cliquez ici pour activer votre compte !</b></a>" +
                          "</br></br>Que les kamas vous bénissent !</br></br>" +
                          "[LogoBubble] </br></br>https://www.bubblebot.fr/";
            string attachmentPath = "images/logo_x128.png";

            var task = Task.Run(() => MailExtension.SendMail(email, subject, body, username, attachmentPath));
            task.Wait();
            bool sendMsg = task.Result;
            if (!sendMsg)
            {
                error = 4;
                return false;
            }

            _panelDb.Users.Add(user);
            _panelDb.SaveChanges();
            return true;
        }
    }
}
