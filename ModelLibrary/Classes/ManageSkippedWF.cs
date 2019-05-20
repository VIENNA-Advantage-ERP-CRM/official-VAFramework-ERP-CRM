using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;
using VAdvantage.DataBase;


namespace VAdvantage.Classes
{
    public class ManageSkippedWF
    {
        public static Dictionary<string, List<PO>> trxList = new Dictionary<string, List<PO>>();
        private static VLogger log = VLogger.GetVLogger(typeof(ManageSkippedWF).FullName);
        public static bool Add(string trxName, PO po)
        {
            try
            {
                if (trxList.ContainsKey(trxName))
                {
                    trxList[trxName].Add(po);
                }
                else
                {
                    trxList.Add(trxName, new List<PO> { po });
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Severe("Not Added To TRXList: " + ex.Message +": "+po);
                return false;
            }
                
        }

        public static bool Remove(String trxName)
        {
            try
            {
                if (trxList.ContainsKey(trxName))
                {
                    trxList.Remove(trxName);
                }
                return true;
            }
            catch (Exception ex)
            {
                log.Severe("Not Removed From TRXList: " + ex.Message );
                return false;
            }
        }
        public static bool Execute(string trxName)
        {
            try
            {
                if (trxList.ContainsKey(trxName))
                {                    
                    PO document = null;                    
                    for (int i = 0; i < trxList[trxName].Count; i++)
                    {
                        document = trxList[trxName][i];
                        document.Set_Trx(null);
                        document.ExecuteWF();                       
                    }
                }
               return  Remove(trxName);
            }
            catch (Exception ex)
            {
                log.Severe("Error While Starting Workflow: " + ex.Message);
                return false;
            }
        }
    }
}
