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
    public class MProductCategoryController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProductCategory(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MProductCategoryModel objProductCategory = new MProductCategoryModel();
                retJSON = JsonConvert.SerializeObject(objProductCategory.GetProductCategory(ctx,fields));
            }         
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}