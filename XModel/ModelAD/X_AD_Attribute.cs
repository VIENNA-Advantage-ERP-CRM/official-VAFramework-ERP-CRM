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
/** Generated Model for AD_Attribute
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Attribute : PO
{
public X_AD_Attribute (Context ctx, int AD_Attribute_ID, Trx trxName) : base (ctx, AD_Attribute_ID, trxName)
{
/** if (AD_Attribute_ID == 0)
{
SetAD_Attribute_ID (0);
SetAD_Reference_ID (0);
SetAD_Table_ID (0);
SetIsEncrypted (false);
SetIsFieldOnly (false);
SetIsHeading (false);
SetIsMandatory (false);
SetIsReadOnly (false);
SetIsSameLine (false);
SetIsUpdateable (false);
SetName (null);
}
 */
}
public X_AD_Attribute (Ctx ctx, int AD_Attribute_ID, Trx trxName) : base (ctx, AD_Attribute_ID, trxName)
{
/** if (AD_Attribute_ID == 0)
{
SetAD_Attribute_ID (0);
SetAD_Reference_ID (0);
SetAD_Table_ID (0);
SetIsEncrypted (false);
SetIsFieldOnly (false);
SetIsHeading (false);
SetIsMandatory (false);
SetIsReadOnly (false);
SetIsSameLine (false);
SetIsUpdateable (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Attribute (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_Attribute()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27562514360715L;
/** Last Updated Timestamp 7/29/2010 1:07:23 PM */
public static long updatedMS = 1280389043926L;
/** AD_Table_ID=405 */
public static int Table_ID;
 // =405;

/** TableName=AD_Attribute */
public static String Table_Name="AD_Attribute";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(7);
/** AccessLevel
@return 7 - System - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_AD_Attribute[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Attribute.
@param AD_Attribute_ID System Attribute */
public void SetAD_Attribute_ID (int AD_Attribute_ID)
{
if (AD_Attribute_ID < 1) throw new ArgumentException ("AD_Attribute_ID is mandatory.");
Set_ValueNoCheck ("AD_Attribute_ID", AD_Attribute_ID);
}
/** Get System Attribute.
@return System Attribute */
public int GetAD_Attribute_ID() 
{
Object ii = Get_Value("AD_Attribute_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_Reference_ID AD_Reference_ID=1 */
public static int AD_REFERENCE_ID_AD_Reference_ID=1;
/** Set Reference.
@param AD_Reference_ID System Reference and Validation */
public void SetAD_Reference_ID (int AD_Reference_ID)
{
if (AD_Reference_ID < 1) throw new ArgumentException ("AD_Reference_ID is mandatory.");
Set_Value ("AD_Reference_ID", AD_Reference_ID);
}
/** Get Reference.
@return System Reference and Validation */
public int GetAD_Reference_ID() 
{
Object ii = Get_Value("AD_Reference_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** AD_Reference_Value_ID AD_Reference_ID=4 */
public static int AD_REFERENCE_VALUE_ID_AD_Reference_ID=4;
/** Set Reference Key.
@param AD_Reference_Value_ID Required to specify, if data type is Table or List */
public void SetAD_Reference_Value_ID (int AD_Reference_Value_ID)
{
if (AD_Reference_Value_ID <= 0) Set_Value ("AD_Reference_Value_ID", null);
else
Set_Value ("AD_Reference_Value_ID", AD_Reference_Value_ID);
}
/** Get Reference Key.
@return Required to specify, if data type is Table or List */
public int GetAD_Reference_Value_ID() 
{
Object ii = Get_Value("AD_Reference_Value_ID");
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
/** Set Dynamic Validation.
@param AD_Val_Rule_ID Dynamic Validation Rule */
public void SetAD_Val_Rule_ID (int AD_Val_Rule_ID)
{
if (AD_Val_Rule_ID <= 0) Set_Value ("AD_Val_Rule_ID", null);
else
Set_Value ("AD_Val_Rule_ID", AD_Val_Rule_ID);
}
/** Get Dynamic Validation.
@return Dynamic Validation Rule */
public int GetAD_Val_Rule_ID() 
{
Object ii = Get_Value("AD_Val_Rule_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Callout Code.
@param Callout External Callout Code - Fully qualified class names and method - separated by semicolons */
public void SetCallout (String Callout)
{
if (Callout != null && Callout.Length > 60)
{
log.Warning("Length > 60 - truncated");
Callout = Callout.Substring(0,60);
}
Set_Value ("Callout", Callout);
}
/** Get Callout Code.
@return External Callout Code - Fully qualified class names and method - separated by semicolons */
public String GetCallout() 
{
return (String)Get_Value("Callout");
}
/** Set Default Logic.
@param DefaultValue Default value hierarchy, separated by ;
 */
public void SetDefaultValue (String DefaultValue)
{
if (DefaultValue != null && DefaultValue.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DefaultValue = DefaultValue.Substring(0,2000);
}
Set_Value ("DefaultValue", DefaultValue);
}
/** Get Default Logic.
@return Default value hierarchy, separated by ;
 */
public String GetDefaultValue() 
{
return (String)Get_Value("DefaultValue");
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
/** Set Display Length.
@param DisplayLength Length of the display in characters */
public void SetDisplayLength (int DisplayLength)
{
Set_Value ("DisplayLength", DisplayLength);
}
/** Get Display Length.
@return Length of the display in characters */
public int GetDisplayLength() 
{
Object ii = Get_Value("DisplayLength");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Display Logic.
@param DisplayLogic If the Field is displayed, the result determines if the field is actually displayed */
public void SetDisplayLogic (String DisplayLogic)
{
if (DisplayLogic != null && DisplayLogic.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
DisplayLogic = DisplayLogic.Substring(0,2000);
}
Set_Value ("DisplayLogic", DisplayLogic);
}
/** Get Display Logic.
@return If the Field is displayed, the result determines if the field is actually displayed */
public String GetDisplayLogic() 
{
return (String)Get_Value("DisplayLogic");
}
/** Set Length.
@param FieldLength Length of the column in the database */
public void SetFieldLength (int FieldLength)
{
Set_Value ("FieldLength", FieldLength);
}
/** Get Length.
@return Length of the column in the database */
public int GetFieldLength() 
{
Object ii = Get_Value("FieldLength");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Encrypted.
@param IsEncrypted Display or Storage is encrypted */
public void SetIsEncrypted (Boolean IsEncrypted)
{
Set_Value ("IsEncrypted", IsEncrypted);
}
/** Get Encrypted.
@return Display or Storage is encrypted */
public Boolean IsEncrypted() 
{
Object oo = Get_Value("IsEncrypted");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Field Only.
@param IsFieldOnly Label is not displayed */
public void SetIsFieldOnly (Boolean IsFieldOnly)
{
Set_Value ("IsFieldOnly", IsFieldOnly);
}
/** Get Field Only.
@return Label is not displayed */
public Boolean IsFieldOnly() 
{
Object oo = Get_Value("IsFieldOnly");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Heading only.
@param IsHeading Field without Column - Only label is displayed */
public void SetIsHeading (Boolean IsHeading)
{
Set_Value ("IsHeading", IsHeading);
}
/** Get Heading only.
@return Field without Column - Only label is displayed */
public Boolean IsHeading() 
{
Object oo = Get_Value("IsHeading");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Mandatory.
@param IsMandatory Data is required in this column */
public void SetIsMandatory (Boolean IsMandatory)
{
Set_Value ("IsMandatory", IsMandatory);
}
/** Get Mandatory.
@return Data is required in this column */
public Boolean IsMandatory() 
{
Object oo = Get_Value("IsMandatory");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Read Only.
@param IsReadOnly Field is read only */
public void SetIsReadOnly (Boolean IsReadOnly)
{
Set_Value ("IsReadOnly", IsReadOnly);
}
/** Get Read Only.
@return Field is read only */
public Boolean IsReadOnly() 
{
Object oo = Get_Value("IsReadOnly");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Same Line.
@param IsSameLine Displayed on same line as previous field */
public void SetIsSameLine (Boolean IsSameLine)
{
Set_Value ("IsSameLine", IsSameLine);
}
/** Get Same Line.
@return Displayed on same line as previous field */
public Boolean IsSameLine() 
{
Object oo = Get_Value("IsSameLine");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Updateable.
@param IsUpdateable Determines, if the field can be updated */
public void SetIsUpdateable (Boolean IsUpdateable)
{
Set_Value ("IsUpdateable", IsUpdateable);
}
/** Get Updateable.
@return Determines, if the field can be updated */
public Boolean IsUpdateable() 
{
Object oo = Get_Value("IsUpdateable");
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
/** Set Value Format.
@param VFormat Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public void SetVFormat (String VFormat)
{
if (VFormat != null && VFormat.Length > 60)
{
log.Warning("Length > 60 - truncated");
VFormat = VFormat.Substring(0,60);
}
Set_Value ("VFormat", VFormat);
}
/** Get Value Format.
@return Format of the value;
 Can contain fixed format elements, Variables: "_lLoOaAcCa09" */
public String GetVFormat() 
{
return (String)Get_Value("VFormat");
}
/** Set Max. Value.
@param ValueMax Maximum Value for a field */
public void SetValueMax (String ValueMax)
{
if (ValueMax != null && ValueMax.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMax = ValueMax.Substring(0,20);
}
Set_Value ("ValueMax", ValueMax);
}
/** Get Max. Value.
@return Maximum Value for a field */
public String GetValueMax() 
{
return (String)Get_Value("ValueMax");
}
/** Set Min. Value.
@param ValueMin Minimum Value for a field */
public void SetValueMin (String ValueMin)
{
if (ValueMin != null && ValueMin.Length > 20)
{
log.Warning("Length > 20 - truncated");
ValueMin = ValueMin.Substring(0,20);
}
Set_Value ("ValueMin", ValueMin);
}
/** Get Min. Value.
@return Minimum Value for a field */
public String GetValueMin() 
{
return (String)Get_Value("ValueMin");
}
}

}
