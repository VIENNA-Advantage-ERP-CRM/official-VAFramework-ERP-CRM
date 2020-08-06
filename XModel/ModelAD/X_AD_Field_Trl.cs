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
using System.Data;
    using VAdvantage.Utility;
/** Generated Model for AD_Field_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_AD_Field_Trl : PO
{
public X_AD_Field_Trl (Context ctx, int AD_Field_Trl_ID, Trx trxName) : base (ctx, AD_Field_Trl_ID, trxName)
{
/** if (AD_Field_Trl_ID == 0)
{
SetAD_Field_ID (0);
SetAD_Language (null);
SetIsTranslated (false);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_Field_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Serial Version No */
//static long serialVersionUID = 27519927395714L;
/** Last Updated Timestamp 3/23/2009 3:24:39 PM */
public static long updatedMS = 1237802078925L;
/** AD_Table_ID=127 */
public static int Table_ID=127;

/** TableName=AD_Field_Trl */
public static String Table_Name="AD_Field_Trl";

protected static KeyNamePair model = new KeyNamePair(127,"AD_Field_Trl");

protected Decimal accessLevel = new Decimal(4);
/** AccessLevel
@return 4 - System 
*/
protected override int Get_AccessLevel()
{
return int.Parse(accessLevel.ToString());
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
StringBuilder sb = new StringBuilder ("X_AD_Field_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Field.
@param AD_Field_ID Field on a tab in a window */
public void SetAD_Field_ID (int AD_Field_ID)
{
if (AD_Field_ID < 1) throw new ArgumentException ("AD_Field_ID is mandatory.");
Set_ValueNoCheck ("AD_Field_ID", AD_Field_ID);
}
/** Get Field.
@return Field on a tab in a window */
public int GetAD_Field_ID() 
{
int? ii = (int)Get_Value("AD_Field_ID");
if (ii == null) return 0;
return (int)ii;
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language.Length > 5)
{
//log.warning("Length > 5 - truncated");
AD_Language = AD_Language.Substring(0,5);
}
Set_ValueNoCheck ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetAD_Language().ToString());
}
/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description)
{
if (Description != null && Description.Length > 255)
{
//log.warning("Length > 255 - truncated");
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
//log.warning("Length > 2000 - truncated");
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
/** Set Translated.
@param IsTranslated This column is translated */
public void SetIsTranslated (Boolean IsTranslated)
{
Set_Value ("IsTranslated", IsTranslated);
}
/** Get Translated.
@return This column is translated */
public Boolean IsTranslated() 
{
Object oo = Get_Value("IsTranslated");
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
//log.warning("Length > 60 - truncated");
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
}

}
