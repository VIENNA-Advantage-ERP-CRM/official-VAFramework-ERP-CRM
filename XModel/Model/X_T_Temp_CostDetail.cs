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
    /** Generated Model for T_Temp_CostDetail
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_T_Temp_CostDetail : PO
    {
        public X_T_Temp_CostDetail(Context ctx, int T_Temp_CostDetail_ID, Trx trxName)
            : base(ctx, T_Temp_CostDetail_ID, trxName)
        {
            /** if (T_Temp_CostDetail_ID == 0)
            {
            SetT_Temp_CostDetail_ID (0);
            }
             */
        }
        public X_T_Temp_CostDetail(Ctx ctx, int T_Temp_CostDetail_ID, Trx trxName)
            : base(ctx, T_Temp_CostDetail_ID, trxName)
        {
            /** if (T_Temp_CostDetail_ID == 0)
            {
            SetT_Temp_CostDetail_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_T_Temp_CostDetail(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_T_Temp_CostDetail(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_T_Temp_CostDetail(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_T_Temp_CostDetail()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27732893351953L;
        /** Last Updated Timestamp 12/22/2015 12:37:15 PM */
        public static long updatedMS = 1450768035164L;
        /** AD_Table_ID=1000477 */
        public static int Table_ID;
        // =1000477;

        /** TableName=T_Temp_CostDetail */
        public static String Table_Name = "T_Temp_CostDetail";

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
            StringBuilder sb = new StringBuilder("X_T_Temp_CostDetail[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Invoice Line.
        @param C_InvoiceLine_ID Invoice Detail Line */
        public void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            if (C_InvoiceLine_ID <= 0) Set_Value("C_InvoiceLine_ID", null);
            else
                Set_Value("C_InvoiceLine_ID", C_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetC_InvoiceLine_ID()
        {
            Object ii = Get_Value("C_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
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
        /** Set Cost Detail.
        @param M_CostDetail_ID Cost Detail Information */
        public void SetM_CostDetail_ID(int M_CostDetail_ID)
        {
            if (M_CostDetail_ID <= 0) Set_Value("M_CostDetail_ID", null);
            else
                Set_Value("M_CostDetail_ID", M_CostDetail_ID);
        }
        /** Get Cost Detail.
        @return Cost Detail Information */
        public int GetM_CostDetail_ID()
        {
            Object ii = Get_Value("M_CostDetail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Queue.
        @param M_CostQueue_ID FiFo/LiFo Cost Queue */
        public void SetM_CostQueue_ID(int M_CostQueue_ID)
        {
            if (M_CostQueue_ID <= 0) Set_Value("M_CostQueue_ID", null);
            else
                Set_Value("M_CostQueue_ID", M_CostQueue_ID);
        }
        /** Get Cost Queue.
        @return FiFo/LiFo Cost Queue */
        public int GetM_CostQueue_ID()
        {
            Object ii = Get_Value("M_CostQueue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID <= 0) Set_Value("M_InOutLine_ID", null);
            else
                Set_Value("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys Inventory Line.
        @param M_InventoryLine_ID Unique line in an Inventory document */
        public void SetM_InventoryLine_ID(int M_InventoryLine_ID)
        {
            if (M_InventoryLine_ID <= 0) Set_Value("M_InventoryLine_ID", null);
            else
                Set_Value("M_InventoryLine_ID", M_InventoryLine_ID);
        }
        /** Get Phys Inventory Line.
        @return Unique line in an Inventory document */
        public int GetM_InventoryLine_ID()
        {
            Object ii = Get_Value("M_InventoryLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Move Line.
        @param M_MovementLine_ID Inventory Move document Line */
        public void SetM_MovementLine_ID(int M_MovementLine_ID)
        {
            if (M_MovementLine_ID <= 0) Set_Value("M_MovementLine_ID", null);
            else
                Set_Value("M_MovementLine_ID", M_MovementLine_ID);
        }
        /** Get Move Line.
        @return Inventory Move document Line */
        public int GetM_MovementLine_ID()
        {
            Object ii = Get_Value("M_MovementLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
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
        /** Set T_Temp_CostDetail_ID.
        @param T_Temp_CostDetail_ID T_Temp_CostDetail_ID */
        public void SetT_Temp_CostDetail_ID(int T_Temp_CostDetail_ID)
        {
            if (T_Temp_CostDetail_ID < 1) throw new ArgumentException("T_Temp_CostDetail_ID is mandatory.");
            Set_ValueNoCheck("T_Temp_CostDetail_ID", T_Temp_CostDetail_ID);
        }
        /** Get T_Temp_CostDetail_ID.
        @return T_Temp_CostDetail_ID */
        public int GetT_Temp_CostDetail_ID()
        {
            Object ii = Get_Value("T_Temp_CostDetail_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Accounting Schema.
        @param C_AcctSchema_ID Accounting Schema */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID <= 0) Set_Value("C_AcctSchema_ID", null);
            else
                Set_Value("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Accounting Schema */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
       @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Record From Form.
@param isRecordFromForm Record From Form */
        public void SetisRecordFromForm(Boolean isRecordFromForm) { Set_Value("isRecordFromForm", isRecordFromForm); }/** Get Record From Form.
@return Record From Form */
        public Boolean IsRecordFromForm() { Object oo = Get_Value("isRecordFromForm"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

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
