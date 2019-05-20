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
/** Generated Model for AD_ImpFormat
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_ImpFormat : PO
{
public X_AD_ImpFormat (Context ctx, int AD_ImpFormat_ID, Trx trxName) : base (ctx, AD_ImpFormat_ID, trxName)
{
/** if (AD_ImpFormat_ID == 0)
{
SetAD_ImpFormat_ID (0);
SetAD_Table_ID (0);
SetFormatType (null);
SetName (null);
SetProcessing (false);	// N
}
 */
}
public X_AD_ImpFormat (Ctx ctx, int AD_ImpFormat_ID, Trx trxName) : base (ctx, AD_ImpFormat_ID, trxName)
{
/** if (AD_ImpFormat_ID == 0)
{
SetAD_ImpFormat_ID (0);
SetAD_Table_ID (0);
SetFormatType (null);
SetName (null);
SetProcessing (false);	// N
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ImpFormat (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ImpFormat (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ImpFormat (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ImpFormat()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514361467L;
/** Last Updated Timestamp 7/29/2010 1:07:24 PM */
public static long updatedMS = 1280389044678L;
/** AD_Table_ID=381 */
public static int Table_ID;
 // =381;

/** TableName=AD_ImpFormat */
public static String Table_Name="AD_ImpFormat";

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
StringBuilder sb = new StringBuilder ("X_AD_ImpFormat[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Import Format.
@param AD_ImpFormat_ID Import Format */
public void SetAD_ImpFormat_ID (int AD_ImpFormat_ID)
{
if (AD_ImpFormat_ID < 1) throw new ArgumentException ("AD_ImpFormat_ID is mandatory.");
Set_ValueNoCheck ("AD_ImpFormat_ID", AD_ImpFormat_ID);
}
/** Get Import Format.
@return Import Format */
public int GetAD_ImpFormat_ID() 
{
Object ii = Get_Value("AD_ImpFormat_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
Set_Value ("AD_Table_ID", AD_Table_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_Table_ID() 
{
Object ii = Get_Value("AD_Table_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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

/** FormatType AD_Reference_ID=209 */
public static int FORMATTYPE_AD_Reference_ID=209;
/** Comma Separated = C */
public static String FORMATTYPE_CommaSeparated = "C";
/** Fixed Position = F */
public static String FORMATTYPE_FixedPosition = "F";
/** Tab Separated = T */
public static String FORMATTYPE_TabSeparated = "T";
/** XML = X */
public static String FORMATTYPE_XML = "X";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsFormatTypeValid (String test)
{
return test.Equals("C") || test.Equals("F") || test.Equals("T") || test.Equals("X");
}
/** Set Format.
@param FormatType Format of the data */
public void SetFormatType (String FormatType)
{
if (FormatType == null) throw new ArgumentException ("FormatType is mandatory");
if (!IsFormatTypeValid(FormatType))
throw new ArgumentException ("FormatType Invalid value - " + FormatType + " - Reference_ID=209 - C - F - T - X");
if (FormatType.Length > 1)
{
log.Warning("Length > 1 - truncated");
FormatType = FormatType.Substring(0,1);
}
Set_Value ("FormatType", FormatType);
}
/** Get Format.
@return Format of the data */
public String GetFormatType() 
{
return (String)Get_Value("FormatType");
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
}

}
