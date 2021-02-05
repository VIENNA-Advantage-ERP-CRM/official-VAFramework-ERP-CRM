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
    /** Generated Model for M_CycleCountLock
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_CycleCountLock : PO
    {
        public X_M_CycleCountLock(Context ctx, int M_CycleCountLock_ID, Trx trxName)
            : base(ctx, M_CycleCountLock_ID, trxName)
        {
            /** if (M_CycleCountLock_ID == 0)
            {
            SetIsLocked (false);
            SetM_CycleCountLock_ID (0);
            SetVAM_Inventory_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_Warehouse_ID (0);
            }
             */
        }
        public X_M_CycleCountLock(Ctx ctx, int M_CycleCountLock_ID, Trx trxName)
            : base(ctx, M_CycleCountLock_ID, trxName)
        {
            /** if (M_CycleCountLock_ID == 0)
            {
            SetIsLocked (false);
            SetM_CycleCountLock_ID (0);
            SetVAM_Inventory_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_Warehouse_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CycleCountLock(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CycleCountLock(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_CycleCountLock(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_CycleCountLock()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27581088219483L;
        /** Last Updated Timestamp 3/1/2011 12:31:42 PM */
        public static long updatedMS = 1298962902694L;
        /** VAF_TableView_ID=2153 */
        public static int Table_ID;
        // =2153;

        /** TableName=M_CycleCountLock */
        public static String Table_Name = "M_CycleCountLock";

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
            StringBuilder sb = new StringBuilder("X_M_CycleCountLock[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Lock.
        @param IsLocked Lock */
        public void SetIsLocked(Boolean IsLocked)
        {
            Set_Value("IsLocked", IsLocked);
        }
        /** Get Lock.
        @return Lock */
        public Boolean IsLocked()
        {
            Object oo = Get_Value("IsLocked");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Cycle Count Lock.
        @param M_CycleCountLock_ID Cycle Count Lock */
        public void SetM_CycleCountLock_ID(int M_CycleCountLock_ID)
        {
            if (M_CycleCountLock_ID < 1) throw new ArgumentException("M_CycleCountLock_ID is mandatory.");
            Set_ValueNoCheck("M_CycleCountLock_ID", M_CycleCountLock_ID);
        }
        /** Get Cycle Count Lock.
        @return Cycle Count Lock */
        public int GetM_CycleCountLock_ID()
        {
            Object ii = Get_Value("M_CycleCountLock_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Phys.Inventory.
        @param VAM_Inventory_ID Parameters for a Physical Inventory */
        public void SetVAM_Inventory_ID(int VAM_Inventory_ID)
        {
            if (VAM_Inventory_ID < 1) throw new ArgumentException("VAM_Inventory_ID is mandatory.");
            Set_Value("VAM_Inventory_ID", VAM_Inventory_ID);
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
        /** Set Warehouse.
        @param VAM_Warehouse_ID Storage Warehouse and Service Point */
        public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID)
        {
            if (VAM_Warehouse_ID < 1) throw new ArgumentException("VAM_Warehouse_ID is mandatory.");
            Set_Value("VAM_Warehouse_ID", VAM_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetVAM_Warehouse_ID()
        {
            Object ii = Get_Value("VAM_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
