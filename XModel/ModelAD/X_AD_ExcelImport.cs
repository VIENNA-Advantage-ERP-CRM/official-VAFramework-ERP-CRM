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
/** Generated Model for VAF_ExcelImport
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ExcelImport : PO
{
public X_VAF_ExcelImport (Context ctx, int VAF_ExcelImport_ID, Trx trxName) : base (ctx, VAF_ExcelImport_ID, trxName)
{
/** if (VAF_ExcelImport_ID == 0)
{
SetVAF_ExcelImport_ID (0);
SetAD_Window_ID (0);
SetFileName (null);
SetName (null);
}
 */
}
public X_VAF_ExcelImport (Ctx ctx, int VAF_ExcelImport_ID, Trx trxName) : base (ctx, VAF_ExcelImport_ID, trxName)
{
/** if (VAF_ExcelImport_ID == 0)
{
SetVAF_ExcelImport_ID (0);
SetAD_Window_ID (0);
SetFileName (null);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImport (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImport (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImport (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ExcelImport()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27622995518994L;
/** Last Updated Timestamp 6/28/2012 1:26:42 PM */
public static long updatedMS = 1340870202205L;
/** VAF_TableView_ID=1000042 */
public static int Table_ID;
 // =1000042;

/** TableName=VAF_ExcelImport */
public static String Table_Name="VAF_ExcelImport";

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
StringBuilder sb = new StringBuilder ("X_VAF_ExcelImport[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Excel Import.
@param VAF_ExcelImport_ID Excel Import */
public void SetVAF_ExcelImport_ID (int VAF_ExcelImport_ID)
{
if (VAF_ExcelImport_ID < 1) throw new ArgumentException ("VAF_ExcelImport_ID is mandatory.");
Set_ValueNoCheck ("VAF_ExcelImport_ID", VAF_ExcelImport_ID);
}
/** Get Excel Import.
@return Excel Import */
public int GetVAF_ExcelImport_ID() 
{
Object ii = Get_Value("VAF_ExcelImport_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID < 1) throw new ArgumentException ("AD_Window_ID is mandatory.");
Set_Value ("AD_Window_ID", AD_Window_ID);
}
/** Get Window.
@return Data entry or display window */
public int GetAD_Window_ID() 
{
Object ii = Get_Value("AD_Window_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set File Name.
@param FileName Name of the local file or URL */
public void SetFileName (String FileName)
{
if (FileName == null) throw new ArgumentException ("FileName is mandatory.");
if (FileName.Length > 75)
{
log.Warning("Length > 75 - truncated");
FileName = FileName.Substring(0,75);
}
Set_Value ("FileName", FileName);
}
/** Get File Name.
@return Name of the local file or URL */
public String GetFileName() 
{
return (String)Get_Value("FileName");
}
/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name)
{
if (Name == null) throw new ArgumentException ("Name is mandatory.");
if (Name.Length > 150)
{
log.Warning("Length > 150 - truncated");
Name = Name.Substring(0,150);
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
