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
/** Generated Model for M_Product_Category_Acct
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
public class X_M_Product_Category_Acct : PO
{
public X_M_Product_Category_Acct (Context ctx, int M_Product_Category_Acct_ID, Trx trxName) : base (ctx, M_Product_Category_Acct_ID, trxName)
{
/** if (M_Product_Category_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetM_Product_Category_ID (0);
SetP_Asset_Acct (0);
SetP_COGS_Acct (0);
SetP_CostAdjustment_Acct (0);
SetP_Expense_Acct (0);
SetP_InventoryClearing_Acct (0);
SetP_InvoicePriceVariance_Acct (0);
SetP_PurchasePriceVariance_Acct (0);
SetP_Revenue_Acct (0);
SetP_TradeDiscountGrant_Acct (0);
SetP_TradeDiscountRec_Acct (0);
}
 */
}
public X_M_Product_Category_Acct (Ctx ctx, int M_Product_Category_Acct_ID, Trx trxName) : base (ctx, M_Product_Category_Acct_ID, trxName)
{
/** if (M_Product_Category_Acct_ID == 0)
{
SetC_AcctSchema_ID (0);
SetM_Product_Category_ID (0);
SetP_Asset_Acct (0);
SetP_COGS_Acct (0);
SetP_CostAdjustment_Acct (0);
SetP_Expense_Acct (0);
SetP_InventoryClearing_Acct (0);
SetP_InvoicePriceVariance_Acct (0);
SetP_PurchasePriceVariance_Acct (0);
SetP_Revenue_Acct (0);
SetP_TradeDiscountGrant_Acct (0);
SetP_TradeDiscountRec_Acct (0);
}
 */
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category_Acct (Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category_Acct (Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName)
{
}
/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
public X_M_Product_Category_Acct (Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName)
{
}
/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
static X_M_Product_Category_Acct()
{
 Table_ID = Get_Table_ID(Table_Name);
 model = new KeyNamePair(Table_ID,Table_Name);
}
/** Serial Version No */
//static long serialVersionUID 27562514380838L;
/** Last Updated Timestamp 7/29/2010 1:07:44 PM */
public static long updatedMS = 1280389064049L;
/** AD_Table_ID=401 */
public static int Table_ID;
 // =401;

/** TableName=M_Product_Category_Acct */
public static String Table_Name="M_Product_Category_Acct";

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
StringBuilder sb = new StringBuilder ("X_M_Product_Category_Acct[").Append(Get_ID()).Append("]");
return sb.ToString();
}
/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
public void SetC_AcctSchema_ID (int C_AcctSchema_ID)
{
if (C_AcctSchema_ID < 1) throw new ArgumentException ("C_AcctSchema_ID is mandatory.");
Set_ValueNoCheck ("C_AcctSchema_ID", C_AcctSchema_ID);
}
/** Get Accounting Schema.
@return Rules for accounting */
public int GetC_AcctSchema_ID() 
{
Object ii = Get_Value("C_AcctSchema_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Get Record ID/ColumnName
@return ID/ColumnName pair */
public KeyNamePair GetKeyNamePair() 
{
return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
}

/** CostingLevel AD_Reference_ID=355 */
public static int COSTINGLEVEL_AD_Reference_ID=355;
/** Batch/Lot = B */
public static String COSTINGLEVEL_BatchLot = "B";
/** Client = C */
public static String COSTINGLEVEL_Client = "C";
/** Organization = O */
public static String COSTINGLEVEL_Organization = "O";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCostingLevelValid (String test)
{
return test == null || test.Equals("B") || test.Equals("C") || test.Equals("O");
}
/** Set Costing Level.
@param CostingLevel The lowest level to accumulate Costing Information */
public void SetCostingLevel (String CostingLevel)
{
if (!IsCostingLevelValid(CostingLevel))
throw new ArgumentException ("CostingLevel Invalid value - " + CostingLevel + " - Reference_ID=355 - B - C - O");
if (CostingLevel != null && CostingLevel.Length > 1)
{
log.Warning("Length > 1 - truncated");
CostingLevel = CostingLevel.Substring(0,1);
}
Set_Value ("CostingLevel", CostingLevel);
}
/** Get Costing Level.
@return The lowest level to accumulate Costing Information */
public String GetCostingLevel() 
{
return (String)Get_Value("CostingLevel");
}

/** CostingMethod AD_Reference_ID=122 */
public static int COSTINGMETHOD_AD_Reference_ID=122;
/** Average PO = A */
public static String COSTINGMETHOD_AveragePO = "A";
/** Fifo = F */
public static String COSTINGMETHOD_Fifo = "F";
/** Average Invoice = I */
public static String COSTINGMETHOD_AverageInvoice = "I";
/** Lifo = L */
public static String COSTINGMETHOD_Lifo = "L";
/** Standard Costing = S */
public static String COSTINGMETHOD_StandardCosting = "S";
/** User Defined = U */
public static String COSTINGMETHOD_UserDefined = "U";
/** Last Invoice = i */
public static String COSTINGMETHOD_LastInvoice = "i";
/** Last PO Price = p */
public static String COSTINGMETHOD_LastPOPrice = "p";
/** _ = x */
public static String COSTINGMETHOD__ = "x";
/** Is test a valid value.
@param test testvalue
@returns true if valid **/
public bool IsCostingMethodValid (String test)
{
return test == null || test.Equals("A") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("S") || test.Equals("U") || test.Equals("i") || test.Equals("p") || test.Equals("x");
}
/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
public void SetCostingMethod (String CostingMethod)
{
if (!IsCostingMethodValid(CostingMethod))
throw new ArgumentException ("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - F - I - L - S - U - i - p - x");
if (CostingMethod != null && CostingMethod.Length > 1)
{
log.Warning("Length > 1 - truncated");
CostingMethod = CostingMethod.Substring(0,1);
}
Set_Value ("CostingMethod", CostingMethod);
}
/** Get Costing Method.
@return Indicates how Costs will be calculated */
public String GetCostingMethod() 
{
return (String)Get_Value("CostingMethod");
}
/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
public void SetM_Product_Category_ID (int M_Product_Category_ID)
{
if (M_Product_Category_ID < 1) throw new ArgumentException ("M_Product_Category_ID is mandatory.");
Set_ValueNoCheck ("M_Product_Category_ID", M_Product_Category_ID);
}
/** Get Product Category.
@return Category of a Product */
public int GetM_Product_Category_ID() 
{
Object ii = Get_Value("M_Product_Category_ID");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Asset.
@param P_Asset_Acct Account for Product Asset (Inventory) */
public void SetP_Asset_Acct (int P_Asset_Acct)
{
Set_Value ("P_Asset_Acct", P_Asset_Acct);
}
/** Get Product Asset.
@return Account for Product Asset (Inventory) */
public int GetP_Asset_Acct() 
{
Object ii = Get_Value("P_Asset_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product COGS.
@param P_COGS_Acct Account for Cost of Goods Sold */
public void SetP_COGS_Acct (int P_COGS_Acct)
{
Set_Value ("P_COGS_Acct", P_COGS_Acct);
}
/** Get Product COGS.
@return Account for Cost of Goods Sold */
public int GetP_COGS_Acct() 
{
Object ii = Get_Value("P_COGS_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Cost Adjustment.
@param P_CostAdjustment_Acct Product Cost Adjustment Account */
public void SetP_CostAdjustment_Acct (int P_CostAdjustment_Acct)
{
Set_Value ("P_CostAdjustment_Acct", P_CostAdjustment_Acct);
}
/** Get Cost Adjustment.
@return Product Cost Adjustment Account */
public int GetP_CostAdjustment_Acct() 
{
Object ii = Get_Value("P_CostAdjustment_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Expense.
@param P_Expense_Acct Account for Product Expense */
public void SetP_Expense_Acct (int P_Expense_Acct)
{
Set_Value ("P_Expense_Acct", P_Expense_Acct);
}
/** Get Product Expense.
@return Account for Product Expense */
public int GetP_Expense_Acct() 
{
Object ii = Get_Value("P_Expense_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Inventory Clearing.
@param P_InventoryClearing_Acct Product Inventory Clearing Account */
public void SetP_InventoryClearing_Acct (int P_InventoryClearing_Acct)
{
Set_Value ("P_InventoryClearing_Acct", P_InventoryClearing_Acct);
}
/** Get Inventory Clearing.
@return Product Inventory Clearing Account */
public int GetP_InventoryClearing_Acct() 
{
Object ii = Get_Value("P_InventoryClearing_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Invoice Price Variance.
@param P_InvoicePriceVariance_Acct Difference between Costs and Invoice Price (IPV) */
public void SetP_InvoicePriceVariance_Acct (int P_InvoicePriceVariance_Acct)
{
Set_Value ("P_InvoicePriceVariance_Acct", P_InvoicePriceVariance_Acct);
}
/** Get Invoice Price Variance.
@return Difference between Costs and Invoice Price (IPV) */
public int GetP_InvoicePriceVariance_Acct() 
{
Object ii = Get_Value("P_InvoicePriceVariance_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Purchase Price Variance.
@param P_PurchasePriceVariance_Acct Difference between Standard Cost and Purchase Price (PPV) */
public void SetP_PurchasePriceVariance_Acct (int P_PurchasePriceVariance_Acct)
{
Set_Value ("P_PurchasePriceVariance_Acct", P_PurchasePriceVariance_Acct);
}
/** Get Purchase Price Variance.
@return Difference between Standard Cost and Purchase Price (PPV) */
public int GetP_PurchasePriceVariance_Acct() 
{
Object ii = Get_Value("P_PurchasePriceVariance_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Product Revenue.
@param P_Revenue_Acct Account for Product Revenue (Sales Account) */
public void SetP_Revenue_Acct (int P_Revenue_Acct)
{
Set_Value ("P_Revenue_Acct", P_Revenue_Acct);
}
/** Get Product Revenue.
@return Account for Product Revenue (Sales Account) */
public int GetP_Revenue_Acct() 
{
Object ii = Get_Value("P_Revenue_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Trade Discount Granted.
@param P_TradeDiscountGrant_Acct Trade Discount Granted Account */
public void SetP_TradeDiscountGrant_Acct (int P_TradeDiscountGrant_Acct)
{
Set_Value ("P_TradeDiscountGrant_Acct", P_TradeDiscountGrant_Acct);
}
/** Get Trade Discount Granted.
@return Trade Discount Granted Account */
public int GetP_TradeDiscountGrant_Acct() 
{
Object ii = Get_Value("P_TradeDiscountGrant_Acct");
if (ii == null) return 0;
return Convert.ToInt32(ii);
}
/** Set Trade Discount Received.
@param P_TradeDiscountRec_Acct Trade Discount Receivable Account */
public void SetP_TradeDiscountRec_Acct (int P_TradeDiscountRec_Acct)
{
Set_Value ("P_TradeDiscountRec_Acct", P_TradeDiscountRec_Acct);
}
/** Get Trade Discount Received.
@return Trade Discount Receivable Account */
public int GetP_TradeDiscountRec_Acct() 
{
Object ii = Get_Value("P_TradeDiscountRec_Acct");
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
