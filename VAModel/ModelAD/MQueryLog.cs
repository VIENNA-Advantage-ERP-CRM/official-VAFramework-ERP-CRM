
/********************************************************
 * Module/Class Name    : Model Classes (MQueryLog.cs)
 * Purpose              : Handle the table AD_QueryLog
 * Class Used           : X_AD_QueryLog
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
    public class MQueryLog : X_AD_QueryLog
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_QueryLog_ID">id</param>
        /// <param name="trxName">trx</param>
        public MQueryLog(Ctx ctx, int AD_QueryLog_ID, Trx trxName)
            : base(ctx, AD_QueryLog_ID, trxName)
        {
            //super(ctx, AD_QueryLog_ID, trxName);
            if (AD_QueryLog_ID == 0)
            {
                int AD_Role_ID = ctx.GetAD_Role_ID();
                SetAD_Role_ID(AD_Role_ID);
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
        /// <param name="AD_Session_ID">session</param>
        /// <param name="AD_Client_ID">login client</param>
        /// <param name="AD_Org_ID">org</param>
        /// <param name="AD_Table_ID">table</param>
        /// <param name="WhereClause">where</param>
        /// <param name="RecordCount">count</param>
        /// <param name="Parameter">parameter</param>
        public MQueryLog(Ctx ctx, int AD_Session_ID,
        int AD_Client_ID, int AD_Org_ID,
        int AD_Table_ID, String whereClause, int recordCount, String parameter)
            : this(ctx, 0, null)
        {
            //	out of trx
            SetAD_Session_ID(AD_Session_ID);
            SetClientOrg(AD_Client_ID, AD_Org_ID);
            //
            SetAD_Table_ID(AD_Table_ID);
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
