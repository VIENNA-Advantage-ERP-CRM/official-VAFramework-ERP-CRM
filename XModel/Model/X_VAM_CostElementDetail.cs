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
    using System.Data;/** Generated Model for VAM_ProductCostElementDetail
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_CostElementDetail : PO
    {
        public X_VAM_CostElementDetail(Context ctx, int VAM_ProductCostElementDetail_ID, Trx trxName)
            : base(ctx, VAM_ProductCostElementDetail_ID, trxName)
        {/** if (VAM_ProductCostElementDetail_ID == 0){SetAmt (0.0);SetVAB_AccountBook_ID (0);SetVAM_ProductCostElementDetail_ID (0);SetVAM_ProductCostElement_ID (0);SetVAM_Product_ID (0);SetQty (0.0);} */
        }
        public X_VAM_CostElementDetail(Ctx ctx, int VAM_ProductCostElementDetail_ID, Trx trxName)
            : base(ctx, VAM_ProductCostElementDetail_ID, trxName)
        {/** if (VAM_ProductCostElementDetail_ID == 0){SetAmt (0.0);SetVAB_AccountBook_ID (0);SetVAM_ProductCostElementDetail_ID (0);SetVAM_ProductCostElement_ID (0);SetVAM_Product_ID (0);SetQty (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_CostElementDetail(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_CostElementDetail(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAM_CostElementDetail(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAM_CostElementDetail() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27744376170769L;/** Last Updated Timestamp 5/3/2016 10:17:33 AM */
        public static long updatedMS = 1462250853980L;/** VAF_TableView_ID=1000643 */
        public static int Table_ID; // =1000643;
        /** TableName=VAM_ProductCostElementDetail */
        public static String Table_Name = "VAM_ProductCostElementDetail";
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
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAM_ProductCostElementDetail[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int VAA_Asset_ID)
        {
            if (VAA_Asset_ID <= 0) Set_Value("VAA_Asset_ID", null);
            else
                Set_Value("VAA_Asset_ID", VAA_Asset_ID);
        }/** Get Asset.
@return Asset used internally or by customers */
        public int GetA_Asset_ID() { Object ii = Get_Value("VAA_Asset_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Amount.
@param Amt Amount */
        public void SetAmt(Decimal? Amt) { if (Amt == null) throw new ArgumentException("Amt is mandatory."); Set_Value("Amt", (Decimal?)Amt); }/** Get Amount.
@return Amount */
        public Decimal GetAmt() { Object bd = Get_Value("Amt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Accounting Schema.
@param VAB_AccountBook_ID Rules for accounting */
        public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID) { if (VAB_AccountBook_ID < 1) throw new ArgumentException("VAB_AccountBook_ID is mandatory."); Set_Value("VAB_AccountBook_ID", VAB_AccountBook_ID); }/** Get Accounting Schema.
@return Rules for accounting */
        public int GetVAB_AccountBook_ID() { Object ii = Get_Value("VAB_AccountBook_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice Line.
@param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_Value("VAB_InvoiceLine_ID", null);
            else
                Set_Value("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }/** Get Invoice Line.
@return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID() { Object ii = Get_Value("VAB_InvoiceLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Order Line.
@param VAB_OrderLine_ID Order Line */
        public void SetVAB_OrderLine_ID(int VAB_OrderLine_ID)
        {
            if (VAB_OrderLine_ID <= 0) Set_Value("VAB_OrderLine_ID", null);
            else
                Set_Value("VAB_OrderLine_ID", VAB_OrderLine_ID);
        }/** Get Order Line.
@return Order Line */
        public int GetVAB_OrderLine_ID() { Object ii = Get_Value("VAB_OrderLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Project Issue.
@param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
        public void SetVAB_ProjectSupply_ID(int VAB_ProjectSupply_ID)
        {
            if (VAB_ProjectSupply_ID <= 0) Set_Value("VAB_ProjectSupply_ID", null);
            else
                Set_Value("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
        }/** Get Project Issue.
@return Project Issues (Material, Labor) */
        public int GetVAB_ProjectSupply_ID() { Object ii = Get_Value("VAB_ProjectSupply_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Current Cost.
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
@param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID <= 0) Set_Value("VAM_PFeature_SetInstance_ID", null);
            else
                Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }/** Get Attribute Set Instance.
@return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID() { Object ii = Get_Value("VAM_PFeature_SetInstance_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set VAM_ProductCostElementDetail_ID.
@param VAM_ProductCostElementDetail_ID VAM_ProductCostElementDetail_ID */
        public void SetVAM_ProductCostElementDetail_ID(int VAM_ProductCostElementDetail_ID) { if (VAM_ProductCostElementDetail_ID < 1) throw new ArgumentException("VAM_ProductCostElementDetail_ID is mandatory."); Set_ValueNoCheck("VAM_ProductCostElementDetail_ID", VAM_ProductCostElementDetail_ID); }/** Get VAM_ProductCostElementDetail_ID.
@return VAM_ProductCostElementDetail_ID */
        public int GetVAM_ProductCostElementDetail_ID() { Object ii = Get_Value("VAM_ProductCostElementDetail_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cost Element.
@param VAM_ProductCostElement_ID Product Cost Element */
        public void SetVAM_ProductCostElement_ID(int VAM_ProductCostElement_ID) { if (VAM_ProductCostElement_ID < 1) throw new ArgumentException("VAM_ProductCostElement_ID is mandatory."); Set_Value("VAM_ProductCostElement_ID", VAM_ProductCostElement_ID); }/** Get Cost Element.
@return Product Cost Element */
        public int GetVAM_ProductCostElement_ID() { Object ii = Get_Value("VAM_ProductCostElement_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Shipment/Receipt Line.
@param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_Value("VAM_Inv_InOutLine_ID", null);
            else
                Set_Value("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }/** Get Shipment/Receipt Line.
@return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID() { Object ii = Get_Value("VAM_Inv_InOutLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Phys Inventory Line.
@param VAM_InventoryLine_ID Unique line in an Inventory document */
        public void SetVAM_InventoryLine_ID(int VAM_InventoryLine_ID)
        {
            if (VAM_InventoryLine_ID <= 0) Set_Value("VAM_InventoryLine_ID", null);
            else
                Set_Value("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
        }/** Get Phys Inventory Line.
@return Unique line in an Inventory document */
        public int GetVAM_InventoryLine_ID() { Object ii = Get_Value("VAM_InventoryLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Move Line.
@param VAM_InvTrf_Line_ID Inventory Move document Line */
        public void SetVAM_InvTrf_Line_ID(int VAM_InvTrf_Line_ID)
        {
            if (VAM_InvTrf_Line_ID <= 0) Set_Value("VAM_InvTrf_Line_ID", null);
            else
                Set_Value("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
        }/** Get Move Line.
@return Inventory Move document Line */
        public int GetVAM_InvTrf_Line_ID() { Object ii = Get_Value("VAM_InvTrf_Line_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Product.
@param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID) { if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory."); Set_ValueNoCheck("VAM_Product_ID", VAM_Product_ID); }/** Get Product.
@return Product, Service, Item */
        public int GetVAM_Product_ID() { Object ii = Get_Value("VAM_Product_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Production Line.
@param VAM_ProductionLine_ID Document Line representing a production */
        public void SetVAM_ProductionLine_ID(int VAM_ProductionLine_ID)
        {
            if (VAM_ProductionLine_ID <= 0) Set_Value("VAM_ProductionLine_ID", null);
            else
                Set_Value("VAM_ProductionLine_ID", VAM_ProductionLine_ID);
        }/** Get Production Line.
@return Document Line representing a production */
        public int GetVAM_ProductionLine_ID() { Object ii = Get_Value("VAM_ProductionLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Work Order Resource Transaction Line.
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
        @param VAM_Warehouse_ID Storage Warehouse and Service Point */
        public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID)
        {
            if (VAM_Warehouse_ID <= 0) Set_Value("VAM_Warehouse_ID", null);
            else
                Set_Value("VAM_Warehouse_ID", VAM_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetVAM_Warehouse_ID()
        {
            Object ii = Get_Value("VAM_Warehouse_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }
    }
}