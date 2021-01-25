using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class VChargeController : Controller
    {
        //
        // GET: /VIS/VCharge/
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CreateElementValue(int m_VAF_Org_ID, String value, String name, bool expense, int m_VAB_Element_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                VChargeModel obj = new VChargeModel();
                obj.CreateElementValue(ctx, m_VAF_Org_ID, value, name, expense, m_VAB_Element_ID);
                return Json(new { obj.ID, obj.Msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateCharge(int m_VAB_AccountBook_ID, int m_C_TaxCategory_ID, String name, int VAB_Acct_Element_ID, bool expense)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                VChargeModel obj = new VChargeModel();
                obj.CreateCharge(ctx, m_VAB_AccountBook_ID, m_C_TaxCategory_ID, name, VAB_Acct_Element_ID, expense);
                return Json(new { obj.ID, obj.Msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateChargeByList(int AcctSchemaID, int TaxCategoryID, List<string> namepara, List<string> ElementValuID, List<bool> expense)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                VChargeModel obj = new VChargeModel();
                obj.CreateChargeByList(ctx, AcctSchemaID, TaxCategoryID, namepara, ElementValuID, expense);
                return Json(new { obj.listCreatedP, obj.listRejectedP }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// get data from VAB_Acct_Element for load grid
        /// </summary>
        /// <param name="mcAcctSchemaID"></param>
        /// <param name="mADClientId"></param>
        /// <returns></returns>
        public JsonResult VChargeLodGrideData(int mcAcctSchemaID, int mADClientId)
        {
            var ctx = Session["ctx"] as Ctx;
            VChargeModel obj = new VChargeModel();
            var value = obj.VChargeLodGrideData(ctx, mcAcctSchemaID,  mADClientId);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

    }
}