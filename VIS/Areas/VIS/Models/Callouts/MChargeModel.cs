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
    public class MChargeModel
    {
        /// <summary>
        ///Get charge Amount
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Decimal GetCharge(Ctx ctx, string fields)
        {              
            string[] paramValue = fields.Split(',');
            int Cid;
            //Assign parameter value
            Cid = Util.GetValueOfInt(paramValue[0].ToString());
            //End Assign parameter
            X_C_Charge charge = new X_C_Charge(ctx, Cid, null);
            return charge.GetChargeAmt();                  
        }

        /// <summary>
        ///Get charge Details
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Dictionary<String, Object> GetChargeDetails(Ctx ctx, string fields)
        {
            Dictionary<String, Object> chargeDic = new Dictionary<string, Object>();
            string[] paramValue = fields.Split(',');
            DataSet dsChargeInfo = null;
            int cId = Util.GetValueOfInt(paramValue[0].ToString());         

            string sql = @"SELECT PrintDescription, ChargeAmt FROM C_Charge WHERE IsActive = 'Y' AND C_Charge_ID = " + cId;

            dsChargeInfo = DB.ExecuteDataset(sql);
            if (dsChargeInfo != null && dsChargeInfo.Tables[0].Rows.Count > 0)
            {
                chargeDic["PrintDescription"] = Util.GetValueOfString(dsChargeInfo.Tables[0].Rows[0]["PrintDescription"]);
                chargeDic["ChargeAmt"] = Util.GetValueOfDecimal(dsChargeInfo.Tables[0].Rows[0]["ChargeAmt"]);
            }
            return chargeDic;
        }
    }
}