/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportColumn
 * Purpose        : Report Column Model
 * Class Used     : X_VAPA_FR_ColumnSet
 * Chronological    Development
 * Deepak           18-Jan-2010
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

namespace VAdvantage.Report
{
    public class MReportColumn: VAPA_FR_Column
	{
   /// <summary>
   /// 	Copy
    /// </summary>
   /// <param name="ctx">context</param>
   /// <param name="VAF_Client_ID">parent</param>
   /// <param name="VAF_Org_ID">parent</param>
   /// <param name="VAPA_FR_ColumnSet_ID">parent</param>
   /// <param name="source">copy source</param>
   /// <param name="trxName">transaction</param>
   /// <returns>report column</returns>
	public static MReportColumn Copy (Ctx ctx, int VAF_Client_ID, int VAF_Org_ID, 
		int VAPA_FR_ColumnSet_ID, MReportColumn source, Trx trxName)
	{
		MReportColumn retValue = new MReportColumn (ctx, 0, trxName);
		MReportColumn.CopyValues(source, retValue, VAF_Client_ID, VAF_Org_ID);
		//
		retValue.SetVAPA_FR_ColumnSet_ID(VAPA_FR_ColumnSet_ID);	//	parent
		retValue.SetOper_1_ID(0);
		retValue.SetOper_2_ID(0);
		return retValue;
	}	//	copy

	
	/// <summary>
	///	Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="VAPA_FR_ColumnSet_ID">id</param>
	/// <param name="trxName">transction</param>
	public MReportColumn(Ctx ctx, int VAPA_FR_ColumnSet_ID, Trx trxName):base(ctx, VAPA_FR_ColumnSet_ID, trxName)
	{
		
		if (VAPA_FR_ColumnSet_ID == 0)
		{
			SetIsPrinted (true);
			SetSeqNo (0);
		}
	}	//	MReportColumn

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="ctx">context</param>
	/// <param name="rs">datarow</param>
	/// <param name="trxName">transaction</param>
	public MReportColumn (Ctx ctx,DataRow dr, Trx trxName):base(ctx,dr, trxName)
	{
		
	}	//	MReportColumn

	/// <summary>
	///	Get Column SQL Select Clause.
	/// </summary>
	/// <param name="withSum">withSum with SUM() function</param>
	/// <returns> select clause - AmtAcctCR+AmtAcctDR/etc or "null" if not defined</returns>
	public String GetSelectClause (Boolean withSum)
	{
		//	Amount Type = Period Balance, Period Credit
		String amountType = GetAmountType().Substring(0,1);	//	first character
		StringBuilder sb = new StringBuilder();
		if (withSum)
        {
			sb.Append("SUM(");
        }
		if (AmountType_Balance.Equals(amountType))
		//	sb.append("AmtAcctDr-AmtAcctCr");
			sb.Append("acctBalance(Account_ID,AmtAcctDr,AmtAcctCr)");
		else if (AmountType_CR.Equals(amountType))
			sb.Append("AmtAcctCr");
		else if (AmountType_DR.Equals(amountType))
			sb.Append("AmtAcctDr");
		else if (AmountType_Qty.Equals(amountType))
			sb.Append("Qty");
		else
		{
			log.Log(Level.SEVERE, "AmountType=" + GetAmountType () + ", at=" + amountType);
			return "NULL";
		}
		if (withSum)
			sb.Append(")");
		return sb.ToString();
	}	//	getSelectClause

	/// <summary>
	///	Is it Period Info ?
	/// </summary>
	/// <returns>true if Period Amount Type</returns>
	public Boolean IsPeriod()
	{
		String at = GetAmountType();
		if (at == null)
        {
			return false;
        }
		return AMOUNTTYPE_PeriodBalance.Equals(at)
			|| AMOUNTTYPE_PeriodCreditOnly.Equals(at)
			|| AMOUNTTYPE_PeriodDebitOnly.Equals(at)
			|| AMOUNTTYPE_PeriodQuantity.Equals(at);
	}	//	isPeriod

	/// <summary>
	///	Is it Year Info ?
	/// </summary>
	/// <returns>true if Year Amount Type</returns>
	public Boolean IsYear()
	{
		String at = GetAmountType();
		if (at == null)
        {
			return false;
        }
		return AMOUNTTYPE_YearBalance.Equals(at)
			|| AMOUNTTYPE_YearCreditOnly.Equals(at)
			|| AMOUNTTYPE_YearDebitOnly.Equals(at)
			|| AMOUNTTYPE_YearQuantity.Equals(at);
	}	//	isYear

