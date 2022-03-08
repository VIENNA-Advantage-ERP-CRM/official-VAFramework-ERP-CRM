namespace VAdvantage.Model{
/** Generated Model - DO NOT CHANGE */
using System;using System.Text;using VAdvantage.DataBase;using VAdvantage.Common;using VAdvantage.Classes;using VAdvantage.Process;using VAdvantage.Model;using VAdvantage.Utility;using System.Data;/** Generated Model for M_CostQueueTransaction
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_CostQueueTransaction : PO{public X_M_CostQueueTransaction (Context ctx, int M_CostQueueTransaction_ID, Trx trxName) : base (ctx, M_CostQueueTransaction_ID, trxName){/** if (M_CostQueueTransaction_ID == 0){SetM_CostQueueTransaction_ID (0);SetM_CostQueue_ID (0);} */
}public X_M_CostQueueTransaction (Ctx ctx, int M_CostQueueTransaction_ID, Trx trxName) : base (ctx, M_CostQueueTransaction_ID, trxName){/** if (M_CostQueueTransaction_ID == 0){SetM_CostQueueTransaction_ID (0);SetM_CostQueue_ID (0);} */
}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostQueueTransaction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostQueueTransaction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName){}/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_CostQueueTransaction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName){}/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_CostQueueTransaction(){ Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID,Table_Name);}/** Serial Version No */
static long serialVersionUID = 27893851374279L;/** Last Updated Timestamp 1/27/2021 5:40:57 AM */
public static long updatedMS = 1611726057490L;/** AD_Table_ID=1000547 */
public static int Table_ID; // =1000547;
/** TableName=M_CostQueueTransaction */
public static String Table_Name="M_CostQueueTransaction";
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
public override String ToString(){StringBuilder sb = new StringBuilder ("X_M_CostQueueTransaction[").Append(Get_ID()).Append("]");return sb.ToString();}/** Set Project Issue.
@param C_ProjectIssue_ID Project Issues (Material, Labor) */
public void SetC_ProjectIssue_ID (int C_ProjectIssue_ID){if (C_ProjectIssue_ID <= 0) Set_Value ("C_ProjectIssue_ID", null);else
Set_Value ("C_ProjectIssue_ID", C_ProjectIssue_ID);}/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetC_ProjectIssue_ID() {Object ii = Get_Value("C_ProjectIssue_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Export.
@param Export_ID Export */
public void SetExport_ID (String Export_ID){if (Export_ID != null && Export_ID.Length > 50){log.Warning("Length > 50 - truncated");Export_ID = Export_ID.Substring(0,50);}Set_Value ("Export_ID", Export_ID);}/** Get Export.
@return Export */
public String GetExport_ID() {return (String)Get_Value("Export_ID");}/** Set Internal Use.
@param IsInternalUse The Record is internal use */
public void SetIsInternalUse (Boolean IsInternalUse){Set_Value ("IsInternalUse", IsInternalUse);}/** Get Internal Use.
@return The Record is internal use */
public Boolean IsInternalUse() {Object oo = Get_Value("IsInternalUse");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Return Transaction.
@param IsReturnTrx This is a return transaction */
public void SetIsReturnTrx (Boolean IsReturnTrx){Set_Value ("IsReturnTrx", IsReturnTrx);}/** Get Return Transaction.
@return This is a return transaction */
public Boolean IsReturnTrx() {Object oo = Get_Value("IsReturnTrx");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
public void SetIsSOTrx (Boolean IsSOTrx){Set_Value ("IsSOTrx", IsSOTrx);}/** Get Sales Transaction.
@return This is a Sales Transaction */
public Boolean IsSOTrx() {Object oo = Get_Value("IsSOTrx");if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo);}return false;}/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID){if (M_AttributeSetInstance_ID <= 0) Set_Value ("M_AttributeSetInstance_ID", null);else
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);}/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() {Object ii = Get_Value("M_AttributeSetInstance_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cost Queue Transaction.
@param M_CostQueueTransaction_ID Cost Queue Transactional Document */
public void SetM_CostQueueTransaction_ID (int M_CostQueueTransaction_ID){if (M_CostQueueTransaction_ID < 1) throw new ArgumentException ("M_CostQueueTransaction_ID is mandatory.");Set_ValueNoCheck ("M_CostQueueTransaction_ID", M_CostQueueTransaction_ID);}/** Get Cost Queue Transaction.
@return Cost Queue Transactional Document */
public int GetM_CostQueueTransaction_ID() {Object ii = Get_Value("M_CostQueueTransaction_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Cost Queue.
@param M_CostQueue_ID FiFo/LiFo Cost Queue */
public void SetM_CostQueue_ID (int M_CostQueue_ID){if (M_CostQueue_ID < 1) throw new ArgumentException ("M_CostQueue_ID is mandatory.");Set_ValueNoCheck ("M_CostQueue_ID", M_CostQueue_ID);}/** Get Cost Queue.
@return FiFo/LiFo Cost Queue */
public int GetM_CostQueue_ID() {Object ii = Get_Value("M_CostQueue_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
public void SetM_InOutLine_ID (int M_InOutLine_ID){if (M_InOutLine_ID <= 0) Set_Value ("M_InOutLine_ID", null);else
Set_Value ("M_InOutLine_ID", M_InOutLine_ID);}/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
public int GetM_InOutLine_ID() {Object ii = Get_Value("M_InOutLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Phys Inventory Line.
@param M_InventoryLine_ID Unique line in an Inventory document */
public void SetM_InventoryLine_ID (int M_InventoryLine_ID){if (M_InventoryLine_ID <= 0) Set_Value ("M_InventoryLine_ID", null);else
Set_Value ("M_InventoryLine_ID", M_InventoryLine_ID);}/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetM_InventoryLine_ID() {Object ii = Get_Value("M_InventoryLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Move Line.
@param M_MovementLine_ID Inventory Move document Line */
public void SetM_MovementLine_ID (int M_MovementLine_ID){if (M_MovementLine_ID <= 0) Set_Value ("M_MovementLine_ID", null);else
Set_Value ("M_MovementLine_ID", M_MovementLine_ID);}/** Get Move Line.
@return Inventory Move document Line */
public int GetM_MovementLine_ID() {Object ii = Get_Value("M_MovementLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID){if (M_Product_ID <= 0) Set_Value ("M_Product_ID", null);else
Set_Value ("M_Product_ID", M_Product_ID);}/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() {Object ii = Get_Value("M_Product_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Production Line.
@param M_ProductionLine_ID Document Line representing a production */
public void SetM_ProductionLine_ID (int M_ProductionLine_ID){if (M_ProductionLine_ID <= 0) Set_Value ("M_ProductionLine_ID", null);else
Set_Value ("M_ProductionLine_ID", M_ProductionLine_ID);}/** Get Production Line.
@return Document Line representing a production */
public int GetM_ProductionLine_ID() {Object ii = Get_Value("M_ProductionLine_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Warehouse.
@param M_Warehouse_ID Storage Warehouse and Service Point */
public void SetM_Warehouse_ID (int M_Warehouse_ID){if (M_Warehouse_ID <= 0) Set_Value ("M_Warehouse_ID", null);else
Set_Value ("M_Warehouse_ID", M_Warehouse_ID);}/** Get Warehouse.
@return Storage Warehouse and Service Point */
public int GetM_Warehouse_ID() {Object ii = Get_Value("M_Warehouse_ID");if (ii == null) return 0;return Convert.ToInt32(ii);}/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate){Set_Value ("MovementDate", (DateTime?)MovementDate);}/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() {return (DateTime?)Get_Value("MovementDate");}/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty){Set_Value ("MovementQty", (Decimal?)MovementQty);}/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() {Object bd =Get_Value("MovementQty");if (bd == null) return Env.ZERO;return  Convert.ToDecimal(bd);}}
}