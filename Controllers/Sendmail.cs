using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Configuration;

namespace SuperbrainManagement.Controllers
{
    public class Sendmail
    {
        public void SendMail( string to, string cc, string subject, string body)
        {
            // Thay đổi các thông tin sau đây để phù hợp với thông tin tài khoản email của bạn
            string smtpServer = "smtp.gmail.com"; // SMTP server của bạn
            int smtpPort = 587; // Port của SMTP server
            string username = ConfigurationManager.AppSettings["EmailUsername"];
            string password = ConfigurationManager.AppSettings["EmailPassword"];

            // Tạo một đối tượng MailMessage để chứa email
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(username);
            mail.To.Add(to);
            if (!string.IsNullOrEmpty(cc))
            {
                foreach (var ccAddress in cc.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.CC.Add(ccAddress.Trim());
                }
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true; // Thiết lập nội dung email là HTML (nếu cần)

            // Tạo một đối tượng SmtpClient để gửi email
            SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort);
            smtpClient.EnableSsl = true; // Kích hoạt SSL để gửi email an toàn
            smtpClient.UseDefaultCredentials = false; // Sử dụng thông tin đăng nhập riêng biệt
            smtpClient.Credentials = new NetworkCredential(username, password);
            try
            {
                smtpClient.Send(mail);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                smtpClient.Dispose();
            }
        }
    }
}