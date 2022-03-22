using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class AcctViewerDataController : Controller
    {
        /// <summary>
        /// Get Accouting schema
        /// </summary>
        /// <param name="ad_client_id"></param>
        /// <param name="ad_org_id"></param>
        /// <returns></returns>
        public JsonResult GetClientAcctSchema(int ad_client_id, int ad_org_id)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData(); 
            var value = obj.GetClientAcctSchema(ctx, ad_client_id, ad_org_id);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Table Data
        /// </summary>
        /// <returns></returns>
        public JsonResult AcctViewerGetTabelData()
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerGetTabelData(ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Organizations
        /// </summary>
        /// <param name="client_id"></param>
        /// <returns></returns>
        public JsonResult AcctViewerGetOrgData(int client_id)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerGetOrgData(ctx, client_id);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Posting Type
        /// </summary>
        /// <param name="reference_id"></param>
        /// <returns></returns>
        public JsonResult AcctViewerGetPostingType(int reference_id)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerGetPostingType(ctx, reference_id);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Accouting Schema Elements
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public JsonResult AcctViewerGetAcctSchElements(int keys)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerGetAcctSchElements(ctx, keys);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get data according for Selected Document type and appended the Info window details also
        /// </summary>
        /// <param name="lookupDirEmbeded"></param>
        /// <param name="tName"></param>
        /// <param name="wheres"></param>
        /// <param name="selectSQLs"></param>
        /// <returns></returns>
        public JsonResult AcctViewerGetButtonText(string lookupDirEmbeded, string tName, string wheres, string selectSQLs)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerGetButtonText(ctx, lookupDirEmbeded, tName, wheres, selectSQLs);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create Repost
        /// </summary>
        /// <param name="dataRecID"></param>
        /// <returns></returns>
        public JsonResult AcctViewerRePost(int dataRecID)
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.AcctViewerRePost(ctx, dataRecID);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to check if organization unit window is available or not.
        /// </summary>
        /// <returns></returns>
        public JsonResult HasOrganizationUnit()
        {
            var ctx = Session["ctx"] as Ctx;
            AcctViewerData obj = new AcctViewerData();
            var value = obj.HasOrganizationUnit(ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

    }
}