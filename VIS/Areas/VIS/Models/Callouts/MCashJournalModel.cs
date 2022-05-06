using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Utility;

namespace VIS.Models
{
    public class MCashJournalModel
    {
        //Arpit
        private static VLogger log = VLogger.GetVLogger(typeof(MCashJournalModel).FullName);

        //To Get Business Partner Location on Cash Journal Line
        public Dictionary<string, object> GetLocationData(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_BPartner_ID = Util.GetValueOfInt(paramValue[0]);
            int Client_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT BP.CreditStatusSettingOn, " +
                         "(SELECT MIN(L.C_BPartner_Location_ID) FROM C_BPartner_Location L WHERE IsActive='Y' AND L.C_BPartner_ID="
                          + C_BPartner_ID + " AND L.AD_Client_ID=" + Client_ID + ")AS C_BPartner_Location_ID" +
                            " FROM C_BPartner BP Where BP.C_BPartner_ID=" + C_BPartner_ID +
                            " AND AD_Client_ID=" + Client_ID;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, null);
                if (idr != null)
                {
                    if (idr.Read())
                    {
                        retDic = new Dictionary<string, object>();
                        retDic["CreditStatusSettingOn"] = Util.GetValueOfString(idr[0]);
                        retDic["C_BPartner_Location_ID"] = Util.GetValueOfInt(idr[1]);
                    }
                }

            }
            catch (Exception ex)
            {
                if (idr != null)
                {
                    idr.Close();
                    idr = null;
                }
                log.Log(Level.SEVERE, sql, ex);
                log.Severe("Error while getting location for Business Partner" + ex.Message);
            }
            if (idr != null)
            {
                idr.Close();
                idr = null;
            }
            return retDic;
        }

        ///  <summary>
        /// Get account no and routing no against selected bank account
        /// </summary>        
        /// <param name="ctx"> Context Object </param>
        /// <param name="C_BankAccount_ID"> ID of bank account </param>
        /// <returns>Account number and Routing Number</returns> //Added by manjot on 22/02/2019 
        public Dictionary<string, object> GetAccountData(Ctx ctx, int C_BankAccount_ID)
        {
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            DataSet _ds = DB.ExecuteDataset(@" SELECT ac.AccountNo, b.RoutingNo FROM C_BankAccount ac INNER JOIN C_Bank b ON b.C_Bank_ID= ac.C_Bank_ID WHERE ac.C_BankAccount_ID = " + C_BankAccount_ID);
            if (_ds != null && _ds.Tables[0].Rows.Count > 0)
            {
                retValue["AccountNo"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["AccountNo"]);
                retValue["RoutingNo"] = Util.GetValueOfString(_ds.Tables[0].Rows[0]["RoutingNo"]);
            }
            return retValue;
        }

        /// <summary>
        /// To get beginning balance
        /// </summary>
        /// <param name="ctx"> Context Object </param>
        /// <param name="fields">Ids of Org, Client and Cashbook</param>
        /// <returns>beginning balance</returns>
        public decimal GetBeginningBalCalc(Ctx ctx, string fields)
        {
            string[] paramValue = fields.Split(',');
            int C_CashBook_ID = Util.GetValueOfInt(paramValue[0]);
            int AD_Client_ID = Util.GetValueOfInt(paramValue[1]);
            int AD_Org_ID = Util.GetValueOfInt(paramValue[2]);
            string sql = "SELECT EndingBalance FROM C_Cash WHERE C_CashBook_ID=" + C_CashBook_ID + " AND" +
             " AD_Client_ID=" + AD_Client_ID + " AND AD_Org_ID=" + AD_Org_ID + " AND " +
             "c_cash_id IN (SELECT Max(c_cash_id) FROM C_Cash WHERE C_CashBook_ID=" + C_CashBook_ID
             + "AND AD_Client_ID=" + AD_Client_ID + " AND AD_Org_ID=" + AD_Org_ID + ") AND Processed='Y'";

            return Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
        }

        /// <summary>
        /// This function is used to get the Amoun and Converted Amount When cash Type is "Cash Received From"
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="fields">Ids</param>
        /// <returns>Dictionary Object</returns>
        /// <writer>VIS_0045</writer>
        public Dictionary<string, object> ConvertedAmt(Ctx ctx, string fields)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            string[] paramValue = fields.Split(',');
            int C_CashLine_ID = Util.GetValueOfInt(paramValue[0]);
            int C_Cash_ID = Util.GetValueOfInt(paramValue[1]);
            int AD_Org_ID = Util.GetValueOfInt(paramValue[2]);
            int C_ConversionType_ID = Util.GetValueOfInt(paramValue[3]) <= 0 ? 0 : Util.GetValueOfInt(paramValue[3]);

            String sql = @"SELECT C_Currency_ID FROM C_CashBook WHERE C_CashBook_ID=
                            (SELECT C_CashBook_ID FROM C_Cash WHERE C_Cash_ID=" + C_Cash_ID + ")";
            result["currTo"] = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            // When CashBook Currency not found
            if (Util.GetValueOfInt(result["currTo"]) <= 0)
            {
                return result;
            }

            // Get from Currency of selected cash journal line
            sql = "SELECT C_Currency_ID FROM C_Cashbook WHERE C_Cashbook_ID= (SELECT C_CashBook_ID FROM C_Cash WHERE C_Cash_ID="
                + " (SELECT C_Cash_ID FROM C_CashLine WHERE C_CashLine_ID= " + C_CashLine_ID + "))";
            result["CurrFrom"] = Util.GetValueOfInt(DB.ExecuteScalar(sql));

            // Get Amount From Cash Line
            sql = @"SELECT CASE WHEN CashType = 'A' THEN " + DBFunctionCollection.TypecastColumnAsDecimal("cashline.convertedamt")
                    + @" ELSE CASHLINE.Amount END AS Amount FROM C_CashLine CASHLINE
                    INNER JOIN C_Cashbook CB ON (CB.C_Cashbook_ID=CASHLINE.C_Cashbook_ID)
                    WHERE CASHLINE.C_CashLine_ID =" + C_CashLine_ID;
            result["amt"] = Util.GetValueOfDecimal(DB.ExecuteScalar(sql));

            // get Converted Amount
            result["transferdAmt"] = MConversionRate.Convert(ctx, Util.GetValueOfDecimal(result["amt"]), Util.GetValueOfInt(result["CurrFrom"]),
                Util.GetValueOfInt(result["currTo"]), null, C_ConversionType_ID, ctx.GetAD_Client_ID(), AD_Org_ID);

            return result;
        }

    }

}