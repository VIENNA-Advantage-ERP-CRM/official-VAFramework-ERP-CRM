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
/** Generated Model for VAF_ExcelImportMapping
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAF_ExcelImportMapping : PO
{
public X_VAF_ExcelImportMapping (Context ctx, int VAF_ExcelImportMapping_ID, Trx trxName) : base (ctx, VAF_ExcelImportMapping_ID, trxName)
{
/** if (VAF_ExcelImportMapping_ID == 0)
{
SetVAF_ExcelImportMapping_ID (0);
SetVAF_ExcelImport_ID (0);
}
 */
}
public X_VAF_ExcelImportMapping (Ctx ctx, int VAF_ExcelImportMapping_ID, Trx trxName) : base (ctx, VAF_ExcelImportMapping_ID, trxName)
{
/** if (VAF_ExcelImportMapping_ID == 0)
{
SetVAF_ExcelImportMapping_ID (0);
SetVAF_ExcelImport_ID (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImportMapping (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImportMapping (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_VAF_ExcelImportMapping (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_VAF_ExcelImportMapping()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID = 27623011438779L;
/** Last Updated Timestamp 6/28/2012 5:52:02 PM */
public static long updatedMS = 1340886121990L;
/** VAF_TableView_ID=1000044 */
public static int Table_ID;
 // =1000044;

/** TableName=VAF_ExcelImportMapping */
public static String Table_Name="VAF_ExcelImportMapping";

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
StringBuilder sb = new StringBuilder ("X_VAF_ExcelImportMapping[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Column.
@param VAF_Column_ID Column in the table */
public void SetVAF_Column_ID (int VAF_Column_ID)
{
if (VAF_Column_ID <= 0) Set_Value ("VAF_Column_ID", null);
else
Set_Value ("VAF_Column_ID", VAF_Column_ID);
}
/** Get Column.
@return Column in the table */
public int GetVAF_Column_ID() 
{
Object ii = Get_Value("VAF_Column_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Excel Import Mapping.
@param VAF_ExcelImportMapping_ID Excel Import Mapping */
public void SetVAF_ExcelImportMapping_ID (int VAF_ExcelImportMapping_ID)
{
if (VAF_ExcelImportMapping_ID < 1) throw new ArgumentException ("VAF_ExcelImportMapping_ID is mandatory.");
Set_ValueNoCheck ("VAF_ExcelImportMapping_ID", VAF_ExcelImportMapping_ID);
}
/** Get Excel Import Mapping.
@return Excel Import Mapping */
public int GetVAF_ExcelImportMapping_ID() 
{
Object ii = Get_Value("VAF_ExcelImportMapping_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Tab.
@param VAF_Tab_ID Tab within a Window */
public void SetVAF_Tab_ID (int VAF_Tab_ID)
{
if (VAF_Tab_ID <= 0) Set_Value ("VAF_Tab_ID", null);
else
Set_Value ("VAF_Tab_ID", VAF_Tab_ID);
}
/** Get Tab.
@return Tab within a Window */
public int GetVAF_Tab_ID() 
{
Object ii = Get_Value("VAF_Tab_ID");
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
