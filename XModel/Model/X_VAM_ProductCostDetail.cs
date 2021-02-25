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
    /** Generated Model for VAM_ProductCostDetail
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductCostDetail : VAdvantage.Model.PO
    {
        public X_VAM_ProductCostDetail(Context ctx, int VAM_ProductCostDetail_ID, Trx trxName)
            : base(ctx, VAM_ProductCostDetail_ID, trxName)
        {
            /** if (VAM_ProductCostDetail_ID == 0)
            {
            SetAmt (0.0);
            SetVAB_AccountBook_ID (0);
            SetIsSOTrx (false);
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductCostDetail_ID (0);
            SetVAM_Product_ID (0);
            SetProcessed (false);	// N
            SetQty (0.0);
            }
             */
        }
        public X_VAM_ProductCostDetail(Ctx ctx, int VAM_ProductCostDetail_ID, Trx trxName)
            : base(ctx, VAM_ProductCostDetail_ID, trxName)
        {
            /** if (VAM_ProductCostDetail_ID == 0)
            {
            SetAmt (0.0);
            SetVAB_AccountBook_ID (0);
            SetIsSOTrx (false);
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductCostDetail_ID (0);
            SetVAM_Product_ID (0);
            SetProcessed (false);	// N
            SetQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostDetail(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostDetail(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostDetail(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductCostDetail()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new VAdvantage.Model.KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514378911L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062122L;
        /** VAF_TableView_ID=808 */
        public static int Table_ID;
        // =808;

        /** TableName=VAM_ProductCostDetail */
        public static String Table_Name = "VAM_ProductCostDetail";

        protected static VAdvantage.Model.KeyNamePair model;
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
        @return VAdvantage.Model.PO Info
        */
        protected override VAdvantage.Model.POInfo InitPO(Context ctx)
        {
            VAdvantage.Model.POInfo poi = VAdvantage.Model.POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return VAdvantage.Model.PO Info
        */
        protected override VAdvantage.Model.POInfo InitPO(Ctx ctx)
        {
            VAdvantage.Model.POInfo poi = VAdvantage.Model.POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAM_ProductCostDetail[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Amount.
        @param Amt Amount */
        public void SetAmt(Decimal? Amt)
        {
            if (Amt == null) throw new ArgumentException("Amt is mandatory.");
            Set_Value("Amt", (Decimal?)Amt);
        }
        /** Get Amount.
        @return Amount */
        public Decimal GetAmt()
        {
            Object bd = Get_Value("Amt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Accounting Schema.
        @param VAB_AccountBook_ID Rules for accounting */
        public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID)
        {
            if (VAB_AccountBook_ID < 1) throw new ArgumentException("VAB_AccountBook_ID is mandatory.");
            Set_ValueNoCheck("VAB_AccountBook_ID", VAB_AccountBook_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetVAB_AccountBook_ID()
        {
            Object ii = Get_Value("VAB_AccountBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Line.
        @param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_ValueNoCheck("VAB_InvoiceLine_ID", null);
            else
                Set_ValueNoCheck("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID()
        {
            Object ii = Get_Value("VAB_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Line.
        @param VAB_OrderLine_ID Order Line */
        public void SetVAB_OrderLine_ID(int VAB_OrderLine_ID)
        {
            if (VAB_OrderLine_ID <= 0) Set_ValueNoCheck("VAB_OrderLine_ID", null);
            else
                Set_ValueNoCheck("VAB_OrderLine_ID", VAB_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetVAB_OrderLine_ID()
        {
            Object ii = Get_Value("VAB_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Issue.
        @param VAB_ProjectSupply_ID Project Issues (Material, Labor) */
        public void SetVAB_ProjectSupply_ID(int VAB_ProjectSupply_ID)
        {
            if (VAB_ProjectSupply_ID <= 0) Set_Value("VAB_ProjectSupply_ID", null);
            else
                Set_Value("VAB_ProjectSupply_ID", VAB_ProjectSupply_ID);
        }
        /** Get Project Issue.
        @return Project Issues (Material, Labor) */
        public int GetVAB_ProjectSupply_ID()
        {
            Object ii = Get_Value("VAB_ProjectSupply_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Delta Amount.
        @param DeltaAmt Difference Amount */
        public void SetDeltaAmt(Decimal? DeltaAmt)
        {
            Set_Value("DeltaAmt", (Decimal?)DeltaAmt);
        }
        /** Get Delta Amount.
        @return Difference Amount */
        public Decimal GetDeltaAmt()
        {
            Object bd = Get_Value("DeltaAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Delta Quantity.
        @param DeltaQty Quantity Difference */
        public void SetDeltaQty(Decimal? DeltaQty)
        {
            Set_Value("DeltaQty", (Decimal?)DeltaQty);
        }
        /** Get Delta Quantity.
        @return Quantity Difference */
        public Decimal GetDeltaQty()
        {
            Object bd = Get_Value("DeltaQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Description.
        @param Description Optional short description of the record */
        public void SetDescription(String Description)
        {
            if (Description != null && Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Description = Description.Substring(0, 255);
            }
            Set_Value("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_Value("IsSOTrx", IsSOTrx);
        }
        /** Get Sales Transaction.
        @return This is a Sales Transaction */
        public Boolean IsSOTrx()
        {
            Object oo = Get_Value("IsSOTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID < 0) throw new ArgumentException("VAM_PFeature_SetInstance_ID is mandatory.");
            Set_ValueNoCheck("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID()
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Detail.
        @param VAM_ProductCostDetail_ID Cost Detail Information */
        public void SetVAM_ProductCostDetail_ID(int VAM_ProductCostDetail_ID)
        {
            if (VAM_ProductCostDetail_ID < 1) throw new ArgumentException("VAM_ProductCostDetail_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductCostDetail_ID", VAM_ProductCostDetail_ID);
        }
        /** Get Cost Detail.
        @return Cost Detail Information */
        public int GetVAM_ProductCostDetail_ID()
        {
            Object ii = Get_Value("VAM_ProductCostDetail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Element.
        @param VAM_ProductCostElement_ID Product Cost Element */
        public void SetVAM_ProductCostElement_ID(int VAM_ProductCostElement_ID)
        {
            if (VAM_ProductCostElement_ID <= 0) Set_ValueNoCheck("VAM_ProductCostElement_ID", null);
            else
                Set_ValueNoCheck("VAM_ProductCostElement_ID", VAM_ProductCostElement_ID);
        }
        /** Get Cost Element.
        @return Product Cost Element */
        public int GetVAM_ProductCostElement_ID()
        {
            Object ii = Get_Value("VAM_ProductCostElement_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_ValueNoCheck("VAM_Inv_InOutLine_ID", null);
            else
                Set_ValueNoCheck("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys Inventory Line.
        @param VAM_InventoryLine_ID Unique line in an Inventory document */
        public void SetVAM_InventoryLine_ID(int VAM_InventoryLine_ID)
        {
            if (VAM_InventoryLine_ID <= 0) Set_Value("VAM_InventoryLine_ID", null);
            else
                Set_Value("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetVAM_InventoryLine_ID()
        {
            Object ii = Get_Value("VAM_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Move Line.
        @param VAM_InvTrf_Line_ID Inventory Move document Line */
        public void SetVAM_InvTrf_Line_ID(int VAM_InvTrf_Line_ID)
        {
            if (VAM_InvTrf_Line_ID <= 0) Set_Value("VAM_InvTrf_Line_ID", null);
            else
                Set_Value("VAM_InvTrf_Line_ID", VAM_InvTrf_Line_ID);
        }
        /** Get Move Line.
        @return Inventory Move document Line */
        public int GetVAM_InvTrf_Line_ID()
        {
            Object ii = Get_Value("VAM_InvTrf_Line_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory.");
            Set_ValueNoCheck("VAM_Product_ID", VAM_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetVAM_Product_ID()
        {
            Object ii = Get_Value("VAM_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Production Line.
        @param VAM_ProductionLine_ID Document Line representing a production */
        public void SetVAM_ProductionLine_ID(int VAM_ProductionLine_ID)
        {
            if (VAM_ProductionLine_ID <= 0) Set_Value("VAM_ProductionLine_ID", null);
            else
                Set_Value("VAM_ProductionLine_ID", VAM_ProductionLine_ID);
        }
        /** Get Production Line.
        @return Document Line representing a production */
        public int GetVAM_ProductionLine_ID()
        {
            Object ii = Get_Value("VAM_ProductionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Price.
        @param Price Price */
        public void SetPrice(Decimal? Price)
        {
            throw new ArgumentException("Price Is virtual column");
        }
        /** Get Price.
        @return Price */
        public Decimal GetPrice()
        {
            Object bd = Get_Value("Price");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
        }
        /** Get Processed.
        @return The document has been processed */
        public Boolean IsProcessed()
        {
            Object oo = Get_Value("Processed");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Quantity.
        @param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            if (Qty == null) throw new ArgumentException("Qty is mandatory.");
            Set_Value("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Work Order Transaction Line.
@param VAM_WorkOrderTransactionLine_ID Work Order Transaction Line */
        public void SetVAM_WorkOrderTransactionLine_ID(int VAM_WorkOrderTransactionLine_ID)
        {
            if (VAM_WorkOrderTransactionLine_ID <= 0) Set_Value("VAM_WorkOrderTransactionLine_ID", null);
            else
                Set_Value("VAM_WorkOrderTransactionLine_ID", VAM_WorkOrderTransactionLine_ID);
        }
        /** Get Work Order Transaction Line.
        @return Work Order Transaction Line */
        public int GetVAM_WorkOrderTransactionLine_ID()
        {
            Object ii = Get_Value("VAM_WorkOrderTransactionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Resource Transaction Line.
@param VAM_WorkOrderResourceTxnLine_ID Identifies the production resource detail lines in a work order transaction */
        public void SetVAM_WorkOrderResourceTxnLine_ID(int VAM_WorkOrderResourceTxnLine_ID)
        {
            if (VAM_WorkOrderResourceTxnLine_ID <= 0) Set_Value("VAM_WorkOrderResourceTxnLine_ID", null);
            else
                Set_Value("VAM_WorkOrderResourceTxnLine_ID", VAM_WorkOrderResourceTxnLine_ID);
        }
        /** Get Work Order Resource Transaction Line.
        @return Identifies the production resource detail lines in a work order transaction */
        public int GetVAM_WorkOrderResourceTxnLine_ID()
        {
            Object ii = Get_Value("VAM_WorkOrderResourceTxnLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        // Added By Pratap 8-4-2015 VAFAM
        /** Set Asset.
@param VAA_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int VAA_Asset_ID)
        {
            if (VAA_Asset_ID < 1) throw new ArgumentException("VAA_Asset_ID is mandatory.");
            Set_ValueNoCheck("VAA_Asset_ID", VAA_Asset_ID);
        }
        /** Get Asset.
        @return Asset used internally or by customers */
        public int GetA_Asset_ID()
        {
            Object ii = Get_Value("VAA_Asset_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Asset Depreciation Line.
@param VAFAM_AssetDepreciationLine_ID Asset Depreciation Line */
        public void SetVAFAM_AssetDepreciationLine_ID(int VAFAM_AssetDepreciationLine_ID)
        {
            if (VAFAM_AssetDepreciationLine_ID <= 0) Set_Value("VAFAM_AssetDepreciationLine_ID", null);
            else
                Set_Value("VAFAM_AssetDepreciationLine_ID", VAFAM_AssetDepreciationLine_ID);
        }
        /** Get Asset Depreciation Line.
        @return Asset Depreciation Line */
        public int GetVAFAM_AssetDepreciationLine_ID()
        {
            Object ii = Get_Value("VAFAM_AssetDepreciationLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Current Cost.
@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice)
        {
            Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice);
        }
        /** Get Current Cost.
        @return The currently used cost price */
        public Decimal GetCurrentCostPrice()
        {
            Object bd = Get_Value("CurrentCostPrice");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //End

        /** Set Work Order Resource Transaction Line.@param VAMFG_M_WrkOdrRscTxnLine_ID Work Order Resource Transaction Line */
        public void SetVAMFG_M_WrkOdrRscTxnLine_ID(int VAMFG_M_WrkOdrRscTxnLine_ID)
        {
            if (VAMFG_M_WrkOdrRscTxnLine_ID <= 0) Set_Value("VAMFG_M_WrkOdrRscTxnLine_ID", null);
            else
                Set_Value("VAMFG_M_WrkOdrRscTxnLine_ID", VAMFG_M_WrkOdrRscTxnLine_ID);
        }
        /** Get Work Order Resource Transaction Line.@return Work Order Resource Transaction Line */
        public int GetVAMFG_M_WrkOdrRscTxnLine_ID()
        {
            Object ii = Get_Value("VAMFG_M_WrkOdrRscTxnLine_ID");
            if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        /** Set VAMFG_M_WrkOdrTrnsctionLine_ID.@param VAMFG_M_WrkOdrTrnsctionLine_ID VAMFG_M_WrkOdrTrnsctionLine_ID */
        public void SetVAMFG_M_WrkOdrTrnsctionLine_ID(int VAMFG_M_WrkOdrTrnsctionLine_ID)
        {
            if (VAMFG_M_WrkOdrTrnsctionLine_ID <= 0) Set_Value("VAMFG_M_WrkOdrTrnsctionLine_ID", null);
            else
                Set_Value("VAMFG_M_WrkOdrTrnsctionLine_ID", VAMFG_M_WrkOdrTrnsctionLine_ID);
        }
        /** Get VAMFG_M_WrkOdrTrnsctionLine_ID.@return VAMFG_M_WrkOdrTrnsctionLine_ID */
        public int GetVAMFG_M_WrkOdrTrnsctionLine_ID()
        {
            Object ii = Get_Value("VAMFG_M_WrkOdrTrnsctionLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }


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
