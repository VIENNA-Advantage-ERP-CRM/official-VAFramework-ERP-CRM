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
/** Generated Model for CM_Template
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_Template : PO
{
public X_CM_Template (Context ctx, int CM_Template_ID, Trx trxName) : base (ctx, CM_Template_ID, trxName)
{
/** if (CM_Template_ID == 0)
{
SetCM_Template_ID (0);
SetIsInclude (false);
SetIsNews (false);
SetIsSummary (false);
SetIsUseAd (false);
SetIsValid (false);
SetName (null);
SetValue (null);
}
 */
}
public X_CM_Template (Ctx ctx, int CM_Template_ID, Trx trxName) : base (ctx, CM_Template_ID, trxName)
{
/** if (CM_Template_ID == 0)
{
SetCM_Template_ID (0);
SetIsInclude (false);
SetIsNews (false);
SetIsSummary (false);
SetIsUseAd (false);
SetIsValid (false);
SetName (null);
SetValue (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_Template (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_Template()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514369178L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052389L;
/** AD_Table_ID=854 */
public static int Table_ID;
 // =854;

/** TableName=CM_Template */
public static String Table_Name="CM_Template";

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
StringBuilder sb = new StringBuilder ("X_CM_Template[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Template.
@param CM_Template_ID Template defines how content is displayed */
public void SetCM_Template_ID (int CM_Template_ID)
{
if (CM_Template_ID < 1) throw new ArgumentException ("CM_Template_ID is mandatory.");
Set_ValueNoCheck ("CM_Template_ID", CM_Template_ID);
}
/** Get Template.
@return Template defines how content is displayed */
public int GetCM_Template_ID() 
{
Object ii = Get_Value("CM_Template_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Project.
@param CM_WebProject_ID A web project is the main data container for Containers, URLs, Ads, Media etc. */
public void SetCM_WebProject_ID (int CM_WebProject_ID)
{
if (CM_WebProject_ID <= 0) Set_ValueNoCheck ("CM_WebProject_ID", null);
else
Set_ValueNoCheck ("CM_WebProject_ID", CM_WebProject_ID);
}
/** Get Web Project.
@return A web project is the main data container for Containers, URLs, Ads, Media etc. */
public int GetCM_WebProject_ID() 
{
Object ii = Get_Value("CM_WebProject_ID");
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
/** Set Elements.
@param Elements Contains list of elements seperated by CR */
public void SetElements (String Elements)
{
if (Elements != null && Elements.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
Elements = Elements.Substring(0,2000);
}
Set_Value ("Elements", Elements);
}
/** Get Elements.
@return Contains list of elements seperated by CR */
public String GetElements() 
{
return (String)Get_Value("Elements");
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
/** Set Included.
@param IsInclude Defines whether this content / template is included into another one */
public void SetIsInclude (Boolean IsInclude)
{
Set_Value ("IsInclude", IsInclude);
}
/** Get Included.
@return Defines whether this content / template is included into another one */
public Boolean IsInclude() 
{
Object oo = Get_Value("IsInclude");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Uses News.
@param IsNews Template or container uses news channels */
public void SetIsNews (Boolean IsNews)
{
Set_Value ("IsNews", IsNews);
}
/** Get Uses News.
@return Template or container uses news channels */
public Boolean IsNews() 
{
Object oo = Get_Value("IsNews");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Summary Level.
@param IsSummary This is a summary entity */
public void SetIsSummary (Boolean IsSummary)
{
Set_Value ("IsSummary", IsSummary);
}
/** Get Summary Level.
@return This is a summary entity */
public Boolean IsSummary() 
{
Object oo = Get_Value("IsSummary");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Use Ad.
@param IsUseAd Whether or not this templates uses Ad's */
public void SetIsUseAd (Boolean IsUseAd)
{
Set_Value ("IsUseAd", IsUseAd);
}
/** Get Use Ad.
@return Whether or not this templates uses Ad's */
public Boolean IsUseAd() 
{
Object oo = Get_Value("IsUseAd");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Valid.
@param IsValid Element is valid */
public void SetIsValid (Boolean IsValid)
{
Set_Value ("IsValid", IsValid);
}
/** Get Valid.
@return Element is valid */
public Boolean IsValid() 
{
Object oo = Get_Value("IsValid");
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
/** Set TemplateXST.
@param TemplateXST Contains the template code itself */
public void SetTemplateXST (String TemplateXST)
{
Set_Value ("TemplateXST", TemplateXST);
}
/** Get TemplateXST.
@return Contains the template code itself */
public String GetTemplateXST() 
{
return (String)Get_Value("TemplateXST");
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
