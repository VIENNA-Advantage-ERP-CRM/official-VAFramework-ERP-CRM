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
    /** Generated Model for C_TaxCategory
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_TaxCategory : PO
    {
        public X_C_TaxCategory(Context ctx, int C_TaxCategory_ID, Trx trxName)
            : base(ctx, C_TaxCategory_ID, trxName)
        {
            /** if (C_TaxCategory_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetIsDefault (false);
            SetName (null);
            }
             */
        }
        public X_C_TaxCategory(Ctx ctx, int C_TaxCategory_ID, Trx trxName)
            : base(ctx, C_TaxCategory_ID, trxName)
        {
            /** if (C_TaxCategory_ID == 0)
            {
            SetC_TaxCategory_ID (0);
            SetIsDefault (false);
            SetName (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_TaxCategory(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_TaxCategory(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_TaxCategory(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_TaxCategory()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514375384L;
        /** Last Updated Timestamp 7/29/2010 1:07:38 PM */
        public static long updatedMS = 1280389058595L;
        /** AD_Table_ID=252 */
        public static int Table_ID;
        // =252;

        /** TableName=C_TaxCategory */
        public static String Table_Name = "C_TaxCategory";

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
            StringBuilder sb = new StringBuilder("X_C_TaxCategory[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Tax Category.
        @param C_TaxCategory_ID Tax Category */
        public void SetC_TaxCategory_ID(int C_TaxCategory_ID)
        {
            if (C_TaxCategory_ID < 1) throw new ArgumentException("C_TaxCategory_ID is mandatory.");
            Set_ValueNoCheck("C_TaxCategory_ID", C_TaxCategory_ID);
        }
        /** Get Tax Category.
        @return Tax Category */
        public int GetC_TaxCategory_ID()
        {
            Object ii = Get_Value("C_TaxCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Default Tax.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Default Tax.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Commodity Code.
        @param CommodityCode Commodity code used for tax calculation */
        public void SetCommodityCode(String CommodityCode)
        {
            if (CommodityCode != null && CommodityCode.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                CommodityCode = CommodityCode.Substring(0, 20);
            }
            Set_Value("CommodityCode", CommodityCode);
        }
        /** Get Commodity Code.
        @return Commodity code used for tax calculation */
        public String GetCommodityCode()
        {
            return (String)Get_Value("CommodityCode");
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
        /** Set Default.
        @param IsDefault Default value */
        public void SetIsDefault(Boolean IsDefault)
        {
            Set_Value("IsDefault", IsDefault);
        }
        /** Get Default.
        @return Default value */
        public Boolean IsDefault()
        {
            Object oo = Get_Value("IsDefault");
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

        /** Set Reference Tax Cat ID.
        @param VA007_RefTaxCat_ID Reference Tax Cat ID */
        public void SetVA007_RefTaxCat_ID(String VA007_RefTaxCat_ID)
        {
            if (VA007_RefTaxCat_ID != null && VA007_RefTaxCat_ID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA007_RefTaxCat_ID = VA007_RefTaxCat_ID.Substring(0, 50);
            }
            Set_Value("VA007_RefTaxCat_ID", VA007_RefTaxCat_ID);
        }
        /** Get Reference Tax Cat ID.
        @return Reference Tax Cat ID */
        public String GetVA007_RefTaxCat_ID()
        {
            return (String)Get_Value("VA007_RefTaxCat_ID");
        }
        /** Set Generate Lines.
@param VATAX_GenerateLines Generate Lines */
        public void SetVATAX_GenerateLines(String VATAX_GenerateLines)
        {
            if (VATAX_GenerateLines != null && VATAX_GenerateLines.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_GenerateLines = VATAX_GenerateLines.Substring(0, 1);
            }
            Set_Value("VATAX_GenerateLines", VATAX_GenerateLines);
        }
        /** Get Generate Lines.
        @return Generate Lines */
        public String GetVATAX_GenerateLines()
        {
            return (String)Get_Value("VATAX_GenerateLines");
        }
        /** VATAX_Location AD_Reference_ID=1000191 */
        public static int VATAX_LOCATION_AD_Reference_ID = 1000191;
        /** Invoice To Address = I */
        public static String VATAX_LOCATION_InvoiceToAddress = "I";
        /** Ship To Address = S */
        public static String VATAX_LOCATION_ShipToAddress = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVATAX_LocationValid(String test)
        {
            return test == null || test.Equals("I") || test.Equals("S");
        }
        /** Set Location.
        @param VATAX_Location Location */
        public void SetVATAX_Location(String VATAX_Location)
        {
            if (!IsVATAX_LocationValid(VATAX_Location))
                throw new ArgumentException("VATAX_Location Invalid value - " + VATAX_Location + " - Reference_ID=1000191 - I - S");
            if (VATAX_Location != null && VATAX_Location.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_Location = VATAX_Location.Substring(0, 1);
            }
            Set_Value("VATAX_Location", VATAX_Location);
        }
        /** Get Location.
        @return Location */
        public String GetVATAX_Location()
        {
            return (String)Get_Value("VATAX_Location");
        }
        /** VATAX_Preference1 AD_Reference_ID=1000190 */
        public static int VATAX_PREFERENCE1_AD_Reference_ID = 1000190;
        /** Location = L */
        public static String VATAX_PREFERENCE1_Location = "L";
        /** Tax Region = R */
        public static String VATAX_PREFERENCE1_TaxRegion = "R";
        /** Tax Class = T */
        public static String VATAX_PREFERENCE1_TaxClass = "T";
        /** Tax Class = D */
        public static String VATAX_PREFERENCE1_DocumentType = "D";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVATAX_Preference1Valid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("R") || test.Equals("T") || test.Equals("D");
        }
        /** Set Tax Preference 1.
        @param VATAX_Preference1 Tax Preference 1 */
        public void SetVATAX_Preference1(String VATAX_Preference1)
        {
            if (!IsVATAX_Preference1Valid(VATAX_Preference1))
                throw new ArgumentException("VATAX_Preference1 Invalid value - " + VATAX_Preference1 + " - Reference_ID=1000190 - L - R - T - D");
            if (VATAX_Preference1 != null && VATAX_Preference1.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_Preference1 = VATAX_Preference1.Substring(0, 1);
            }
            Set_Value("VATAX_Preference1", VATAX_Preference1);
        }
        /** Get Tax Preference 1.
        @return Tax Preference 1 */
        public String GetVATAX_Preference1()
        {
            return (String)Get_Value("VATAX_Preference1");
        }

        /** VATAX_Preference2 AD_Reference_ID=1000190 */
        public static int VATAX_PREFERENCE2_AD_Reference_ID = 1000190;
        /** Location = L */
        public static String VATAX_PREFERENCE2_Location = "L";
        /** Tax Region = R */
        public static String VATAX_PREFERENCE2_TaxRegion = "R";
        /** Tax Class = T */
        public static String VATAX_PREFERENCE2_TaxClass = "T";
        /** Tax Class = D */
        public static String VATAX_PREFERENCE2_DocumentType = "D";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVATAX_Preference2Valid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("R") || test.Equals("T") || test.Equals("D");
        }
        /** Set Tax Preference 2.
        @param VATAX_Preference2 Tax Preference 2 */
        public void SetVATAX_Preference2(String VATAX_Preference2)
        {
            if (!IsVATAX_Preference2Valid(VATAX_Preference2))
                throw new ArgumentException("VATAX_Preference2 Invalid value - " + VATAX_Preference2 + " - Reference_ID=1000190 - L - R - T - D");
            if (VATAX_Preference2 != null && VATAX_Preference2.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_Preference2 = VATAX_Preference2.Substring(0, 1);
            }
            Set_Value("VATAX_Preference2", VATAX_Preference2);
        }
        /** Get Tax Preference 2.
        @return Tax Preference 2 */
        public String GetVATAX_Preference2()
        {
            return (String)Get_Value("VATAX_Preference2");
        }

        /** VATAX_Preference3 AD_Reference_ID=1000190 */
        public static int VATAX_PREFERENCE3_AD_Reference_ID = 1000190;
        /** Location = L */
        public static String VATAX_PREFERENCE3_Location = "L";
        /** Tax Region = R */
        public static String VATAX_PREFERENCE3_TaxRegion = "R";
        /** Tax Class = T */
        public static String VATAX_PREFERENCE3_TaxClass = "T";
        /** Tax Class = D */
        public static String VATAX_PREFERENCE3_DocumentType = "D";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVATAX_Preference3Valid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("R") || test.Equals("T") || test.Equals("D");
        }
        /** Set Tax Preference 3.
        @param VATAX_Preference3 Tax Preference 3 */
        public void SetVATAX_Preference3(String VATAX_Preference3)
        {
            if (!IsVATAX_Preference3Valid(VATAX_Preference3))
                throw new ArgumentException("VATAX_Preference3 Invalid value - " + VATAX_Preference3 + " - Reference_ID=1000190 - L - R - T - D");
            if (VATAX_Preference3 != null && VATAX_Preference3.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VATAX_Preference3 = VATAX_Preference3.Substring(0, 1);
            }
            Set_Value("VATAX_Preference3", VATAX_Preference3);
        }
        /** Get Tax Preference 3.
        @return Tax Preference 3 */
        public String GetVATAX_Preference3()
        {
            return (String)Get_Value("VATAX_Preference3");
        }
    }
}
