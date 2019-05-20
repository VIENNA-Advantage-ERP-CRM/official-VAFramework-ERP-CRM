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
        /// <summary>
        /// Create Element Values
        /// </summary>
        /// <param name="m_AD_Org_ID"></param>
        /// <param name="value"></param>
        /// <param name="name"></param>
        /// <param name="expense"></param>
        /// <param name="m_C_Element_ID"></param>
        /// <returns></returns>
        public JsonResult CreateElementValue(int m_AD_Org_ID, String value, String name, bool expense, int m_C_Element_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                VChargeModel obj = new VChargeModel();
                obj.CreateElementValue(ctx, m_AD_Org_ID, value, name, expense, m_C_Element_ID);
                return Json(new { obj.ID, obj.Msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create Charge
        /// </summary>
        /// <param name="m_C_AcctSchema_ID"></param>
        /// <param name="m_C_TaxCategory_ID"></param>
        /// <param name="name"></param>
        /// <param name="C_ElementValue_ID"></param>
        /// <param name="expense"></param>
        /// <returns></returns>
        public JsonResult CreateCharge(int m_C_AcctSchema_ID, int m_C_TaxCategory_ID, String name, int C_ElementValue_ID, bool expense)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                VChargeModel obj = new VChargeModel();
                obj.CreateCharge(ctx, m_C_AcctSchema_ID, m_C_TaxCategory_ID, name, C_ElementValue_ID, expense);
                return Json(new { obj.ID, obj.Msg }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Create Charge by list
        /// </summary>
        /// <param name="AcctSchemaID"></param>
        /// <param name="TaxCategoryID"></param>
        /// <param name="namepara"></param>
        /// <param name="ElementValuID"></param>
        /// <param name="expense"></param>
        /// <returns></returns>
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
        /// get data from c_elementvalue for load grid
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