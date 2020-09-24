using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;


namespace loginform.Models
{
    public class EmailVerification
    {
        PropertyData1Entities2 q = new PropertyData1Entities2();

        public void SendEmailToUser(string emailId, string activationCode)
        {
            try
            {

                var GenarateUserVerificationLink = "/registration/UserVerification/" + activationCode;
                var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, GenarateUserVerificationLink);

                var fromMail = new MailAddress("", "SRED"); // set your email  
                var fromEmailpassword = ""; // Set your password   
                var toEmail = new MailAddress(emailId);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

                var Message = new MailMessage(fromMail, toEmail);
                Message.Subject = "Registration Completed-Demo";
                Message.Body = "<br/> Your registration completed succesfully." +
                               "<br/> please click on the below link for account verification" +
                               "<br/><br/><a href=" + link + ">" + link + "</a>";
                Message.IsBodyHtml = true;
                smtp.Send(Message);

            }
            catch (Exception)
            {

            }
        }
      
        public bool IsEmailExists(string eMail)
        {
            var IsCheck = q.UserMs.Where(email => email.Email == eMail).FirstOrDefault();
            return IsCheck != null;
        }


     
    }
}