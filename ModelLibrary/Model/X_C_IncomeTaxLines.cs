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
/** Generated Model for C_IncomeTaxLines
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_IncomeTaxLines : PO
{
public X_C_IncomeTaxLines (Context ctx, int C_IncomeTaxLines_ID, Trx trxName) : base (ctx, C_IncomeTaxLines_ID, trxName)
{
/** if (C_IncomeTaxLines_ID == 0)
{
SetC_IncomeTaxLines_ID (0);
SetC_IncomeTax_ID (0);
}
 */
}
public X_C_IncomeTaxLines (Ctx ctx, int C_IncomeTaxLines_ID, Trx trxName) : base (ctx, C_IncomeTaxLines_ID, trxName)
{
/** if (C_IncomeTaxLines_ID == 0)
{
SetC_IncomeTaxLines_ID (0);
SetC_IncomeTax_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTaxLines (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTaxLines (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_IncomeTaxLines (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_IncomeTaxLines()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27681667617660L;
/** Last Updated Timestamp 5/8/2014 3:15:00 PM */
public static long updatedMS = 1399542300871L;
/** AD_Table_ID=1000442 */
public static int Table_ID;
 // =1000442;

/** TableName=C_IncomeTaxLines */
public static String Table_Name="C_IncomeTaxLines";

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
StringBuilder sb = new StringBuilder ("X_C_IncomeTaxLines[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Income Tax Lines.
@param C_IncomeTaxLines_ID Income Tax Lines */
public void SetC_IncomeTaxLines_ID (int C_IncomeTaxLines_ID)
{
if (C_IncomeTaxLines_ID < 1) throw new ArgumentException ("C_IncomeTaxLines_ID is mandatory.");
Set_ValueNoCheck ("C_IncomeTaxLines_ID", C_IncomeTaxLines_ID);
}
/** Get Income Tax Lines.
@return Income Tax Lines */
public int GetC_IncomeTaxLines_ID() 
{
Object ii = Get_Value("C_IncomeTaxLines_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Income Tax Account.
@param C_IncomeTax_Acct Income Tax Account */
public void SetC_IncomeTax_Acct (int C_IncomeTax_Acct)
{
Set_Value ("C_IncomeTax_Acct", C_IncomeTax_Acct);
}
/** Get Income Tax Account.
@return Income Tax Account */
public int GetC_IncomeTax_Acct() 
{
Object ii = Get_Value("C_IncomeTax_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Income Tax.
@param C_IncomeTax_ID Income Tax */
public void SetC_IncomeTax_ID (int C_IncomeTax_ID)
{
if (C_IncomeTax_ID < 1) throw new ArgumentException ("C_IncomeTax_ID is mandatory.");
Set_ValueNoCheck ("C_IncomeTax_ID", C_IncomeTax_ID);
}
/** Get Income Tax.
@return Income Tax */
public int GetC_IncomeTax_ID() 
{
Object ii = Get_Value("C_IncomeTax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Profit Dimension.
@param C_ProfitAndLoss_ID Profit Dimension */
public void SetC_ProfitAndLoss_ID (int C_ProfitAndLoss_ID)
{
if (C_ProfitAndLoss_ID <= 0) Set_Value ("C_ProfitAndLoss_ID", null);
else
Set_Value ("C_ProfitAndLoss_ID", C_ProfitAndLoss_ID);
}
/** Get Profit Dimension.
@return Profit Dimension */
public int GetC_ProfitAndLoss_ID() 
{
Object ii = Get_Value("C_ProfitAndLoss_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Income Tax Rate.
@param C_Tax_ID Tax identifier */
public void SetC_Tax_ID (int C_Tax_ID)
{
if (C_Tax_ID <= 0) Set_Value ("C_Tax_ID", null);
else
Set_Value ("C_Tax_ID", C_Tax_ID);
}
/** Get Income Tax Rate.
@return Tax identifier */
public int GetC_Tax_ID() 
{
Object ii = Get_Value("C_Tax_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID)
{
if (Export_ID != null && Export_ID.Length > 50)
{
log.Warning("Length > 50 - truncated");
Export_ID = Export_ID.Substring(0,50);
}
Set_Value ("Export_ID", Export_ID);
}
/** Get Export.
@return Export */
public String GetExport_ID() 
{
return (String)Get_Value("Export_ID");
}
/** Set Income Tax Amount.
@param IncomeTaxAmount Income Tax Amount */
public void SetIncomeTaxAmount (Decimal? IncomeTaxAmount)
{
Set_Value ("IncomeTaxAmount", (Decimal?)IncomeTaxAmount);
}
/** Get Income Tax Amount.
@return Income Tax Amount */
public Decimal GetIncomeTaxAmount() 
{
Object bd =Get_Value("IncomeTaxAmount");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Ledger Code.
@param LedgerCode Ledger Code */
public void SetLedgerCode (String LedgerCode)
{
if (LedgerCode != null && LedgerCode.Length > 50)
{
log.Warning("Length > 50 - truncated");
LedgerCode = LedgerCode.Substring(0,50);
}
Set_Value ("LedgerCode", LedgerCode);
}
/** Get Ledger Code.
@return Ledger Code */
public String GetLedgerCode() 
{
return (String)Get_Value("LedgerCode");
}
/** Set Ledger Name.
@param LedgerName Ledger Name */
public void SetLedgerName (String LedgerName)
{
if (LedgerName != null && LedgerName.Length > 100)
{
log.Warning("Length > 100 - truncated");
LedgerName = LedgerName.Substring(0,100);
}
Set_Value ("LedgerName", LedgerName);
}
/** Get Ledger Name.
@return Ledger Name */
public String GetLedgerName() 
{
return (String)Get_Value("LedgerName");
}
/** Set Processed.
@param Processed The document has been processed */
public void SetProcessed (Boolean Processed)
{
Set_Value ("Processed", Processed);
}
/** Get Processed.
@return The document has been processed */
public Boolean IsProcessed() 
{
Object oo = Get_Value("Processed");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
