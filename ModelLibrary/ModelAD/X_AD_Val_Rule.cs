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
/** Generated Model for AD_Val_Rule
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Val_Rule : PO
{
public X_AD_Val_Rule (Context ctx, int AD_Val_Rule_ID, Trx trxName) : base (ctx, AD_Val_Rule_ID, trxName)
{
/** if (AD_Val_Rule_ID == 0)
{
SetAD_Val_Rule_ID (0);
SetEntityType (null);	// U
SetName (null);
SetType (null);
}
 */
}
public X_AD_Val_Rule (Ctx ctx, int AD_Val_Rule_ID, Trx trxName) : base (ctx, AD_Val_Rule_ID, trxName)
{
/** if (AD_Val_Rule_ID == 0)
{
SetAD_Val_Rule_ID (0);
SetEntityType (null);	// U
SetName (null);
SetType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Val_Rule (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Val_Rule (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Val_Rule (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Val_Rule()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514365699L;
/** Last Updated Timestamp 7/29/2010 1:07:28 PM */
public static long updatedMS = 1280389048910L;
/** AD_Table_ID=108 */
public static int Table_ID;
 // =108;

/** TableName=AD_Val_Rule */
public static String Table_Name="AD_Val_Rule";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
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
StringBuilder sb = new StringBuilder ("X_AD_Val_Rule[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Dynamic Validation.
@param AD_Val_Rule_ID Dynamic Validation Rule */
public void SetAD_Val_Rule_ID (int AD_Val_Rule_ID)
{
if (AD_Val_Rule_ID < 1) throw new ArgumentException ("AD_Val_Rule_ID is mandatory.");
Set_ValueNoCheck ("AD_Val_Rule_ID", AD_Val_Rule_ID);
}
/** Get Dynamic Validation.
@return Dynamic Validation Rule */
public int GetAD_Val_Rule_ID() 
{
Object ii = Get_Value("AD_Val_Rule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Code.
@param Code Code to execute or to validate */
public void SetCode (String Code)
{
if (Code != null && Code.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Code = Code.Substring(0,2000);
}
Set_Value ("Code", Code);
}
/** Get Code.
@return Code to execute or to validate */
public String GetCode() 
{
return (String)Get_Value("Code");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
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

/** Type AD_Reference_ID=101 */
public static int TYPE_AD_Reference_ID=101;
/** Java Script = E */
public static String TYPE_JavaScript = "E";
/** Java Language = J */
public static String TYPE_JavaLanguage = "J";
/** SQL = S */
public static String TYPE_SQL = "S";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsTypeValid (String test)
{
return test.Equals("E") || test.Equals("J") || test.Equals("S");
}
/** Set Code Type.
@param Type Type of Code/Validation (SQL, Java Script, Java Language) */
public void SetType (String Type)
{
if (Type == null) throw new ArgumentException ("Type is mandatory");
if (!IsTypeValid(Type))
throw new ArgumentException ("Type Invalid value - " + Type + " - Reference_ID=101 - E - J - S");
if (Type.Length > 1)
{
log.Warning("Length > 1 - truncated");
Type = Type.Substring(0,1);
}
Set_Value ("Type", Type);
}
/** Get Code Type.
@return Type of Code/Validation (SQL, Java Script, Java Language) */
public new String GetType() 
{
return (String)Get_Value("Type");
}
}

}