	/// <summary>
	///	Is it Total Info ?
	/// </summary>
	/// <returns> true if Year Amount Type</returns>
	public Boolean IsTotal()
	{
		String at = GetAmountType();
		if (at == null)
        {
			return false;
        }
		return AMOUNTTYPE_TotalBalance.Equals(at)
			|| AMOUNTTYPE_TotalCreditOnly.Equals(at)
			|| AMOUNTTYPE_TotalDebitOnly.Equals(at)
			|| AMOUNTTYPE_TotalQuantity.Equals(at);
	}	//	isTotalBalance


	/// <summary>
	///	Get Segment Value Where Clause
	/// </summary>
	/// <param name="VAPA_FinancialReportingOrder_ID"> hierarchy</param>
	/// <returns> where clause</returns>
	public String GetWhereClause(int VAPA_FinancialReportingOrder_ID)
	{
		if (!IsColumnTypeSegmentValue())
        {
			return "";
        }
		
		String et = GetElementType();
		int ID = 0;
		if (MReportColumn.ELEMENTTYPE_Organization.Equals(et))
			ID = GetOrg_ID();
		else if (MReportColumn.ELEMENTTYPE_BPartner.Equals(et))
			ID = GetVAB_BusinessPartner_ID();
		else if (MReportColumn.ELEMENTTYPE_Product.Equals(et))
			ID = GetVAM_Product_ID();
		else if (MReportColumn.ELEMENTTYPE_Project.Equals(et))
			ID = GetVAB_Project_ID();
		else if (MReportColumn.ELEMENTTYPE_Activity.Equals(et))
			ID = GetVAB_BillingCode_ID();
		else if (MReportColumn.ELEMENTTYPE_Campaign.Equals(et))
			ID = GetVAB_Promotion_ID();
		else if (MReportColumn.ELEMENTTYPE_LocationFrom.Equals(et))
			ID = GetVAB_Address_ID();
		else if (MReportColumn.ELEMENTTYPE_LocationTo.Equals(et))
			ID = GetVAB_Address_ID();
		else if (MReportColumn.ELEMENTTYPE_OrgTrx.Equals(et))
			ID = GetOrg_ID();
		else if (MReportColumn.ELEMENTTYPE_SalesRegion.Equals(et))
			ID = GetVAB_SalesRegionState_ID();
		else if (MReportColumn.ELEMENTTYPE_Account.Equals(et))
			ID = GetVAB_Acct_Element_ID();
		else if (MReportColumn.ELEMENTTYPE_UserList1.Equals(et))
			ID = GetVAB_Acct_Element_ID();
		else if (MReportColumn.ELEMENTTYPE_UserList2.Equals(et))
			ID = GetVAB_Acct_Element_ID();
	//	else if (MReportColumn.ELEMENTTYPE_UserElement1.equals(et))
	//		ID = getVAB_Acct_Element_ID();
	//	else if (MReportColumn.ELEMENTTYPE_UserElement2.equals(et))
	//		ID = getVAB_Acct_Element_ID();
		else
			log.Warning("Unsupported Element Type=" + et);

		if (ID == 0)
		{
			log.Fine("No Restrictions - No ID for EntityType=" + et);
			return "";
		}
		return " AND " + MReportTree.GetWhereClause (GetCtx(), VAPA_FinancialReportingOrder_ID, et, ID);
	}	//	getWhereClause
	
	/// <summary>
	///	Get String Representation
	 /// </summary>
	/// <returns> String Representation</returns>
	public override String ToString ()
	{
		StringBuilder sb = new StringBuilder ("MReportColumn[")
			.Append(Get_ID()).Append(" - ").Append(GetName()).Append(" - ").Append(GetDescription())
			.Append(", SeqNo=").Append(GetSeqNo()).Append(", AmountType=").Append(GetAmountType())
			.Append(", CurrencyType=").Append(GetCurrencyType()).Append("/").Append(GetVAB_Currency_ID())
			.Append(" - ColumnType=").Append(GetColumnType());
		if (IsColumnTypeCalculation())
        {
			sb.Append(" - Calculation=").Append(GetCalculationType())
				.Append(" - ").Append(GetOper_1_ID()).Append(" - ").Append(GetOper_2_ID());
        }
		else if (IsColumnTypeRelativePeriod())
        {
			sb.Append(" - Period=").Append(GetRelativePeriod());
        }
		else
        {
			sb.Append(" - SegmentValue ElementType=").Append(GetElementType());
        }
		sb.Append ("]");
		return sb.ToString ();
	}	//	toString

