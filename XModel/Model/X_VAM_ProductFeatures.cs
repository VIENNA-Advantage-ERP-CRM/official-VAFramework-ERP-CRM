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
    /** Generated Model for VAM_ProductFeatures
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductFeatures : PO
    {
        public X_VAM_ProductFeatures(Context ctx, int VAM_ProductFeatures_ID, Trx trxName)
            : base(ctx, VAM_ProductFeatures_ID, trxName)
        {
            /** if (VAM_ProductFeatures_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductFeatures_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        public X_VAM_ProductFeatures(Ctx ctx, int VAM_ProductFeatures_ID, Trx trxName)
            : base(ctx, VAM_ProductFeatures_ID, trxName)
        {
            /** if (VAM_ProductFeatures_ID == 0)
            {
            SetVAM_PFeature_SetInstance_ID (0);
            SetVAM_ProductFeatures_ID (0);
            SetVAM_Product_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductFeatures(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductFeatures(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductFeatures(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductFeatures()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27723400322823L;
        /** Last Updated Timestamp 9/3/2015 3:40:06 PM */
        public static long updatedMS = 1441275006034L;
        /** VAF_TableView_ID=1000455 */
        public static int Table_ID;
        // =1000455;

        /** TableName=VAM_ProductFeatures */
        public static String Table_Name = "VAM_ProductFeatures";

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
            StringBuilder sb = new StringBuilder("X_VAM_ProductFeatures[").Append(Get_ID()).Append("]");
            return sb.ToString();
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
        /** Set VAM_ProductFeatures_ID.
        @param VAM_ProductFeatures_ID VAM_ProductFeatures_ID */
        public void SetVAM_ProductFeatures_ID(int VAM_ProductFeatures_ID)
        {
            if (VAM_ProductFeatures_ID < 1) throw new ArgumentException("VAM_ProductFeatures_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductFeatures_ID", VAM_ProductFeatures_ID);
        }
        /** Get VAM_ProductFeatures_ID.
        @return VAM_ProductFeatures_ID */
        public int GetVAM_ProductFeatures_ID()
        {
            Object ii = Get_Value("VAM_ProductFeatures_ID");
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
        /** Set Attribute Code.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            if (UPC != null && UPC.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                UPC = UPC.Substring(0, 100);
            }
            Set_Value("UPC", UPC);
        }
        /** Get Attribute Code.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }
        //Code added by Anuj 03/06/16
        public void SetVA007_RefProAtt_ID(String VA007_RefProAtt_ID)
        {
            if (VA007_RefProAtt_ID != null && VA007_RefProAtt_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA007_RefProAtt_ID = VA007_RefProAtt_ID.Substring(0, 50);
            }
            Set_Value("VA007_RefProAtt_ID", VA007_RefProAtt_ID);
        }/** Get Refernce Prod Attributes ID.
       @return Refernce Prod Attributes ID */
        public String GetVA007_RefProAtt_ID()
        {
            return (String)Get_Value("VA007_RefProAtt_ID");
        }
        //end
    }

}
