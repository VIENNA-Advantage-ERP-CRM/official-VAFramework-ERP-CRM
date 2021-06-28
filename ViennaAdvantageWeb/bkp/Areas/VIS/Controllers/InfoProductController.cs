using System.Collections.Generic;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;
using VIS.Classes;
namespace VIS.Controllers
{
    public class InfoProductController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetInfoColumns(string tableName)
        {
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetInfoColumns(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult GetData(string sql, string where, string tableName, int pageNo, bool ForMobile)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            sql = SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
            where = SecureEngineBridge.DecryptByClientKey(where, ctx.GetSecureKey());
            return Json(JsonConvert.SerializeObject(model.GetData(sql, where, tableName, pageNo, ForMobile, ctx)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCart(string sql, int pageNo, bool isCart, int windowID, int WarehouseID, int WarehouseToID, int LocatorID, int LocatorToID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            sql = SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
            return Json(JsonConvert.SerializeObject(model.GetCart(sql, pageNo, isCart, windowID, WarehouseID, WarehouseToID, LocatorID, LocatorToID, ctx)), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(int id, string keyColumn, string prod, string C_UOM_ID, string listAst, string qty, int locatorTo, int lineID, string InvCountID, string ReferenceNo, int Locator_ID, int WindowID, int ContainerID, int ContainerToID)
        {
            List<string> prodID = new List<string>();
            if (prod != null && prod.Trim().Length > 0)
            {
                prodID = JsonConvert.DeserializeObject<List<string>>(prod);
            }
            List<string> uomID = new List<string>();
            if (C_UOM_ID != null && C_UOM_ID.Trim().Length > 0)
            {
                uomID = JsonConvert.DeserializeObject<List<string>>(C_UOM_ID);
            }
            List<string> Attributes = new List<string>();
            if (listAst != null && listAst.Trim().Length > 0)
            {
                Attributes = JsonConvert.DeserializeObject<List<string>>(listAst);
            }
            List<string> quantity = new List<string>();
            if (qty != null && qty.Trim().Length > 0)
            {
                quantity = JsonConvert.DeserializeObject<List<string>>(qty);
            }
            List<string> countID = new List<string>();
            if (InvCountID != null && InvCountID.Trim().Length > 0)
            {
                countID = JsonConvert.DeserializeObject<List<string>>(InvCountID);
            }
            List<string> RefNo = new List<string>();
            if (ReferenceNo != null && ReferenceNo.Trim().Length > 0)
            {
                RefNo = JsonConvert.DeserializeObject<List<string>>(ReferenceNo);
            }
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            var value = model.SetProductQty(id, keyColumn, prodID, uomID, Attributes, quantity, locatorTo, lineID, countID, RefNo, Locator_ID, WindowID, ContainerID, ContainerToID, Session["ctx"] as Ctx);
            return Json(JsonConvert.SerializeObject(value), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveStockTfr(int id, string keyColumn, int AD_Table_ID, string prod, string C_UOM_ID, string listAst, string qty, string listLoc, int locatorTo, string astID, int lineID, int ContainerID)
        {
            List<string> prodID = new List<string>();
            if (prod != null && prod.Trim().Length > 0)
            {
                prodID = JsonConvert.DeserializeObject<List<string>>(prod);
            }
            List<string> uomID = new List<string>();
            if (C_UOM_ID != null && C_UOM_ID.Trim().Length > 0)
            {
                uomID = JsonConvert.DeserializeObject<List<string>>(C_UOM_ID);
            }
            List<string> Attributes = new List<string>();
            if (listAst != null && listAst.Trim().Length > 0)
            {
                Attributes = JsonConvert.DeserializeObject<List<string>>(listAst);
            }
            List<string> quantity = new List<string>();
            if (qty != null && qty.Trim().Length > 0)
            {
                quantity = JsonConvert.DeserializeObject<List<string>>(qty);
            }
            
            List<string> Locators = new List<string>();
            if (listLoc != null && listLoc.Trim().Length > 0)
            {
                Locators = JsonConvert.DeserializeObject<List<string>>(listLoc);
            }
            
            VIS.Models.InfoProductModel model = new Models.InfoProductModel();
            var value = model.SetProductQtyStockTrasfer(id, keyColumn, AD_Table_ID, prodID, uomID, Attributes, quantity, Locators, locatorTo, lineID, ContainerID, Session["ctx"] as Ctx);
            return Json(new { result = value }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAttribute(string fields)
        {
            KeyNamePair retJSON = null;
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                VIS.Models.InfoProductModel obj = new VIS.Models.InfoProductModel();
                retJSON = obj.GetAttribute(ctx, fields);
            }
            return Json(JsonConvert.SerializeObject(retJSON), JsonRequestBehavior.AllowGet);
        }

        // Variant Changes Done By Mohit 08/07/20161
        [HttpPost]
        public JsonResult GetVariants(int M_Product_ID, int M_Warehouse_ID, int ParentRec_ID, int M_AttributeSetInstance_ID, string AttributeCode)
        {

            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            //model.GetSchema(Ad_InfoWindow_ID);
            return Json(JsonConvert.SerializeObject(model.GetVariants(M_Product_ID, M_Warehouse_ID, ParentRec_ID, M_AttributeSetInstance_ID, AttributeCode, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetWindowID(string fields)
        {            
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();           
            return Json(JsonConvert.SerializeObject(model.GetWindowID(fields, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetWarehouse()
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetWarehouse(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// function to fetch UOM 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUOM()
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetUOM(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetPriceList(int PriceList)
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetPriceList(PriceList, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetDefaultPriceList()
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetDefaultPriceList(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetPriceListVersion(int PriceList)
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetPriceListVersion(PriceList, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult GetAttributeSet()
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetAttributeSet(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        public JsonResult DeleteCart(int invCount_ID)
        {
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.DeleteCart(invCount_ID, Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }

        // Added by Bharat on 31 May 2017
        // Added new parameter for Window ID by Lokesh Chauhan on 20 Dec 2018
        public JsonResult GetCartData(string invCount_ID, int WindowID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            VIS.Models.InfoProductModel model = new VIS.Models.InfoProductModel();
            return Json(JsonConvert.SerializeObject(model.GetCartData(invCount_ID, WindowID, ctx)), JsonRequestBehavior.AllowGet);
        }
    }
}