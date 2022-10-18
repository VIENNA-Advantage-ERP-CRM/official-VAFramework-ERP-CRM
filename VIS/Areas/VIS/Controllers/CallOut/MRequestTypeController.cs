﻿using System;
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
    public class MRequestTypeController: Controller
    {


        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetDefaultR_Status_ID(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MRequestTypeModel rt = new MRequestTypeModel();
                int R_Status_ID = rt.GetDefaultR_Status_ID(ctx,fields);
                retJSON = JsonConvert.SerializeObject(R_Status_ID);
            }        
            return Json(retJSON , JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Resolution text on Resolution change
        /// </summary>
        /// <param name="fields">Resolution ID</param>
        /// <returns>Result</returns>
        public JsonResult GetResolutionText(string fields) //VIS_0336 calling method for fetching the comments of selected resolution in request window.
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MRequestTypeModel rt = new MRequestTypeModel();
                retJSON = JsonConvert.SerializeObject(rt.GetResolutionText(Util.GetValueOfInt(fields)));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }

}
