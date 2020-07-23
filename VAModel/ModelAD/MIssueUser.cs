/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIssueUser
 * Purpose        : Issue User Model
 * Class Used     : X_R_IssueUser
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
    public class MIssueUser : X_R_IssueUser
    {
   /// <summary>
   /// Get/Set User for Issue
   /// </summary>
   /// <param name="issue">issue</param>
   /// <returns>user</returns>
	static public MIssueUser Get(MIssue issue)
	{
		if (issue.GetUserName() == null)
        {
			return null;
        }
		MIssueUser user = null;
		//	Find Issue User
		String sql = "SELECT * FROM R_IssueUser WHERE UserName=@param";
		SqlParameter[] param=new SqlParameter[1];
        IDataReader idr=null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, null);
			//pstmt.setString (1, issue.getUserName());
            param[0]=new SqlParameter("@param",issue.GetUserName());
			idr=DataBase.DB.ExecuteReader(sql,param,null);
			if (idr.Read())
            {
				user = new MIssueUser (issue.GetCtx(), idr, null);
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
		if (user == null)
		{
			user = new MIssueUser(issue.GetCtx(), 0, null);
			user.SetUserName(issue.GetUserName());
			user.SetAD_User_ID();
			if (!user.Save())
            {
				return null;
            }
		}
		
		issue.SetR_IssueUser_ID(user.GetR_IssueUser_ID());
		return user;
	}	//	MIssueUser
	
	/**	Logger	*/
    private static VLogger _log = VLogger.GetVLogger(typeof(MIssueUser).FullName);
	
	    /// <summary>
        ///Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="R_IssueUser_ID">id</param>
        /// <param name="trxName">trx</param>
	public MIssueUser (Ctx ctx, int R_IssueUser_ID, Trx trxName):base(ctx, R_IssueUser_ID, trxName)
	{
		//super (ctx, R_IssueUser_ID, trxName);
	}	//	MIssueUser

	/// <summary>
    /// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MIssueUser (Ctx ctx, DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	}	//	MIssueUser

    public MIssueUser(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }
	/// <summary>
    ///	Set AD_User_ID
	/// </summary>
	public void SetAD_User_ID ()
	{
		int AD_User_ID = DataBase.DB.GetSQLValue(null, 
			"SELECT AD_User_ID FROM AD_User WHERE EMail=@param1",Utility.Util.GetValueOfInt(GetUserName()));
        if (AD_User_ID != 0)
        {
            base.SetAD_User_ID(AD_User_ID);
        }
	}	//	setAD_User_ID
	
	/// <summary>
	///	String Representation
	/// </summary>
    /// <returns>info</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MIssueUser[");
		sb.Append (Get_ID())
			.Append ("-").Append(GetUserName())
			.Append(",AD_User_ID=").Append(GetAD_User_ID())
			.Append ("]");
		return sb.ToString ();
	}	//	toString
}	//	MIssueUser

}
