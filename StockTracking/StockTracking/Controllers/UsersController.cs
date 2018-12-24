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
    public class UsersController : Controller
    {
        private StockTrackingContext db = new StockTrackingContext();

        // GET: Users
        public ActionResult Index()
        {
            if (User.IsInRole("admin")|| User.IsInRole("staff"))
            {
                var users = db.Users.Include(u => u.Department).Include(u => u.UserRole);
                return View(users.ToList());
            }
            else
            {
               
                var userDepartmentID = db.Users.Where(u=>u.UserName==User.Identity.Name).Select(d=>d.DepartmentID);
                var UserSpecTeam = db.Users.Where(u => u.DepartmentID == (userDepartmentID.ToList().FirstOrDefault()));
                return View(UserSpecTeam.ToList());
            }
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        [Authorize(Roles = "admin")]
        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName");
            ViewBag.RoleID = new SelectList(db.UserRoles, "UserRoleID", "UserRoleName");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles ="admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,UserName,UserSurname,RoleID,DepartmentID,UserIsActive,UserPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.RoleID = new SelectList(db.UserRoles, "UserRoleID", "UserRoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Edit/5
        [Authorize(Roles ="admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.RoleID = new SelectList(db.UserRoles, "UserRoleID", "UserRoleName", user.RoleID);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,UserName,UserSurname,RoleID,DepartmentID,UserIsActive,UserPassword")] User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "DepartmentName", user.DepartmentID);
            ViewBag.RoleID = new SelectList(db.UserRoles, "UserRoleID", "UserRoleName", user.RoleID);
            return View(user);
        }

        // GET: Users/Delete/5
        [Authorize(Roles ="admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles ="admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
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
