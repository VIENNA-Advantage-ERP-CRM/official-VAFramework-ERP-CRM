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
/** Generated Model for VADMS_WindowDocLink
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VADMS_WindowDocLink : PO
{
public X_VADMS_WindowDocLink (Context ctx, int VADMS_WindowDocLink_ID, Trx trxName) : base (ctx, VADMS_WindowDocLink_ID, trxName)
{
/** if (VADMS_WindowDocLink_ID == 0)
{
SetVADMS_WindowDocLink_ID (0);
}
 */
}
public X_VADMS_WindowDocLink (Ctx ctx, int VADMS_WindowDocLink_ID, Trx trxName) : base (ctx, VADMS_WindowDocLink_ID, trxName)
{
/** if (VADMS_WindowDocLink_ID == 0)
{
SetVADMS_WindowDocLink_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_WindowDocLink (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_WindowDocLink (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_WindowDocLink (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VADMS_WindowDocLink()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27662728790351L;
/** Last Updated Timestamp 10/1/2013 10:27:53 AM */
public static long updatedMS = 1380603473562L;
/** AD_Table_ID=1000413 */
public static int Table_ID;
 // =1000413;

/** TableName=VADMS_WindowDocLink */
public static String Table_Name="VADMS_WindowDocLink";

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
StringBuilder sb = new StringBuilder ("X_VADMS_WindowDocLink[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID <= 0) Set_Value ("AD_Table_ID", null);
else
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
/** Set Window.
@param AD_Window_ID Data entry or display window */
public void SetAD_Window_ID (int AD_Window_ID)
{
if (AD_Window_ID <= 0) Set_Value ("AD_Window_ID", null);
else
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
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_Value ("Record_ID", null);
else
Set_Value ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Document.
@param VADMS_Document_ID Document */
public void SetVADMS_Document_ID (int VADMS_Document_ID)
{
if (VADMS_Document_ID <= 0) Set_Value ("VADMS_Document_ID", null);
else
Set_Value ("VADMS_Document_ID", VADMS_Document_ID);
}
/** Get Document.
@return Document */
public int GetVADMS_Document_ID() 
{
Object ii = Get_Value("VADMS_Document_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VADMS_WindowDocLink_ID.
@param VADMS_WindowDocLink_ID VADMS_WindowDocLink_ID */
public void SetVADMS_WindowDocLink_ID (int VADMS_WindowDocLink_ID)
{
if (VADMS_WindowDocLink_ID < 1) throw new ArgumentException ("VADMS_WindowDocLink_ID is mandatory.");
Set_ValueNoCheck ("VADMS_WindowDocLink_ID", VADMS_WindowDocLink_ID);
}
/** Get VADMS_WindowDocLink_ID.
@return VADMS_WindowDocLink_ID */
public int GetVADMS_WindowDocLink_ID() 
{
Object ii = Get_Value("VADMS_WindowDocLink_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
