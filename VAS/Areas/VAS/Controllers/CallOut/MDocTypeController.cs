using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MDocTypeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDocType(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MDocTypeModel objDocType = new MDocTypeModel();
                retJSON = JsonConvert.SerializeObject(objDocType.GetDocType(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetDocTypeData(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MDocTypeModel objDocType = new MDocTypeModel();
                retJSON = JsonConvert.SerializeObject(objDocType.GetDocTypeData(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // JID_0811
        // Added by Bharat on 24 August 2018 to get Warehouse from Organization Info
        public JsonResult GetWarehouse(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                int AD_Org_ID = Util.GetValueOfInt(fields);
                int warehouse_ID = 0;
                MOrgInfo oInfo = new MOrgInfo(ctx, AD_Org_ID, null);                
                if (oInfo != null)
                {
                    warehouse_ID = Util.GetValueOfInt(oInfo.Get_Value("M_Warehouse_ID"));
                }
                retJSON = JsonConvert.SerializeObject(warehouse_ID);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}