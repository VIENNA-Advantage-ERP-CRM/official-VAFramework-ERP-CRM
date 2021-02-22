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
    /** Generated Model for VAM_ProductLocator
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductLocator : PO
    {
        public X_VAM_ProductLocator(Context ctx, int VAM_ProductLocator_ID, Trx trxName)
            : base(ctx, VAM_ProductLocator_ID, trxName)
        {
            /** if (VAM_ProductLocator_ID == 0)
            {
            SetVAM_Locator_ID (0);
            SetVAM_ProductLocator_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        public X_VAM_ProductLocator(Ctx ctx, int VAM_ProductLocator_ID, Trx trxName)
            : base(ctx, VAM_ProductLocator_ID, trxName)
        {
            /** if (VAM_ProductLocator_ID == 0)
            {
            SetVAM_Locator_ID (0);
            SetVAM_ProductLocator_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductLocator(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductLocator(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductLocator(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductLocator()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514380697L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063908L;
        /** VAF_TableView_ID=915 */
        public static int Table_ID;
        // =915;

        /** TableName=VAM_ProductLocator */
        public static String Table_Name = "VAM_ProductLocator";

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
            StringBuilder sb = new StringBuilder("X_VAM_ProductLocator[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set Product Locator.
        @param VAM_ProductLocator_ID The preferred Locators for a Product */
        public void SetVAM_ProductLocator_ID(int VAM_ProductLocator_ID)
        {
            if (VAM_ProductLocator_ID < 1) throw new ArgumentException("VAM_ProductLocator_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductLocator_ID", VAM_ProductLocator_ID);
        }
        /** Get Product Locator.
        @return The preferred Locators for a Product */
        public int GetVAM_ProductLocator_ID()
        {
            Object ii = Get_Value("VAM_ProductLocator_ID");
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
        /** Set Relative Priority.
        @param PriorityNo Where inventory should be picked from first */
        public void SetPriorityNo(int PriorityNo)
        {
            throw new ArgumentException("PriorityNo Is virtual column");
        }
        /** Get Relative Priority.
        @return Where inventory should be picked from first */
        public int GetPriorityNo()
        {
            Object ii = Get_Value("PriorityNo");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Volume Limit.
        @param VolumeLimit Volume Limit of a Locator */
        public void SetVolumeLimit(Decimal? VolumeLimit)
        {
            Set_Value("VolumeLimit", (Decimal?)VolumeLimit);
        }
        /** Get Volume Limit.
        @return Volume Limit of a Locator */
        public Decimal GetVolumeLimit()
        {
            Object bd = Get_Value("VolumeLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Weight Limit.
        @param WeightLimit Weight Limit of a Locator */
        public void SetWeightLimit(Decimal? WeightLimit)
        {
            Set_Value("WeightLimit", (Decimal?)WeightLimit);
        }
        /** Get Weight Limit.
        @return Weight Limit of a Locator */
        public Decimal GetWeightLimit()
        {
            Object bd = Get_Value("WeightLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        // Mohit 20-8-2015 VAWMS

        /** Set Max Stocking Quantity.
        @param MaxQuantity Maximum stocking capacity of the locator in units */
        public void SetMaxQuantity(Decimal? MaxQuantity)
        {
            Set_Value("MaxQuantity", (Decimal?)MaxQuantity);
        }
        /** Get Max Stocking Quantity.
        @return Maximum stocking capacity of the locator in units */
        public Decimal GetMaxQuantity()
        {
            Object bd = Get_Value("MaxQuantity");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        //END
    }

}
