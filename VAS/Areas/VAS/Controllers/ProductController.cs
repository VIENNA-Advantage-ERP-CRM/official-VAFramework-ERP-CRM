using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
namespace ViennaAdvantageWeb.Areas.VIS.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /VIS/Product/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetProduct(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID;

                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MProduct product = MProduct.Get(ctx, VAM_Product_ID);


                Dictionary<String, String> retPDic = new Dictionary<string, string>();
                retPDic["IsStocked"] = product.IsStocked().ToString();
                //retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retPDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProductStdPrecision(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID;

                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //Price = Util.GetValueOfInt(paramValue[2].ToString());


                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var VAM_Warehouse_ID = ctx.getContextAsInt(WindowNo, "VAM_Warehouse_ID");
                //var VAM_PFeature_SetInstance_ID = ctx.getContextAsInt(WindowNo, "VAM_PFeature_SetInstance_ID");

                //Decimal? QtyOrdered = (Decimal?)MUOMConversion.ConvertProductTo(ctx, VAM_Product_ID,
                //      VAB_UOM_To_ID, QtyEntered);

                int retValue = MProduct.Get(ctx, VAM_Product_ID).GetUOMPrecision();

                List<int> retlst = new List<int>();

                retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retlst);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
    }
}
