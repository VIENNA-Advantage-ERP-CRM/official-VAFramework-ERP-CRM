
/********************************************************
 * Module/Class Name    : Model Classes (MQueryLog.cs)
 * Purpose              : Handle the table VAF_DBQueryLog
 * Class Used           : X_VAF_DBQueryLog
 * Created By           : Mukesh Arora
 * Date                 : 05-May-09
**********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MQueryLog : X_VAF_DBQueryLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_DBQueryLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MQueryLog(Ctx ctx, int VAF_DBQueryLog_ID, Trx trxName)
            : base(ctx, VAF_DBQueryLog_ID, trxName)
        {
            //super(ctx, VAF_DBQueryLog_ID, trxName);
            if (VAF_DBQueryLog_ID == 0)
            {
                int VAF_Role_ID = ctx.GetVAF_Role_ID();
                SetVAF_Role_ID(VAF_Role_ID);
            }
        }	//	MQueryLog

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">trx</param>
        public MQueryLog(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

        }	//	MQueryLog


        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">ctx</param>
        /// <param name="VAF_Session_ID">session</param>
        /// <param name="VAF_Client_ID">login client</param>
        /// <param name="VAF_Org_ID">org</param>
        /// <param name="VAF_TableView_ID">table</param>
        /// <param name="WhereClause">where</param>
        /// <param name="RecordCount">count</param>
        /// <param name="Parameter">parameter</param>
        public MQueryLog(Ctx ctx, int VAF_Session_ID,
        int VAF_Client_ID, int VAF_Org_ID,
        int VAF_TableView_ID, String whereClause, int recordCount, String parameter)
            : this(ctx, 0, null)
        {
            //	out of trx
            SetVAF_Session_ID(VAF_Session_ID);
            SetClientOrg(VAF_Client_ID, VAF_Org_ID);
            //
            SetVAF_TableView_ID(VAF_TableView_ID);
            SetWhereClause(whereClause);
            SetRecordCount(recordCount);
            SetParameter(parameter);
        }	//	MQueryLog

        /// <summary>
        /// Set Where Clause
        /// </summary>
        /// <param name="sql">sql or where clause</param>
        public new void SetWhereClause(String sql)
        {
            String where = "";
            if (sql != null)
            {
                where = sql.Trim();
                int indexW = where.IndexOf(" WHERE ");
                if (indexW >= 0)
                    where = where.Substring(indexW + 7);
                int indexO = where.IndexOf(" ORDER BY ");
                if (indexO >= 0)
                    where = where.Substring(0, indexO);
            }
            base.SetWhereClause(where);
        }	//	setWhereClause
    }
}
