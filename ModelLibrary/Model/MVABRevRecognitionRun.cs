using System;
using System.Collections.Generic;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MVABRevRecognitionRun : X_VAB_Rev_RecognitionRun
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABRevRecognitionRun).FullName);
        public MVABRevRecognitionRun(Ctx ctx, int VAB_Rev_RecognitionRun_ID, Trx trxName)
            : base(ctx, VAB_Rev_RecognitionRun_ID, trxName)
        {

        }
        public MVABRevRecognitionRun(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        /// This function is used to get data of MRevenueRecognitionRun
        /// </summary>
        /// <param name="revenueRecognition">Revenue Recognition reference</param>
        /// <param name="recognitionDate">Recognition Date</param>
        /// <param name="_orgId">Org ID</param>
        /// <param name="reverse">is reverse or not</param>
        /// <returns>array of MRevenueRecognitionRun</returns>
        public static MVABRevRecognitionRun[] GetRecognitionRuns(MVABRevRecognition revenueRecognition, DateTime? recognitionDate, int _orgId, bool reverse)
        {
            List<MVABRevRecognitionRun> list = new List<MVABRevRecognitionRun>();
            string sql = "Select  rn.* from VAB_Rev_RecognitionRun rn " +
                        " JOIN VAB_Rev_RecognitionStrtgy pl ON pl.VAB_Rev_RecognitionStrtgy_id = rn.VAB_Rev_RecognitionStrtgy_id " +
                        " WHERE pl.VAB_Rev_Recognition_id =" + revenueRecognition.GetVAB_Rev_Recognition_ID();
            if (!reverse)
            {
                sql += " And RecognitionDate <=" + GlobalVariable.TO_DATE(recognitionDate, true);
            }
            if (_orgId > 0)
            {
                sql += " AND pl.VAF_Org_ID=" + _orgId;
            }
            sql += " And NVL(VAGL_JRNL_ID,0) <= 0 ORDER BY rn.RecognitionDate, pl.VAB_Currency_id, pl.VAB_AccountBook_id";

            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DB.ExecuteReader(sql, null, revenueRecognition.Get_Trx());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVABRevRecognitionRun(revenueRecognition.GetCtx(), dr, revenueRecognition.Get_Trx()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            MVABRevRecognitionRun[] retValue = new MVABRevRecognitionRun[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// This function is used to set the values
        /// </summary>
        /// <param name="revenueRecognitionPlan">Revenue Recognnition plan ID</param>
        public void SetRecognitionRun(MVABRevRecognitionStrtgy revenueRecognitionPlan)
        {
            SetVAF_Client_ID(revenueRecognitionPlan.GetVAF_Client_ID());
            SetVAF_Org_ID(revenueRecognitionPlan.GetVAF_Org_ID());
            SetVAB_Rev_RecognitionStrtgy_ID(revenueRecognitionPlan.GetVAB_Rev_RecognitionStrtgy_ID());
        }
    }
}
