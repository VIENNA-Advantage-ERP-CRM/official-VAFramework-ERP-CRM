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
            int VAB_BusinessPartner_ID = Util.GetValueOfInt(paramValue[0]);
            int Client_ID = Util.GetValueOfInt(paramValue[1]);
            Dictionary<string, object> retDic = null;
            string sql = "SELECT BP.CreditStatusSettingOn, " +
                         "(SELECT MIN(L.VAB_BPart_Location_ID) FROM VAB_BPart_Location L WHERE IsActive='Y' AND L.VAB_BusinessPartner_ID="
                          + VAB_BusinessPartner_ID + " AND L.VAF_Client_ID=" + Client_ID + ")AS VAB_BPart_Location_ID" +
                            " FROM VAB_BusinessPartner BP Where BP.VAB_BusinessPartner_ID=" + VAB_BusinessPartner_ID +
                            " AND VAF_Client_ID=" + Client_ID;
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
                        retDic["VAB_BPart_Location_ID"] = Util.GetValueOfInt(idr[1]);
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
        /// <param name="VAB_Bank_Acct_ID"> ID of bank account </param>
        /// <returns>Account number and Routing Number</returns> //Added by manjot on 22/02/2019 
        public Dictionary<string, object> GetAccountData(Ctx ctx, int VAB_Bank_Acct_ID)
        {
            Dictionary<string, object> retValue = new Dictionary<string, object>();
            DataSet _ds = DB.ExecuteDataset(@" SELECT ac.AccountNo, b.RoutingNo FROM VAB_Bank_Acct ac INNER JOIN VAB_Bank b ON b.VAB_Bank_ID= ac.VAB_Bank_ID WHERE ac.VAB_Bank_Acct_ID = " + VAB_Bank_Acct_ID);
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
            int VAB_CashBook_ID = Util.GetValueOfInt(paramValue[0]);
            int VAF_Client_ID = Util.GetValueOfInt(paramValue[1]);
            int VAF_Org_ID = Util.GetValueOfInt(paramValue[2]);
            string sql = "SELECT EndingBalance FROM VAB_CashJRNL WHERE VAB_CashBook_ID=" + VAB_CashBook_ID + " AND" +
             " VAF_Client_ID=" + VAF_Client_ID + " AND VAF_Org_ID=" + VAF_Org_ID + " AND " +
             "VAB_CashBook_id IN (SELECT Max(VAB_CashBook_id) FROM VAB_CashJRNL WHERE VAB_CashBook_ID=" + VAB_CashBook_ID
             + "AND VAF_Client_ID=" + VAF_Client_ID + " AND VAF_Org_ID=" + VAF_Org_ID + ") AND Processed='Y'";

            return Util.GetValueOfDecimal(DB.ExecuteScalar(sql));
        }

    }

}