/********************************************************
 * Class Name     : MImpFormat
 * Purpose        : Import Format Model
 * Class Used     : X_VAF_ImportFormat
 * Chronological    Development
 * Deepak         : 03-feb-2010
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.DataBase;


namespace VAdvantage.Model
{
    public class MImpFormat : X_VAF_ImportFormat
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_ImportFormat_ID">id</param>
        /// <param name="trxName">trx</param>
        public MImpFormat(Ctx ctx, int VAF_ImportFormat_ID, Trx trxName)
            : base(ctx, VAF_ImportFormat_ID, trxName)
        {

        }	//	MImpFormat

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">trx</param>
        public MImpFormat(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MImpFormat

        /// <summary>
        /// Get (all) Rows
        /// </summary>
        /// <returns>array of Rows</returns>
        public MImpFormatRow[] GetRows()
        {
            List<MImpFormatRow> list = new List<MImpFormatRow>();
            String sql = "SELECT * FROM VAF_ImportFormat_Row "
                + "WHERE VAF_ImportFormat_ID=@param "
                + "ORDER BY SeqNo";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, get_TrxName());
                //pstmt.setInt (1, getVAF_ImportFormat_ID());
                param[0] = new SqlParameter("@param", GetVAF_ImportFormat_ID());
                idr = DB.ExecuteReader(sql, param, Get_TrxName());
                while (idr.Read())
                {
                    list.Add(new MImpFormatRow(GetCtx(), idr, Get_TrxName()));
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, "getRows", e);
            }

            MImpFormatRow[] retValue = new MImpFormatRow[list.Count];
            retValue = list.ToArray();
            return retValue;
        }	//	getRows

    }	
}
