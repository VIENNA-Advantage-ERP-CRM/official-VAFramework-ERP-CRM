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
    using System.Data;/** Generated Model for M_RevaluationLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_RevaluationLine : PO
    {
        public X_M_RevaluationLine(Context ctx, int M_RevaluationLine_ID, Trx trxName) : base(ctx, M_RevaluationLine_ID, trxName)
        {/** if (M_RevaluationLine_ID == 0){SetC_UOM_ID (0);SetCostingLevel (null);SetCostingMethod (null);SetCurrentCostPrice (0.0);SetDifferenceCostPrice (0.0);SetM_InventoryRevaluation_ID (0);SetM_Product_Category_ID (0);SetM_Product_ID (0);SetM_RevaluationLine_ID (0);SetNetRealizableValue (0.0);SetNewCostPrice (0.0);SetProcessed (false);SetQtyOnHand (0.0);SetSalesPrice (0.0);SetTotalDifference (0.0);} */
        }
        public X_M_RevaluationLine(Ctx ctx, int M_RevaluationLine_ID, Trx trxName) : base(ctx, M_RevaluationLine_ID, trxName)
        {/** if (M_RevaluationLine_ID == 0){SetC_UOM_ID (0);SetCostingLevel (null);SetCostingMethod (null);SetCurrentCostPrice (0.0);SetDifferenceCostPrice (0.0);SetM_InventoryRevaluation_ID (0);SetM_Product_Category_ID (0);SetM_Product_ID (0);SetM_RevaluationLine_ID (0);SetNetRealizableValue (0.0);SetNewCostPrice (0.0);SetProcessed (false);SetQtyOnHand (0.0);SetSalesPrice (0.0);SetTotalDifference (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_RevaluationLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_RevaluationLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_RevaluationLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_RevaluationLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27960466297605L;/** Last Updated Timestamp 3/9/2023 5:49:40 AM */
        public static long updatedMS = 1678340980816L;/** AD_Table_ID=1000575 */
        public static int Table_ID; // =1000575;
        /** TableName=M_RevaluationLine */
        public static String Table_Name = "M_RevaluationLine";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(3);/** AccessLevel
@return 3 - Client - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_RevaluationLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set UOM.
@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID) { if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory."); Set_Value("C_UOM_ID", C_UOM_ID); }/** Get UOM.
@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** CostingLevel AD_Reference_ID=355 */
        public static int COSTINGLEVEL_AD_Reference_ID = 355;/** Org + Batch = A */
        public static String COSTINGLEVEL_OrgPlusBatch = "A";/** Batch/Lot = B */
        public static String COSTINGLEVEL_BatchLot = "B";/** Client = C */
        public static String COSTINGLEVEL_Client = "C";/** Warehouse + Batch = D */
        public static String COSTINGLEVEL_WarehousePlusBatch = "D";/** Organization = O */
        public static String COSTINGLEVEL_Organization = "O";/** Warehouse = W */
        public static String COSTINGLEVEL_Warehouse = "W";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCostingLevelValid(String test) { return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("O") || test.Equals("W"); }/** Set Costing Level.
@param CostingLevel The lowest level to accumulate Costing Information */
        public void SetCostingLevel(String CostingLevel)
        {
            if (CostingLevel == null) throw new ArgumentException("CostingLevel is mandatory"); if (!IsCostingLevelValid(CostingLevel))
                throw new ArgumentException("CostingLevel Invalid value - " + CostingLevel + " - Reference_ID=355 - A - B - C - D - O - W"); if (CostingLevel.Length > 1) { log.Warning("Length > 1 - truncated"); CostingLevel = CostingLevel.Substring(0, 1); }
            Set_Value("CostingLevel", CostingLevel);
        }/** Get Costing Level.
@return The lowest level to accumulate Costing Information */
        public String GetCostingLevel() { return (String)Get_Value("CostingLevel"); }
        /** CostingMethod AD_Reference_ID=122 */
        public static int COSTINGMETHOD_AD_Reference_ID = 122;/** Average PO = A */
        public static String COSTINGMETHOD_AveragePO = "A";/** Provisional Weighted Average = B */
        public static String COSTINGMETHOD_ProvisionalWeightedAverage = "B";/** Cost Combination = C */
        public static String COSTINGMETHOD_CostCombination = "C";/** Fifo = F */
        public static String COSTINGMETHOD_Fifo = "F";/** Average Invoice = I */
        public static String COSTINGMETHOD_AverageInvoice = "I";/** Lifo = L */
        public static String COSTINGMETHOD_Lifo = "L";/** Weighted Average PO = O */
        public static String COSTINGMETHOD_WeightedAveragePO = "O";/** Standard Costing = S */
        public static String COSTINGMETHOD_StandardCosting = "S";/** User Defined = U */
        public static String COSTINGMETHOD_UserDefined = "U";/** Weighted Average Cost = W */
        public static String COSTINGMETHOD_WeightedAverageCost = "W";/** Last Invoice = i */
        public static String COSTINGMETHOD_LastInvoice = "i";/** Last PO Price = p */
        public static String COSTINGMETHOD_LastPOPrice = "p";/** _ = x */
        public static String COSTINGMETHOD_ = "x";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsCostingMethodValid(String test) { return test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("O") || test.Equals("S") || test.Equals("U") || test.Equals("W") || test.Equals("i") || test.Equals("p") || test.Equals("x"); }/** Set Costing Method.
@param CostingMethod Indicates how Costs will be calculated */
        public void SetCostingMethod(String CostingMethod)
        {
            if (CostingMethod == null) throw new ArgumentException("CostingMethod is mandatory"); if (!IsCostingMethodValid(CostingMethod))
                throw new ArgumentException("CostingMethod Invalid value - " + CostingMethod + " - Reference_ID=122 - A - B - C - F - I - L - O - S - U - W - i - p - x"); if (CostingMethod.Length > 1) { log.Warning("Length > 1 - truncated"); CostingMethod = CostingMethod.Substring(0, 1); }
            Set_Value("CostingMethod", CostingMethod);
        }/** Get Costing Method.
@return Indicates how Costs will be calculated */
        public String GetCostingMethod() { return (String)Get_Value("CostingMethod"); }/** Set Current Cost/Unit.
@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { if (CurrentCostPrice == null) throw new ArgumentException("CurrentCostPrice is mandatory."); Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }/** Get Current Cost/Unit.
@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Difference CostPrice.
@param DifferenceCostPrice This indicates the cost price per unit difference (New Cost/Unit - Current Cost/Unit) */
        public void SetDifferenceCostPrice(Decimal? DifferenceCostPrice) { if (DifferenceCostPrice == null) throw new ArgumentException("DifferenceCostPrice is mandatory."); Set_Value("DifferenceCostPrice", (Decimal?)DifferenceCostPrice); }/** Get Difference CostPrice.
@return This indicates the cost price per unit difference (New Cost/Unit - Current Cost/Unit) */
        public Decimal GetDifferenceCostPrice() { Object bd = Get_Value("DifferenceCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Difference Value.
@param DifferenceValue New Value – Sold Value */
        public void SetDifferenceValue(Decimal? DifferenceValue) { Set_Value("DifferenceValue", (Decimal?)DifferenceValue); }/** Get Difference Value.
@return New Value – Sold Value */
        public Decimal GetDifferenceValue() { Object bd = Get_Value("DifferenceValue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Line No.
@param LineNo Line No */
        public void SetLineNo(int LineNo) { Set_Value("LineNo", LineNo); }/** Get Line No.
@return Line No */
        public int GetLineNo() { Object ii = Get_Value("LineNo"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set  Revaluation No.
@param M_InventoryRevaluation_ID This represents the revaluation number of the Inventory revaluation record. */
        public void SetM_InventoryRevaluation_ID(int M_InventoryRevaluation_ID) { if (M_InventoryRevaluation_ID < 1) throw new ArgumentException("M_InventoryRevaluation_ID is mandatory."); Set_ValueNoCheck("M_InventoryRevaluation_ID", M_InventoryRevaluation_ID); }/** Get  Revaluation No.
@return This represents the revaluation number of the Inventory revaluation record. */
        public int GetM_InventoryRevaluation_ID() { Object ii = Get_Value("M_InventoryRevaluation_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product Category.
@param M_Product_Category_ID Category of a Product */
        public void SetM_Product_Category_ID(int M_Product_Category_ID) { if (M_Product_Category_ID < 1) throw new ArgumentException("M_Product_Category_ID is mandatory."); Set_Value("M_Product_Category_ID", M_Product_Category_ID); }/** Get Product Category.
@return Category of a Product */
        public int GetM_Product_Category_ID() { Object ii = Get_Value("M_Product_Category_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID) { if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory."); Set_Value("M_Product_ID", M_Product_ID); }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set M_RevaluationLine_ID.
@param M_RevaluationLine_ID M_RevaluationLine_ID */
        public void SetM_RevaluationLine_ID(int M_RevaluationLine_ID) { if (M_RevaluationLine_ID < 1) throw new ArgumentException("M_RevaluationLine_ID is mandatory."); Set_ValueNoCheck("M_RevaluationLine_ID", M_RevaluationLine_ID); }/** Get M_RevaluationLine_ID.
@return M_RevaluationLine_ID */
        public int GetM_RevaluationLine_ID() { Object ii = Get_Value("M_RevaluationLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Net Realizable Value.
@param NetRealizableValue Sales Price – Current Cost Price */
        public void SetNetRealizableValue(Decimal? NetRealizableValue) { if (NetRealizableValue == null) throw new ArgumentException("NetRealizableValue is mandatory."); Set_Value("NetRealizableValue", (Decimal?)NetRealizableValue); }/** Get Net Realizable Value.
@return Sales Price – Current Cost Price */
        public Decimal GetNetRealizableValue() { Object bd = Get_Value("NetRealizableValue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set New Cost/Unit.
@param NewCostPrice Sold Quantity * New Cost/Unit */
        public void SetNewCostPrice(Decimal? NewCostPrice) { if (NewCostPrice == null) throw new ArgumentException("NewCostPrice is mandatory."); Set_Value("NewCostPrice", (Decimal?)NewCostPrice); }/** Get New Cost/Unit.
@return Sold Quantity * New Cost/Unit */
        public Decimal GetNewCostPrice() { Object bd = Get_Value("NewCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set New Value.
@param NewValue New field value */
        public void SetNewValue(Decimal? NewValue) { Set_Value("NewValue", (Decimal?)NewValue); }/** Get New Value.
@return New field value */
        public Decimal GetNewValue() { Object bd = Get_Value("NewValue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set In Stock Qty.
@param QtyOnHand In Stock Qty */
        public void SetQtyOnHand(Decimal? QtyOnHand) { if (QtyOnHand == null) throw new ArgumentException("QtyOnHand is mandatory."); Set_Value("QtyOnHand", (Decimal?)QtyOnHand); }/** Get In Stock Qty.
@return In Stock Qty */
        public Decimal GetQtyOnHand() { Object bd = Get_Value("QtyOnHand"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Sales Price.
@param SalesPrice Maximum price of the product from the sales price of the organization (Selected Organization or Star Organization) in base currency. */
        public void SetSalesPrice(Decimal? SalesPrice) { if (SalesPrice == null) throw new ArgumentException("SalesPrice is mandatory."); Set_Value("SalesPrice", (Decimal?)SalesPrice); }/** Get Sales Price.
@return Maximum price of the product from the sales price of the organization (Selected Organization or Star Organization) in base currency. */
        public Decimal GetSalesPrice() { Object bd = Get_Value("SalesPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Sold/Consumed Quantity.
@param SoldQty Quantity of the Product Sold or Consumed in the selected period. */
        public void SetSoldQty(Decimal? SoldQty) { Set_Value("SoldQty", (Decimal?)SoldQty); }/** Get Sold/Consumed Quantity.
@return Quantity of the Product Sold or Consumed in the selected period. */
        public Decimal GetSoldQty() { Object bd = Get_Value("SoldQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Sold/Consumed Value.
@param SoldValue Value of the Sold or Consumed quantity of the product in the selected period.  */
        public void SetSoldValue(Decimal? SoldValue) { Set_Value("SoldValue", (Decimal?)SoldValue); }/** Get Sold/Consumed Value.
@return Value of the Sold or Consumed quantity of the product in the selected period.  */
        public Decimal GetSoldValue() { Object bd = Get_Value("SoldValue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Difference.
@param TotalDifference Difference/unit * In Stock Qty */
        public void SetTotalDifference(Decimal? TotalDifference) { if (TotalDifference == null) throw new ArgumentException("TotalDifference is mandatory."); Set_Value("TotalDifference", (Decimal?)TotalDifference); }/** Get Total Difference.
@return Difference/unit * In Stock Qty */
        public Decimal GetTotalDifference() { Object bd = Get_Value("TotalDifference"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}