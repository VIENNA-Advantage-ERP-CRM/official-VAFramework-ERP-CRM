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

        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult InsertDimensionLine(int recordId, decimal totalAmount, decimal lineAmount, string acctSchemaID, string elementTypeID, int dimensionValue, int elementID, int oldDimensionName)
        {
            int[] accountSchema = JsonConvert.DeserializeObject<int[]>(acctSchemaID);
            return Json(JsonConvert.SerializeObject(model.InsertDimensionLine(Session["ctx"] as Ctx, recordId, totalAmount, lineAmount, accountSchema, elementTypeID, dimensionValue, elementID, oldDimensionName)), JsonRequestBehavior.AllowGet);
        }
        [AjaxAuthorizeAttribute]
        [AjaxSessionFilterAttribute]
        [HttpPost]
        public JsonResult GetAccountingSchema(int OrgID)
        {

            return Json(JsonConvert.SerializeObject(model.GetAccountingSchema(Session["ctx"] as Ctx,OrgID)), JsonRequestBehavior.AllowGet);
        }
	}
}