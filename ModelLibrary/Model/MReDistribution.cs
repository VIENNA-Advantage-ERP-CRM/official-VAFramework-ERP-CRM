/********************************************************
 * Class Name     : MReDistribution
 * Purpose        : GL Re-Distribution Model
 * Class Used     : X_GL_ReDistribution 
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
using System.Windows.Forms;
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
    public class MReDistribution : X_GL_ReDistribution
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="GL_ReDistribution_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReDistribution(Ctx ctx, int GL_ReDistribution_ID, Trx trxName)
            : base(ctx, GL_ReDistribution_ID, trxName)
        {
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MReDistribution(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Before Delete
        /// </summary>
        /// <returns>true if it can be deleted</returns>
        protected override bool BeforeDelete()
        {
            int AD_Table_ID = Get_Table_ID("GL_ReDistribution");

            string sql = "SELECT COUNT(Fact_Acct_ID) FROM Fact_Acct WHERE AD_Table_ID=" + AD_Table_ID + " AND Record_ID=" + Get_ID();
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
