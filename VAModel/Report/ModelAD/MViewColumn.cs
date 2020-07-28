/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MViewColumn
 * Purpose        : Database View Column Model
 * Class Used     : X_AD_ViewColumn
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
    public class MViewColumn : X_AD_ViewColumn
    {      
	//private static long serialVersionUID = 1L;
    /// <summary>
    /// Standard Constructor	
    /// </summary>
    /// <param name="ctx">context</param>
    /// <param name="AD_ViewColumn_ID">view</param>
    /// <param name="trxName">trx</param>    
	public MViewColumn(Ctx ctx, int AD_ViewColumn_ID, Trx trxName):base(ctx, AD_ViewColumn_ID, trxName)
	{
		
	}

	/// <summary>
	/// Load Constructor	
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	public MViewColumn(Ctx ctx, DataRow rs, Trx trxName):base(ctx,rs,trxName)
	{		
	}
    public MViewColumn(Ctx ctx, IDataReader rs, Trx trxName)
        : base(ctx, rs, trxName)
    {
    }
        /// <summary>
        /// 	Parent Constructor 
        /// </summary>
        /// <param name="parent">parent</param>
	public MViewColumn (MViewComponent parent):this (parent.GetCtx(), 0, parent.Get_TrxName())
	{		
		SetClientOrg (parent);
		SetAD_ViewComponent_ID (parent.GetAD_ViewComponent_ID());
	}	//	MViewColumn

	
    /// <summary>
    /// 	String Representation
    /// </summary>
    /// <returns>info</returns>
	public override String ToString()
    {
	    StringBuilder sb = new StringBuilder("MViewColumn[")
	    	.Append(Get_ID())
	        .Append("-").Append(GetColumnName());
	    sb.Append("]");
	    return sb.ToString();
    }
    }
}
