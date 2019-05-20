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
    public class MChargeController:Controller
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
                MChargeModel objChargeModel = new MChargeModel();
                retJSON = JsonConvert.SerializeObject(objChargeModel.GetCharge(ctx,fields));
            }
           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
     
    }
}