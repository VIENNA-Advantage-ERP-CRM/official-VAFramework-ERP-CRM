namespace ViennaAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_ProductContainer
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ProductContainer : PO{public X_M_ProductContainer (Context ctx, int M_ProductContainer_ID, Trx trxName) : base (ctx, M_ProductContainer_ID, trxName){/** if (M_ProductContainer_ID == 0){SetM_Locator_ID (0);// -1
SetM_ProductContainer_ID (0);SetM_Warehouse_ID (0);// -1
SetName (null);} */
}public X_M_ProductContainer (Ctx ctx, int M_ProductContainer_ID, Trx trxName) : base (ctx, M_ProductContainer_ID, trxName){/** if (M_ProductContainer_ID == 0){SetM_Locator_ID (0);// -1
SetM_ProductContainer_ID (0);SetM_Warehouse_ID (0);// -1
SetName (null);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductContainer (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductContainer (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ProductContainer (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ProductContainer(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27817490194683L;/** Last Updated Timestamp 8/27/2018 3:44:38 PM */
public static long updatedMS = 1535364877894L;/** AD_Table_ID=1000524 */
public static int Table_ID; // =1000524;
/** TableName=M_ProductContainer */
public static String Table_Name="M_ProductContainer";
protected static KeyNamePair model;protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
protected override int Get_AccessLevel(){return Convert.ToInt32(accessLevel.ToString());}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Context ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Load Meta Data
@param ctx context
@return PO Info
*/
protected override POInfo InitPO (Ctx ctx){POInfo poi = POInfo.GetPOInfo (ctx, Table_ID);return poi;}/** Info
@return info
*/
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_ProductContainer[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Description.
@param Description Optional short description of the record */
public void SetDescription (String Description){if (Description != null && Description.Length > 255){log.Warning("Length > 255 - truncated");Description = Description.Substring(0,255);}Set_Value ("Description", Description);}/** Get Description.
@return Optional short description of the record */
public String GetDescription() {return (String)Get_Value("Description");}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Height.
@param Height Height of the container */
public void SetHeight (Decimal? Height){Set_Value ("Height", (Decimal?)Height);}/** Get Height.
@return Height of the container */
public Decimal GetHeight() {Object bd =Get_Value("Height");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}/** Set Default.
@param IsDefault Default value */
public void SetIsDefault (Boolean IsDefault){Set_Value ("IsDefault", IsDefault);}/** Get Default.
@return Default value */
public Boolean IsDefault() {Object oo = Get_Value("IsDefault");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID){if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");Set_Value ("M_Locator_ID", M_Locator_ID);}/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() {Object ii = Get_Value("M_Locator_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product Container.
@param M_ProductContainer_ID Product Container */
public void SetM_ProductContainer_ID (int M_ProductContainer_ID){if (M_ProductContainer_ID < 1) throw new ArgumentException ("M_ProductContainer_ID is mandatory.");Set_ValueNoCheck ("M_ProductContainer_ID", M_ProductContainer_ID);}/** Get Product Container.
@return Product Container */
public int GetM_ProductContainer_ID() {Object ii = Get_Value("M_ProductContainer_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID){if (M_Warehouse_ID < 1) throw new ArgumentException ("M_Warehouse_ID is mandatory.");Set_Value ("M_Warehouse_ID", M_Warehouse_ID);}/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() {Object ii = Get_Value("M_Warehouse_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Name.
@param Name Alphanumeric identifier of the entity */
public void SetName (String Name){if (Name == null) throw new ArgumentException ("Name is mandatory.");if (Name.Length > 100){log.Warning("Length > 100 - truncated");Name = Name.Substring(0,100);}Set_Value ("Name", Name);}/** Get Name.
@return Alphanumeric identifier of the entity */
public String GetName() {return (String)Get_Value("Name");}/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() {return new KeyNamePair(Get_ID(), GetName());}
/** Ref_M_Container_ID AD_Reference_ID=1000205 */
public static int REF_M_CONTAINER_ID_AD_Reference_ID=1000205;/** Set Parent Container.
@param Ref_M_Container_ID Use to store parent container id */
public void SetRef_M_Container_ID (int Ref_M_Container_ID){if (Ref_M_Container_ID <= 0) Set_Value ("Ref_M_Container_ID", null);else
Set_Value ("Ref_M_Container_ID", Ref_M_Container_ID);}/** Get Parent Container.
@return Use to store parent container id */
public int GetRef_M_Container_ID() {Object ii = Get_Value("Ref_M_Container_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Search Key.
@param Value Search key for the record in the format required - must be unique */
public void SetValue (String Value){if (Value != null && Value.Length > 100){log.Warning("Length > 100 - truncated");Value = Value.Substring(0,100);}Set_Value ("Value", Value);}/** Get Search Key.
@return Search key for the record in the format required - must be unique */
public String GetValue() {return (String)Get_Value("Value");}/** Set Width.
@param Width Width of the container */
public void SetWidth (Decimal? Width){Set_Value ("Width", (Decimal?)Width);}/** Get Width.
@return Width of the container */
public Decimal GetWidth() {Object bd =Get_Value("Width");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}