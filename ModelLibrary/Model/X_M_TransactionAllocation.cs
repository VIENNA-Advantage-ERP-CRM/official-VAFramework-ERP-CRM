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
/** Generated Model for M_TransactionAllocation
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_TransactionAllocation : PO
{
public X_M_TransactionAllocation (Context ctx, int M_TransactionAllocation_ID, Trx trxName) : base (ctx, M_TransactionAllocation_ID, trxName)
{
/** if (M_TransactionAllocation_ID == 0)
{
SetAllocationStrategyType (null);
SetIsAllocated (false);	// N
SetIsManual (false);	// N
SetM_AttributeSetInstance_ID (0);
SetM_Product_ID (0);
SetM_Transaction_ID (0);
SetQty (0.0);
}
 */
}
public X_M_TransactionAllocation (Ctx ctx, int M_TransactionAllocation_ID, Trx trxName) : base (ctx, M_TransactionAllocation_ID, trxName)
{
/** if (M_TransactionAllocation_ID == 0)
{
SetAllocationStrategyType (null);
SetIsAllocated (false);	// N
SetIsManual (false);	// N
SetM_AttributeSetInstance_ID (0);
SetM_Product_ID (0);
SetM_Transaction_ID (0);
SetQty (0.0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_TransactionAllocation (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_TransactionAllocation (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_TransactionAllocation (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_TransactionAllocation()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514381465L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064676L;
/** AD_Table_ID=636 */
public static int Table_ID;
 // =636;

/** TableName=M_TransactionAllocation */
public static String Table_Name="M_TransactionAllocation";

protected static KeyNamePair model;
protected Decimal accessLevel = new Decimal(1);
/** AccessLevel
@return 1 - Org 
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
StringBuilder sb = new StringBuilder ("X_M_TransactionAllocation[").Append(Get_ID()).Append("]");
return sb.ToString();
}

/** AllocationStrategyType AD_Reference_ID=294 */
public static int ALLOCATIONSTRATEGYTYPE_AD_Reference_ID=294;
/** FiFo = F */
public static String ALLOCATIONSTRATEGYTYPE_FiFo = "F";
/** LiFo = L */
public static String ALLOCATIONSTRATEGYTYPE_LiFo = "L";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsAllocationStrategyTypeValid (String test)
{
return test.Equals("F") || test.Equals("L");
}
/** Set Allocation Strategy.
@param AllocationStrategyType Allocation Strategy */
public void SetAllocationStrategyType (String AllocationStrategyType)
{
if (AllocationStrategyType == null) throw new ArgumentException ("AllocationStrategyType is mandatory");
if (!IsAllocationStrategyTypeValid(AllocationStrategyType))
throw new ArgumentException ("AllocationStrategyType Invalid value - " + AllocationStrategyType + " - Reference_ID=294 - F - L");
if (AllocationStrategyType.Length > 1)
{
log.Warning("Length > 1 - truncated");
AllocationStrategyType = AllocationStrategyType.Substring(0,1);
}
Set_ValueNoCheck ("AllocationStrategyType", AllocationStrategyType);
}
/** Get Allocation Strategy.
@return Allocation Strategy */
public String GetAllocationStrategyType() 
{
return (String)Get_Value("AllocationStrategyType");
}
/** Set Allocated.
@param IsAllocated Indicates if the payment has been allocated */
public void SetIsAllocated (Boolean IsAllocated)
{
Set_Value ("IsAllocated", IsAllocated);
}
/** Get Allocated.
@return Indicates if the payment has been allocated */
public Boolean IsAllocated() 
{
Object oo = Get_Value("IsAllocated");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Manual.
@param IsManual This is a manual process */
public void SetIsManual (Boolean IsManual)
{
Set_Value ("IsManual", IsManual);
}
/** Get Manual.
@return This is a manual process */
public Boolean IsManual() 
{
Object oo = Get_Value("IsManual");
if (oo != null) 
{
 if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
 return "Y".Equals(oo);
}
return false;
}
/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
public void SetM_AttributeSetInstance_ID (int M_AttributeSetInstance_ID)
{
if (M_AttributeSetInstance_ID < 0) throw new ArgumentException ("M_AttributeSetInstance_ID is mandatory.");
Set_Value ("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
}
/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
public int GetM_AttributeSetInstance_ID() 
{
Object ii = Get_Value("M_AttributeSetInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
public void SetM_InOutLine_ID (int M_InOutLine_ID)
{
if (M_InOutLine_ID <= 0) Set_Value ("M_InOutLine_ID", null);
else
Set_Value ("M_InOutLine_ID", M_InOutLine_ID);
}
/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
public int GetM_InOutLine_ID() 
{
Object ii = Get_Value("M_InOutLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Phys Inventory Line.
@param M_InventoryLine_ID Unique line in an Inventory document */
public void SetM_InventoryLine_ID (int M_InventoryLine_ID)
{
if (M_InventoryLine_ID <= 0) Set_Value ("M_InventoryLine_ID", null);
else
Set_Value ("M_InventoryLine_ID", M_InventoryLine_ID);
}
/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
public int GetM_InventoryLine_ID() 
{
Object ii = Get_Value("M_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product.
@param M_Product_ID Product, Service, Item */
public void SetM_Product_ID (int M_Product_ID)
{
if (M_Product_ID < 1) throw new ArgumentException ("M_Product_ID is mandatory.");
Set_Value ("M_Product_ID", M_Product_ID);
}
/** Get Product.
@return Product, Service, Item */
public int GetM_Product_ID() 
{
Object ii = Get_Value("M_Product_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Production Line.
@param M_ProductionLine_ID Document Line representing a production */
public void SetM_ProductionLine_ID (int M_ProductionLine_ID)
{
if (M_ProductionLine_ID <= 0) Set_Value ("M_ProductionLine_ID", null);
else
Set_Value ("M_ProductionLine_ID", M_ProductionLine_ID);
}
/** Get Production Line.
@return Document Line representing a production */
public int GetM_ProductionLine_ID() 
{
Object ii = Get_Value("M_ProductionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Transaction.
@param M_Transaction_ID Inventory Transaction */
public void SetM_Transaction_ID (int M_Transaction_ID)
{
if (M_Transaction_ID < 1) throw new ArgumentException ("M_Transaction_ID is mandatory.");
Set_ValueNoCheck ("M_Transaction_ID", M_Transaction_ID);
}
/** Get Inventory Transaction.
@return Inventory Transaction */
public int GetM_Transaction_ID() 
{
Object ii = Get_Value("M_Transaction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetM_Transaction_ID().ToString());
}

/** Out_M_InOutLine_ID AD_Reference_ID=295 */
public static int OUT_M_INOUTLINE_ID_AD_Reference_ID=295;
/** Set Out Shipment Line.
@param Out_M_InOutLine_ID Outgoing Shipment/Receipt */
public void SetOut_M_InOutLine_ID (int Out_M_InOutLine_ID)
{
if (Out_M_InOutLine_ID <= 0) Set_Value ("Out_M_InOutLine_ID", null);
else
Set_Value ("Out_M_InOutLine_ID", Out_M_InOutLine_ID);
}
/** Get Out Shipment Line.
@return Outgoing Shipment/Receipt */
public int GetOut_M_InOutLine_ID() 
{
Object ii = Get_Value("Out_M_InOutLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Out_M_InventoryLine_ID AD_Reference_ID=296 */
public static int OUT_M_INVENTORYLINE_ID_AD_Reference_ID=296;
/** Set Out Inventory Line.
@param Out_M_InventoryLine_ID Outgoing Inventory Line */
public void SetOut_M_InventoryLine_ID (int Out_M_InventoryLine_ID)
{
if (Out_M_InventoryLine_ID <= 0) Set_Value ("Out_M_InventoryLine_ID", null);
else
Set_Value ("Out_M_InventoryLine_ID", Out_M_InventoryLine_ID);
}
/** Get Out Inventory Line.
@return Outgoing Inventory Line */
public int GetOut_M_InventoryLine_ID() 
{
Object ii = Get_Value("Out_M_InventoryLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Out_M_ProductionLine_ID AD_Reference_ID=297 */
public static int OUT_M_PRODUCTIONLINE_ID_AD_Reference_ID=297;
/** Set Out Production Line.
@param Out_M_ProductionLine_ID Outgoing Production Line */
public void SetOut_M_ProductionLine_ID (int Out_M_ProductionLine_ID)
{
if (Out_M_ProductionLine_ID <= 0) Set_Value ("Out_M_ProductionLine_ID", null);
else
Set_Value ("Out_M_ProductionLine_ID", Out_M_ProductionLine_ID);
}
/** Get Out Production Line.
@return Outgoing Production Line */
public int GetOut_M_ProductionLine_ID() 
{
Object ii = Get_Value("Out_M_ProductionLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Out_M_Transaction_ID AD_Reference_ID=298 */
public static int OUT_M_TRANSACTION_ID_AD_Reference_ID=298;
/** Set Out Transaction.
@param Out_M_Transaction_ID Outgoing Transaction */
public void SetOut_M_Transaction_ID (int Out_M_Transaction_ID)
{
if (Out_M_Transaction_ID <= 0) Set_Value ("Out_M_Transaction_ID", null);
else
Set_Value ("Out_M_Transaction_ID", Out_M_Transaction_ID);
}
/** Get Out Transaction.
@return Outgoing Transaction */
public int GetOut_M_Transaction_ID() 
{
Object ii = Get_Value("Out_M_Transaction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Quantity.
@param Qty Quantity */
public void SetQty (Decimal? Qty)
{
if (Qty == null) throw new ArgumentException ("Qty is mandatory.");
Set_Value ("Qty", (Decimal?)Qty);
}
/** Get Quantity.
@return Quantity */
public Decimal GetQty() 
{
Object bd =Get_Value("Qty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}
}

}
