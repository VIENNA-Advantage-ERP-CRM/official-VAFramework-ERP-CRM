/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIssueSystem
 * Purpose        : Issue System Model
 * Class Used     : X_R_IssueSystem
 * Chronological    Development
 * Deepak           27-Jan-2010
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
    public class MIssueSystem : X_R_IssueSystem
    {
   /// <summary>
   /// Get/Set System
   /// </summary>
   /// <param name="issue">issue</param>
   /// <returns>system</returns>
	static public MIssueSystem Get(MIssue issue)
	{
		if (issue.GetDBAddress() == null)
        {
			return null;
        }
		MIssueSystem system = null;
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		String sql = "SELECT * FROM R_IssueSystem WHERE DBAddress=@param";
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);
			//pstmt.setString (1, issue.getDBAddress());
			param[0]=new SqlParameter("@param",issue.GetDBAddress());
            idr=DataBase.DB.ExecuteReader(sql,param,null);
			if (idr.Read())
            {
				system = new MIssueSystem(issue.GetCtx(),idr, null);
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
	
		//	New
		if (system == null)
		{
			system = new MIssueSystem(issue.GetCtx(), 0, null);
			system.SetDBAddress(issue.GetDBAddress());
			system.SetA_Asset_ID(issue.GetA_Asset_ID());
		}
		system.SetSystemStatus(issue.GetSystemStatus());
		system.SetStatisticsInfo(issue.GetStatisticsInfo());
		system.SetProfileInfo(issue.GetProfileInfo());
		if (issue.GetA_Asset_ID() != 0 
			&& system.GetA_Asset_ID() != issue.GetA_Asset_ID())
			system.SetA_Asset_ID(issue.GetA_Asset_ID());
		//
		if (!system.Save())
        {
			return null;
        }
		
		//	Set 
		issue.SetR_IssueSystem_ID(system.GetR_IssueSystem_ID());
		if (system.GetA_Asset_ID() != 0)
        {
			issue.SetA_Asset_ID(system.GetA_Asset_ID());
        }
		return system;
	}	//	get
	
	/**	Logger	*/
	private static VLogger _log = VLogger.GetVLogger (typeof(MIssueSystem).FullName);
	
	/// <summary>
	/// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="R_IssueSystem_ID">id</param>
	/// <param name="trxName">trx</param>
	public MIssueSystem (Ctx ctx, int R_IssueSystem_ID, Trx trxName):base (ctx, R_IssueSystem_ID, trxName)
	{
		//super (ctx, R_IssueSystem_ID, trxName);
	}	//	MIssueSystem

	/// <summary>
    ///	Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MIssueSystem (Ctx ctx, DataRow dr, Trx trxName):base(ctx,dr,trxName)
	{
		//super (ctx, rs, trxName);
	}	//	MIssueSystem
    public MIssueSystem(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }
	
	/// <summary>
	///	String Representation
	/// </summary>
    /// <returns>info</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MIssueSystem[");
		sb.Append(Get_ID())
			.Append ("-").Append (GetDBAddress())
			.Append(",A_Asset_ID=").Append(GetA_Asset_ID())
			.Append ("]");
		return sb.ToString ();
	}	//	toString
}	//	MIssueSystem

}