	static  String		AmountType_Balance = "B";
	static  String		AmountType_CR = "C";
	static  String		AmountType_DR = "D";
	static  String		AmountType_Qty = "Q";
	//
	//static  String		AmountType_Period = "P";
	//static  String		AmountType_Year = "Y";
	//static  String		AmountType_Total = "T";

	/// <summary>
	/// Calculation Type Range
	/// </summary>
    /// <returns>true if range</returns>
	public Boolean IsCalculationTypeRange()
	{
		return CALCULATIONTYPE_AddRangeOp1ToOp2.Equals(GetCalculationType());
	}
	/// <summary>
	///	Calculation Type Add
    /// </summary>
    /// <returns>true id add</returns>
	public Boolean IsCalculationTypeAdd()
	{
		return CALCULATIONTYPE_AddOp1PlusOp2.Equals(GetCalculationType());
	}
	/// <summary>
	///	Calculation Type Subtract
	/// </summary>
    /// <returns>true if subtract</returns>
	public Boolean IsCalculationTypeSubtract()
	{
		return CALCULATIONTYPE_SubtractOp1_Op2.Equals(GetCalculationType());
	}
	/// <summary>
	///	Calculation Type Percent
	/// </summary>
    /// <returns>true if percent</returns>
	public Boolean IsCalculationTypePercent()
	{
		return CALCULATIONTYPE_PercentageOp1OfOp2.Equals(GetCalculationType());
	}


	/// <summary>
	///	Column Type Calculation
	/// </summary>
    /// <returns> true if calculation</returns>
	public Boolean IsColumnTypeCalculation()
	{
		return COLUMNTYPE_Calculation.Equals(GetColumnType());
	}
	/// <summary>
	///	Column Type Relative Period
	/// </summary>
    /// <returns>true if relative period</returns>
	public Boolean IsColumnTypeRelativePeriod()
	{
		return COLUMNTYPE_RelativePeriod.Equals(GetColumnType());
	}
	/// <summary>
	///	Column Type Segment Value
	/// </summary>
    /// <returns> true if segment value</returns>
	public Boolean IsColumnTypeSegmentValue()
	{
		return COLUMNTYPE_SegmentValue.Equals(GetColumnType());
	}
	/// <summary>
    ///	Get Relative Period As Int 
    /// </summary>
    /// <returns> relative period</returns>
	public int? GetRelativePeriodAsInt ()
	{
		Decimal? bd = GetRelativePeriod();
        if (bd == null)
        {
            return 0;
        }
		return Utility.Util.GetValueOfInt(bd);//.intValue();
	}	//	getRelativePeriodAsInt

	/// <summary>
	/// 	Get Relative Period
	/// </summary>
    /// <returns>relative period</returns>
	public new Decimal? GetRelativePeriod()
	{
        if (GetColumnType().Equals(COLUMNTYPE_RelativePeriod)
            || GetColumnType().Equals(COLUMNTYPE_SegmentValue))
        {
            return base.GetRelativePeriod();
        }
		return null;
	}	//	getRelativePeriod
	
	/// <summary>
	///	Get Element Type
	/// </summary>
	/// <returns></returns>
	public new String GetElementType()
	{
        if (GetColumnType().Equals(COLUMNTYPE_SegmentValue))
        {
            return base.GetElementType();
        }
		return null;
	}	//	getElementType
	
	/// <summary>
	///	Get Calculation Type
	/// </summary>
	/// <returns></returns>
	public new String GetCalculationType()
	{
        if (GetColumnType().Equals(COLUMNTYPE_Calculation))
        {
            return base.GetCalculationType();
        }
		return null;
	}	//	getCalculationType
	
	/// <summary>
	/// Before Save
	/// </summary>
	/// <param name="newRecord">new</param>
	/// <returns>true</returns>
	protected override Boolean BeforeSave(Boolean newRecord)
	{
		//	Validate Type
		String ct = GetColumnType();
		if (ct.Equals(COLUMNTYPE_RelativePeriod))
		{
			SetElementType(null);
			SetCalculationType(null);
		}
		else if (ct.Equals(COLUMNTYPE_Calculation))
		{
			SetElementType(null);
			SetRelativePeriod(null);
		}
		else if (ct.Equals(COLUMNTYPE_SegmentValue))
		{
			SetCalculationType(null);
		}
		return true;
	}	//	beforeSave

}	//	MReportColumn

}
