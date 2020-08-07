/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MDistributionRun
 * Purpose        :Distribution Run Model
 * Class Used     : X_M_DistributionListLine
 * Chronological    Development
 * Raghunandan     03-Nov-2009
  ******************************************************/
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
    public class MDistributionRun : X_M_DistributionRun
    {
        // Cached Lines					
        private MDistributionRunLine[] _lines = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="M_DistributionRun_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MDistributionRun(Ctx ctx, int M_DistributionRun_ID, Trx trxName)
            : base(ctx, M_DistributionRun_ID, trxName)
        {

        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MDistributionRun(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }

        /// <summary>
        /// Get active, non zero lines
        /// </summary>
        /// <param name="reload">true if reload</param>
        /// <returns>lines</returns>
        public MDistributionRunLine[] GetLines(bool reload)
        {
            if (!reload && _lines != null)
            {
                return _lines;
            }
            //
            String sql = "SELECT * FROM M_DistributionRunLine "
                + "WHERE M_DistributionRun_ID=" + GetM_DistributionRun_ID() + " AND IsActive='Y' AND TotalQty IS NOT NULL AND TotalQty<> 0 ORDER BY Line";
            List<MDistributionRunLine> list = new List<MDistributionRunLine>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MDistributionRunLine(GetCtx(), dr, Get_TrxName()));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            finally
            {
                dt = null;
            }

            _lines = new MDistributionRunLine[list.Count];
            _lines = list.ToArray();
            return _lines;
        }
    }
}
