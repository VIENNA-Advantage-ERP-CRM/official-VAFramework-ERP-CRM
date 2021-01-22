/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : ChangeLogProcess
 * Purpose        : Process Change Logs
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           30-Jan-2010
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
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class ChangeLogProcess:ProcessEngine.SvrProcess
    {
    /** The Change Log (when applied directly)		*/
	private int	_VAF_AlterLog_ID = 0;
	
	/** UnDo - Check New Value			*/
	private Boolean? _CheckNewValue = null;
	/** ReDo - Check Old Value			*/
	private Boolean? _CheckOldValue = null;
	/** Set Customization				*/
	private Boolean			_SetCustomization = false;

	/**	The Update Set Command		*/
	private StringBuilder	_sqlUpdate = null;
	/**	The Where Clause			*/
	private StringBuilder	_sqlUpdateWhere = null;
	/**	Is it an insert command		*/
	private Boolean			_isInsert = false;
	/**	The Insert Command			*/
	private StringBuilder	_sqlInsert = null;
	/**	The Insert Value clause		*/
	private StringBuilder	_sqlInsertValue = null;
	
	/** The Table					*/
	private MTable			_table = null;
	/** The Column					*/
	private MColumn 		_column = null;
	/** Old Record ID				*/
	private int				_oldRecord_ID = 0;
	/**	Old Record2 ID				*/
	private String			_oldRecord2_ID = "";
	/** Key Column Name				*/
	private String			_keyColumn = null;
	/** Number of Columns			*/
	private int				_numberColumns = 0;
	/** Array of Columns			*/
	private List<String>	_columns = new List<String>();

	/**	Number of Errors			*/
	private int				_errors = 0;
	/** Number of Failures			*/
	private int 			_checkFailed = 0;
	/** Number of Successes			*/
	private int				_ok = 0;

	
	/// <summary>
	/// Prepare
	/// </summary>
	protected override void Prepare ()
	{
		ProcessInfoParameter[] para = GetParameter();
		for (int i = 0; i < para.Length; i++)
		{
			String name = para[i].GetParameterName();
			if (para[i].GetParameter() == null)
            {
				;
            }
			else if (name.Equals("CheckNewValue"))
            {
				_CheckNewValue =Utility.Util.GetValueOfBool("Y".Equals(para[i].GetParameter()));
            }
			else if (name.Equals("CheckOldValue"))
            {
                _CheckOldValue = Utility.Util.GetValueOfBool("Y".Equals(para[i].GetParameter()));
            }
			else if (name.Equals("SetCustomization"))
            {
				_SetCustomization = "Y".Equals(para[i].GetParameter());
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
		_VAF_AlterLog_ID = GetRecord_ID();
	}	//	prepare

	
	/// <summary>
	/// Doit
	/// </summary>
	/// <returns>Message</returns>
	protected override String DoIt () 
     {
         if (_SetCustomization)
         {
             return SetCustomization();
         }
		
		log.Info("VAF_AlterLog_ID=" + _VAF_AlterLog_ID
			+ ", CheckOldValue=" + _CheckOldValue + ", CheckNewValue=" + _CheckNewValue);
		
		//	Single Change or All Customizations
		String sql = "SELECT * FROM VAF_AlterLog WHERE VAF_AlterLog_ID=@param "
			+ "ORDER BY VAF_TableView_ID, Record_ID, VAF_Column_ID";
        if (_VAF_AlterLog_ID == 0)
        {
            sql = "SELECT * FROM VAF_AlterLog WHERE IsCustomization='Y' AND IsActive='Y' "
                + "ORDER BY VAF_TableView_ID, VAF_AlterLog_ID, Record_ID, VAF_Column_ID";
        }
        SqlParameter[] param = null;
        IDataReader idr = null;
        DataTable dt = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, Get_Trx());
            if (_VAF_AlterLog_ID != 0)
            {
                param = new SqlParameter[1];
                //pstmt.setInt (1, _VAF_AlterLog_ID);
                param[0] = new SqlParameter("@param", _VAF_AlterLog_ID);
            }
            idr = DataBase.DB.ExecuteReader(sql, param, Get_Trx());
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
			foreach(DataRow dr in dt.Rows)
			{
				CreateStatement (new MChangeLog(GetCtx(), dr, Get_Trx()), Get_Trx());
			}
            dt = null;
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
            if (dt != null)
            {
                dt = null;
            }
			log.Log(Level.SEVERE, sql, e);
		}
		//	final call
		ExecuteStatement();
		
		return "@OK@: " + _ok + " - @Errors@: " + _errors + " - @Failed@: " + _checkFailed;
	}	//	doIt

	
	/// <summary>
	/// Create + execute Statement
	/// </summary>
	/// <param name="cLog">log</param>
	/// <param name="trxName">trx</param>
	private void CreateStatement (MChangeLog cLog, Trx trxName)
	{
		//	New Table 
		if (_table != null)
		{
			if (cLog.GetVAF_TableView_ID() != _table.GetVAF_TableView_ID())
			{
				ExecuteStatement();
				_table = null;
			}
		}
        if (_table == null)
        {
            _table = new MTable(GetCtx(), cLog.GetVAF_TableView_ID(), trxName);
        }
		//	New Record
        if (_sqlUpdate != null
            && (cLog.GetRecord_ID() != _oldRecord_ID
                || !cLog.GetRecord2_ID().Equals(_oldRecord2_ID)))
        {
            ExecuteStatement();
        }
		//	Column Info
		_column = new MColumn (GetCtx(), cLog.GetVAF_Column_ID(), Get_Trx());
		//	Same Column twice
        if (_columns.Contains(_column.GetColumnName()))
        {
            ExecuteStatement();
        }
		_columns.Add(_column.GetColumnName());

		//	Create new Statement
		if (_sqlUpdate == null)
		{
			String tableName = _table.GetTableName();
			_keyColumn = _table.GetTableName() + "_ID";
            if (tableName.Equals("VAF_CtrlRef_Table"))
            {
                _keyColumn = "VAF_Control_Ref_ID";
            }
			//
			_sqlUpdate = new StringBuilder ("UPDATE ")
				.Append(tableName)
				.Append(" SET ");
			//	Key
			_sqlUpdateWhere = new StringBuilder (" WHERE ");
            if (cLog.GetRecord_ID() != 0)
            {
                _sqlUpdateWhere.Append(_keyColumn).Append("=").Append(cLog.GetRecord_ID());
            }
            else
            {
                _sqlUpdateWhere.Append(cLog.GetRecord2_ID());
            }
			_oldRecord_ID = cLog.GetRecord_ID();
			_oldRecord2_ID = cLog.GetRecord2_ID();
			
			//	Insert - new value is null and UnDo only
			_isInsert = cLog.IsNewNull() && _CheckNewValue != null;
			if (_isInsert)
			{
				_sqlInsert = new StringBuilder ("INSERT INTO ")
					.Append(tableName).Append("(");
				_sqlInsertValue = new StringBuilder (") VALUES (");
				if (cLog.GetRecord_ID() != 0)
				{
					_sqlInsert.Append(_keyColumn);
					_sqlInsertValue.Append(cLog.GetRecord_ID());
					if (!_keyColumn.Equals(_column.GetColumnName()))
					{
						_sqlInsert.Append(",").Append(_column.GetColumnName());
						_sqlInsertValue.Append(",").Append(GetSQLValue(cLog.GetOldValue()));
					}
				}
			}
			_numberColumns = 1;
		}
		//	Just new Column
		else
		{
			_sqlUpdate.Append(", ");
			//	Insert
			if (_isInsert)
				_isInsert = cLog.IsNewNull();
			if (_isInsert && !_keyColumn.Equals(_column.GetColumnName()))
			{
				_sqlInsert.Append(",").Append(_column.GetColumnName());
				_sqlInsertValue.Append(",").Append(GetSQLValue(cLog.GetOldValue()));
			}
			_numberColumns++;
		}
		
		//	Update Set clause -- columnName=value
		_sqlUpdate.Append(_column.GetColumnName())
			.Append("=");
		//	UnDo a <- (b)
		if (_CheckNewValue != null)
		{
			_sqlUpdate.Append(GetSQLValue(cLog.GetOldValue()));
			//if (_CheckNewValue.booleanValue())
            if (Utility.Util.GetValueOfBool(_CheckNewValue))
			{
				_sqlUpdateWhere.Append(" AND ").Append(_column.GetColumnName());
				String newValue = GetSQLValue(cLog.GetNewValue());
				if (newValue == null || "NULL".Equals(newValue))
					_sqlUpdateWhere.Append(" IS NULL");
				else
					_sqlUpdateWhere.Append("=").Append(newValue);
			}
		}
		//	ReDo (a) -> b
		else if (_CheckOldValue != null)
		{
			_sqlUpdate.Append(GetSQLValue(cLog.GetNewValue()));
			//if (_CheckOldValue.booleanValue())
            if (Utility.Util.GetValueOfBool(_CheckOldValue))
			{
				String newValue = GetSQLValue(cLog.GetOldValue());
				_sqlUpdateWhere.Append(" AND ").Append(_column.GetColumnName());
				if (newValue == null || "NULL".Equals(newValue))
					_sqlUpdateWhere.Append(" IS NULL");
				else
					_sqlUpdateWhere.Append("=").Append(newValue);
			}
		}
	}	//	createStatement

	/// <summary>
	/// Get SQL Value
	/// </summary>
	/// <param name="value">string value</param>
    /// <returns>sql compliant value</returns>
	private String GetSQLValue (String value)
	{
		if (value == null || value.Length == 0 || value.Equals("NULL"))
			return "NULL";
		
		//	Data Types
		if (DisplayType.IsNumeric (_column.GetVAF_Control_Ref_ID())
			|| DisplayType.IsID (_column.GetVAF_Control_Ref_ID()) )
			return value;
		if (DisplayType.YesNo == _column.GetVAF_Control_Ref_ID()) 
		{
			if (value.Equals("true"))
				return "'Y'";
			else
				return "'N'";
		}
		if (DisplayType.IsDate(_column.GetVAF_Control_Ref_ID()) )
			return DataBase.DB.TO_DATE (Convert.ToDateTime(value));

		//	String, etc.
		return DataBase.DB.TO_STRING(value);
	}	//	getSQLValue
	
	
	/// <summary>
	/// Execute Statement
	/// </summary>
	/// <returns>true if OK</returns>
	private Boolean ExecuteStatement()
	{
		if (_sqlUpdate == null)
			return false;
		int no = 0;
		
		//	Insert SQL
		if (_isInsert && _numberColumns > 2)
		{
			_sqlInsert.Append(_sqlInsertValue).Append(")");
			log.Info(_sqlInsert.ToString());
			//
			no = DataBase.DB.ExecuteQuery(_sqlInsert.ToString(),null,Get_Trx());
			if (no == -1)
			{
			//	log.warning("Insert failed - " + _sqlInsert);
				_errors++;
			}
			else if (no == 0)
			{
				log.Warning("Insert failed - " + _sqlInsert);
				_checkFailed++;
			}
			else
				_ok++;
		}
		else	//	Update SQL
		{
			_sqlUpdate.Append(_sqlUpdateWhere);
			log.Info(_sqlUpdate.ToString());
			//
			no = DataBase.DB.ExecuteQuery(_sqlUpdate.ToString(),null, Get_Trx());
			if (no == -1)
			{
			//	log.warning("Failed - " + _sqlUpdate);
				_errors++;
			}
			else if (no == 0)
			{
				log.Warning("Failed - " + _sqlUpdate);
				_checkFailed++;
			}
			else
				_ok++;
		}
		//	Reset
		_sqlUpdate = null;
		_sqlUpdateWhere = null;
		_sqlInsert = null;
		_sqlInsertValue = null;
		_columns = new List<String>();
		return no > 0;
	}	//	executeStatement
	
	/// <summary>
	/// Set Customization Flag
	/// </summary>
	/// <returns>summary</returns>
	private String SetCustomization()
	{
		log.Info("");
		String sql = "UPDATE VAF_AlterLog SET IsCustomization='N' WHERE IsCustomization='Y'";
		int resetNo = DataBase.DB.ExecuteQuery(sql,null, Get_Trx());
		
		int updateNo = 0;
		//	Get Tables
		sql = "SELECT * FROM VAF_TableView t "
		//	Table with EntityType
			+ "WHERE EXISTS (SELECT * FROM VAF_Column c "
				+ "WHERE t.VAF_TableView_ID=c.VAF_TableView_ID AND c.ColumnName='EntityType')"
		//	Changed Tables
			+ " AND EXISTS (SELECT * FROM VAF_AlterLog l "
				+ "WHERE t.VAF_TableView_ID=l.VAF_TableView_ID)";
		StringBuilder update = null;
        IDataReader idr = null;
        DataTable dt = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, Get_Trx());
			//ResultSet rs = pstmt.executeQuery ();
            idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
            dt = new DataTable();
            dt.Load(idr);
            idr.Close();
			foreach(DataRow dr in dt.Rows)
            {
				MTable table = new MTable (GetCtx(),dr, Get_Trx());
				
				String tableName = table.GetTableName();
				String columnName = tableName + "_ID";
				if (tableName.Equals("VAF_CtrlRef_Table"))
					columnName = "VAF_Control_Ref_ID";
				update = new StringBuilder ("UPDATE VAF_AlterLog SET IsCustomization='Y' "
					+ "WHERE VAF_TableView_ID=").Append(table.GetVAF_TableView_ID());
				update.Append (" AND Record_ID IN (SELECT ")
					.Append (columnName)
					.Append (" FROM ").Append(tableName)
					.Append (" WHERE EntityType IN ('D','C'))");
				int no = DataBase.DB.ExecuteQuery(update.ToString(),null, Get_Trx());
				log.Config(table.GetTableName() + " = " + no);
				updateNo += no;
				
			}
            dt = null;
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
            if (dt != null)
            {
                dt=null;
            }
			log.Log(Level.SEVERE, sql + " --- " + update, e);
		}
		return "@Reset@: " + resetNo + " - @Updated@: " + updateNo;
	}	//	setCustomization
	
}	//	ChangeLogProcess

}
