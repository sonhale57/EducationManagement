using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;

namespace SuperbrainManagement.Helpers
{
    public class SendEmailHelper
    {
        public void SendEmailWithTemplate(string toEmail, string subject, dynamic model, string template)
        {
            // Đường dẫn tới template HTML
            var templatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", template);

            // Đọc nội dung của file template
            string body = File.ReadAllText(templatePath);

            // Tùy chỉnh template với dữ liệu trong model
            body = body.Replace("@Model.TenNguoiNhan", model.TenNguoiNhan);
            body = body.Replace("@Model.LinkXacNhan", model.LinkXacNhan);

            // Cấu hình email
            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("superbrain.noreply@gmail.com", "gkodzhiacgwrbsvf"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("superbrain.noreply@gmail.com"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true, // Đặt IsBodyHtml thành true để gửi nội dung HTML
            };

            mailMessage.To.Add("sonhale57@gmail.com");

            // Gửi email
            smtpClient.Send(mailMessage);
        }
    }
}