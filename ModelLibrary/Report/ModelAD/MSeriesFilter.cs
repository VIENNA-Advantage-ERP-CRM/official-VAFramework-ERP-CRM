using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Classes;
using System.Data.SqlClient;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MSeriesFilter : X_D_SeriesFilter
    {
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="D_Chart_ID">series ID</param>
        /// <param name="trxName">trx name (optional)</param>
        public MSeriesFilter(Ctx ctx, int D_SeriesFilter_ID, Trx trxName)
            : base(ctx, D_SeriesFilter_ID, trxName)
        {
            if (D_SeriesFilter_ID == 0)
            {
                //save default records
            }
        }


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">DataRow</param>
        /// <param name="trxName">Transaction</param>
        public MSeriesFilter(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }	//	MPrintColor


        /**	Logger				*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MSeriesFilter).FullName);
        /** Cached Fonts						*/
        private static CCache<int, MSeriesFilter> s_chart = new CCache<int, MSeriesFilter>("D_SeriesFilter", 5);

        /// <summary>
        /// Get Charts
        /// </summary>
        /// <param name="D_Chart_ID">Chart ID</param>
        /// <returns>Charts</returns>
        static public MSeriesFilter Get(int D_SeriesFilter_ID)
        {
            int key = D_SeriesFilter_ID;
            MSeriesFilter msf = null;
            if (s_chart.ContainsKey(key))
                msf = s_chart[key];

            if (msf == null)
            {
                msf = new MSeriesFilter(Env.GetCtx(), key, null);
                if (s_chart.ContainsKey(key))
                    s_chart[key] = msf;
                else
                    s_chart.Add(key, msf);
            }
            else
                s_log.Config("Chart ID = " + key);

            return msf;
        }


        static public MSeriesFilter[] GetFilters(int D_SeriesFilter_ID)
        {
            int key = D_SeriesFilter_ID;
            List<MSeriesFilter> list = new List<MSeriesFilter>();
            String sql = "SELECT * FROM D_SeriesFilter pfi "
                + "WHERE pfi.IsActive='Y' And pfi.D_Series_ID=@D_Series_ID";
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@D_Series_ID", key);
                DataSet ds = SqlExec.ExecuteQuery.ExecuteDataset(sql, param);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MSeriesFilter pfi = new MSeriesFilter(Env.GetCtx(), dr, null);
                    //if (role.IsColumnAccess(GetAD_Table_ID(), pfi.GetAD_Column_ID(), true))
                    list.Add(pfi);
                }
            }
            catch (Exception e)
            {
                s_log.Severe(e.ToString());
                //log entry, if any
            }
            MSeriesFilter[] retValue = new MSeriesFilter[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

    }
}
