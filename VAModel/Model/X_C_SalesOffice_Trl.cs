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
/** Generated Model for C_SalesOffice_Trl
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_C_SalesOffice_Trl : PO
{
public X_C_SalesOffice_Trl (Context ctx, int C_SalesOffice_Trl_ID, Trx trxName) : base (ctx, C_SalesOffice_Trl_ID, trxName)
{
/** if (C_SalesOffice_Trl_ID == 0)
{
SetAD_Language (null);
SetC_SalesOffice_ID (0);
SetIsTranslated (true);	// Y
SetName (null);
SetPrintName (null);
}
 */
}
public X_C_SalesOffice_Trl (Ctx ctx, int C_SalesOffice_Trl_ID, Trx trxName) : base (ctx, C_SalesOffice_Trl_ID, trxName)
{
/** if (C_SalesOffice_Trl_ID == 0)
{
SetAD_Language (null);
SetC_SalesOffice_ID (0);
SetIsTranslated (true);	// Y
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
public X_C_SalesOffice_Trl (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_SalesOffice_Trl (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_C_SalesOffice_Trl (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_C_SalesOffice_Trl()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27716737543101L;
/** Last Updated Timestamp 6/18/2015 12:53:47 PM */
public static long updatedMS = 1434612226312L;
/** AD_Table_ID=1000468 */
public static int Table_ID;
 // =1000468;

/** TableName=C_SalesOffice_Trl */
public static String Table_Name="C_SalesOffice_Trl";

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
protected override POInfo InitPO (Context ctx)
{
POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);
return poi;
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
StringBuilder sb = new StringBuilder ("X_C_SalesOffice_Trl[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AD_Language AD_Reference_ID=106 */
public static int AD_LANGUAGE_AD_Reference_ID=106;
/** Set Language.
@param AD_Language Language for this entity */
public void SetAD_Language (String AD_Language)
{
if (AD_Language.Length > 10)
{
log.Warning("Length > 10 - truncated");
AD_Language = AD_Language.Substring(0,10);
}
Set_ValueNoCheck ("AD_Language", AD_Language);
}
/** Get Language.
@return Language for this entity */
public String GetAD_Language() 
{
return (String)Get_Value("AD_Language");
}
/** Set Sales Office.
@param C_SalesOffice_ID Sales Office */
public void SetC_SalesOffice_ID (int C_SalesOffice_ID)
{
if (C_SalesOffice_ID < 1) throw new ArgumentException ("C_SalesOffice_ID is mandatory.");
Set_ValueNoCheck ("C_SalesOffice_ID", C_SalesOffice_ID);
}
/** Get Sales Office.
@return Sales Office */
public int GetC_SalesOffice_ID() 
{
Object ii = Get_Value("C_SalesOffice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
if (Name.Length > 100)
{
log.Warning("Length > 100 - truncated");
Name = Name.Substring(0,100);
}
Set_Value ("Name", Name);
}
/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() 
{
return (String)Get_Value("Name");
}
/** Set Print Text.
@param PrintName The label text to be printed on a document or correspondence. */
public void SetPrintName (String PrintName)
{
if (PrintName == null) throw new ArgumentException ("PrintName is mandatory.");
if (PrintName.Length > 100)
{
log.Warning("Length > 100 - truncated");
PrintName = PrintName.Substring(0,100);
}
Set_Value ("PrintName", PrintName);
}
/** Get Print Text.
@return The label text to be printed on a document or correspondence. */
public String GetPrintName() 
{
return (String)Get_Value("PrintName");
}
}

}
