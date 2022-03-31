using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MConversionRateModel
    {
        /// <summary>
        /// Get Rate
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetRate(Ctx ctx,string fields)
        {
           
                string[] paramValue = fields.Split(',');
                int CurFrom_ID;
                int CurTo_ID;
                DateTime? convDate;
                int ConversionType_ID;
                int AD_Client_ID;
                int AD_Org_ID;

                //Assign parameter value
                CurFrom_ID = Util.GetValueOfInt(paramValue[0].ToString());
                CurTo_ID = Util.GetValueOfInt(paramValue[1].ToString());
            
                // 18/7/2016 create only try catch block. if problem the get current date time
                try {
                    convDate = System.Convert.ToDateTime(paramValue[2].ToString());
                }
                catch {
                    convDate = DateTime.Now;
                }
                //end 18/7/2016
                

                ConversionType_ID = Util.GetValueOfInt(paramValue[3].ToString());
                AD_Client_ID = Util.GetValueOfInt(paramValue[4].ToString());
                AD_Org_ID = Util.GetValueOfInt(paramValue[5].ToString());
               //End Assign parameter value

                Decimal rate = MConversionRate.GetRate(CurFrom_ID, CurTo_ID, convDate, ConversionType_ID, AD_Client_ID, AD_Org_ID);
                return rate;
                
        }
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal Convert(Ctx ctx, string fields)
        {
                string[] paramValue = fields.Split(',');
                int CurFrom_ID;
                int CurTo_ID;
                //DateTime? convDate;
                //int ConversionType_ID;
                int AD_Client_ID;
                int AD_Org_ID;

                //Assign parameter value
                Decimal amt = Util.GetValueOfDecimal(paramValue[0].ToString());
                CurFrom_ID = Util.GetValueOfInt(paramValue[1].ToString());
                CurTo_ID = Util.GetValueOfInt(paramValue[2].ToString());
                //CurTo_ID = Util.GetValueOfInt(paramValue[1].ToString());
                //CurFrom_ID = Util.GetValueOfInt(paramValue[2].ToString());
                AD_Client_ID = Util.GetValueOfInt(paramValue[3].ToString());
                AD_Org_ID = Util.GetValueOfInt(paramValue[4].ToString());
            
                Decimal convert = MConversionRate.Convert(ctx, amt, CurFrom_ID, CurTo_ID, AD_Client_ID, AD_Org_ID);
                return convert;
        }

        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal CurrencyConvert(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int CurFrom_ID;
            int CurTo_ID;
            DateTime? convDate;
            int ConversionType_ID;
            int AD_Client_ID;
            int AD_Org_ID;

            //Assign parameter value
            Decimal amt = Util.GetValueOfDecimal(paramValue[0].ToString());
            CurFrom_ID = Util.GetValueOfInt(paramValue[1].ToString());
            CurTo_ID = Util.GetValueOfInt(paramValue[2].ToString());            
            try
            {
                convDate = System.Convert.ToDateTime(paramValue[3].ToString());
            }
            catch
            {
                convDate = DateTime.Now;
            }
            ConversionType_ID = Util.GetValueOfInt(paramValue[4].ToString());
            AD_Client_ID = Util.GetValueOfInt(paramValue[5].ToString());
            AD_Org_ID = Util.GetValueOfInt(paramValue[6].ToString());

            Decimal convert = MConversionRate.Convert(ctx, amt, CurFrom_ID, CurTo_ID, convDate,ConversionType_ID, AD_Client_ID, AD_Org_ID);
            return convert;
        }
    }
}