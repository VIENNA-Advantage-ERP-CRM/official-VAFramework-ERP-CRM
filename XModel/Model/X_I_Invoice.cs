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
    /** Generated Model for I_Invoice
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_I_Invoice : PO
    {
        public X_I_Invoice(Context ctx, int I_Invoice_ID, Trx trxName)
            : base(ctx, I_Invoice_ID, trxName)
        {
            /** if (I_Invoice_ID == 0)
            {
            SetI_Invoice_ID (0);
            SetI_IsImported (null);	// N
            }
             */
        }
        public X_I_Invoice(Ctx ctx, int I_Invoice_ID, Trx trxName)
            : base(ctx, I_Invoice_ID, trxName)
        {
            /** if (I_Invoice_ID == 0)
            {
            SetI_Invoice_ID (0);
            SetI_IsImported (null);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Invoice(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Invoice(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Invoice(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_I_Invoice()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514377218L;
        /** Last Updated Timestamp 7/29/2010 1:07:40 PM */
        public static long updatedMS = 1280389060429L;
        /** VAF_TableView_ID=598 */
        public static int Table_ID;
        // =598;

        /** TableName=I_Invoice */
        public static String Table_Name = "I_Invoice";

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
            StringBuilder sb = new StringBuilder("X_I_Invoice[").Append(Get_ID()).Append("]");
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
        /** Set Address 1.
        @param Address1 Address line 1 for this location */
        public void SetAddress1(String Address1)
        {
            if (Address1 != null && Address1.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address1 = Address1.Substring(0, 60);
            }
            Set_Value("Address1", Address1);
        }
        /** Get Address 1.
        @return Address line 1 for this location */
        public String GetAddress1()
        {
            return (String)Get_Value("Address1");
        }
        /** Set Address 2.
        @param Address2 Address line 2 for this location */
        public void SetAddress2(String Address2)
        {
            if (Address2 != null && Address2.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address2 = Address2.Substring(0, 60);
            }
            Set_Value("Address2", Address2);
        }
        /** Get Address 2.
        @return Address line 2 for this location */
        public String GetAddress2()
        {
            return (String)Get_Value("Address2");
        }
        /** Set Business Partner Key.
        @param BPartnerValue Key of the Business Partner */
        public void SetBPartnerValue(String BPartnerValue)
        {
            if (BPartnerValue != null && BPartnerValue.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                BPartnerValue = BPartnerValue.Substring(0, 40);
            }
            Set_Value("BPartnerValue", BPartnerValue);
        }
        /** Get Business Partner Key.
        @return Key of the Business Partner */
        public String GetBPartnerValue()
        {
            return (String)Get_Value("BPartnerValue");
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
        /** Set Business Partner.
        @param VAB_BusinessPartner_ID Identifies a Business Partner */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_Value("VAB_BusinessPartner_ID", null);
            else
                Set_Value("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetVAB_BusinessPartner_ID()
        {
            Object ii = Get_Value("VAB_BusinessPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
        @param VAB_BPart_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetVAB_BPart_Location_ID(int VAB_BPart_Location_ID)
        {
            if (VAB_BPart_Location_ID <= 0) Set_Value("VAB_BPart_Location_ID", null);
            else
                Set_Value("VAB_BPart_Location_ID", VAB_BPart_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetVAB_BPart_Location_ID()
        {
            Object ii = Get_Value("VAB_BPart_Location_ID");
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
        /** Set Country.
        @param VAB_Country_ID Country */
        public void SetVAB_Country_ID(int VAB_Country_ID)
        {
            if (VAB_Country_ID <= 0) Set_Value("VAB_Country_ID", null);
            else
                Set_Value("VAB_Country_ID", VAB_Country_ID);
        }
        /** Get Country.
        @return Country */
        public int GetVAB_Country_ID()
        {
            Object ii = Get_Value("VAB_Country_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param VAB_Currency_ID The Currency for this record */
        public void SetVAB_Currency_ID(int VAB_Currency_ID)
        {
            if (VAB_Currency_ID <= 0) Set_Value("VAB_Currency_ID", null);
            else
                Set_Value("VAB_Currency_ID", VAB_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetVAB_Currency_ID()
        {
            Object ii = Get_Value("VAB_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Type.
        @param VAB_DocTypes_ID Document type or rules */
        public void SetVAB_DocTypes_ID(int VAB_DocTypes_ID)
        {
            if (VAB_DocTypes_ID <= 0) Set_Value("VAB_DocTypes_ID", null);
            else
                Set_Value("VAB_DocTypes_ID", VAB_DocTypes_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetVAB_DocTypes_ID()
        {
            Object ii = Get_Value("VAB_DocTypes_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Line.
        @param VAB_InvoiceLine_ID Invoice Detail Line */
        public void SetVAB_InvoiceLine_ID(int VAB_InvoiceLine_ID)
        {
            if (VAB_InvoiceLine_ID <= 0) Set_Value("VAB_InvoiceLine_ID", null);
            else
                Set_Value("VAB_InvoiceLine_ID", VAB_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetVAB_InvoiceLine_ID()
        {
            Object ii = Get_Value("VAB_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param VAB_Invoice_ID Invoice Identifier */
        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            if (VAB_Invoice_ID <= 0) Set_Value("VAB_Invoice_ID", null);
            else
                Set_Value("VAB_Invoice_ID", VAB_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetVAB_Invoice_ID()
        {
            Object ii = Get_Value("VAB_Invoice_ID");
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
        /** Set Payment Term.
        @param VAB_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetVAB_PaymentTerm_ID(int VAB_PaymentTerm_ID)
        {
            if (VAB_PaymentTerm_ID <= 0) Set_Value("VAB_PaymentTerm_ID", null);
            else
                Set_Value("VAB_PaymentTerm_ID", VAB_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetVAB_PaymentTerm_ID()
        {
            Object ii = Get_Value("VAB_PaymentTerm_ID");
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
        /** Set Region.
        @param VAB_RegionState_ID Identifies a geographical Region */
        public void SetVAB_RegionState_ID(int VAB_RegionState_ID)
        {
            if (VAB_RegionState_ID <= 0) Set_Value("VAB_RegionState_ID", null);
            else
                Set_Value("VAB_RegionState_ID", VAB_RegionState_ID);
        }
        /** Get Region.
        @return Identifies a geographical Region */
        public int GetVAB_RegionState_ID()
        {
            Object ii = Get_Value("VAB_RegionState_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax.
        @param VAB_TaxRate_ID Tax identifier */
        public void SetVAB_TaxRate_ID(int VAB_TaxRate_ID)
        {
            if (VAB_TaxRate_ID <= 0) Set_Value("VAB_TaxRate_ID", null);
            else
                Set_Value("VAB_TaxRate_ID", VAB_TaxRate_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetVAB_TaxRate_ID()
        {
            Object ii = Get_Value("VAB_TaxRate_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set City.
        @param City Identifies a City */
        public void SetCity(String City)
        {
            if (City != null && City.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                City = City.Substring(0, 60);
            }
            Set_Value("City", City);
        }
        /** Get City.
        @return Identifies a City */
        public String GetCity()
        {
            return (String)Get_Value("City");
        }
        /** Set Contact Name.
        @param ContactName Business Partner Contact Name */
        public void SetContactName(String ContactName)
        {
            if (ContactName != null && ContactName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ContactName = ContactName.Substring(0, 60);
            }
            Set_Value("ContactName", ContactName);
        }
        /** Get Contact Name.
        @return Business Partner Contact Name */
        public String GetContactName()
        {
            return (String)Get_Value("ContactName");
        }
        /** Set ISO Country Code.
        @param CountryCode Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
        public void SetCountryCode(String CountryCode)
        {
            if (CountryCode != null && CountryCode.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                CountryCode = CountryCode.Substring(0, 2);
            }
            Set_Value("CountryCode", CountryCode);
        }
        /** Get ISO Country Code.
        @return Upper-case two-letter alphabetic ISO Country code according to ISO 3166-1 - http://www.chemie.fu-berlin.de/diverse/doc/ISO_3166.html */
        public String GetCountryCode()
        {
            return (String)Get_Value("CountryCode");
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            Set_Value("DateAcct", (DateTime?)DateAcct);
        }
        /** Get Account Date.
        @return General Ledger Date */
        public DateTime? GetDateAcct()
        {
            return (DateTime?)Get_Value("DateAcct");
        }
        /** Set Date Invoiced.
        @param DateInvoiced Date printed on Invoice */
        public void SetDateInvoiced(DateTime? DateInvoiced)
        {
            Set_Value("DateInvoiced", (DateTime?)DateInvoiced);
        }
        /** Get Date Invoiced.
        @return Date printed on Invoice */
        public DateTime? GetDateInvoiced()
        {
            return (DateTime?)Get_Value("DateInvoiced");
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
        /** Set Document Type Name.
        @param DocTypeName Name of the Document Type */
        public void SetDocTypeName(String DocTypeName)
        {
            if (DocTypeName != null && DocTypeName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                DocTypeName = DocTypeName.Substring(0, 60);
            }
            Set_Value("DocTypeName", DocTypeName);
        }
        /** Get Document Type Name.
        @return Name of the Document Type */
        public String GetDocTypeName()
        {
            return (String)Get_Value("DocTypeName");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo != null && DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_Value("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Set EMail Address.
        @param EMail Electronic Mail Address */
        public void SetEMail(String EMail)
        {
            if (EMail != null && EMail.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                EMail = EMail.Substring(0, 60);
            }
            Set_Value("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
        }
        /** Set Import Error Message.
        @param I_ErrorMsg Messages generated from import process */
        public void SetI_ErrorMsg(String I_ErrorMsg)
        {
            if (I_ErrorMsg != null && I_ErrorMsg.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                I_ErrorMsg = I_ErrorMsg.Substring(0, 2000);
            }
            Set_Value("I_ErrorMsg", I_ErrorMsg);
        }
        /** Get Import Error Message.
        @return Messages generated from import process */
        public String GetI_ErrorMsg()
        {
            return (String)Get_Value("I_ErrorMsg");
        }
        /** Set Import Invoice.
        @param I_Invoice_ID Import Invoice */
        public void SetI_Invoice_ID(int I_Invoice_ID)
        {
            if (I_Invoice_ID < 1) throw new ArgumentException("I_Invoice_ID is mandatory.");
            Set_ValueNoCheck("I_Invoice_ID", I_Invoice_ID);
        }
        /** Get Import Invoice.
        @return Import Invoice */
        public int GetI_Invoice_ID()
        {
            Object ii = Get_Value("I_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** I_IsImported VAF_Control_Ref_ID=420 */
        public static int I_ISIMPORTED_VAF_Control_Ref_ID = 420;
        /** Error = E */
        public static String I_ISIMPORTED_Error = "E";
        /** No = N */
        public static String I_ISIMPORTED_No = "N";
        /** Yes = Y */
        public static String I_ISIMPORTED_Yes = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsI_IsImportedValid(String test)
        {
            return test.Equals("E") || test.Equals("N") || test.Equals("Y");
        }
        /** Set Imported.
        @param I_IsImported Has this import been processed */
        public void SetI_IsImported(String I_IsImported)
        {
            if (I_IsImported == null) throw new ArgumentException("I_IsImported is mandatory");
            if (!IsI_IsImportedValid(I_IsImported))
                throw new ArgumentException("I_IsImported Invalid value - " + I_IsImported + " - Reference_ID=420 - E - N - Y");
            if (I_IsImported.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                I_IsImported = I_IsImported.Substring(0, 1);
            }
            Set_Value("I_IsImported", I_IsImported);
        }
        /** Get Imported.
        @return Has this import been processed */
        public String GetI_IsImported()
        {
            return (String)Get_Value("I_IsImported");
        }
        /** Set Sales Transaction.
        @param IsSOTrx This is a Sales Transaction */
        public void SetIsSOTrx(Boolean IsSOTrx)
        {
            Set_Value("IsSOTrx", IsSOTrx);
        }
        /** Get Sales Transaction.
        @return This is a Sales Transaction */
        public Boolean IsSOTrx()
        {
            Object oo = Get_Value("IsSOTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line Description.
        @param LineDescription Description of the Line */
        public void SetLineDescription(String LineDescription)
        {
            if (LineDescription != null && LineDescription.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LineDescription = LineDescription.Substring(0, 255);
            }
            Set_Value("LineDescription", LineDescription);
        }
        /** Get Line Description.
        @return Description of the Line */
        public String GetLineDescription()
        {
            return (String)Get_Value("LineDescription");
        }
        /** Set Price List.
        @param VAM_PriceList_ID Unique identifier of a Price List */
        public void SetVAM_PriceList_ID(int VAM_PriceList_ID)
        {
            if (VAM_PriceList_ID <= 0) Set_Value("VAM_PriceList_ID", null);
            else
                Set_Value("VAM_PriceList_ID", VAM_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetVAM_PriceList_ID()
        {
            Object ii = Get_Value("VAM_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Name.
        @param Name Alphanumeric identifier of the entity */
        public void SetName(String Name)
        {
            if (Name != null && Name.Length > 60)
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

        /** PaymentRule VAF_Control_Ref_ID=195 */
        public static int PAYMENTRULE_VAF_Control_Ref_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULE_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULE_CreditCard = "K";
        /** On Credit = P */
        public static String PAYMENTRULE_OnCredit = "P";
        /** Check = S */
        public static String PAYMENTRULE_Check = "S";
        /** Direct Deposit = T */
        public static String PAYMENTRULE_DirectDeposit = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentRuleValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Method.
        @param PaymentRule How you pay the invoice */
        public void SetPaymentRule(String PaymentRule)
        {
            //if (!IsPaymentRuleValid(PaymentRule))
            //    throw new ArgumentException("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - P - S - T");
            if (PaymentRule != null && PaymentRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRule = PaymentRule.Substring(0, 1);
            }
            Set_Value("PaymentRule", PaymentRule);
        }
        /** Get Payment Method.
        @return How you pay the invoice */
        public String GetPaymentRule()
        {
            return (String)Get_Value("PaymentRule");
        }
        /** Set Payment Rule Name.
        @param PaymentRuleName Name of the Payment Rule */
        public void SetPaymentRuleName(String PaymentRuleName)
        {
            if (PaymentRuleName != null && PaymentRuleName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                PaymentRuleName = PaymentRuleName.Substring(0, 60);
            }
            Set_Value("PaymentRuleName", PaymentRuleName);
        }
        /** Get Payment Rule Name.
        @return Name of the Payment Rule */
        public String GetPaymentRuleName()
        {
            return (String)Get_Value("PaymentRuleName");
        }
        /** Set Payment Term Key.
        @param PaymentTermValue Key of the Payment Term */
        public void SetPaymentTermValue(String PaymentTermValue)
        {
            if (PaymentTermValue != null && PaymentTermValue.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                PaymentTermValue = PaymentTermValue.Substring(0, 40);
            }
            Set_Value("PaymentTermValue", PaymentTermValue);
        }
        /** Get Payment Term Key.
        @return Key of the Payment Term */
        public String GetPaymentTermValue()
        {
            return (String)Get_Value("PaymentTermValue");
        }
        /** Set Phone.
        @param Phone Identifies a telephone number */
        public void SetPhone(String Phone)
        {
            if (Phone != null && Phone.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Phone = Phone.Substring(0, 40);
            }
            Set_Value("Phone", Phone);
        }
        /** Get Phone.
        @return Identifies a telephone number */
        public String GetPhone()
        {
            return (String)Get_Value("Phone");
        }
        /** Set ZIP.
        @param Postal Postal code */
        public void SetPostal(String Postal)
        {
            if (Postal != null && Postal.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                Postal = Postal.Substring(0, 10);
            }
            Set_Value("Postal", Postal);
        }
        /** Get ZIP.
        @return Postal code */
        public String GetPostal()
        {
            return (String)Get_Value("Postal");
        }
        /** Set Unit Price.
        @param PriceActual Actual Price */
        public void SetPriceActual(Decimal? PriceActual)
        {
            Set_Value("PriceActual", (Decimal?)PriceActual);
        }
        /** Get Unit Price.
        @return Actual Price */
        public Decimal GetPriceActual()
        {
            Object bd = Get_Value("PriceActual");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_Value("Processed", Processed);
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
        /** Set Product Key.
        @param ProductValue Key of the Product */
        public void SetProductValue(String ProductValue)
        {
            if (ProductValue != null && ProductValue.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                ProductValue = ProductValue.Substring(0, 40);
            }
            Set_Value("ProductValue", ProductValue);
        }
        /** Get Product Key.
        @return Key of the Product */
        public String GetProductValue()
        {
            return (String)Get_Value("ProductValue");
        }
        /** Set Ordered Quantity.
        @param QtyOrdered Ordered Quantity */
        public void SetQtyOrdered(Decimal? QtyOrdered)
        {
            Set_Value("QtyOrdered", (Decimal?)QtyOrdered);
        }
        /** Get Ordered Quantity.
        @return Ordered Quantity */
        public Decimal GetQtyOrdered()
        {
            Object bd = Get_Value("QtyOrdered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Region.
        @param RegionName Name of the Region */
        public void SetRegionName(String RegionName)
        {
            if (RegionName != null && RegionName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                RegionName = RegionName.Substring(0, 60);
            }
            Set_Value("RegionName", RegionName);
        }
        /** Get Region.
        @return Name of the Region */
        public String GetRegionName()
        {
            return (String)Get_Value("RegionName");
        }
        /** Set SKU.
        @param SKU Stock Keeping Unit */
        public void SetSKU(String SKU)
        {
            if (SKU != null && SKU.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                SKU = SKU.Substring(0, 30);
            }
            Set_Value("SKU", SKU);
        }
        /** Get SKU.
        @return Stock Keeping Unit */
        public String GetSKU()
        {
            return (String)Get_Value("SKU");
        }

        /** SalesRep_ID VAF_Control_Ref_ID=190 */
        public static int SALESREP_ID_VAF_Control_Ref_ID = 190;
        /** Set Representative.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Representative.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Amount.
        @param TaxAmt Tax Amount for a document */
        public void SetTaxAmt(Decimal? TaxAmt)
        {
            Set_Value("TaxAmt", (Decimal?)TaxAmt);
        }
        /** Get Tax Amount.
        @return Tax Amount for a document */
        public Decimal GetTaxAmt()
        {
            Object bd = Get_Value("TaxAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Tax Indicator.
        @param TaxIndicator Short form for Tax to be printed on documents */
        public void SetTaxIndicator(String TaxIndicator)
        {
            if (TaxIndicator != null && TaxIndicator.Length > 5)
            {
                log.Warning("Length > 5 - truncated");
                TaxIndicator = TaxIndicator.Substring(0, 5);
            }
            Set_Value("TaxIndicator", TaxIndicator);
        }
        /** Get Tax Indicator.
        @return Short form for Tax to be printed on documents */
        public String GetTaxIndicator()
        {
            return (String)Get_Value("TaxIndicator");
        }
        /** Set UPC/EAN.
        @param UPC Bar Code (Universal Product Code or its superset European Article Number) */
        public void SetUPC(String UPC)
        {
            if (UPC != null && UPC.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                UPC = UPC.Substring(0, 30);
            }
            Set_Value("UPC", UPC);
        }
        /** Get UPC/EAN.
        @return Bar Code (Universal Product Code or its superset European Article Number) */
        public String GetUPC()
        {
            return (String)Get_Value("UPC");
        }
    }

}
