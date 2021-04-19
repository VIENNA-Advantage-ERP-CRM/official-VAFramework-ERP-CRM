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
    /** Generated Model for VAM_ProductCostQueue
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductCostQueue : PO
    {
        public X_VAM_ProductCostQueue(Context ctx, int VAM_ProductCostQueue_ID, Trx trxName)
            : base(ctx, VAM_ProductCostQueue_ID, trxName)
        {
            /** if (VAM_ProductCostQueue_ID == 0)
            {
            SetVAB_AccountBook_ID (0);
            SetCurrentCostPrice (0.0);
            SetCurrentQty (0.0);
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductCostElement_ID (0);
            SetVAM_ProductCostQueue_ID (0);
            SetVAM_CostType_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        public X_VAM_ProductCostQueue(Ctx ctx, int VAM_ProductCostQueue_ID, Trx trxName)
            : base(ctx, VAM_ProductCostQueue_ID, trxName)
        {
            /** if (VAM_ProductCostQueue_ID == 0)
            {
            SetVAB_AccountBook_ID (0);
            SetCurrentCostPrice (0.0);
            SetCurrentQty (0.0);
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductCostElement_ID (0);
            SetVAM_ProductCostQueue_ID (0);
            SetVAM_CostType_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostQueue(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostQueue(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostQueue(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductCostQueue()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514378973L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062184L;
        /** VAF_TableView_ID=817 */
        public static int Table_ID;
        // =817;

        /** TableName=VAM_ProductCostQueue */
        public static String Table_Name = "VAM_ProductCostQueue";

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
            StringBuilder sb = new StringBuilder("X_VAM_ProductCostQueue[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Cost Element.
        @param VAM_ProductCostElement_ID Product Cost Element */
        public void SetVAM_ProductCostElement_ID(int VAM_ProductCostElement_ID)
        {
            if (VAM_ProductCostElement_ID < 1) throw new ArgumentException("VAM_ProductCostElement_ID is mandatory.");
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
        /** Set Cost Queue.
        @param VAM_ProductCostQueue_ID FiFo/LiFo Cost Queue */
        public void SetVAM_ProductCostQueue_ID(int VAM_ProductCostQueue_ID)
        {
            if (VAM_ProductCostQueue_ID < 1) throw new ArgumentException("VAM_ProductCostQueue_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductCostQueue_ID", VAM_ProductCostQueue_ID);
        }
        /** Get Cost Queue.
        @return FiFo/LiFo Cost Queue */
        public int GetVAM_ProductCostQueue_ID()
        {
            Object ii = Get_Value("VAM_ProductCostQueue_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Type.
        @param VAM_CostType_ID Type of Cost (e.g. Current, Plan, Future) */
        public void SetVAM_CostType_ID(int VAM_CostType_ID)
        {
            if (VAM_CostType_ID < 1) throw new ArgumentException("VAM_CostType_ID is mandatory.");
            Set_ValueNoCheck("VAM_CostType_ID", VAM_CostType_ID);
        }
        /** Get Cost Type.
        @return Type of Cost (e.g. Current, Plan, Future) */
        public int GetVAM_CostType_ID()
        {
            Object ii = Get_Value("VAM_CostType_ID");
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
