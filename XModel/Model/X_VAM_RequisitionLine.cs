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
    /** Generated Model for VAM_RequisitionLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_RequisitionLine : PO
    {
        public X_VAM_RequisitionLine(Context ctx, int VAM_RequisitionLine_ID, Trx trxName)
            : base(ctx, VAM_RequisitionLine_ID, trxName)
        {
            /** if (VAM_RequisitionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAM_RequisitionLine WHERE VAM_Requisition_ID=@VAM_Requisition_ID@
            SetLineNetAmt (0.0);
            SetVAM_RequisitionLine_ID (0);
            SetVAM_Requisition_ID (0);
            SetPriceActual (0.0);
            SetQty (0.0);	// 1
            }
             */
        }
        public X_VAM_RequisitionLine(Ctx ctx, int VAM_RequisitionLine_ID, Trx trxName)
            : base(ctx, VAM_RequisitionLine_ID, trxName)
        {
            /** if (VAM_RequisitionLine_ID == 0)
            {
            SetLine (0);	// @SQL=SELECT COALESCE(MAX(Line),0)+10 AS DefaultValue FROM VAM_RequisitionLine WHERE VAM_Requisition_ID=@VAM_Requisition_ID@
            SetLineNetAmt (0.0);
            SetVAM_RequisitionLine_ID (0);
            SetVAM_Requisition_ID (0);
            SetPriceActual (0.0);
            SetQty (0.0);	// 1
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_RequisitionLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_RequisitionLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_RequisitionLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_RequisitionLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514381168L;
        /** Last Updated Timestamp 7/29/2010 1:07:44 PM */
        public static long updatedMS = 1280389064379L;
        /** VAF_TableView_ID=703 */
        public static int Table_ID;
        // =703;

        /** TableName=VAM_RequisitionLine */
        public static String Table_Name = "VAM_RequisitionLine";

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
        protected override POInfo InitPO(Ctx ctx)
        {
            POInfo poi = POInfo.GetPOInfo(ctx, Table_ID);
            return poi;
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
        /** Info
        @return info
        */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("X_VAM_RequisitionLine[").Append(Get_ID()).Append("]");
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
        /** Set Order Line.
        @param VAB_OrderLine_ID Order Line */
        public void SetVAB_OrderLine_ID(int VAB_OrderLine_ID)
        {
            if (VAB_OrderLine_ID <= 0) Set_Value("VAB_OrderLine_ID", null);
            else
                Set_Value("VAB_OrderLine_ID", VAB_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetVAB_OrderLine_ID()
        {
            Object ii = Get_Value("VAB_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Delivered Quantity.
        @param DTD001_DeliveredQty Delivered Quantity */
        public void SetDTD001_DeliveredQty(Decimal? DTD001_DeliveredQty)
        {
            Set_Value("DTD001_DeliveredQty", (Decimal?)DTD001_DeliveredQty);
        }
        /** Get Delivered Quantity.
        @return Delivered Quantity */
        public Decimal GetDTD001_DeliveredQty()
        {
            Object bd = Get_Value("DTD001_DeliveredQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Requisition Reserved Quantity.
        @param DTD001_ReservedQty Requisition Reserved Quantity */
        public void SetDTD001_ReservedQty(Decimal? DTD001_ReservedQty)
        {
            Set_Value("DTD001_ReservedQty", (Decimal?)DTD001_ReservedQty);
        }
        /** Get Requisition Reserved Quantity.
        @return Requisition Reserved Quantity */
        public Decimal GetDTD001_ReservedQty()
        {
            Object bd = Get_Value("DTD001_ReservedQty");
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
        /** Set Line Amount.
        @param LineNetAmt Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public void SetLineNetAmt(Decimal? LineNetAmt)
        {
            if (LineNetAmt == null) throw new ArgumentException("LineNetAmt is mandatory.");
            Set_Value("LineNetAmt", (Decimal?)LineNetAmt);
        }
        /** Get Line Amount.
        @return Line Extended Amount (Quantity * Actual Price) without Freight and Charges */
        public Decimal GetLineNetAmt()
        {
            Object bd = Get_Value("LineNetAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID <= 0) Set_Value("VAM_Product_ID", null);
            else
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
            if (VAM_RequisitionLine_ID < 1) throw new ArgumentException("VAM_RequisitionLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_RequisitionLine_ID", VAM_RequisitionLine_ID);
        }
        /** Get Requisition Line.
        @return Material Requisition Line */
        public int GetVAM_RequisitionLine_ID()
        {
            Object ii = Get_Value("VAM_RequisitionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Requisition.
        @param VAM_Requisition_ID Material Requisition */
        public void SetVAM_Requisition_ID(int VAM_Requisition_ID)
        {
            if (VAM_Requisition_ID < 1) throw new ArgumentException("VAM_Requisition_ID is mandatory.");
            Set_ValueNoCheck("VAM_Requisition_ID", VAM_Requisition_ID);
        }
        /** Get Requisition.
        @return Material Requisition */
        public int GetVAM_Requisition_ID()
        {
            Object ii = Get_Value("VAM_Requisition_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** OrderLocator_ID VAF_Control_Ref_ID=191 */
        public static int ORDERLOCATOR_ID_VAF_Control_Ref_ID = 191;/** Set Order Locator.
@param OrderLocator_ID Warehouse Locator */
        public void SetOrderLocator_ID(int OrderLocator_ID)
        {
            if (OrderLocator_ID <= 0) Set_Value("OrderLocator_ID", null);
            else
                Set_Value("OrderLocator_ID", OrderLocator_ID);
        }
        /** Get Order Locator.
        @return Warehouse Locator */
        public int GetOrderLocator_ID()
        {
            Object ii = Get_Value("OrderLocator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unit Price.
        @param PriceActual Actual Price */
        public void SetPriceActual(Decimal? PriceActual)
        {
            if (PriceActual == null) throw new ArgumentException("PriceActual is mandatory.");
            Set_Value("PriceActual", (Decimal?)PriceActual);
        }
        /** Get Unit Price.
        @return Actual Price */
        public Decimal GetPriceActual()
        {
            Object bd = Get_Value("PriceActual");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** ReserveLocator_ID VAF_Control_Ref_ID=191 */
        public static int RESERVELOCATOR_ID_VAF_Control_Ref_ID = 191;
        /** Set Reserve Locator.
        @param ReserveLocator_ID Warehouse Locator */
        public void SetReserveLocator_ID(int ReserveLocator_ID)
        {
            if (ReserveLocator_ID <= 0) Set_Value("ReserveLocator_ID", null);
            else
                Set_Value("ReserveLocator_ID", ReserveLocator_ID);
        }
        /** Get Reserve Locator.
        @return Warehouse Locator */
        public int GetReserveLocator_ID()
        {
            Object ii = Get_Value("ReserveLocator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Processed.
            @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
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

    }

}
