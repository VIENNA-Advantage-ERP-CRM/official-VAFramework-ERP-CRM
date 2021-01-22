/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MImpFormatRow
 * Purpose        : Import Format Row Model
 * Class Used     : X_VAF_ImportFormat_Row
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
    public class MImpFormatRow: X_VAF_ImportFormat_Row
    {

	/// <summary>
	///Standard Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="VAF_ImportFormat_Row_ID">id</param>
	/// <param name="trxName">trx</param>
	public MImpFormatRow (Ctx ctx, int VAF_ImportFormat_Row_ID, Trx trxName):base(ctx, VAF_ImportFormat_Row_ID, trxName)
	{
	
		if (VAF_ImportFormat_Row_ID == 0)
		{
		//	setVAF_ImportFormat_ID (0);		Parent
		//	setVAF_Column_ID (0);
		//	setDataType (null);
		//	setName (null);
		//	setSeqNo (10);
			SetDecimalPoint (".");
			SetDivideBy100 (false);
		}
	}	//	MImpFormatRow

	/// <summary>
	/// Load Construcor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">trx</param>
	
    public MImpFormatRow (Ctx ctx,DataRow dr, Trx trxName):base(ctx, dr, trxName)
	{
		
	}	//	MImpFormatRow
    public MImpFormatRow(Ctx ctx,IDataReader idr, Trx trxName)
        : base(ctx,idr, trxName)
    {

    }
	/// <summary>
	/// Parent Construcor
	/// </summary>
    /// <param name="format">format parent</param>
	public MImpFormatRow (MImpFormat format):this (format.GetCtx(), 0, format.Get_TrxName())
	{
		
		SetVAF_ImportFormat_ID(format.GetVAF_ImportFormat_ID());
	}	//	MImpFormatRow
	
	/// <summary>
	/// Parent/Copy Construcor
	/// </summary>
	/// <param name="parent">format parent</param>
	/// <param name="original">to copy</param>
	public MImpFormatRow (MImpFormat parent, MImpFormatRow original):this (parent.GetCtx(), 0, parent.Get_TrxName())
	{
		
		CopyValues(original, this);
		SetClientOrg(parent);
		SetVAF_ImportFormat_ID(parent.GetVAF_ImportFormat_ID());
	}	//	MImpFormatRow
	
}	


}
