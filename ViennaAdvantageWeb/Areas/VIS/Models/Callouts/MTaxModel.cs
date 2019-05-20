using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MTaxModel
    {

        /// <summary>
        /// Calculate Tax
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal? CalculateTax(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Tax_ID;
            //ecimal Qty;
            //bool isSOTrx;
            List<Decimal?> retval = new List<Decimal?>();
            //Assign parameter value
            C_Tax_ID = Util.GetValueOfInt(paramValue[0].ToString());
            Decimal LineNetAmt = Util.GetValueOfDecimal(paramValue[1].ToString());
            Boolean IsTaxIncluded = Convert.ToBoolean(paramValue[2]);
            int StdPrecision = Util.GetValueOfInt(paramValue[3].ToString());
            //End Assign parameter value
            MTax tax = new MTax(ctx, C_Tax_ID, null);
            Decimal? TaxAmt = tax.CalculateTax(LineNetAmt, IsTaxIncluded, StdPrecision);
            return TaxAmt;

        }
        /// <summary>
        /// Get tax Id
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public int Get(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Charge_ID;
            DateTime? billDate;
            DateTime? shipDate;
            int AD_Org_ID;
            int M_Warehouse_ID;
            int C_BPartner_Location_ID;
            bool isSOTrx;

            //Assign parameter value
            C_Charge_ID = Util.GetValueOfInt(paramValue[0].ToString());
            billDate = Util.GetValueOfDateTime(paramValue[1]);
            shipDate = Util.GetValueOfDateTime(paramValue[2]);
            AD_Org_ID = Util.GetValueOfInt(paramValue[3].ToString());
            M_Warehouse_ID = Util.GetValueOfInt(paramValue[4].ToString());
            C_BPartner_Location_ID = Util.GetValueOfInt(paramValue[5].ToString());
            isSOTrx = Convert.ToBoolean(paramValue[6]);
            //End Assign parameter value

            int Tax_ID = Tax.Get(ctx, 0, C_Charge_ID, billDate, shipDate,
                                AD_Org_ID, M_Warehouse_ID, C_BPartner_Location_ID, C_BPartner_Location_ID,
                                isSOTrx);

            return Tax_ID;

        }
        public int Get_Tax_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //int C_Tax_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int M_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int C_Charge_ID = Util.GetValueOfInt(paramValue[1].ToString());
            DateTime? billDate = Util.GetValueOfDateTime(paramValue[2].ToString());
            DateTime? shipDate = Util.GetValueOfDateTime(paramValue[3].ToString());
            int AD_Org_ID = Util.GetValueOfInt(paramValue[4].ToString());
            int M_Warehouse_ID = Util.GetValueOfInt(paramValue[5].ToString());
            int billC_BPartner_Location_ID = Util.GetValueOfInt(paramValue[6].ToString());
            int shipC_BPartner_Location_ID = Util.GetValueOfInt(paramValue[7].ToString());
            bool isSOTrx = Util.GetValueOfBool(paramValue[8]);
            int C_Tax_ID = VAdvantage.Model.Tax.Get(ctx, M_Product_ID, C_Charge_ID, billDate, shipDate,
               AD_Org_ID, M_Warehouse_ID, billC_BPartner_Location_ID, shipC_BPartner_Location_ID,
               isSOTrx);
            return C_Tax_ID;
        }

        //Added By Bharat on 12/May/2017
        public Decimal GetTaxRate(Ctx ctx, string fields)
        {
            int C_Tax_ID = Util.GetValueOfInt(fields);
            MTax tax = new MTax(ctx, C_Tax_ID, null);
            return tax.GetRate();
        }
    }
}