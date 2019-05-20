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
    public class MCurrencyController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetCurrency(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MCurrencyModel objCurrency = new MCurrencyModel();
                retJSON = JsonConvert.SerializeObject(objCurrency.GetCurrency(ctx,fields));
            }            
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}