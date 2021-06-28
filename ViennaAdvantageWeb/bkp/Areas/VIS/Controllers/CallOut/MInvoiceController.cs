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
    public class MInvoiceController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetInvoice(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel objInvoice = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(objInvoice.GetInvoice(ctx,fields));
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTax(string fields)
        {

            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel obj = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(obj.GetTaxId(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Changes done by mohit to remove client side queries- 12 May 2017
        public JsonResult GetInvPaySchedDetail(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel obj = new MInvoiceModel();                
                retJSON = JsonConvert.SerializeObject(obj.GetInvPaySchedDetail(fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
            
        }

        public JsonResult GetInvoiceDetails(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel obj = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(obj.GetInvoiceDetails(fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 24 May 2017
        public JsonResult GetInvoiceAmount(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel obj = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(obj.GetInvoiceAmount(fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Get Price of Product
        /// </summary>
        /// <param name="fields">List of Parameters</param>
        /// <returns>Data in JSON format</returns>
        public JsonResult GetPrices(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel objInvoice = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(objInvoice.GetPrices(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting the percision values
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns>get Percision value</returns>
        public JsonResult GetPercision(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInvoiceModel objInvoice = new MInvoiceModel();
                retJSON = JsonConvert.SerializeObject(objInvoice.GetPrecision(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }   
}