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
    public class MAcctSchemaController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //Get AcctSchema Detail
        public JsonResult GetAcctSchema(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;               
                MAcctSchemaModel objAcctSchema = new MAcctSchemaModel();
                retJSON = JsonConvert.SerializeObject(objAcctSchema.GetAcctSchema(ctx, fields));
            }            
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }


    }
}