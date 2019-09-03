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

    }

}