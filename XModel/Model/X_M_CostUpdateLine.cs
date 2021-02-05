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
    /** Generated Model for VAM_ProductCostUpdateLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_ProductCostUpdateLine : PO
    {
        public X_VAM_ProductCostUpdateLine(Context ctx, int VAM_ProductCostUpdateLine_ID, Trx trxName)
            : base(ctx, VAM_ProductCostUpdateLine_ID, trxName)
        {
            /** if (VAM_ProductCostUpdateLine_ID == 0)
            {
            SetVAB_UOM_ID (0);
            SetVAM_ProductCostUpdateLine_ID (0);
            SetVAM_ProductCostUpdate_ID (0);
            SetVAM_Product_ID (0);
            SetProcessed (false);
            }
             */
        }
        public X_VAM_ProductCostUpdateLine(Ctx ctx, int VAM_ProductCostUpdateLine_ID, Trx trxName)
            : base(ctx, VAM_ProductCostUpdateLine_ID, trxName)
        {
            /** if (VAM_ProductCostUpdateLine_ID == 0)
            {
            SetVAB_UOM_ID (0);
            SetVAM_ProductCostUpdateLine_ID (0);
            SetVAM_ProductCostUpdate_ID (0);
            SetVAM_Product_ID (0);
            SetProcessed (false);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostUpdateLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostUpdateLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_ProductCostUpdateLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_ProductCostUpdateLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27588604210335L;
        /** Last Updated Timestamp 5/27/2011 12:18:13 PM */
        public static long updatedMS = 1306478893546L;
        /** VAF_TableView_ID=1037 */
        public static int Table_ID;
        // =1037;

        /** TableName=VAM_ProductCostUpdateLine */
        public static String Table_Name = "VAM_ProductCostUpdateLine";

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
            StringBuilder sb = new StringBuilder("X_VAM_ProductCostUpdateLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** VAF_OrgTrx_ID VAF_Control_Ref_ID=130 */
        public static int VAF_ORGTRX_ID_VAF_Control_Ref_ID = 130;
        /** Set Trx Organization.
        @param VAF_OrgTrx_ID Performing or initiating organization */
        public void SetVAF_OrgTrx_ID(int VAF_OrgTrx_ID)
        {
            if (VAF_OrgTrx_ID <= 0) Set_Value("VAF_OrgTrx_ID", null);
            else
                Set_Value("VAF_OrgTrx_ID", VAF_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetVAF_OrgTrx_ID()
        {
            Object ii = Get_Value("VAF_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Activity.
        @param VAB_BillingCode_ID Business Activity */
        public void SetVAB_BillingCode_ID(int VAB_BillingCode_ID)
        {
            if (VAB_BillingCode_ID <= 0) Set_Value("VAB_BillingCode_ID", null);
            else
                Set_Value("VAB_BillingCode_ID", VAB_BillingCode_ID);
        }
        /** Get Activity.
        @return Business Activity */
        public int GetVAB_BillingCode_ID()
        {
            Object ii = Get_Value("VAB_BillingCode_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param VAB_Promotion_ID Marketing Campaign */
        public void SetVAB_Promotion_ID(int VAB_Promotion_ID)
        {
            if (VAB_Promotion_ID <= 0) Set_Value("VAB_Promotion_ID", null);
            else
                Set_Value("VAB_Promotion_ID", VAB_Promotion_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetVAB_Promotion_ID()
        {
            Object ii = Get_Value("VAB_Promotion_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Project Task.
        @param VAB_ProjectJob_ID Actual Project Task in a Phase */
        public void SetVAB_ProjectJob_ID(int VAB_ProjectJob_ID)
        {
            if (VAB_ProjectJob_ID <= 0) Set_Value("VAB_ProjectJob_ID", null);
            else
                Set_Value("VAB_ProjectJob_ID", VAB_ProjectJob_ID);
        }
        /** Get Project Task.
        @return Actual Project Task in a Phase */
        public int GetVAB_ProjectJob_ID()
        {
            Object ii = Get_Value("VAB_ProjectJob_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param VAB_Project_ID Financial Project */
        public void SetVAB_Project_ID(int VAB_Project_ID)
        {
            if (VAB_Project_ID <= 0) Set_Value("VAB_Project_ID", null);
            else
                Set_Value("VAB_Project_ID", VAB_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetVAB_Project_ID()
        {
            Object ii = Get_Value("VAB_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param VAB_UOM_ID Unit of Measure */
        public void SetVAB_UOM_ID(int VAB_UOM_ID)
        {
            if (VAB_UOM_ID < 1) throw new ArgumentException("VAB_UOM_ID is mandatory.");
            Set_Value("VAB_UOM_ID", VAB_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetVAB_UOM_ID()
        {
            Object ii = Get_Value("VAB_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Cost Update Line.
        @param VAM_ProductCostUpdateLine_ID Cost Update Line */
        public void SetVAM_ProductCostUpdateLine_ID(int VAM_ProductCostUpdateLine_ID)
        {
            if (VAM_ProductCostUpdateLine_ID < 1) throw new ArgumentException("VAM_ProductCostUpdateLine_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductCostUpdateLine_ID", VAM_ProductCostUpdateLine_ID);
        }
        /** Get Cost Update Line.
        @return Cost Update Line */
        public int GetVAM_ProductCostUpdateLine_ID()
        {
            Object ii = Get_Value("VAM_ProductCostUpdateLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Update.
        @param VAM_ProductCostUpdate_ID Cost Update */
        public void SetVAM_ProductCostUpdate_ID(int VAM_ProductCostUpdate_ID)
        {
            if (VAM_ProductCostUpdate_ID < 1) throw new ArgumentException("VAM_ProductCostUpdate_ID is mandatory.");
            Set_ValueNoCheck("VAM_ProductCostUpdate_ID", VAM_ProductCostUpdate_ID);
        }
        /** Get Cost Update.
        @return Cost Update */
        public int GetVAM_ProductCostUpdate_ID()
        {
            Object ii = Get_Value("VAM_ProductCostUpdate_ID");
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

        /** User1_ID VAF_Control_Ref_ID=134 */
        public static int USER1_ID_VAF_Control_Ref_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID VAF_Control_Ref_ID=137 */
        public static int USER2_ID_VAF_Control_Ref_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
