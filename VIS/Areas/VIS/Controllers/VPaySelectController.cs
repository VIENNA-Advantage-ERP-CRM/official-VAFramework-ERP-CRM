/********************************************************
 * Module Name    : VIS
 * Purpose        : Controller class for payment Selection(Manual) Form
 * Class Used     : 
 * Chronological Development
 * Sarbjit Kaur     14 May 2015
 ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class VPaySelectController : Controller
    {
        //
        // GET: /VIS/VPaySelect/
        public ActionResult Index()
        {
            return View();
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetDetail()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPaySelectModel objVPaySelect = new VPaySelectModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.GetDetail(ctx)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetPaymentMethod(int C_BankAccount_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VPaySelectModel objVPaySelect = new VPaySelectModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.GetPaymentMethod(ctx,C_BankAccount_ID)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetGridRecords(string[] param)
        {
            int bankAccountID = Util.GetValueOfInt(param[0]);
            int currencyID = Util.GetValueOfInt(param[1]);
            int BPartnerID=Util.GetValueOfInt(param[2]);
            DateTime? paymentDate= Util.GetValueOfDateTime(param[3]);
            string paymentMethod= Util.GetValueOfString(param[4]);
            string paymentAmount= Util.GetValueOfString(param[5]);
            bool onlyDueInvoice= Util.GetValueOfBool(param[6]);
            //bool onlyDueInvoice = false;
            Ctx ctx = Session["ctx"] as Ctx;
            VPaySelectModel objVPaySelect = new VPaySelectModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.GetGridData(ctx, bankAccountID, BPartnerID, paymentDate, paymentMethod, onlyDueInvoice, currencyID)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GeneratePaymentSelection(string[] param)
        {
            int bankAccountID = Util.GetValueOfInt(param[0]);
            int currencyID = Util.GetValueOfInt(param[1]);
            int BPartnerID = Util.GetValueOfInt(param[2]);
            DateTime? paymentDate = Util.GetValueOfDateTime(param[3]);
            string paymentMethod = Util.GetValueOfString(param[4]);
            Decimal? paymentAmount = Util.GetValueOfDecimal(param[5]);
            bool onlyDueInvoice= Util.GetValueOfBool(param[6]);
            // bool onlyDueInvoice = false;
            List<GridRecords> lstSelectedRecords= JsonConvert.DeserializeObject<List<GridRecords>>(param[7]);
            Ctx ctx = Session["ctx"] as Ctx;
            VPaySelectModel objVPaySelect = new VPaySelectModel();
            return Json(JsonConvert.SerializeObject(objVPaySelect.GeneratePaySelect(ctx,lstSelectedRecords,paymentAmount,paymentMethod,bankAccountID,paymentDate)), JsonRequestBehavior.AllowGet);
        }
	}
}