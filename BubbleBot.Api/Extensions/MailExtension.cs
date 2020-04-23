using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace BubbleBot.Api.Extensions
{
    public class MailExtension
    {
        public static class Constants
        {
            public static MailAddress fromAddress = new MailAddress("no-reply@example.com", "Bubble Bot");
            public static string fromPassword = "CHANGE_ME";
            public static string smtpHost = "smtp.gmail.com";
            public static int smtpPort = 587;
            public static bool smtpSsl = true;
        }

        public static bool SendMail(string destAddr, string subject, string body, string username, string attachmentPath = null)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = Constants.smtpHost,
                    Port = Constants.smtpPort,
                    EnableSsl = Constants.smtpSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(Constants.fromAddress.Address, Constants.fromPassword)
                };

                MailAddress toAddress = new MailAddress(destAddr, username);
                using (var mail = new MailMessage(Constants.fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    if (attachmentPath != null)
                    {
                        try
                        {
                            LinkedResource inlineLogo = new LinkedResource(attachmentPath);
                            inlineLogo.ContentId = "Logo";
                            inlineLogo.TransferEncoding = TransferEncoding.Base64;
                            inlineLogo.ContentType.Name = inlineLogo.ContentId;
                            inlineLogo.ContentLink = new Uri("cid:" + inlineLogo.ContentId);
                            string newBody = mail.Body.Replace("[LogoBubble]", string.Format(@"<img src='cid:{0}'/>", inlineLogo.ContentId));
                            AlternateView view = AlternateView.CreateAlternateViewFromString(newBody, Encoding.UTF8, MediaTypeNames.Text.Html);
                            view.LinkedResources.Add(inlineLogo);
                            mail.AlternateViews.Add(view);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Erreur {0} \n Chemin: {1} vers la pièce jointe introuvable ...", ex, attachmentPath);
                        }
                    }
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.IsBodyHtml = true;
                    mail.Priority = MailPriority.Normal;

                    smtp.Send(mail);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur :" + ex);
                return false;
            }
        }
    }

}
