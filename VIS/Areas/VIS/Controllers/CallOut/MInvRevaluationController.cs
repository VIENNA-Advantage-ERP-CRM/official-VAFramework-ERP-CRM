/********************************************************
 * Project Name   : VAdvantage
 * Module Name    : VIS
 * Class Name     : MInvRevaluationController
 * Purpose        : Callout Handling
 * Class Used     : none
 * Chronological  : Development
 * VIS_0045       : 10-March-2023
  ******************************************************/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.DataBase;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    [AjaxAuthorizeAttribute] // redirect to login page if reques`t is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class MInvRevaluationController : Controller
    {
        // GET: VIS/CalloutTable
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get Currency From Accounting Schema
        /// </summary>
        /// <param name="fields">Accounting Schema ID</param>
        /// <returns>Currency ID</returns>
        public JsonResult GetAccountingSchemaCurrency(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvRevaluationModel objInvRevaluationModel = new MInvRevaluationModel();
                retJSON = JsonConvert.SerializeObject(objInvRevaluationModel.GetAccountingSchemaCurrency(ctx, fields));
            }

            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Inventory Revaluation Details
        /// </summary>
        /// <param name="fields">Inventory Revaluation ID</param>
        /// <returns>Dictionary Object of Details</returns>
        public JsonResult GetInvRevaluationDetails(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvRevaluationModel objInvRevaluationModel = new MInvRevaluationModel();
                retJSON = JsonConvert.SerializeObject(objInvRevaluationModel.GetInvRevaluationDetails(ctx, fields));
            }

            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}