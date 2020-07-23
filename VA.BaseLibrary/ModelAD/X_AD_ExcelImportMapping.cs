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
/** Generated Model for AD_ExcelImportMapping
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_AD_ExcelImportMapping : PO
{
public X_AD_ExcelImportMapping (Context ctx, int AD_ExcelImportMapping_ID, Trx trxName) : base (ctx, AD_ExcelImportMapping_ID, trxName)
{
/** if (AD_ExcelImportMapping_ID == 0)
{
SetAD_ExcelImportMapping_ID (0);
SetAD_ExcelImport_ID (0);
}
 */
}
public X_AD_ExcelImportMapping (Ctx ctx, int AD_ExcelImportMapping_ID, Trx trxName) : base (ctx, AD_ExcelImportMapping_ID, trxName)
{
/** if (AD_ExcelImportMapping_ID == 0)
{
SetAD_ExcelImportMapping_ID (0);
SetAD_ExcelImport_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExcelImportMapping (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExcelImportMapping (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_AD_ExcelImportMapping (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_AD_ExcelImportMapping()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27623011438779L;
/** Last Updated Timestamp 6/28/2012 5:52:02 PM */
public static long updatedMS = 1340886121990L;
/** AD_Table_ID=1000044 */
public static int Table_ID;
 // =1000044;

/** TableName=AD_ExcelImportMapping */
public static String Table_Name="AD_ExcelImportMapping";

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
StringBuilder sb = new StringBuilder ("X_AD_ExcelImportMapping[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param AD_Column_ID Column in the table */
public void SetAD_Column_ID (int AD_Column_ID)
{
if (AD_Column_ID <= 0) Set_Value ("AD_Column_ID", null);
else
Set_Value ("AD_Column_ID", AD_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetAD_Column_ID() 
{
Object ii = Get_Value("AD_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Excel Import Mapping.
@param AD_ExcelImportMapping_ID Excel Import Mapping */
public void SetAD_ExcelImportMapping_ID (int AD_ExcelImportMapping_ID)
{
if (AD_ExcelImportMapping_ID < 1) throw new ArgumentException ("AD_ExcelImportMapping_ID is mandatory.");
Set_ValueNoCheck ("AD_ExcelImportMapping_ID", AD_ExcelImportMapping_ID);
}
/** Get Excel Import Mapping.
@return Excel Import Mapping */
public int GetAD_ExcelImportMapping_ID() 
{
Object ii = Get_Value("AD_ExcelImportMapping_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Excel Import.
@param AD_ExcelImport_ID Excel Import */
public void SetAD_ExcelImport_ID (int AD_ExcelImport_ID)
{
if (AD_ExcelImport_ID < 1) throw new ArgumentException ("AD_ExcelImport_ID is mandatory.");
Set_ValueNoCheck ("AD_ExcelImport_ID", AD_ExcelImport_ID);
}
/** Get Excel Import.
@return Excel Import */
public int GetAD_ExcelImport_ID() 
{
Object ii = Get_Value("AD_ExcelImport_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Tab.
@param AD_Tab_ID Tab within a Window */
public void SetAD_Tab_ID (int AD_Tab_ID)
{
if (AD_Tab_ID <= 0) Set_Value ("AD_Tab_ID", null);
else
Set_Value ("AD_Tab_ID", AD_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetAD_Tab_ID() 
{
Object ii = Get_Value("AD_Tab_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Excel Col Index.
@param ExcelColIndex Excel Col Index */
public void SetExcelColIndex (Decimal? ExcelColIndex)
{
Set_Value ("ExcelColIndex", (Decimal?)ExcelColIndex);
}
/** Get Excel Col Index.
@return Excel Col Index */
public Decimal GetExcelColIndex() 
{
Object bd =Get_Value("ExcelColIndex");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
/** Set Excel Col Name.
@param ExcelColName Excel Col Name */
public void SetExcelColName (String ExcelColName)
{
if (ExcelColName != null && ExcelColName.Length > 50)
{
log.Warning("Length > 50 - truncated");
ExcelColName = ExcelColName.Substring(0,50);
}
Set_Value ("ExcelColName", ExcelColName);
}
/** Get Excel Col Name.
@return Excel Col Name */
public String GetExcelColName() 
{
return (String)Get_Value("ExcelColName");
}

public void SetDefaultData(String DefaultData)
{
    if (DefaultData != null && DefaultData.Length > 100)
    {
        log.Warning("Length > 100 - truncated");
        DefaultData = DefaultData.Substring(0, 100);
    }
    Set_Value("DefaultData", DefaultData);
}
public String GetDefaultData()
{
    return (String)Get_Value("DefaultData");
}
/** Set Identifier.
@param IsIdentifier This column is part of the record identifier */
public void SetIsIdentifier (Boolean IsIdentifier)
{
Set_Value ("IsIdentifier", IsIdentifier);
}
/** Get Identifier.
@return This column is part of the record identifier */
public Boolean IsIdentifier() 
{
Object oo = Get_Value("IsIdentifier");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Location Mapped.
@param IsLocationMapped Location Mapped */
public void SetIsLocationMapped (Boolean IsLocationMapped)
{
Set_Value ("IsLocationMapped", IsLocationMapped);
}
/** Get Location Mapped.
@return Location Mapped */
public Boolean IsLocationMapped() 
{
Object oo = Get_Value("IsLocationMapped");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
