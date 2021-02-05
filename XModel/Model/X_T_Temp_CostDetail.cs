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
    /** Generated Model for VAT_Temp_CostDetail
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAT_Temp_CostDetail : PO
    {
        public X_VAT_Temp_CostDetail(Context ctx, int VAT_Temp_CostDetail_ID, Trx trxName)
            : base(ctx, VAT_Temp_CostDetail_ID, trxName)
        {
            /** if (VAT_Temp_CostDetail_ID == 0)
            {
            SetVAT_Temp_CostDetail_ID (0);
            }
             */
        }
        public X_VAT_Temp_CostDetail(Ctx ctx, int VAT_Temp_CostDetail_ID, Trx trxName)
            : base(ctx, VAT_Temp_CostDetail_ID, trxName)
        {
            /** if (VAT_Temp_CostDetail_ID == 0)
            {
            SetVAT_Temp_CostDetail_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAT_Temp_CostDetail(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAT_Temp_CostDetail(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAT_Temp_CostDetail(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAT_Temp_CostDetail()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27732893351953L;
        /** Last Updated Timestamp 12/22/2015 12:37:15 PM */
        public static long updatedMS = 1450768035164L;
        /** VAF_TableView_ID=1000477 */
        public static int Table_ID;
        // =1000477;

        /** TableName=VAT_Temp_CostDetail */
        public static String Table_Name = "VAT_Temp_CostDetail";

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
            StringBuilder sb = new StringBuilder("X_VAT_Temp_CostDetail[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice Line.
        @param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_Value("VAB_InvoiceLine_ID", null);
            else
                Set_Value("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
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
        /** Set Amount.
        @param Amt The currently used cost price */
        public void SetAmt(Decimal? Amt)
        {
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
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_Value("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
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
        /** Set Cost Detail.
        @param VAM_ProductCostDetail_ID Cost Detail Information */
        public void SetVAM_ProductCostDetail_ID(int VAM_ProductCostDetail_ID)
        {
            if (VAM_ProductCostDetail_ID <= 0) Set_Value("VAM_ProductCostDetail_ID", null);
            else
                Set_Value("VAM_ProductCostDetail_ID", VAM_ProductCostDetail_ID);
        }
        /** Get Cost Detail.
        @return Cost Detail Information */
        public int GetVAM_ProductCostDetail_ID()
        {
            Object ii = Get_Value("VAM_ProductCostDetail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Queue.
        @param VAM_ProductCostQueue_ID FiFo/LiFo Cost Queue */
        public void SetVAM_ProductCostQueue_ID(int VAM_ProductCostQueue_ID)
        {
            if (VAM_ProductCostQueue_ID <= 0) Set_Value("VAM_ProductCostQueue_ID", null);
            else
                Set_Value("VAM_ProductCostQueue_ID", VAM_ProductCostQueue_ID);
        }
        /** Get Cost Queue.
        @return FiFo/LiFo Cost Queue */
        public int GetVAM_ProductCostQueue_ID()
        {
            Object ii = Get_Value("VAM_ProductCostQueue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_Value("VAM_Inv_InOutLine_ID", null);
            else
                Set_Value("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
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
        /** Set VAT_Temp_CostDetail_ID.
        @param VAT_Temp_CostDetail_ID VAT_Temp_CostDetail_ID */
        public void SetVAT_Temp_CostDetail_ID(int VAT_Temp_CostDetail_ID)
        {
            if (VAT_Temp_CostDetail_ID < 1) throw new ArgumentException("VAT_Temp_CostDetail_ID is mandatory.");
            Set_ValueNoCheck("VAT_Temp_CostDetail_ID", VAT_Temp_CostDetail_ID);
        }
        /** Get VAT_Temp_CostDetail_ID.
        @return VAT_Temp_CostDetail_ID */
        public int GetVAT_Temp_CostDetail_ID()
        {
            Object ii = Get_Value("VAT_Temp_CostDetail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Accounting Schema.
        @param VAB_AccountBook_ID Accounting Schema */
        public void SetVAB_AccountBook_ID(int VAB_AccountBook_ID)
        {
            if (VAB_AccountBook_ID <= 0) Set_Value("VAB_AccountBook_ID", null);
            else
                Set_Value("VAB_AccountBook_ID", VAB_AccountBook_ID);
        }
        /** Get Accounting Schema.
        @return Accounting Schema */
        public int GetVAB_AccountBook_ID()
        {
            Object ii = Get_Value("VAB_AccountBook_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
       @param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID <= 0) Set_Value("VAB_Currency_ID", null);
            else
                Set_Value("VAB_Currency_ID", VAB_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVAB_Currency_ID()
        {
            Object ii = Get_Value("VAB_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Record From Form.
@param isRecordFromForm Record From Form */
        public void SetisRecordFromForm(Boolean isRecordFromForm) { Set_Value("isRecordFromForm", isRecordFromForm); }/** Get Record From Form.
@return Record From Form */
        public Boolean IsRecordFromForm() { Object oo = Get_Value("isRecordFromForm"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

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
