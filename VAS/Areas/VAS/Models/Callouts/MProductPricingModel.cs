using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Controllers;

//using ViennaAdvantageWeb.Areas.VIS.DataContracts;

namespace VIS.Models
{
    public class MVAMProductPricingModel
    {
        /// <summary>
        /// GetProductPricing
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public ProductDataOut GetProductPricing(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAM_Product_ID, VAB_BusinessPartner_ID, VAM_PriceList_ID, VAM_PriceListVersion_ID, VAM_PFeature_SetInstance_ID = 0, countED011 = 0, VAB_UOM_ID = 0;
            decimal Qty;
            bool isSOTrx;

            //Assign parameter value
            VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            VAB_BusinessPartner_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Qty = Util.GetValueOfDecimal(paramValue[2].ToString());
            isSOTrx = Convert.ToBoolean(paramValue[3]);
            VAM_PriceList_ID = Util.GetValueOfInt(paramValue[4].ToString());
            VAM_PriceListVersion_ID = Util.GetValueOfInt(paramValue[5].ToString());
            DateTime? orderDate = Util.GetValueOfDateTime(paramValue[6]);
            DateTime? orderDate1 = Util.GetValueOfDateTime(paramValue[7]);

            //if (paramValue.Length > 8)    
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                VAM_PFeature_SetInstance_ID = Util.GetValueOfInt(paramValue[8].ToString());
            }

            if (paramValue.Length > 9)
            {
                if (paramValue.Length == 11)
                {
                    VAB_UOM_ID = Util.GetValueOfInt(paramValue[9].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[10].ToString());
                }
                else if (paramValue.Length == 10)
                {
                    VAB_UOM_ID = Util.GetValueOfInt(paramValue[8].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[9].ToString());
                }
            }

            //End Assign parameter value

            MVAMProductPricing pp = new MVAMProductPricing(ctx.GetVAF_Client_ID(), ctx.GetVAF_Org_ID(),
                        VAM_Product_ID, VAB_BusinessPartner_ID, Qty, isSOTrx);

            //var VAM_PriceList_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceList_ID");
            pp.SetVAM_PriceList_ID(VAM_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            //var VAM_PriceListVersion_ID = ctx.GetContextAsInt(WindowNo, "VAM_PriceListVersion_ID");
            pp.SetVAM_PriceListVersion_ID(VAM_PriceListVersion_ID);

            //if (paramValue.Length > 8)
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                pp.SetVAM_PFeature_SetInstance_ID(VAM_PFeature_SetInstance_ID);
            }
            //var orderDate = System.Convert.ToDateTime(mTab.getValue("DateOrdered"));
            pp.SetPriceDate(orderDate);
            pp.SetPriceDate1(orderDate1);

            if (countED011 > 0)
            {
                pp.SetVAB_UOM_ID(VAB_UOM_ID);
            }

            //Get product stock
            MVAMProduct product = MVAMProduct.Get(ctx, VAM_Product_ID);


            ProductDataOut objInfo = new ProductDataOut
            {


                PriceList = pp.GetPriceList(),
                PriceLimit = pp.GetPriceLimit(),
                PriceActual = pp.GetPriceStd(),
                PriceEntered = pp.GetPriceStd(),
                PriceStd = pp.GetPriceStd(),
                LineAmt = pp.GetLineAmt(2),
                VAB_Currency_ID = System.Convert.ToInt32(pp.GetVAB_Currency_ID()),
                Discount = pp.GetDiscount(),
                VAB_UOM_ID = System.Convert.ToInt32(pp.GetVAB_UOM_ID()),
                //QtyOrdered= mTab.GetValue("QtyEntered"));
                EnforcePriceLimit = pp.IsEnforcePriceLimit(),
                DiscountSchema = pp.IsDiscountSchema(),
                IsStocked = product.IsStocked()

            };
            product = null;
            pp = null;
            return objInfo;
        }
    }
}