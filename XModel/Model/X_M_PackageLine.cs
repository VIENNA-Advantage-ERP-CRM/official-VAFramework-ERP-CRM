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
    /** Generated Model for VAM_PackagingLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_PackagingLine : PO
    {
        public X_VAM_PackagingLine(Context ctx, int VAM_PackagingLine_ID, Trx trxName)
            : base(ctx, VAM_PackagingLine_ID, trxName)
        {
            /** if (VAM_PackagingLine_ID == 0)
            {
            SetVAM_Inv_InOutLine_ID (0);
            SetVAM_PackagingLine_ID (0);
            SetVAM_Packaging_ID (0);
            SetQty (0.0);
            }
             */
        }
        public X_VAM_PackagingLine(Ctx ctx, int VAM_PackagingLine_ID, Trx trxName)
            : base(ctx, VAM_PackagingLine_ID, trxName)
        {
            /** if (VAM_PackagingLine_ID == 0)
            {
            SetVAM_Inv_InOutLine_ID (0);
            SetVAM_PackagingLine_ID (0);
            SetVAM_Packaging_ID (0);
            SetQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PackagingLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PackagingLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PackagingLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_PackagingLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27562514380321L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063532L;
        /** VAF_TableView_ID=663 */
        public static int Table_ID;
        // =663;

        /** TableName=VAM_PackagingLine */
        public static String Table_Name = "VAM_PackagingLine";

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
            StringBuilder sb = new StringBuilder("X_VAM_PackagingLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Already Pack Qty.
        @param DTD001_AlreadyPackQty Already Pack Qty */
        public void SetDTD001_AlreadyPackQty(Decimal? DTD001_AlreadyPackQty)
        {
            Set_Value("DTD001_AlreadyPackQty", (Decimal?)DTD001_AlreadyPackQty);
        }
        /** Get Already Pack Qty.
        @return Already Pack Qty */
        public Decimal GetDTD001_AlreadyPackQty()
        {
            Object bd = Get_Value("DTD001_AlreadyPackQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Box No.
        @param DTD001_BoxNo Box No */
        public void SetDTD001_BoxNo(String DTD001_BoxNo)
        {
            if (DTD001_BoxNo != null && DTD001_BoxNo.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                DTD001_BoxNo = DTD001_BoxNo.Substring(0, 100);
            }
            Set_Value("DTD001_BoxNo", DTD001_BoxNo);
        }
        /** Get Box No.
        @return Box No */
        public String GetDTD001_BoxNo()
        {
            return (String)Get_Value("DTD001_BoxNo");
        }
        /** Set Confirm Date.
        @param DTD001_ConfirmDate Confirm Date */
        public void SetDTD001_ConfirmDate(DateTime? DTD001_ConfirmDate)
        {
            Set_Value("DTD001_ConfirmDate", (DateTime?)DTD001_ConfirmDate);
        }
        /** Get Confirm Date.
        @return Confirm Date */
        public DateTime? GetDTD001_ConfirmDate()
        {
            return (DateTime?)Get_Value("DTD001_ConfirmDate");
        }
        /** Set Pallete Number.
        @param DTD001_PalleteNo Pallete Number */
        public void SetDTD001_PalleteNo(String DTD001_PalleteNo)
        {
            if (DTD001_PalleteNo != null && DTD001_PalleteNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                DTD001_PalleteNo = DTD001_PalleteNo.Substring(0, 50);
            }
            Set_Value("DTD001_PalleteNo", DTD001_PalleteNo);
        }
        /** Get Pallete Number.
        @return Pallete Number */
        public String GetDTD001_PalleteNo()
        {
            return (String)Get_Value("DTD001_PalleteNo");
        }
        /** Set Quantity Entered.
        @param DTD001_QtyEntered Quantity Entered */
        public void SetDTD001_QtyEntered(Decimal? DTD001_QtyEntered)
        {
            Set_Value("DTD001_QtyEntered", (Decimal?)DTD001_QtyEntered);
        }
        /** Get Quantity Entered.
        @return Quantity Entered */
        public Decimal GetDTD001_QtyEntered()
        {
            Object bd = Get_Value("DTD001_QtyEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Reference No.
        @param DTD001_ReferenceNo Reference No */
        public void SetDTD001_ReferenceNo(String DTD001_ReferenceNo)
        {
            if (DTD001_ReferenceNo != null && DTD001_ReferenceNo.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                DTD001_ReferenceNo = DTD001_ReferenceNo.Substring(0, 50);
            }
            Set_Value("DTD001_ReferenceNo", DTD001_ReferenceNo);
        }
        /** Get Reference No.
        @return Reference No */
        public String GetDTD001_ReferenceNo()
        {
            return (String)Get_Value("DTD001_ReferenceNo");
        }
        /** Set Total Quatity.
        @param DTD001_TotalQty Total Quatity */
        public void SetDTD001_TotalQty(Decimal? DTD001_TotalQty)
        {
            Set_Value("DTD001_TotalQty", (Decimal?)DTD001_TotalQty);
        }
        /** Get Total Quatity.
        @return Total Quatity */
        public Decimal GetDTD001_TotalQty()
        {
            Object bd = Get_Value("DTD001_TotalQty");
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
        /** Set Difference.
        @param DifferenceQty Difference Quantity */
        public void SetDifferenceQty(Decimal? DifferenceQty)
        {
            Set_Value("DifferenceQty", (Decimal?)DifferenceQty);
        }
        /** Get Difference.
        @return Difference Quantity */
        public Decimal GetDifferenceQty()
        {
            Object bd = Get_Value("DifferenceQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID < 1) throw new ArgumentException("VAM_Inv_InOutLine_ID is mandatory.");
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
        /** Set Package Line.
        @param VAM_PackagingLine_ID The detail content of the Package */
        public void SetVAM_PackagingLine_ID(int VAM_PackagingLine_ID)
        {
            if (VAM_PackagingLine_ID < 1) throw new ArgumentException("VAM_PackagingLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_PackagingLine_ID", VAM_PackagingLine_ID);
        }
        /** Get Package Line.
        @return The detail content of the Package */
        public int GetVAM_PackagingLine_ID()
        {
            Object ii = Get_Value("VAM_PackagingLine_ID");
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
        /** Set Package.
        @param VAM_Packaging_ID Shipment Package */
        public void SetVAM_Packaging_ID(int VAM_Packaging_ID)
        {
            if (VAM_Packaging_ID < 1) throw new ArgumentException("VAM_Packaging_ID is mandatory.");
            Set_ValueNoCheck("VAM_Packaging_ID", VAM_Packaging_ID);
        }
        /** Get Package.
        @return Shipment Package */
        public int GetVAM_Packaging_ID()
        {
            Object ii = Get_Value("VAM_Packaging_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetVAM_Packaging_ID().ToString());
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
        /** Set Confirm.
        @param DTD001_IsConfirm Confirm */
        public void SetDTD001_IsConfirm(Boolean DTD001_IsConfirm)
        {
            Set_Value("DTD001_IsConfirm", DTD001_IsConfirm);
        }
        /** Get Confirm.
        @return Confirm */
        public Boolean IsDTD001_IsConfirm()
        {
            Object oo = Get_Value("DTD001_IsConfirm");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

    }

}
