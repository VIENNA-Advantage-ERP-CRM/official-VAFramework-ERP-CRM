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
    /** Generated Model for VAM_BOM
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_BOM : PO
    {
        public X_VAM_BOM(Context ctx, int VAM_BOM_ID, Trx trxName)
            : base(ctx, VAM_BOM_ID, trxName)
        {
            /** if (VAM_BOM_ID == 0)
            {
            SetBOMType (null);	// A
            SetBOMUse (null);	// A
            SetVAM_BOM_ID (0);
            SetVAM_Product_ID (0);
            SetName (null);
            }
             */
        }
        public X_VAM_BOM(Ctx ctx, int VAM_BOM_ID, Trx trxName)
            : base(ctx, VAM_BOM_ID, trxName)
        {
            /** if (VAM_BOM_ID == 0)
            {
            SetBOMType (null);	// A
            SetBOMUse (null);	// A
            SetVAM_BOM_ID (0);
            SetVAM_Product_ID (0);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_BOM(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_BOM(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_BOM(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_BOM()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514378441L;
        /** Last Updated Timestamp 7/29/2010 1:07:41 PM */
        public static long updatedMS = 1280389061652L;
        /** VAF_TableView_ID=798 */
        public static int Table_ID;
        // =798;

        /** TableName=VAM_BOM */
        public static String Table_Name = "VAM_BOM";

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
            StringBuilder sb = new StringBuilder("X_VAM_BOM[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** BOMType VAF_Control_Ref_ID=347 */
        public static int BOMTYPE_VAF_Control_Ref_ID = 347;
        /** Current Active = A */
        public static String BOMTYPE_CurrentActive = "A";
        /** Future = F */
        public static String BOMTYPE_Future = "F";
        /** Maintenance = M */
        public static String BOMTYPE_Maintenance = "M";
        /** Make-To-Order = O */
        public static String BOMTYPE_Make_To_Order = "O";
        /** Previous = P */
        public static String BOMTYPE_Previous = "P";
        /** Repair = R */
        public static String BOMTYPE_Repair = "R";
        /** Previous, Spare = S */
        public static String BOMTYPE_PreviousSpare = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsBOMTypeValid(String test)
        {
            return test.Equals("A") || test.Equals("F") || test.Equals("M") || test.Equals("O") || test.Equals("P") || test.Equals("R") || test.Equals("S");
        }
        /** Set BOM Type.
        @param BOMType Type of BOM */
        public void SetBOMType(String BOMType)
        {
            if (BOMType == null) throw new ArgumentException("BOMType is mandatory");
            if (!IsBOMTypeValid(BOMType))
                throw new ArgumentException("BOMType Invalid value - " + BOMType + " - Reference_ID=347 - A - F - M - O - P - R - S");
            if (BOMType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                BOMType = BOMType.Substring(0, 1);
            }
            Set_Value("BOMType", BOMType);
        }
        /** Get BOM Type.
        @return Type of BOM */
        public String GetBOMType()
        {
            return (String)Get_Value("BOMType");
        }

        /** BOMUse VAF_Control_Ref_ID=348 */
        public static int BOMUSE_VAF_Control_Ref_ID = 348;
        /** Master = A */
        public static String BOMUSE_Master = "A";
        /** Engineering = E */
        public static String BOMUSE_Engineering = "E";
        /** Manufacturing = M */
        public static String BOMUSE_Manufacturing = "M";
        /** Planning = P */
        public static String BOMUSE_Planning = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsBOMUseValid(String test)
        {
            return test.Equals("A") || test.Equals("E") || test.Equals("M") || test.Equals("P");
        }
        /** Set BOM Use.
        @param BOMUse The use of the Bill of Material */
        public void SetBOMUse(String BOMUse)
        {
            if (BOMUse == null) throw new ArgumentException("BOMUse is mandatory");
            if (!IsBOMUseValid(BOMUse))
                throw new ArgumentException("BOMUse Invalid value - " + BOMUse + " - Reference_ID=348 - A - E - M - P");
            if (BOMUse.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                BOMUse = BOMUse.Substring(0, 1);
            }
            Set_Value("BOMUse", BOMUse);
        }
        /** Get BOM Use.
        @return The use of the Bill of Material */
        public String GetBOMUse()
        {
            return (String)Get_Value("BOMUse");
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
        /** Set BOM.
        @param VAM_BOM_ID Bill of Material */
        public void SetVAM_BOM_ID(int VAM_BOM_ID)
        {
            if (VAM_BOM_ID < 1) throw new ArgumentException("VAM_BOM_ID is mandatory.");
            Set_ValueNoCheck("VAM_BOM_ID", VAM_BOM_ID);
        }
        /** Get BOM.
        @return Bill of Material */
        public int GetVAM_BOM_ID()
        {
            Object ii = Get_Value("VAM_BOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Change Notice.
        @param M_ChangeNotice_ID Bill of Materials (Engineering) Change Notice (Version) */
        public void SetM_ChangeNotice_ID(int M_ChangeNotice_ID)
        {
            if (M_ChangeNotice_ID <= 0) Set_Value("M_ChangeNotice_ID", null);
            else
                Set_Value("M_ChangeNotice_ID", M_ChangeNotice_ID);
        }
        /** Get Change Notice.
        @return Bill of Materials (Engineering) Change Notice (Version) */
        public int GetM_ChangeNotice_ID()
        {
            Object ii = Get_Value("M_ChangeNotice_ID");
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
        /** Set Process Now.
        @param Processing Process Now */
        public void SetProcessing(Boolean Processing)
        {
            Set_Value("Processing", Processing);
        }
        /** Get Process Now.
        @return Process Now */ 
        public Boolean IsProcessing()
        {
            Object oo = Get_Value("Processing");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID <= 0) 
                Set_Value("VAM_PFeature_SetInstance_ID", null);
            else
                Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID() 
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) 
                return 0; 
            return Convert.ToInt32(ii); 
        }
    }

}
