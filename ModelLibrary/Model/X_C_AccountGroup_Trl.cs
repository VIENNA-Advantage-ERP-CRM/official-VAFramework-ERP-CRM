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
/** Generated Model for C_AccountGroup_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_AccountGroup_Trl : PO
{
public X_C_AccountGroup_Trl (Context ctx, int C_AccountGroup_Trl_ID, Trx trxName) : base (ctx, C_AccountGroup_Trl_ID, trxName)
{
/** if (C_AccountGroup_Trl_ID == 0)
{
SetC_AccountGroup_ID (0);
SetC_AccountGroup_Trl_ID (0);
SetName (null);
}
 */
}
public X_C_AccountGroup_Trl (Ctx ctx, int C_AccountGroup_Trl_ID, Trx trxName) : base (ctx, C_AccountGroup_Trl_ID, trxName)
{
/** if (C_AccountGroup_Trl_ID == 0)
{
SetC_AccountGroup_ID (0);
SetC_AccountGroup_Trl_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_AccountGroup_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_AccountGroup_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27634044506984L;
/** Last Updated Timestamp 11/3/2012 10:36:30 AM */
public static long updatedMS = 1351919190195L;
/** AD_Table_ID=1000381 */
public static int Table_ID;
 // =1000381;

/** TableName=C_AccountGroup_Trl */
public static String Table_Name="C_AccountGroup_Trl";

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
StringBuilder sb = new StringBuilder ("X_C_AccountGroup_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language != null && AD_Language.Length > 10)
{
log.Warning("Length > 10 - truncated");
AD_Language = AD_Language.Substring(0,10);
}
Set_Value ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set C_AccountGroup_ID.
@param C_AccountGroup_ID C_AccountGroup_ID */
public void SetC_AccountGroup_ID (int C_AccountGroup_ID)
{
if (C_AccountGroup_ID < 1) throw new ArgumentException ("C_AccountGroup_ID is mandatory.");
Set_ValueNoCheck ("C_AccountGroup_ID", C_AccountGroup_ID);
}
/** Get C_AccountGroup_ID.
@return C_AccountGroup_ID */
public int GetC_AccountGroup_ID() 
{
Object ii = Get_Value("C_AccountGroup_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set C_AccountGroup_Trl_ID.
@param C_AccountGroup_Trl_ID C_AccountGroup_Trl_ID */
public void SetC_AccountGroup_Trl_ID (int C_AccountGroup_Trl_ID)
{
if (C_AccountGroup_Trl_ID < 1) throw new ArgumentException ("C_AccountGroup_Trl_ID is mandatory.");
Set_ValueNoCheck ("C_AccountGroup_Trl_ID", C_AccountGroup_Trl_ID);
}
/** Get C_AccountGroup_Trl_ID.
@return C_AccountGroup_Trl_ID */
public int GetC_AccountGroup_Trl_ID() 
{
Object ii = Get_Value("C_AccountGroup_Trl_ID");
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
