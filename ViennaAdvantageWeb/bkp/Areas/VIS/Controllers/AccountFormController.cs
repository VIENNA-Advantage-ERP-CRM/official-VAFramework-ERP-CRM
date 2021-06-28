using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class AccountFormController : Controller
    {
        //
        // GET: /VIS/AccountForm/
        public ActionResult Index(string windowno)
        {
            ViewBag.WindowNumber = windowno;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                ViewBag.lang = ctx.GetAD_Language();
            }

            return PartialView();
        }

        /// <summary>
        /// load controls in the form
        /// </summary>
        /// <param name="pref"></param>
        /// <returns></returns>
        public JsonResult LoadControls(int windowNo, int C_AcctSchema_ID)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                AccountFormModel obj = new AccountFormModel();
                var value = obj.AccountSchemaLoad(ctx, windowNo, C_AcctSchema_ID);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Account Schema Records
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="C_AcctSchema_ID"></param>
        /// <returns></returns>
        public JsonResult Save(string AD_Client_ID, string AD_Org_ID, string C_AcctSchema_ID, string AD_Account_ID, string C_SubAcct_ID, string M_Product_ID,
            string C_BPartner_ID, string AD_OrgTrx_ID, string C_LocFrom_ID, string C_LocTo_ID, string C_SRegion_ID, string C_Project_ID, string C_Campaign_ID,
            string C_Activity_ID, string User1_ID, string User2_ID, string UserElement1_ID, string UserElement2_ID, string UserElement3_ID, string UserElement4_ID,
            string UserElement5_ID, string UserElement6_ID, string UserElement7_ID, string UserElement8_ID, string UserElement9_ID, string Alias)
        {
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;               
                AccountFormModel obj = new AccountFormModel();                
                var value = obj.SaveAccount(ctx, Convert.ToInt32(AD_Client_ID), Convert.ToInt32(AD_Org_ID), Convert.ToInt32(C_AcctSchema_ID),
                    Convert.ToInt32(AD_Account_ID), Convert.ToInt32(C_SubAcct_ID), Convert.ToInt32(M_Product_ID), Convert.ToInt32(C_BPartner_ID), Convert.ToInt32(AD_OrgTrx_ID), 
                    Convert.ToInt32(C_LocFrom_ID), Convert.ToInt32(C_LocTo_ID), Convert.ToInt32(C_SRegion_ID), Convert.ToInt32(C_Project_ID), Convert.ToInt32(C_Campaign_ID),
                    Convert.ToInt32(C_Activity_ID), Convert.ToInt32(User1_ID), Convert.ToInt32(User2_ID), Convert.ToInt32(UserElement1_ID), Convert.ToInt32(UserElement2_ID),
                    Convert.ToInt32(UserElement3_ID), Convert.ToInt32(UserElement4_ID), Convert.ToInt32(UserElement5_ID), Convert.ToInt32(UserElement6_ID), Convert.ToInt32(UserElement7_ID),
                    Convert.ToInt32(UserElement8_ID), Convert.ToInt32(UserElement9_ID), Alias);                
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }
    }
}