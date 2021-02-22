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
    public class MVAMProductController:Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetProduct(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetProduct(ctx,fields));
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }    
        public JsonResult GetUOMPrecision(string fields)
        {
            
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetUOMPrecision(ctx, fields));
            }           
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVAB_UOM_ID(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetVAB_UOM_ID(ctx,fields));
            }            
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductType(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetProductType(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        // Added by amit
        public JsonResult GetTaxCategory(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetTaxCategory(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
        //End

        /// <summary>
        /// Get VAB_Rev_Recognition_ID
        /// </summary>
        /// <param name="fields">C_Product_ID</param>
        /// <returns>VAB_Rev_Recognition_ID</returns>
        public JsonResult GetRevenuRecognition(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                MVAMProductModel objProduct = new MVAMProductModel();
                retJSON = JsonConvert.SerializeObject(objProduct.GetRevenuRecognition(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
    }
}