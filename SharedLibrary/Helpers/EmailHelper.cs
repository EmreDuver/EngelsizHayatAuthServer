using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Helpers
{
    public class EmailHelper 
    {
        public static void SendEmailForConfirmation(string link, string email)
        {
            var mail = new MailMessage();
            var smtpClient = new SmtpClient("mail.engelsizbirey.com");
            mail.From = new MailAddress("info@engelsizbirey.com");
            mail.To.Add(email);

            mail.Subject = $"Engelsiz Birey::Email Doğrulama";
            mail.Body = "<h4 >E postanızı onaylamak için lütfen aşağıdaki linke tıklayınız.</h4><br>";
            mail.Body += $"<a href='http://{link}'>E postayı onaylamak için için buraya tıklayın</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@engelsizbirey.com", "S6z6VH3tkMa6qXx!");
         

            smtpClient.SendMailAsync(mail);
        }

        public static void SendEmailForEmailChange(string link, string email)
        {
            var mail = new MailMessage();
            var smtpClient = new SmtpClient("mail.engelsizbirey.com");
            mail.From = new MailAddress("info@engelsizbirey.com");
            mail.To.Add(email);

            mail.Subject = $"Engelsiz Birey::Email Değiştirme İsteği";
            mail.Body = "<h4 >E postanızı değiştirmek için lütfen aşağıdaki linke tıklayınız.</h4><br>";
            mail.Body += $"<a href='http://{link}'>E postayı değiştirmek için için buraya tıklayın</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@engelsizbirey.com", "S6z6VH3tkMa6qXx!");
            smtpClient.SendMailAsync(mail);
        }

        public static void SendEmailForResetPassword(string link, string email)
        {
            var mail = new MailMessage();
            var smtpClient = new SmtpClient("mail.engelsizbirey.com");
            mail.From = new MailAddress("info@engelsizbirey.com");
            mail.To.Add(email);

            mail.Subject = $"Engelsiz Birey::Şifre Yenileme İsteği";
            mail.Body = "<h4 >Şifrenizi sıfırlamak için lütfen aşağıdaki linke tıklayınız.</h4><hr>";
            mail.Body += $"<a href='http://{link}'>Şifre sıfırlama için buraya tıklayın</a>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@engelsizbirey.com", "S6z6VH3tkMa6qXx!");
            smtpClient.SendMailAsync(mail);
        }

        public static void SendEmailForPassword(string email, string newPassword)
        {
            var mail = new MailMessage();
            var smtpClient = new SmtpClient("mail.engelsizbirey.com");

            mail.From = new MailAddress("info@engelsizbirey.com");
            mail.To.Add(email);

            mail.Subject = $"Engelsiz Birey::Yeni Şifreniz";
            mail.Body = $"<h4 >Yeni şifreniz : {newPassword}</h4>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@engelsizbirey.com", "S6z6VH3tkMa6qXx!");
            smtpClient.SendMailAsync(mail);
        }

        public static void SendEmailForOldAndNewEmail(string oldMail,string newMail)
        {
            var mail = new MailMessage();
            var smtpClient = new SmtpClient("mail.engelsizbirey.com");

            mail.From = new MailAddress("info@engelsizbirey.com");
            mail.To.Add(oldMail);
            mail.To.Add(newMail);

            mail.Subject = $"Engelsiz Hayat::Eposta değişikliği";
            mail.Body = "<h4 >Epostanız değiştirildi</h4>";
            mail.Body += $"<h4 >Eski postanız:{oldMail}</h4>";
            mail.Body += $"<h4 >Yeni postanız:{newMail}</h4>";
            mail.Body += "<h4 >Bilginiz dahilinde değilse lütfen bize bildiriniz.</h4>";
            mail.IsBodyHtml = true;
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false;
            smtpClient.Credentials = new System.Net.NetworkCredential("info@engelsizbirey.com", "S6z6VH3tkMa6qXx!");
            smtpClient.SendMailAsync(mail);
        }
    }
}
