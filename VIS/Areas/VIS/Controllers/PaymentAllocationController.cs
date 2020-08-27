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

        /// <summary>
        /// To check state of period weather it is Open or Close
        /// </summary>
        /// <param name="DateTrx">Transaction Date </param>
        /// <returns>Return Empty if period is OPEN else it will return ErrorMsg</returns>
        public string CheckPeriodState(string DateTrx)
        {
            Ctx ct = Session["ctx"] as Ctx;
            DateTime date = Convert.ToDateTime(DateTrx);
            PaymentAllocation payments = new PaymentAllocation(ct);
            return payments.CheckPeriodState(date);
        }

        /// <summary>
        /// To get all the unallocated payments
        /// </summary>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="isInterBPartner">Inter-Business Partner</param>
        /// <param name="chk">For MultiCurrency Check</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <returns>No of unallocated payments</returns>
        public JsonResult GetPayments(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size,int c_docType_ID,string docBaseType, DateTime? fromDate, DateTime? toDate,string srchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetPayments(AD_Org_ID, _C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, page, size, c_docType_ID, docBaseType, fromDate, toDate, srchText)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// To get all the unallocated Cash Lines
        /// </summary>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="isInterBPartner">Inter-Business Partner</param>
        /// <param name="chk">For MultiCurrency Check</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <returns>No of unallocated Cash Lines</returns>
        public JsonResult GetCashJounral(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, int page, int size, DateTime? fromDate, DateTime? toDate,string paymentType_ID,string srchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetCashJounral(AD_Org_ID, _C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, page, size, fromDate, toDate, paymentType_ID, srchText)), JsonRequestBehavior.AllowGet);
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
        public JsonResult GetInvoice(int AD_Org_ID, int _C_Currency_ID, int _C_BPartner_ID, bool isInterBPartner, bool chk, string date, int page, int size, string docNo, int c_docType_ID,string docBaseType, DateTime? fromDate, DateTime? toDate, string conversionDate,string srchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetInvoice(AD_Org_ID,_C_Currency_ID, _C_BPartner_ID, isInterBPartner, chk, date, page, size, docNo, c_docType_ID, docBaseType, fromDate, toDate, conversionDate, srchText)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to get DataTypes
        /// </summary>
        /// <returns>List of DocBase Types</returns>
        /// payment grid
        public JsonResult GetpayDocbaseType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetpayDocbaseType()));
        }

        /// <summary>
        /// to get DataTypes
        /// </summary>
        /// <returns>List of DocBase Types</returns>
        public JsonResult GetDocbaseType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetDocbaseType()));
        }

        /// <summary>
        /// to get DataTypes
        /// </summary>
        /// <returns>List of Data Types</returns>
        public JsonResult GetDocType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetDocType()));
        }

        /// <summary>
        /// to get DataTypes
        /// </summary>
        /// <returns>List of Data Types</returns>
        public JsonResult GetpayDocType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetpayDocType()));
        }

        public JsonResult GetPaymentType()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetPaymentType()));
        }

        /// <summary>
        /// TO get currency precision from currency window
        /// </summary>
        /// <param name="_C_Currency_ID">Currency</param>
        /// <returns>precision of currency</returns>
        public JsonResult GetCurrencyPrecision(int _C_Currency_ID)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetCurrencyPrecision(_C_Currency_ID)), JsonRequestBehavior.AllowGet);
        }

        ///  <summary>
        /// Get all Organization which are accessable by login user
        /// </summary>        
        /// <returns>AD_Org_ID and Organization Name</returns> //Added by koteswar on 10/07/2020 
        public JsonResult GetOrg()
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetOrg(ct)), JsonRequestBehavior.AllowGet);
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

        /// <summary>
        /// To get all the unallocated GL Lines
        /// <param name="_C_Currency_ID">Currency</param>
        /// <param name="_C_BPartner_ID">Business Partner</param>
        /// <param name="page">Page Number</param>
        /// <param name="size">Page Size</param>
        /// <returns>No of unallocated GL Lines</returns>
        public JsonResult GetGLData(int AD_Org_ID,int _C_Currency_ID, int _C_BPartner_ID, int page, int size, DateTime? fromDate, DateTime? toDate,string srchText)
        {
            Ctx ct = Session["ctx"] as Ctx;
            PaymentAllocation payments = new PaymentAllocation(ct);
            return Json(JsonConvert.SerializeObject(payments.GetGLData(AD_Org_ID,_C_Currency_ID, _C_BPartner_ID, page, size, fromDate, toDate, srchText)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// to create view allocation against GL journal line
        /// </summary>
        /// <param name="paymentData">Selected payment data</param>
        /// <param name="invoiceData">Selected invoice data</param>
        /// <param name="cashData"> Selected cash line data</param>
        /// <param name="glData"> Selected gl line data</param>
        /// <param name="DateTrx"> Transaction Date </param>
        /// <param name="_windowNo"> Window Number</param>
        /// <param name="C_Currency_ID">Currency</param>
        /// <param name="C_BPartner_ID"> Business Partner</param>
        /// <param name="AD_Org_ID">Org ID</param>
        /// <param name="C_CurrencyType_ID">Currency ConversionType ID</param>
        /// <returns>Will Return Msg Either Allocation is Saved or Not Saved</returns>
        [HttpPost]
        public string saveGLJData(string paymentData, string invoiceData, string cashData, string glData, string DateTrx, string _windowNo, int C_Currency_ID, int C_BPartner_ID, string AD_Org_ID, int C_CurrencyType_ID, string DateAcct, string applied, string discount, string open, string payment, string writeOff)
        {
            List<Dictionary<string, string>> pData = null;
            List<Dictionary<string, string>> cData = null;
            List<Dictionary<string, string>> gData = null;
            List<Dictionary<string, string>> iData = null;
            Ctx ct = Session["ctx"] as Ctx;
            string msg = string.Empty;
            DateTime date = Convert.ToDateTime(DateTrx);
            DateTime dateAcct = Convert.ToDateTime(DateAcct);
            if (paymentData != null)
            {
                pData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(paymentData);
            }
            if (cashData != null)
            {
                cData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(cashData);
            }
            if (glData != null)
            {
                gData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(glData);
            }
            if (invoiceData != null)
            {

                iData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(invoiceData);
            }

            PaymentAllocation payments = new PaymentAllocation(ct);
            msg = payments.SaveGLData(pData, iData, cData, gData, date, Util.GetValueOfInt(_windowNo), Util.GetValueOfInt(C_Currency_ID), Util.GetValueOfInt(C_BPartner_ID), Util.GetValueOfInt(AD_Org_ID), Util.GetValueOfInt(C_CurrencyType_ID), dateAcct,applied, discount, open, payment, writeOff);
            return msg;
        }

    }


}