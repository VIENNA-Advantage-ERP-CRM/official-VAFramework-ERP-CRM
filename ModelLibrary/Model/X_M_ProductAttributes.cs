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
    /** Generated Model for M_ProductAttributes
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_ProductAttributes : PO
    {
        public X_M_ProductAttributes(Context ctx, int M_ProductAttributes_ID, Trx trxName)
            : base(ctx, M_ProductAttributes_ID, trxName)
        {
            /** if (M_ProductAttributes_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_ProductAttributes_ID (0);
            SetM_Product_ID (0);
            }
             */
        }
        public X_M_ProductAttributes(Ctx ctx, int M_ProductAttributes_ID, Trx trxName)
            : base(ctx, M_ProductAttributes_ID, trxName)
        {
            /** if (M_ProductAttributes_ID == 0)
            {
            SetM_AttributeSetInstance_ID (0);
            SetM_ProductAttributes_ID (0);
            SetM_Product_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductAttributes(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductAttributes(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_ProductAttributes(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_ProductAttributes()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27723400322823L;
        /** Last Updated Timestamp 9/3/2015 3:40:06 PM */
        public static long updatedMS = 1441275006034L;
        /** AD_Table_ID=1000455 */
        public static int Table_ID;
        // =1000455;

        /** TableName=M_ProductAttributes */
        public static String Table_Name = "M_ProductAttributes";

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
            StringBuilder sb = new StringBuilder("X_M_ProductAttributes[").Append(Get_ID()).Append("]");
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
        /** Set M_ProductAttributes_ID.
        @param M_ProductAttributes_ID M_ProductAttributes_ID */
        public void SetM_ProductAttributes_ID(int M_ProductAttributes_ID)
        {
            if (M_ProductAttributes_ID < 1) throw new ArgumentException("M_ProductAttributes_ID is mandatory.");
            Set_ValueNoCheck("M_ProductAttributes_ID", M_ProductAttributes_ID);
        }
        /** Get M_ProductAttributes_ID.
        @return M_ProductAttributes_ID */
        public int GetM_ProductAttributes_ID()
        {
            Object ii = Get_Value("M_ProductAttributes_ID");
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
