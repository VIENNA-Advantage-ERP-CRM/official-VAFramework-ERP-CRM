using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;
namespace ViennaAdvantageWeb.Areas.VIS.Controllers
{
    public class OrderController : Controller
    {
        //
        // GET: /VIS/Order/

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetOrder(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                int VAB_Order_ID;

                //Assign parameter value
                VAB_Order_ID = Util.GetValueOfInt(paramValue[0].ToString());
                MVABOrder order = new MVABOrder(ctx, VAB_Order_ID, null);


                Dictionary<String, String> retDic = new Dictionary<string, string>();
                // Reset Orig Shipment

                

                retDic["C_BParter_ID"]= order.GetVAB_BusinessPartner_ID().ToString();
                retDic["VAB_BPart_Location_ID"]= order.GetVAB_BPart_Location_ID().ToString();
                retDic["Bill_BPartner_ID"]= order.GetBill_BPartner_ID().ToString();
                retDic["Bill_Location_ID"]= order.GetBill_Location_ID().ToString();

                if (order.GetVAF_UserContact_ID() != 0)
                    retDic["VAF_UserContact_ID"]= order.GetVAF_UserContact_ID().ToString();

                if (order.GetBill_User_ID() != 0)
                    retDic["Bill_User_ID"]= order.GetBill_User_ID().ToString();

                //if (ctx.IsSOTrx(WindowNo))
                  //  retDic["M_ReturnPolicy_ID"]= bpartner.getM_ReturnPolicy_ID();
                //else
                 //   retDic["M_ReturnPolicy_ID"]= bpartner.getPO_ReturnPolicy_ID();

                //retDic["DateOrdered", order.GetDateOrdered());
                retDic["M_PriceList_ID"]= order.GetM_PriceList_ID().ToString();
                retDic["PaymentRule"]= order.GetPaymentRule();
                retDic["VAB_PaymentTerm_ID"]= order.GetVAB_PaymentTerm_ID().ToString();
                //mTab.setValue ("DeliveryRule", X_VAB_Order.DELIVERYRULE_Manual);

                retDic["Bill_Location_ID"]= order.GetBill_Location_ID().ToString();
                retDic["InvoiceRule"]= order.GetInvoiceRule();
                retDic["PaymentRule"]= order.GetPaymentRule();
                retDic["DeliveryViaRule"]= order.GetDeliveryViaRule();
                retDic["FreightCostRule"]= order.GetFreightCostRule();
                //retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOrderLine(string param)
        {
            string retError = "";
            string retJSON = "";
            if (Session["ctx"] != null)
            {
                VAdvantage.Utility.Ctx ctx = Session["ctx"] as Ctx;
                string[] paramValue = param.Split(',');
                
                Dictionary<String, String> retDic = new Dictionary<string, string>();

                //Assign parameter value
                int id;
                id = Util.GetValueOfInt(paramValue[0].ToString());


                MVABOrderLine orderline = new MVABOrderLine(ctx, id, null);
                //retDic["Orig_InOutLine_ID", null);
                retDic["VAB_TaxRate_ID"]= orderline.GetVAB_TaxRate_ID().ToString();
                retDic["PriceList"]= orderline.GetPriceList().ToString();
                retDic["PriceLimit"]= orderline.GetPriceLimit().ToString();
                retDic["PriceActual"]= orderline.GetPriceActual().ToString();
                retDic["PriceEntered"]= orderline.GetPriceEntered().ToString();
                retDic["VAB_Currency_ID"]= orderline.GetVAB_Currency_ID().ToString();
                retDic["Discount"]= orderline.GetDiscount().ToString();
                retDic["Discount"]= orderline.GetDiscount().ToString();
                //retlst.Add(retValue);

                //retVal.Add(notReserved);


                retJSON = JsonConvert.SerializeObject(retDic);
            }
            else
            {
                retError = "Session Expired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }



    }
}
