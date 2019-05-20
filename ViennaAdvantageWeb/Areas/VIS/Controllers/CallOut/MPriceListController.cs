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
    public class MPriceListController:Controller
    {


        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetPriceList(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPriceListModel objPriceList = new MPriceListModel();
                retJSON = JsonConvert.SerializeObject(objPriceList.GetPriceList(ctx,fields));
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}