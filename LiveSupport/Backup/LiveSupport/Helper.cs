using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace LiveSupport
{
    public class Helper
    {
        internal static void SendEmail(MailAddress fromAddress, MailAddress toAddress, string subject, string body)
        {
            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };

            var client = new SmtpClient();
            client.Send(message);
        }

        internal static void SendNotifyMessage(MailAddress toAddress, string subject, string body)
        {
            MailAddress fromAddress = new MailAddress(LiveSupportSettings.Settings.NotificationSender);
            SendEmail(fromAddress, toAddress, subject, body);
        }

        internal static void SendNotifyMessage(string toAddress, string subject, string body)
        {
            MailAddress fromAddress = new MailAddress(LiveSupportSettings.Settings.NotificationSender);
            SendEmail(fromAddress, new MailAddress(toAddress), subject, body);
        }


        internal static string Linkify(string SearchText)
        {
            // this will find links like:
            // http://www.mysite.com
            // as well as any links with other characters directly in front of it like:
            // href="http://www.mysite.com"
            // you can then use your own logic to determine which links to linkify
            Regex regx = new Regex(@"\b(((\S+)?)(@|mailto\:|(news|(ht|f)tp(s?))\://)\S+)\b", RegexOptions.IgnoreCase);
            SearchText = SearchText.Replace("&nbsp;", " ");
            MatchCollection matches = regx.Matches(SearchText);

            foreach (Match match in matches)
            {
                if (match.Value.StartsWith("http"))
                { // if it starts with anything else then dont linkify -- may already be linked!
                    SearchText = SearchText.Replace(match.Value, "<a href='" + match.Value + "' target='_blank'>" + match.Value + "</a>");
                }
            }

            return SearchText;
        }





    }
}