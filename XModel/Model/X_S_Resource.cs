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
/** Generated Model for VAS_Resource
 *  @author Jagmohan Bhatt (generated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_S_Resource : PO
    {
        public X_S_Resource(Context ctx, int VAS_Resource_ID, Trx trxName)
            : base(ctx, VAS_Resource_ID, trxName)
        {
            /** if (VAS_Resource_ID == 0)
            {
            SetIsAvailable (true);	// Y
            SetVAM_Warehouse_ID (0);
            SetName (null);
            SetVAS_Res_Type_ID (0);
            SetS_Resource_ID (0);
            SetValue (null);
            }
             */
        }
        public X_S_Resource(Ctx ctx, int VAS_Resource_ID, Trx trxName)
            : base(ctx, VAS_Resource_ID, trxName)
        {
            /** if (VAS_Resource_ID == 0)
            {
            SetIsAvailable (true);	// Y
            SetVAM_Warehouse_ID (0);
            SetName (null);
            SetVAS_Res_Type_ID (0);
            SetS_Resource_ID (0);
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_Resource(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_Resource(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_Resource(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_S_Resource()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514383597L;
        /** Last Updated Timestamp 7/29/2010 1:07:46 PM */
        public static long updatedMS = 1280389066808L;
        /** VAF_TableView_ID=487 */
        public static int Table_ID;
        // =487;

        /** TableName=VAS_Resource */
        public static String Table_Name = "VAS_Resource";

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
            StringBuilder sb = new StringBuilder("X_S_Resource[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set User/Contact.
        @param VAF_UserContact_ID User within the system - Internal or Business Partner Contact */
        public void SetVAF_UserContact_ID(int VAF_UserContact_ID)
        {
            if (VAF_UserContact_ID <= 0) Set_Value("VAF_UserContact_ID", null);
            else
                Set_Value("VAF_UserContact_ID", VAF_UserContact_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetVAF_UserContact_ID()
        {
            Object ii = Get_Value("VAF_UserContact_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Chargeable Quantity.
        @param ChargeableQty Chargeable Quantity */
        public void SetChargeableQty(Decimal? ChargeableQty)
        {
            Set_Value("ChargeableQty", (Decimal?)ChargeableQty);
        }
        /** Get Chargeable Quantity.
        @return Chargeable Quantity */
        public Decimal GetChargeableQty()
        {
            Object bd = Get_Value("ChargeableQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Available.
        @param IsAvailable Resource is available */
        public void SetIsAvailable(Boolean IsAvailable)
        {
            Set_Value("IsAvailable", IsAvailable);
        }
        /** Get Available.
        @return Resource is available */
        public Boolean IsAvailable()
        {
            Object oo = Get_Value("IsAvailable");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Resource Type.
        @param VAS_Res_Type_ID Resource Type */
        public void SetVAS_Res_Type_ID(int VAS_Res_Type_ID)
        {
            if (VAS_Res_Type_ID < 1) throw new ArgumentException("VAS_Res_Type_ID is mandatory.");
            Set_Value("VAS_Res_Type_ID", VAS_Res_Type_ID);
        }
        /** Get Resource Type.
        @return Resource Type */
        public int GetVAS_Res_Type_ID()
        {
            Object ii = Get_Value("VAS_Res_Type_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Resource.
        @param VAS_Resource_ID Resource */
        public void SetS_Resource_ID(int VAS_Resource_ID)
        {
            if (VAS_Resource_ID < 1) throw new ArgumentException("VAS_Resource_ID is mandatory.");
            Set_ValueNoCheck("VAS_Resource_ID", VAS_Resource_ID);
        }
        /** Get Resource.
        @return Resource */
        public int GetS_Resource_ID()
        {
            Object ii = Get_Value("VAS_Resource_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Value = Value.Substring(0, 40);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }

        ////////
        /** VS_CATEGORY VAF_Control_Ref_ID=1000000 */
        public static int VS_CATEGORY_VAF_Control_Ref_ID = 1000000;
        /** Profile = F */
        public static String VS_CATEGORY_Profile = "F";
        /** Product = P */
        public static String VS_CATEGORY_Product = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVS_CATEGORYValid(String test)
        {
            return test.Equals("F") || test.Equals("P");
        }
        /** Set Resource Category.
    @param VS_CATEGORY Resource Category */
        public void SetVS_CATEGORY(String VS_CATEGORY)
        {
            if (VS_CATEGORY == null) throw new Exception("VS_CATEGORY is mandatory");
            if (!IsVS_CATEGORYValid(VS_CATEGORY))
                throw new Exception("VS_CATEGORY Invalid value - " + VS_CATEGORY + " - Reference_ID=1000000 - F - P");
            if (VS_CATEGORY.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VS_CATEGORY = VS_CATEGORY.Substring(0, 1);
            }
            Set_Value("VS_CATEGORY", VS_CATEGORY);
        }
        /** Get Resource Category.
        @return Resource Category */
        public String GetVS_CATEGORY()
        {
            return (String)Get_Value("VS_CATEGORY");
        }

        /** Set Role.
        @param VAF_Role_ID Responsibility Role */
        public void SetVAF_Role_ID(int VAF_Role_ID)
        {
            if (VAF_Role_ID <= 0) Set_Value("VAF_Role_ID", null);
            else
                Set_Value("VAF_Role_ID", VAF_Role_ID);
        }
        /** Get Role.
        @return Responsibility Role */
        public int GetVAF_Role_ID()
        {
            Object ii = Get_Value("VAF_Role_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Activate For Assignment.
        @param ActivateForAssignment Activate For Assignment */
        public void SetActivateForAssignment(String ActivateForAssignment)
        {
            if (ActivateForAssignment != null && ActivateForAssignment.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ActivateForAssignment = ActivateForAssignment.Substring(0, 1);
            }
            Set_Value("ActivateForAssignment", ActivateForAssignment);
        }
        /** Get Activate For Assignment.
        @return Activate For Assignment */
        public String GetActivateForAssignment()
        {
            return (String)Get_Value("ActivateForAssignment");
        }
        /** Set Customer/Prospect Group.
        @param VAB_BPart_Category_ID Customer/Prospect Group */
        public void SetVAB_BPart_Category_ID(int VAB_BPart_Category_ID)
        {
            if (VAB_BPart_Category_ID <= 0) Set_Value("VAB_BPart_Category_ID", null);
            else
                Set_Value("VAB_BPart_Category_ID", VAB_BPart_Category_ID);
        }
        /** Get Customer/Prospect Group.
        @return Customer/Prospect Group */
        public int GetVAB_BPart_Category_ID()
        {
            Object ii = Get_Value("VAB_BPart_Category_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Customer/Prospect.
        @param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }
        /** Get Customer/Prospect.
        @return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID()
        {
            Object ii = Get_Value("VAB_BusinessPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param VAB_Address_ID Location or Address */
        public void SetVAB_Address_ID(int VAB_Address_ID)
        {
            if (VAB_Address_ID <= 0) Set_Value("VAB_Address_ID", null);
            else
                Set_Value("VAB_Address_ID", VAB_Address_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetVAB_Address_ID()
        {
            Object ii = Get_Value("VAB_Address_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set EMail Address.
        @param EMail Electronic Mail Address */
        public void SetEMail(String EMail)
        {
            if (EMail != null && EMail.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                EMail = EMail.Substring(0, 50);
            }
            Set_Value("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
        }
        /** Set Experience.
        @param Experience Experience */
        public void SetExperience(Decimal? Experience)
        {
            Set_Value("Experience", (Decimal?)Experience);
        }
        /** Get Experience.
        @return Experience */
        public Decimal GetExperience()
        {
            Object bd = Get_Value("Experience");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set HourlyRate.
        @param HourlyRate HourlyRate */
        public void SetHourlyRate(Decimal? HourlyRate)
        {
            Set_Value("HourlyRate", (Decimal?)HourlyRate);
        }
        /** Get HourlyRate.
        @return HourlyRate */
        public Decimal GetHourlyRate()
        {
            Object bd = Get_Value("HourlyRate");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Key Skill.
        @param KeySkill Key Skill */
        public void SetKeySkill(String KeySkill)
        {
            if (KeySkill != null && KeySkill.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                KeySkill = KeySkill.Substring(0, 100);
            }
            Set_Value("KeySkill", KeySkill);
        }
        /** Get Key Skill.
        @return Key Skill */
        public String GetKeySkill()
        {
            return (String)Get_Value("KeySkill");
        }
        /** Set Keyword.
        @param Keyword Case insensitive keyword */
        public void SetKeyword(String Keyword)
        {
            if (Keyword != null && Keyword.Length > 100)
            {
                log.Warning("Length > 100 - truncated");
                Keyword = Keyword.Substring(0, 100);
            }
            Set_Value("Keyword", Keyword);
        }
        /** Get Keyword.
        @return Case insensitive keyword */
        public String GetKeyword()
        {
            return (String)Get_Value("Keyword");
        }
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID <= 0) Set_Value("VAM_Product_ID", null);
            else
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

        /** Set Mobile.
        @param Mobile Identifies the mobile number. */
        public void SetMobile(String Mobile)
        {
            if (Mobile != null && Mobile.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Mobile = Mobile.Substring(0, 50);
            }
            Set_Value("Mobile", Mobile);
        }
        /** Get Mobile.
        @return Identifies the mobile number. */
        public String GetMobile()
        {
            return (String)Get_Value("Mobile");
        }

        /** Set Profile.
        @param Profile Profile */
        public void SetProfile(String Profile)
        {
            if (Profile != null && Profile.Length > 3000)
            {
                log.Warning("Length > 3000 - truncated");
                Profile = Profile.Substring(0, 3000);
            }
            Set_Value("Profile", Profile);
        }
        /** Get Profile.
        @return Profile */
        public String GetProfile()
        {
            return (String)Get_Value("Profile");
        }

        /** ProfileType VAF_Control_Ref_ID=1000102 */
        public static int PROFILETYPE_VAF_Control_Ref_ID = 1000102;
        /** External = E */
        public static String PROFILETYPE_External = "E";
        /** Internal = I */
        public static String PROFILETYPE_Internal = "I";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProfileTypeValid(String test)
        {
            return test == null || test.Equals("E") || test.Equals("I");
        }
        /** Set Profile Type.
        @param ProfileType Profile Type */
        public void SetProfileType(String ProfileType)
        {
            if (!IsProfileTypeValid(ProfileType))
                throw new ArgumentException("ProfileType Invalid value - " + ProfileType + " - Reference_ID=1000102 - E - I");
            if (ProfileType != null && ProfileType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProfileType = ProfileType.Substring(0, 1);
            }
            Set_Value("ProfileType", ProfileType);
        }
        /** Get Profile Type.
        @return Profile Type */
        public String GetProfileType()
        {
            return (String)Get_Value("ProfileType");
        }

        /** Ref_BPartner_ID VAF_Control_Ref_ID=1000103 */
        public static int REF_BPARTNER_ID_VAF_Control_Ref_ID = 1000103;
        /** Set Prospects.
        @param Ref_BPartner_ID Identifies a Prospect */
        public void SetRef_BPartner_ID(int Ref_BPartner_ID)
        {
            if (Ref_BPartner_ID <= 0) Set_Value("Ref_BPartner_ID", null);
            else
                Set_Value("Ref_BPartner_ID", Ref_BPartner_ID);
        }
        /** Get Prospects.
        @return Identifies a Prospect */
        public int GetRef_BPartner_ID()
        {
            Object ii = Get_Value("Ref_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }
}
