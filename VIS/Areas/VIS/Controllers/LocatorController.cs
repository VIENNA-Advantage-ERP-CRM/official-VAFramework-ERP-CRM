using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class LocatorController : Controller
    {
        public ActionResult Index(string windowno, int locatorId)
        {
            ViewBag.WindowNumber = windowno;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                ViewBag.lang = ctx.GetAD_Language();
            }
            return PartialView();
        }

        public JsonResult Save(string warehouseId, string tValue, string tX, string tY, string tZ)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                LocatorModel obj = new LocatorModel();
                var id = obj.LocatorSave(ctx, warehouseId, tValue, tX, tY, tZ);
                return Json(new { locatorId = id }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 09 June 2017
        public JsonResult GetWarehouse(int Warehouse_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            LocatorModel model = new LocatorModel();
            return Json(JsonConvert.SerializeObject(model.GetWarehouse(Warehouse_ID, ct)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 09 June 2017
        public JsonResult GetWarehouseData(int Locator_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            LocatorModel model = new LocatorModel();
            return Json(JsonConvert.SerializeObject(model.GetWarehouseData(Locator_ID, ct)), JsonRequestBehavior.AllowGet);
        }

    }
}
