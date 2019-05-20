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
/** Generated Model for I_TLForm_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_I_TLForm_Trl : PO
{
public X_I_TLForm_Trl (Context ctx, int I_TLForm_Trl_ID, Trx trxName) : base (ctx, I_TLForm_Trl_ID, trxName)
{
/** if (I_TLForm_Trl_ID == 0)
{
SetAD_Form_ID (0);
SetI_TLLanguage_ID (0);
SetIsTranslated (false);
SetName (null);
}
 */
}
public X_I_TLForm_Trl (Ctx ctx, int I_TLForm_Trl_ID, Trx trxName) : base (ctx, I_TLForm_Trl_ID, trxName)
{
/** if (I_TLForm_Trl_ID == 0)
{
SetAD_Form_ID (0);
SetI_TLLanguage_ID (0);
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
public X_I_TLForm_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLForm_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_I_TLForm_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_I_TLForm_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27638799885442L;
/** Last Updated Timestamp 12/28/2012 11:32:48 AM */
public static long updatedMS = 1356674568653L;
/** AD_Table_ID=1000411 */
public static int Table_ID;
 // =1000411;

/** TableName=I_TLForm_Trl */
public static String Table_Name="I_TLForm_Trl";

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
StringBuilder sb = new StringBuilder ("X_I_TLForm_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Special Form.
@param AD_Form_ID Special Form */
public void SetAD_Form_ID (int AD_Form_ID)
{
if (AD_Form_ID < 1) throw new ArgumentException ("AD_Form_ID is mandatory.");
Set_ValueNoCheck ("AD_Form_ID", AD_Form_ID);
}
/** Get Special Form.
@return Special Form */
public int GetAD_Form_ID() 
{
Object ii = Get_Value("AD_Form_ID");
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
