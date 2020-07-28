namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_ContainerStorage
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_ContainerStorage : PO{public X_M_ContainerStorage (Context ctx, int M_ContainerStorage_ID, Trx trxName) : base (ctx, M_ContainerStorage_ID, trxName){/** if (M_ContainerStorage_ID == 0){SetM_ContainerStorage_ID (0);SetM_Product_ID (0);} */
}public X_M_ContainerStorage (Ctx ctx, int M_ContainerStorage_ID, Trx trxName) : base (ctx, M_ContainerStorage_ID, trxName){/** if (M_ContainerStorage_ID == 0){SetM_ContainerStorage_ID (0);SetM_Product_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ContainerStorage (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set  
@param trxName transaction
*/
public X_M_ContainerStorage (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_ContainerStorage (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_ContainerStorage(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27818161238381L;/** Last Updated Timestamp 9/4/2018 10:08:41 AM */
public static long updatedMS = 1536035921592L;/** AD_Table_ID=1000525 */
public static int Table_ID; // =1000525;
/** TableName=M_ContainerStorage */
public static String Table_Name="M_ContainerStorage";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_ContainerStorage[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Material Policy Date.
@param MMPolicyDate Time used for LIFO and FIFO Material Policy */
public void SetMMPolicyDate (DateTime? MMPolicyDate){Set_Value ("MMPolicyDate", (DateTime?)MMPolicyDate);}/** Get Material Policy Date.
@return Time used for LIFO and FIFO Material Policy */
public DateTime? GetMMPolicyDate() {return (DateTime?)Get_Value("MMPolicyDate");}/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID){if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);}/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() {Object ii = Get_Value("M_AttributeSetInstance_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set M_ContainerStorage_ID.
@param M_ContainerStorage_ID M_ContainerStorage_ID */
public void SetM_ContainerStorage_ID (int M_ContainerStorage_ID){if (M_ContainerStorage_ID < 1) throw new ArgumentException ("M_ContainerStorage_ID is mandatory.");Set_ValueNoCheck ("M_ContainerStorage_ID", M_ContainerStorage_ID);}/** Get M_ContainerStorage_ID.
@return M_ContainerStorage_ID */
public int GetM_ContainerStorage_ID() {Object ii = Get_Value("M_ContainerStorage_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID){if (M_Locator_ID <= 0) Set_Value ("M_Locator_ID", null);else
Set_Value ("M_Locator_ID", M_Locator_ID);}/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() {Object ii = Get_Value("M_Locator_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product Container.
@param M_ProductContainer_ID Product Container */
public void SetM_ProductContainer_ID (int M_ProductContainer_ID){if (M_ProductContainer_ID <= 0) Set_Value ("M_ProductContainer_ID", null);else
Set_Value ("M_ProductContainer_ID", M_ProductContainer_ID);}/** Get Product Container.
@return Product Container */
public int GetM_ProductContainer_ID() {Object ii = Get_Value("M_ProductContainer_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty){Set_Value ("Qty", (Decimal?)Qty);}/** Get Quantity.
@return Quantity */
public Decimal GetQty() {Object bd =Get_Value("Qty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}
/** Set Actual Quantity.@param ActualQty The actual quantity */
public void SetActualQty(Decimal? ActualQty) { Set_Value("ActualQty", (Decimal?)ActualQty); }/** Get Actual Quantity.
@return The actual quantity */
public Decimal GetActualQty() { Object bd = Get_Value("ActualQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
/** Set Physical Inventory.@param IsPhysicalInventory Physical Inventory */
public void SetIsPhysicalInventory(Boolean IsPhysicalInventory) { Set_Value("IsPhysicalInventory", IsPhysicalInventory); }
    /** Get Physical Inventory.@return Physical Inventory */
public Boolean IsPhysicalInventory() { Object oo = Get_Value("IsPhysicalInventory"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
/** Set Qty Calculation.@param QtyCalculation Qty Calculation */
public void SetQtyCalculation(Decimal? QtyCalculation) { Set_Value("QtyCalculation", (Decimal?)QtyCalculation); }
    /** Get Qty Calculation.@return Qty Calculation */
public Decimal GetQtyCalculation() { Object bd = Get_Value("QtyCalculation"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
}
}