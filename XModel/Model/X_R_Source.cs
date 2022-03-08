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
/** Generated Model for R_Source
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_R_Source : PO
{
public X_R_Source (Context ctx, int R_Source_ID, Trx trxName) : base (ctx, R_Source_ID, trxName)
{
/** if (R_Source_ID == 0)
{
SetName (null);
SetR_Source_ID (0);
SetSourceCreateType (null);	// L
SetValue (null);
}
 */
}
public X_R_Source (Ctx ctx, int R_Source_ID, Trx trxName) : base (ctx, R_Source_ID, trxName)
{
/** if (R_Source_ID == 0)
{
SetName (null);
SetR_Source_ID (0);
SetSourceCreateType (null);	// L
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Source (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Source (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_R_Source (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_R_Source()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514383440L;
/** Last Updated Timestamp 7/29/2010 1:07:46 PM */
public static long updatedMS = 1280389066651L;
/** AD_Table_ID=914 */
public static int Table_ID;
 // =914;

/** TableName=R_Source */
public static String Table_Name="R_Source";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(6);
/** AccessLevel
@return 6 - System - Client 
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
StringBuilder sb = new StringBuilder ("X_R_Source[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set Comment.
@param Help Comment, Help or Hint */
public void SetHelp (String Help)
{
if (Help != null && Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Help = Help.Substring(0,2000);
}
Set_Value ("Help", Help);
}
/** Get Comment.
@return Comment, Help or Hint */
public String GetHelp() 
{
return (String)Get_Value("Help");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 120)
{
log.Warning("Length > 120 - truncated");
Name = Name.Substring(0,120);
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
/** Set Source.
@param R_Source_ID Source for the Lead or Request */
public void SetR_Source_ID (int R_Source_ID)
{
if (R_Source_ID < 1) throw new ArgumentException ("R_Source_ID is mandatory.");
Set_ValueNoCheck ("R_Source_ID", R_Source_ID);
}
/** Get Source.
@return Source for the Lead or Request */
public int GetR_Source_ID() 
{
Object ii = Get_Value("R_Source_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** SourceCreateType AD_Reference_ID=423 */
public static int SOURCECREATETYPE_AD_Reference_ID=423;
/** Both = B */
public static String SOURCECREATETYPE_Both = "B";
/** Lead = L */
public static String SOURCECREATETYPE_Lead = "L";
/** Request = R */
public static String SOURCECREATETYPE_Request = "R";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsSourceCreateTypeValid (String test)
{
return test.Equals("B") || test.Equals("L") || test.Equals("R");
}
/** Set Create Type.
@param SourceCreateType Automatically create Lead or Source */
public void SetSourceCreateType (String SourceCreateType)
{
if (SourceCreateType == null) throw new ArgumentException ("SourceCreateType is mandatory");
if (!IsSourceCreateTypeValid(SourceCreateType))
throw new ArgumentException ("SourceCreateType Invalid value - " + SourceCreateType + " - Reference_ID=423 - B - L - R");
if (SourceCreateType.Length > 1)
{
log.Warning("Length > 1 - truncated");
SourceCreateType = SourceCreateType.Substring(0,1);
}
Set_Value ("SourceCreateType", SourceCreateType);
}
/** Get Create Type.
@return Automatically create Lead or Source */
public String GetSourceCreateType() 
{
return (String)Get_Value("SourceCreateType");
}
/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value)
{
if (Value == null) throw new ArgumentException ("Value is mandatory.");
if (Value.Length > 40)
{
log.Warning("Length > 40 - truncated");
Value = Value.Substring(0,40);
}
Set_Value ("Value", Value);
}
/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() 
{
return (String)Get_Value("Value");
}
}

}
