/********************************************************
 * Class Name     : MImpFormat
 * Purpose        : Import Format Model
 * Class Used     : X_AD_ImpFormat
 * Chronological    Development
 * Deepak         : 03-feb-2010
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
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MImpFormat : X_AD_ImpFormat
    {

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_ImpFormat_ID">id</param>
        /// <param name="trxName">trx</param>
        public MImpFormat(Ctx ctx, int AD_ImpFormat_ID, Trx trxName)
            : base(ctx, AD_ImpFormat_ID, trxName)
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
            String sql = "SELECT * FROM AD_ImpFormat_Row "
                + "WHERE AD_ImpFormat_ID=@param "
                + "ORDER BY SeqNo";
            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            try
            {
                //pstmt = DataBase.prepareStatement (sql, get_TrxName());
                //pstmt.setInt (1, getAD_ImpFormat_ID());
                param[0] = new SqlParameter("@param", GetAD_ImpFormat_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
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
