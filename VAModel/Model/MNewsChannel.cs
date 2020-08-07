/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MNewsChannel
 * Purpose        : News Channel Model
 * Class Used     : X_CM_NewsChannel
 * Chronological    Development
 * Deepak           05-Feb-2010
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
    public class MNewsChannel: X_CM_NewsChannel
{
	/// <summary>
    /// Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_NewsChannel_ID">id</param>
	/// <param name="trxName">trx</param>
	public MNewsChannel (Ctx ctx, int CM_NewsChannel_ID, Trx trxName):base(ctx, CM_NewsChannel_ID, trxName)
    {
		
	} // MNewsChannel
    /// <summary>
    /// Load Constructor
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="rs">id</param>
    /// <param name="trxName">yrx</param>
	public MNewsChannel (Ctx ctx, DataRow dr, Trx trxName):base (ctx, dr, trxName)
	{
		
	}
    public MNewsChannel(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }
	
	/// <summary>
	/// Get News Items
	/// </summary>
	/// <param name="where">where clause</param>
	/// <returns>array of news items</returns>
	public MNewsItem[] GetNewsItems(String where)
	{
		List<MNewsItem> list = new List<MNewsItem>();
		String sql = "SELECT * FROM CM_NewsItem WHERE CM_NewsChannel_ID=@param AND IsActive='Y'";
        if (where != null && where.Length > 0)
        {
            sql += " AND " + where;
        }
		sql += " ORDER BY pubDate DESC";
        SqlParameter[] param = new SqlParameter[1];
        IDataReader idr = null;
		try
		{
			//pstmt = DataBase.prepareStatement (sql, this.get_TrxName());
			//pstmt.setInt (1, this.get_ID());
            param[0] = new SqlParameter("@param", this.Get_ID());
            idr = DataBase.DB.ExecuteReader(sql, param, this.Get_TrxName());
            while (idr.Read())
            {
                list.Add(new MNewsItem(this.GetCtx(), idr, this.Get_TrxName()));
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
		
		MNewsItem[] retValue = new MNewsItem[list.Count];
        retValue=list.ToArray();
		return retValue;
	}	//	getNewsItems


	
	/// <summary>
	/// Get rss2 Channel Code
	/// </summary>
	/// <param name="xmlCode">xml</param>
	/// <param name="showFutureItems">future</param>
	/// <returns>channel code</returns>
	public StringBuilder Get_rss2ChannelCode(StringBuilder xmlCode, bool showFutureItems) 
	{
		if (this != null)	//	never null ??
		{
			xmlCode.Append ("<channel>");
			xmlCode.Append ("  <title><![CDATA[" + this.GetName ()
				+ "]]></title>");
			xmlCode.Append ("  <link>" + this.GetLink ()
				+ "</link>");
			xmlCode.Append ("  <description><![CDATA["
				+ this.GetDescription () + "]]></description>");
			xmlCode.Append ("  <language>"
				+ this.GetAD_Language () + "</language>");
			xmlCode.Append ("  <copyright>" + "" + "</copyright>");
			xmlCode.Append ("  <pubDate>"
				+ this.GetCreated () + "</pubDate>");
			xmlCode.Append ("  <image>");
			xmlCode.Append ("    <url>" + "" + "</url>");
			xmlCode.Append ("    <title><![CDATA[" + "" + "]]></title>");
			xmlCode.Append ("    <link>" + "" + "</link>");
			xmlCode.Append ("  </image>");

			String whereClause = "";
			//jz if (!showFutureItems) whereClause = "sysdate>pubdate"; 
			if (!showFutureItems) whereClause = " SYSDATE > pubdate"; 
			MNewsItem[] theseItems = GetNewsItems(whereClause);
				
			for(int i=0;i<theseItems.Length;i++) 
				xmlCode=theseItems[i].Get_rss2ItemCode(xmlCode,this);
			xmlCode.Append ("</channel>");
		}
		return xmlCode;
	}

	
	/// <summary>
    /// After Save.
	/// </summary>
	/// <param name="newRecord">new record</param>
	/// <param name="success">success</param>
	/// <returns>true if inserted</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		ReIndex(newRecord);
		return success;
	}	//	afterSave
	

	public void ReIndex(bool newRecord) 
	{
		String[] toBeIndexed = new String[2];
		toBeIndexed[0] = this.GetName();
		toBeIndexed[1] = this.GetDescription();
		MIndex.ReIndex (newRecord, toBeIndexed, GetCtx(), GetAD_Client_ID(), Get_Table_ID(), Get_ID(), GetCM_WebProject_ID(), this.GetUpdated());
	}
}	//	

}
