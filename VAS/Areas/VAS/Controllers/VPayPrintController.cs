using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VISModel.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class VPayPrintController : Controller
    {
        //
        // GET: /VIS/VPayPrint/
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDetail(bool showPrintedPayment, int VAB_PaymentSelect_ID, bool isFirstTime)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.GetDetail(ctx, showPrintedPayment, VAB_PaymentSelect_ID, isFirstTime)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult LoadPaymentRuleInfo(string paymentMethod_ID, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.LoadPaymentRuleInfo(ctx,paymentMethod_ID,VAB_PaymentOption_ID,m_VAB_Bank_Acct_ID,PaymentRule)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult Cmd_Print(string paymentMethod_ID, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule,string checkNo)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.Cmd_Print(ctx, VAB_PaymentOption_ID, m_VAB_Bank_Acct_ID, paymentMethod_ID, Util.GetValueOfInt(checkNo))), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult ContinueCheckPrint(string[] data)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string paymentMethod_ID = Util.GetValueOfString(data[0]);
            int VAB_PaymentOption_ID = Util.GetValueOfInt(data[1]);
            int m_VAB_Bank_Acct_ID = Util.GetValueOfInt(data[2]);
            string PaymentRule = Util.GetValueOfString(data[3]);
            string checkNo = Util.GetValueOfString(data[4]);
            List<int> check_ID = JsonConvert.DeserializeObject<List<int>>(data[5]);
            VAdvantage.Model.MVABPaymentBatch m_batch = new VAdvantage.Model.MVABPaymentBatch(ctx, Util.GetValueOfInt(data[6]),null);
            VAdvantage.Model.MVABPaymentOptionCheck[] m_checks = null;
            List<VAdvantage.Model.MVABPaymentOptionCheck> list = new List<VAdvantage.Model.MVABPaymentOptionCheck>();
            for (int i = 0; i <check_ID.Count; i++)
            {
                list.Add(new VAdvantage.Model.MVABPaymentOptionCheck(ctx, check_ID[i], null));
            }
            m_checks = list.ToArray();
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.ContinueCheckPrint(ctx, VAB_PaymentOption_ID, m_VAB_Bank_Acct_ID, paymentMethod_ID, checkNo, check_ID, m_checks, m_batch)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult VPayPrintRemittance(string[] data)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            data[0] = "[" + data[0] + "]";
            List<int> payment_ID = JsonConvert.DeserializeObject<List<int>>(data[0]);          

            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.VPayPrintRemittance(ctx, payment_ID)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Cmd_Export(string paymentMethod_ID, int VAB_PaymentOption_ID, int m_VAB_Bank_Acct_ID, string PaymentRule, string checkNo)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.Cmd_Export(ctx, VAB_PaymentOption_ID, m_VAB_Bank_Acct_ID, paymentMethod_ID, Util.GetValueOfInt(checkNo))), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult VPayPrintSuccess(string[] data)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            List<int> check_ID = JsonConvert.DeserializeObject<List<int>>(data[0]);
            VAdvantage.Model.MVABPaymentBatch m_batch = new VAdvantage.Model.MVABPaymentBatch(ctx, Util.GetValueOfInt(data[1]), null);
            VAdvantage.Model.MVABPaymentOptionCheck[] m_checks = null;
            List<VAdvantage.Model.MVABPaymentOptionCheck> list = new List<VAdvantage.Model.MVABPaymentOptionCheck>();
            for (int i = 0; i < check_ID.Count; i++)
            {
                list.Add(new VAdvantage.Model.MVABPaymentOptionCheck(ctx, check_ID[i], null));
            }
            m_checks = list.ToArray();
            VPayPrintModel objVPaySelect = new VPayPrintModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.VPayPrintSuccess(ctx,m_checks, m_batch)), JsonRequestBehavior.AllowGet);
        }
	}
}