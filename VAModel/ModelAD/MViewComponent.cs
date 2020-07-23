/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MViewComponent
 * Purpose        : Database View Component Model
 * Class Used     : X_AD_ViewComponent
 * Chronological    Development
 * Deepak           14-Jan-2010
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

namespace VAdvantage.Model
{
    public class MViewComponent:X_AD_ViewComponent
    {    
	///private static  long serialVersionUID = 1L;
	/// <summary>
	/// Standard Constructor	
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="AD_ViewComponent_ID">id</param>
	/// <param name="trxName">trx</param>
	public MViewComponent(Ctx ctx, int AD_ViewComponent_ID,
        Trx trxName)
        : base(ctx, AD_ViewComponent_ID, trxName)
	{
		
	}	//	MViewComponent

	/// <summary>
	/// Load Constructor	 
	/// </summary>
	/// <param name="ctx"></param>
	/// <param name="rs"></param>
	/// <param name="trxName"></param>
	public MViewComponent(Ctx ctx,DataRow rs, Trx trxName):base(ctx,rs,trxName)
	{
		
	}	//	MViewComponent
	public MViewComponent(Ctx ctx,IDataReader rs, Trx trxName):base(ctx,rs,trxName)
    {}
	/// <summary>
	/// Parent Constructor	
	/// </summary>
	/// <param name="parent">parent</param>
	public MViewComponent(MTable parent):this (parent.GetCtx(), 0, parent.Get_TrxName())
	{		       
		SetClientOrg (parent);
		SetAD_Table_ID (parent.GetAD_Table_ID());
	}	//	MViewComponent

	//The Columns
	private MViewColumn[]		_columns = null;
	
	/// <summary>
	/// Get Columns	
	/// </summary>
	/// <param name="reload">reload data</param>
	/// <returns>array of columns</returns>
	public MViewColumn[] GetColumns(bool reload)
	{
		if (_columns != null && !reload)
			return _columns;
		String sql = "SELECT * FROM AD_ViewColumn WHERE AD_ViewComponent_ID=@param ORDER BY AD_ViewColumn_ID";
		List<MViewColumn> list = new List<MViewColumn>();
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		try
		{
            param[0]=new SqlParameter("@param",GetAD_ViewComponent_ID());			
			idr=DataBase.DB.ExecuteReader(sql,param,Get_TrxName());			
			while(idr.Read())
			{
				list.Add (new MViewColumn (GetCtx(),idr, Get_TrxName ()));
			}
			idr.Close();
		}
		catch (Exception e)
		{
            if(idr!=null)
            {
                idr.Close();
            }
			log.Log (Level.SEVERE, sql, e);
		}		
		//
		_columns = new MViewColumn[list.Count];
        _columns=list.ToArray();
		return _columns;
	} //	getColumns

	/// <summary>
	/// Get SQL Select	
	/// </summary>
	/// <param name="requery">refresh columns</param>
	/// <param name="vCols">model MViewColumn array</param>
	/// <returns>selest statement</returns>
	public String GetSelect(bool requery, MViewColumn[] vCols)
	{
		GetColumns(requery);
		if (_columns == null || _columns.Length == 0)
			return null;
		
		if (vCols == null)
			vCols = _columns;
		
		StringBuilder sb = new StringBuilder("SELECT ");
		//

		for (int i = 0; i < vCols.Length; i++)
		//	for (int i = 0; i < m_columns.length; i++)
		{
			String colName = vCols[i].GetColumnName();
			MViewColumn vc = null;
			foreach (MViewColumn element in _columns) 
            {
				if (element.GetColumnName().Equals(colName))
				{
					vc = element;
					break;
				}
			}
			if (i>0)
				sb.Append(", ");
			String colSQL = vc.GetColumnSQL();
			//String colName = vc.getColumnName();
			
			if (colSQL == null || colSQL.ToUpper().Equals("NULL"))
			{
				String dt = vc.GetDBDataType();
				if (dt!=null)
				{
					if (dt.Equals(X_AD_ViewColumn.DBDATATYPE_CharacterFixed) || 
							dt.Equals(X_AD_ViewColumn.DBDATATYPE_CharacterVariable))
							colSQL = "NULLIF('a','a')";
					else
					if (dt.Equals(X_AD_ViewColumn.DBDATATYPE_Decimal) || 
							dt.Equals(X_AD_ViewColumn.DBDATATYPE_Integer) ||
							dt.Equals(X_AD_ViewColumn.DBDATATYPE_Number))
							colSQL = "NULLIF(1,1)";
					else
					if (dt.Equals(X_AD_ViewColumn.DBDATATYPE_Timestamp))
						colSQL = "NULL";
				}
				else
					colSQL = "NULL";
			}
			
			sb.Append(colSQL);
			if (!colName.Equals("*"))
				sb.Append(" AS ").Append(colName);
		}
		
		sb.Append(" ").Append(GetFromClause());
		String t = GetWhereClause();
		if (t!=null && t.Length>0)
			sb.Append(" ").Append(t);
		t = GetOtherClause();
		if (t!=null && t.Length>0)
			sb.Append(" ").Append(t);
		
		return sb.ToString();
	}	//	getViewCreate
	

   /// <summary>
   /// 	String Representation       
   /// </summary>
   /// <returns>info</returns>
	public override String ToString()
    {
	    StringBuilder sb = new StringBuilder("MViewComponent[")
	    	.Append(Get_ID())
	        .Append("-").Append(GetName());
	    sb.Append("]");
	    return sb.ToString();
    }
    }
}
