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
    using System.Data;/** Generated Model for M_CostElementDetail
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_CostElementDetail : PO
    {
        public X_M_CostElementDetail(Context ctx, int M_CostElementDetail_ID, Trx trxName)
            : base(ctx, M_CostElementDetail_ID, trxName)
        {/** if (M_CostElementDetail_ID == 0){SetAmt (0.0);SetC_AcctSchema_ID (0);SetM_CostElementDetail_ID (0);SetM_CostElement_ID (0);SetM_Product_ID (0);SetQty (0.0);} */
        }
        public X_M_CostElementDetail(Ctx ctx, int M_CostElementDetail_ID, Trx trxName)
            : base(ctx, M_CostElementDetail_ID, trxName)
        {/** if (M_CostElementDetail_ID == 0){SetAmt (0.0);SetC_AcctSchema_ID (0);SetM_CostElementDetail_ID (0);SetM_CostElement_ID (0);SetM_Product_ID (0);SetQty (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostElementDetail(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostElementDetail(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_M_CostElementDetail(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_M_CostElementDetail() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27744376170769L;/** Last Updated Timestamp 5/3/2016 10:17:33 AM */
        public static long updatedMS = 1462250853980L;/** AD_Table_ID=1000643 */
        public static int Table_ID; // =1000643;
        /** TableName=M_CostElementDetail */
        public static String Table_Name = "M_CostElementDetail";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(7);/** AccessLevel
@return 7 - System - Client - Org 
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_M_CostElementDetail[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Asset.
@param A_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int A_Asset_ID)
        {
            if (A_Asset_ID <= 0) Set_Value("A_Asset_ID", null);
            else
                Set_Value("A_Asset_ID", A_Asset_ID);
        }/** Get Asset.
@return Asset used internally or by customers */
        public int GetA_Asset_ID() { Object ii = Get_Value("A_Asset_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Amount.
@param Amt Amount */
        public void SetAmt(Decimal? Amt) { if (Amt == null) throw new ArgumentException("Amt is mandatory."); Set_Value("Amt", (Decimal?)Amt); }/** Get Amount.
@return Amount */
        public Decimal GetAmt() { Object bd = Get_Value("Amt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Accounting Schema.
@param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID) { if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory."); Set_Value("C_AcctSchema_ID", C_AcctSchema_ID); }/** Get Accounting Schema.
@return Rules for accounting */
        public int GetC_AcctSchema_ID() { Object ii = Get_Value("C_AcctSchema_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice Line.
@param C_InvoiceLine_ID Invoice Detail Line */
        public void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            if (C_InvoiceLine_ID <= 0) Set_Value("C_InvoiceLine_ID", null);
            else
                Set_Value("C_InvoiceLine_ID", C_InvoiceLine_ID);
        }/** Get Invoice Line.
@return Invoice Detail Line */
        public int GetC_InvoiceLine_ID() { Object ii = Get_Value("C_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetC_OrderLine_ID() { Object ii = Get_Value("C_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Project Issue.
@param C_ProjectIssue_ID Project Issues (Material, Labor) */
        public void SetC_ProjectIssue_ID(int C_ProjectIssue_ID)
        {
            if (C_ProjectIssue_ID <= 0) Set_Value("C_ProjectIssue_ID", null);
            else
                Set_Value("C_ProjectIssue_ID", C_ProjectIssue_ID);
        }/** Get Project Issue.
@return Project Issues (Material, Labor) */
        public int GetC_ProjectIssue_ID() { Object ii = Get_Value("C_ProjectIssue_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Current Cost.
@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }/** Get Current Cost.
@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Delta Amount.
@param DeltaAmt Difference Amount */
        public void SetDeltaAmt(Decimal? DeltaAmt) { Set_Value("DeltaAmt", (Decimal?)DeltaAmt); }/** Get Delta Amount.
@return Difference Amount */
        public Decimal GetDeltaAmt() { Object bd = Get_Value("DeltaAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Delta Quantity.
@param DeltaQty Quantity Difference */
        public void SetDeltaQty(Decimal? DeltaQty) { Set_Value("DeltaQty", (Decimal?)DeltaQty); }/** Get Delta Quantity.
@return Quantity Difference */
        public Decimal GetDeltaQty() { Object bd = Get_Value("DeltaQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Description.
@param Description Optional short description of the record */
        public void SetDescription(String Description) { if (Description != null && Description.Length > 255) { log.Warning("Length > 255 - truncated"); Description = Description.Substring(0, 255); } Set_Value("Description", Description); }/** Get Description.
@return Optional short description of the record */
        public String GetDescription() { return (String)Get_Value("Description"); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_Value("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Sales Transaction.
@param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx) { Set_Value("IsSOTrx", IsSOTrx); }/** Get Sales Transaction.
@return This is a Sales Transaction */
        public Boolean IsSOTrx() { Object oo = Get_Value("IsSOTrx"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Attribute Set Instance.
@param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID() { Object ii = Get_Value("M_AttributeSetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set M_CostElementDetail_ID.
@param M_CostElementDetail_ID M_CostElementDetail_ID */
        public void SetM_CostElementDetail_ID(int M_CostElementDetail_ID) { if (M_CostElementDetail_ID < 1) throw new ArgumentException("M_CostElementDetail_ID is mandatory."); Set_ValueNoCheck("M_CostElementDetail_ID", M_CostElementDetail_ID); }/** Get M_CostElementDetail_ID.
@return M_CostElementDetail_ID */
        public int GetM_CostElementDetail_ID() { Object ii = Get_Value("M_CostElementDetail_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cost Element.
@param M_CostElement_ID Product Cost Element */
        public void SetM_CostElement_ID(int M_CostElement_ID) { if (M_CostElement_ID < 1) throw new ArgumentException("M_CostElement_ID is mandatory."); Set_Value("M_CostElement_ID", M_CostElement_ID); }/** Get Cost Element.
@return Product Cost Element */
        public int GetM_CostElement_ID() { Object ii = Get_Value("M_CostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Shipment/Receipt Line.
@param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_Value("M_InOutLine_ID", null);
            else
                Set_Value("M_InOutLine_ID", M_InOutLine_ID);
        }/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID() { Object ii = Get_Value("M_InOutLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Phys Inventory Line.
@param M_InventoryLine_ID Unique line in an Inventory document */
        public void SetM_InventoryLine_ID(int M_InventoryLine_ID)
        {
            if (M_InventoryLine_ID <= 0) Set_Value("M_InventoryLine_ID", null);
            else
                Set_Value("M_InventoryLine_ID", M_InventoryLine_ID);
        }/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
        public int GetM_InventoryLine_ID() { Object ii = Get_Value("M_InventoryLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Move Line.
@param M_MovementLine_ID Inventory Move document Line */
        public void SetM_MovementLine_ID(int M_MovementLine_ID)
        {
            if (M_MovementLine_ID <= 0) Set_Value("M_MovementLine_ID", null);
            else
                Set_Value("M_MovementLine_ID", M_MovementLine_ID);
        }/** Get Move Line.
@return Inventory Move document Line */
        public int GetM_MovementLine_ID() { Object ii = Get_Value("M_MovementLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID) { if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory."); Set_ValueNoCheck("M_Product_ID", M_Product_ID); }/** Get Product.
@return Product, Service, Item */
        public int GetM_Product_ID() { Object ii = Get_Value("M_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Production Line.
@param M_ProductionLine_ID Document Line representing a production */
        public void SetM_ProductionLine_ID(int M_ProductionLine_ID)
        {
            if (M_ProductionLine_ID <= 0) Set_Value("M_ProductionLine_ID", null);
            else
                Set_Value("M_ProductionLine_ID", M_ProductionLine_ID);
        }/** Get Production Line.
@return Document Line representing a production */
        public int GetM_ProductionLine_ID() { Object ii = Get_Value("M_ProductionLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Work Order Resource Transaction Line.
@param M_WorkOrderResourceTxnLine_ID Identifies the production resource detail lines in a work order transaction */
        public void SetM_WorkOrderResourceTxnLine_ID(int M_WorkOrderResourceTxnLine_ID)
        {
            if (M_WorkOrderResourceTxnLine_ID <= 0) Set_Value("M_WorkOrderResourceTxnLine_ID", null);
            else
                Set_Value("M_WorkOrderResourceTxnLine_ID", M_WorkOrderResourceTxnLine_ID);
        }/** Get Work Order Resource Transaction Line.
@return Identifies the production resource detail lines in a work order transaction */
        public int GetM_WorkOrderResourceTxnLine_ID() { Object ii = Get_Value("M_WorkOrderResourceTxnLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Work Order Transaction Line.
@param M_WorkOrderTransactionLine_ID Work Order Transaction Line */
        public void SetM_WorkOrderTransactionLine_ID(int M_WorkOrderTransactionLine_ID)
        {
            if (M_WorkOrderTransactionLine_ID <= 0) Set_Value("M_WorkOrderTransactionLine_ID", null);
            else
                Set_Value("M_WorkOrderTransactionLine_ID", M_WorkOrderTransactionLine_ID);
        }/** Get Work Order Transaction Line.
@return Work Order Transaction Line */
        public int GetM_WorkOrderTransactionLine_ID() { Object ii = Get_Value("M_WorkOrderTransactionLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Price.
@param Price Price */
        public void SetPrice(Decimal? Price) { throw new ArgumentException("Price Is virtual column"); }/** Get Price.
@return Price */
        public Decimal GetPrice() { Object bd = Get_Value("Price"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Processed.
@param Processed The document has been processed */
        public void SetProcessed(Boolean Processed) { Set_Value("Processed", Processed); }/** Get Processed.
@return The document has been processed */
        public Boolean IsProcessed() { Object oo = Get_Value("Processed"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Quantity.
@param Qty Quantity */
        public void SetQty(Decimal? Qty) { if (Qty == null) throw new ArgumentException("Qty is mandatory."); Set_Value("Qty", (Decimal?)Qty); }/** Get Quantity.
@return Quantity */
        public Decimal GetQty() { Object bd = Get_Value("Qty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
    }
}