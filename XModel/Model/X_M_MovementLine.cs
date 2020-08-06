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
    /** Generated Model for M_MovementLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_MovementLine : PO
    {
        public X_M_MovementLine(Context ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            /** if (M_MovementLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=@M_Movement_ID@
            SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_MovementLine_ID (0);
            SetM_Movement_ID (0);
            SetM_Product_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            }
             */
        }
        public X_M_MovementLine(Ctx ctx, int M_MovementLine_ID, Trx trxName)
            : base(ctx, M_MovementLine_ID, trxName)
        {
            /** if (M_MovementLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_MovementLine WHERE M_Movement_ID=@M_Movement_ID@
            SetM_LocatorTo_ID (0);	// @M_LocatorTo_ID@
            SetM_Locator_ID (0);	// @M_Locator_ID@
            SetM_MovementLine_ID (0);
            SetM_Movement_ID (0);
            SetM_Product_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_MovementLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_MovementLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514380102L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063313L;
        /** AD_Table_ID=324 */
        public static int Table_ID;
        // =324;

        /** TableName=M_MovementLine */
        public static String Table_Name = "M_MovementLine";

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
            StringBuilder sb = new StringBuilder("X_M_MovementLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Asset.
        @param A_Asset_ID Asset used internally or by customers */
        public void SetA_Asset_ID(int A_Asset_ID)
        {
            if (A_Asset_ID <= 0) Set_Value("A_Asset_ID", null);
            else
                Set_Value("A_Asset_ID", A_Asset_ID);
        }
        /** Get Asset.
        @return Asset used internally or by customers */
        public int GetA_Asset_ID()
        {
            Object ii = Get_Value("A_Asset_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Confirmed Quantity.
        @param ConfirmedQty Confirmation of a received quantity */
        public void SetConfirmedQty(Decimal? ConfirmedQty)
        {
            Set_Value("ConfirmedQty", (Decimal?)ConfirmedQty);
        }
        /** Get Confirmed Quantity.
        @return Confirmation of a received quantity */
        public Decimal GetConfirmedQty()
        {
            Object bd = Get_Value("ConfirmedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Number.
        @param DTD001_AttributeNumber Attribute Number */
        public void SetDTD001_AttributeNumber(String DTD001_AttributeNumber)
        {
            Set_Value("DTD001_AttributeNumber", DTD001_AttributeNumber);
        }
        /** Get Attribute Number.
        @return Attribute Number */
        public String GetDTD001_AttributeNumber()
        {
            return (String)Get_Value("DTD001_AttributeNumber");
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

        /** M_AttributeSetInstanceTo_ID AD_Reference_ID=418 */
        public static int M_ATTRIBUTESETINSTANCETO_ID_AD_Reference_ID = 418;
        /** Set Attribute Set Instance To.
        @param M_AttributeSetInstanceTo_ID Target Product Attribute Set Instance */
        public void SetM_AttributeSetInstanceTo_ID(int M_AttributeSetInstanceTo_ID)
        {
            if (M_AttributeSetInstanceTo_ID <= 0) Set_ValueNoCheck("M_AttributeSetInstanceTo_ID", null);
            else
                Set_ValueNoCheck("M_AttributeSetInstanceTo_ID", M_AttributeSetInstanceTo_ID);
        }
        /** Get Attribute Set Instance To.
        @return Target Product Attribute Set Instance */
        public int GetM_AttributeSetInstanceTo_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstanceTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_LocatorTo_ID AD_Reference_ID=191 */
        public static int M_LOCATORTO_ID_AD_Reference_ID = 191;
        /** Set Locator To.
        @param M_LocatorTo_ID Location inventory is moved to */
        public void SetM_LocatorTo_ID(int M_LocatorTo_ID)
        {
            if (M_LocatorTo_ID < 1) throw new ArgumentException("M_LocatorTo_ID is mandatory.");
            Set_Value("M_LocatorTo_ID", M_LocatorTo_ID);
        }
        /** Get Locator To.
        @return Location inventory is moved to */
        public int GetM_LocatorTo_ID()
        {
            Object ii = Get_Value("M_LocatorTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID < 1) throw new ArgumentException("M_Locator_ID is mandatory.");
            Set_Value("M_Locator_ID", M_Locator_ID);
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
        public void SetM_MovementLine_ID(int M_MovementLine_ID)
        {
            if (M_MovementLine_ID < 1) throw new ArgumentException("M_MovementLine_ID is mandatory.");
            Set_ValueNoCheck("M_MovementLine_ID", M_MovementLine_ID);
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
        public void SetM_Movement_ID(int M_Movement_ID)
        {
            if (M_Movement_ID < 1) throw new ArgumentException("M_Movement_ID is mandatory.");
            Set_ValueNoCheck("M_Movement_ID", M_Movement_ID);
        }
        /** Get Inventory Move.
        @return Movement of Inventory */
        public int GetM_Movement_ID()
        {
            Object ii = Get_Value("M_Movement_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_Product_ID AD_Reference_ID=171 */
        public static int M_PRODUCT_ID_AD_Reference_ID = 171;
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
            Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Requisition Line.
        @param M_RequisitionLine_ID Material Requisition Line */
        public void SetM_RequisitionLine_ID(int M_RequisitionLine_ID)
        {
            if (M_RequisitionLine_ID <= 0) Set_Value("M_RequisitionLine_ID", null);
            else
                Set_Value("M_RequisitionLine_ID", M_RequisitionLine_ID);
        }
        /** Get Requisition Line.
        @return Material Requisition Line */
        public int GetM_RequisitionLine_ID()
        {
            Object ii = Get_Value("M_RequisitionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_Value("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
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
        /** Set Scrapped Quantity.
        @param ScrappedQty The Quantity scrapped due to QA issues */
        public void SetScrappedQty(Decimal? ScrappedQty)
        {
            Set_Value("ScrappedQty", (Decimal?)ScrappedQty);
        }
        /** Get Scrapped Quantity.
        @return The Quantity scrapped due to QA issues */
        public Decimal GetScrappedQty()
        {
            Object bd = Get_Value("ScrappedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Target Quantity.
        @param TargetQty Target Movement Quantity */
        public void SetTargetQty(Decimal? TargetQty)
        {
            Set_Value("TargetQty", (Decimal?)TargetQty);
        }
        /** Get Target Quantity.
        @return Target Movement Quantity */
        public Decimal GetTargetQty()
        {
            Object bd = Get_Value("TargetQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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

        /** Set Post Current Cost Price.
@param PostCurrentCostPrice It indicate the Product Cost after cost calculation of the current record. */
        public void SetPostCurrentCostPrice(Decimal? PostCurrentCostPrice) { Set_Value("PostCurrentCostPrice", (Decimal?)PostCurrentCostPrice); }/** Get Post Current Cost Price.
@return It indicate the Product Cost after cost calculation of the current record. */
        public Decimal GetPostCurrentCostPrice() { Object bd = Get_Value("PostCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set To Post Current  Cost Price.
@param ToPostCurrentCostPrice To Post Current  Cost Price */
        public void SetToPostCurrentCostPrice(Decimal? ToPostCurrentCostPrice) { Set_Value("ToPostCurrentCostPrice", (Decimal?)ToPostCurrentCostPrice); }/** Get To Post Current  Cost Price.
@return To Post Current  Cost Price */
        public Decimal GetToPostCurrentCostPrice() { Object bd = Get_Value("ToPostCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set To Current Cost Price.
@param ToCurrentCostPrice To Current Cost Price */
        public void SetToCurrentCostPrice(Decimal? ToCurrentCostPrice) { Set_Value("ToCurrentCostPrice", (Decimal?)ToCurrentCostPrice); }/** Get To Current Cost Price.
@return To Current Cost Price */
        public Decimal GetToCurrentCostPrice() { Object bd = Get_Value("ToCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Order Line.
@param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetC_OrderLine_ID()
        {
            Object ii = Get_Value("C_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        // Arpit 19th Dec,2016
        /** Set Quantity.
@param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            Set_Value("QtyEntered", (Decimal?)QtyEntered);
        }/** Get Quantity.
@return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered()
        {
            Object bd = Get_Value("QtyEntered");
            if (bd == null)
                return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //Arpit
        //end

        /** Set Product Container.@param M_ProductContainer_ID Product Container */
        public void SetM_ProductContainer_ID(int M_ProductContainer_ID)
        {
            if (M_ProductContainer_ID <= 0) Set_Value("M_ProductContainer_ID", null);
            else
                Set_Value("M_ProductContainer_ID", M_ProductContainer_ID);
        }
        /** Get Product Container.@return Product Container */
        public int GetM_ProductContainer_ID() { Object ii = Get_Value("M_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }


        /** Ref_M_ProductContainerTo_ID AD_Reference_ID=1000205 */
        public static int REF_M_PRODUCTCONTAINERTO_ID_AD_Reference_ID = 1000205;
        /** Set To Container.@param Ref_M_ProductContainerTo_ID To Container */
        public void SetRef_M_ProductContainerTo_ID(int Ref_M_ProductContainerTo_ID)
        {
            if (Ref_M_ProductContainerTo_ID <= 0) Set_Value("Ref_M_ProductContainerTo_ID", null);
            else
                Set_Value("Ref_M_ProductContainerTo_ID", Ref_M_ProductContainerTo_ID);
        }
        /** Get To Container.@return To Container */
        public int GetRef_M_ProductContainerTo_ID() { Object ii = Get_Value("Ref_M_ProductContainerTo_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set UOM.@param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.@return Unit of Measure */
        public int GetC_UOM_ID() { Object ii = Get_Value("C_UOM_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Move Full Container.@param MoveFullContainer Move Full Container */
        public void SetMoveFullContainer(Boolean MoveFullContainer) { Set_Value("MoveFullContainer", MoveFullContainer); }
        /** Get Move Full Container.@return Move Full Container */
        public Boolean IsMoveFullContainer() { Object oo = Get_Value("MoveFullContainer"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** ReversalDoc_ID AD_Reference_ID=1000207 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 1000207;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Parent Move.@param IsParentMove Parent Move */
        public void SetIsParentMove(Boolean IsParentMove) { Set_Value("IsParentMove", IsParentMove); }
        /** Get Parent Move.@return Parent Move */
        public Boolean IsParentMove() { Object oo = Get_Value("IsParentMove"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Actual Requisition Reserved / Ordered.@param ActualReqReserved Actual Requisition Reserved / Ordered */
        public void SetActualReqReserved(Decimal? ActualReqReserved) { Set_Value("ActualReqReserved", (Decimal?)ActualReqReserved); }
        /** Get Actual Requisition Reserved / Ordered.@return Actual Requisition Reserved / Ordered */
        public Decimal GetActualReqReserved() { Object bd = Get_Value("ActualReqReserved"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** TargetContainer_ID AD_Reference_ID=1000205 */
        public static int TARGETCONTAINER_ID_AD_Reference_ID = 1000205;
        /** Set Target Container.@param TargetContainer_ID Is Used to refer the Parent product container to whom user try to move into another product container. */
        public void SetTargetContainer_ID(int TargetContainer_ID)
        {
            if (TargetContainer_ID <= 0) Set_Value("TargetContainer_ID", null);
            else
                Set_Value("TargetContainer_ID", TargetContainer_ID);
        }
        /** Get Target Container.@return Is Used to refer the Parent product container to whom user try to move into another product container. */
        public int GetTargetContainer_ID() { Object ii = Get_Value("TargetContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

    }

}
