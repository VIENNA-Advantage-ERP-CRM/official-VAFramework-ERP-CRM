using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;
using System.Globalization;

namespace VIS.Controllers
{
    public class MInOutController:Controller
    {
        //
        // GET: /VIS/InOut/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetInOut(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                retJSON = JsonConvert.SerializeObject(objInOut.GetInOut(ctx,fields));
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Change GSI Barcode
        public JsonResult GetASIID(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                PAttributesModel pMod = new PAttributesModel();

                string[] vals = null;

                int M_Product_ID = 0;
                int windowNo = 0;
                string attrCode = "";

                if (fields != null)
                {
                    vals = fields.Split(',');
                    M_Product_ID = Util.GetValueOfInt(vals[1]);
                    windowNo = Util.GetValueOfInt(vals[2]);
                    attrCode = Util.GetValueOfString(vals[0]);
                }
                AttributeInstance aIns = null;
                string lotNo = "";
                int M_AttributeSetInstance_ID = 0;

                try
                {
                    if (attrCode.IndexOf("(10)") > 0)
                    {
                        lotNo = attrCode.Substring(attrCode.IndexOf("(10)")).Replace("(10)", "");
                        if (lotNo.IndexOf("(17)") > 0)
                        {
                            lotNo = lotNo.Substring(0, lotNo.IndexOf("(17)"));
                        }
                    }

                    string expDate = "";

                    if (attrCode.IndexOf("(17)") > 0)
                    {
                        expDate = attrCode.Substring(attrCode.IndexOf("(17)")).Replace("(17)", "");
                        if (expDate.IndexOf("(10)") > 0)
                        {
                            expDate = expDate.Substring(0, expDate.IndexOf("(10)"));
                        }
                    }
                    //DateTime.ParseExact(expDate, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None);
                    DateTime? dTime = null;
                    if (expDate != null && expDate != "")
                    {
                        expDate = DateTime.ParseExact(expDate, "yyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None).ToShortDateString();
                    }
                    else
                    {
                        expDate = "";
                    }

                    aIns = pMod.SaveAttributeMR(0, lotNo, "", expDate, attrCode, false, 0, M_Product_ID, 0, null, ctx);
                    M_AttributeSetInstance_ID = aIns.M_AttributeSetInstance_ID;
                }
                catch (Exception ex)
                {
                    M_AttributeSetInstance_ID = 0;
                    aIns = null;
                }

                retJSON = Util.GetValueOfString(M_AttributeSetInstance_ID);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Change by mohit to remove client side queries- 19 May 2017
        public JsonResult GetDOcTypeData(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                retJSON = JsonConvert.SerializeObject(objInOut.GetDocumentTypeData(fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWHLocator(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                retJSON = JsonConvert.SerializeObject(objInOut.GetWarehouseLocator(fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Get Product, Uom Conversion
        public JsonResult GetUOMConv(string fields)
        {
            String retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                objInOut.GetUOMConversion(ctx, fields);
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        // Get Locator from product

        // Added by Bharat on 19 May 2017
        public JsonResult GetWarehouse(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInOutModel objInOut = new MInOutModel();
                retJSON = JsonConvert.SerializeObject(objInOut.GetWarehouse(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Default Locator ID
        /// </summary>
        /// <param name="fields">Warehouse ID</param>
        /// <returns>Default Locator ID</returns>
        /// <writer>Amit</writer>
        public JsonResult GetDefaultLocatorID(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                retJSON = JsonConvert.SerializeObject(MWarehouse.Get(ctx, Util.GetValueOfInt(fields)).GetDefaultM_Locator_ID());
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

    }
}