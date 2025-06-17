using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory_Sys.Models;
using FrontCode.Libraries;

namespace Inventory_Sys.Controllers
{
    public class LoginController : Controller
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Employee_tbl emp)
        {
            var verifiedPassowrd = Hashing.VerifyPassword(emp.Password);

            var employee = db.Employee_tbl.FirstOrDefault(x => x.Email == emp.Email && x.Password == verifiedPassowrd);
            var admin = db.Admin_tbl.FirstOrDefault(x => x.Email == emp.Email && x.Password == verifiedPassowrd);
            if (employee != null)
            {
                Session["username"] = employee.FirstName;
                Session["Surname"] = employee.LastName;
                Session["Role"] = employee.Role;
                return RedirectToAction("Index","Home");
            }
            if (admin != null)
            {
                Session["username"] = admin.FirstName;
                Session["Surname"] = admin.LastName;
                Session["Role"] = admin.Role;
                return RedirectToAction("Index", "Home");
            }
            // If credentials are not found, add a model error and return to the login view
            ModelState.AddModelError(string.Empty, "Invalid email or password");
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login","Login");
        }
    }
}