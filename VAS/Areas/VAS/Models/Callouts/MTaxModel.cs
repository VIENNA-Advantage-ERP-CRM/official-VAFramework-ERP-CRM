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
            int VAB_TaxRate_ID;
            //ecimal Qty;
            //bool isSOTrx;
            List<Decimal?> retval = new List<Decimal?>();
            //Assign parameter value
            VAB_TaxRate_ID = Util.GetValueOfInt(paramValue[0].ToString());
            Decimal LineNetAmt = Util.GetValueOfDecimal(paramValue[1].ToString());
            Boolean IsTaxIncluded = Convert.ToBoolean(paramValue[2]);
            int StdPrecision = Util.GetValueOfInt(paramValue[3].ToString());
            //End Assign parameter value
            MVABTaxRate tax = new MVABTaxRate(ctx, VAB_TaxRate_ID, null);
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
            int VAB_Charge_ID;
            DateTime? billDate;
            DateTime? shipDate;
            int VAF_Org_ID;
            int VAM_Warehouse_ID;
            int VAB_BPart_Location_ID;
            bool isSOTrx;

            //Assign parameter value
            VAB_Charge_ID = Util.GetValueOfInt(paramValue[0].ToString());
            billDate = Util.GetValueOfDateTime(paramValue[1]);
            shipDate = Util.GetValueOfDateTime(paramValue[2]);
            VAF_Org_ID = Util.GetValueOfInt(paramValue[3].ToString());
            VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[4].ToString());
            VAB_BPart_Location_ID = Util.GetValueOfInt(paramValue[5].ToString());
            isSOTrx = Convert.ToBoolean(paramValue[6]);
            //End Assign parameter value

            int Tax_ID = Tax.Get(ctx, 0, VAB_Charge_ID, billDate, shipDate,
                                VAF_Org_ID, VAM_Warehouse_ID, VAB_BPart_Location_ID, VAB_BPart_Location_ID,
                                isSOTrx);

            return Tax_ID;

        }
        public int Get_Tax_ID(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            //int VAB_TaxRate_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAM_Product_ID = Util.GetValueOfInt(paramValue[0].ToString());
            int VAB_Charge_ID = Util.GetValueOfInt(paramValue[1].ToString());
            DateTime? billDate = Util.GetValueOfDateTime(paramValue[2].ToString());
            DateTime? shipDate = Util.GetValueOfDateTime(paramValue[3].ToString());
            int VAF_Org_ID = Util.GetValueOfInt(paramValue[4].ToString());
            int VAM_Warehouse_ID = Util.GetValueOfInt(paramValue[5].ToString());
            int billVAB_BPart_Location_ID = Util.GetValueOfInt(paramValue[6].ToString());
            int shipVAB_BPart_Location_ID = Util.GetValueOfInt(paramValue[7].ToString());
            bool isSOTrx = Util.GetValueOfBool(paramValue[8]);
            int VAB_TaxRate_ID = VAdvantage.Model.Tax.Get(ctx, VAM_Product_ID, VAB_Charge_ID, billDate, shipDate,
               VAF_Org_ID, VAM_Warehouse_ID, billVAB_BPart_Location_ID, shipVAB_BPart_Location_ID,
               isSOTrx);
            return VAB_TaxRate_ID;
        }

        //Added By Bharat on 12/May/2017
        public Decimal GetTaxRate(Ctx ctx, string fields)
        {
            int VAB_TaxRate_ID = Util.GetValueOfInt(fields);
            MVABTaxRate tax = new MVABTaxRate(ctx, VAB_TaxRate_ID, null);
            return tax.GetRate();
        }

        /// <summary>
        /// Calculate Surcharge Tax
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, Object> CalculateSurcharge(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int VAB_TaxRate_ID;

            Dictionary<String, Object> retval = new Dictionary<String, Object>();
            //Assign parameter value
            VAB_TaxRate_ID = Util.GetValueOfInt(paramValue[0]);
            Decimal LineNetAmt = Util.GetValueOfDecimal(paramValue[1]);            
            int StdPrecision = Util.GetValueOfInt(paramValue[2]);
            Boolean IsTaxIncluded = true;

            if (paramValue.Length == 4)
            {
                IsTaxIncluded = Util.GetValueOfBool(paramValue[3]);
            }
            //End Assign parameter value
            MVABTaxRate tax = new MVABTaxRate(ctx, VAB_TaxRate_ID, null);
            Decimal surchargeAmt = Env.ZERO;
            Decimal TaxAmt = Env.ZERO;
            if (tax.Get_ColumnIndex("Surcharge_Tax_ID") > 0 && tax.GetSurcharge_Tax_ID() > 0)
            {
                TaxAmt = tax.CalculateSurcharge(LineNetAmt, IsTaxIncluded, StdPrecision, out surchargeAmt);
            }
            else
            {
                TaxAmt = tax.CalculateTax(LineNetAmt, IsTaxIncluded, StdPrecision);
            }
            retval["TaxAmt"] = TaxAmt;
            retval["SurchargeAmt"] = surchargeAmt;
            return retval;
        }
    }
}