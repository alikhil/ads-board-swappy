using Microsoft.AspNet.Identity;
using NLog;
using SendGrid;
using Swappy_V2.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Swappy_V2.Modules
{
    public static class EmailModule
    {
        public static async Task SendAsync(string from, IdentityMessage message)
        {
            var myMessage = new SendGridMessage();
            myMessage.AddTo(message.Destination);
            myMessage.From = new System.Net.Mail.MailAddress(
                                from, "Swappy Inc ©");
            myMessage.Subject = message.Subject;
            myMessage.Text = message.Body + "<p>С уважением, команда Swappy.ru</p>";
            myMessage.Html = message.Body + "<p>С уважением, команда Swappy.ru</p>";

            // Create a Web transport for sending email.
            var transportWeb = new Web("***REMOVED***", TimeSpan.FromSeconds(4));

            // Send the email.
            if (transportWeb != null)
            {
                await transportWeb.DeliverAsync(myMessage);
            }
            else
            {
                LogManager.GetCurrentClassLogger().Warn("Не удалось создать Web transport");
                await Task.FromResult(0);
            }
        }
    }
}