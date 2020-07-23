///********************************************************
// * Project Name   : VAdvantage
// * Class Name     : MDunningLevel
// * Purpose        : Dunning Level Model
// * Class Used     : X_C_DunningLevel
// * Chronological    Development
// * Deepak          10-Nov-2009
//  ******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MDunningLevel : X_C_DunningLevel
    {
        //Logger								
        private static VLogger _log = VLogger.GetVLogger(typeof(MDunningLevel).FullName);
        private MDunning _dunning = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="C_DunningLevel_ID"></param>
        /// <param name="trxName"></param>
        public MDunningLevel(Ctx ctx, int C_DunningLevel_ID, Trx trxName)
            : base(ctx, C_DunningLevel_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDunningLevel(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// get Parent
        /// </summary>
        /// <returns>Parent Dunning</returns>
        public MDunning GetParent()
        {
            if (_dunning == null)
            {
                _dunning = new MDunning(GetCtx(), GetC_Dunning_ID(), Get_TrxName());
            }
            return _dunning;
        }

        /// <summary>
        /// 	get Previous Levels
        /// </summary>
        /// <returns>Array of previous DunningLevels</returns>
        public MDunningLevel[] GetPreviousLevels()
        {
            // Prevent generation if not Sequentially
            if (!GetParent().IsCreateLevelsSequentially())
            {
                return null;
            }
            List<MDunningLevel> list = new List<MDunningLevel>();
            String sql = "SELECT * FROM C_DunningLevel WHERE C_Dunning_ID=@Param1 AND DaysAfterDue+DaysBetweenDunning<@Param2";
            DataTable dt = null;
            IDataReader idr = null;
            SqlParameter[] Param = new SqlParameter[2];
            try
            {
                int totalDays = Utility.Util.GetValueOfInt(GetDaysAfterDue()) + GetDaysBetweenDunning();
                Param[0] = new SqlParameter("@Param1", GetParent().Get_ID());
                Param[1] = new SqlParameter("@param2", totalDays);
                idr = DataBase.DB.ExecuteReader(sql, Param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MDunningLevel(GetCtx(), dr, Get_TrxName()));
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
            //
            MDunningLevel[] retValue = new MDunningLevel[list.Count];
            retValue = list.ToArray();
            return retValue;
        }
    }

}
