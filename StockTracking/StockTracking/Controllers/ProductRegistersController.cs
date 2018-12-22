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
    public class ProductRegistersController : Controller
    {
        private StockTrackingContext db = new StockTrackingContext();


        // GET: ProductRegisters
        public ActionResult Index()
        {
            var productRegisters = db.ProductRegisters.Include(p => p.Product).Include(p => p.User);
            return View(productRegisters.ToList());
        }

        // GET: ProductRegisters/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRegister productRegister = db.ProductRegisters.Find(id);
            if (productRegister == null)
            {
                return HttpNotFound();
            }
            return View(productRegister);
        }

        // GET: ProductRegisters/Create
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName");
            return View();
        }

        // POST: ProductRegisters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RegisterID,UserID,ProductID,Quantity")] ProductRegister productRegister)
        {
            Product product = db.Products.Find(productRegister.ProductID);//product id si ile ürünü bulduk. ekleme işleminden sonra quantity kontrol işlemini yaptıracağız.
            if (ModelState.IsValid && (product.ProductQuantity - productRegister.Quantity) >= 0)//eğer miktar 0 veya üzeirndeyse işleme izin vermeyecek.
            {
                db.ProductRegisters.Add(productRegister);
                product.ProductQuantity = product.ProductQuantity - productRegister.Quantity;//quantity düşüyor. miktar kontrol edilip uuygunluğu kontrol edilecek.
                db.Entry(product).State = EntityState.Modified;//değişiklik kaydı.
                db.SaveChanges();//save change
                return RedirectToAction("Index");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", productRegister.UserID);
            return View(productRegister);
        }

        // GET: ProductRegisters/Edit/5
        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRegister productRegister = db.ProductRegisters.Find(id);

            if (productRegister == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", productRegister.UserID);
            return View(productRegister);
        }

        // POST: ProductRegisters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RegisterID,UserID,ProductID,Quantity")] ProductRegister productRegister)
        {
            Product product = new Product();
            product = db.Products.Where(w => w.ProductID == productRegister.ProductID).FirstOrDefault();//güncellenecek ürün bulundu
            ProductRegister tempProductReg = db.ProductRegisters.Where(w => w.RegisterID == productRegister.RegisterID).FirstOrDefault();//ilk değeri lazım olduğu için işlemleri yapmak için temp e atadık.


            if (ModelState.IsValid)
            {

                if (productRegister.Quantity < tempProductReg.Quantity) product.ProductQuantity += tempProductReg.Quantity - productRegister.Quantity;//daha düşük değere güncellendi iade var.
                else if (tempProductReg.Quantity == productRegister.Quantity) product.ProductQuantity = product.ProductQuantity; // işlem yok.
                else product.ProductQuantity -= productRegister.Quantity - tempProductReg.Quantity;//alım var stoktan düşüyoruz.


                tempProductReg.Quantity = productRegister.Quantity;//işlemler bittiğinde tempin quantity değerini formdan gelen değere eşitledik.
                db.Entry(tempProductReg).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", productRegister.UserID);
            return View(productRegister);

        }

        // GET: ProductRegisters/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductRegister productRegister = db.ProductRegisters.Find(id);
            if (productRegister == null)
            {
                return HttpNotFound();
            }
            return View(productRegister);
        }

        // POST: ProductRegisters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductRegister productRegister = db.ProductRegisters.Find(id);
            Product product = db.Products.Find(productRegister.ProductID);
            product.ProductQuantity += productRegister.Quantity;
            db.ProductRegisters.Remove(productRegister);
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
