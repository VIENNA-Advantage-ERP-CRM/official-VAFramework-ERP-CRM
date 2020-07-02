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
    public class MOrderController : Controller
    {
        //
        // GET: /VIS/MOrder/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrder(string fields)
        {

            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetOrder(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added By Amit
        public JsonResult GetM_PriceList(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetM_PriceList(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetC_Currency_ID(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetC_Currency_ID(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        //End

        // Added by Bharat on 16 May 2017
        public JsonResult GetPaymentMethod(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetPaymentMethod(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 30 Jan 2018 to Get Inco Term
        public JsonResult GetIncoTerm(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetIncoTerm(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // when doc type = Warehouse Order / Credit Order / POS Order / Prepay order --- and payment term is advance -- then system return false
        public JsonResult checkAdvancePaymentTerm(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.checkAdvancePaymentTerm(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Neha Thakur on 20 June 2018 to copy Blnaket PO ,Blanket So header as per selected Blanket Order Reference
        public JsonResult GetOrderHeader(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetOrderHeader(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting the percision values
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns>get Percision value</returns>
        public JsonResult GetPrecision(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MOrderModel objOrder = new MOrderModel();
                retJSON = JsonConvert.SerializeObject(objOrder.GetPrecision(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}