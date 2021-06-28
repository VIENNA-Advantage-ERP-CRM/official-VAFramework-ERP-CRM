using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;
using VIS.Classes;

namespace VIS.Controllers
{
    public class PAttributesController : Controller
    {
        //
        // GET: /VIS/PAttributes/
        public ActionResult Index()
        {
            return PartialView();
        }

        public JsonResult Load(int mAttributeSetInstanceId, int mProductId, bool productWindow, int windowNo, int AD_Column_ID, int window_ID, bool IsSOTrx, string IsInternalUse)
        {
            PAttributesModel obj = new PAttributesModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.LoadInit(mAttributeSetInstanceId, mProductId, productWindow, windowNo, ctx, AD_Column_ID, window_ID, IsSOTrx, IsInternalUse);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExcludeEntry(int productId, int adColumn, int windowNo)
        {
            PAttributesModel obj = new PAttributesModel();
            var value = obj.GetExcludeEntry(productId, adColumn, windowNo, Session["ctx"] as Ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(int windowNoParent, string strLotString, string strSerNo, string dtGuaranteeDate, string strAttrCode,
           bool productWindow, int mAttributeSetInstanceId, int mProductId, int windowNo, string description, bool isEdited, List<KeyNamePair> lst)
        {
            PAttributesModel obj = new PAttributesModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.SaveAttribute(windowNoParent, strLotString, strSerNo, dtGuaranteeDate, strAttrCode,
                 productWindow, mAttributeSetInstanceId, mProductId, windowNo, description, isEdited, lst, ctx);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSerNo(int mAttributeSetInstanceId, int mProductId)
        {
            PAttributesModel obj = new PAttributesModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.GetSerNo(ctx, mAttributeSetInstanceId, mProductId);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CreateLot(int mAttributeSetInstanceId, int mProductId)
        {
            PAttributesModel obj = new PAttributesModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var value = obj.CreateLot(ctx, mAttributeSetInstanceId, mProductId);
                return Json(new { result = value }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttribute(int mAttributeSetInstanceId, int mProductId, bool productWindow, int windowNo, int AD_Column_ID, string attrcode)
        {
            PAttributesModel obj = new PAttributesModel();
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                var AttrValue = obj.GetAttribute(mAttributeSetInstanceId, mProductId, productWindow, windowNo, ctx, AD_Column_ID, attrcode);
                var value = obj.GetAttributeInstance(mAttributeSetInstanceId, mProductId, productWindow, windowNo, ctx, AD_Column_ID, attrcode);
                if (value != null)
                {
                    return Json(JsonConvert.SerializeObject(new { result = AttrValue, lot = value[0], serial = value[1], gdate = value[2] }), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(JsonConvert.SerializeObject(new { result = AttrValue, lot = "", serial = "", gdate = "" }), JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { result = false }, JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 01 june 2017
        public JsonResult GetBPData(int Product_ID, int BPartner_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PAttributesModel model = new PAttributesModel();
            return Json(JsonConvert.SerializeObject(model.GetBPData(Product_ID, BPartner_ID, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 01 june 2017
        public JsonResult GetAttributeData(string Sq1Atribute, int Product_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            Sq1Atribute = SecureEngineBridge.DecryptByClientKey(Sq1Atribute, ctx.GetSecureKey());
            PAttributesModel model = new PAttributesModel();
            return Json(JsonConvert.SerializeObject(model.GetAttributeData(Sq1Atribute, Product_ID, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 01 june 2017
        public JsonResult CheckUniqueLot()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PAttributesModel model = new PAttributesModel();
            return Json(JsonConvert.SerializeObject(model.CheckUniqueLot(ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 01 june 2017
        // Code commented as discussed with Mukesh Sir on 24 May 2018
        // Added by Bharat on 01 june 2017
        public JsonResult CheckAttribute(int WindowNo, int Product_ID, string LotNumber)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PAttributesModel model = new PAttributesModel();
            return Json(JsonConvert.SerializeObject(model.CheckAttribute(WindowNo, Product_ID, LotNumber, ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 01 june 2017
        public JsonResult GetTitle(int Warehouse_ID, int Product_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            PAttributesModel model = new PAttributesModel();
            return Json(JsonConvert.SerializeObject(model.GetTitle(Warehouse_ID, Product_ID, ctx)), JsonRequestBehavior.AllowGet);
        }
    }
}