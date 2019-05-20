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
    public class MCashBookController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        //Get CashBook Detail
        public JsonResult GetCashBook(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MCashBookModel objCashBookModel = new MCashBookModel();
                retJSON = JsonConvert.SerializeObject(objCashBookModel.GetCashBook(ctx,fields));
            }
          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        //Get Cash Journal Detail Added by Bharat 16/12/2016
        public JsonResult GetCashJournal(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MCashBookModel objCashJournalModel = new MCashBookModel();
                retJSON = JsonConvert.SerializeObject(objCashJournalModel.GetCashJournal(ctx, fields));
            }

            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Mohit to remove client side queries 10/05/2017
        //Get Cashbook Beginning Balance
        public JsonResult GetBegiBalance(string fields)
        {
            decimal BeginBal = 0;
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            BeginBal = objCashBookModel.GetBeginBalance(ctx, Util.GetValueOfInt( fields));
            return Json(JsonConvert.SerializeObject(BeginBal), JsonRequestBehavior.AllowGet);
        }

        //Get tax rate from tax
        public JsonResult GetTaxRate(string fields)
        {
            decimal taxRate = 0;
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            taxRate = objCashBookModel.GetTaxRate(ctx, Util.GetValueOfInt(fields));
            return Json(JsonConvert.SerializeObject(taxRate), JsonRequestBehavior.AllowGet);
        }

        //Get Converted Amount
        public JsonResult GetConvertedAmt(string fields)
        {
         
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();            
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetConvertedAmt(ctx, fields)), JsonRequestBehavior.AllowGet);
        }

        // Get Invoice Payshedule Amount
        public JsonResult GetPaySheduleAmt(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetPaySheduleAmount(fields)), JsonRequestBehavior.AllowGet);
        }

        // Get Invoice PaySchedule Data
        public JsonResult GetPaySheduleData(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetPaySheduleData(fields)), JsonRequestBehavior.AllowGet);
        }

        // Get Due Amount From InvoiceSchedule
        public JsonResult GetInvSchedDueAmt(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetInvSchedDueAmount(fields)), JsonRequestBehavior.AllowGet);
        }

        // Get Bank Account Currency
        public JsonResult GetBankAcctCurr(string fields)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            MCashBookModel objCashBookModel = new MCashBookModel();
            return Json(JsonConvert.SerializeObject(objCashBookModel.GetBankAcctCurrency(fields)), JsonRequestBehavior.AllowGet);
        }

        // 
    }
}