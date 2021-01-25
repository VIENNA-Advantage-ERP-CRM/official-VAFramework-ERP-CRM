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
/** Generated Model for VAB_AccountGroup_TL
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_AccountGroup_TL : PO
{
public X_VAB_AccountGroup_TL (Context ctx, int VAB_AccountGroup_TL_ID, Trx trxName) : base (ctx, VAB_AccountGroup_TL_ID, trxName)
{
/** if (VAB_AccountGroup_TL_ID == 0)
{
SetVAB_AccountGroup_ID (0);
SetVAB_AccountGroup_TL_ID (0);
SetName (null);
}
 */
}
public X_VAB_AccountGroup_TL (Ctx ctx, int VAB_AccountGroup_TL_ID, Trx trxName) : base (ctx, VAB_AccountGroup_TL_ID, trxName)
{
/** if (VAB_AccountGroup_TL_ID == 0)
{
SetVAB_AccountGroup_ID (0);
SetVAB_AccountGroup_TL_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroup_TL (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroup_TL (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_AccountGroup_TL (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_AccountGroup_TL()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634044506984L;
/** Last Updated Timestamp 11/3/2012 10:36:30 AM */
public static long updatedMS = 1351919190195L;
/** VAF_TableView_ID=1000381 */
public static int Table_ID;
 // =1000381;

/** TableName=VAB_AccountGroup_TL */
public static String Table_Name="VAB_AccountGroup_TL";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(3);
/** AccessLevel
@return 3 - Client - Org 
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
StringBuilder sb = new StringBuilder ("X_VAB_AccountGroup_TL[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_Language VAF_Control_Ref_ID=106 */
public static int VAF_LANGUAGE_VAF_Control_Ref_ID=106;
/** Set Language.
@param VAF_Language Language for this entity */
public void SetVAF_Language (String VAF_Language)
{
if (VAF_Language != null && VAF_Language.Length > 10)
{
log.Warning("Length > 10 - truncated");
VAF_Language = VAF_Language.Substring(0,10);
}
Set_Value ("VAF_Language", VAF_Language);
}
/** Get Language.
@return Language for this entity */
public String GetVAF_Language() 
{
return (String)Get_Value("VAF_Language");
}
/** Set VAB_AccountGroup_ID.
@param VAB_AccountGroup_ID VAB_AccountGroup_ID */
public void SetVAB_AccountGroup_ID (int VAB_AccountGroup_ID)
{
if (VAB_AccountGroup_ID < 1) throw new ArgumentException ("VAB_AccountGroup_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountGroup_ID", VAB_AccountGroup_ID);
}
/** Get VAB_AccountGroup_ID.
@return VAB_AccountGroup_ID */
public int GetVAB_AccountGroup_ID() 
{
Object ii = Get_Value("VAB_AccountGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAB_AccountGroup_TL_ID.
@param VAB_AccountGroup_TL_ID VAB_AccountGroup_TL_ID */
public void SetVAB_AccountGroup_TL_ID (int VAB_AccountGroup_TL_ID)
{
if (VAB_AccountGroup_TL_ID < 1) throw new ArgumentException ("VAB_AccountGroup_TL_ID is mandatory.");
Set_ValueNoCheck ("VAB_AccountGroup_TL_ID", VAB_AccountGroup_TL_ID);
}
/** Get VAB_AccountGroup_TL_ID.
@return VAB_AccountGroup_TL_ID */
public int GetVAB_AccountGroup_TL_ID() 
{
Object ii = Get_Value("VAB_AccountGroup_TL_ID");
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
@param Name Name */
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
@return Name */
public String GetName() 
{
return (String)Get_Value("Name");
}
}

}
