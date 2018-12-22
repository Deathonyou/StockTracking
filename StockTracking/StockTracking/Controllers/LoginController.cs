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
                var model = context.Users.FirstOrDefault(u => u.UserName == user.UserName && u.UserPassword == user.UserPassword);
                if (model != null)
                {
                    FormsAuthentication.SetAuthCookie(user.UserName, false);
                    return RedirectToAction("Index", "Departments");
                }
                else
                {
                    ViewBag.Error = 1;
                    return View();
                }
            }
        }
    }
}