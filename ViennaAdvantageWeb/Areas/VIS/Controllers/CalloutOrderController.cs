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
                int M_Product_ID, C_BPartner_ID, M_PriceList_ID, M_PriceList_Version_ID;
                decimal Qty;
                bool isSOTrx;

                //Assign parameter value
                M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                C_BPartner_ID = Util.GetValueOfInt(paramValue[1].ToString());
                Qty = Util.GetValueOfDecimal(paramValue[2].ToString());
                isSOTrx = Util.GetValueOfBool(paramValue[3].ToString());
                M_PriceList_ID = Util.GetValueOfInt(paramValue[4].ToString());
                M_PriceList_Version_ID = Util.GetValueOfInt(paramValue[5].ToString());
                DateTime orderDate = System.Convert.ToDateTime(paramValue[6].ToString());

                //End Assign parameter value

                MProductPricing pp = new MProductPricing(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
                            M_Product_ID, C_BPartner_ID, Qty, isSOTrx);

                //var M_PriceList_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_ID");
                pp.SetM_PriceList_ID(M_PriceList_ID);
                /** PLV is only accurate if PL selected in header */
                //var M_PriceList_Version_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
                pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

                //var orderDate = System.Convert.ToDateTime(mTab.getValue("DateOrdered"));
                pp.SetPriceDate(orderDate);

                //Get product stock
                MProduct product = MProduct.Get(ctx, M_Product_ID);


                VIS.DataContracts.ProductDataOut objInfo = new VIS.DataContracts.ProductDataOut
                {


                    PriceList = pp.GetPriceList(),
                    PriceLimit = pp.GetPriceLimit(),
                    PriceActual = pp.GetPriceStd(),
                    PriceEntered = pp.GetPriceStd(),
                    PriceStd=pp.GetPriceStd(),
                    C_Currency_ID = System.Convert.ToInt32(pp.GetC_Currency_ID()),
                    Discount = pp.GetDiscount(),

                    C_UOM_ID = System.Convert.ToInt32(pp.GetC_UOM_ID()),
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
                int M_Product_ID, M_Warehouse_ID, M_AttributeSetInstance_ID, C_OrderLine_ID;
                
                //Assign parameter value
                M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                M_Warehouse_ID = Util.GetValueOfInt(paramValue[1].ToString());
                M_AttributeSetInstance_ID = Util.GetValueOfInt(paramValue[2].ToString());
                C_OrderLine_ID = Util.GetValueOfInt(paramValue[3].ToString());
                
                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var M_Warehouse_ID = ctx.getContextAsInt(WindowNo, "M_Warehouse_ID");
                //var M_AttributeSetInstance_ID = ctx.getContextAsInt(WindowNo, "M_AttributeSetInstance_ID");
                Decimal? available = MStorage.GetQtyAvailable(M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID, null);
                
                Decimal notReserved = MOrderLine.GetNotReserved(ctx,
                                M_Warehouse_ID, M_Product_ID, M_AttributeSetInstance_ID,
                                C_OrderLine_ID);
                
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
                int M_Product_ID, C_UOM_To_ID, QtyEntered;

                //Assign parameter value
                M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                C_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
                QtyEntered = Util.GetValueOfInt(paramValue[2].ToString());
                

                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var M_Warehouse_ID = ctx.getContextAsInt(WindowNo, "M_Warehouse_ID");
                //var M_AttributeSetInstance_ID = ctx.getContextAsInt(WindowNo, "M_AttributeSetInstance_ID");

                Decimal? retValue = (Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                      C_UOM_To_ID, QtyEntered);


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
                int M_Product_ID, C_UOM_To_ID, Price;

                //Assign parameter value
                M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                C_UOM_To_ID = Util.GetValueOfInt(paramValue[1].ToString());
                Price = Util.GetValueOfInt(paramValue[2].ToString());


                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var M_Warehouse_ID = ctx.getContextAsInt(WindowNo, "M_Warehouse_ID");
                //var M_AttributeSetInstance_ID = ctx.getContextAsInt(WindowNo, "M_AttributeSetInstance_ID");

                //Decimal? QtyOrdered = (Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //      C_UOM_To_ID, QtyEntered);

                Decimal? retValue = (Decimal?)MUOMConversion.ConvertProductFrom(ctx, M_Product_ID,
               C_UOM_To_ID, Price);

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
                int  C_UOM_To_ID;

                //Assign parameter value
                //M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
                C_UOM_To_ID = Util.GetValueOfInt(paramValue[0].ToString());
                //Price = Util.GetValueOfInt(paramValue[2].ToString());


                //End Assign parameter value
                //var QtyOrdered = Utility.Util.getValueOfDecimal(mTab.getValue("QtyOrdered"));
                //var M_Warehouse_ID = ctx.getContextAsInt(WindowNo, "M_Warehouse_ID");
                //var M_AttributeSetInstance_ID = ctx.getContextAsInt(WindowNo, "M_AttributeSetInstance_ID");

                //Decimal? QtyOrdered = (Decimal?)MUOMConversion.ConvertProductTo(ctx, M_Product_ID,
                //      C_UOM_To_ID, QtyEntered);

                int retValue=MUOM.GetPrecision(ctx, C_UOM_To_ID);

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
