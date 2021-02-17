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
    /** Generated Model for VAM_InventoryLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_InventoryLine : PO
    {
        public X_VAM_InventoryLine(Context ctx, int VAM_InventoryLine_ID, Trx trxName)
            : base(ctx, VAM_InventoryLine_ID, trxName)
        {
            /** if (VAM_InventoryLine_ID == 0)
            {
            SetInventoryType (null);	// D
            SetIsInternalUse (false);
            SetVAM_InventoryLine_ID (0);
            SetVAM_Inventory_ID (0);
            SetVAM_Locator_ID (0);	// @VAM_Locator_ID@
            SetVAM_Product_ID (0);
            SetProcessed (false);	// N
            SetQtyBook (0.0);
            SetQtyCount (0.0);
            }
             */
        }
        public X_VAM_InventoryLine(Ctx ctx, int VAM_InventoryLine_ID, Trx trxName)
            : base(ctx, VAM_InventoryLine_ID, trxName)
        {
            /** if (VAM_InventoryLine_ID == 0)
            {
            SetInventoryType (null);	// D
            SetIsInternalUse (false);
            SetVAM_InventoryLine_ID (0);
            SetVAM_Inventory_ID (0);
            SetVAM_Locator_ID (0);	// @VAM_Locator_ID@
            SetVAM_Product_ID (0);
            SetProcessed (false);	// N
            SetQtyBook (0.0);
            SetQtyCount (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_InventoryLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_InventoryLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_InventoryLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_InventoryLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514379741L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062952L;
        /** VAF_TableView_ID=322 */
        public static int Table_ID;
        // =322;

        /** TableName=VAM_InventoryLine */
        public static String Table_Name = "VAM_InventoryLine";

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
        protected override POInfo InitPO(Context ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Load Meta Data
        @param ctx context
        @return PO Info
        */
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
        }
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAM_InventoryLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Charge.
        @param VAB_Charge_ID Additional document charges */
        public void SetVAB_Charge_ID(int VAB_Charge_ID)
        {
            if (VAB_Charge_ID <= 0) Set_Value("VAB_Charge_ID", null);
            else
                Set_Value("VAB_Charge_ID", VAB_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetVAB_Charge_ID()
        {
            Object ii = Get_Value("VAB_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** InventoryType VAF_Control_Ref_ID=292 */
        public static int INVENTORYTYPE_VAF_Control_Ref_ID = 292;
        /** Charge Account = C */
        public static String INVENTORYTYPE_ChargeAccount = "C";
        /** Inventory Difference = D */
        public static String INVENTORYTYPE_InventoryDifference = "D";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsInventoryTypeValid(String test)
        {
            return test.Equals("C") || test.Equals("D");
        }
        /** Set Inventory Type.
        @param InventoryType Type of inventory difference */
        public void SetInventoryType(String InventoryType)
        {
            if (InventoryType == null) throw new ArgumentException("InventoryType is mandatory");
            if (!IsInventoryTypeValid(InventoryType))
                throw new ArgumentException("InventoryType Invalid value - " + InventoryType + " - Reference_ID=292 - C - D");
            if (InventoryType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                InventoryType = InventoryType.Substring(0, 1);
            }
            Set_Value("InventoryType", InventoryType);
        }
        /** Get Inventory Type.
        @return Type of inventory difference */
        public String GetInventoryType()
        {
            return (String)Get_Value("InventoryType");
        }
        /** Set Internal Use.
        @param IsInternalUse The Record is internal use */
        public void SetIsInternalUse(Boolean IsInternalUse)
        {
            Set_Value("IsInternalUse", IsInternalUse);
        }
        /** Get Internal Use.
        @return The Record is internal use */
        public Boolean IsInternalUse()
        {
            Object oo = Get_Value("IsInternalUse");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID <= 0) Set_Value("VAM_PFeature_SetInstance_ID", null);
            else
                Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID()
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys Inventory Line.
        @param VAM_InventoryLine_ID Unique line in an Inventory document */
        public void SetVAM_InventoryLine_ID(int VAM_InventoryLine_ID)
        {
            if (VAM_InventoryLine_ID < 1) throw new ArgumentException("VAM_InventoryLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_InventoryLine_ID", VAM_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetVAM_InventoryLine_ID()
        {
            Object ii = Get_Value("VAM_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys.Inventory.
        @param VAM_Inventory_ID Parameters for a Physical Inventory */
        public void SetVAM_Inventory_ID(int VAM_Inventory_ID)
        {
            if (VAM_Inventory_ID < 1) throw new ArgumentException("VAM_Inventory_ID is mandatory.");
            Set_ValueNoCheck("VAM_Inventory_ID", VAM_Inventory_ID);
        }
        /** Get Phys.Inventory.
        @return Parameters for a Physical Inventory */
        public int GetVAM_Inventory_ID()
        {
            Object ii = Get_Value("VAM_Inventory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param VAM_Locator_ID Warehouse Locator */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID < 1) throw new ArgumentException("VAM_Locator_ID is mandatory.");
            Set_Value("VAM_Locator_ID", VAM_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetVAM_Locator_ID()
        {
            Object ii = Get_Value("VAM_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAM_Product_ID VAF_Control_Ref_ID=171 */
        public static int VAM_Product_ID_VAF_Control_Ref_ID = 171;
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory.");
            Set_Value("VAM_Product_ID", VAM_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetVAM_Product_ID()
        {
            Object ii = Get_Value("VAM_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Requisition Line.
        @param VAM_RequisitionLine_ID Material Requisition Line */
        public void SetVAM_RequisitionLine_ID(int VAM_RequisitionLine_ID)
        {
            if (VAM_RequisitionLine_ID <= 0) Set_Value("VAM_RequisitionLine_ID", null);
            else
                Set_Value("VAM_RequisitionLine_ID", VAM_RequisitionLine_ID);
        }
        /** Get Requisition Line.
        @return Material Requisition Line */
        public int GetVAM_RequisitionLine_ID()
        {
            Object ii = Get_Value("VAM_RequisitionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Quantity book.
        @param QtyBook Book Quantity */
        public void SetQtyBook(Decimal? QtyBook)
        {
            if (QtyBook == null) throw new ArgumentException("QtyBook is mandatory.");
            Set_ValueNoCheck("QtyBook", (Decimal?)QtyBook);
        }
        /** Get Quantity book.
        @return Book Quantity */
        public Decimal GetQtyBook()
        {
            Object bd = Get_Value("QtyBook");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity count.
        @param QtyCount Counted Quantity */
        public void SetQtyCount(Decimal? QtyCount)
        {
            if (QtyCount == null) throw new ArgumentException("QtyCount is mandatory.");
            Set_Value("QtyCount", (Decimal?)QtyCount);
        }
        /** Get Quantity count.
        @return Counted Quantity */
        public Decimal GetQtyCount()
        {
            Object bd = Get_Value("QtyCount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Internal Use Qty.
        @param QtyInternalUse Internal Use Quantity removed from Inventory */
        public void SetQtyInternalUse(Decimal? QtyInternalUse)
        {
            Set_Value("QtyInternalUse", (Decimal?)QtyInternalUse);
        }
        /** Get Internal Use Qty.
        @return Internal Use Quantity removed from Inventory */
        public Decimal GetQtyInternalUse()
        {
            Object bd = Get_Value("QtyInternalUse");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** AdjustmentType VAF_Control_Ref_ID=1000164 */
        public static int ADJUSTMENTTYPE_VAF_Control_Ref_ID = 1000164;
        /** As On Date Count = A */
        public static String ADJUSTMENTTYPE_AsOnDateCount = "A";
        /** Quantity Difference = D */
        public static String ADJUSTMENTTYPE_QuantityDifference = "D";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsAdjustmentTypeValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("D");
        }
        /** Set Adjustment Type.
        @param AdjustmentType Adjustment Type */
        public void SetAdjustmentType(String AdjustmentType)
        {
            if (!IsAdjustmentTypeValid(AdjustmentType))
                throw new ArgumentException("AdjustmentType Invalid value - " + AdjustmentType + " - Reference_ID=1000164 - A - D");
            if (AdjustmentType != null && AdjustmentType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                AdjustmentType = AdjustmentType.Substring(0, 1);
            }
            Set_Value("AdjustmentType", AdjustmentType);
        }
        /** Get Adjustment Type.
        @return Adjustment Type */
        public String GetAdjustmentType()
        {
            return (String)Get_Value("AdjustmentType");
        }
        /** Set Difference Quantity.
        @param DifferenceQty Difference Quantity */
        public void SetDifferenceQty(Decimal? DifferenceQty)
        {
            Set_Value("DifferenceQty", (Decimal?)DifferenceQty);
        }
        /** Get Difference Quantity.
        @return Difference Quantity */
        public Decimal GetDifferenceQty()
        {
            Object bd = Get_Value("DifferenceQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set As On Date Count.
        @param AsOnDateCount As On Date Count */
        public void SetAsOnDateCount(Decimal? AsOnDateCount)
        {
            Set_Value("AsOnDateCount", (Decimal?)AsOnDateCount);
        }
        /** Get As On Date Count.
        @return As On Date Count */
        public Decimal GetAsOnDateCount()
        {
            Object bd = Get_Value("AsOnDateCount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Opening Stock.
        @param OpeningStock Opening Stock */
        public void SetOpeningStock(Decimal? OpeningStock)
        {
            Set_Value("OpeningStock", (Decimal?)OpeningStock);
        }
        /** Get Opening Stock.
        @return Opening Stock */
        public Decimal GetOpeningStock()
        {
            Object bd = Get_Value("OpeningStock");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set UPC/EAN.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            throw new ArgumentException("UPC Is virtual column");
        }
        /** Get UPC/EAN.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            throw new ArgumentException("Value Is virtual column");
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }

        /** Set Analysis Group.
@param M_ABCAnalysisGroup_ID Analysis Group */
        public void SetM_ABCAnalysisGroup_ID(int M_ABCAnalysisGroup_ID)
        {
            if (M_ABCAnalysisGroup_ID <= 0) Set_Value("M_ABCAnalysisGroup_ID", null);
            else
                Set_Value("M_ABCAnalysisGroup_ID", M_ABCAnalysisGroup_ID);
        }
        /** Get Analysis Group.
        @return Analysis Group */
        public int GetM_ABCAnalysisGroup_ID()
        {
            Object ii = Get_Value("M_ABCAnalysisGroup_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Employee.
        @param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }
        /** Get Employee.
        @return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID()
        {
            Object ii = Get_Value("VAB_BusinessPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Cost Calculated.@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }
        /** Get Cost Calculated. @return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Reversed Cost Calculated.@param IsReversedCostCalculated Reversed Cost Calculated */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }
        /** Get Reversed Cost Calculated. @return Reversed Cost Calculated */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Cost Immediately.@param IsCostImmediate Update Costs immediately for testing */
        public void SetIsCostImmediate(Boolean IsCostImmediate) { Set_Value("IsCostImmediate", IsCostImmediate); }
        /** Get Cost Immediately.@return Update Costs immediately for testing */
        public Boolean IsCostImmediate() { Object oo = Get_Value("IsCostImmediate"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }


        /** Set Cost Price. @param VA024_CostPrice Cost Price */
        public void SetVA024_CostPrice(Decimal? VA024_CostPrice) { Set_ValueNoCheck("VA024_CostPrice", (Decimal?)VA024_CostPrice); }
        /** Get Cost Price.@return Cost Price */
        public Decimal GetVA024_CostPrice() { Object bd = Get_Value("VA024_CostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Unit Price.@param VA024_UnitPrice Unit Price */
        public void SetVA024_UnitPrice(Decimal? VA024_UnitPrice) { Set_ValueNoCheck("VA024_UnitPrice", (Decimal?)VA024_UnitPrice); }
        /** Get Unit Price.@return Unit Price */
        public Decimal GetVA024_UnitPrice() { Object bd = Get_Value("VA024_UnitPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Current Cost.@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }
        /** Get Current Cost.@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set From Process. @param IsFromProcess Specifies that the record created from Process. */
        public void SetIsFromProcess(Boolean IsFromProcess)
        {
            Set_Value("IsFromProcess", IsFromProcess);
        }
        /** Get From Process. @return Specifies that the record created from Process. */
        public Boolean IsFromProcess()
        {
            Object oo = Get_Value("IsFromProcess"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false;
        }

        /** Set Product Container.@param VAM_ProductContainer_ID Product Container */
        public void SetVAM_ProductContainer_ID(int VAM_ProductContainer_ID)
        {
            if (VAM_ProductContainer_ID <= 0) Set_Value("VAM_ProductContainer_ID", null);
            else
                Set_Value("VAM_ProductContainer_ID", VAM_ProductContainer_ID);
        }
        /** Get Product Container.@return Product Container */
        public int GetVAM_ProductContainer_ID() { Object ii = Get_Value("VAM_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** ReversalDoc_ID VAF_Control_Ref_ID=1000209 */
        public static int REVERSALDOC_ID_VAF_Control_Ref_ID = 1000209;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Actual Requisition Reserved / Ordered.@param ActualReqReserved Actual Requisition Reserved / Ordered */
        public void SetActualReqReserved(Decimal? ActualReqReserved) { Set_Value("ActualReqReserved", (Decimal?)ActualReqReserved); }
        /** Get Actual Requisition Reserved / Ordered.@return Actual Requisition Reserved / Ordered */
        public Decimal GetActualReqReserved() { Object bd = Get_Value("ActualReqReserved"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Post Current Cost Price.@param PostCurrentCostPrice It indicate the cost after cost calculation of current record */
        public void SetPostCurrentCostPrice(Decimal? PostCurrentCostPrice) { Set_Value("PostCurrentCostPrice", (Decimal?)PostCurrentCostPrice); }
        /** Get Post Current Cost Price. @return It indicate the cost after cost calculation of current record */
        public Decimal GetPostCurrentCostPrice() { Object bd = Get_Value("PostCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

    }

}
