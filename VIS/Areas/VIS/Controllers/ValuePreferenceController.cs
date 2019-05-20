using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Classes;

namespace VIS.Controllers
{
    public class ValuePreferenceController : Controller
    {
        //
        // GET: /VIS/ValuePreference/
        public ActionResult Index(string windowno)
        {
            ViewBag.WindowNumber = windowno;
            return PartialView();
        }

        /// <summary>
        /// delete prefrence
        /// </summary>
        /// <param name="preferenceId"></param>
        /// <returns></returns>
        public JsonResult Delete(string preferenceId)
        {
            var returnValue = false;

            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as VAdvantage.Utility.Ctx; 
                ValuePreferenceModel obj = new ValuePreferenceModel();
                returnValue = obj.DeletePrefrence(ctx, preferenceId);
            }
            return Json(new { result = returnValue }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save(string preferenceId, string clientId, string orgId, string chkWindow, string AD_Window_ID, string chkUser, string attribute, string userId, string value)
        {
            var returnValue = false;

            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as VAdvantage.Utility.Ctx;
                ValuePreferenceModel obj = new ValuePreferenceModel();
                returnValue = obj.SavePrefrence(ctx, preferenceId, clientId, orgId, chkWindow, AD_Window_ID, chkUser, attribute, userId, value);
            }

            return Json(new { result = returnValue }, JsonRequestBehavior.AllowGet);
        }

    }
}