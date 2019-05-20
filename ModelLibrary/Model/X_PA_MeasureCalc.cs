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
/** Generated Model for PA_MeasureCalc
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_PA_MeasureCalc : PO
{
public X_PA_MeasureCalc (Context ctx, int PA_MeasureCalc_ID, Trx trxName) : base (ctx, PA_MeasureCalc_ID, trxName)
{
/** if (PA_MeasureCalc_ID == 0)
{
SetAD_Table_ID (0);
SetDateColumn (null);	// x.Date
SetEntityType (null);	// U
SetKeyColumn (null);
SetName (null);
SetPA_MeasureCalc_ID (0);
SetSelectClause (null);	// SELECT ... FROM ...
SetWhereClause (null);	// WHERE ...
}
 */
}
public X_PA_MeasureCalc (Ctx ctx, int PA_MeasureCalc_ID, Trx trxName) : base (ctx, PA_MeasureCalc_ID, trxName)
{
/** if (PA_MeasureCalc_ID == 0)
{
SetAD_Table_ID (0);
SetDateColumn (null);	// x.Date
SetEntityType (null);	// U
SetKeyColumn (null);
SetName (null);
SetPA_MeasureCalc_ID (0);
SetSelectClause (null);	// SELECT ... FROM ...
SetWhereClause (null);	// WHERE ...
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_MeasureCalc (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_MeasureCalc (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_PA_MeasureCalc (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_PA_MeasureCalc()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381951L;
/** Last Updated Timestamp 7/29/2010 1:07:45 PM */
public static long updatedMS = 1280389065162L;
/** AD_Table_ID=442 */
public static int Table_ID;
 // =442;

/** TableName=PA_MeasureCalc */
public static String Table_Name="PA_MeasureCalc";

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
StringBuilder sb = new StringBuilder ("X_PA_MeasureCalc[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Table.
@param AD_Table_ID Database Table information */
public void SetAD_Table_ID (int AD_Table_ID)
{
if (AD_Table_ID < 1) throw new ArgumentException ("AD_Table_ID is mandatory.");
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
/** Set B.Partner Column.
@param BPartnerColumn Fully qualified Business Partner key column (C_BPartner_ID) */
public void SetBPartnerColumn (String BPartnerColumn)
{
if (BPartnerColumn != null && BPartnerColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
BPartnerColumn = BPartnerColumn.Substring(0,60);
}
Set_Value ("BPartnerColumn", BPartnerColumn);
}
/** Get B.Partner Column.
@return Fully qualified Business Partner key column (C_BPartner_ID) */
public String GetBPartnerColumn() 
{
return (String)Get_Value("BPartnerColumn");
}
/** Set Date Column.
@param DateColumn Fully qualified date column */
public void SetDateColumn (String DateColumn)
{
if (DateColumn == null) throw new ArgumentException ("DateColumn is mandatory.");
if (DateColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
DateColumn = DateColumn.Substring(0,60);
}
Set_Value ("DateColumn", DateColumn);
}
/** Get Date Column.
@return Fully qualified date column */
public String GetDateColumn() 
{
return (String)Get_Value("DateColumn");
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

/** EntityType AD_Reference_ID=389 */
public static int ENTITYTYPE_AD_Reference_ID=389;
/** Set Entity Type.
@param EntityType Dictionary Entity Type;
 Determines ownership and synchronization */
public void SetEntityType (String EntityType)
{
if (EntityType.Length > 4)
{
log.Warning("Length > 4 - truncated");
EntityType = EntityType.Substring(0,4);
}
Set_Value ("EntityType", EntityType);
}
/** Get Entity Type.
@return Dictionary Entity Type;
 Determines ownership and synchronization */
public String GetEntityType() 
{
return (String)Get_Value("EntityType");
}
/** Set Key Column.
@param KeyColumn Key Column for Table */
public void SetKeyColumn (String KeyColumn)
{
if (KeyColumn == null) throw new ArgumentException ("KeyColumn is mandatory.");
if (KeyColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
KeyColumn = KeyColumn.Substring(0,60);
}
Set_Value ("KeyColumn", KeyColumn);
}
/** Get Key Column.
@return Key Column for Table */
public String GetKeyColumn() 
{
return (String)Get_Value("KeyColumn");
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
/** Set Org Column.
@param OrgColumn Fully qualified Organization column (AD_Org_ID) */
public void SetOrgColumn (String OrgColumn)
{
if (OrgColumn != null && OrgColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
OrgColumn = OrgColumn.Substring(0,60);
}
Set_Value ("OrgColumn", OrgColumn);
}
/** Get Org Column.
@return Fully qualified Organization column (AD_Org_ID) */
public String GetOrgColumn() 
{
return (String)Get_Value("OrgColumn");
}
/** Set Measure Calculation.
@param PA_MeasureCalc_ID Calculation method for measuring performance */
public void SetPA_MeasureCalc_ID (int PA_MeasureCalc_ID)
{
if (PA_MeasureCalc_ID < 1) throw new ArgumentException ("PA_MeasureCalc_ID is mandatory.");
Set_ValueNoCheck ("PA_MeasureCalc_ID", PA_MeasureCalc_ID);
}
/** Get Measure Calculation.
@return Calculation method for measuring performance */
public int GetPA_MeasureCalc_ID() 
{
Object ii = Get_Value("PA_MeasureCalc_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Column.
@param ProductColumn Fully qualified Product column (M_Product_ID) */
public void SetProductColumn (String ProductColumn)
{
if (ProductColumn != null && ProductColumn.Length > 60)
{
log.Warning("Length > 60 - truncated");
ProductColumn = ProductColumn.Substring(0,60);
}
Set_Value ("ProductColumn", ProductColumn);
}
/** Get Product Column.
@return Fully qualified Product column (M_Product_ID) */
public String GetProductColumn() 
{
return (String)Get_Value("ProductColumn");
}
/** Set Sql SELECT.
@param SelectClause SQL SELECT clause */
public void SetSelectClause (String SelectClause)
{
if (SelectClause == null) throw new ArgumentException ("SelectClause is mandatory.");
if (SelectClause.Length > 2000)
{
log.Warning("Length > 2000 - truncated");
SelectClause = SelectClause.Substring(0,2000);
}
Set_Value ("SelectClause", SelectClause);
}
/** Get Sql SELECT.
@return SQL SELECT clause */
public String GetSelectClause() 
{
return (String)Get_Value("SelectClause");
}
/** Set Sql WHERE.
@param WhereClause Fully qualified SQL WHERE clause */
public void SetWhereClause (String WhereClause)
{
if (WhereClause == null) throw new ArgumentException ("WhereClause is mandatory.");
if (WhereClause.Length > 2000)
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
