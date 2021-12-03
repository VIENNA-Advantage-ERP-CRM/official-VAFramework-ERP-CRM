using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
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

        /// <summary>
        /// Calculate Surcharge Tax
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, Object> CalculateSurcharge(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_Tax_ID;

            Dictionary<String, Object> retval = new Dictionary<String, Object>();
            //Assign parameter value
            C_Tax_ID = Util.GetValueOfInt(paramValue[0]);
            Decimal LineNetAmt = Util.GetValueOfDecimal(paramValue[1]);            
            int StdPrecision = Util.GetValueOfInt(paramValue[2]);
            Boolean IsTaxIncluded = true;

            if (paramValue.Length == 4)
            {
                IsTaxIncluded = Util.GetValueOfBool(paramValue[3]);
            }
            //End Assign parameter value
            MTax tax = new MTax(ctx, C_Tax_ID, null);
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

        /// <summary>
        /// Get TaxExempt details from Tax
        /// </summary>
        /// <param name="Tax_ID">Tax</param>
        /// <writer>1052</writer>
        /// <returns>TaxExempt details</returns>
        public Dictionary<String, Object> GetTaxExempt(int Tax_ID)
        {
            Dictionary<String, Object> retval = new Dictionary<String, Object>();
            string sql = "SELECT IsTaxExempt, C_TaxExemptReason_ID FROM C_Tax WHERE IsActive='Y' AND C_Tax_ID= "+Tax_ID;
            DataSet ds = DB.ExecuteDataset(sql);
            if (ds != null && ds.Tables[0].Rows.Count>0)
            {
                retval["IsTaxExempt"] = Util.GetValueOfString(ds.Tables[0].Rows[0]["IsTaxExempt"]);
                retval["C_TaxExemptReason_ID"] = Util.GetValueOfInt(ds.Tables[0].Rows[0]["C_TaxExemptReason_ID"]); 
            }
            return retval;
        }

    }
}