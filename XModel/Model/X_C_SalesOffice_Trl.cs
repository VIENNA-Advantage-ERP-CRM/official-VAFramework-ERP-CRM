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
/** Generated Model for VAB_SalesOffice_TL
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAB_SalesOffice_TL : PO
{
public X_VAB_SalesOffice_TL (Context ctx, int VAB_SalesOffice_TL_ID, Trx trxName) : base (ctx, VAB_SalesOffice_TL_ID, trxName)
{
/** if (VAB_SalesOffice_TL_ID == 0)
{
SetVAF_Language (null);
SetVAB_SalesOffice_ID (0);
SetIsTranslated (true);	// Y
SetName (null);
SetPrintName (null);
}
 */
}
public X_VAB_SalesOffice_TL (Ctx ctx, int VAB_SalesOffice_TL_ID, Trx trxName) : base (ctx, VAB_SalesOffice_TL_ID, trxName)
{
/** if (VAB_SalesOffice_TL_ID == 0)
{
SetVAF_Language (null);
SetVAB_SalesOffice_ID (0);
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
public X_VAB_SalesOffice_TL (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SalesOffice_TL (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAB_SalesOffice_TL (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAB_SalesOffice_TL()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27716737543101L;
/** Last Updated Timestamp 6/18/2015 12:53:47 PM */
public static long updatedMS = 1434612226312L;
/** VAF_TableView_ID=1000468 */
public static int Table_ID;
 // =1000468;

/** TableName=VAB_SalesOffice_TL */
public static String Table_Name="VAB_SalesOffice_TL";

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
StringBuilder sb = new StringBuilder ("X_VAB_SalesOffice_TL[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** VAF_Language VAF_Control_Ref_ID=106 */
public static int VAF_LANGUAGE_VAF_Control_Ref_ID=106;
/** Set Language.
@param VAF_Language Language for this entity */
public void SetVAF_Language (String VAF_Language)
{
if (VAF_Language.Length > 10)
{
log.Warning("Length > 10 - truncated");
VAF_Language = VAF_Language.Substring(0,10);
}
Set_ValueNoCheck ("VAF_Language", VAF_Language);
}
/** Get Language.
@return Language for this entity */
public String GetVAF_Language() 
{
return (String)Get_Value("VAF_Language");
}
/** Set Sales Office.
@param VAB_SalesOffice_ID Sales Office */
public void SetVAB_SalesOffice_ID (int VAB_SalesOffice_ID)
{
if (VAB_SalesOffice_ID < 1) throw new ArgumentException ("VAB_SalesOffice_ID is mandatory.");
Set_ValueNoCheck ("VAB_SalesOffice_ID", VAB_SalesOffice_ID);
}
/** Get Sales Office.
@return Sales Office */
public int GetVAB_SalesOffice_ID() 
{
Object ii = Get_Value("VAB_SalesOffice_ID");
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
