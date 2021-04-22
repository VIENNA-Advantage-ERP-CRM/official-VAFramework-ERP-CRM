/********************************************************
 * Class Name     : MVAGLReDistribution
 * Purpose        : GL Re-Distribution Model
 * Class Used     : X_VAGL_ReDistribution 
 * Chronological    Development
 * Bharat           19-Nov-2019
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;


namespace VAdvantage.Model
{
    public class MVAGLReDistribution : X_VAGL_ReDistribution
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAGL_ReDistribution_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAGLReDistribution(Ctx ctx, int VAGL_ReDistribution_ID, Trx trxName)
            : base(ctx, VAGL_ReDistribution_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAGLReDistribution(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            int VAF_TableView_ID = Get_Table_ID("VAGL_ReDistribution");

            string sql = "SELECT COUNT(Actual_Acct_Detail_ID) FROM Actual_Acct_Detail WHERE VAF_TableView_ID=" + VAF_TableView_ID + " AND Record_ID=" + Get_ID();
            int no = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, Get_Trx()));

            if (no > 0)
            {
                log.SaveError("DeleteError", Msg.GetMsg(GetCtx(), "AccountFactExist"));
                return false;
            }
            return true;
        }
    }
}
