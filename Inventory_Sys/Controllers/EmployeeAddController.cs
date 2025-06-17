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
    public class EmployeeAddController : BaseController
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();
        public ActionResult Employees()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var employees = from k in db.Employee_tbl select k;
            return View(employees.ToList());
        }

        public ActionResult NewEmployee()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            return View();
        }

        [HttpPost]
        public ActionResult NewEmployee(Employee_tbl emp)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            // Validate email format
            if (!IsValidEmail(emp.Email))
            {
                ModelState.AddModelError("Email", "Email is not in a valid format");
            }

            // Validate phone number length
            if (emp.PhoneNumber != null && emp.PhoneNumber.Length != 10)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number must be 10 digits long");
            }

            // Validate password complexity
            if (!IsValidPassword(emp.Password))
            {
                ModelState.AddModelError("Password", "Password must be 9-20 characters long and include uppercase letters, lowercase letters, numbers, and symbols.");
            }

            // Check if email already exists
            var exist = db.Employee_tbl.FirstOrDefault(x => x.Email == emp.Email);
            var existAdmin = db.Admin_tbl.FirstOrDefault(x => x.Email == emp.Email);
            if (exist != null || existAdmin != null)
            {
                ModelState.AddModelError(string.Empty, "This email already exists.");
            }

            // Check if there are any model errors
            if (!ModelState.IsValid)
            {
                return View(emp); // Return the view with the entered data to correct errors
            }   

            // Hash the password before saving
            var hashedPassword = Hashing.CreateHash(emp.Password);
            emp.Password = hashedPassword;

            // Add employee to database
            db.Employee_tbl.Add(emp);
            db.SaveChanges();

            return RedirectToAction("Employees", "EmployeeAdd");
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidPassword(string password)
        {
            // Password must be between 9 to 20 characters and include uppercase, lowercase, numbers, and symbols
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{9,20}$");
        }

        public ActionResult EmployeeRemove(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var Employee = db.Employee_tbl.Find(id);
            var DeviceAssignment = db.DeviceAssignment_tbl.FirstOrDefault(a => a.AssignedToEmployeeID == id);

            if (DeviceAssignment != null)
            {
                DeviceAssignment.AssignedToEmployeeID = null;
            }




            db.Employee_tbl.Remove(Employee);
            db.SaveChanges();

            return RedirectToAction("Employees", "EmployeeAdd");
        }

        public ActionResult EmployeeEdit(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var employee = db.Employee_tbl.Find(id);
            employee.Password = ""; // Clear password for security reasons
            return View(employee);
        }

        [HttpPost]
        public ActionResult EmployeeEdit(Employee_tbl emp)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            // Find the existing employee record
            var employee = db.Employee_tbl.Find(emp.EmployeeID);

            if (employee.PhoneNumber != null && employee.PhoneNumber.Length != 10)
            {
                ModelState.AddModelError("PhoneNumber", "Phone number must be 10 digits long");
            }


            // Update employee details
            employee.FirstName = emp.FirstName;
            employee.LastName = emp.LastName;
            employee.Department = emp.Department;
            employee.Role = emp.Role;
            employee.PhoneNumber = emp.PhoneNumber;

            // Validate and update password if provided
            if (!string.IsNullOrEmpty(emp.Password))
            {
                if (!IsValidPassword(emp.Password))
                {
                    ModelState.AddModelError("Password", "Password must be 9-20 characters long and include uppercase letters, lowercase letters, numbers, and symbols.");
                }
                else
                {
                    var hashedPassword = Hashing.CreateHash(emp.Password);
                    employee.Password = hashedPassword;
                }
            }

            // Check if there are any model errors
            if (!ModelState.IsValid)
            {
                return View(emp); // Return the view with the entered data to correct errors
            }

            // Save changes to the database
            db.SaveChanges();

            return RedirectToAction("Employees", "EmployeeAdd");
        }

    }
}