using System;
using System.Collections.Generic;
using System.Data;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MRevenueRecognitionRun : X_C_RevenueRecognition_Run
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(MRevenueRecognitionRun).FullName);
        public MRevenueRecognitionRun(Ctx ctx, int C_RevenueRecognition_Run_ID, Trx trxName)
            : base(ctx, C_RevenueRecognition_Run_ID, trxName)
        {

        }
        public MRevenueRecognitionRun(Ctx ctx, DataRow rs, Trx trxName)
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
        public static MRevenueRecognitionRun[] GetRecognitionRuns(MRevenueRecognition revenueRecognition, DateTime? recognitionDate, int _orgId, bool reverse)
        {
            List<MRevenueRecognitionRun> list = new List<MRevenueRecognitionRun>();
            string sql = "Select  rn.* from C_RevenueRecognition_Run rn " +
                        " JOIN c_revenuerecognition_plan pl ON pl.c_revenuerecognition_plan_id = rn.c_revenuerecognition_plan_id " +
                        " WHERE pl.c_revenuerecognition_id =" + revenueRecognition.GetC_RevenueRecognition_ID();
            if (!reverse)
            {
                sql += " And RecognitionDate <=" + GlobalVariable.TO_DATE(recognitionDate, true);
            }
            if (_orgId > 0)
            {
                sql += " AND pl.AD_Org_ID=" + _orgId;
            }
            sql += " And NVL(GL_Journal_ID,0) <= 0 ORDER BY rn.RecognitionDate, pl.c_currency_id, pl.c_acctschema_id";

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
                    list.Add(new MRevenueRecognitionRun(revenueRecognition.GetCtx(), dr, revenueRecognition.Get_Trx()));
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

            MRevenueRecognitionRun[] retValue = new MRevenueRecognitionRun[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// This function is used to set the values
        /// </summary>
        /// <param name="revenueRecognitionPlan">Revenue Recognnition plan ID</param>
        public void SetRecognitionRun(MRevenueRecognitionPlan revenueRecognitionPlan)
        {
            SetAD_Client_ID(revenueRecognitionPlan.GetAD_Client_ID());
            SetAD_Org_ID(revenueRecognitionPlan.GetAD_Org_ID());
            SetC_RevenueRecognition_Plan_ID(revenueRecognitionPlan.GetC_RevenueRecognition_Plan_ID());
        }
    }
}
