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
    public class MBPartnerController : Controller
    {
        //
        // GET: /VIS/BPartner/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetBPartner(string fields)
        {            
            string retJSON = "";      
            VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPartner(ctx,fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 10/May/2017
        public JsonResult GetBPGroup(string fields)
        {
            string retJSON = "";
            VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPGroup(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 11/May/2017
        public JsonResult GetBPData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetBPartnerData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPartnerData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 12/May/2017
        public JsonResult GetLocationData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetLocationData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 13/May/2017
        public JsonResult GetBPartnerOrderData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPartnerOrderData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 13/May/2017
        public JsonResult GetBPartnerBillData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPartnerBillData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

            // Added by Bharat on 16/May/2017
        public JsonResult GetBPDocTypeData(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPDocTypeData(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        //Added by Manjot on 11/june/2018
        //VA009_PO_PaymentMethod_ID added new column for enhancement.. Google Sheet ID-- SI_0036
        public JsonResult GetBPDetails(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPDetails(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Data from BPartner
        /// </summary>
        /// <param name="fields">Parameters</param>
        /// <returns>List of BPData in JSON format</returns>
        public JsonResult GetBPDataForProvisionalInvoice(string fields)
        {
            string retJSON = "";
            Ctx ctx = Session["ctx"] as Ctx;
            MBPartnerModel objBPModel = new MBPartnerModel();
            retJSON = JsonConvert.SerializeObject(objBPModel.GetBPDataForProvisionalInvoice(ctx, fields));
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}