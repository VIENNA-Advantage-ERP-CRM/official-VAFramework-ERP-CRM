/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MNewsItem
 * Purpose        :  News ItemModel
 * Class Used     : X_CM_NewsItem
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
    public class MNewsItem : X_CM_NewsItem
{
	/// <summary>
    ///  Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="CM_NewsItem_ID">id</param>
	/// <param name="trxName">trx</param>
	public MNewsItem(Ctx ctx, int CM_NewsItem_ID, Trx trxName):base (ctx, CM_NewsItem_ID, trxName)
	{
		
	}	// MNewsItem

	/// <summary>
    /// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MNewsItem (Ctx ctx,DataRow dr, Trx trxName):base (ctx, dr, trxName)
	{
		
	} // MNewsItem
    public MNewsItem(Ctx ctx, IDataReader idr, Trx trxName)
        : base(ctx,idr, trxName)
    { }
	
	/// <summary>
    /// getNewsChannel
	/// </summary>
	/// <returns>news Chennel</returns>
	public MNewsChannel GetNewsChannel() 
	{
		int[] thisNewsChannel = MNewsChannel.GetAllIDs("CM_NewsChannel","CM_NewsChannel_ID=" + this.GetCM_NewsChannel_ID(), Get_TrxName());
		if (thisNewsChannel!=null) 
		{
			if (thisNewsChannel.Length==1)
				return new MNewsChannel(GetCtx(), thisNewsChannel[0], Get_TrxName());
		}
		return null;
	} // getNewsChannel

	/// <summary>
	/// Get rss2 Item Code
	/// </summary>
	/// <param name="xmlCode">xml</param>
	/// <param name="thisChannel">channel</param>
	/// <returns>rss item code</returns>
	public StringBuilder Get_rss2ItemCode(StringBuilder xmlCode, MNewsChannel thisChannel) 
	{
		if (this != null)	//	never null ??
		{
			xmlCode.Append ("<item>");
			xmlCode.Append ("<CM_NewsItem_ID>"+ this.Get_ID() + "</CM_NewsItem_ID>");
			xmlCode.Append ("  <title><![CDATA["
				+ this.GetTitle () + "]]></title>");
			xmlCode.Append ("  <description><![CDATA["
				+ this.GetDescription ()
				+ "]]></description>");
			xmlCode.Append ("  <content><![CDATA["
				+ this.GetContentHTML ()
				+ "]]></content>");
			xmlCode.Append ("  <link>"
				+ thisChannel.GetLink ()
				+ "?CM_NewsItem_ID=" + this.Get_ID() + "</link>");
			xmlCode.Append ("  <author><![CDATA["
				+ this.GetAuthor () + "]]></author>");
			xmlCode.Append ("  <pubDate>"
				+ this.GetPubDate () + "</pubDate>");
			xmlCode.Append ("</item>");
		}
		return xmlCode;
	}
	
	/// <summary>
    /// After Save.
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <param name="success">success</param>
	/// <returns>true if inserted</returns>
	protected override bool AfterSave (bool newRecord, bool success)
	{
		if (!success)
			return success;
		if (!newRecord)
		{
			MIndex.CleanUp(Get_TrxName(), GetAD_Client_ID(), Get_Table_ID(), Get_ID());
		}
		ReIndex(newRecord);
		return success;
	}	//	afterSave
	
	/// <summary>
    /// reIndex
	/// </summary>
	/// <param name="newRecord">new record</param>
	public void ReIndex(bool newRecord)
	{
		int CMWebProjectID = 0;
		if (GetNewsChannel()!=null)
			CMWebProjectID = GetNewsChannel().GetCM_WebProject_ID();
		String [] toBeIndexed = new String[4];
		toBeIndexed[0] = this.GetAuthor();
		toBeIndexed[1] = this.GetDescription();
		toBeIndexed[2] = this.GetTitle();
		toBeIndexed[3] = this.GetContentHTML();
		MIndex.ReIndex (newRecord, toBeIndexed, GetCtx(), GetAD_Client_ID(), Get_Table_ID(), Get_ID(), CMWebProjectID, this.GetUpdated());
	} // reIndex
}

}
