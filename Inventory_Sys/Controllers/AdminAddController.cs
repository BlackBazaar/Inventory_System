using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory_Sys.Models;
using FrontCode.Libraries;
using System.Text.RegularExpressions;

namespace Inventory_Sys.Controllers
{
    public class AdminAddController : BaseController
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();
        public ActionResult Admins()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            var admins = from k in db.Admin_tbl select k;
            return View(admins.ToList());
        }

        public ActionResult NewAdmin()
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewAdmin(Admin_tbl admin)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            // Check if the email already exists
            var existEmployee = db.Employee_tbl.FirstOrDefault(x => x.Email == admin.Email);
            var exist = db.Admin_tbl.FirstOrDefault(x => x.Email == admin.Email);
            if (exist != null || existEmployee != null)
            {
                ModelState.AddModelError("Email", "This email already exists.");
            }

            // Validate password
            if (!IsValidPassword(admin.Password))
            {
                ModelState.AddModelError("Password", "Password must be 9-20 characters long and include uppercase letters, lowercase letters, numbers, and symbols.");
            }
            else
            {
                var hashedPassword = Hashing.CreateHash(admin.Password);
                admin.Password = hashedPassword;
            }

            // Validate phone number length (if provided)
            if (!string.IsNullOrEmpty(admin.PhoneNumber) && admin.PhoneNumber.Length != 10)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number must be 10 digits long.");
            }

            // Check if there are any model errors
            if (!ModelState.IsValid)
            {
                return View(admin); // Return the view with the entered data to correct errors
            }

            // Add new admin to database
            db.Admin_tbl.Add(admin);
            db.SaveChanges();

            return RedirectToAction("Admins", "AdminAdd");
        }

        private bool IsValidPassword(string password)
        {
            // Password must be between 9 to 20 characters and include uppercase, lowercase, numbers, and symbols
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{9,20}$");
        }

    public ActionResult AdminRemove(int id)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var admin = db.Admin_tbl.Find(id);
            var DeviceAssignment = db.DeviceAssignment_tbl.FirstOrDefault(a => a.AssignedByAdminID == id);

            if (DeviceAssignment != null)
            {
                DeviceAssignment.AssignedByAdminID = null;
            }

            


            db.Admin_tbl.Remove(admin);
            db.SaveChanges();

            return RedirectToAction("Admins", "AdminAdd");
        }
        public ActionResult AdminEdit(int id)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var admin = db.Admin_tbl.Find(id);

            return View(admin);
        }

        [HttpPost]
        public ActionResult AdminEdit(Admin_tbl adm)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var admin = db.Admin_tbl.Find(adm.AdminID);

            if (!string.IsNullOrEmpty(admin.PhoneNumber) || admin.PhoneNumber.Length != 10)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number must be 10 digits long.");
            }

            admin.FirstName = adm.FirstName;
            admin.LastName = adm.LastName;
            admin.Department = adm.Department;
            admin.Role = adm.Role;
            admin.PhoneNumber = adm.PhoneNumber;

            var hashedpassw = Hashing.CreateHash(adm.Password);
            admin.Password = hashedpassw;

            db.SaveChanges();

            return RedirectToAction("Admins", "AdminAdd");
        }


    }
}