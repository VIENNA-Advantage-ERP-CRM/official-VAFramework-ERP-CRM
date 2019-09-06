using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MDiscountSchemaController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDiscountType(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MDiscountSchemaModel objDiscountType = new MDiscountSchemaModel();
                retJSON = JsonConvert.SerializeObject(objDiscountType.GetDiscountType(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDiscount(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MDiscountSchemaModel objdiscount = new MDiscountSchemaModel();
                retJSON = JsonConvert.SerializeObject(objdiscount.GetDiscount(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}