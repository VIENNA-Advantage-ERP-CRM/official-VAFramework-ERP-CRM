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
/** Generated Model for C_BankStatementMatcher
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_BankStatementMatcher : PO
{
public X_C_BankStatementMatcher (Context ctx, int C_BankStatementMatcher_ID, Trx trxName) : base (ctx, C_BankStatementMatcher_ID, trxName)
{
/** if (C_BankStatementMatcher_ID == 0)
{
SetC_BankStatementMatcher_ID (0);
SetClassname (null);
SetName (null);
SetSeqNo (0);
}
 */
}
public X_C_BankStatementMatcher (Ctx ctx, int C_BankStatementMatcher_ID, Trx trxName) : base (ctx, C_BankStatementMatcher_ID, trxName)
{
/** if (C_BankStatementMatcher_ID == 0)
{
SetC_BankStatementMatcher_ID (0);
SetClassname (null);
SetName (null);
SetSeqNo (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementMatcher (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementMatcher (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_BankStatementMatcher (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_BankStatementMatcher()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370933L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054144L;
/** AD_Table_ID=658 */
public static int Table_ID;
 // =658;

/** TableName=C_BankStatementMatcher */
public static String Table_Name="C_BankStatementMatcher";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(2);
/** AccessLevel
@return 2 - Client 
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
StringBuilder sb = new StringBuilder ("X_C_BankStatementMatcher[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Bank Statement Matcher.
@param C_BankStatementMatcher_ID Algorithm to match Bank Statement Info to Business Partners, Invoices and Payments */
public void SetC_BankStatementMatcher_ID (int C_BankStatementMatcher_ID)
{
if (C_BankStatementMatcher_ID < 1) throw new ArgumentException ("C_BankStatementMatcher_ID is mandatory.");
Set_ValueNoCheck ("C_BankStatementMatcher_ID", C_BankStatementMatcher_ID);
}
/** Get Bank Statement Matcher.
@return Algorithm to match Bank Statement Info to Business Partners, Invoices and Payments */
public int GetC_BankStatementMatcher_ID() 
{
Object ii = Get_Value("C_BankStatementMatcher_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Classname.
@param Classname Java Classname */
public void SetClassname (String Classname)
{
if (Classname == null) throw new ArgumentException ("Classname is mandatory.");
if (Classname.Length > 60)
{
log.Warning("Length > 60 - truncated");
Classname = Classname.Substring(0,60);
}
Set_Value ("Classname", Classname);
}
/** Get Classname.
@return Java Classname */
public String GetClassname() 
{
return (String)Get_Value("Classname");
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
/** Set Sequence.
@param SeqNo Method of ordering elements;
 lowest number comes first */
public void SetSeqNo (int SeqNo)
{
Set_Value ("SeqNo", SeqNo);
}
/** Get Sequence.
@return Method of ordering elements;
 lowest number comes first */
public int GetSeqNo() 
{
Object ii = Get_Value("SeqNo");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
