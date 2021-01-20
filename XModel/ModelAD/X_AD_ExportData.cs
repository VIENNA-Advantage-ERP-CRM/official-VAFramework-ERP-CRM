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
/** Generated Model for VAF_ExportData
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_VAF_ExportData : PO
{
public X_VAF_ExportData (Context ctx, int VAF_ExportData_ID, Trx trxName) : base (ctx, VAF_ExportData_ID, trxName)
{
/** if (VAF_ExportData_ID == 0)
{
SetVAF_ExportData_ID (0);
}
 */
}

public X_VAF_ExportData (Ctx ctx, int VAF_ExportData_ID, Trx trxName) : base (ctx, VAF_ExportData_ID, trxName)
{
/** if (VAF_ExportData_ID == 0)
{
SetVAF_ExportData_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExportData (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExportData (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExportData (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ExportData()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27577392900846L;
/** Last Updated Timestamp 1/17/2011 6:03:04 PM */
public static long updatedMS = 1295267584057L;
/** VAF_TableView_ID=1000172 */
public static int Table_ID;
 // =1000172;

/** TableName=VAF_ExportData */
public static String Table_Name="VAF_ExportData";

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
StringBuilder sb = new StringBuilder ("X_VAF_ExportData[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Primay Col 1.
@param AD_ColOne_ID Primay Col 1 */
public void SetAD_ColOne_ID (int AD_ColOne_ID)
{
if (AD_ColOne_ID <= 0) Set_Value ("AD_ColOne_ID", null);
else
Set_Value ("AD_ColOne_ID", AD_ColOne_ID);
}
/** Get Primay Col 1.
@return Primay Col 1 */
public int GetAD_ColOne_ID() 
{
Object ii = Get_Value("AD_ColOne_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set VAF_ExportData_ID.
@param VAF_ExportData_ID VAF_ExportData_ID */
public void SetVAF_ExportData_ID (int VAF_ExportData_ID)
{
if (VAF_ExportData_ID < 1) throw new ArgumentException ("VAF_ExportData_ID is mandatory.");
Set_ValueNoCheck ("VAF_ExportData_ID", VAF_ExportData_ID);
}
/** Get VAF_ExportData_ID.
@return VAF_ExportData_ID */
public int GetVAF_ExportData_ID() 
{
Object ii = Get_Value("VAF_ExportData_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetVAF_TableView_ID (int VAF_TableView_ID)
{
if (VAF_TableView_ID <= 0) Set_Value ("VAF_TableView_ID", null);
else
Set_Value ("VAF_TableView_ID", VAF_TableView_ID);
}
/** Get Table.
@return Database Table information */
public int GetVAF_TableView_ID() 
{
Object ii = Get_Value("VAF_TableView_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID < 0) Set_Value ("Record_ID", null);
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
/** Set Table.
@param VAF_TableView_ID Database Table information */
public void SetAD_ModuleInfo_ID(int AD_ModuleInfo_ID)
{
    if (AD_ModuleInfo_ID <= 0) Set_Value("AD_ModuleInfo_ID", null);
    else
        Set_Value("AD_ModuleInfo_ID", AD_ModuleInfo_ID);
}
/** Get Table.
@return Database Table information */
public int GetAD_ModuleInfo_ID()
{
    Object ii = Get_Value("AD_ModuleInfo_ID");
    if (ii == null) return 0;
    return Convert.ToInt32(ii);
}
}

}
