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
/** Generated Model for M_PerpetualInv
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_PerpetualInv : PO
{
public X_M_PerpetualInv (Context ctx, int M_PerpetualInv_ID, Trx trxName) : base (ctx, M_PerpetualInv_ID, trxName)
{
/** if (M_PerpetualInv_ID == 0)
{
SetCountHighMovement (false);
SetDateNextRun (DateTime.Now);
SetM_PerpetualInv_ID (0);
SetName (null);
SetNoInventoryCount (0);	// 1
SetNoProductCount (0);	// 1
SetNumberOfRuns (0);	// 1
}
 */
}
public X_M_PerpetualInv (Ctx ctx, int M_PerpetualInv_ID, Trx trxName) : base (ctx, M_PerpetualInv_ID, trxName)
{
/** if (M_PerpetualInv_ID == 0)
{
SetCountHighMovement (false);
SetDateNextRun (DateTime.Now);
SetM_PerpetualInv_ID (0);
SetName (null);
SetNoInventoryCount (0);	// 1
SetNoProductCount (0);	// 1
SetNumberOfRuns (0);	// 1
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PerpetualInv (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PerpetualInv (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_PerpetualInv (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_PerpetualInv()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380337L;
/** Last Updated Timestamp 7/29/2010 1:07:43 PM */
public static long updatedMS = 1280389063548L;
/** AD_Table_ID=342 */
public static int Table_ID;
 // =342;

/** TableName=M_PerpetualInv */
public static String Table_Name="M_PerpetualInv";

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
StringBuilder sb = new StringBuilder ("X_M_PerpetualInv[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Count high turnover items.
@param CountHighMovement Count High Movement products */
public void SetCountHighMovement (Boolean CountHighMovement)
{
Set_Value ("CountHighMovement", CountHighMovement);
}
/** Get Count high turnover items.
@return Count High Movement products */
public Boolean IsCountHighMovement() 
{
Object oo = Get_Value("CountHighMovement");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Date last run.
@param DateLastRun Date the process was last run. */
public void SetDateLastRun (DateTime? DateLastRun)
{
Set_ValueNoCheck ("DateLastRun", (DateTime?)DateLastRun);
}
/** Get Date last run.
@return Date the process was last run. */
public DateTime? GetDateLastRun() 
{
return (DateTime?)Get_Value("DateLastRun");
}
/** Set Date next run.
@param DateNextRun Date the process will run next */
public void SetDateNextRun (DateTime? DateNextRun)
{
if (DateNextRun == null) throw new ArgumentException ("DateNextRun is mandatory.");
Set_ValueNoCheck ("DateNextRun", (DateTime?)DateNextRun);
}
/** Get Date next run.
@return Date the process will run next */
public DateTime? GetDateNextRun() 
{
return (DateTime?)Get_Value("DateNextRun");
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
/** Set Perpetual Inventory.
@param M_PerpetualInv_ID Rules for generating physical inventory */
public void SetM_PerpetualInv_ID (int M_PerpetualInv_ID)
{
if (M_PerpetualInv_ID < 1) throw new ArgumentException ("M_PerpetualInv_ID is mandatory.");
Set_ValueNoCheck ("M_PerpetualInv_ID", M_PerpetualInv_ID);
}
/** Get Perpetual Inventory.
@return Rules for generating physical inventory */
public int GetM_PerpetualInv_ID() 
{
Object ii = Get_Value("M_PerpetualInv_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID <= 0) Set_Value ("M_Product_Category_ID", null);
else
Set_Value ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID)
{
if (M_Warehouse_ID <= 0) Set_Value ("M_Warehouse_ID", null);
else
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);
}
/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() 
{
Object ii = Get_Value("M_Warehouse_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Number of Inventory counts.
@param NoInventoryCount Frequency of inventory counts per year */
public void SetNoInventoryCount (int NoInventoryCount)
{
Set_Value ("NoInventoryCount", NoInventoryCount);
}
/** Get Number of Inventory counts.
@return Frequency of inventory counts per year */
public int GetNoInventoryCount() 
{
Object ii = Get_Value("NoInventoryCount");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Number of Product counts.
@param NoProductCount Frequency of product counts per year */
public void SetNoProductCount (int NoProductCount)
{
Set_Value ("NoProductCount", NoProductCount);
}
/** Get Number of Product counts.
@return Frequency of product counts per year */
public int GetNoProductCount() 
{
Object ii = Get_Value("NoProductCount");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Number of runs.
@param NumberOfRuns Frequency of processing Perpetual Inventory */
public void SetNumberOfRuns (int NumberOfRuns)
{
Set_Value ("NumberOfRuns", NumberOfRuns);
}
/** Get Number of runs.
@return Frequency of processing Perpetual Inventory */
public int GetNumberOfRuns() 
{
Object ii = Get_Value("NumberOfRuns");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Process Now.
@param Processing Process Now */
public void SetProcessing (Boolean Processing)
{
Set_Value ("Processing", Processing);
}
/** Get Process Now.
@return Process Now */
public Boolean IsProcessing() 
{
Object oo = Get_Value("Processing");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
}

}
