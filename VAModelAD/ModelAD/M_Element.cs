/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : M_Element
 * Purpose        : System Element Model
 * Class Used     : M_Element inherits from X_VAF_ColumnDic class
 * Chronological    Development
 * Raghunandan      08-May-2009
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using VAdvantage.Logging;
using VAdvantage.Utility;

namespace VAdvantage.Model
{
    public class M_Element : X_VAF_ColumnDic
    {
        //	Logger	
        private static VLogger _log = VLogger.GetVLogger(typeof(M_Element).FullName);
        /// <summary>
        ///Get case sensitive Column Name
        /// </summary>
        /// <param name="columnName">columnName case insensitive column name</param>
        /// <returns>case sensitive column name</returns>
        public static String GetColumnName(string columnName)
        {
            if (columnName == null || columnName.Length == 0)
                return columnName;
            String retValue = columnName;
            String sql = "SELECT ColumnName FROM VAF_ColumnDic WHERE UPPER(ColumnName)=" + columnName.ToUpper();
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = rs[1].ToString();//.getString(1);
                    _log.Warning("Not unique: " + columnName + " -> " + retValue + " - " + rs[0].ToString());
                }
                if (ds == null)
                {
                    _log.Warning("No found: " + columnName);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log (Level.SEVERE, columnName, e);
            }
            return retValue;
        }

