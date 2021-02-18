﻿/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MNonBusinessDay
 * Purpose        : To Ristict transaction for non working day
 * Class Used     : X_VAB_NonBusinessDay
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
    public class MVABNonBusinessDay : X_VAB_NonBusinessDay
    {
        //	Logger
        private static VLogger _log = VLogger.GetVLogger(typeof(MVABNonBusinessDay).FullName);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_NonBusinessDay_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVABNonBusinessDay(Ctx ctx, int VAB_NonBusinessDay_ID, Trx trxName)
            : base(ctx, VAB_NonBusinessDay_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MVABNonBusinessDay(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Check trascation record for non business day
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="VAB_Calender_ID"></param>
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
        /// <param name="VAF_Org_ID">optional organization</param>
        /// <returns>true for nonbusinessday</returns>
        public static bool IsNonBusinessDay(Ctx ctx, DateTime? dt, int VAF_Org_ID = 0)
        {
            bool nbDay = false;
            int VAB_YearPeriod_ID = MVABYearPeriod.GetVAB_YearPeriod_ID(ctx, dt, VAF_Org_ID);
            string sql = "SELECT VAB_Calender_ID FROM VAB_YEAR WHERE VAB_YEAR_ID=(SELECT VAB_YEAR_ID FROM VAB_YEARPERIOD  WHERE VAB_YEARPERIOD_ID=" + VAB_YearPeriod_ID + ")";
            int VAB_Calender_ID = Convert.ToInt32(DataBase.DB.ExecuteScalar(sql, null, null));

            sql = MVAFRole.GetDefault(ctx, false).AddAccessSQL(
               "SELECT count(*) FROM C_NONBUSINESSDAY WHERE ISACTIVE = 'Y' AND VAB_Calender_ID=" + VAB_Calender_ID
               + (VAF_Org_ID > 0 ? " AND VAF_Org_ID IN (0, " + VAF_Org_ID + ")" : "") + " AND DATE1=TO_DATE('" + dt.Value.ToShortDateString() + "', 'MM-DD-YY')",
               "VAB_NonBusinessDay", false, false);   // JID_1205: At the trx, need to check any non business day in that org. if not fund then check * org.
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
