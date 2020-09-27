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
                var GenarateUserVerificationLink = "registration/userverification/" + activationCode;
                var link = HttpContext.Current.Request.Url.AbsoluteUri + GenarateUserVerificationLink;

                var fromMail = new MailAddress("jahanzaibshahiddeveloper@gmail.com", "SRED"); // set your email  
                var fromEmailpassword = "imbsd@ilc#"; // Set your password   
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

        public string GeneratePassword()
        {
            string OTPLength = "4";
            string OTP = string.Empty;

            string Chars = string.Empty;
            Chars = "1,2,3,4,5,6,7,8,9,0";

            char[] seplitChar = { ',' };
            string[] arr = Chars.Split(seplitChar);
            string NewOTP = "";
            string temp = "";
            Random rand = new Random();
            for (int i = 0; i < Convert.ToInt32(OTPLength); i++)
            {
                temp = arr[rand.Next(0, arr.Length)];
                NewOTP += temp;
                OTP = NewOTP;
            }
            return OTP;
        }

        public void ForgetPasswordEmailToUser(string emailId, string activationCode, string OTP)
        {
            try
            {

                var GenarateUserVerificationLink = "/registration/ChangePassword/";
                var link = HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.PathAndQuery, GenarateUserVerificationLink);

                var fromMail = new MailAddress("jahanzaibshahiddeveloper@gmail.com", "SRED"); // set your email  
                var fromEmailpassword = "imbsd@ilc#"; // Set your password   
                var toEmail = new MailAddress(emailId);

                var smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(fromMail.Address, fromEmailpassword);

                var Message = new MailMessage(fromMail, toEmail);
                Message.Subject = "Forget Password Completed-Demo";
                Message.Body = "<br/> please click on the below link for password change" +
                               "<br/><br/><a href=" + link + ">" + link + "</a>" +
                               "<br/> OTP for password change: " + OTP +
                               "<br/> OTP for activation code: " + activationCode;
                Message.IsBodyHtml = true;
                smtp.Send(Message);

            }
            catch (Exception)
            {

            }
        }

    }
}