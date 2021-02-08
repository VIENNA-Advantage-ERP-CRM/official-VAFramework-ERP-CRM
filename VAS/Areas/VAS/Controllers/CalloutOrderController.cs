using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
namespace VIS.Controllers
{
    public class CalloutOrderController : Controller
    {
        //
        // GET: /VIS/CalloutOrder/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetProductPricing(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID, VAB_BusinessPartner_ID, VAM_PriceList_ID, VAM_PriceListVersion_ID;
                decimal Qty;
                bool isSOTrx;

                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                VAB_BusinessPartner_ID = Util.GetValueOfInt(paramValue[1].ToString());
                Qty = Util.GetValueOfDecimal(paramValue[2].ToString());
                isSOTrx = Util.GetValueOfBool(paramValue[3].ToString());
                VAM_PriceList_ID = Util.GetValueOfInt(paramValue[4].ToString());
                VAM_PriceListVersion_ID = Util.GetValueOfInt(paramValue[5].ToString());
                DateTime orderDate = System.Convert.ToDateTime(paramValue[6].ToString());

                //End Assign parameter value

                MProductPricing pp = new MProductPricing(ctx.GetVAF_Client_ID(), ctx.GetVAF_Org_ID(),
                            VAM_Product_ID, VAB_BusinessPartner_ID, Qty, isSOTrx);

                //var VAM_PriceList_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceList_ID");
                pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
                /** PLV is only accurate if PL selected in header */
                //var VAM_PriceListVersion_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
                pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);

                //var orderDate = System.Convert.ToDateTime(mTab.getValue("DateOrdered"));
                pp.SetPriceDate(orderDate);

                //Get product stock
                MProduct product = MProduct.Get(ctx, VAM_Product_ID);


                VIS.DataContracts.ProductDataOut objInfo = new VIS.DataContracts.ProductDataOut
                {


                    PriceList = pp.GetPriceList(),
                    PriceLimit = pp.GetPriceLimit(),
                    PriceActual = pp.GetPriceStd(),
                    PriceEntered = pp.GetPriceStd(),
                    PriceStd=pp.GetPriceStd(),
                    VAB_Currency_ID = System.Convert.ToInt32(pp.GetVAB_Currency_ID()),
                    Discount = pp.GetDiscount(),

                    VAB_UOM_ID = System.Convert.ToInt32(pp.GetVAB_UOM_ID()),
                    //QtyOrdered= mTab.GetValue("QtyEntered"));
                    EnforcePriceLimit = pp.IsEnforcePriceLimit(),
                    DiscountSchema = pp.IsDiscountSchema(),
                    IsStocked=product.IsStocked()

                };
                product = null;
                pp = null;
                retJSON = JsonConvert.SerializeObject(objInfo);
            }
            else {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQtyInfo(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID, VAM_Warehouse_ID, VAM_PFeature_SetInstance_ID, VAB_OrderLine_ID;
                
                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[1].ToString());
                VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
                VAB_OrderLine_ID = Util.GetValueOfInt(paramValue[3].ToString());
                
                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var VAM_Warehouse_ID = ctx.getContextAsInt(WindowNo, "VAM_Warehouse_ID");
                //var VAM_PFeature_SetInstance_ID = ctx.getContextAsInt(WindowNo, "VAM_PFeature_SetInstance_ID");
                Decimal? available = MStorage.GetQtyAvailable(VAM_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID, null);
                
                Decimal notReserved = MVABOrderLine.GetNotReserved(ctx,
                                VAM_Warehouse_ID, VAM_Product_ID, VAM_PFeature_SetInstance_ID,
                                VAB_OrderLine_ID);
                
                List<Decimal?> retVal = new List<Decimal?>();
                
                retVal.Add(available);
                
                retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retVal);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConvertProductTo(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID, VAB_UOM_To_ID, QtyEntered;

                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
                QtyEntered = Util.GetValueOfInt(paramValue[2].ToString());
                

                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var VAM_Warehouse_ID = ctx.getContextAsInt(WindowNo, "VAM_Warehouse_ID");
                //var VAM_PFeature_SetInstance_ID = ctx.getContextAsInt(WindowNo, "VAM_PFeature_SetInstance_ID");

                Decimal? retValue = (Decimal?)MUOMConversion.ConvertProductTo(ctx, VAM_Product_ID,
                      VAB_UOM_To_ID, QtyEntered);


                List<Decimal?> retlst = new List<Decimal?>();

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
        public JsonResult ConvertProductFrom(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int VAM_Product_ID, VAB_UOM_To_ID, Price;

                //Assign parameter value
                VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
                Price = Util.GetValueOfInt(paramValue[2].ToString());


                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var VAM_Warehouse_ID = ctx.getContextAsInt(WindowNo, "VAM_Warehouse_ID");
                //var VAM_PFeature_SetInstance_ID = ctx.getContextAsInt(WindowNo, "VAM_PFeature_SetInstance_ID");

                //Decimal? QtyOrdered = (Decimal?)MUOMConversion.ConvertProductTo(ctx, VAM_Product_ID,
                //      VAB_UOM_To_ID, QtyEntered);

                Decimal? retValue = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, VAM_Product_ID,
               VAB_UOM_To_ID, Price);

                List<Decimal?> retlst = new List<Decimal?>();

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
        public JsonResult GetPrecision(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int  VAB_UOM_To_ID;

                //Assign parameter value
                //VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                VAB_UOM_To_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //Price = Util.GetValueOfInt(paramValue[2].ToString());


                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var VAM_Warehouse_ID = ctx.getContextAsInt(WindowNo, "VAM_Warehouse_ID");
                //var VAM_PFeature_SetInstance_ID = ctx.getContextAsInt(WindowNo, "VAM_PFeature_SetInstance_ID");

                //Decimal? QtyOrdered = (Decimal?)MUOMConversion.ConvertProductTo(ctx, VAM_Product_ID,
                //      VAB_UOM_To_ID, QtyEntered);

                int retValue=MUOM.GetPrecision(ctx, VAB_UOM_To_ID);

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
       
        public JsonResult GetMovementQty(string fields)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = fields.Split(',');
                int inOutLine_ID;

                //Assign parameter value
                inOutLine_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MInOutLine inOutLine = new MInOutLine(ctx, inOutLine_ID, null);
                var retValue = inOutLine.GetMovementQty();


                List<Decimal> retlst = new List<Decimal>();

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