        /// <summary>
        ///Get Element
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="columnName">columnName case insensitive column name</param>
        /// <returns>case sensitive column name</returns>
        public static M_Element Get(Ctx ctx, String columnName)
        {
            if (columnName == null || columnName.Length == 0)
                return null;
            M_Element retValue = null;
            String sql = "SELECT * FROM VAF_ColumnDic WHERE UPPER(ColumnName)=" + columnName.ToUpper();
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new M_Element(ctx, rs, null);
                    _log.Warning("Not unique: " + columnName+ " -> " + retValue + " - " + rs["ColumnName"].ToString());
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log (Level.SEVERE, sql, e);
            }
            return retValue;
        }

        public static M_Element Get(Ctx ctx, String columnName, Trx trxName)
        {
            if (columnName == null || columnName.Length == 0)
                return null;
            M_Element retValue = null;
            String sql = "SELECT * FROM VAF_ColumnDic WHERE UPPER(ColumnName)='" + columnName.ToUpper() + "'";
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
            }
            catch
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);            //in case the transaction error comes, try again without trx
            }
            try
            {
                //ds = BaseLibrary.DataBase.DB.ExecuteDataset(sql, null, trxName);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new M_Element(ctx, rs, trxName);
                    _log.Warning("Not unique: " + columnName + " -> " + retValue + " - " + rs["ColumnName"].ToString());
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        ///Get Element
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Column_ID">columnName case insensitive column name</param>
        /// <returns>case sensitive column name</returns>
        public static M_Element GetOfColumn(Ctx ctx, int VAF_Column_ID)
        {
            if (VAF_Column_ID == 0)
                return null;
            M_Element retValue = null;
            String sql = "SELECT * FROM VAF_ColumnDic e "
                + "WHERE EXISTS (SELECT * FROM VAF_Column c "
                    + "WHERE c.VAF_ColumnDic_ID=e.VAF_ColumnDic_ID AND c.VAF_Column_ID=" + VAF_Column_ID + ")";
            DataSet ds = null;
            try
            {
                ds = CoreLibrary.DataBase.DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    retValue = new M_Element(ctx, rs, null);
                }
                ds = null;
            }
            catch (Exception e)
            {
                _log.Log (Level.SEVERE, sql, e);
            }
            return retValue;
        }

        /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_ColumnDic_ID">element</param>
        /// <param name="trxName">transaction</param>
        public M_Element(Ctx ctx, int VAF_ColumnDic_ID, Trx trxName)
            : base(ctx, VAF_ColumnDic_ID, trxName)
        {
            if (VAF_ColumnDic_ID == 0)
            {
                //	setColumnName (null);
                //	setEntityType (null);	// U
                //	setName (null);
                //	setPrintName (null);
            }
        }

        /// <summary>
        ///Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">result set</param>
        /// <param name="trxName">transaction</param>
        public M_Element(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {

        }

        /// <summary>
        ///Minimum Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="columnName">column</param>
        /// <param name="EntityType">entity type</param>
        /// <param name="trxName">trx</param>
        public M_Element(Ctx ctx, string columnName, string entityType, Trx trxName)
            : base(ctx, 0, trxName)
        {
            SetColumnName(columnName);
            SetName(columnName);
            SetPrintName(columnName);
            SetEntityType(entityType);	// U
        }

        /// <summary>
        ///After Save
        /// </summary>
        /// <param name="newRecord">newRecord new</param>
        /// <param name="success">success</param>
        /// <returns>success</returns>
        protected override bool AfterSave(bool newRecord, bool success)
	{
		//	Update Columns, Fields, Parameters, Print Info
		if (!newRecord)
		{
			//	Column
			StringBuilder sql = new StringBuilder("UPDATE VAF_Column SET ColumnName=")
				.Append(DataBase.DB.TO_STRING(GetColumnName()))
				.Append(", Name=").Append(DataBase.DB.TO_STRING(GetName()))
				.Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
				.Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
				.Append(" WHERE VAF_ColumnDic_ID=").Append(Get_ID());

            int no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			log.Fine("Columns updated #" + no);
			//	Field
            sql = new StringBuilder("UPDATE VAF_Field SET ");

            //In Case of migration- do not update name of fields
            if(!DB.UseMigratedConnection)
            {
                sql.Append("Name = ").Append(DataBase.DB.TO_STRING(GetName())).Append(",") ;
            }
				sql.Append(" Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
				.Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
				.Append(" WHERE VAF_Column_ID IN (SELECT VAF_Column_ID FROM VAF_Column WHERE VAF_ColumnDic_ID=")
				.Append(Get_ID())
				.Append(") AND IsCentrallyMaintained='Y'")
				.Append(" AND VAF_Tab_ID IN (SELECT VAF_Tab_ID FROM VAF_Tab t INNER JOIN VAF_Screen w ON (t.VAF_Screen_ID=w.VAF_Screen_ID) WHERE t.VAF_ContextScope_ID IS NULL AND w.VAF_ContextScope_ID IS NULL)");

            no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			log.Fine("Fields updated #" + no);
			
			//	Parameter 
			sql = new StringBuilder("UPDATE VAF_Job_Para SET ColumnName=")
				.Append(DataBase.DB.TO_STRING(GetColumnName()));
            if (!DB.UseMigratedConnection)
            {
                sql.Append(", Name = ").Append(DataBase.DB.TO_STRING(GetName()));
            }
				//.Append(", Name=").Append(DataBase.DB.TO_STRING(GetName()))
                 sql.Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
				.Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
				.Append(", VAF_ColumnDic_ID=").Append(Get_ID())
				.Append(" WHERE UPPER(ColumnName)=")
				.Append(DataBase.DB.TO_STRING(GetColumnName().ToUpper()))
				.Append(" AND IsCentrallyMaintained='Y' AND VAF_ColumnDic_ID IS NULL");

            no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			sql = new StringBuilder("UPDATE VAF_Job_Para SET ColumnName=")
				.Append(DataBase.DB.TO_STRING(GetColumnName()));
                 if (!DB.UseMigratedConnection)
            {
                sql.Append(", Name = ").Append(DataBase.DB.TO_STRING(GetName()));
            }
			
				//.Append(", Name=").Append(DataBase.DB.TO_STRING(GetName()))
				sql.Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
				.Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
				.Append(" WHERE VAF_ColumnDic_ID=").Append(Get_ID())
				.Append(" AND IsCentrallyMaintained='Y'")
				.Append(" AND VAF_Job_ID IN (SELECT VAF_Job_ID FROM VAF_Job WHERE VAF_ContextScope_ID IS NULL)");

            no += Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			log.Fine("Parameters updated #" + no);
			//	Print Info
            sql = new StringBuilder("UPDATE VAF_Print_Rpt_LItem pi SET PrintName=")
                .Append(DataBase.DB.TO_STRING(GetPrintName()));
                 if (!DB.UseMigratedConnection)
            {
                sql.Append(", Name = ").Append(DataBase.DB.TO_STRING(GetName()));
            }
			
				//.Append(", Name=").Append(DataBase.DB.TO_STRING(GetName()))
				sql.Append(" WHERE IsCentrallyMaintained='Y'")	
				.Append(" AND EXISTS (SELECT * FROM VAF_Column c ")
					.Append("WHERE c.VAF_Column_ID=pi.VAF_Column_ID AND c.VAF_ColumnDic_ID=")
					.Append(Get_ID()).Append(")");

            no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			log.Fine("PrintFormatItem updated #" + no);
			// Info Column
			sql = new StringBuilder ("UPDATE VAF_QuickSearchColumn SET Name=")
				.Append(DataBase.DB.TO_STRING(GetName()))
				.Append(", Description=").Append(DataBase.DB.TO_STRING(GetDescription()))
				.Append(", Help=").Append(DataBase.DB.TO_STRING(GetHelp()))
				.Append(" WHERE VAF_ColumnDic_ID=").Append(Get_ID())
				.Append(" AND IsCentrallyMaintained='Y'");

            no = Utility.Util.GetValueOfInt(DataBase.DB.ExecuteQuery(sql.ToString(), null, Get_TrxName()));
			log.Fine("InfoWindow updated #" + no);
		}
		return success;
	}

        /// <summary>
        ///String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("M_Element[");
            sb.Append(Get_ID()).Append("-").Append(GetColumnName()).Append("]");
            return sb.ToString();
        }

    }
}
