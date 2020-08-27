/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MNonBusinessDay
 * Purpose        : To Ristict transaction for non working day
 * Class Used     : X_C_NonBusinessDay
 * Chronological    Development
 * Raghunandan      16-Jun-2015
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.Classes;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Process;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MNonBusinessDay : X_C_NonBusinessDay
    {
        //	Logger
        private static VLogger _log = VLogger.GetVLogger(typeof(MNonBusinessDay).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="C_NonBusinessDay_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MNonBusinessDay(Ctx ctx, int C_NonBusinessDay_ID, Trx trxName)
            : base(ctx, C_NonBusinessDay_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MNonBusinessDay(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Check trascation record for non business day
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_Calendar_ID"></param>
        /// <param name="dt"></param>
        /// <returns>true for nonbusinessday</returns>
        public static bool IsNonBusinessDay(Ctx ctx, DateTime? dt)
        {
            return IsNonBusinessDay(ctx, dt, 0);
        }

        /// <summary>
        /// Check trascation record for non business day organization wise
        /// </summary>
        /// <param name="ctx">Context</param>
        /// <param name="dt">date</param>
        /// <param name="AD_Org_ID">optional organization</param>
        /// <returns>true for nonbusinessday</returns>
        public static bool IsNonBusinessDay(Ctx ctx, DateTime? dt, int AD_Org_ID = 0)
        {
            bool nbDay = false;
            int C_Period_ID = MPeriod.GetC_Period_ID(ctx, dt);
            string sql = "SELECT C_CALENDAR_ID FROM C_YEAR WHERE C_YEAR_ID=(SELECT C_YEAR_ID FROM C_PERIOD  WHERE C_PERIOD_ID=" + C_Period_ID + ")";
            int C_Calendar_ID = Convert.ToInt32(DataBase.DB.ExecuteScalar(sql, null, null));

            sql = MRole.GetDefault(ctx, false).AddAccessSQL(
               "SELECT count(*) FROM C_NONBUSINESSDAY WHERE ISACTIVE = 'Y' AND C_CALENDAR_ID=" + C_Calendar_ID
               + (AD_Org_ID > 0 ? " AND AD_Org_ID IN (0, " + AD_Org_ID + ")" : "") + " AND DATE1=TO_DATE('" + dt.Value.ToShortDateString() + "', 'MM-DD-YY')",
               "C_NonBusinessDay", false, false);   // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
            try
            {
                int count = Convert.ToInt32(DataBase.DB.ExecuteScalar(sql, null, null));
                if (count > 0)
                {
                    nbDay = true;
                }
            }
            catch (Exception e)
            {

            }
            return nbDay;
        }
    }
}
