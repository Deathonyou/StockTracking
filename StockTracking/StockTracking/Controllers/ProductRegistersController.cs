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
            if (User.IsInRole("admin") || User.IsInRole("staff"))
            {
                var productRegisters = db.ProductRegisters.Include(p => p.Product).Include(p => p.User);
                return View(productRegisters.ToList());

            }
            else
            {
                var DepartmentID = db.Users.Where(u => u.UserName == User.Identity.Name).Select(d => d.DepartmentID).FirstOrDefault();

                var productRegistersSpec = db.ProductRegisters
                    .Include(p => p.Product)
                    .Include(p => p.User)
                    .Where(u=>u.User.DepartmentID == DepartmentID);
                return View(productRegistersSpec.ToList());
            }
           
            
        }

        // GET: ProductRegisters/Details/5
        [Authorize(Roles ="admin,staff")]
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
        [Authorize(Roles ="admin,staff")]
        public ActionResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products.Where(p => p.ProductStockState == true), "ProductID", "ProductName");
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName");
            return View();
        }

        // POST: ProductRegisters/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,staff")]
        public ActionResult Create([Bind(Include = "RegisterID,UserID,ProductID,Quantity")] ProductRegister productRegister)
        {
            Product product = db.Products.Find(productRegister.ProductID);//product id si ile ürünü bulduk. ekleme işleminden sonra quantity kontrol işlemini yaptıracağız.
            if (product.ProductQuantity - productRegister.Quantity == 0) product.ProductStockState = false;// ürün adedi 0 a düştüyse durumu false yap
            if (ModelState.IsValid && (product.ProductQuantity - productRegister.Quantity) >= 0)//eğer miktar 0 veya üzeirndeyse işleme izin vermeyecek.
            {
                db.ProductRegisters.Add(productRegister);
                product.ProductQuantity = product.ProductQuantity - productRegister.Quantity;//quantity düşüyor. miktar kontrol edilip uuygunluğu kontrol edilecek.
                db.Entry(product).State = EntityState.Modified;//değişiklik kaydı.
                db.SaveChanges();//save change
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.StockState= "Insufficient Stock! Available Stock : "+product.ProductQuantity;
            }

            ViewBag.ProductID = new SelectList(db.Products.Where(u=>u.ProductStockState==true), "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName", productRegister.UserID);
            return View(productRegister);
        }

        // GET: ProductRegisters/Edit/5
        [Authorize(Roles = "admin,staff")]
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
            ViewBag.ProductID = new SelectList(db.Products.Where(p=>p.ProductStockState==true), "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName", productRegister.UserID);
            return View(productRegister);
        }

        // POST: ProductRegisters/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin,staff")]
        public ActionResult Edit([Bind(Include = "RegisterID,UserID,ProductID,Quantity")] ProductRegister productRegister)
        {
            Product product = new Product();
            product = db.Products.Where(w => w.ProductID == productRegister.ProductID).FirstOrDefault();//güncellenecek ürün bulundu
            ProductRegister tempProductReg = db.ProductRegisters.Where(w => w.RegisterID == productRegister.RegisterID).FirstOrDefault();//ilk değeri lazım olduğu için işlemleri yapmak için temp e atadık.
            bool stockQuantityIsValidToUpdate;
            if ((Convert.ToInt32(productRegister.Quantity) > Convert.ToInt32(tempProductReg.Quantity)) && Convert.ToInt32(product.ProductQuantity - (Math.Abs(Convert.ToInt32(productRegister.Quantity - tempProductReg.Quantity)))) >= 0)
                stockQuantityIsValidToUpdate = true;
            else if ((Convert.ToInt32(productRegister.Quantity) < Convert.ToInt32(tempProductReg.Quantity)))
                stockQuantityIsValidToUpdate = true;
            else
                stockQuantityIsValidToUpdate = false;
            //Burada güncellenen miktar eğer önceki miktardan büyükse ve stok bunu karşılıyorsa true 
            //girilen miktar daha düşükse stokun önemi yok true 
            //şartlar karşılanmıyorsa false


            if (ModelState.IsValid && stockQuantityIsValidToUpdate ==true)
            {

                if (productRegister.Quantity < tempProductReg.Quantity) product.ProductQuantity += tempProductReg.Quantity - productRegister.Quantity;//daha düşük değere güncellendi iade var.
                else if (tempProductReg.Quantity == productRegister.Quantity) product.ProductQuantity = product.ProductQuantity; // işlem yok.
                else product.ProductQuantity -= productRegister.Quantity - tempProductReg.Quantity;//alım var stoktan düşüyoruz.
                
                db.Entry(product).State = EntityState.Modified;

                if (product.ProductQuantity == 0)
                {
                    product.ProductStockState = false;
                }
                else product.ProductStockState = true;

                tempProductReg.Quantity = productRegister.Quantity;//işlemler bittiğinde tempin quantity değerini formdan gelen değere eşitledik.
                db.Entry(tempProductReg).State = EntityState.Modified;
                db.SaveChanges(); 
             
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.StockState = "Insufficient Stock!Available Stock: " + product.ProductQuantity;
            }
            ViewBag.ProductID = new SelectList(db.Products.Where(u => u.ProductStockState == true), "ProductID", "ProductName", productRegister.ProductID);
            ViewBag.UserID = new SelectList(db.Users.Where(u=>u.UserIsActive==true), "UserID", "UserName", productRegister.UserID);
            return View(productRegister);

        }

        // GET: ProductRegisters/Delete/5
        [Authorize(Roles = "admin,staff")]
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
        [Authorize(Roles = "admin,staff")]
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
