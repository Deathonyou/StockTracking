using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StockTracking.Models;

namespace StockTracking.Controllers
{
    [Authorize(Roles = "admin,staff")]
    public class ProductsController : Controller
    {
        private StockTrackingContext db = new StockTrackingContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Brand).Include(p => p.ProductType).Include(p => p.User);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName");
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "ProductName");
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,ProductTypeID,ProductStockState,BrandID,AcceptDate,ProductPrice,ProductQuantity,UserID")] Product product)
        {
            
            if (product.ProductQuantity==0)
                product.ProductStockState = false;
            
            if (ModelState.IsValid && !(product.ProductQuantity<0))
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "ProductName", product.ProductTypeID);
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName", product.UserID);
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "ProductName", product.ProductTypeID);
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName", product.UserID);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,ProductDescription,ProductTypeID,ProductStockState,BrandID,AcceptDate,ProductPrice,ProductQuantity,UserID")] Product product)
        {
            if (product.ProductQuantity == 0)
                product.ProductStockState = false;

            if (ModelState.IsValid && !(product.ProductQuantity<0))
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "BrandID", "BrandName", product.BrandID);
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ProductTypeID", "ProductName", product.ProductTypeID);
            ViewBag.UserID = new SelectList(db.Users.Where(u => u.UserIsActive == true), "UserID", "UserName", product.UserID);
            return View(product);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
