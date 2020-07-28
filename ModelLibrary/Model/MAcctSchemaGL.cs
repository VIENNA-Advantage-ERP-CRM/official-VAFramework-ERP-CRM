/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAcctSchemaGL
 * Purpose        : Accounting Schema GL info
 * Class Used     : X_C_AcctSchema_GL
 * Chronological    Development
 * Deepak           23-Nov-2009
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
using System.Data.SqlClient;
namespace VAdvantage.Model
{
    public class MAcctSchemaGL : X_C_AcctSchema_GL
    {
        /**	Logger							*/
	protected static VLogger			_log = VLogger.GetVLogger(typeof(MAcctSchemaGL).FullName);
    /// <summary>
    /// Get Accounting Schema GL Info
	/// </summary>
    /// <param name="ctx">context</param>
    /// <param name="C_AcctSchema_ID">id</param>
    /// <returns>defaults</returns>
	public static MAcctSchemaGL Get(Ctx ctx, int C_AcctSchema_ID)
	{
		MAcctSchemaGL retValue = null;
		String sql = "SELECT * FROM C_AcctSchema_GL WHERE C_AcctSchema_ID=@Param1";
        SqlParameter[] Param=new SqlParameter[1];
        IDataReader idr=null;
        DataTable dt=null;
		//PreparedStatement pstmt = null;
		try 
		{
            Param[0]=new SqlParameter("@Param1",C_AcctSchema_ID);
			//pstmt = DataBase.prepareStatement(sql, null);
			//pstmt.setInt(1, C_AcctSchema_ID);
            idr=DataBase.DB.ExecuteReader(sql,Param,null);
            dt=new DataTable();
            dt.Load(idr);
			//ResultSet rs = pstmt.executeQuery();
            idr.Close();
            foreach(DataRow dr in dt.Rows)
			{
				retValue = new MAcctSchemaGL (ctx, dr, null);
			}
			
		}
		catch (Exception e)
		{
            if (idr != null)
            {
                idr.Close();
            }
			_log.Log(Level.SEVERE, sql, e);
		}
		finally
        {
            if (idr != null)
            {
                idr.Close();
            }
            dt = null;
        }
		return retValue;
	}	//	get
	
	/// <summary>
    /// Load Constructor
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="C_AcctSchema_ID">AcctSchema </param>
    /// <param name="trxName">transaction</param>
	public MAcctSchemaGL (Ctx ctx, int C_AcctSchema_ID, Trx trxName):base(ctx, C_AcctSchema_ID, trxName)
	{
		//super(ctx, C_AcctSchema_ID, trxName);
		if (C_AcctSchema_ID == 0)
		{
			SetUseCurrencyBalancing(false);
			SetUseSuspenseBalancing(false);
			SetUseSuspenseError(false);
		}
	}	//	MAcctSchemaGL

	/// <summary>
	/// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
    /// <param name="trxName">transaction</param>
	public MAcctSchemaGL(Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		//super(ctx, rs, trxName);
	}	//	MAcctSchemaGL

	/// <summary>
	/// 	Get Acct Info list 
	/// </summary>
    /// <returns>list</returns>
	//public ArrayList<KeyNamePair> getAcctInfo()
    public List<KeyNamePair> GetAcctInfo()
	{
		//ArrayList<KeyNamePair> list = new ArrayList<KeyNamePair>();
        List<KeyNamePair> list = new List<KeyNamePair>();
		for (int i = 0; i < Get_ColumnCount(); i++)
		{
			String columnName = Get_ColumnName(i);
			if (columnName.EndsWith("Acct"))
			{
				int id =Utility.Util.GetValueOfInt(Get_Value(i));
				list.Add(new KeyNamePair (id, columnName));
			}
		}
		return list;
	}	//	getAcctInfo
	
	/// <summary>
	/// Set Value (don't use)
	/// </summary>
	/// <param name="columnName">column name</param>
    /// <param name="value">value</param>
	/// <returns>true if set</returns>
	public Boolean SetValue(String columnName,int value)
	{
		return base.Set_Value (columnName, value);
	}	//	setValue
	
	/// <summary>
	/// Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected Boolean beforeSave (Boolean newRecord)
	{
        if (GetAD_Org_ID() != 0)
        {
            SetAD_Org_ID(0);
        }
		return true;
	}	//	beforeSave

}	//	MAcctSchemaGL

   
}
