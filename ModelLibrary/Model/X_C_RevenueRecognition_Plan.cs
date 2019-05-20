namespace VAdvantage.Model
{

/** Generated Model - DO NOT CHANGE */
using System;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Classes;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.Utility;
using System.Data;
/** Generated Model for C_RevenueRecognition_Plan
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_RevenueRecognition_Plan : PO
{
public X_C_RevenueRecognition_Plan (Context ctx, int C_RevenueRecognition_Plan_ID, Trx trxName) : base (ctx, C_RevenueRecognition_Plan_ID, trxName)
{
/** if (C_RevenueRecognition_Plan_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Currency_ID (0);
SetC_InvoiceLine_ID (0);
SetC_RevenueRecognition_ID (0);
SetC_RevenueRecognition_Plan_ID (0);
SetP_Revenue_Acct (0);
SetRecognizedAmt (0.0);
SetTotalAmt (0.0);
SetUnEarnedRevenue_Acct (0);
}
 */
}
public X_C_RevenueRecognition_Plan (Ctx ctx, int C_RevenueRecognition_Plan_ID, Trx trxName) : base (ctx, C_RevenueRecognition_Plan_ID, trxName)
{
/** if (C_RevenueRecognition_Plan_ID == 0)
{
SetC_AcctSchema_ID (0);
SetC_Currency_ID (0);
SetC_InvoiceLine_ID (0);
SetC_RevenueRecognition_ID (0);
SetC_RevenueRecognition_Plan_ID (0);
SetP_Revenue_Acct (0);
SetRecognizedAmt (0.0);
SetTotalAmt (0.0);
SetUnEarnedRevenue_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Plan (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Plan (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_RevenueRecognition_Plan (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_RevenueRecognition_Plan()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514374616L;
/** Last Updated Timestamp 7/29/2010 1:07:37 PM */
public static long updatedMS = 1280389057827L;
/** AD_Table_ID=443 */
public static int Table_ID;
 // =443;

/** TableName=C_RevenueRecognition_Plan */
public static String Table_Name="C_RevenueRecognition_Plan";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO(Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
}
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_RevenueRecognition_Plan[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Currency.
@param C_Currency_ID The Currency for this record */
public void SetC_Currency_ID (int C_Currency_ID)
{
if (C_Currency_ID < 1) throw new ArgumentException ("C_Currency_ID is mandatory.");
Set_ValueNoCheck ("C_Currency_ID", C_Currency_ID);
}
/** Get Currency.
@return The Currency for this record */
public int GetC_Currency_ID() 
{
Object ii = Get_Value("C_Currency_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
public void SetC_InvoiceLine_ID (int C_InvoiceLine_ID)
{
if (C_InvoiceLine_ID < 1) throw new ArgumentException ("C_InvoiceLine_ID is mandatory.");
Set_ValueNoCheck ("C_InvoiceLine_ID", C_InvoiceLine_ID);
}
/** Get Invoice Line.
@return Invoice Detail Line */
public int GetC_InvoiceLine_ID() 
{
Object ii = Get_Value("C_InvoiceLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Revenue Recognition.
@param C_RevenueRecognition_ID Method for recording revenue */
public void SetC_RevenueRecognition_ID (int C_RevenueRecognition_ID)
{
if (C_RevenueRecognition_ID < 1) throw new ArgumentException ("C_RevenueRecognition_ID is mandatory.");
Set_ValueNoCheck ("C_RevenueRecognition_ID", C_RevenueRecognition_ID);
}
/** Get Revenue Recognition.
@return Method for recording revenue */
public int GetC_RevenueRecognition_ID() 
{
Object ii = Get_Value("C_RevenueRecognition_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_RevenueRecognition_ID().ToString());
}
/** Set Revenue Recognition Plan.
@param C_RevenueRecognition_Plan_ID Plan for recognizing or recording revenue */
public void SetC_RevenueRecognition_Plan_ID (int C_RevenueRecognition_Plan_ID)
{
if (C_RevenueRecognition_Plan_ID < 1) throw new ArgumentException ("C_RevenueRecognition_Plan_ID is mandatory.");
Set_ValueNoCheck ("C_RevenueRecognition_Plan_ID", C_RevenueRecognition_Plan_ID);
}
/** Get Revenue Recognition Plan.
@return Plan for recognizing or recording revenue */
public int GetC_RevenueRecognition_Plan_ID() 
{
Object ii = Get_Value("C_RevenueRecognition_Plan_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Revenue.
@param P_Revenue_Acct Account for Product Revenue (Sales Account) */
public void SetP_Revenue_Acct (int P_Revenue_Acct)
{
Set_ValueNoCheck ("P_Revenue_Acct", P_Revenue_Acct);
}
/** Get Product Revenue.
@return Account for Product Revenue (Sales Account) */
public int GetP_Revenue_Acct() 
{
Object ii = Get_Value("P_Revenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Recognized Amount.
@param RecognizedAmt Recognized Amount */
public void SetRecognizedAmt (Decimal? RecognizedAmt)
{
if (RecognizedAmt == null) throw new ArgumentException ("RecognizedAmt is mandatory.");
Set_ValueNoCheck ("RecognizedAmt", (Decimal?)RecognizedAmt);
}
/** Get Recognized Amount.
@return Recognized Amount */
public Decimal GetRecognizedAmt() 
{
Object bd =Get_Value("RecognizedAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Total Amount.
@param TotalAmt Total Amount */
public void SetTotalAmt (Decimal? TotalAmt)
{
if (TotalAmt == null) throw new ArgumentException ("TotalAmt is mandatory.");
Set_ValueNoCheck ("TotalAmt", (Decimal?)TotalAmt);
}
/** Get Total Amount.
@return Total Amount */
public Decimal GetTotalAmt() 
{
Object bd =Get_Value("TotalAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Unearned Revenue.
@param UnEarnedRevenue_Acct Account for unearned revenue */
public void SetUnEarnedRevenue_Acct (int UnEarnedRevenue_Acct)
{
Set_ValueNoCheck ("UnEarnedRevenue_Acct", UnEarnedRevenue_Acct);
}
/** Get Unearned Revenue.
@return Account for unearned revenue */
public int GetUnEarnedRevenue_Acct() 
{
Object ii = Get_Value("UnEarnedRevenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
