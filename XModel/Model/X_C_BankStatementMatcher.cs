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
/** Generated Model for VAB_BankingJRNLMatcher
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_BankingJRNLMatcher : PO
{
public X_VAB_BankingJRNLMatcher (Context ctx, int VAB_BankingJRNLMatcher_ID, Trx trxName) : base (ctx, VAB_BankingJRNLMatcher_ID, trxName)
{
/** if (VAB_BankingJRNLMatcher_ID == 0)
{
SetVAB_BankingJRNLMatcher_ID (0);
SetClassname (null);
SetName (null);
SetSeqNo (0);
}
 */
}
public X_VAB_BankingJRNLMatcher (Ctx ctx, int VAB_BankingJRNLMatcher_ID, Trx trxName) : base (ctx, VAB_BankingJRNLMatcher_ID, trxName)
{
/** if (VAB_BankingJRNLMatcher_ID == 0)
{
SetVAB_BankingJRNLMatcher_ID (0);
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
public X_VAB_BankingJRNLMatcher (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BankingJRNLMatcher (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_BankingJRNLMatcher (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_BankingJRNLMatcher()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514370933L;
/** Last Updated Timestamp 7/29/2010 1:07:34 PM */
public static long updatedMS = 1280389054144L;
/** VAF_TableView_ID=658 */
public static int Table_ID;
 // =658;

/** TableName=VAB_BankingJRNLMatcher */
public static String Table_Name="VAB_BankingJRNLMatcher";

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
StringBuilder sb = new StringBuilder ("X_VAB_BankingJRNLMatcher[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Bank Statement Matcher.
@param VAB_BankingJRNLMatcher_ID Algorithm to match Bank Statement Info to Business Partners, Invoices and Payments */
public void SetVAB_BankingJRNLMatcher_ID (int VAB_BankingJRNLMatcher_ID)
{
if (VAB_BankingJRNLMatcher_ID < 1) throw new ArgumentException ("VAB_BankingJRNLMatcher_ID is mandatory.");
Set_ValueNoCheck ("VAB_BankingJRNLMatcher_ID", VAB_BankingJRNLMatcher_ID);
}
/** Get Bank Statement Matcher.
@return Algorithm to match Bank Statement Info to Business Partners, Invoices and Payments */
public int GetVAB_BankingJRNLMatcher_ID() 
{
Object ii = Get_Value("VAB_BankingJRNLMatcher_ID");
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
