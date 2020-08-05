/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : TranslationDocSync
 * Purpose        : Document Translation Sync
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           01-Dec-2009
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
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class TranslationDocSync : ProcessEngine.SvrProcess
    {
    /// <summary>
    /// Prepare - e.g., get Parameters.
	/// </summary>
	protected override void Prepare()
	{
		ProcessInfoParameter[] para = GetParameter();
		for (int i = 0; i < para.Length; i++)
		{
			String name = para[i].GetParameterName();
			if (para[i].GetParameter() == null)
            {
				;
            }
			else
            {
				log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
		}
	}	//	prepare

	/// <summary>
	/// Perrform Process.
	/// </summary>
	/// <returns>Message</returns>
	protected override String DoIt()
	{
		MClient client = MClient.Get(GetCtx());
        if (client.IsMultiLingualDocument())
        {
            throw new Exception("@AD_Client_ID@: @IsMultiLingualDocument@");
        }
		//
		log.Info("" + client);
		String sql = "SELECT * FROM AD_Table "
			+ "WHERE TableName LIKE '%_Trl' AND TableName NOT LIKE 'AD%' "
			+ "ORDER BY TableName";
		//PreparedStatement pstmt = null;
        IDataReader idr = null;
        DataTable dt = null;
        try
        {
            //pstmt = DataBase.prepareStatement (sql, Get_Trx());
            //ResultSet rs = pstmt.executeQuery ();
            idr = DataBase.DB.ExecuteReader(sql, null, Get_Trx());
            dt = new DataTable();
            dt.Load(idr);
            foreach (DataRow dr in dt.Rows)
            {
                ProcessTable(new MTable(GetCtx(), dr, null), client.GetAD_Client_ID());
            }

        }
        catch (Exception e)
        {
            log.Log(Level.SEVERE, sql, e);
        }
        finally
        {
            dt = null;
            idr.Close();
        }
		return "OK";
	}	//	doIt
	
	/// <summary>
	/// Process Translation Table
	/// </summary>
	/// <param name="table">table</param>
	/// <param name="AD_Client_ID">AD_Client_ID</param>
	private void ProcessTable (MTable table, int AD_Client_ID)
	{
		StringBuilder sql = new StringBuilder();
		MColumn[] columns = table.GetColumns(false);
		for (int i = 0; i < columns.Length; i++)
		{
			MColumn column = columns[i];
			if (column.GetAD_Reference_ID() == DisplayType.String
				|| column.GetAD_Reference_ID() == DisplayType.Text)
			{
				String columnName = column.GetColumnName();
                if (sql.Length != 0)
                {
                    sql.Append(",");
                }
				sql.Append(columnName);
			}
		}
		String baseTable = table.GetTableName();
		baseTable = baseTable.Substring(0, baseTable.Length-4);
		
		log.Config(baseTable + ": " + sql);
		String columnNames = sql.ToString();
		
		sql = new StringBuilder();
		sql.Append("UPDATE ").Append(table.GetTableName()).Append(" t SET (")
			.Append(columnNames).Append(") = (SELECT ").Append(columnNames)
			.Append(" FROM ").Append(baseTable).Append(" b WHERE t.")
			.Append(baseTable).Append("_ID=b.").Append(baseTable).Append("_ID) WHERE AD_Client_ID=")
			.Append(AD_Client_ID);
		int no = DataBase.DB.ExecuteQuery(sql.ToString(),null,Get_Trx());
		AddLog(0, null, new Decimal(no), baseTable);
	}	//	processTable
	
}	//	TranslationDocSync

}
