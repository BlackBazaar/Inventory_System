using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory_Sys.Models;
using FrontCode.Libraries;
using System.Globalization;

namespace Inventory_Sys.Controllers
{
    public class DeviceController : BaseController
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();

        public ActionResult Devices()
        {
            var devices = from k in db.Devices_tbl select k;
            return View(devices.ToList());
        }

        public ActionResult AddDevice()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            ViewBag.DeviceTypes = new SelectList(db.Device_Type_tbl, "DeviceTypeID", "TypeName");
            return View();
        }

        [HttpPost]
        public ActionResult AddDevice(Devices_tbl device)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                device.PurchaseDate = DateTime.ParseExact(Request.Form["PurchaseDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                device.LastMaintenanceDate = DateTime.ParseExact(Request.Form["LastMaintenanceDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                db.Devices_tbl.Add(device);
                db.SaveChanges();

                return RedirectToAction("Devices");
            }

            ViewBag.DeviceTypes = new SelectList(db.Device_Type_tbl, "DeviceTypeID", "TypeName");
            return View(device);
        }

        public ActionResult DeviceRemove(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var Device = db.Devices_tbl.Find(id);
            db.Devices_tbl.Remove(Device);
            db.SaveChanges();

            return RedirectToAction("Devices", "Device");
        }

        public ActionResult DeviceEdit(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var device = db.Devices_tbl.Find(id);
            if (device == null)
            {
                return HttpNotFound();
            }
            ViewBag.DeviceTypes = new SelectList(db.Device_Type_tbl, "DeviceTypeID", "TypeName", device.DeviceTypeID);
            ViewBag.StatusList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Available", Value = "Available" },
                new SelectListItem { Text = "Assigned", Value = "Assigned" }
            }, "Value", "Text", device.Status);
            ViewBag.RetiredList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "No", Value = "False" },
                new SelectListItem { Text = "Yes", Value = "True" }
            }, "Value", "Text", device.Retired);
            return View(device);
        }

        [HttpPost]
        public ActionResult DeviceEdit(Devices_tbl device)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                device.PurchaseDate = DateTime.ParseExact(Request.Form["PurchaseDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                device.LastMaintenanceDate = DateTime.ParseExact(Request.Form["LastMaintenanceDate"], "yyyy-MM-dd", CultureInfo.InvariantCulture);

                db.Entry(device).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Devices");
            }

            ViewBag.DeviceTypes = new SelectList(db.Device_Type_tbl, "DeviceTypeID", "TypeName", device.DeviceTypeID);
            ViewBag.StatusList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Available", Value = "Available" },
                new SelectListItem { Text = "Not Available", Value = "Not Available" }
            }, "Value", "Text", device.Status);
            ViewBag.RetiredList = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "No", Value = "False" },
                new SelectListItem { Text = "Yes", Value = "True" }
            }, "Value", "Text", device.Retired);

            return View(device);
        }

        public ActionResult DevicesPdf()
        {
            var devices = db.Devices_tbl.ToList();
            return new ViewAsPdf("DevicesPdf", devices)
            {
                FileName = "Devices.pdf",
                PageSize = Rotativa.Options.Size.A4,
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }
    }
}
