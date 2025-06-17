using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory_Sys.Models;
using Rotativa;
using System.Globalization;

namespace Inventory_Sys.Controllers
{
    public class AssignmentController : BaseController
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();

        public ActionResult Assignments()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var assignments = (from assign in db.DeviceAssignment_tbl
                               select new AssignmentViewModel
                               {
                                   AssignmentID = assign.AssignmentID,
                                   DeviceSerialNumber = assign.SerialNumber,
                                   AssignedTo = assign.AssignedToEmployee,
                                   AssignedBy = assign.AssignedByAdmin,
                                   AssignmentDate = assign.AssignmentDate,
                                   ReturnDate = assign.ReturnDate,
                                   Returned = assign.Returned,
                                   Notes = assign.Notes
                               })
                               .ToList();

            return View(assignments);
        }
        public ActionResult AssignmentsPdf()
        {
              if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var assignments = (from assign in db.DeviceAssignment_tbl
                               select new AssignmentViewModel
                               {
                                   AssignmentID = assign.AssignmentID,
                                   DeviceSerialNumber = assign.SerialNumber,
                                   AssignedTo = assign.AssignedToEmployee,
                                   AssignedBy = assign.AssignedByAdmin,
                                   AssignmentDate = assign.AssignmentDate,
                                   ReturnDate = assign.ReturnDate,
                                   Returned = assign.Returned,
                                   Notes = assign.Notes
                               })
                               .ToList();

            return new ViewAsPdf("AssignmentsPdf", assignments)
            {
                FileName = "Assignments.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }

        public ActionResult AddAssign()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            var brands = db.Devices_tbl.Where(d => d.Status == "Available").Select(d => d.Brand).Distinct().ToList();
            ViewBag.Brands = new SelectList(brands);

            var employees = db.Employee_tbl.Select(e => new
            {
                e.EmployeeID,
                FullName = e.FirstName + " " + e.LastName
            }).ToList();
            ViewBag.Employees = new SelectList(employees, "EmployeeId", "FullName");

            return View();
        }

        [HttpPost]
        public ActionResult AddAssign(DeviceAssignment_tbl assign, string Brand, string Model, string SerialNumber)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            // Retrieve AdminID from session
            int adminId = GetAdminIdFromSession();
            if (adminId == 0)
            {
                ModelState.AddModelError("", "Admin not found in session.");
                return View(assign);
            }

            // Find the DeviceID based on selected Brand, Model, and SerialNumber
            var device = db.Devices_tbl.FirstOrDefault(d => d.Brand == Brand && d.Model == Model && d.SerialNumber == SerialNumber && d.Status == "Available");
            if (device == null)
            {
                ModelState.AddModelError("", "Selected device is not available.");
                return View(assign);
            }

            var employee = db.Employee_tbl.FirstOrDefault(e => e.EmployeeID == assign.AssignedToEmployeeID);
            if (employee == null)
            {
                ModelState.AddModelError("", "Selected employee not found.");
                return View(assign);
            }
            var admin = db.Admin_tbl.FirstOrDefault(a => a.AdminID == adminId);
            if (admin == null)
            {
                ModelState.AddModelError("", "Selected employee not found.");
                return View(assign);
            }


            assign.DeviceID = device.DeviceID;
            assign.AssignedByAdminID = adminId;
            assign.SerialNumber = device.SerialNumber;
            assign.AssignedByAdmin = admin.FirstName + " " + admin.LastName;
            assign.AssignmentDate = DateTime.Now;
            assign.AssignedToEmployee = employee.FirstName + " " + employee.LastName;

            if (ModelState.IsValid)
            {
                db.DeviceAssignment_tbl.Add(assign);

                // Update the device status to assigned
                device.Status = "Assigned";
                db.SaveChanges();

                return RedirectToAction("Assignments");
            }

            return View(assign);
        }


        public ActionResult DeleteAssign(int id)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            var assignment = db.DeviceAssignment_tbl.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }

            // Retrieve the associated device
            var device = db.Devices_tbl.Find(assignment.DeviceID);
            if (device != null)
            {
                // Update device status to "Available"
                device.Status = "Available";
            }

            db.DeviceAssignment_tbl.Remove(assignment);
            db.SaveChanges();

            return RedirectToAction("Assignments", "Assignment");
        }


        public ActionResult ConfirmAssign(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var assignment = db.DeviceAssignment_tbl.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            var device = db.Devices_tbl.Find(assignment.DeviceID);
            if (device != null)
            {
                // Update device status to "Available"
                device.Status = "Available";
            }
            PastAssignment_tbl Passign = new PastAssignment_tbl();

            var deviceAssignment = db.DeviceAssignment_tbl.Find(id);
            //AssignedTo
            var AssignedTo = deviceAssignment.AssignedToEmployee;
            //AssignedBy
            var AssignedBy = deviceAssignment.AssignedByAdmin;
            //AssignmentDate
            var AssignmentDate = deviceAssignment.AssignmentDate;
            //Notes
            var Notes = deviceAssignment.Notes;
            //SerialNumber
            var SerialNumber = deviceAssignment.SerialNumber;

            Passign.SerialNumber = SerialNumber;
            Passign.AssignedTo = AssignedTo;
            Passign.AssignedBy = AssignedBy;
            Passign.AssignmentDate = AssignmentDate;
            Passign.ReturnDate = DateTime.Now;
            Passign.Notes = Notes;
            Passign.Returned = "Yes";

            db.PastAssignment_tbl.Add(Passign);
            db.DeviceAssignment_tbl.Remove(deviceAssignment);
            db.SaveChanges();

            return RedirectToAction("Assignments", "Assignment");
        }

        public ActionResult PastAssignments()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            var Passignments = from k in db.PastAssignment_tbl select k;
            return View(Passignments.ToList());
        }
        public ActionResult RemovePastAssignment(int id)
        {
            if (Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var PastAssignments = db.PastAssignment_tbl.Find(id);
            db.PastAssignment_tbl.Remove(PastAssignments);
            db.SaveChanges();

            return RedirectToAction("Assignments","Assignment");
        }
        public ActionResult PastAssignmentsPdf()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var pastAssignments = db.PastAssignment_tbl.ToList();
            return new ViewAsPdf("PastAssignmentsPdf", pastAssignments)
            {
                FileName = "PastAssignments.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }

        // Helper method to get AdminID from session
        private int GetAdminIdFromSession()
        {
            var adminFirstName = Session["username"] as string;
            var adminLastName = Session["Surname"] as string;

            if (adminFirstName == null || adminLastName == null)
                return 0;

            var admin = db.Admin_tbl.FirstOrDefault(a => a.FirstName == adminFirstName && a.LastName == adminLastName);
            return admin != null ? admin.AdminID : 0;
        }

        public JsonResult GetModels(string brand)
        {
            var models = db.Devices_tbl.Where(d => d.Brand == brand && d.Status == "Available").Select(d => d.Model).Distinct().ToList();
            return Json(models, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSerialNumbers(string model)
        {
            var serialNumbers = db.Devices_tbl.Where(d => d.Model == model && d.Status == "Available").Select(d => d.SerialNumber).ToList();
            return Json(serialNumbers, JsonRequestBehavior.AllowGet);
        }
    }
}
