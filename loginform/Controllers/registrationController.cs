using loginform.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;

namespace loginform.Controllers
{
    public class registrationController : Controller
    {

        PropertyData1Entities2 q = new PropertyData1Entities2();
        EmailVerification email = new EmailVerification();
        
        // GET: registration
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserM objUsr)
        {
            try
            {
               
                // email not verified on registration time  
                objUsr.EmailVerification = false;

                var check = email.IsEmailExists(objUsr.Email);
                if (check)
                {
                    ModelState.AddModelError("EmailExists", "Email already exist");
                    return View();
                }

                //it generate unique code     
                objUsr.ActivetionCode = Guid.NewGuid();

                //password convert  
                objUsr.Password = encryptPassword.texttoExcript(objUsr.Password);
                q.UserMs.Add(objUsr);
                q.SaveChanges();

                var message = "Registration completed please check email: " + objUsr.Email;
                ViewBag.message = message;

                email.SendEmailToUser(objUsr.Email, objUsr.ActivetionCode.ToString());
             
            }
            catch (Exception)
            {
                var message = "Registration not completed please try again:";
                ViewBag.message = message;
            }

            return View();
        }


        public ActionResult UserVerification(string id)
        {
            try
            {
                bool Status = false;

                q.Configuration.ValidateOnSaveEnabled = false; // Ignore to password confirmation   
                var IsVerify = q.UserMs.Where(u => u.ActivetionCode == new Guid(id)).FirstOrDefault();

                if (IsVerify != null)
                {
                    IsVerify.EmailVerification = true;
                    q.SaveChanges();
                    ViewBag.Message = "Email Verification completed";
                    ViewBag.Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request...Email not verify";
                    ViewBag.Status = false;
                }

            }
            catch (Exception)
            {

                ViewBag.Message = "Invalid Request...Email not verify";
                ViewBag.Status = false;
            }

            return View();
        }

        
        // GET: login
        public ActionResult Login()
        {
            return View();
        }

        // GET: login
        [HttpPost]
        public ActionResult Login(UserLogin LgnUsr)
        {
            try
            {
                var _passWord = encryptPassword.texttoExcript(LgnUsr.Password);

                bool Isvalid = q.UserMs.Any(x => x.Email == LgnUsr.EmailId && x.EmailVerification == true &&

                x.Password == _passWord);

                if (Isvalid)
                {
                    int timeout = LgnUsr.Rememberme ? 60 : 5; // Timeout in minutes, 60 = 1 hour.  
                    var ticket = new FormsAuthenticationTicket(LgnUsr.EmailId, false, timeout);
                    string encrypted = FormsAuthentication.Encrypt(ticket);
                    var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                    cookie.Expires = System.DateTime.Now.AddMinutes(timeout);
                    cookie.HttpOnly = true;
                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Signin");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Information... Please try again!");
                }
            }
            catch (Exception)
            {

            }

            return View();
        }


        [Authorize]
        public ActionResult Signin()
        {
            return View();
        }


        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "registration");
        }


        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(ForgetPassword pass)
        {
            try
            {

                var IsExists = email.IsEmailExists(pass.EmailId);
                if (!IsExists)
                {
                    ModelState.AddModelError("EmailNotExists", "This email is not exists");
                    return View();
                }
                var objUsr = q.UserMs.Where(x => x.Email == pass.EmailId).FirstOrDefault();

                // Genrate OTP   
                string OTP = email.GeneratePassword();

                objUsr.ActivetionCode = Guid.NewGuid();
                objUsr.OTP = OTP;
                q.Entry(objUsr).State = System.Data.EntityState.Modified;
                q.SaveChanges();

                email.ForgetPasswordEmailToUser(objUsr.Email, objUsr.ActivetionCode.ToString(), objUsr.OTP);

                ViewBag.message = "verification code to this email: " + objUsr.Email;

            }
            catch (Exception)
            {


            }

            return View();
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword ch)
        {
            try
            {

                var objUsr = q.UserMs.Where(x => x.OTP == ch.OTP).FirstOrDefault();

                
                bool Status = false;
                if (objUsr == null)
                {
                    ViewBag.Status = false;
                    ViewBag.Message = "Invalid OTP code...";
                }
                else
                {
                    if (ch.OTP == objUsr.OTP && ch.ActivationCode.ToString() == objUsr.ActivetionCode.ToString())
                    {
                        var _passWord = encryptPassword.texttoExcript(ch.Password);
                        objUsr.Password = _passWord;
                        q.Entry(objUsr).State = System.Data.EntityState.Modified;
                        q.SaveChanges();
                        ViewBag.Status = true;
                        ViewBag.Message = "Password Successfully Change...!!!";
                       
                    }
                    else
                    {
                        ViewBag.Status = false;
                        ViewBag.Message = "Invalid Activation Code...";
                    }
                }
            }
            catch (Exception)
            {


            }
            return View();
        }
    }
}