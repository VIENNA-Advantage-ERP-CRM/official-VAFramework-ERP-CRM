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
    public class MVABChargeController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //Get Charge AMount
        public JsonResult GetCharge(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVABChargeModel objChargeModel = new MVABChargeModel();
                retJSON = JsonConvert.SerializeObject(objChargeModel.GetCharge(ctx,fields));
            }
           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
     
    }
}