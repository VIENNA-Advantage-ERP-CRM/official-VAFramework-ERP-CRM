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
    /** Generated Model for M_CostQueue
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_CostQueue : PO
    {
        public X_M_CostQueue(Context ctx, int M_CostQueue_ID, Trx trxName)
            : base(ctx, M_CostQueue_ID, trxName)
        {
            /** if (M_CostQueue_ID == 0)
            {
            SetC_AcctSchema_ID (0);
            SetCurrentCostPrice (0.0);
            SetCurrentQty (0.0);
            SetM_AttributeSetInstance_ID (0);
            SetM_CostElement_ID (0);
            SetM_CostQueue_ID (0);
            SetM_CostType_ID (0);
            SetM_Product_ID (0);
            }
             */
        }
        public X_M_CostQueue(Ctx ctx, int M_CostQueue_ID, Trx trxName)
            : base(ctx, M_CostQueue_ID, trxName)
        {
            /** if (M_CostQueue_ID == 0)
            {
            SetC_AcctSchema_ID (0);
            SetCurrentCostPrice (0.0);
            SetCurrentQty (0.0);
            SetM_AttributeSetInstance_ID (0);
            SetM_CostElement_ID (0);
            SetM_CostQueue_ID (0);
            SetM_CostType_ID (0);
            SetM_Product_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CostQueue(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CostQueue(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CostQueue(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_CostQueue()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514378973L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062184L;
        /** AD_Table_ID=817 */
        public static int Table_ID;
        // =817;

        /** TableName=M_CostQueue */
        public static String Table_Name = "M_CostQueue";

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
            StringBuilder sb = new StringBuilder("X_M_CostQueue[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Current Cost Price.
        @param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice)
        {
            if (CurrentCostPrice == null) throw new ArgumentException("CurrentCostPrice is mandatory.");
            Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice);
        }
        /** Get Current Cost Price.
        @return The currently used cost price */
        public Decimal GetCurrentCostPrice()
        {
            Object bd = Get_Value("CurrentCostPrice");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Current Quantity.
        @param CurrentQty Current Quantity */
        public void SetCurrentQty(Decimal? CurrentQty)
        {
            if (CurrentQty == null) throw new ArgumentException("CurrentQty is mandatory.");
            Set_Value("CurrentQty", (Decimal?)CurrentQty);
        }
        /** Get Current Quantity.
        @return Current Quantity */
        public Decimal GetCurrentQty()
        {
            Object bd = Get_Value("CurrentQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID < 0) throw new ArgumentException("M_AttributeSetInstance_ID is mandatory.");
            Set_ValueNoCheck("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Element.
        @param M_CostElement_ID Product Cost Element */
        public void SetM_CostElement_ID(int M_CostElement_ID)
        {
            if (M_CostElement_ID < 1) throw new ArgumentException("M_CostElement_ID is mandatory.");
            Set_ValueNoCheck("M_CostElement_ID", M_CostElement_ID);
        }
        /** Get Cost Element.
        @return Product Cost Element */
        public int GetM_CostElement_ID()
        {
            Object ii = Get_Value("M_CostElement_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Queue.
        @param M_CostQueue_ID FiFo/LiFo Cost Queue */
        public void SetM_CostQueue_ID(int M_CostQueue_ID)
        {
            if (M_CostQueue_ID < 1) throw new ArgumentException("M_CostQueue_ID is mandatory.");
            Set_ValueNoCheck("M_CostQueue_ID", M_CostQueue_ID);
        }
        /** Get Cost Queue.
        @return FiFo/LiFo Cost Queue */
        public int GetM_CostQueue_ID()
        {
            Object ii = Get_Value("M_CostQueue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Type.
        @param M_CostType_ID Type of Cost (e.g. Current, Plan, Future) */
        public void SetM_CostType_ID(int M_CostType_ID)
        {
            if (M_CostType_ID < 1) throw new ArgumentException("M_CostType_ID is mandatory.");
            Set_ValueNoCheck("M_CostType_ID", M_CostType_ID);
        }
        /** Get Cost Type.
        @return Type of Cost (e.g. Current, Plan, Future) */
        public int GetM_CostType_ID()
        {
            Object ii = Get_Value("M_CostType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
            Set_ValueNoCheck("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Queue Date.
        @param QueueDate Queue Date */
        public void SetQueueDate(DateTime? QueueDate)
        {
            Set_Value("QueueDate", (DateTime?)QueueDate);
        }
        /** Get Last Run.
        @return Last Run */
        public DateTime? GetQueueDate()
        {
            return (DateTime?)Get_Value("QueueDate");
        }


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

        /** Set Actual Quantity.@param ActualQty The actual quantity */
        public void SetActualQty(Decimal? ActualQty) { Set_Value("ActualQty", (Decimal?)ActualQty); }
        /** Get Actual Quantity.@return The actual quantity */
        public Decimal GetActualQty() { Object bd = Get_Value("ActualQty"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }

}
