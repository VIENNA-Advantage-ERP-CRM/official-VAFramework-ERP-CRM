using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Models;

namespace VIS.Controllers
{
    public class MInventoryLineController:Controller
    {
        //
        // GET: /VIS/CalloutOrder/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetMInventoryLine(string fields)
        {
           
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInventoryLineModel objInventoryLine = new MInventoryLineModel();
                retJSON = JsonConvert.SerializeObject(objInventoryLine.GetMInventoryLine(ctx,fields));
            }          
            return Json(retJSON, JsonRequestBehavior.AllowGet);
           // return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        // Added by mohit to get product UOM- 12 June 2018
        /// <summary>
        /// get Product UOM
        /// </summary>
        /// <param name="fields=product ID"></param>
        /// <returns></returns>
        public JsonResult GetProductUOM(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInventoryLineModel objInventoryLine = new MInventoryLineModel();
                retJSON = JsonConvert.SerializeObject(objInventoryLine.GetproductUOM(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get the attribute of a product from a given UPC/EAN number.
        /// </summary>
        /// <param name="fields">Parameters sent from client side includes product id and attribute code(UPC/EAN).</param>
        /// <returns>Returns attributesetinstance id if found as jsonString.</returns>
        public JsonResult GetProductAttribute(string fields)
        {
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                MInventoryLineModel objInventoryLine = new MInventoryLineModel();
                retJSON = JsonConvert.SerializeObject(objInventoryLine.GetProductAttribute(ctx, fields));
            }
            return Json(retJSON, JsonRequestBehavior.AllowGet);
        }
      
    }
}