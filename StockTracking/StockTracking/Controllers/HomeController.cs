using StockTracking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StockTracking.Controllers
{
    public class HomeController : Controller
    {
        StockTrackingContext context = new StockTrackingContext();

        public ActionResult Index()
        {
            var model = context.Brands.ToList();
            return View("Index",model);
        }

       

       
    }
}