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
    /** Generated Model for VAM_PFeature_Set
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_PFeature_Set : PO
    {
        public X_VAM_PFeature_Set(Context ctx, int VAM_PFeature_Set_ID, Trx trxName)
            : base(ctx, VAM_PFeature_Set_ID, trxName)
        {
            /** if (VAM_PFeature_Set_ID == 0)
            {
            SetIsGuaranteeDate (false);
            SetIsGuaranteeDateMandatory (false);
            SetIsInstanceAttribute (false);
            SetIsLot (false);
            SetIsLotMandatory (false);
            SetIsSerNo (false);
            SetIsSerNoMandatory (false);
            SetVAM_PFeature_Set_ID (0);
            SetMandatoryType (null);
            SetName (null);
            }
             */
        }
        public X_VAM_PFeature_Set(Ctx ctx, int VAM_PFeature_Set_ID, Trx trxName)
            : base(ctx, VAM_PFeature_Set_ID, trxName)
        {
            /** if (VAM_PFeature_Set_ID == 0)
            {
            SetIsGuaranteeDate (false);
            SetIsGuaranteeDateMandatory (false);
            SetIsInstanceAttribute (false);
            SetIsLot (false);
            SetIsLotMandatory (false);
            SetIsSerNo (false);
            SetIsSerNoMandatory (false);
            SetVAM_PFeature_Set_ID (0);
            SetMandatoryType (null);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Set(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Set(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_PFeature_Set(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_PFeature_Set()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514378315L;
        /** Last Updated Timestamp 7/29/2010 1:07:41 PM */
        public static long updatedMS = 1280389061526L;
        /** VAF_TableView_ID=560 */
        public static int Table_ID;
        // =560;

        /** TableName=VAM_PFeature_Set */
        public static String Table_Name = "VAM_PFeature_Set";

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
            StringBuilder sb = new StringBuilder("X_VAM_PFeature_Set[").Append(Get_ID()).Append("]");
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
        /** Set Guarantee Days.
        @param GuaranteeDays Number of days the product is guaranteed or available */
        public void SetGuaranteeDays(int GuaranteeDays)
        {
            Set_Value("GuaranteeDays", GuaranteeDays);
        }
        /** Get Guarantee Days.
        @return Number of days the product is guaranteed or available */
        public int GetGuaranteeDays()
        {
            Object ii = Get_Value("GuaranteeDays");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Guarantee Date.
        @param IsGuaranteeDate Product has Guarantee or Expiry Date */
        public void SetIsGuaranteeDate(Boolean IsGuaranteeDate)
        {
            Set_Value("IsGuaranteeDate", IsGuaranteeDate);
        }
        /** Get Guarantee Date.
        @return Product has Guarantee or Expiry Date */
        public Boolean IsGuaranteeDate()
        {
            Object oo = Get_Value("IsGuaranteeDate");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory Guarantee Date.
        @param IsGuaranteeDateMandatory The entry of a Guarantee Date is mandatory when creating a Product Instance */
        public void SetIsGuaranteeDateMandatory(Boolean IsGuaranteeDateMandatory)
        {
            Set_Value("IsGuaranteeDateMandatory", IsGuaranteeDateMandatory);
        }
        /** Get Mandatory Guarantee Date.
        @return The entry of a Guarantee Date is mandatory when creating a Product Instance */
        public Boolean IsGuaranteeDateMandatory()
        {
            Object oo = Get_Value("IsGuaranteeDateMandatory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Instance Attribute.
        @param IsInstanceAttribute The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
        public void SetIsInstanceAttribute(Boolean IsInstanceAttribute)
        {
            Set_Value("IsInstanceAttribute", IsInstanceAttribute);
        }
        /** Get Instance Attribute.
        @return The product attribute is specific to the instance (like Serial No, Lot or Guarantee Date) */
        public Boolean IsInstanceAttribute()
        {
            Object oo = Get_Value("IsInstanceAttribute");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Lot.
        @param IsLot The product instances have a Lot Number */
        public void SetIsLot(Boolean IsLot)
        {
            Set_Value("IsLot", IsLot);
        }
        /** Get Lot.
        @return The product instances have a Lot Number */
        public Boolean IsLot()
        {
            Object oo = Get_Value("IsLot");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory Lot.
        @param IsLotMandatory The entry of Lot info is mandatory when creating a Product Instance */
        public void SetIsLotMandatory(Boolean IsLotMandatory)
        {
            Set_Value("IsLotMandatory", IsLotMandatory);
        }
        /** Get Mandatory Lot.
        @return The entry of Lot info is mandatory when creating a Product Instance */
        public Boolean IsLotMandatory()
        {
            Object oo = Get_Value("IsLotMandatory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Serial No.
        @param IsSerNo The product instances have Serial Numbers */
        public void SetIsSerNo(Boolean IsSerNo)
        {
            Set_Value("IsSerNo", IsSerNo);
        }
        /** Get Serial No.
        @return The product instances have Serial Numbers */
        public Boolean IsSerNo()
        {
            Object oo = Get_Value("IsSerNo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Mandatory Serial No.
        @param IsSerNoMandatory The entry of a Serial No is mandatory when creating a Product Instance */
        public void SetIsSerNoMandatory(Boolean IsSerNoMandatory)
        {
            Set_Value("IsSerNoMandatory", IsSerNoMandatory);
        }
        /** Get Mandatory Serial No.
        @return The entry of a Serial No is mandatory when creating a Product Instance */
        public Boolean IsSerNoMandatory()
        {
            Object oo = Get_Value("IsSerNoMandatory");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Lot Char End Overwrite.
        @param LotCharEOverwrite Lot/Batch End Indicator overwrite - default » */
        public void SetLotCharEOverwrite(String LotCharEOverwrite)
        {
            if (LotCharEOverwrite != null && LotCharEOverwrite.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                LotCharEOverwrite = LotCharEOverwrite.Substring(0, 1);
            }
            Set_Value("LotCharEOverwrite", LotCharEOverwrite);
        }
        /** Get Lot Char End Overwrite.
        @return Lot/Batch End Indicator overwrite - default » */
        public String GetLotCharEOverwrite()
        {
            return (String)Get_Value("LotCharEOverwrite");
        }
        /** Set Lot Char Start Overwrite.
        @param LotCharSOverwrite Lot/Batch Start Indicator overwrite - default « */
        public void SetLotCharSOverwrite(String LotCharSOverwrite)
        {
            if (LotCharSOverwrite != null && LotCharSOverwrite.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                LotCharSOverwrite = LotCharSOverwrite.Substring(0, 1);
            }
            Set_Value("LotCharSOverwrite", LotCharSOverwrite);
        }
        /** Get Lot Char Start Overwrite.
        @return Lot/Batch Start Indicator overwrite - default « */
        public String GetLotCharSOverwrite()
        {
            return (String)Get_Value("LotCharSOverwrite");
        }
        /** Set Attribute Set.
        @param VAM_PFeature_Set_ID Product Attribute Set */
        public void SetVAM_PFeature_Set_ID(int VAM_PFeature_Set_ID)
        {
            if (VAM_PFeature_Set_ID < 0) throw new ArgumentException("VAM_PFeature_Set_ID is mandatory.");
            Set_ValueNoCheck("VAM_PFeature_Set_ID", VAM_PFeature_Set_ID);
        }
        /** Get Attribute Set.
        @return Product Attribute Set */
        public int GetVAM_PFeature_Set_ID()
        {
            Object ii = Get_Value("VAM_PFeature_Set_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Lot Control.
        @param VAM_LotControl_ID Product Lot Control */
        public void SetVAM_LotControl_ID(int VAM_LotControl_ID)
        {
            if (VAM_LotControl_ID <= 0) Set_Value("VAM_LotControl_ID", null);
            else
                Set_Value("VAM_LotControl_ID", VAM_LotControl_ID);
        }
        /** Get Lot Control.
        @return Product Lot Control */
        public int GetVAM_LotControl_ID()
        {
            Object ii = Get_Value("VAM_LotControl_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Serial No Control.
        @param VAM_CtlSerialNo_ID Product Serial Number Control */
        public void SetVAM_CtlSerialNo_ID(int VAM_CtlSerialNo_ID)
        {
            if (VAM_CtlSerialNo_ID <= 0) Set_Value("VAM_CtlSerialNo_ID", null);
            else
                Set_Value("VAM_CtlSerialNo_ID", VAM_CtlSerialNo_ID);
        }
        /** Get Serial No Control.
        @return Product Serial Number Control */
        public int GetVAM_CtlSerialNo_ID()
        {
            Object ii = Get_Value("VAM_CtlSerialNo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** MandatoryType VAF_Control_Ref_ID=324 */
        public static int MANDATORYTYPE_VAF_Control_Ref_ID = 324;
        /** Not Mandatary = N */
        public static String MANDATORYTYPE_NotMandatary = "N";
        /** When Shipping = S */
        public static String MANDATORYTYPE_WhenShipping = "S";
        /** Always Mandatory = Y */
        public static String MANDATORYTYPE_AlwaysMandatory = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMandatoryTypeValid(String test)
        {
            return test.Equals("N") || test.Equals("S") || test.Equals("Y");
        }
        /** Set Mandatory Type.
        @param MandatoryType The specification of a Attribute is mandatory */
        public void SetMandatoryType(String MandatoryType)
        {
            if (MandatoryType == null) throw new ArgumentException("MandatoryType is mandatory");
            if (!IsMandatoryTypeValid(MandatoryType))
                throw new ArgumentException("MandatoryType Invalid value - " + MandatoryType + " - Reference_ID=324 - N - S - Y");
            if (MandatoryType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MandatoryType = MandatoryType.Substring(0, 1);
            }
            Set_Value("MandatoryType", MandatoryType);
        }
        /** Get Mandatory Type.
        @return The specification of a Attribute is mandatory */
        public String GetMandatoryType()
        {
            return (String)Get_Value("MandatoryType");
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
        /** Set SerNo Char End Overwrite.
        @param SerNoCharEOverwrite Serial Number End Indicator overwrite - default empty */
        public void SetSerNoCharEOverwrite(String SerNoCharEOverwrite)
        {
            if (SerNoCharEOverwrite != null && SerNoCharEOverwrite.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SerNoCharEOverwrite = SerNoCharEOverwrite.Substring(0, 1);
            }
            Set_Value("SerNoCharEOverwrite", SerNoCharEOverwrite);
        }
        /** Get SerNo Char End Overwrite.
        @return Serial Number End Indicator overwrite - default empty */
        public String GetSerNoCharEOverwrite()
        {
            return (String)Get_Value("SerNoCharEOverwrite");
        }
        /** Set SerNo Char Start Overwrite.
        @param SerNoCharSOverwrite Serial Number Start Indicator overwrite - default # */
        public void SetSerNoCharSOverwrite(String SerNoCharSOverwrite)
        {
            if (SerNoCharSOverwrite != null && SerNoCharSOverwrite.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SerNoCharSOverwrite = SerNoCharSOverwrite.Substring(0, 1);
            }
            Set_Value("SerNoCharSOverwrite", SerNoCharSOverwrite);
        }
        /** Get SerNo Char Start Overwrite.
        @return Serial Number Start Indicator overwrite - default # */
        public String GetSerNoCharSOverwrite()
        {
            return (String)Get_Value("SerNoCharSOverwrite");
        }
        /** Set Reference Attribute Set ID.
        @param VA007_RefAttrSet_ID Reference Attribute Set ID */
        public void SetVA007_RefAttrSet_ID(String VA007_RefAttrSet_ID)
        {
            if (VA007_RefAttrSet_ID != null && VA007_RefAttrSet_ID.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                VA007_RefAttrSet_ID = VA007_RefAttrSet_ID.Substring(0, 20);
            }
            Set_Value("VA007_RefAttrSet_ID", VA007_RefAttrSet_ID);
        }
        /** Get Reference Attribute Set ID.
        @return Reference Attribute Set ID */
        public String GetVA007_RefAttrSet_ID()
        {
            return (String)Get_Value("VA007_RefAttrSet_ID");
        }
    }

}
