using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class PaymentAllocationController : Controller
    {
        //
        // GET: /VIS/PaymentAllocation/
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// to create view allocation against cash journal line
        /// </summary>
        /// <param name="paymentData">Selected payment data</param>
        /// <param name="cashData"> Selected cash line data</param>
        /// <param name="invoiceData">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="isCash"> bool Value </param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="appliedamt"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner</param>
        /// <returns></returns>
        [HttpPost]
        public string SaveCashData(string paymentData, string cashData, string invoiceData, string currency, bool isCash, int _C_BPartner_ID, int _windowNo, string payment, string DateTrx,
            string appliedamt, string discount, string writeOff, string open, string DateAcct , int _CurrencyType_ID , bool isInterBPartner)
        {

            List<Dictionary<string, string>> pData = null;
            List<Dictionary<string, string>> cData = null;
            List<Dictionary<string, string>> iData = null;
            Ctx ct = Session["ctx"] as Ctx;
            string msg = string.Empty;
            DateTime date = Convert.ToDateTime(DateTrx);
            if (paymentData != null)
            {
                pData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(paymentData);
            }
            if (cashData != null)
            {
                cData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(cashData);
            }
            if (paymentData != null)
            {
                iData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(invoiceData);
            }


            PaymentAllocation payments = new PaymentAllocation(ct);
            msg = payments.SaveCashData(pData, cData, iData, currency, isCash, _C_BPartner_ID, _windowNo, payment, date, appliedamt, discount, writeOff, open, Convert.ToDateTime(DateAcct), _CurrencyType_ID, isInterBPartner);

            return msg;
        }

        /// <summary>
        /// to create view allocation against Payment
        /// </summary>
        /// <param name="paymentData">Selected payment data</param>
        /// <param name="cashData"> Selected cash line data</param>
        /// <param name="invoiceData">Selected invoice data</param>
        /// <param name="currency">Currency ID</param>
        /// <param name="isCash"> bool Value </param>
        /// <param name="_C_BPartner_ID"> Business Partner ID </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="payment"> Payment ID </param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="appliedamt"> Applied Amount </param>
        /// <param name="discount">Discount Amount</param>
        /// <param name="writeOff">Writeoff Amount</param>
        /// <param name="open">Open Amount</param>
        /// <param name="DateAcct">Account Date</param>
        /// <param name="_CurrencyType_ID">Currency ConversionType ID</param>
        /// <param name="isInterBPartner">Inter Business Partner</param>
        /// <returns></returns>
        [HttpPost]
        public string SavePaymentData(string paymentData, string cashData, string invoiceData, string currency, bool isCash, int _C_BPartner_ID, int _windowNo, string payment, string DateTrx,
        string appliedamt, string discount, string writeOff, string open, string DateAcct, int _CurrencyType_ID, bool isInterBPartner)
        {
            List<Dictionary<string, string>> pData = null;
            List<Dictionary<string, string>> cData = null;
            List<Dictionary<string, string>> iData = null;
            Ctx ct = Session["ctx"] as Ctx;
            string msg = string.Empty;
            DateTime date = Convert.ToDateTime(DateTrx);
            if (paymentData != null)
            {
                pData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(paymentData);
            }
            if (cashData != null)
            {
                cData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(cashData);
            }
            if (paymentData != null)
            {
                iData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(invoiceData);
            }


            PaymentAllocation payments = new PaymentAllocation(ct);
            msg = payments.SavePaymentData(pData, cData, iData, currency, isCash, _C_BPartner_ID, _windowNo, payment, date, appliedamt, discount, writeOff, open, Convert.ToDateTime(DateAcct), _CurrencyType_ID, isInterBPartner);

            return msg;
        }

        public string CheckPeriodState(string DateTrx)
        {
            Ctx ct = Session["ctx"] as Ctx;
            DateTime date = Convert.ToDateTime(DateTrx);
            PaymentAllocation payments = new PaymentAllocation(ct);
            return payments.CheckPeriodState(date);
        }

        public JsonResult GetPayments(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner,  bool chk, int page, int size)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetPayments(_C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, page, size)), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetCashJounral(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetCashJounral(_C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, page, size)), JsonRequestBehavior.AllowGet);
        }

        //Added new parameters---Neha---
        /// <summary>
        /// To get all the invoices 
        /// </summary>
        /// <param name="_C_Currency_ID">Currency ID</param>
        /// <param name="_C_BPartner_ID"> Business Partner ID</param>
        /// <param name="isInterBPartner">bool Value </param>
        /// <param name="chk">bool Value </param>
        /// <param name="date">Transaction Date</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Total Page Size</param>
        /// <param name="docNo">Document Number</param>
        /// <param name="c_docType_ID">Document Type ID</param>
        /// <param name="fromDate">From Date</param>
        /// <param name="toDate">To Date</param>
        /// <param name="conversionDate">ConversionType Date</param>
        /// <returns></returns>
        public JsonResult GetInvoice(int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, string date, int page, int size, string docNo, int c_docType_ID, DateTime? fromDate, DateTime? toDate, string conversionDate)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);     
            return Json(JsonConvert.SerializeObject(payments.GetInvoice(_C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, date, page, size, docNo, c_docType_ID, fromDate, toDate, conversionDate)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDocType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetDocType()));
        }

        public JsonResult GetCurrencyPrecision(int _C_Currency_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetCurrencyPrecision(_C_Currency_ID)), JsonRequestBehavior.AllowGet);
        }

        ///  <summary>
        /// Get all Organization which are accessable by login user
        /// </summary>        
        /// <returns>AD_Org_ID and Organization Name</returns> //Added by manjot on 27/02/2019 
        public JsonResult GetOrganization()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetOrganization(ct)), JsonRequestBehavior.AllowGet);
        }
    }


}