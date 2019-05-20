/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_AD_InfoWindow
 * Chronological Development
 * Veena Pandey     31-Aug-09
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class MInfoWindow : X_AD_InfoWindow
    {
        // The Lines
        private MInfoColumn[] _lines = null;
        // Table Name
        private String _tableName = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_InfoWindow_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MInfoWindow(Ctx ctx, int AD_InfoWindow_ID, Trx trxName)
            : base(ctx, AD_InfoWindow_ID, trxName)
        {
            if (AD_InfoWindow_ID == 0)
            {
                SetEntityType(ENTITYTYPE_UserMaintained);	// U
                SetIsCustomDefault(false);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MInfoWindow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <returns>array of lines</returns>
        public MInfoColumn[] GetLines(Boolean reload)
        {
            if (_lines != null && !reload)
            {
                return _lines;
            }

            String sql = "SELECT * FROM AD_InfoColumn WHERE AD_InfoWindow_ID=@infowindid ORDER BY SeqNo";
            List<MInfoColumn> list = new List<MInfoColumn>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@infowindid", GetAD_InfoWindow_ID());

                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInfoColumn(GetCtx(), dr, Get_TrxName()));
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
                if (idr != null)
                {
                    idr.Close();
                }
            }
            //
            _lines = new MInfoColumn[list.Count];
            _lines = list.ToArray();
            return _lines;
        }

        /// <summary>
        /// Get Table Name
        /// </summary>
        /// <returns>table name</returns>
        protected String GetTableName()
        {
            if (_tableName == null)
            {
                MTable table = MTable.Get(GetCtx(), GetAD_Table_ID());
                _tableName = table.Get_TableName();
            }
            return _tableName;
        }

        /// <summary>
        /// Get SQL for Role
        /// </summary>
        /// <param name="role">role</param>
        /// <returns>statement</returns>
        public String GetSQL(MRole role)
        {
            if (_lines == null)
            {
                GetLines(true);
            }

            StringBuilder sql = new StringBuilder("SELECT ");
            for (int i = 0; i < _lines.Length; i++)
            {
                MInfoColumn col = _lines[i];
                if (i > 0)
                {
                    sql.Append(",");
                }
                sql.Append(col.GetSelectClause());
            }
            sql.Append(" FROM ").Append(GetFromClause());

            //	Access
            if (role == null)
            {
                role = MRole.GetDefault(GetCtx(), false);
            }
            String finalSQL = role.AddAccessSQL(sql.ToString(), GetTableName(), MRole.SQL_FULLYQUALIFIED, MRole.SQL_RO);
            log.Finer(finalSQL);
            return finalSQL;
        }
    }
}
