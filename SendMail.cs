using RestSharp;
using System;
using System.Configuration;

namespace UpdateSQLPassword
{
    class SendMail
    {
        public static IRestResponse SendEmail(string newPassword)
        {
            string MailBody = string.Format(Constants.MAIL_BODY, newPassword);
            RestClient client = new RestClient();
            client.BaseUrl = "https://api.mailgun.net/v2";
            string apiKey = Convert.ToString(ConfigurationManager.AppSettings["APIKey"]);
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.Authenticator = new HttpBasicAuthenticator("api", apiKey);//Api Authentication
            }
            else
            {
                Console.WriteLine(Constants.MISSING_API_KEY_VALUE);
            }
            //Container for data used to make requests
            RestRequest request = new RestRequest();
            string domainName = Convert.ToString(ConfigurationManager.AppSettings["DomainName"]);
            if (!string.IsNullOrEmpty(domainName))
            {
                request.AddParameter("domain", domainName, ParameterType.UrlSegment);
            }
            else
            {
                Console.WriteLine(Constants.MISSING_DOMAIN_KEY_VALUE);
            }
            request.Resource = "{domain}/messages";
            string fromEmail = Convert.ToString(ConfigurationManager.AppSettings["FromEmail"]);
            if (!string.IsNullOrEmpty(fromEmail))
            {
                request.AddParameter("from", fromEmail);
            }
            else
            {
                Console.WriteLine(Constants.MISSING_FROM_EMAIL_KEY_VALUE);
            }
            string toEmail = Convert.ToString(ConfigurationManager.AppSettings["ToEmail"]);
            if (!string.IsNullOrEmpty(toEmail))
            {
            request.AddParameter("to", toEmail);
            //request.AddParameter("to", toEmail);
            }
            else
            {
                Console.WriteLine(Constants.MISSING_TO_EMAIL_KEY_VALUE);
            }
            string ccEmail = Convert.ToString(ConfigurationManager.AppSettings["CcEmail"]);
            if (!string.IsNullOrEmpty(ccEmail))
            {
            request.AddParameter("cc",ccEmail );
            }
            string subjectEmail = Convert.ToString(ConfigurationManager.AppSettings["SubjectEmail"]);
            if (!string.IsNullOrEmpty(subjectEmail))
            {
                request.AddParameter("subject", subjectEmail);
            }
            else
            {
                Console.WriteLine(Constants.MISSING_SUBJECT_MESSAGE);
            }
            request.AddParameter("text", "");
            request.AddParameter("html", MailBody);
            request.Method = Method.POST;
            var response = client.Execute(request);
            return response;
        }

    }
}
