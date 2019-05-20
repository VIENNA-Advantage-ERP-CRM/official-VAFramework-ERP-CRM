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
    /** Generated Model for A_Asset_Group
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_A_Asset_Group : PO
    {
        public X_A_Asset_Group(Context ctx, int A_Asset_Group_ID, Trx trxName)
            : base(ctx, A_Asset_Group_ID, trxName)
        {
            /** if (A_Asset_Group_ID == 0)
            {
            SetA_Asset_Group_ID (0);
            SetIsCreateAsActive (true);	// Y
            SetIsDepreciated (false);
            SetIsOneAssetPerUOM (false);
            SetIsOwned (false);
            SetIsTrackIssues (false);	// N
            SetName (null);
            }
             */
        }
        public X_A_Asset_Group(Ctx ctx, int A_Asset_Group_ID, Trx trxName)
            : base(ctx, A_Asset_Group_ID, trxName)
        {
            /** if (A_Asset_Group_ID == 0)
            {
            SetA_Asset_Group_ID (0);
            SetIsCreateAsActive (true);	// Y
            SetIsDepreciated (false);
            SetIsOneAssetPerUOM (false);
            SetIsOwned (false);
            SetIsTrackIssues (false);	// N
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset_Group(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset_Group(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_A_Asset_Group(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_A_Asset_Group()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514367047L;
        /** Last Updated Timestamp 7/29/2010 1:07:30 PM */
        public static long updatedMS = 1280389050258L;
        /** AD_Table_ID=542 */
        public static int Table_ID;
        // =542;

        /** TableName=A_Asset_Group */
        public static String Table_Name = "A_Asset_Group";

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
            StringBuilder sb = new StringBuilder("X_A_Asset_Group[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Asset Group.
        @param A_Asset_Group_ID Group of Assets */
        public void SetA_Asset_Group_ID(int A_Asset_Group_ID)
        {
            if (A_Asset_Group_ID < 1) throw new ArgumentException("A_Asset_Group_ID is mandatory.");
            Set_ValueNoCheck("A_Asset_Group_ID", A_Asset_Group_ID);
        }
        /** Get Asset Group.
        @return Group of Assets */
        public int GetA_Asset_Group_ID()
        {
            Object ii = Get_Value("A_Asset_Group_ID");
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
        /** Set Create As Active.
        @param IsCreateAsActive Create Asset and activate it */
        public void SetIsCreateAsActive(Boolean IsCreateAsActive)
        {
            Set_Value("IsCreateAsActive", IsCreateAsActive);
        }
        /** Get Create As Active.
        @return Create Asset and activate it */
        public Boolean IsCreateAsActive()
        {
            Object oo = Get_Value("IsCreateAsActive");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Depreciate.
        @param IsDepreciated The asset will be depreciated */
        public void SetIsDepreciated(Boolean IsDepreciated)
        {
            Set_Value("IsDepreciated", IsDepreciated);
        }
        /** Get Depreciate.
        @return The asset will be depreciated */
        public Boolean IsDepreciated()
        {
            Object oo = Get_Value("IsDepreciated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set One Asset Per UOM.
        @param IsOneAssetPerUOM Create one asset per UOM */
        public void SetIsOneAssetPerUOM(Boolean IsOneAssetPerUOM)
        {
            Set_Value("IsOneAssetPerUOM", IsOneAssetPerUOM);
        }
        /** Get One Asset Per UOM.
        @return Create one asset per UOM */
        public Boolean IsOneAssetPerUOM()
        {
            Object oo = Get_Value("IsOneAssetPerUOM");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Owned.
        @param IsOwned The asset is owned by the organization */
        public void SetIsOwned(Boolean IsOwned)
        {
            Set_Value("IsOwned", IsOwned);
        }
        /** Get Owned.
        @return The asset is owned by the organization */
        public Boolean IsOwned()
        {
            Object oo = Get_Value("IsOwned");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Track Issues.
        @param IsTrackIssues Enable tracking issues for this asset */
        public void SetIsTrackIssues(Boolean IsTrackIssues)
        {
            Set_Value("IsTrackIssues", IsTrackIssues);
        }
        /** Get Track Issues.
        @return Enable tracking issues for this asset */
        public Boolean IsTrackIssues()
        {
            Object oo = Get_Value("IsTrackIssues");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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

        /** SupportLevel AD_Reference_ID=412 */
        public static int SUPPORTLEVEL_AD_Reference_ID = 412;
        /** Enterprise = E */
        public static String SUPPORTLEVEL_Enterprise = "E";
        /** Standard = S */
        public static String SUPPORTLEVEL_Standard = "S";
        /** Unsupported = U */
        public static String SUPPORTLEVEL_Unsupported = "U";
        /** Self-Service = X */
        public static String SUPPORTLEVEL_Self_Service = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSupportLevelValid(String test)
        {
            return test == null || test.Equals("E") || test.Equals("S") || test.Equals("U") || test.Equals("X");
        }
        /** Set Support Level.
        @param SupportLevel Subscribed Support level */
        public void SetSupportLevel(String SupportLevel)
        {
            if (!IsSupportLevelValid(SupportLevel))
                throw new ArgumentException("SupportLevel Invalid value - " + SupportLevel + " - Reference_ID=412 - E - S - U - X");
            if (SupportLevel != null && SupportLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SupportLevel = SupportLevel.Substring(0, 1);
            }
            Set_Value("SupportLevel", SupportLevel);
        }
        /** Get Support Level.
        @return Subscribed Support level */
        public String GetSupportLevel()
        {
            return (String)Get_Value("SupportLevel");
        }
        /** Set Serial No Control.
        @param M_SerNoCtl_ID Product Serial Number Control */
        public void SetM_SerNoCtl_ID(int M_SerNoCtl_ID)
        {
            if (M_SerNoCtl_ID <= 0) Set_Value("M_SerNoCtl_ID", null);
            else
                Set_Value("M_SerNoCtl_ID", M_SerNoCtl_ID);
        }/** Get Serial No Control.
        @return Product Serial Number Control */
        public int GetM_SerNoCtl_ID()
        { Object ii = Get_Value("M_SerNoCtl_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
