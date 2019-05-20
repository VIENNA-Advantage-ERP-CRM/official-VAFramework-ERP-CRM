/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MIndexLog 
 * Purpose        : Text Search Log record
 * Class Used     :  X_K_IndexLog
 * Chronological    Development
 * Deepak           05-Feb-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using VAdvantage.Utility;
using VAdvantage.DataBase;
namespace VAdvantage.Model
{
   public class MIndexLog : X_K_IndexLog
{
	/// <summary>
    /// standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="K_IndexLog_ID">id</param>
	/// <param name="trxName">trx</param>
	public MIndexLog (Ctx ctx, int K_IndexLog_ID, Trx trxName):base(ctx, K_IndexLog_ID, trxName)
	{
		
	}	//	MIndexStop

	/// <summary>
    /// Load Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="dr">datarow</param>
	/// <param name="trxName">trx</param>
	public MIndexLog (Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	}	//	MIndexStop
    public MIndexLog(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx, idr, trxName)
    { }
}	
}
