using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using VAdvantage.Utility;
using VIS.Filters;
using VIS.Models;

namespace VIS.Controllers
{
    public class AmountDivisionController : Controller
    {
        AmountDivisionModel model = new AmountDivisionModel();
        
        // GET: /VIS/AmountDivision/
        public ActionResult Index()
        {
            return View();
        }
       

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetDiminsionType()
        {
            
            return Json(JsonConvert.SerializeObject(model.GetDimensionType(Session["ctx"] as Ctx)), JsonRequestBehavior.AllowGet);
        }
        //[AjaxAuthorizeAttribute]
        //[AjaxSessionFilterAttribute]
        //[HttpPost]
        //[ValidateInput(false)]
        //public JsonResult InsertDimensionAmount(string acctSchema,string elementType, decimal amount, string dimensionLine, int DimAmtId)
        //{
        //    List<AmountDivisionModel> dim = JsonConvert.DeserializeObject<List<AmountDivisionModel>>(dimensionLine);
        //    int[] accountSchema = JsonConvert.DeserializeObject<int[]>(acctSchema);
        //    var temp = accountSchema.Distinct().ToArray();
        //    accountSchema = temp;
        //    string[] dimensionElement = JsonConvert.DeserializeObject<string[]>(elementType);
        //    return Json(JsonConvert.SerializeObject(model.InsertDimensionAmount(accountSchema, dimensionElement, amount, dim, Session["ctx"] as Ctx, DimAmtId)), JsonRequestBehavior.AllowGet);
        //}

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetDiminsionLine(string accountingSchema, int dimensionID, int DimensionLineID = 0, int pageNo = 0, int pSize = 0)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(accountingSchema);
            return Json(JsonConvert.SerializeObject(model.GetDimensionLine(Session["ctx"] as Ctx,accountSchema, dimensionID, DimensionLineID, pageNo, pSize)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Insert Line on Amount Dimension
        /// </summary>
        /// <param name="recordId">Amount Dimension ID</param>
        /// <param name="totalAmount">Total Amount</param>
        /// <param name="lineAmount">Line Amount</param>
        /// <param name="acctSchemaID">selected Accounting Schema</param>
        /// <param name="elementTypeID">Element Type</param>
        /// <param name="dimensionValue">Dimension Value</param>
        /// <param name="elementID">Element ID</param>
        /// <param name="oldDimensionName">Old Dimension Value</param>
        /// <param name="bpartner_ID">Business Partner</param>
        /// <param name="oldBPartner_ID"Old Business Partner></param>
        /// <returns>Data in JSON format</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult InsertDimensionLine(int recordId, decimal totalAmount, decimal lineAmount, string acctSchemaID, string elementTypeID, int dimensionValue, int elementID, int oldDimensionName, int bpartner_ID, int oldBPartner_ID)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(acctSchemaID);
            return Json(JsonConvert.SerializeObject(model.InsertDimensionLine(Session["ctx"] as Ctx, recordId, totalAmount, lineAmount, accountSchema, elementTypeID, dimensionValue, elementID, oldDimensionName, bpartner_ID, oldBPartner_ID)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetAccountingSchema(int OrgID)
        {

            return Json(JsonConvert.SerializeObject(model.GetAccountingSchema(Session["ctx"] as Ctx,OrgID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Element linked with Account on Accounting Schema Element
        /// </summary>
        /// <param name="accountingSchema">selected Accounting Schema</param>
        /// <returns>Element ID</returns>
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        public JsonResult GetElementID(string accountingSchema)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(accountingSchema);
            return Json(JsonConvert.SerializeObject(model.GetElementID(Session["ctx"] as Ctx, accountSchema)), JsonRequestBehavior.AllowGet);
        }
    }
}