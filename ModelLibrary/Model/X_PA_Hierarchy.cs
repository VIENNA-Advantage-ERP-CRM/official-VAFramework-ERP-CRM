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
    /** Generated Model for PA_Hierarchy
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_PA_Hierarchy : PO
    {
        public X_PA_Hierarchy(Context ctx, int PA_Hierarchy_ID, Trx trxName)
            : base(ctx, PA_Hierarchy_ID, trxName)
        {
            /** if (PA_Hierarchy_ID == 0)
            {
            SetAD_Tree_Account_ID (0);
            SetAD_Tree_Activity_ID (0);
            SetAD_Tree_BPartner_ID (0);
            SetAD_Tree_Campaign_ID (0);
            SetAD_Tree_Org_ID (0);
            SetAD_Tree_Product_ID (0);
            SetAD_Tree_Project_ID (0);
            SetAD_Tree_SalesRegion_ID (0);
            SetName (null);
            SetPA_Hierarchy_ID (0);
            }
             */
        }
        public X_PA_Hierarchy(Ctx ctx, int PA_Hierarchy_ID, Trx trxName)
            : base(ctx, PA_Hierarchy_ID, trxName)
        {
            /** if (PA_Hierarchy_ID == 0)
            {
            SetAD_Tree_Account_ID (0);
            SetAD_Tree_Activity_ID (0);
            SetAD_Tree_BPartner_ID (0);
            SetAD_Tree_Campaign_ID (0);
            SetAD_Tree_Org_ID (0);
            SetAD_Tree_Product_ID (0);
            SetAD_Tree_Project_ID (0);
            SetAD_Tree_SalesRegion_ID (0);
            SetName (null);
            SetPA_Hierarchy_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Hierarchy(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Hierarchy(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_PA_Hierarchy(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_PA_Hierarchy()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27719528950122L;
        /** Last Updated Timestamp 7/20/2015 8:17:13 PM */
        public static long updatedMS = 1437403633333L;
        /** AD_Table_ID=821 */
        public static int Table_ID;
        // =821;

        /** TableName=PA_Hierarchy */
        public static String Table_Name = "PA_Hierarchy";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
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
            StringBuilder sb = new StringBuilder("X_PA_Hierarchy[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_Tree_Account_ID AD_Reference_ID=184 */
        public static int AD_TREE_ACCOUNT_ID_AD_Reference_ID = 184;
        /** Set Account Tree.
        @param AD_Tree_Account_ID Tree for Natural Account Tree */
        public void SetAD_Tree_Account_ID(int AD_Tree_Account_ID)
        {
            if (AD_Tree_Account_ID < 1) throw new ArgumentException("AD_Tree_Account_ID is mandatory.");
            Set_Value("AD_Tree_Account_ID", AD_Tree_Account_ID);
        }
        /** Get Account Tree.
        @return Tree for Natural Account Tree */
        public int GetAD_Tree_Account_ID()
        {
            Object ii = Get_Value("AD_Tree_Account_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Activity_ID AD_Reference_ID=184 */
        public static int AD_TREE_ACTIVITY_ID_AD_Reference_ID = 184;
        /** Set Activity Tree.
        @param AD_Tree_Activity_ID Tree to determine activity hierarchy */
        public void SetAD_Tree_Activity_ID(int AD_Tree_Activity_ID)
        {
            if (AD_Tree_Activity_ID < 1) throw new ArgumentException("AD_Tree_Activity_ID is mandatory.");
            Set_Value("AD_Tree_Activity_ID", AD_Tree_Activity_ID);
        }
        /** Get Activity Tree.
        @return Tree to determine activity hierarchy */
        public int GetAD_Tree_Activity_ID()
        {
            Object ii = Get_Value("AD_Tree_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_BPartner_ID AD_Reference_ID=184 */
        public static int AD_TREE_BPARTNER_ID_AD_Reference_ID = 184;
        /** Set BPartner Tree.
        @param AD_Tree_BPartner_ID Tree to determine business partner hierarchy */
        public void SetAD_Tree_BPartner_ID(int AD_Tree_BPartner_ID)
        {
            if (AD_Tree_BPartner_ID < 1) throw new ArgumentException("AD_Tree_BPartner_ID is mandatory.");
            Set_Value("AD_Tree_BPartner_ID", AD_Tree_BPartner_ID);
        }
        /** Get BPartner Tree.
        @return Tree to determine business partner hierarchy */
        public int GetAD_Tree_BPartner_ID()
        {
            Object ii = Get_Value("AD_Tree_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Campaign_ID AD_Reference_ID=184 */
        public static int AD_TREE_CAMPAIGN_ID_AD_Reference_ID = 184;
        /** Set Campaign Tree.
        @param AD_Tree_Campaign_ID Tree to determine marketing campaign hierarchy */
        public void SetAD_Tree_Campaign_ID(int AD_Tree_Campaign_ID)
        {
            if (AD_Tree_Campaign_ID < 1) throw new ArgumentException("AD_Tree_Campaign_ID is mandatory.");
            Set_Value("AD_Tree_Campaign_ID", AD_Tree_Campaign_ID);
        }
        /** Get Campaign Tree.
        @return Tree to determine marketing campaign hierarchy */
        public int GetAD_Tree_Campaign_ID()
        {
            Object ii = Get_Value("AD_Tree_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Org_ID AD_Reference_ID=184 */
        public static int AD_TREE_ORG_ID_AD_Reference_ID = 184;
        /** Set Organization Tree.
        @param AD_Tree_Org_ID Tree to determine organizational hierarchy */
        public void SetAD_Tree_Org_ID(int AD_Tree_Org_ID)
        {
            if (AD_Tree_Org_ID < 1) throw new ArgumentException("AD_Tree_Org_ID is mandatory.");
            Set_Value("AD_Tree_Org_ID", AD_Tree_Org_ID);
        }
        /** Get Organization Tree.
        @return Tree to determine organizational hierarchy */
        public int GetAD_Tree_Org_ID()
        {
            Object ii = Get_Value("AD_Tree_Org_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Product_ID AD_Reference_ID=184 */
        public static int AD_TREE_PRODUCT_ID_AD_Reference_ID = 184;
        /** Set Product Tree.
        @param AD_Tree_Product_ID Tree to determine product hierarchy */
        public void SetAD_Tree_Product_ID(int AD_Tree_Product_ID)
        {
            if (AD_Tree_Product_ID < 1) throw new ArgumentException("AD_Tree_Product_ID is mandatory.");
            Set_Value("AD_Tree_Product_ID", AD_Tree_Product_ID);
        }
        /** Get Product Tree.
        @return Tree to determine product hierarchy */
        public int GetAD_Tree_Product_ID()
        {
            Object ii = Get_Value("AD_Tree_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_Project_ID AD_Reference_ID=184 */
        public static int AD_TREE_PROJECT_ID_AD_Reference_ID = 184;
        /** Set Project Tree.
        @param AD_Tree_Project_ID Tree to determine project hierarchy */
        public void SetAD_Tree_Project_ID(int AD_Tree_Project_ID)
        {
            if (AD_Tree_Project_ID < 1) throw new ArgumentException("AD_Tree_Project_ID is mandatory.");
            Set_Value("AD_Tree_Project_ID", AD_Tree_Project_ID);
        }
        /** Get Project Tree.
        @return Tree to determine project hierarchy */
        public int GetAD_Tree_Project_ID()
        {
            Object ii = Get_Value("AD_Tree_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_Tree_SalesRegion_ID AD_Reference_ID=184 */
        public static int AD_TREE_SALESREGION_ID_AD_Reference_ID = 184;
        /** Set Sales Region Tree.
        @param AD_Tree_SalesRegion_ID Tree to determine sales regional hierarchy */
        public void SetAD_Tree_SalesRegion_ID(int AD_Tree_SalesRegion_ID)
        {
            if (AD_Tree_SalesRegion_ID < 1) throw new ArgumentException("AD_Tree_SalesRegion_ID is mandatory.");
            Set_Value("AD_Tree_SalesRegion_ID", AD_Tree_SalesRegion_ID);
        }
        /** Get Sales Region Tree.
        @return Tree to determine sales regional hierarchy */
        public int GetAD_Tree_SalesRegion_ID()
        {
            Object ii = Get_Value("AD_Tree_SalesRegion_ID");
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
        /** Set Export.
        @param Export_ID Export */
        public void SetExport_ID(String Export_ID)
        {
            if (Export_ID != null && Export_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Export_ID = Export_ID.Substring(0, 50);
            }
            Set_ValueNoCheck("Export_ID", Export_ID);
        }
        /** Get Export.
        @return Export */
        public String GetExport_ID()
        {
            return (String)Get_Value("Export_ID");
        }
        /** Set Comment.
        @param Help Comment, Help or Hint */
        public void SetHelp(String Help)
        {
            if (Help != null && Help.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Help = Help.Substring(0, 2000);
            }
            Set_Value("Help", Help);
        }
        /** Get Comment.
        @return Comment, Help or Hint */
        public String GetHelp()
        {
            return (String)Get_Value("Help");
        }
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name == null) throw new ArgumentException("Name is mandatory.");
            if (Name.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name = Name.Substring(0, 60);
            }
            Set_Value("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetName());
        }
        /** Set Reporting Hierarchy.
        @param PA_Hierarchy_ID Optional Reporting Hierarchy - If not selected the default hierarchy trees are used. */
        public void SetPA_Hierarchy_ID(int PA_Hierarchy_ID)
        {
            if (PA_Hierarchy_ID < 1) throw new ArgumentException("PA_Hierarchy_ID is mandatory.");
            Set_ValueNoCheck("PA_Hierarchy_ID", PA_Hierarchy_ID);
        }
        /** Get Reporting Hierarchy.
        @return Optional Reporting Hierarchy - If not selected the default hierarchy trees are used. */
        public int GetPA_Hierarchy_ID()
        {
            Object ii = Get_Value("PA_Hierarchy_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Ref Tree Org ID.
        @param Ref_Tree_Org_ID Ref Tree Org ID */
        public void SetRef_Tree_Org_ID(int Ref_Tree_Org_ID)
        {
            if (Ref_Tree_Org_ID <= 0) Set_Value("Ref_Tree_Org_ID", null);
            else
                Set_Value("Ref_Tree_Org_ID", Ref_Tree_Org_ID);
        }
        /** Get Ref Tree Org ID.
        @return Ref Tree Org ID */
        public int GetRef_Tree_Org_ID()
        {
            Object ii = Get_Value("Ref_Tree_Org_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }

}
