using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VIS.Models;
using VAdvantage.Utility;

namespace VIS.Controllers
{
    public class RelatedProductController : Controller
    {
        // GET: RelatedProduct
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Getting Product Data
        /// </summary>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute</param>
        /// <param name="M_PriceList_ID">Price List</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Product data in json format</returns>
        public JsonResult GetProductData(int M_Product_ID, int M_AttributeSetInstance_ID, int C_UOM_ID, int M_PriceList_ID, int Table_ID, int Record_ID)
        {
            string Result = string.Empty;
            if (Session["ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                RelatedProductModel obj = new RelatedProductModel();
                Result = JsonConvert.SerializeObject(obj.GetProductData(ctx, M_Product_ID, M_AttributeSetInstance_ID, C_UOM_ID, M_PriceList_ID, Table_ID, Record_ID));
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Getting Related Product Data
        /// </summary>
        /// <param name="M_Product_ID">Product</param>
        /// <param name="M_AttributeSetInstance_ID">Attribute</param>
        /// <param name="M_PriceList_ID">Price List</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>Related Product data in json format</returns>
        public JsonResult GetRelatedProduct(int M_Product_ID, int M_AttributeSetInstance_ID, int C_UOM_ID, int M_PriceList_ID, string RelatedProductType, int Table_ID, int Record_ID)
        {
            string Result = string.Empty;
            if (Session["ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                RelatedProductModel obj = new RelatedProductModel();
                Result = JsonConvert.SerializeObject(obj.GetRelatedProduct(ctx, M_Product_ID, M_AttributeSetInstance_ID, C_UOM_ID, M_PriceList_ID, RelatedProductType, Table_ID, Record_ID));
            }
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save Related Product lines 
        /// </summary>
        /// <param name="Data">Related Product Data</param>
        /// <param name="Table_ID">Table ID</param>
        /// <param name="Record_ID">Record ID</param>
        /// <returns>message as string</returns>
        [HttpPost]
        public JsonResult SaveRelatedLines(List<RelatedProductData> Data, int Table_ID, int Record_ID)
        {
            string result = "";
            if (Session["ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                RelatedProductModel obj = new RelatedProductModel();
                result = obj.CreateRelatedLines(ctx, Data, Table_ID, Record_ID);
            }
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }
    }
}