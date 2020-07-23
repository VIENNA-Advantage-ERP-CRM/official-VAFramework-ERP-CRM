using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using System.Data;
using VAdvantage.Common;
using VAdvantage.Utility;
using VAdvantage.Logging;
using VAdvantage.DataBase;

namespace VAdvantage.Model
{
   public  class MTableAccess:X_AD_Table_Access
    {

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="rs"></param>
        /// <param name="trxName"></param>
        public MTableAccess(Ctx ctx, DataRow  rs, Trx trxName):base(ctx, rs, trxName)
        {

        }	//	MTableAccess

        public String ToStringX(Ctx ctx)
        {
            String inn = Msg.GetMsg(ctx,"Include");
            String ex = Msg.GetMsg(ctx,"Exclude");
            StringBuilder sb = new StringBuilder();
            sb.Append(Msg.Translate(ctx, "AD_Table_ID"))
                .Append("=").Append(GetTableName(ctx));
            if (ACCESSTYPERULE_Accessing.Equals(GetAccessTypeRule()))
                sb.Append(" - ").Append(Msg.Translate(ctx, "IsReadOnly")).Append("=").Append(IsReadOnly());
            else if (ACCESSTYPERULE_Exporting.Equals(GetAccessTypeRule()))
                sb.Append(" - ").Append(Msg.Translate(ctx, "IsCanExport")).Append("=").Append(IsCanExport());
            else if (ACCESSTYPERULE_Reporting.Equals(GetAccessTypeRule()))
                sb.Append(" - ").Append(Msg.Translate(ctx, "IsCanReport")).Append("=").Append(IsCanReport());
            sb.Append(" - ").Append(IsExclude() ? ex : inn);
            return sb.ToString();
        }	//	toStringX


        /**	TableName			*/
        private String m_tableName;

        public String GetTableName(Ctx ctx)
        {
            if (m_tableName == null)
            {
                String sql = "SELECT TableName FROM AD_Table WHERE AD_Table_ID='" + GetAD_Table_ID() + "'";
                IDataReader dr = null;
                try
                {
                     dr = VAdvantage.DataBase.DB.ExecuteReader(sql, null, Get_TrxName());
                     if (dr.Read())
                     {
                         m_tableName = dr[0].ToString();
                     }
                    dr.Close();
                    dr = null;
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                        dr = null;
                    }
                    log.Log(Level.SEVERE, "getTableName", e);
                }
                //	Get Clear Text
                String realName = Msg.Translate(ctx, m_tableName + "_ID");
                if (!realName.Equals(m_tableName + "_ID"))
                    m_tableName = realName;
            }
            return m_tableName;
        }	

    }
}
