using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Inventory_Sys.Models;

namespace Inventory_Sys.Controllers
{
    public class RequestController : BaseController
    {
        InventorySYS_dbEntities db = new InventorySYS_dbEntities();
        public ActionResult Index()
        {
            return View();
        }
    }
}