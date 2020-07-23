namespace ViennaAdvantage.Model
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
/** Generated Model for VADMS_AttachMetaData
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VADMS_AttachMetaData : PO
{
public X_VADMS_AttachMetaData (Context ctx, int VADMS_AttachMetaData_ID, Trx trxName) : base (ctx, VADMS_AttachMetaData_ID, trxName)
{
/** if (VADMS_AttachMetaData_ID == 0)
{
SetVADMS_AttachMetaData_ID (0);
}
 */
}
public X_VADMS_AttachMetaData(Ctx ctx, int VADMS_AttachMetaData_ID, Trx trxName)
    : base(ctx, VADMS_AttachMetaData_ID, trxName)
{
/** if (VADMS_AttachMetaData_ID == 0)
{
SetVADMS_AttachMetaData_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_AttachMetaData(Context ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_AttachMetaData(Ctx ctx, DataRow rs, Trx trxName)
    : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VADMS_AttachMetaData(Ctx ctx, IDataReader dr, Trx trxName)
    : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VADMS_AttachMetaData()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
static long serialVersionUID = 27723132101570L;
/** Last Updated Timestamp 8/31/2015 1:09:44 PM */
public static long updatedMS = 1441006784781L;
/** AD_Table_ID=1000522 */
public static int Table_ID;
 // =1000522;

/** TableName=VADMS_AttachMetaData */
public static String Table_Name="VADMS_AttachMetaData";

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
StringBuilder sb = new StringBuilder ("X_VADMS_AttachMetaData[").Append(Get_ID()).Append("]");
return sb.ToString();
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
/** Set VADMS_AttachMetaData_ID.
@param VADMS_AttachMetaData_ID VADMS_AttachMetaData_ID */
public void SetVADMS_AttachMetaData_ID (int VADMS_AttachMetaData_ID)
{
if (VADMS_AttachMetaData_ID < 1) throw new ArgumentException ("VADMS_AttachMetaData_ID is mandatory.");
Set_ValueNoCheck ("VADMS_AttachMetaData_ID", VADMS_AttachMetaData_ID);
}
/** Get VADMS_AttachMetaData_ID.
@return VADMS_AttachMetaData_ID */
public int GetVADMS_AttachMetaData_ID() 
{
Object ii = Get_Value("VADMS_AttachMetaData_ID");
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
/** Set VADMS_MetaData_ID.
@param VADMS_MetaData_ID VADMS_MetaData_ID */
public void SetVADMS_MetaData_ID (int VADMS_MetaData_ID)
{
if (VADMS_MetaData_ID <= 0) Set_Value ("VADMS_MetaData_ID", null);
else
Set_Value ("VADMS_MetaData_ID", VADMS_MetaData_ID);
}
/** Get VADMS_MetaData_ID.
@return VADMS_MetaData_ID */
public int GetVADMS_MetaData_ID() 
{
Object ii = Get_Value("VADMS_MetaData_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VADMS_WindowDocLink_ID.
@param VADMS_WindowDocLink_ID VADMS_WindowDocLink_ID */
public void SetVADMS_WindowDocLink_ID (int VADMS_WindowDocLink_ID)
{
if (VADMS_WindowDocLink_ID <= 0) Set_Value ("VADMS_WindowDocLink_ID", null);
else
Set_Value ("VADMS_WindowDocLink_ID", VADMS_WindowDocLink_ID);
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
