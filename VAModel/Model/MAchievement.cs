/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_PA_Achievement
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
    public class MAchievement : X_PA_Achievement
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MAchievement).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_Achievement_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAchievement(Ctx ctx, int PA_Achievement_ID, Trx trxName)
            : base(ctx, PA_Achievement_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MAchievement(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get achieved Achievements Of Measure
        /// </summary>
        /// <param name="measure">Measure</param>
        /// <returns>array of Achievements</returns>
        public static MAchievement[] Get(MMeasure measure)
        {
            return GetOfMeasure(measure.GetCtx(), measure.GetPA_Measure_ID());
        }

        /// <summary>
        /// Get achieved Achievements Of Measure
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_Measure_ID">measure id</param>
        /// <returns>array of Achievements</returns>
        public static MAchievement[] GetOfMeasure(Ctx ctx, int PA_Measure_ID)
        {
            List<MAchievement> list = new List<MAchievement>();
            String sql = "SELECT * FROM PA_Achievement "
                + "WHERE PA_Measure_ID=" + PA_Measure_ID + " AND IsAchieved='Y' ORDER BY SeqNo, DateDoc";
            try
            {
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MAchievement(ctx, dr, null));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            //
            MAchievement[] retValue = new MAchievement[list.Count];
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
            MMeasure measure = MMeasure.Get(GetCtx(), GetPA_Measure_ID());
            measure.UpdateGoals();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MAchievement[");
            sb.Append(Get_ID()).Append("-").Append(GetName()).Append("]");
            return sb.ToString();
        }
    }
}