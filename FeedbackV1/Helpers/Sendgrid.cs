using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;
using FeedbackV1.Models;

namespace FeedbackV1.Helpers
{
    internal class Sendgrid
    {


        static async Task SendGridEmail(User sender, User receiver)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(sender.Email, sender.Name);
            var subject = "Sending with Twilio SendGrid is Fun";
            var to = new EmailAddress(receiver.Email, receiver.Name);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
           
        }
    }
}
