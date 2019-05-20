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
    public class MUOMController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetPrecision(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MUOMModel objUOMModel = new MUOMModel();
                retJSON = JsonConvert.SerializeObject(objUOMModel.GetPrecision(ctx,fields));
            }            
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}