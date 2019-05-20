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
    }
}