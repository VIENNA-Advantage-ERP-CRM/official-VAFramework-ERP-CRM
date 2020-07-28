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
/** Generated Model for C_BankAccountLine
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankAccountLine : PO
{
public X_C_BankAccountLine (Context ctx, int C_BankAccountLine_ID, Trx trxName) : base (ctx, C_BankAccountLine_ID, trxName)
{
/** if (C_BankAccountLine_ID == 0)
{
SetC_BankAccountLine_ID (0);
SetC_BankAccount_ID (0);
SetStatementDate (DateTime.Now);	// @#Date@
}
 */
}
public X_C_BankAccountLine (Ctx ctx, int C_BankAccountLine_ID, Trx trxName) : base (ctx, C_BankAccountLine_ID, trxName)
{
/** if (C_BankAccountLine_ID == 0)
{
SetC_BankAccountLine_ID (0);
SetC_BankAccount_ID (0);
SetStatementDate (DateTime.Now);	// @#Date@
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccountLine(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccountLine(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankAccountLine(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankAccountLine()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27629568992807L;
/** Last Updated Timestamp 9/12/2012 3:24:36 PM */
public static long updatedMS = 1347443676018L;
/** AD_Table_ID=1000372 */
public static int Table_ID;
 // =1000372;

/** TableName=C_BankAccountLine */
public static String Table_Name="C_BankAccountLine";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel()
{
return Convert.ToInt32(accessLevel.ToString());
}
/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
/** Info
@return info
*/
public override String ToString()
{
StringBuilder sb = new StringBuilder ("X_C_BankAccountLine[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set C_BankAccountLine_ID.
@param C_BankAccountLine_ID C_BankAccountLine_ID */
public void SetC_BankAccountLine_ID (int C_BankAccountLine_ID)
{
if (C_BankAccountLine_ID < 1) throw new ArgumentException ("C_BankAccountLine_ID is mandatory.");
Set_ValueNoCheck ("C_BankAccountLine_ID", C_BankAccountLine_ID);
}
/** Get C_BankAccountLine_ID.
@return C_BankAccountLine_ID */
public int GetC_BankAccountLine_ID() 
{
Object ii = Get_Value("C_BankAccountLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID < 1) throw new ArgumentException ("C_BankAccount_ID is mandatory.");
Set_ValueNoCheck ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Account Date.
@param StatementDate General Ledger Date */
public void SetStatementDate (DateTime? StatementDate)
{
if (StatementDate == null) throw new ArgumentException ("StatementDate is mandatory.");
Set_Value ("StatementDate", (DateTime?)StatementDate);
}
/** Get Account Date.
@return General Ledger Date */
public DateTime? GetStatementDate() 
{
return (DateTime?)Get_Value("StatementDate");
}
/** Set Ending balance.
@param EndingBalance Ending  or closing balance */
public void SetEndingBalance (Decimal? EndingBalance)
{
Set_Value ("EndingBalance", (Decimal?)EndingBalance);
}
/** Get Ending balance.
@return Ending  or closing balance */
public Decimal GetEndingBalance() 
{
Object bd =Get_Value("EndingBalance");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Statement difference.
@param StatementDifference Difference between statement ending balance and actual ending balance */
public void SetStatementDifference (Decimal? StatementDifference)
{
Set_Value ("StatementDifference", (Decimal?)StatementDifference);
}
/** Get Statement difference.
@return Difference between statement ending balance and actual ending balance */
public Decimal GetStatementDifference() 
{
Object bd =Get_Value("StatementDifference");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
