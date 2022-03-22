using System;
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
    public class MPaymentController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetPayment(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPaymentModel objPayment = new MPaymentModel();
                retJSON = JsonConvert.SerializeObject(objPayment.GetPayment(ctx,fields));
            }           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetPayAmt(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPaymentModel objPayment = new MPaymentModel();
                retJSON = JsonConvert.SerializeObject(objPayment.GetPayAmt(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 17/May/2017
        public JsonResult GetInvoiceData(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPaymentModel objPayment = new MPaymentModel();
                retJSON = JsonConvert.SerializeObject(objPayment.GetInvoiceData(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 17/May/2017
        public JsonResult GetOutStandingAmt(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPaymentModel objPayment = new MPaymentModel();
                retJSON = JsonConvert.SerializeObject(objPayment.GetOutStandingAmt(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 18/May/2017
        public JsonResult GetOrderData(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MPaymentModel objPayment = new MPaymentModel();
                retJSON = JsonConvert.SerializeObject(objPayment.GetOrderData(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Bank Account Currency
        /// </summary>
        /// <param name="fields">Parameters</param>
        /// <returns>Currency</returns>
        public JsonResult GetBankAcctCurrency(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MPaymentModel objCashBookModel = new MPaymentModel();
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetBankAcctCurrency(fields)), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Set override check based on autocheck control
        /// Author:VA230
        /// </summary>
        /// <param name="fields">bank account id,paymentMethodId</param>
        /// <returns>true/false</returns>
        public JsonResult GetAutoCheckControl(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MPaymentModel model = new MPaymentModel();
            bool result = Util.GetValueOfBool(model.GetAutoCheckControl(fields));
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Provisional Invoice data 
        /// </summary>
        /// <param name="fields">C_ProvisionalInvoice_ID</param>
        /// <writer>209</writer>
        /// <returns>Provisionalnvoice Data</returns>
        public JsonResult GetProvisionalInvoiceData(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MPaymentModel objPInvModel = new MPaymentModel();
            return Json(JsonConvert.SerializeObject(objPInvModel.GetProvisionalInvoiceData(Util.GetValueOfInt(fields))), JsonRequestBehavior.AllowGet);
        }
    }
}