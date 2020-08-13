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
    public class MInvoiceBatchLineController:Controller
    {

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetInvoiceBatchLine(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceBatchLineModel objInvoiceBatchLine = new MInvoiceBatchLineModel();
                retJSON = JsonConvert.SerializeObject(objInvoiceBatchLine.GetInvoiceBatchLine(ctx,fields));
            }          
            //return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
            return Json(retJSON , JsonRequestBehavior.AllowGet);
        }
    }
}