﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;
using VIS.DataContracts;
using VIS.Controllers;
using VAdvantage.DataBase;

//using ViennaAdvantageWeb.Areas.VIS.DataContracts;

namespace VIS.Models
{
    public class MProductPricingModel
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
            int M_Product_ID, C_BPartner_ID, M_PriceList_ID, M_PriceList_Version_ID, M_AttributeSetInstance_ID = 0, countED011 = 0, C_UOM_ID = 0;
            decimal Qty;
            bool isSOTrx;

            //Assign parameter value
            M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            C_BPartner_ID = Util.GetValueOfInt(paramValue[1].ToString());
            Qty = Util.GetValueOfDecimal(paramValue[2].ToString());
            isSOTrx = Convert.ToBoolean(paramValue[3]);
            M_PriceList_ID = Util.GetValueOfInt(paramValue[4].ToString());
            M_PriceList_Version_ID = Util.GetValueOfInt(paramValue[5].ToString());
            DateTime? orderDate = Util.GetValueOfDateTime(paramValue[6]);
            DateTime? orderDate1 = Util.GetValueOfDateTime(paramValue[7]);
                    
            //if (paramValue.Length > 8)    
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                M_AttributeSetInstance_ID = Util.GetValueOfInt(paramValue[8].ToString());
            }

            if (paramValue.Length > 9)
            {
                if (paramValue.Length == 11)
                {
                    C_UOM_ID = Util.GetValueOfInt(paramValue[9].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[10].ToString());
                }
                else if (paramValue.Length == 10)
                {
                    C_UOM_ID = Util.GetValueOfInt(paramValue[8].ToString());
                    countED011 = Util.GetValueOfInt(paramValue[9].ToString());
                }
            }

            /** Price List - ValidFrom date validation ** Dt:01/02/2021 ** Modified By: Kumar **/
            if (!string.IsNullOrEmpty(Util.GetValueOfString(orderDate)))
            {
                StringBuilder sbparams = new StringBuilder();
                sbparams.Append(Util.GetValueOfInt(M_PriceList_ID));
                sbparams.Append(",").Append(Convert.ToDateTime(orderDate).ToString("MM-dd-yyyy"));
                sbparams.Append(",").Append(Util.GetValueOfInt(M_Product_ID));
                sbparams.Append(",").Append(Util.GetValueOfInt(C_UOM_ID));
                sbparams.Append(",").Append(Util.GetValueOfInt(M_AttributeSetInstance_ID));
                MPriceListVersionModel objPriceList = new MPriceListVersionModel();
                M_PriceList_Version_ID = objPriceList.GetM_PriceList_Version_ID_On_Transaction_Date(ctx, sbparams.ToString());
            }

            //End Assign parameter value

            MProductPricing pp = new MProductPricing(ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
                        M_Product_ID, C_BPartner_ID, Qty, isSOTrx);

            //var M_PriceList_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_ID");
            pp.SetM_PriceList_ID(M_PriceList_ID);
            /** PLV is only accurate if PL selected in header */
            //var M_PriceList_Version_ID = ctx.GetContextAsInt(WindowNo, "M_PriceList_Version_ID");
            pp.SetM_PriceList_Version_ID(M_PriceList_Version_ID);

            //if (paramValue.Length > 8)
            if (paramValue.Length == 9 || paramValue.Length == 11)
            {
                pp.SetM_AttributeSetInstance_ID(M_AttributeSetInstance_ID);
            }
            //var orderDate = System.Convert.ToDateTime(mTab.getValue("DateOrdered"));
            pp.SetPriceDate(orderDate);
            pp.SetPriceDate1(orderDate1);

            if (countED011 > 0)
            {
                pp.SetC_UOM_ID(C_UOM_ID);
            }

            //Get product stock
            MProduct product = MProduct.Get(ctx, M_Product_ID);


            ProductDataOut objInfo = new ProductDataOut
            {


                PriceList = pp.GetPriceList(),
                PriceLimit = pp.GetPriceLimit(),
                PriceActual = pp.GetPriceStd(),
                PriceEntered = pp.GetPriceStd(),
                PriceStd = pp.GetPriceStd(),
                LineAmt = pp.GetLineAmt(2),
                C_Currency_ID = System.Convert.ToInt32(pp.GetC_Currency_ID()),
                Discount = pp.GetDiscount(),
                C_UOM_ID = System.Convert.ToInt32(pp.GetC_UOM_ID()),
                //QtyOrdered= mTab.GetValue("QtyEntered"));
                EnforcePriceLimit = pp.IsEnforcePriceLimit(),
                DiscountSchema = pp.IsDiscountSchema(),
                IsStocked = product.IsStocked()

            };
            product = null;
            pp = null;
            return objInfo;
        }


        /// <summary>
        /// Get StdPrice from Product Price
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="fields">Fields</param>
        /// <returns>StdPrice</returns>
        public Decimal GetProductdata(Ctx ctx, string fields)
        {
            int UOM=0, M_Product_ID = 0, Attribute=0, M_PriceList_ID=0;
            string[] paramValue = fields.Split(',');
            M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            Attribute = Util.GetValueOfInt(paramValue[1].ToString());
            M_PriceList_ID = Util.GetValueOfInt(paramValue[2].ToString());
            UOM = Util.GetValueOfInt(paramValue[3].ToString());
           
            string sql = "SELECT PriceStd FROM M_ProductPrice WHERE M_PriceList_Version_ID = (SELECT MAX(M_PriceList_Version_ID) FROM M_PriceList_Version WHERE IsActive = 'Y'" +
                "AND M_PriceList_ID ="+M_PriceList_ID +") AND M_Product_id=" + M_Product_ID;

            if (Attribute > 0)
            {
                sql += " AND M_AttributeSetInstance_ID=" + Attribute;
            }
            if (UOM > 0)
            {
                sql += " AND C_UOM_ID=" + UOM;
            }

            return Util.GetValueOfDecimal(DB.ExecuteScalar(sql, null, null));
        }
    }
}