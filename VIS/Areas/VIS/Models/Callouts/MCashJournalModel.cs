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

    }

}