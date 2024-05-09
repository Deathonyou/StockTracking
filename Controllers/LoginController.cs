using StockTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace StockTracking.Controllers
{
    
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public ActionResult userLogin()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult userLogin(User user)
        {
            //User İs Exist ?Control
            //Temp Use MD5
            using (var context = new StockTrackingContext())
            {
                var MD5Password = Helper.Encoder(user.UserPassword);
                var model = context.Users
                    .FirstOrDefault(u => u.UserName == user.UserName && u.UserPassword == MD5Password );
                if (model != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index","Auth");
                }
                else
                {
                    ViewBag.Error = 1;
                    return View();
                }
            }
        }

        public ActionResult userLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("userLogin");
        }
    }
}