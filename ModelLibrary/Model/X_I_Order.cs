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
    /** Generated Model for I_Order
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_I_Order : PO
    {
        public X_I_Order(Context ctx, int I_Order_ID, Trx trxName)
            : base(ctx, I_Order_ID, trxName)
        {
            /** if (I_Order_ID == 0)
            {
            SetI_IsImported (null);	// N
            SetI_Order_ID (0);
            }
             */
        }
        public X_I_Order(Ctx ctx, int I_Order_ID, Trx trxName)
            : base(ctx, I_Order_ID, trxName)
        {
            /** if (I_Order_ID == 0)
            {
            SetI_IsImported (null);	// N
            SetI_Order_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Order(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Order(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_I_Order(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_I_Order()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514377343L;
        /** Last Updated Timestamp 7/29/2010 1:07:40 PM */
        public static long updatedMS = 1280389060554L;
        /** AD_Table_ID=591 */
        public static int Table_ID;
        // =591;

        /** TableName=I_Order */
        public static String Table_Name = "I_Order";

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
            StringBuilder sb = new StringBuilder("X_I_Order[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_OrgTrx_ID AD_Reference_ID=130 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 130;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_Value("AD_OrgTrx_ID", null);
            else
                Set_Value("AD_OrgTrx_ID", AD_OrgTrx_ID);
        }
        /** Get Trx Organization.
        @return Performing or initiating organization */
        public int GetAD_OrgTrx_ID()
        {
            Object ii = Get_Value("AD_OrgTrx_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set User/Contact.
        @param AD_User_ID User within the system - Internal or Business Partner Contact */
        public void SetAD_User_ID(int AD_User_ID)
        {
            if (AD_User_ID <= 0) Set_Value("AD_User_ID", null);
            else
                Set_Value("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
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

        /** BillTo_ID AD_Reference_ID=159 */
        public static int BILLTO_ID_AD_Reference_ID = 159;
        /** Set Invoice To.
        @param BillTo_ID Bill to Address */
        public void SetBillTo_ID(int BillTo_ID)
        {
            if (BillTo_ID <= 0) Set_Value("BillTo_ID", null);
            else
                Set_Value("BillTo_ID", BillTo_ID);
        }
        /** Get Invoice To.
        @return Bill to Address */
        public int GetBillTo_ID()
        {
            Object ii = Get_Value("BillTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Activity.
        @param C_Activity_ID Business Activity */
        public void SetC_Activity_ID(int C_Activity_ID)
        {
            if (C_Activity_ID <= 0) Set_Value("C_Activity_ID", null);
            else
                Set_Value("C_Activity_ID", C_Activity_ID);
        }
        /** Get Activity.
        @return Business Activity */
        public int GetC_Activity_ID()
        {
            Object ii = Get_Value("C_Activity_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Business Partner */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Partner Location.
        @param C_BPartner_Location_ID Identifies the (ship to) address for this Business Partner */
        public void SetC_BPartner_Location_ID(int C_BPartner_Location_ID)
        {
            if (C_BPartner_Location_ID <= 0) Set_Value("C_BPartner_Location_ID", null);
            else
                Set_Value("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Campaign.
        @param C_Campaign_ID Marketing Campaign */
        public void SetC_Campaign_ID(int C_Campaign_ID)
        {
            if (C_Campaign_ID <= 0) Set_Value("C_Campaign_ID", null);
            else
                Set_Value("C_Campaign_ID", C_Campaign_ID);
        }
        /** Get Campaign.
        @return Marketing Campaign */
        public int GetC_Campaign_ID()
        {
            Object ii = Get_Value("C_Campaign_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Country.
        @param C_Country_ID Country */
        public void SetC_Country_ID(int C_Country_ID)
        {
            if (C_Country_ID <= 0) Set_Value("C_Country_ID", null);
            else
                Set_Value("C_Country_ID", C_Country_ID);
        }
        /** Get Country.
        @return Country */
        public int GetC_Country_ID()
        {
            Object ii = Get_Value("C_Country_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Currency.
        @param C_Currency_ID The Currency for this record */
        public void SetC_Currency_ID(int C_Currency_ID)
        {
            if (C_Currency_ID <= 0) Set_Value("C_Currency_ID", null);
            else
                Set_Value("C_Currency_ID", C_Currency_ID);
        }
        /** Get Currency.
        @return The Currency for this record */
        public int GetC_Currency_ID()
        {
            Object ii = Get_Value("C_Currency_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID <= 0) Set_Value("C_DocType_ID", null);
            else
                Set_Value("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Address.
        @param C_Location_ID Location or Address */
        public void SetC_Location_ID(int C_Location_ID)
        {
            if (C_Location_ID <= 0) Set_Value("C_Location_ID", null);
            else
                Set_Value("C_Location_ID", C_Location_ID);
        }
        /** Get Address.
        @return Location or Address */
        public int GetC_Location_ID()
        {
            Object ii = Get_Value("C_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Line.
        @param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_Value("C_OrderLine_ID", null);
            else
                Set_Value("C_OrderLine_ID", C_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetC_OrderLine_ID()
        {
            Object ii = Get_Value("C_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_Value("C_Order_ID", null);
            else
                Set_Value("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Term.
        @param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID)
        {
            if (C_PaymentTerm_ID <= 0) Set_Value("C_PaymentTerm_ID", null);
            else
                Set_Value("C_PaymentTerm_ID", C_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetC_PaymentTerm_ID()
        {
            Object ii = Get_Value("C_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Region.
        @param C_Region_ID Identifies a geographical Region */
        public void SetC_Region_ID(int C_Region_ID)
        {
            if (C_Region_ID <= 0) Set_Value("C_Region_ID", null);
            else
                Set_Value("C_Region_ID", C_Region_ID);
        }
        /** Get Region.
        @return Identifies a geographical Region */
        public int GetC_Region_ID()
        {
            Object ii = Get_Value("C_Region_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax.
        @param C_Tax_ID Tax identifier */
        public void SetC_Tax_ID(int C_Tax_ID)
        {
            if (C_Tax_ID <= 0) Set_Value("C_Tax_ID", null);
            else
                Set_Value("C_Tax_ID", C_Tax_ID);
        }
        /** Get Tax.
        @return Tax identifier */
        public int GetC_Tax_ID()
        {
            Object ii = Get_Value("C_Tax_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID <= 0) Set_Value("C_UOM_ID", null);
            else
                Set_Value("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
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
        /** Set Date Ordered.
        @param DateOrdered Date of Order */
        public void SetDateOrdered(DateTime? DateOrdered)
        {
            Set_Value("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
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
        /** Set Freight Amount.
        @param FreightAmt Freight Amount */
        public void SetFreightAmt(Decimal? FreightAmt)
        {
            Set_Value("FreightAmt", (Decimal?)FreightAmt);
        }
        /** Get Freight Amount.
        @return Freight Amount */
        public Decimal GetFreightAmt()
        {
            Object bd = Get_Value("FreightAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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

        /** I_IsImported AD_Reference_ID=420 */
        public static int I_ISIMPORTED_AD_Reference_ID = 420;
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
        /** Set Import Order.
        @param I_Order_ID Import Orders */
        public void SetI_Order_ID(int I_Order_ID)
        {
            if (I_Order_ID < 1) throw new ArgumentException("I_Order_ID is mandatory.");
            Set_ValueNoCheck("I_Order_ID", I_Order_ID);
        }
        /** Get Import Order.
        @return Import Orders */
        public int GetI_Order_ID()
        {
            Object ii = Get_Value("I_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        @param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID <= 0) Set_Value("M_PriceList_ID", null);
            else
                Set_Value("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID <= 0) Set_Value("M_Product_ID", null);
            else
                Set_Value("M_Product_ID", M_Product_ID);
        }
        /** Get Product.
        @return Product, Service, Item */
        public int GetM_Product_ID()
        {
            Object ii = Get_Value("M_Product_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Freight Carrier.
        @param M_Shipper_ID Method or manner of product delivery */
        public void SetM_Shipper_ID(int M_Shipper_ID)
        {
            if (M_Shipper_ID <= 0) Set_Value("M_Shipper_ID", null);
            else
                Set_Value("M_Shipper_ID", M_Shipper_ID);
        }
        /** Get Freight Carrier.
        @return Method or manner of product delivery */
        public int GetM_Shipper_ID()
        {
            Object ii = Get_Value("M_Shipper_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse.
        @param M_Warehouse_ID Storage Warehouse and Service Point */
        public void SetM_Warehouse_ID(int M_Warehouse_ID)
        {
            if (M_Warehouse_ID <= 0) Set_Value("M_Warehouse_ID", null);
            else
                Set_Value("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
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

        /** PaymentRule AD_Reference_ID=195 */
        public static int PAYMENTRULE_AD_Reference_ID = 195;
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
            //throw new ArgumentException ("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - P - S - T");
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

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
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
