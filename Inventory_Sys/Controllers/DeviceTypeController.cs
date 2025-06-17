using System.Linq;
using System.Web.Mvc;
using Inventory_Sys.Models;

namespace Inventory_Sys.Controllers
{
    public class DeviceTypeController : BaseController
    {
        private InventorySYS_dbEntities db = new InventorySYS_dbEntities();

        public ActionResult AddDeviceType()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            return View();
        }

        public ActionResult DeviceTypes()
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var devicetypes = from k in db.Device_Type_tbl select k;
            return View(devicetypes.ToList());
    }

        [HttpPost]
        public ActionResult AddDeviceType(Device_Type_tbl deviceType)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            if (ModelState.IsValid)
            {
                db.Device_Type_tbl.Add(deviceType);
                db.SaveChanges();
                return RedirectToAction("AddDevice","Device");
            }

            return View(deviceType);
        }
        public ActionResult DeleteDeviceType(int id)
        {
            if (Session["Role"].ToString() != "Admin" && Session["Role"].ToString() != "SuperAdmin")
            {
                return HttpNotFound();
            }
            var devicetype = db.Device_Type_tbl.Find(id);

            if(devicetype == null)
            {
                return HttpNotFound();
            }

            db.Device_Type_tbl.Remove(devicetype);
            db.SaveChanges();
            return RedirectToAction("DeviceTypes","DeviceType");
        }



    }
}
