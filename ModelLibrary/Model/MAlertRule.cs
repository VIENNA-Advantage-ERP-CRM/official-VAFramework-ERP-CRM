using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Utility;
//using System.util;
using System.Data;
using VAdvantage.Classes;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
    public class MAlertRule : X_AD_AlertRule
    {
        public MAlertRule(Ctx ctx, int AD_AlertRule_ID, Trx trx)
            : base(ctx, AD_AlertRule_ID, trx)
        {
            
        }	//	MAlertRule


        public MAlertRule(Ctx ctx, DataRow rs, Trx trx)
            : base(ctx, rs, trx)
        {
            
        }	//	MAlertRule


        public String GetSql()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ").Append(GetSelectClause())
                .Append(" FROM ").Append(GetFromClause());
            if (GetWhereClause() != null && GetWhereClause().Length > 0)
                sql.Append(" WHERE ").Append(GetWhereClause());
            if (GetOtherClause() != null && GetOtherClause().Length > 0)
                sql.Append(" ").Append(GetOtherClause());
            return sql.ToString();
        }	//	getSql

        public String GetTableName()
        {
            int AD_Table_ID = GetAD_Table_ID();
            if (AD_Table_ID != 0)
            {
                MTable table = MTable.Get(GetCtx(), AD_Table_ID);
                String tableName = table.GetTableName();
                if (!VAdvantage.Utility.Util.IsEmpty(tableName))
                    return tableName;
            }
            //	FROM clause
            String from = GetFromClause().Trim();
            StringTokenizer st = new StringTokenizer(from, " ,\t\n\r\f", false);
            int tokens = st.CountTokens();
            if (tokens == 0)
                return null;
            if (tokens == 1)
                return st.NextToken();
            String mainTable = st.NextToken();
            if (st.HasMoreTokens())
            {
                String next = st.NextToken();
                if (next.Equals("RIGHT", StringComparison.OrdinalIgnoreCase)
                    || next.Equals("LEFT", StringComparison.OrdinalIgnoreCase)
                    || next.Equals("INNER", StringComparison.OrdinalIgnoreCase)
                    || next.Equals("FULL", StringComparison.OrdinalIgnoreCase))
                    return mainTable;
                return next;
            }
            return mainTable;
        }	//	getTableName

        protected override bool BeforeSave(bool newRecord)
        {
            if (newRecord)
                SetIsValid(true);
            if (IsValid())
                SetErrorMsg(null);
            return true;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("MAlertRule[");
            sb.Append(Get_ID())
                .Append("-").Append(GetName())
                .Append(",Valid=").Append(IsValid())
                .Append(",").Append(GetSql());
            sb.Append("]");
            return sb.ToString();
            
        }
    }
}
