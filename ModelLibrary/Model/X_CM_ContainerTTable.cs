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
/** Generated Model for CM_ContainerTTable
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_CM_ContainerTTable : PO
{
public X_CM_ContainerTTable (Context ctx, int CM_ContainerTTable_ID, Trx trxName) : base (ctx, CM_ContainerTTable_ID, trxName)
{
/** if (CM_ContainerTTable_ID == 0)
{
SetCM_ContainerTTable_ID (0);
SetCM_Container_ID (0);
SetCM_TemplateTable_ID (0);
SetName (null);
}
 */
}
public X_CM_ContainerTTable (Ctx ctx, int CM_ContainerTTable_ID, Trx trxName) : base (ctx, CM_ContainerTTable_ID, trxName)
{
/** if (CM_ContainerTTable_ID == 0)
{
SetCM_ContainerTTable_ID (0);
SetCM_Container_ID (0);
SetCM_TemplateTable_ID (0);
SetName (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ContainerTTable (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ContainerTTable (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_CM_ContainerTTable (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_CM_ContainerTTable()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514368943L;
/** Last Updated Timestamp 7/29/2010 1:07:32 PM */
public static long updatedMS = 1280389052154L;
/** AD_Table_ID=880 */
public static int Table_ID;
 // =880;

/** TableName=CM_ContainerTTable */
public static String Table_Name="CM_ContainerTTable";

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
StringBuilder sb = new StringBuilder ("X_CM_ContainerTTable[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Container T.Table.
@param CM_ContainerTTable_ID Container Template Table */
public void SetCM_ContainerTTable_ID (int CM_ContainerTTable_ID)
{
if (CM_ContainerTTable_ID < 1) throw new ArgumentException ("CM_ContainerTTable_ID is mandatory.");
Set_ValueNoCheck ("CM_ContainerTTable_ID", CM_ContainerTTable_ID);
}
/** Get Container T.Table.
@return Container Template Table */
public int GetCM_ContainerTTable_ID() 
{
Object ii = Get_Value("CM_ContainerTTable_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Web Container.
@param CM_Container_ID Web Container contains content like images, text etc. */
public void SetCM_Container_ID (int CM_Container_ID)
{
if (CM_Container_ID < 1) throw new ArgumentException ("CM_Container_ID is mandatory.");
Set_ValueNoCheck ("CM_Container_ID", CM_Container_ID);
}
/** Get Web Container.
@return Web Container contains content like images, text etc. */
public int GetCM_Container_ID() 
{
Object ii = Get_Value("CM_Container_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Template Table.
@param CM_TemplateTable_ID CM Template Table Link */
public void SetCM_TemplateTable_ID (int CM_TemplateTable_ID)
{
if (CM_TemplateTable_ID < 1) throw new ArgumentException ("CM_TemplateTable_ID is mandatory.");
Set_ValueNoCheck ("CM_TemplateTable_ID", CM_TemplateTable_ID);
}
/** Get Template Table.
@return CM Template Table Link */
public int GetCM_TemplateTable_ID() 
{
Object ii = Get_Value("CM_TemplateTable_ID");
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
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetName());
}
/** Set Other SQL Clause.
@param OtherClause Other SQL Clause */
public void SetOtherClause (String OtherClause)
{
if (OtherClause != null && OtherClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
OtherClause = OtherClause.Substring(0,2000);
}
Set_Value ("OtherClause", OtherClause);
}
/** Get Other SQL Clause.
@return Other SQL Clause */
public String GetOtherClause() 
{
return (String)Get_Value("OtherClause");
}
/** Set Record ID.
@param Record_ID Direct internal record ID */
public void SetRecord_ID (int Record_ID)
{
if (Record_ID <= 0) Set_ValueNoCheck ("Record_ID", null);
else
Set_ValueNoCheck ("Record_ID", Record_ID);
}
/** Get Record ID.
@return Direct internal record ID */
public int GetRecord_ID() 
{
Object ii = Get_Value("Record_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause != null && WhereClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
WhereClause = WhereClause.Substring(0,2000);
}
Set_Value ("WhereClause", WhereClause);
}
/** Get Sql WHERE.
@return Fully qualified SQL WHERE clause */
public String GetWhereClause() 
{
return (String)Get_Value("WhereClause");
}
}

}
