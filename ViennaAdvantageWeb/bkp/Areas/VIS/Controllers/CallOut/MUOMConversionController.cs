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
    public class MUOMConversionController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
   
        public JsonResult ConvertProductTo(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MUOMConversionModel objConversionModel = new MUOMConversionModel();
                retJSON = JsonConvert.SerializeObject(objConversionModel.ConvertProductTo(ctx,fields));
            }       
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConvertProductFrom(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MUOMConversionModel objConversionModel = new MUOMConversionModel();
                retJSON = JsonConvert.SerializeObject(objConversionModel.ConvertProductFrom(ctx, fields));
            }         
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}