using System.Net.Mail;
using System.Net;

namespace PHDCommunityFinal.LoginMail
{

    public interface IEmailVerification
    {

        public void SendEmail(string toEmail, string subject, string body);
    }
    public class EmailVerifications:IEmailVerification
    {
        private readonly IConfiguration _configuration;

        public EmailVerifications(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail(string toEmail, string subject, string body)
        {
            var smtpServer = "smtp.gmail.com";
            //_configuration["EmailSettings:SmtpServer"];
            var smtpPort = 587;
            //int.Parse(_configuration["EmailSettings:SmtpPort"]);
            var senderEmail = "arulgnanapragasamc@gmail.com";
            //_configuration["EmailSettings:SenderEmail"];
            var senderPassword = "ajrt sfkz ibes ftdn";
            //_configuration["EmailSettings:SenderPassword"];

            try
            {
                var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(toEmail);

                smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
