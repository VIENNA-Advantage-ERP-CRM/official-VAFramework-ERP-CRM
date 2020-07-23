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
/** Generated Model for T_Transaction
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_T_Transaction : PO
{
public X_T_Transaction (Context ctx, int T_Transaction_ID, Trx trxName) : base (ctx, T_Transaction_ID, trxName)
{
/** if (T_Transaction_ID == 0)
{
SetAD_PInstance_ID (0);
SetM_AttributeSetInstance_ID (0);
SetM_Locator_ID (0);
SetM_Product_ID (0);
SetM_Transaction_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetMovementType (null);
}
 */
}
public X_T_Transaction (Ctx ctx, int T_Transaction_ID, Trx trxName) : base (ctx, T_Transaction_ID, trxName)
{
/** if (T_Transaction_ID == 0)
{
SetAD_PInstance_ID (0);
SetM_AttributeSetInstance_ID (0);
SetM_Locator_ID (0);
SetM_Product_ID (0);
SetM_Transaction_ID (0);
SetMovementDate (DateTime.Now);
SetMovementQty (0.0);
SetMovementType (null);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Transaction (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Transaction (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_T_Transaction (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_T_Transaction()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514384569L;
/** Last Updated Timestamp 7/29/2010 1:07:47 PM */
public static long updatedMS = 1280389067780L;
/** AD_Table_ID=758 */
public static int Table_ID;
 // =758;

/** TableName=T_Transaction */
public static String Table_Name="T_Transaction";

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
StringBuilder sb = new StringBuilder ("X_T_Transaction[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Process Instance.
@param AD_PInstance_ID Instance of the process */
public void SetAD_PInstance_ID (int AD_PInstance_ID)
{
if (AD_PInstance_ID < 1) throw new ArgumentException ("AD_PInstance_ID is mandatory.");
Set_Value ("AD_PInstance_ID", AD_PInstance_ID);
}
/** Get Process Instance.
@return Instance of the process */
public int GetAD_PInstance_ID() 
{
Object ii = Get_Value("AD_PInstance_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project Issue.
@param C_ProjectIssue_ID Project Issues (Material, Labor) */
public void SetC_ProjectIssue_ID (int C_ProjectIssue_ID)
{
if (C_ProjectIssue_ID <= 0) Set_Value ("C_ProjectIssue_ID", null);
else
Set_Value ("C_ProjectIssue_ID", C_ProjectIssue_ID);
}
/** Get Project Issue.
@return Project Issues (Material, Labor) */
public int GetC_ProjectIssue_ID() 
{
Object ii = Get_Value("C_ProjectIssue_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Project.
@param C_Project_ID Financial Project */
public void SetC_Project_ID (int C_Project_ID)
{
if (C_Project_ID <= 0) Set_Value ("C_Project_ID", null);
else
Set_Value ("C_Project_ID", C_Project_ID);
}
/** Get Project.
@return Financial Project */
public int GetC_Project_ID() 
{
Object ii = Get_Value("C_Project_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
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
/** Set Shipment/Receipt.
@param M_InOut_ID Material Shipment Document */
public void SetM_InOut_ID (int M_InOut_ID)
{
if (M_InOut_ID <= 0) Set_Value ("M_InOut_ID", null);
else
Set_Value ("M_InOut_ID", M_InOut_ID);
}
/** Get Shipment/Receipt.
@return Material Shipment Document */
public int GetM_InOut_ID() 
{
Object ii = Get_Value("M_InOut_ID");
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
/** Set Phys.Inventory.
@param M_Inventory_ID Parameters for a Physical Inventory */
public void SetM_Inventory_ID (int M_Inventory_ID)
{
if (M_Inventory_ID <= 0) Set_Value ("M_Inventory_ID", null);
else
Set_Value ("M_Inventory_ID", M_Inventory_ID);
}
/** Get Phys.Inventory.
@return Parameters for a Physical Inventory */
public int GetM_Inventory_ID() 
{
Object ii = Get_Value("M_Inventory_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Locator.
@param M_Locator_ID Warehouse Locator */
public void SetM_Locator_ID (int M_Locator_ID)
{
if (M_Locator_ID < 1) throw new ArgumentException ("M_Locator_ID is mandatory.");
Set_Value ("M_Locator_ID", M_Locator_ID);
}
/** Get Locator.
@return Warehouse Locator */
public int GetM_Locator_ID() 
{
Object ii = Get_Value("M_Locator_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Move Line.
@param M_MovementLine_ID Inventory Move document Line */
public void SetM_MovementLine_ID (int M_MovementLine_ID)
{
if (M_MovementLine_ID <= 0) Set_Value ("M_MovementLine_ID", null);
else
Set_Value ("M_MovementLine_ID", M_MovementLine_ID);
}
/** Get Move Line.
@return Inventory Move document Line */
public int GetM_MovementLine_ID() 
{
Object ii = Get_Value("M_MovementLine_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Move.
@param M_Movement_ID Movement of Inventory */
public void SetM_Movement_ID (int M_Movement_ID)
{
if (M_Movement_ID <= 0) Set_Value ("M_Movement_ID", null);
else
Set_Value ("M_Movement_ID", M_Movement_ID);
}
/** Get Inventory Move.
@return Movement of Inventory */
public int GetM_Movement_ID() 
{
Object ii = Get_Value("M_Movement_ID");
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
/** Set Production.
@param M_Production_ID Plan for producing a product */
public void SetM_Production_ID (int M_Production_ID)
{
if (M_Production_ID <= 0) Set_Value ("M_Production_ID", null);
else
Set_Value ("M_Production_ID", M_Production_ID);
}
/** Get Production.
@return Plan for producing a product */
public int GetM_Production_ID() 
{
Object ii = Get_Value("M_Production_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Transaction.
@param M_Transaction_ID Inventory Transaction */
public void SetM_Transaction_ID (int M_Transaction_ID)
{
if (M_Transaction_ID < 1) throw new ArgumentException ("M_Transaction_ID is mandatory.");
Set_Value ("M_Transaction_ID", M_Transaction_ID);
}
/** Get Inventory Transaction.
@return Inventory Transaction */
public int GetM_Transaction_ID() 
{
Object ii = Get_Value("M_Transaction_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Movement Date.
@param MovementDate Date a product was moved in or out of inventory */
public void SetMovementDate (DateTime? MovementDate)
{
if (MovementDate == null) throw new ArgumentException ("MovementDate is mandatory.");
Set_Value ("MovementDate", (DateTime?)MovementDate);
}
/** Get Movement Date.
@return Date a product was moved in or out of inventory */
public DateTime? GetMovementDate() 
{
return (DateTime?)Get_Value("MovementDate");
}
/** Set Movement Quantity.
@param MovementQty Quantity of a product moved. */
public void SetMovementQty (Decimal? MovementQty)
{
if (MovementQty == null) throw new ArgumentException ("MovementQty is mandatory.");
Set_Value ("MovementQty", (Decimal?)MovementQty);
}
/** Get Movement Quantity.
@return Quantity of a product moved. */
public Decimal GetMovementQty() 
{
Object bd =Get_Value("MovementQty");
if (bd == null) return Env.ZERO;
return  Convert.ToDecimal(bd);
}

/** MovementType AD_Reference_ID=189 */
public static int MOVEMENTTYPE_AD_Reference_ID=189;
/** Customer Returns = C+ */
public static String MOVEMENTTYPE_CustomerReturns = "C+";
/** Customer Shipment = C- */
public static String MOVEMENTTYPE_CustomerShipment = "C-";
/** Inventory In = I+ */
public static String MOVEMENTTYPE_InventoryIn = "I+";
/** Inventory Out = I- */
public static String MOVEMENTTYPE_InventoryOut = "I-";
/** Movement To = M+ */
public static String MOVEMENTTYPE_MovementTo = "M+";
/** Movement From = M- */
public static String MOVEMENTTYPE_MovementFrom = "M-";
/** Production + = P+ */
public static String MOVEMENTTYPE_ProductionPlus = "P+";
/** Production - = P- */
public static String MOVEMENTTYPE_Production_ = "P-";
/** Vendor Receipts = V+ */
public static String MOVEMENTTYPE_VendorReceipts = "V+";
/** Vendor Returns = V- */
public static String MOVEMENTTYPE_VendorReturns = "V-";
/** Work Order + = W+ */
public static String MOVEMENTTYPE_WorkOrderPlus = "W+";
/** Work Order - = W- */
public static String MOVEMENTTYPE_WorkOrder_ = "W-";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsMovementTypeValid (String test)
{
return test.Equals("C+") || test.Equals("C-") || test.Equals("I+") || test.Equals("I-") || test.Equals("M+") || test.Equals("M-") || test.Equals("P+") || test.Equals("P-") || test.Equals("V+") || test.Equals("V-") || test.Equals("W+") || test.Equals("W-");
}
/** Set Movement Type.
@param MovementType Method of moving the inventory */
public void SetMovementType (String MovementType)
{
if (MovementType == null) throw new ArgumentException ("MovementType is mandatory");
if (!IsMovementTypeValid(MovementType))
throw new ArgumentException ("MovementType Invalid value - " + MovementType + " - Reference_ID=189 - C+ - C- - I+ - I- - M+ - M- - P+ - P- - V+ - V- - W+ - W-");
if (MovementType.Length > 2)
{
log.Warning("Length > 2 - truncated");
MovementType = MovementType.Substring(0,2);
}
Set_Value ("MovementType", MovementType);
}
/** Get Movement Type.
@return Method of moving the inventory */
public String GetMovementType() 
{
return (String)Get_Value("MovementType");
}

/** Search_InOut_ID AD_Reference_ID=337 */
public static int SEARCH_INOUT_ID_AD_Reference_ID=337;
/** Set Search Shipment/Receipt.
@param Search_InOut_ID Material Shipment Document */
public void SetSearch_InOut_ID (int Search_InOut_ID)
{
if (Search_InOut_ID <= 0) Set_Value ("Search_InOut_ID", null);
else
Set_Value ("Search_InOut_ID", Search_InOut_ID);
}
/** Get Search Shipment/Receipt.
@return Material Shipment Document */
public int GetSearch_InOut_ID() 
{
Object ii = Get_Value("Search_InOut_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Search_Invoice_ID AD_Reference_ID=336 */
public static int SEARCH_INVOICE_ID_AD_Reference_ID=336;
/** Set Search Invoice.
@param Search_Invoice_ID Search Invoice Identifier */
public void SetSearch_Invoice_ID (int Search_Invoice_ID)
{
if (Search_Invoice_ID <= 0) Set_Value ("Search_Invoice_ID", null);
else
Set_Value ("Search_Invoice_ID", Search_Invoice_ID);
}
/** Get Search Invoice.
@return Search Invoice Identifier */
public int GetSearch_Invoice_ID() 
{
Object ii = Get_Value("Search_Invoice_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}

/** Search_Order_ID AD_Reference_ID=290 */
public static int SEARCH_ORDER_ID_AD_Reference_ID=290;
/** Set Search Order.
@param Search_Order_ID Order Identifier */
public void SetSearch_Order_ID (int Search_Order_ID)
{
if (Search_Order_ID <= 0) Set_Value ("Search_Order_ID", null);
else
Set_Value ("Search_Order_ID", Search_Order_ID);
}
/** Get Search Order.
@return Order Identifier */
public int GetSearch_Order_ID() 
{
Object ii = Get_Value("Search_Order_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
}

}
