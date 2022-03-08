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
/** Generated Model for I_TLElement_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_TLElement_Trl : PO
{
public X_I_TLElement_Trl (Context ctx, int I_TLElement_Trl_ID, Trx trxName) : base (ctx, I_TLElement_Trl_ID, trxName)
{
/** if (I_TLElement_Trl_ID == 0)
{
SetAD_Element_ID (0);
SetI_TLLanguage_ID (0);
SetIsTranslated (false);
SetName (null);
SetPrintName (null);
}
 */
}
public X_I_TLElement_Trl (Ctx ctx, int I_TLElement_Trl_ID, Trx trxName) : base (ctx, I_TLElement_Trl_ID, trxName)
{
/** if (I_TLElement_Trl_ID == 0)
{
SetAD_Element_ID (0);
SetI_TLLanguage_ID (0);
SetIsTranslated (false);
SetName (null);
SetPrintName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLElement_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLElement_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLElement_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_TLElement_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638799867526L;
/** Last Updated Timestamp 12/28/2012 11:32:30 AM */
public static long updatedMS = 1356674550737L;
/** AD_Table_ID=1000404 */
public static int Table_ID;
 // =1000404;

/** TableName=I_TLElement_Trl */
public static String Table_Name="I_TLElement_Trl";

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
StringBuilder sb = new StringBuilder ("X_I_TLElement_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set System Element.
@param AD_Element_ID System Element enables the central maintenance of column description and help. */
public void SetAD_Element_ID (int AD_Element_ID)
{
if (AD_Element_ID < 1) throw new ArgumentException ("AD_Element_ID is mandatory.");
Set_ValueNoCheck ("AD_Element_ID", AD_Element_ID);
}
/** Get System Element.
@return System Element enables the central maintenance of column description and help. */
public int GetAD_Element_ID() 
{
Object ii = Get_Value("AD_Element_ID");
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
/** Set I_TLLanguage_ID.
@param I_TLLanguage_ID I_TLLanguage_ID */
public void SetI_TLLanguage_ID (int I_TLLanguage_ID)
{
if (I_TLLanguage_ID < 1) throw new ArgumentException ("I_TLLanguage_ID is mandatory.");
Set_ValueNoCheck ("I_TLLanguage_ID", I_TLLanguage_ID);
}
/** Get I_TLLanguage_ID.
@return I_TLLanguage_ID */
public int GetI_TLLanguage_ID() 
{
Object ii = Get_Value("I_TLLanguage_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set PO Description.
@param PO_Description Description in PO Screens */
public void SetPO_Description (String PO_Description)
{
if (PO_Description != null && PO_Description.Length > 255)
{
log.Warning("Length > 255 - truncated");
PO_Description = PO_Description.Substring(0,255);
}
Set_Value ("PO_Description", PO_Description);
}
/** Get PO Description.
@return Description in PO Screens */
public String GetPO_Description() 
{
return (String)Get_Value("PO_Description");
}
/** Set PO Help.
@param PO_Help Help for PO Screens */
public void SetPO_Help (String PO_Help)
{
if (PO_Help != null && PO_Help.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
PO_Help = PO_Help.Substring(0,2000);
}
Set_Value ("PO_Help", PO_Help);
}
/** Get PO Help.
@return Help for PO Screens */
public String GetPO_Help() 
{
return (String)Get_Value("PO_Help");
}
/** Set PO Name.
@param PO_Name Name on PO Screens */
public void SetPO_Name (String PO_Name)
{
if (PO_Name != null && PO_Name.Length > 60)
{
log.Warning("Length > 60 - truncated");
PO_Name = PO_Name.Substring(0,60);
}
Set_Value ("PO_Name", PO_Name);
}
/** Get PO Name.
@return Name on PO Screens */
public String GetPO_Name() 
{
return (String)Get_Value("PO_Name");
}
/** Set PO Print name.
@param PO_PrintName Print name on PO Screens/Reports */
public void SetPO_PrintName (String PO_PrintName)
{
if (PO_PrintName != null && PO_PrintName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PO_PrintName = PO_PrintName.Substring(0,60);
}
Set_Value ("PO_PrintName", PO_PrintName);
}
/** Get PO Print name.
@return Print name on PO Screens/Reports */
public String GetPO_PrintName() 
{
return (String)Get_Value("PO_PrintName");
}
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName == null) throw new ArgumentException ("PrintName is mandatory.");
if (PrintName.Length > 60)
{
log.Warning("Length > 60 - truncated");
PrintName = PrintName.Substring(0,60);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
/** Set Continue Translation.
@param IsContinueTranslation Continue Translation */
public void SetIsContinueTranslation(Boolean IsContinueTranslation)
{
    Set_Value("IsContinueTranslation", IsContinueTranslation);
}
/** Get Continue Translation.
@return Continue Translation */
public Boolean IsContinueTranslation()
{
    Object oo = Get_Value("IsContinueTranslation");
    if (oo != null)
    {
        if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
        return "Y".Equals(oo);
    }
    return false;
}


}

}
