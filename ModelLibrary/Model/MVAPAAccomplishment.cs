/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAPA_Accomplishment
 * Chronological Development
 * Veena Pandey     19-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVAPAAccomplishment : X_VAPA_Accomplishment
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MVAPAAccomplishment).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_Accomplishment_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPAAccomplishment(Ctx ctx, int VAPA_Accomplishment_ID, Trx trxName)
            : base(ctx, VAPA_Accomplishment_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVAPAAccomplishment(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get achieved Achievements Of Measure
        /// </summary>
        /// <param name="measure">Measure</param>
        /// <returns>array of Achievements</returns>
        public static MVAPAAccomplishment[] Get(MVAPAEvaluate measure)
        {
            return GetOfMeasure(measure.GetCtx(), measure.GetVAPA_Evaluate_ID());
        }

        /// <summary>
        /// Get achieved Achievements Of Measure
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_Evaluate_ID">measure id</param>
        /// <returns>array of Achievements</returns>
        public static MVAPAAccomplishment[] GetOfMeasure(Ctx ctx, int VAPA_Evaluate_ID)
        {
            List<MVAPAAccomplishment> list = new List<MVAPAAccomplishment>();
            String sql = "SELECT * FROM VAPA_Accomplishment "
                + "WHERE VAPA_Evaluate_ID=" + VAPA_Evaluate_ID + " AND IsAchieved='Y' ORDER BY SeqNo, DateDoc";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MVAPAAccomplishment(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //
            MVAPAAccomplishment[] retValue = new MVAPAAccomplishment[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /// <summary>
        /// After Delete
        /// </summary>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterDelete(bool success)
        {
            if (success)
                UpdateAchievementGoals();
            return success;
        }

        /// <summary>
        /// After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (success)
                UpdateAchievementGoals();
            return success;
        }

        /// <summary>
        /// Before Save.
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <returns>true</returns>
        protected override bool BeforeSave(bool newRecord)
        {
            if (IsAchieved())
            {
                if (Env.Signum(GetManualActual()) == 0)
                    SetManualActual(Env.ONE);
                if (GetDateDoc() == null)
                    SetDateDoc(DateTime.Now);
            }
            return true;
        }

        /// <summary>
        /// Update Goals with Achievement
        /// </summary>
        private void UpdateAchievementGoals()
        {
            MVAPAEvaluate measure = MVAPAEvaluate.Get(GetCtx(), GetVAPA_Evaluate_ID());
            measure.UpdateGoals();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVAPAAccomplishment[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}