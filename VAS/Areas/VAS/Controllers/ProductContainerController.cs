using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using VAdvantage.Utility;
using VIS.Models;
using VISLogic.Filters;

namespace VIS.Controllers
{
    [AjaxAuthorizeAttribute] // redirect to login page if request is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    public class ProductContainerController : Controller
    {
        /// <summary>
        /// use to Get Product Container on the basis of given paameter 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="WarehouseId"></param>
        /// <param name="LocatorId"></param>
        /// <returns></returns>
        public JsonResult ProductContainer(string Name, int WarehouseId, int LocatorId)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.ProductContainer(Name, WarehouseId, LocatorId)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Update Container reference on the basis of selected parameter
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="RecordId"></param>
        /// <param name="ContainerId"></param>
        /// <returns></returns>
        public JsonResult UpdateProductContainer(string TableName, int RecordId, int ContainerId)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.UpdateProductContainer(TableName, RecordId, ContainerId)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Warehouses
        /// </summary>
        /// <param name="fromWarehouse_ID"></param>
        /// <returns></returns>
        public JsonResult LoadWarehouse(int fromWarehouse_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetWarehouse(fromWarehouse_ID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get locator based on warehouses
        /// </summary>
        /// <param name="fromWarehouse_ID"></param>
        /// <returns></returns>
        public JsonResult LoadLocator(int fromWarehouse_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetLocator(fromWarehouse_ID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get container
        /// </summary>
        /// <param name="warehouse"></param>
        /// <param name="locator"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        public JsonResult LoadContainer(int warehouse, int locator, int container)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetContainer(warehouse, locator, container)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Product from Transaction (Container Wise)
        /// </summary>
        /// <param name="container"></param>
        /// <param name="movementDate"></param>
        /// <param name="AD_Org_ID"></param>
        /// <param name="locator"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public JsonResult MoveContainer(int container, DateTime? movementDate, int AD_Org_ID, int locator, int page, int size)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetProductContainerFromTransaction(container, movementDate, AD_Org_ID, locator, page, size)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Is used to save data on movememt line
        /// </summary>
        /// <param name="moveData"></param>
        /// <returns></returns>
        public JsonResult SaveMoveData(string moveData)
        {
            List<Dictionary<string, string>> mData = null;
            if (moveData != null)
            {
                mData = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(moveData);
            }
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.SaveMovementLine(mData)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// result container in tree structure
        /// </summary>
        /// <param name="warehouse"></param>
        /// <param name="locator"></param>
        /// <param name="container"></param>
        /// <param name="validation"></param>
        /// <returns></returns>
        public JsonResult LoadContainerAsTree(int warehouse, int locator, int container, string validation)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetContainerAsTree(warehouse, locator, container, validation)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Return Value_Name for requested Container's ID
        /// Created BY Karan
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ContentResult GetProductContainerInfo(int ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Content(model.GetProductContainerInfo(ID));
        }

        /// <summary>
        /// Return ID for requested Container 
        /// Created BY Karan
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public JsonResult GetProductContainer(string text, string validation)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.GetProductContainer(text, validation)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// is used to save Product container
        /// </summary>
        /// <param name="warehouseId">Warehouse where we create container</param>
        /// <param name="locatorId">Locator - in which locator we place container</param>
        /// <param name="value">Search key of the container</param>
        /// <param name="name">name of teh container</param>
        /// <param name="height">height of the container</param>
        /// <param name="width">width of the container</param>
        /// <param name="parentContainerId">Parent of the nw container</param>
        /// <returns>Save Or Not Saved message</returns>
        /// <writer>Amit Bansal</writer>
        public JsonResult SaveContainer(int warehouseId, int locatorId, string value, string name, Decimal height, Decimal width, int parentContainerId)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            ProductContainerModel model = new ProductContainerModel(ctx);
            return Json(JsonConvert.SerializeObject(model.SaveProductContainer(warehouseId, locatorId, value, name, height, width, parentContainerId)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Is Used to get Name of Warehouse and Locator
        /// </summary>
        /// <param name="fields">contain M_Locator_ID reference</param>
        /// <returns>Warehouse and Locator Name</returns>
        /// <writer>Amit Bansal</writer>
        public JsonResult GetWarehouseAndLocatorName(string fields)
        {
            var labelInfo = "";
            Ctx ctx = Session["ctx"] as Ctx;
            VAdvantage.Model.MLocator loc = VAdvantage.Model.MLocator.Get(ctx, Util.GetValueOfInt(fields));
            if (loc != null && loc.GetM_Locator_ID() > 0)
            {
                labelInfo = Msg.GetMsg(ctx, "M_Warehouse_ID") + " : " + loc.GetWarehouseName() + " , " + Msg.GetMsg(ctx, "M_Locator_ID") + " : " + loc.GetValue();
            }
            return Json(JsonConvert.SerializeObject(labelInfo), JsonRequestBehavior.AllowGet);
        }
    }
}