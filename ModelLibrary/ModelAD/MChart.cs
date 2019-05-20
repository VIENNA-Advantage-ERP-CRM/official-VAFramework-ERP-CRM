//Created  By   :   Jagmohan Bhatt
//Dated         :   15-Apr-2010
//Purpose       :   Operation on different chart types
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.Classes;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    /// <summary>
    /// Model class for Chart
    /// </summary>
    public class MChart : X_D_Chart
    {
       

        private Ctx _ctx = null;
        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="D_Chart_ID">chart ID</param>
        /// <param name="trxName">trx name (optional)</param>
        public MChart(Ctx ctx, int D_Chart_ID, Trx trxName)
            : base(ctx, D_Chart_ID, trxName)
        {
            _ctx = ctx;
            if (D_Chart_ID == 0)
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
        public MChart(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
            _ctx = ctx;
        }	//	MPrintColor

        /// <summary>
        /// Get Charts
        /// </summary>
        /// <param name="D_Chart_ID">Chart ID</param>
        /// <returns>Charts</returns>
        /// 
        //static public MChart Get(int D_Chart_ID,Ctx ctx)
        //{
        //    int key = D_Chart_ID;
        //    MChart mchart = null;
        //    //if (s_chart.ContainsKey(key))
        //    //    mchart = s_chart[key];

        //    //if (mchart == null)
        //    //{
        //        mchart = new MChart(ctx, D_Chart_ID, null);
        //    //    if (s_chart.ContainsKey(key))
        //    //        s_chart[key] = mchart;
        //    //    else
        //    //        s_chart.Add(key, mchart);
        //    //}
        //    //else
        //    //    s_log.Config("Chart ID = " + D_Chart_ID);

        //    return mchart;
        //}

        /**	Logger				*/
        private static VLogger s_log = VLogger.GetVLogger(typeof(MChart).FullName);
        /** Cached Fonts						*/
        private static CCache<int, MChart> s_chart = new CCache<int, MChart>("D_Chart", 5);

        /// <summary>
        /// Different types of chart available for display
        /// </summary>
        public enum enmChartTypes
        {
            Column = 1,
            Line = 2,
            Pie = 3,
            Bar = 4

        }

        /// <summary>
        /// Get active Items
        /// </summary>
        /// <returns>items</returns>
        public static MChart[] GetCharts(Ctx ctx)
        {
            List<MChart> list = new List<MChart>();
            String sql = "SELECT pfi.AD_CLIENT_ID, " +
                            "pfi.AD_ORG_ID, " +
                            "pfi.CHARTTYPE, " +
                            "pfi.D_CHART_ID, " +
                            "pfi.ENABLE3D , " +
                            "pfi.ISACTIVE, " +
                            "pfi.NAME, " +
                            "pfi.AD_CHART_BG_COLOR_ID, " +
                            "pfi.ISSHOWLEGEND, " +
                            "pfi.ISSHOWZEROSERIES, " +
                            "pfi.SEQNO, " +
                            "pfi.IsTiltLabel " +
                        "FROM D_Chart pfi INNER JOIN D_CHARTACCESS DCA " +
                "ON pfi.D_CHART_ID=DCA.D_CHART_ID " +
                "INNER JOIN (SELECT DISTINCT D_CHART_ID FROM D_Series WHERE ISACTIVE ='Y') ds " +
                "ON DS.D_CHART_ID = DCA.D_CHART_ID " +
                "WHERE pfi.IsActive='Y' " +
                "AND DCA.AD_ROLE_ID='" + ctx.GetAD_Role_ID() + "' AND DCA.ISACTIVE='Y' AND DCA.ISREADWRITE = 'Y' ORDER BY SeqNo asc";

            MRole role = MRole.GetDefault(ctx, false);
            if (role.IsTableAccess(Table_ID, false))
            {

                try
                {
                    //SqlParameter[] param = new SqlParameter[1];
                    //param[0] = new SqlParameter("@D_ChartID", Get_ID());
                    DataSet ds = DB.ExecuteDataset(sql, null);
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        MChart pfi = new MChart(ctx, dr, null);
                        //if (role.IsColumnAccess(GetAD_Table_ID(), pfi.GetAD_Column_ID(), true))
                        list.Add(pfi);
                    }
                }
                catch (Exception e)
                {
                    s_log.Severe(e.ToString());
                    //log entry, if any
                }
            }
            //
            MChart[] retValue = new MChart[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getItems

        public static void EmptyCache()
        {
            s_chart.Clear();
        }

    }
}
