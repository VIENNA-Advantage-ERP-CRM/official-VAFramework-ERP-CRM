/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MColumnAccess
 * Purpose        : Column Access Model
 * Class Used     : X_VAF_Column_Rights
 * Chronological    Development
 * Raghunandan     26-Aug-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;

using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    

    public class MVAFColumnRights : X_VAF_Column_Rights
    {
        //	TableName			
        private String _tableName;
        //	ColumnName			
        private String _columnName;

        /// <summary>
        /// 	Persistency Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="ignored">ignored</param>
        /// <param name="trxName">transaction</param>
        public MVAFColumnRights(Ctx ctx, int ignored, Trx trxName)
            : base(ctx, 0, trxName)
        {
            if (ignored != 0)
            {
                throw new ArgumentException("Multi-Key");
            }
        }


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dr"></param>
        /// <param name="trxName"></param>
        public MVAFColumnRights(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /// <summary>
        /// Get Table Name
        /// </summary>
        /// <param name="ctx">context for translatioin</param>
        /// <returns>table name</returns>
        public String GetTableName(Ctx ctx)
        {
            if (_tableName == null)
            {
                GetColumnName(ctx);
            }
            return _tableName;
        }

        /// <summary>
        /// Get Column Name
        /// </summary>
        /// <param name="ctx">context for translatioin</param>
        /// <returns>column name</returns>
        public String GetColumnName(Ctx ctx)
        {
            if (_columnName == null)
            {
                String sql = "SELECT t.TableName,c.ColumnName, t.VAF_TableView_ID "
                    + "FROM VAF_TableView t INNER JOIN VAF_Column c ON (t.VAF_TableView_ID=c.VAF_TableView_ID) "
                    + "WHERE VAF_Column_ID='" + GetVAF_Column_ID() + "'";
                IDataReader dr = null;
                try
                {
                    dr = VAdvantage.DataBase.DB.ExecuteReader(sql, null, Get_Trx());

                    if (dr.Read())
                    {
                        _tableName = dr[0].ToString();
                        _columnName = dr[1].ToString();
                        if (Utility.Util.GetValueOfInt(dr[2].ToString()) != GetVAF_TableView_ID())
                        {
                            log.Log(Level.SEVERE, "VAF_TableView_ID inconsistent - Access=" + GetVAF_TableView_ID() + " - Table=" + Utility.Util.GetValueOfInt(dr[2].ToString()));
                        }
                    }
                    dr.Close();
                }
                catch (Exception e)
                {
                    if (dr != null)
                    {
                        dr.Close();
                    }
                    log.Log(Level.SEVERE, sql, e);
                }
                //	Get Clear Text
                String realName = Msg.Translate(ctx, _tableName + "_ID");
                if (!realName.Equals(_tableName + "_ID"))
                {
                    _tableName = realName;
                }
                _columnName = Msg.Translate(ctx, _columnName);
            }
            return _columnName;
        }

        /// <summary>
        /// Extended String Representation.
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns>extended info</returns>
        public String ToStringX(Ctx ctx)
        {
            String inn = Msg.GetMsg(Env.GetContext(), "Include");
            String ex = Msg.GetMsg(Env.GetContext(), "Exclude");
            StringBuilder sb = new StringBuilder();
            sb.Append(Msg.Translate(ctx, "VAF_TableView_ID"))
                .Append("=").Append(GetTableName(ctx)).Append(", ")
                .Append(Msg.Translate(ctx, "VAF_Column_ID"))
                .Append("=").Append(GetColumnName(ctx))
                .Append(" (").Append(Msg.Translate(ctx, "IsReadOnly")).Append("=").Append(IsReadOnly())
                .Append(") - ").Append(IsExclude() ? ex : inn);
            return sb.ToString();
        }

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MColumnAccess[");
            sb.Append("VAF_Role_ID=").Append(GetVAF_Role_ID())
                .Append(",VAF_TableView_ID=").Append(GetVAF_TableView_ID())
                .Append(",VAF_Column_ID=").Append(GetVAF_Column_ID())
                .Append(",Exclude=").Append(IsExclude());
            sb.Append("]");
            return sb.ToString();
        }

    }
}
