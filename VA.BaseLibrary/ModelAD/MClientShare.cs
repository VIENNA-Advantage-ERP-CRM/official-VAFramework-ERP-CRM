/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MClientShare
 * Purpose        : Client Share Info
 * Class Used     :  X_AD_ClientShare
 * Chronological    Development
 * Deepak           01-Feb-2009
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
    public class MClientShare : X_AD_ClientShare
    {
      /// <summary>
      /// Is Table Client Level Only
	  /// </summary>
      /// <param name="AD_Client_ID">client</param>
      /// <param name="AD_Table_ID">table</param>
      /// <returns>true if client level only (default false)</returns>
	public static bool IsClientLevelOnly (int AD_Client_ID, int AD_Table_ID)
	{
		Boolean? share = IsShared(AD_Client_ID, AD_Table_ID);
		if (share != null)
        {
			return Utility.Util.GetValueOfBool(share);//.booleanValue();
        }
		return false;
	}	//	isClientLevel
	
	/// <summary>
	/// Is Table Org Level Only
	/// </summary>
	/// <param name="AD_Client_ID">client</param>
	/// <param name="AD_Table_ID">table</param>
	/// <returns>true if Org level only (default false)</returns>
	public static bool IsOrgLevelOnly (int AD_Client_ID, int AD_Table_ID)
	{
		Boolean? share = IsShared(AD_Client_ID, AD_Table_ID);
		if (share != null)
        {
			return ! Utility.Util.GetValueOfBool(share);//.booleanValue();
        }
		return false;
	}	//	isOrgLevel

	/// <summary>
	/// Is Table Shared for Client
	/// </summary>
	/// <param name="AD_Client_ID">client</param>
	/// <param name="AD_Table_ID">table</param>
	/// <returns>info or null</returns>
	public static Boolean? IsShared (int AD_Client_ID, int AD_Table_ID)
	{
		//	Load
		if (_shares.IsEmpty())
		{
			String sql = "SELECT AD_Client_ID, AD_Table_ID, ShareType "
				+ "FROM AD_ClientShare WHERE ShareType<>'x' AND IsActive='Y'";
			IDataReader idr=null;
			try
			{
				//pstmt = DataBase.prepareStatement (sql, null);
				idr=DataBase.DB.ExecuteReader(sql,null,null);
				while (idr.Read())
				{
					int Client_ID =Utility.Util.GetValueOfInt(idr[0]);//  rs.getInt(1);
					int table_ID =Utility.Util.GetValueOfInt(idr[1]); //rs.getInt(2);
					String key = Client_ID + "_" + table_ID;
					String ShareType = Utility.Util.GetValueOfString(idr[2]);// rs.getString(3);
					if (ShareType.Equals(SHARETYPE_ClientAllShared))
                    {
						//_shares.put(key, Boolean.TRUE);
                        _shares.Add(key,true);
                    }
					else if (ShareType.Equals(SHARETYPE_OrgNotShared))
                    {
						//_shares.put(key, Boolean.FALSE);
                        _shares.Add(key, false);
                    }
				}
				idr.Close();
			}
			catch (Exception e)
			{
                if(idr!=null)
                {
                    idr.Close();
                }
				_log.Log (Level.SEVERE, sql, e);
			}
			
			if (_shares.IsEmpty())		//	put in something
            {
				//_shares.put("0_0", Boolean.TRUE);
                _shares.Add("0_0", true);
            }
		}	//	load
		String key1 = AD_Client_ID + "_" + AD_Table_ID;
		return (bool?)_shares.Get(key1);// .get(key);
	}	//	load
	
	/**	Shared Info								*/
	private static CCache<String,Boolean>	_shares 
		= new CCache<String,Boolean>("AD_ClientShare", 10, 120);	//	2h
	/**	Logger	*/
	private static VLogger _log = VLogger.GetVLogger (typeof(MClientShare).FullName);//.class);
	
	/// <summary>
	/// Default Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="AD_ClientShare_ID">id</param>
	/// <param name="trxName">trx</param>
	public MClientShare (Ctx ctx, int AD_ClientShare_ID, Trx trxName):base(ctx, AD_ClientShare_ID, trxName)
	{
		
	}	//	MClientShare

    /// <summary>
    /// Default Constructor
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="dr">datarow</param>
    /// <param name="trxName">trx</param>
	public MClientShare(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	}	//	MClientShare
	
	/**	The Table				*/
	private MTable		_table = null;
	
	/// <summary>
	/// Is Client Level Only
	/// </summary>
	/// <returns>true if client level only (shared)</returns>
	public bool IsClientLevelOnly()
	{
		return GetShareType().Equals(SHARETYPE_ClientAllShared);
	}	//	isClientLevelOnly
	
	/// <summary>
	/// Is Org Level Only
	/// </summary>
	/// <returns>true if org level only (not shared)</returns>
	public bool IsOrgLevelOnly()
	{
		return GetShareType().Equals(SHARETYPE_OrgNotShared);
	}	//	isOrgLevelOnly

	/// <summary>
    /// Get Table model
	/// </summary>
	/// <returns>talble</returns>
	public MTable GetTable()
	{
        if (_table == null)
        {
            _table = MTable.Get(GetCtx(), GetAD_Table_ID());
        }
		return _table;
	}	//	getTable
	
	/// <summary>
    /// Get Table Name
	/// </summary>
	/// <returns>table name</returns>
	public String GetTableName()
	{
		return GetTable().GetTableName();
	}	//	getTableName
	
	/// <summary>
	/// After Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <param name="success">success</param>
	/// <returns>true</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (IsActive())
		{
			SetDataToLevel();
			ListChildRecords();
		}
		return true;
	}	//	afterSave
	
	/// <summary>
	/// Set Data To Level
	/// </summary>
	/// <returns>info</returns>
	public String SetDataToLevel()
	{
		String info = "-";
		if (IsClientLevelOnly())
		{
			StringBuilder sql = new StringBuilder("UPDATE ")
				.Append(GetTableName())
				.Append(" SET AD_Org_ID=0 WHERE AD_Org_ID<>0 AND AD_Client_ID=@param1");
            SqlParameter[] param = new SqlParameter[1];
            param[0]=new SqlParameter("@param1", GetAD_Client_ID());
			int no = DataBase.DB.ExecuteQuery(sql.ToString(),param, Get_TrxName());
			info = GetTableName() + " set to Shared #" + no;
			log.Info(info);
		}
		else if (IsOrgLevelOnly())
		{
			StringBuilder sql = new StringBuilder("SELECT COUNT(*) FROM ")
				.Append(GetTableName())
                .Append(" WHERE AD_Org_ID=0 WHERE AD_Client_ID=").Append(GetAD_Client_ID());
            
            
			int no = DataBase.DB.GetSQLValue(Get_TrxName(), sql.ToString());
           
			info = GetTableName() + " Shared records #" + no;
			log.Info(info);
		}
		return info;
	}	//	setDataToLevel

	/// <summary>
	/// List Child Tables
	/// </summary>
	/// <returns>child tables</returns>
	public String ListChildRecords()
	{
		StringBuilder info = new StringBuilder();
		String sql = "SELECT AD_Table_ID, TableName "
			+ "FROM AD_Table t "
			+ "WHERE AccessLevel='3' AND IsView='N'"  //jz put quote for typing
			+ " AND EXISTS (SELECT * FROM AD_Column c "
				+ "WHERE t.AD_Table_ID=c.AD_Table_ID"
				+ " AND c.IsParent='Y'"
				+ " AND c.ColumnName IN (SELECT ColumnName FROM AD_Column cc "
					+ "WHERE cc.IsKey='Y' AND cc.AD_Table_ID=@param))";
        SqlParameter[] param = new SqlParameter[1];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);
			//pstmt.setInt (1, getAD_Table_ID());
            param[0] = new SqlParameter("@param", GetAD_Table_ID());
            idr = DataBase.DB.ExecuteReader(sql, param, null);
			while (idr.Read())
			{
                int AD_Table_ID = Utility.Util.GetValueOfInt(idr[0]);// rs.getInt(1);
                String TableName = Utility.Util.GetValueOfString(idr[1]);// rs.getString(2);
                if (info.Length != 0)
                {
                    info.Append(", ");
                }
				info.Append(TableName);
			}
            idr.Close();
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			log.Log(Level.SEVERE, sql, e);
		}
		
		log.Info(info.ToString());
		return info.ToString();
	}	//	listChildRecords
	
	/// <summary>
	/// Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected override bool BeforeSave (bool newRecord)
	{
        if (GetAD_Org_ID() != 0)
        {
            SetAD_Org_ID(0);
        }
		return true;
	}	//	beforeSave

}	//	MClientShare

}
