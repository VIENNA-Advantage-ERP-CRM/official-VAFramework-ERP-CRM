/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAF_QuickSearchWindow
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
    public class MVAFQuickSearchWindow : X_VAF_QuickSearchWindow
    {
        // The Lines
        private MVAFQuickSearchColumn[] _lines = null;
        // Table Name
        private String _tableName = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_QuickSearchWindow_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAFQuickSearchWindow(Ctx ctx, int VAF_QuickSearchWindow_ID, Trx trxName)
            : base(ctx, VAF_QuickSearchWindow_ID, trxName)
        {
            if (VAF_QuickSearchWindow_ID == 0)
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
        public MVAFQuickSearchWindow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Lines
        /// </summary>
        /// <param name="reload">reload data</param>
        /// <returns>array of lines</returns>
        public MVAFQuickSearchColumn[] GetLines(Boolean reload)
        {
            if (_lines != null && !reload)
            {
                return _lines;
            }

            String sql = "SELECT * FROM VAF_QuickSearchColumn WHERE VAF_QuickSearchWindow_ID=@infowindid ORDER BY SeqNo";
            List<MVAFQuickSearchColumn> list = new List<MVAFQuickSearchColumn>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@infowindid", GetVAF_QuickSearchWindow_ID());

                idr = CoreLibrary.DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAFQuickSearchColumn(GetCtx(), dr, Get_TrxName()));
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
            _lines = new MVAFQuickSearchColumn[list.Count];
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
                MVAFTableView table = MVAFTableView.Get(GetCtx(), GetVAF_TableView_ID());
                _tableName = table.Get_TableName();
            }
            return _tableName;
        }

        /// <summary>
        /// Get SQL for Role
        /// </summary>
        /// <param name="role">role</param>
        /// <returns>statement</returns>
        public String GetSQL(MVAFRole role)
        {
            if (_lines == null)
            {
                GetLines(true);
            }

            StringBuilder sql = new StringBuilder("SELECT ");
            for (int i = 0; i < _lines.Length; i++)
            {
                MVAFQuickSearchColumn col = _lines[i];
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
                role = MVAFRole.GetDefault(GetCtx(), false);
            }
            String finalSQL = role.AddAccessSQL(sql.ToString(), GetTableName(), MVAFRole.SQL_FULLYQUALIFIED, MVAFRole.SQL_RO);
            log.Finer(finalSQL);
            return finalSQL;
        }
    }
}
