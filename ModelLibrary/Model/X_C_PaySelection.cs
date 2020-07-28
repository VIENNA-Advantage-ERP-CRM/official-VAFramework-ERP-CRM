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
/** Generated Model for C_PaySelection
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_PaySelection : PO
{
public X_C_PaySelection (Context ctx, int C_PaySelection_ID, Trx trxName) : base (ctx, C_PaySelection_ID, trxName)
{
/** if (C_PaySelection_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_PaySelection_ID (0);
SetIsApproved (false);
SetName (null);	// @#Date@
SetPayDate (DateTime.Now);	// @#Date@
SetProcessed (false);	// N
SetTotalAmt (0.0);
}
 */
}
public X_C_PaySelection (Ctx ctx, int C_PaySelection_ID, Trx trxName) : base (ctx, C_PaySelection_ID, trxName)
{
/** if (C_PaySelection_ID == 0)
{
SetC_BankAccount_ID (0);
SetC_PaySelection_ID (0);
SetIsApproved (false);
SetName (null);	// @#Date@
SetPayDate (DateTime.Now);	// @#Date@
SetProcessed (false);	// N
SetTotalAmt (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaySelection (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaySelection (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_PaySelection (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_PaySelection()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514373582L;
/** Last Updated Timestamp 7/29/2010 1:07:36 PM */
public static long updatedMS = 1280389056793L;
/** AD_Table_ID=426 */
public static int Table_ID;
 // =426;

/** TableName=C_PaySelection */
public static String Table_Name="C_PaySelection";

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
StringBuilder sb = new StringBuilder ("X_C_PaySelection[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Bank Account.
@param C_BankAccount_ID Account at the Bank */
public void SetC_BankAccount_ID (int C_BankAccount_ID)
{
if (C_BankAccount_ID < 1) throw new ArgumentException ("C_BankAccount_ID is mandatory.");
Set_Value ("C_BankAccount_ID", C_BankAccount_ID);
}
/** Get Bank Account.
@return Account at the Bank */
public int GetC_BankAccount_ID() 
{
Object ii = Get_Value("C_BankAccount_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Payment Selection.
@param C_PaySelection_ID Payment Selection */
public void SetC_PaySelection_ID (int C_PaySelection_ID)
{
if (C_PaySelection_ID < 1) throw new ArgumentException ("C_PaySelection_ID is mandatory.");
Set_ValueNoCheck ("C_PaySelection_ID", C_PaySelection_ID);
}
/** Get Payment Selection.
@return Payment Selection */
public int GetC_PaySelection_ID() 
{
Object ii = Get_Value("C_PaySelection_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Create lines from.
@param CreateFrom Process which will generate a new document lines based on an existing document */
public void SetCreateFrom (String CreateFrom)
{
if (CreateFrom != null && CreateFrom.Length > 1)
{
log.Warning("Length > 1 - truncated");
CreateFrom = CreateFrom.Substring(0,1);
}
Set_Value ("CreateFrom", CreateFrom);
}
/** Get Create lines from.
@return Process which will generate a new document lines based on an existing document */
public String GetCreateFrom() 
{
return (String)Get_Value("CreateFrom");
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
Description = Description.Substring(0,255);
}
Set_Value ("Description", Description);
}
/** Get Description.
@return Optional short description of the record */
public String GetDescription() 
{
return (String)Get_Value("Description");
}
/** Set Approved.
@param IsApproved Indicates if this document requires approval */
public void SetIsApproved (Boolean IsApproved)
{
Set_Value ("IsApproved", IsApproved);
}
/** Get Approved.
@return Indicates if this document requires approval */
public Boolean IsApproved() 
{
Object oo = Get_Value("IsApproved");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
Name = Name.Substring(0,60);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Payment date.
@param PayDate Date Payment made */
public void SetPayDate (DateTime? PayDate)
{
if (PayDate == null) throw new ArgumentException ("PayDate is mandatory.");
Set_Value ("PayDate", (DateTime?)PayDate);
}
/** Get Payment date.
@return Date Payment made */
public DateTime? GetPayDate() 
{
return (DateTime?)Get_Value("PayDate");
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
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Total Amount.
@param TotalAmt Total Amount */
public void SetTotalAmt (Decimal? TotalAmt)
{
if (TotalAmt == null) throw new ArgumentException ("TotalAmt is mandatory.");
Set_Value ("TotalAmt", (Decimal?)TotalAmt);
}
/** Get Total Amount.
@return Total Amount */
public Decimal GetTotalAmt() 
{
Object bd =Get_Value("TotalAmt");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
