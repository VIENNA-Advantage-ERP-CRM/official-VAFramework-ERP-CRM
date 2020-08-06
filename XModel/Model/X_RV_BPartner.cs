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
    /** Generated Model for RV_BPartner
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_RV_BPartner : PO
    {
        public X_RV_BPartner(Context ctx, int RV_BPartner_ID, Trx trxName)
            : base(ctx, RV_BPartner_ID, trxName)
        {
            /** if (RV_BPartner_ID == 0)
            {
            SetAD_User_ID (0);
            SetC_BP_Group_ID (0);
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Country_ID (0);
            SetContactName (null);
            SetCountryName (null);
            SetIsCustomer (false);
            SetIsEmployee (false);
            SetIsFullBPAccess (false);
            SetIsOneTime (false);
            SetIsProspect (false);
            SetIsSalesRep (false);
            SetIsSummary (false);
            SetIsVendor (false);
            SetLDAPUser (false);
            SetName (null);
            SetSendEMail (false);
            SetValue (null);
            }
             */
        }
        public X_RV_BPartner(Ctx ctx, int RV_BPartner_ID, Trx trxName)
            : base(ctx, RV_BPartner_ID, trxName)
        {
            /** if (RV_BPartner_ID == 0)
            {
            SetAD_User_ID (0);
            SetC_BP_Group_ID (0);
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_Country_ID (0);
            SetContactName (null);
            SetCountryName (null);
            SetIsCustomer (false);
            SetIsEmployee (false);
            SetIsFullBPAccess (false);
            SetIsOneTime (false);
            SetIsProspect (false);
            SetIsSalesRep (false);
            SetIsSummary (false);
            SetIsVendor (false);
            SetLDAPUser (false);
            SetName (null);
            SetSendEMail (false);
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RV_BPartner(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RV_BPartner(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_RV_BPartner(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_RV_BPartner()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514382390L;
        /** Last Updated Timestamp 7/29/2010 1:07:45 PM */
        public static long updatedMS = 1280389065601L;
        /** AD_Table_ID=520 */
        public static int Table_ID;
        // =520;

        /** TableName=RV_BPartner */
        public static String Table_Name = "RV_BPartner";

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
            StringBuilder sb = new StringBuilder("X_RV_BPartner[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_Language AD_Reference_ID=106 */
        public static int AD_LANGUAGE_AD_Reference_ID = 106;
        /** Set Language.
        @param AD_Language Language for this entity */
        public void SetAD_Language(String AD_Language)
        {
            if (AD_Language != null && AD_Language.Length > 5)
            {
                log.Warning("Length > 5 - truncated");
                AD_Language = AD_Language.Substring(0, 5);
            }
            Set_ValueNoCheck("AD_Language", AD_Language);
        }
        /** Get Language.
        @return Language for this entity */
        public String GetAD_Language()
        {
            return (String)Get_Value("AD_Language");
        }

        /** AD_OrgBP_ID AD_Reference_ID=276 */
        public static int AD_ORGBP_ID_AD_Reference_ID = 276;
        /** Set Linked Organization.
        @param AD_OrgBP_ID The Business Partner is another Organization for explicit Inter-Org transactions */
        public void SetAD_OrgBP_ID(int AD_OrgBP_ID)
        {
            if (AD_OrgBP_ID <= 0) Set_ValueNoCheck("AD_OrgBP_ID", null);
            else
                Set_ValueNoCheck("AD_OrgBP_ID", AD_OrgBP_ID);
        }
        /** Get Linked Organization.
        @return The Business Partner is another Organization for explicit Inter-Org transactions */
        public int GetAD_OrgBP_ID()
        {
            Object ii = Get_Value("AD_OrgBP_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** AD_OrgTrx_ID AD_Reference_ID=276 */
        public static int AD_ORGTRX_ID_AD_Reference_ID = 276;
        /** Set Trx Organization.
        @param AD_OrgTrx_ID Performing or initiating organization */
        public void SetAD_OrgTrx_ID(int AD_OrgTrx_ID)
        {
            if (AD_OrgTrx_ID <= 0) Set_ValueNoCheck("AD_OrgTrx_ID", null);
            else
                Set_ValueNoCheck("AD_OrgTrx_ID", AD_OrgTrx_ID);
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
            if (AD_User_ID < 1) throw new ArgumentException("AD_User_ID is mandatory.");
            Set_ValueNoCheck("AD_User_ID", AD_User_ID);
        }
        /** Get User/Contact.
        @return User within the system - Internal or Business Partner Contact */
        public int GetAD_User_ID()
        {
            Object ii = Get_Value("AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Acquisition Cost.
        @param AcqusitionCost The cost of gaining the prospect as a customer */
        public void SetAcqusitionCost(Decimal? AcqusitionCost)
        {
            Set_ValueNoCheck("AcqusitionCost", (Decimal?)AcqusitionCost);
        }
        /** Get Acquisition Cost.
        @return The cost of gaining the prospect as a customer */
        public Decimal GetAcqusitionCost()
        {
            Object bd = Get_Value("AcqusitionCost");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Life Time Value.
        @param ActualLifeTimeValue Actual Life Time Revenue */
        public void SetActualLifeTimeValue(Decimal? ActualLifeTimeValue)
        {
            Set_ValueNoCheck("ActualLifeTimeValue", (Decimal?)ActualLifeTimeValue);
        }
        /** Get Life Time Value.
        @return Actual Life Time Revenue */
        public Decimal GetActualLifeTimeValue()
        {
            Object bd = Get_Value("ActualLifeTimeValue");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
            Set_ValueNoCheck("Address1", Address1);
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
            Set_ValueNoCheck("Address2", Address2);
        }
        /** Get Address 2.
        @return Address line 2 for this location */
        public String GetAddress2()
        {
            return (String)Get_Value("Address2");
        }
        /** Set Address 3.
        @param Address3 Address Line 3 for the location */
        public void SetAddress3(String Address3)
        {
            if (Address3 != null && Address3.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Address3 = Address3.Substring(0, 60);
            }
            Set_ValueNoCheck("Address3", Address3);
        }
        /** Get Address 3.
        @return Address Line 3 for the location */
        public String GetAddress3()
        {
            return (String)Get_Value("Address3");
        }

        /** BPContactGreeting AD_Reference_ID=356 */
        public static int BPCONTACTGREETING_AD_Reference_ID = 356;
        /** Set BP Contact Greeting.
        @param BPContactGreeting Greeting for Business Partner Contact */
        public void SetBPContactGreeting(int BPContactGreeting)
        {
            Set_ValueNoCheck("BPContactGreeting", BPContactGreeting);
        }
        /** Get BP Contact Greeting.
        @return Greeting for Business Partner Contact */
        public int GetBPContactGreeting()
        {
            Object ii = Get_Value("BPContactGreeting");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** BPartner_Parent_ID AD_Reference_ID=124 */
        public static int BPARTNER_PARENT_ID_AD_Reference_ID = 124;
        /** Set Partner Parent.
        @param BPartner_Parent_ID Business Partner Parent */
        public void SetBPartner_Parent_ID(int BPartner_Parent_ID)
        {
            if (BPartner_Parent_ID <= 0) Set_ValueNoCheck("BPartner_Parent_ID", null);
            else
                Set_ValueNoCheck("BPartner_Parent_ID", BPartner_Parent_ID);
        }
        /** Get Partner Parent.
        @return Business Partner Parent */
        public int GetBPartner_Parent_ID()
        {
            Object ii = Get_Value("BPartner_Parent_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Birthday.
        @param Birthday Birthday or Anniversary day */
        public void SetBirthday(DateTime? Birthday)
        {
            Set_ValueNoCheck("Birthday", (DateTime?)Birthday);
        }
        /** Get Birthday.
        @return Birthday or Anniversary day */
        public DateTime? GetBirthday()
        {
            return (DateTime?)Get_Value("Birthday");
        }
        /** Set Business Partner Group.
        @param C_BP_Group_ID Business Partner Group */
        public void SetC_BP_Group_ID(int C_BP_Group_ID)
        {
            if (C_BP_Group_ID < 1) throw new ArgumentException("C_BP_Group_ID is mandatory.");
            Set_ValueNoCheck("C_BP_Group_ID", C_BP_Group_ID);
        }
        /** Get Business Partner Group.
        @return Business Partner Group */
        public int GetC_BP_Group_ID()
        {
            Object ii = Get_Value("C_BP_Group_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Business Partner */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
            Set_ValueNoCheck("C_BPartner_ID", C_BPartner_ID);
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
            if (C_BPartner_Location_ID < 1) throw new ArgumentException("C_BPartner_Location_ID is mandatory.");
            Set_ValueNoCheck("C_BPartner_Location_ID", C_BPartner_Location_ID);
        }
        /** Get Partner Location.
        @return Identifies the (ship to) address for this Business Partner */
        public int GetC_BPartner_Location_ID()
        {
            Object ii = Get_Value("C_BPartner_Location_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Country.
        @param C_Country_ID Country */
        public void SetC_Country_ID(int C_Country_ID)
        {
            if (C_Country_ID < 1) throw new ArgumentException("C_Country_ID is mandatory.");
            Set_ValueNoCheck("C_Country_ID", C_Country_ID);
        }
        /** Get Country.
        @return Country */
        public int GetC_Country_ID()
        {
            Object ii = Get_Value("C_Country_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Dunning.
        @param C_Dunning_ID Dunning Rules for overdue invoices */
        public void SetC_Dunning_ID(int C_Dunning_ID)
        {
            if (C_Dunning_ID <= 0) Set_ValueNoCheck("C_Dunning_ID", null);
            else
                Set_ValueNoCheck("C_Dunning_ID", C_Dunning_ID);
        }
        /** Get Dunning.
        @return Dunning Rules for overdue invoices */
        public int GetC_Dunning_ID()
        {
            Object ii = Get_Value("C_Dunning_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Greeting.
        @param C_Greeting_ID Greeting to print on correspondence */
        public void SetC_Greeting_ID(int C_Greeting_ID)
        {
            if (C_Greeting_ID <= 0) Set_ValueNoCheck("C_Greeting_ID", null);
            else
                Set_ValueNoCheck("C_Greeting_ID", C_Greeting_ID);
        }
        /** Get Greeting.
        @return Greeting to print on correspondence */
        public int GetC_Greeting_ID()
        {
            Object ii = Get_Value("C_Greeting_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Schedule.
        @param C_InvoiceSchedule_ID Schedule for generating Invoices */
        public void SetC_InvoiceSchedule_ID(int C_InvoiceSchedule_ID)
        {
            if (C_InvoiceSchedule_ID <= 0) Set_ValueNoCheck("C_InvoiceSchedule_ID", null);
            else
                Set_ValueNoCheck("C_InvoiceSchedule_ID", C_InvoiceSchedule_ID);
        }
        /** Get Invoice Schedule.
        @return Schedule for generating Invoices */
        public int GetC_InvoiceSchedule_ID()
        {
            Object ii = Get_Value("C_InvoiceSchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Position.
        @param C_Job_ID Job Position */
        public void SetC_Job_ID(int C_Job_ID)
        {
            if (C_Job_ID <= 0) Set_ValueNoCheck("C_Job_ID", null);
            else
                Set_ValueNoCheck("C_Job_ID", C_Job_ID);
        }
        /** Get Position.
        @return Job Position */
        public int GetC_Job_ID()
        {
            Object ii = Get_Value("C_Job_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Term.
        @param C_PaymentTerm_ID The terms of Payment (timing, discount) */
        public void SetC_PaymentTerm_ID(int C_PaymentTerm_ID)
        {
            if (C_PaymentTerm_ID <= 0) Set_ValueNoCheck("C_PaymentTerm_ID", null);
            else
                Set_ValueNoCheck("C_PaymentTerm_ID", C_PaymentTerm_ID);
        }
        /** Get Payment Term.
        @return The terms of Payment (timing, discount) */
        public int GetC_PaymentTerm_ID()
        {
            Object ii = Get_Value("C_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Region.
        @param C_Region_ID Identifies a geographical Region */
        public void SetC_Region_ID(int C_Region_ID)
        {
            if (C_Region_ID <= 0) Set_ValueNoCheck("C_Region_ID", null);
            else
                Set_ValueNoCheck("C_Region_ID", C_Region_ID);
        }
        /** Get Region.
        @return Identifies a geographical Region */
        public int GetC_Region_ID()
        {
            Object ii = Get_Value("C_Region_ID");
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
            Set_ValueNoCheck("City", City);
        }
        /** Get City.
        @return Identifies a City */
        public String GetCity()
        {
            return (String)Get_Value("City");
        }
        /** Set Comments.
        @param Comments Comments or additional information */
        public void SetComments(String Comments)
        {
            if (Comments != null && Comments.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Comments = Comments.Substring(0, 2000);
            }
            Set_ValueNoCheck("Comments", Comments);
        }
        /** Get Comments.
        @return Comments or additional information */
        public String GetComments()
        {
            return (String)Get_Value("Comments");
        }
        /** Set Contact Description.
        @param ContactDescription Description of Contact */
        public void SetContactDescription(String ContactDescription)
        {
            if (ContactDescription != null && ContactDescription.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                ContactDescription = ContactDescription.Substring(0, 255);
            }
            Set_ValueNoCheck("ContactDescription", ContactDescription);
        }
        /** Get Contact Description.
        @return Description of Contact */
        public String GetContactDescription()
        {
            return (String)Get_Value("ContactDescription");
        }
        /** Set Contact Name.
        @param ContactName Business Partner Contact Name */
        public void SetContactName(String ContactName)
        {
            if (ContactName == null) throw new ArgumentException("ContactName is mandatory.");
            if (ContactName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                ContactName = ContactName.Substring(0, 60);
            }
            Set_ValueNoCheck("ContactName", ContactName);
        }
        /** Get Contact Name.
        @return Business Partner Contact Name */
        public String GetContactName()
        {
            return (String)Get_Value("ContactName");
        }
        /** Set Country.
        @param CountryName Country Name */
        public void SetCountryName(String CountryName)
        {
            if (CountryName == null) throw new ArgumentException("CountryName is mandatory.");
            if (CountryName.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                CountryName = CountryName.Substring(0, 60);
            }
            Set_ValueNoCheck("CountryName", CountryName);
        }
        /** Get Country.
        @return Country Name */
        public String GetCountryName()
        {
            return (String)Get_Value("CountryName");
        }
        /** Set D-U-N-S.
        @param DUNS Creditor Check (Dun & Bradstreet) Number */
        public void SetDUNS(String DUNS)
        {
            if (DUNS != null && DUNS.Length > 11)
            {
                log.Warning("Length > 11 - truncated");
                DUNS = DUNS.Substring(0, 11);
            }
            Set_ValueNoCheck("DUNS", DUNS);
        }
        /** Get D-U-N-S.
        @return Creditor Check (Dun & Bradstreet) Number */
        public String GetDUNS()
        {
            return (String)Get_Value("DUNS");
        }

        /** DeliveryRule AD_Reference_ID=151 */
        public static int DELIVERYRULE_AD_Reference_ID = 151;
        /** Availability = A */
        public static String DELIVERYRULE_Availability = "A";
        /** Force = F */
        public static String DELIVERYRULE_Force = "F";
        /** Complete Line = L */
        public static String DELIVERYRULE_CompleteLine = "L";
        /** Manual = M */
        public static String DELIVERYRULE_Manual = "M";
        /** Complete Order = O */
        public static String DELIVERYRULE_CompleteOrder = "O";
        /** After Receipt = R */
        public static String DELIVERYRULE_AfterReceipt = "R";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDeliveryRuleValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("F") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
        }
        /** Set Shipping Rule.
        @param DeliveryRule Defines the timing of Shipping */
        public void SetDeliveryRule(String DeliveryRule)
        {
            if (!IsDeliveryRuleValid(DeliveryRule))
                throw new ArgumentException("DeliveryRule Invalid value - " + DeliveryRule + " - Reference_ID=151 - A - F - L - M - O - R");
            if (DeliveryRule != null && DeliveryRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DeliveryRule = DeliveryRule.Substring(0, 1);
            }
            Set_ValueNoCheck("DeliveryRule", DeliveryRule);
        }
        /** Get Shipping Rule.
        @return Defines the timing of Shipping */
        public String GetDeliveryRule()
        {
            return (String)Get_Value("DeliveryRule");
        }

        /** DeliveryViaRule AD_Reference_ID=152 */
        public static int DELIVERYVIARULE_AD_Reference_ID = 152;
        /** Delivery = D */
        public static String DELIVERYVIARULE_Delivery = "D";
        /** Pickup = P */
        public static String DELIVERYVIARULE_Pickup = "P";
        /** Shipper = S */
        public static String DELIVERYVIARULE_Shipper = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDeliveryViaRuleValid(String test)
        {
            return test == null || test.Equals("D") || test.Equals("P") || test.Equals("S");
        }
        /** Set Shipping Method.
        @param DeliveryViaRule How the order will be delivered */
        public void SetDeliveryViaRule(String DeliveryViaRule)
        {
            if (!IsDeliveryViaRuleValid(DeliveryViaRule))
                throw new ArgumentException("DeliveryViaRule Invalid value - " + DeliveryViaRule + " - Reference_ID=152 - D - P - S");
            if (DeliveryViaRule != null && DeliveryViaRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                DeliveryViaRule = DeliveryViaRule.Substring(0, 1);
            }
            Set_ValueNoCheck("DeliveryViaRule", DeliveryViaRule);
        }
        /** Get Shipping Method.
        @return How the order will be delivered */
        public String GetDeliveryViaRule()
        {
            return (String)Get_Value("DeliveryViaRule");
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
            Set_ValueNoCheck("Description", Description);
        }
        /** Get Description.
        @return Optional short description of the record */
        public String GetDescription()
        {
            return (String)Get_Value("Description");
        }
        /** Set Document Copies.
        @param DocumentCopies Number of copies to be printed */
        public void SetDocumentCopies(int DocumentCopies)
        {
            Set_ValueNoCheck("DocumentCopies", DocumentCopies);
        }
        /** Get Document Copies.
        @return Number of copies to be printed */
        public int GetDocumentCopies()
        {
            Object ii = Get_Value("DocumentCopies");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
            Set_ValueNoCheck("EMail", EMail);
        }
        /** Get EMail Address.
        @return Electronic Mail Address */
        public String GetEMail()
        {
            return (String)Get_Value("EMail");
        }
        /** Set EMail User ID.
        @param EMailUser User Name (ID) in the Mail System */
        public void SetEMailUser(String EMailUser)
        {
            if (EMailUser != null && EMailUser.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                EMailUser = EMailUser.Substring(0, 60);
            }
            Set_ValueNoCheck("EMailUser", EMailUser);
        }
        /** Get EMail User ID.
        @return User Name (ID) in the Mail System */
        public String GetEMailUser()
        {
            return (String)Get_Value("EMailUser");
        }
        /** Set Verification Info.
        @param EMailVerify Verification information of EMail Address */
        public void SetEMailVerify(String EMailVerify)
        {
            if (EMailVerify != null && EMailVerify.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                EMailVerify = EMailVerify.Substring(0, 40);
            }
            Set_ValueNoCheck("EMailVerify", EMailVerify);
        }
        /** Get Verification Info.
        @return Verification information of EMail Address */
        public String GetEMailVerify()
        {
            return (String)Get_Value("EMailVerify");
        }
        /** Set EMail Verify.
        @param EMailVerifyDate Date Email was verified */
        public void SetEMailVerifyDate(DateTime? EMailVerifyDate)
        {
            Set_ValueNoCheck("EMailVerifyDate", (DateTime?)EMailVerifyDate);
        }
        /** Get EMail Verify.
        @return Date Email was verified */
        public DateTime? GetEMailVerifyDate()
        {
            return (DateTime?)Get_Value("EMailVerifyDate");
        }
        /** Set Fax.
        @param Fax Facsimile number */
        public void SetFax(String Fax)
        {
            if (Fax != null && Fax.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Fax = Fax.Substring(0, 40);
            }
            Set_ValueNoCheck("Fax", Fax);
        }
        /** Get Fax.
        @return Facsimile number */
        public String GetFax()
        {
            return (String)Get_Value("Fax");
        }
        /** Set First Sale.
        @param FirstSale Date of First Sale */
        public void SetFirstSale(DateTime? FirstSale)
        {
            Set_ValueNoCheck("FirstSale", (DateTime?)FirstSale);
        }
        /** Get First Sale.
        @return Date of First Sale */
        public DateTime? GetFirstSale()
        {
            return (DateTime?)Get_Value("FirstSale");
        }
        /** Set Flat Discount %.
        @param FlatDiscount Flat discount percentage */
        public void SetFlatDiscount(Decimal? FlatDiscount)
        {
            Set_ValueNoCheck("FlatDiscount", (Decimal?)FlatDiscount);
        }
        /** Get Flat Discount %.
        @return Flat discount percentage */
        public Decimal GetFlatDiscount()
        {
            Object bd = Get_Value("FlatDiscount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** FreightCostRule AD_Reference_ID=153 */
        public static int FREIGHTCOSTRULE_AD_Reference_ID = 153;
        /** Calculated = C */
        public static String FREIGHTCOSTRULE_Calculated = "C";
        /** Fix price = F */
        public static String FREIGHTCOSTRULE_FixPrice = "F";
        /** Freight included = I */
        public static String FREIGHTCOSTRULE_FreightIncluded = "I";
        /** Line = L */
        public static String FREIGHTCOSTRULE_Line = "L";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFreightCostRuleValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L");
        }
        /** Set Freight Cost Rule.
        @param FreightCostRule Method for charging Freight */
        public void SetFreightCostRule(String FreightCostRule)
        {
            if (!IsFreightCostRuleValid(FreightCostRule))
                throw new ArgumentException("FreightCostRule Invalid value - " + FreightCostRule + " - Reference_ID=153 - C - F - I - L");
            if (FreightCostRule != null && FreightCostRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                FreightCostRule = FreightCostRule.Substring(0, 1);
            }
            Set_ValueNoCheck("FreightCostRule", FreightCostRule);
        }
        /** Get Freight Cost Rule.
        @return Method for charging Freight */
        public String GetFreightCostRule()
        {
            return (String)Get_Value("FreightCostRule");
        }

        /** InvoiceRule AD_Reference_ID=150 */
        public static int INVOICERULE_AD_Reference_ID = 150;
        /** After Delivery = D */
        public static String INVOICERULE_AfterDelivery = "D";
        /** Immediate = I */
        public static String INVOICERULE_Immediate = "I";
        /** After Order delivered = O */
        public static String INVOICERULE_AfterOrderDelivered = "O";
        /** Customer Schedule after Delivery = S */
        public static String INVOICERULE_CustomerScheduleAfterDelivery = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsInvoiceRuleValid(String test)
        {
            return test == null || test.Equals("D") || test.Equals("I") || test.Equals("O") || test.Equals("S");
        }
        /** Set Invoicing Rule.
        @param InvoiceRule Frequency and method of invoicing */
        public void SetInvoiceRule(String InvoiceRule)
        {
            if (!IsInvoiceRuleValid(InvoiceRule))
                throw new ArgumentException("InvoiceRule Invalid value - " + InvoiceRule + " - Reference_ID=150 - D - I - O - S");
            if (InvoiceRule != null && InvoiceRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                InvoiceRule = InvoiceRule.Substring(0, 1);
            }
            Set_ValueNoCheck("InvoiceRule", InvoiceRule);
        }
        /** Get Invoicing Rule.
        @return Frequency and method of invoicing */
        public String GetInvoiceRule()
        {
            return (String)Get_Value("InvoiceRule");
        }

        /** Invoice_PrintFormat_ID AD_Reference_ID=259 */
        public static int INVOICE_PRINTFORMAT_ID_AD_Reference_ID = 259;
        /** Set Invoice Print Format.
        @param Invoice_PrintFormat_ID Print Format for printing Invoices */
        public void SetInvoice_PrintFormat_ID(int Invoice_PrintFormat_ID)
        {
            if (Invoice_PrintFormat_ID <= 0) Set_ValueNoCheck("Invoice_PrintFormat_ID", null);
            else
                Set_ValueNoCheck("Invoice_PrintFormat_ID", Invoice_PrintFormat_ID);
        }
        /** Get Invoice Print Format.
        @return Print Format for printing Invoices */
        public int GetInvoice_PrintFormat_ID()
        {
            Object ii = Get_Value("Invoice_PrintFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Customer.
        @param IsCustomer Indicates if this Business Partner is a Customer */
        public void SetIsCustomer(Boolean IsCustomer)
        {
            Set_ValueNoCheck("IsCustomer", IsCustomer);
        }
        /** Get Customer.
        @return Indicates if this Business Partner is a Customer */
        public Boolean IsCustomer()
        {
            Object oo = Get_Value("IsCustomer");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Discount Printed.
        @param IsDiscountPrinted Print Discount on Invoice and Order */
        public void SetIsDiscountPrinted(Boolean IsDiscountPrinted)
        {
            Set_ValueNoCheck("IsDiscountPrinted", IsDiscountPrinted);
        }
        /** Get Discount Printed.
        @return Print Discount on Invoice and Order */
        public Boolean IsDiscountPrinted()
        {
            Object oo = Get_Value("IsDiscountPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Employee.
        @param IsEmployee Indicates if  this Business Partner is an employee */
        public void SetIsEmployee(Boolean IsEmployee)
        {
            Set_ValueNoCheck("IsEmployee", IsEmployee);
        }
        /** Get Employee.
        @return Indicates if  this Business Partner is an employee */
        public Boolean IsEmployee()
        {
            Object oo = Get_Value("IsEmployee");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Full BP Access.
        @param IsFullBPAccess The user/contact has full access to Business Partner information and resources */
        public void SetIsFullBPAccess(Boolean IsFullBPAccess)
        {
            Set_ValueNoCheck("IsFullBPAccess", IsFullBPAccess);
        }
        /** Get Full BP Access.
        @return The user/contact has full access to Business Partner information and resources */
        public Boolean IsFullBPAccess()
        {
            Object oo = Get_Value("IsFullBPAccess");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set One time transaction.
        @param IsOneTime One time transaction */
        public void SetIsOneTime(Boolean IsOneTime)
        {
            Set_ValueNoCheck("IsOneTime", IsOneTime);
        }
        /** Get One time transaction.
        @return One time transaction */
        public Boolean IsOneTime()
        {
            Object oo = Get_Value("IsOneTime");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Prospect.
        @param IsProspect Indicates this is a Prospect */
        public void SetIsProspect(Boolean IsProspect)
        {
            Set_ValueNoCheck("IsProspect", IsProspect);
        }
        /** Get Prospect.
        @return Indicates this is a Prospect */
        public Boolean IsProspect()
        {
            Object oo = Get_Value("IsProspect");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Representative.
        @param IsSalesRep Indicates if  the business partner is a representative or company agent */
        public void SetIsSalesRep(Boolean IsSalesRep)
        {
            Set_ValueNoCheck("IsSalesRep", IsSalesRep);
        }
        /** Get Representative.
        @return Indicates if  the business partner is a representative or company agent */
        public Boolean IsSalesRep()
        {
            Object oo = Get_Value("IsSalesRep");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Summary Level.
        @param IsSummary This is a summary entity */
        public void SetIsSummary(Boolean IsSummary)
        {
            Set_ValueNoCheck("IsSummary", IsSummary);
        }
        /** Get Summary Level.
        @return This is a summary entity */
        public Boolean IsSummary()
        {
            Object oo = Get_Value("IsSummary");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Tax exempt.
        @param IsTaxExempt Business partner is exempt from tax */
        public void SetIsTaxExempt(Boolean IsTaxExempt)
        {
            Set_ValueNoCheck("IsTaxExempt", IsTaxExempt);
        }
        /** Get Tax exempt.
        @return Business partner is exempt from tax */
        public Boolean IsTaxExempt()
        {
            Object oo = Get_Value("IsTaxExempt");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Vendor.
        @param IsVendor Indicates if this Business Partner is a Vendor */
        public void SetIsVendor(Boolean IsVendor)
        {
            Set_ValueNoCheck("IsVendor", IsVendor);
        }
        /** Get Vendor.
        @return Indicates if this Business Partner is a Vendor */
        public Boolean IsVendor()
        {
            Object oo = Get_Value("IsVendor");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set LDAP User Name.
        @param LDAPUser User Name used for authorization via LDAP (directory) services */
        public void SetLDAPUser(Boolean LDAPUser)
        {
            Set_ValueNoCheck("LDAPUser", LDAPUser);
        }
        /** Get LDAP User Name.
        @return User Name used for authorization via LDAP (directory) services */
        public Boolean IsLDAPUser()
        {
            Object oo = Get_Value("LDAPUser");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Last Contact.
        @param LastContact Date this individual was last contacted */
        public void SetLastContact(DateTime? LastContact)
        {
            Set_ValueNoCheck("LastContact", (DateTime?)LastContact);
        }
        /** Get Last Contact.
        @return Date this individual was last contacted */
        public DateTime? GetLastContact()
        {
            return (DateTime?)Get_Value("LastContact");
        }
        /** Set Last Result.
        @param LastResult Result of last contact */
        public void SetLastResult(String LastResult)
        {
            if (LastResult != null && LastResult.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                LastResult = LastResult.Substring(0, 255);
            }
            Set_ValueNoCheck("LastResult", LastResult);
        }
        /** Get Last Result.
        @return Result of last contact */
        public String GetLastResult()
        {
            return (String)Get_Value("LastResult");
        }
        /** Set Discount Schema.
        @param M_DiscountSchema_ID Schema to calculate price lists or the trade discount percentage */
        public void SetM_DiscountSchema_ID(int M_DiscountSchema_ID)
        {
            if (M_DiscountSchema_ID <= 0) Set_ValueNoCheck("M_DiscountSchema_ID", null);
            else
                Set_ValueNoCheck("M_DiscountSchema_ID", M_DiscountSchema_ID);
        }
        /** Get Discount Schema.
        @return Schema to calculate price lists or the trade discount percentage */
        public int GetM_DiscountSchema_ID()
        {
            Object ii = Get_Value("M_DiscountSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Price List.
        @param M_PriceList_ID Unique identifier of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID <= 0) Set_ValueNoCheck("M_PriceList_ID", null);
            else
                Set_ValueNoCheck("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Unique identifier of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set NAICS/SIC.
        @param NAICS Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
        public void SetNAICS(String NAICS)
        {
            if (NAICS != null && NAICS.Length > 6)
            {
                log.Warning("Length > 6 - truncated");
                NAICS = NAICS.Substring(0, 6);
            }
            Set_ValueNoCheck("NAICS", NAICS);
        }
        /** Get NAICS/SIC.
        @return Standard Industry Code or its successor NAIC - http://www.osha.gov/oshstats/sicser.html */
        public String GetNAICS()
        {
            return (String)Get_Value("NAICS");
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
            Set_ValueNoCheck("Name", Name);
        }
        /** Get Name.
        @return Alphanumeric identifier of the entity */
        public String GetName()
        {
            return (String)Get_Value("Name");
        }
        /** Set Name 2.
        @param Name2 Additional Name */
        public void SetName2(String Name2)
        {
            if (Name2 != null && Name2.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name2 = Name2.Substring(0, 60);
            }
            Set_ValueNoCheck("Name2", Name2);
        }
        /** Get Name 2.
        @return Additional Name */
        public String GetName2()
        {
            return (String)Get_Value("Name2");
        }

        /** NotificationType AD_Reference_ID=344 */
        public static int NOTIFICATIONTYPE_AD_Reference_ID = 344;
        /** EMail+Notice = B */
        public static String NOTIFICATIONTYPE_EMailPlusNotice = "B";
        /** EMail = E */
        public static String NOTIFICATIONTYPE_EMail = "E";
        /** Notice = N */
        public static String NOTIFICATIONTYPE_Notice = "N";
        /** None = X */
        public static String NOTIFICATIONTYPE_None = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsNotificationTypeValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("E") || test.Equals("N") || test.Equals("X");
        }
        /** Set Notification Type.
        @param NotificationType Type of Notifications */
        public void SetNotificationType(String NotificationType)
        {
            if (!IsNotificationTypeValid(NotificationType))
                throw new ArgumentException("NotificationType Invalid value - " + NotificationType + " - Reference_ID=344 - B - E - N - X");
            if (NotificationType != null && NotificationType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                NotificationType = NotificationType.Substring(0, 1);
            }
            Set_ValueNoCheck("NotificationType", NotificationType);
        }
        /** Get Notification Type.
        @return Type of Notifications */
        public String GetNotificationType()
        {
            return (String)Get_Value("NotificationType");
        }
        /** Set Employees.
        @param NumberEmployees Number of employees */
        public void SetNumberEmployees(int NumberEmployees)
        {
            Set_ValueNoCheck("NumberEmployees", NumberEmployees);
        }
        /** Get Employees.
        @return Number of employees */
        public int GetNumberEmployees()
        {
            Object ii = Get_Value("NumberEmployees");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order Reference.
        @param POReference Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public void SetPOReference(String POReference)
        {
            if (POReference != null && POReference.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                POReference = POReference.Substring(0, 20);
            }
            Set_ValueNoCheck("POReference", POReference);
        }
        /** Get Order Reference.
        @return Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public String GetPOReference()
        {
            return (String)Get_Value("POReference");
        }

        /** PO_DiscountSchema_ID AD_Reference_ID=249 */
        public static int PO_DISCOUNTSCHEMA_ID_AD_Reference_ID = 249;
        /** Set PO Discount Schema.
        @param PO_DiscountSchema_ID Schema to calculate the purchase trade discount percentage */
        public void SetPO_DiscountSchema_ID(int PO_DiscountSchema_ID)
        {
            if (PO_DiscountSchema_ID <= 0) Set_ValueNoCheck("PO_DiscountSchema_ID", null);
            else
                Set_ValueNoCheck("PO_DiscountSchema_ID", PO_DiscountSchema_ID);
        }
        /** Get PO Discount Schema.
        @return Schema to calculate the purchase trade discount percentage */
        public int GetPO_DiscountSchema_ID()
        {
            Object ii = Get_Value("PO_DiscountSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PO_PaymentTerm_ID AD_Reference_ID=227 */
        public static int PO_PAYMENTTERM_ID_AD_Reference_ID = 227;
        /** Set PO Payment Term.
        @param PO_PaymentTerm_ID Payment rules for a purchase order */
        public void SetPO_PaymentTerm_ID(int PO_PaymentTerm_ID)
        {
            if (PO_PaymentTerm_ID <= 0) Set_ValueNoCheck("PO_PaymentTerm_ID", null);
            else
                Set_ValueNoCheck("PO_PaymentTerm_ID", PO_PaymentTerm_ID);
        }
        /** Get PO Payment Term.
        @return Payment rules for a purchase order */
        public int GetPO_PaymentTerm_ID()
        {
            Object ii = Get_Value("PO_PaymentTerm_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PO_PriceList_ID AD_Reference_ID=166 */
        public static int PO_PRICELIST_ID_AD_Reference_ID = 166;
        /** Set Purchase Pricelist.
        @param PO_PriceList_ID Price List used by this Business Partner */
        public void SetPO_PriceList_ID(int PO_PriceList_ID)
        {
            if (PO_PriceList_ID <= 0) Set_ValueNoCheck("PO_PriceList_ID", null);
            else
                Set_ValueNoCheck("PO_PriceList_ID", PO_PriceList_ID);
        }
        /** Get Purchase Pricelist.
        @return Price List used by this Business Partner */
        public int GetPO_PriceList_ID()
        {
            Object ii = Get_Value("PO_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
            Set_ValueNoCheck("PaymentRule", PaymentRule);
        }
        /** Get Payment Method.
        @return How you pay the invoice */
        public String GetPaymentRule()
        {
            return (String)Get_Value("PaymentRule");
        }

        /** PaymentRulePO AD_Reference_ID=195 */
        public static int PAYMENTRULEPO_AD_Reference_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULEPO_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULEPO_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULEPO_CreditCard = "K";
        /** On Credit = P */
        public static String PAYMENTRULEPO_OnCredit = "P";
        /** Check = S */
        public static String PAYMENTRULEPO_Check = "S";
        /** Direct Deposit = T */
        public static String PAYMENTRULEPO_DirectDeposit = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPaymentRulePOValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Rule.
        @param PaymentRulePO Purchase payment option */
        public void SetPaymentRulePO(String PaymentRulePO)
        {
            //if (!IsPaymentRulePOValid(PaymentRulePO))
            //    throw new ArgumentException("PaymentRulePO Invalid value - " + PaymentRulePO + " - Reference_ID=195 - B - D - K - P - S - T");
            if (PaymentRulePO != null && PaymentRulePO.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRulePO = PaymentRulePO.Substring(0, 1);
            }
            Set_ValueNoCheck("PaymentRulePO", PaymentRulePO);
        }
        /** Get Payment Rule.
        @return Purchase payment option */
        public String GetPaymentRulePO()
        {
            return (String)Get_Value("PaymentRulePO");
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
            Set_ValueNoCheck("Phone", Phone);
        }
        /** Get Phone.
        @return Identifies a telephone number */
        public String GetPhone()
        {
            return (String)Get_Value("Phone");
        }
        /** Set 2nd Phone.
        @param Phone2 Identifies an alternate telephone number. */
        public void SetPhone2(String Phone2)
        {
            if (Phone2 != null && Phone2.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Phone2 = Phone2.Substring(0, 40);
            }
            Set_ValueNoCheck("Phone2", Phone2);
        }
        /** Get 2nd Phone.
        @return Identifies an alternate telephone number. */
        public String GetPhone2()
        {
            return (String)Get_Value("Phone2");
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
            Set_ValueNoCheck("Postal", Postal);
        }
        /** Get ZIP.
        @return Postal code */
        public String GetPostal()
        {
            return (String)Get_Value("Postal");
        }
        /** Set Potential Life Time Value.
        @param PotentialLifeTimeValue Total Revenue expected */
        public void SetPotentialLifeTimeValue(Decimal? PotentialLifeTimeValue)
        {
            Set_ValueNoCheck("PotentialLifeTimeValue", (Decimal?)PotentialLifeTimeValue);
        }
        /** Get Potential Life Time Value.
        @return Total Revenue expected */
        public Decimal GetPotentialLifeTimeValue()
        {
            Object bd = Get_Value("PotentialLifeTimeValue");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Rating.
        @param Rating Classification or Importance */
        public void SetRating(String Rating)
        {
            if (Rating != null && Rating.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Rating = Rating.Substring(0, 1);
            }
            Set_ValueNoCheck("Rating", Rating);
        }
        /** Get Rating.
        @return Classification or Importance */
        public String GetRating()
        {
            return (String)Get_Value("Rating");
        }
        /** Set Reference No.
        @param ReferenceNo Your customer or vendor number at the Business Partner's site */
        public void SetReferenceNo(String ReferenceNo)
        {
            if (ReferenceNo != null && ReferenceNo.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                ReferenceNo = ReferenceNo.Substring(0, 40);
            }
            Set_ValueNoCheck("ReferenceNo", ReferenceNo);
        }
        /** Get Reference No.
        @return Your customer or vendor number at the Business Partner's site */
        public String GetReferenceNo()
        {
            return (String)Get_Value("ReferenceNo");
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
            Set_ValueNoCheck("RegionName", RegionName);
        }
        /** Get Region.
        @return Name of the Region */
        public String GetRegionName()
        {
            return (String)Get_Value("RegionName");
        }

        /** SOCreditStatus AD_Reference_ID=289 */
        public static int SOCREDITSTATUS_AD_Reference_ID = 289;
        /** Credit Hold = H */
        public static String SOCREDITSTATUS_CreditHold = "H";
        /** Credit OK = O */
        public static String SOCREDITSTATUS_CreditOK = "O";
        /** Credit Stop = S */
        public static String SOCREDITSTATUS_CreditStop = "S";
        /** Credit Watch = W */
        public static String SOCREDITSTATUS_CreditWatch = "W";
        /** No Credit Check = X */
        public static String SOCREDITSTATUS_NoCreditCheck = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSOCreditStatusValid(String test)
        {
            return test == null || test.Equals("H") || test.Equals("O") || test.Equals("S") || test.Equals("W") || test.Equals("X");
        }
        /** Set Credit Status.
        @param SOCreditStatus Business Partner Credit Status */
        public void SetSOCreditStatus(String SOCreditStatus)
        {
            if (!IsSOCreditStatusValid(SOCreditStatus))
                throw new ArgumentException("SOCreditStatus Invalid value - " + SOCreditStatus + " - Reference_ID=289 - H - O - S - W - X");
            if (SOCreditStatus != null && SOCreditStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SOCreditStatus = SOCreditStatus.Substring(0, 1);
            }
            Set_ValueNoCheck("SOCreditStatus", SOCreditStatus);
        }
        /** Get Credit Status.
        @return Business Partner Credit Status */
        public String GetSOCreditStatus()
        {
            return (String)Get_Value("SOCreditStatus");
        }
        /** Set Credit Available.
        @param SO_CreditAvailable Available Credit based on Credit Limit (not Total Open Balance) and Credit Used */
        public void SetSO_CreditAvailable(Decimal? SO_CreditAvailable)
        {
            Set_ValueNoCheck("SO_CreditAvailable", (Decimal?)SO_CreditAvailable);
        }
        /** Get Credit Available.
        @return Available Credit based on Credit Limit (not Total Open Balance) and Credit Used */
        public Decimal GetSO_CreditAvailable()
        {
            Object bd = Get_Value("SO_CreditAvailable");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Credit Limit.
        @param SO_CreditLimit Total outstanding invoice amounts allowed */
        public void SetSO_CreditLimit(Decimal? SO_CreditLimit)
        {
            Set_ValueNoCheck("SO_CreditLimit", (Decimal?)SO_CreditLimit);
        }
        /** Get Credit Limit.
        @return Total outstanding invoice amounts allowed */
        public Decimal GetSO_CreditLimit()
        {
            Object bd = Get_Value("SO_CreditLimit");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Credit Used.
        @param SO_CreditUsed Current open balance */
        public void SetSO_CreditUsed(Decimal? SO_CreditUsed)
        {
            Set_ValueNoCheck("SO_CreditUsed", (Decimal?)SO_CreditUsed);
        }
        /** Get Credit Used.
        @return Current open balance */
        public Decimal GetSO_CreditUsed()
        {
            Object bd = Get_Value("SO_CreditUsed");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Order Description.
        @param SO_Description Description to be used on orders */
        public void SetSO_Description(String SO_Description)
        {
            if (SO_Description != null && SO_Description.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                SO_Description = SO_Description.Substring(0, 255);
            }
            Set_ValueNoCheck("SO_Description", SO_Description);
        }
        /** Get Order Description.
        @return Description to be used on orders */
        public String GetSO_Description()
        {
            return (String)Get_Value("SO_Description");
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Representative.
        @param SalesRep_ID Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_ValueNoCheck("SalesRep_ID", null);
            else
                Set_ValueNoCheck("SalesRep_ID", SalesRep_ID);
        }
        /** Get Representative.
        @return Company Agent like Sales Representitive, Purchase Agent, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sales Volume.
        @param SalesVolume Total Volume of Sales in Thousands of Base Currency */
        public void SetSalesVolume(Decimal? SalesVolume)
        {
            Set_ValueNoCheck("SalesVolume", (Decimal?)SalesVolume);
        }
        /** Get Sales Volume.
        @return Total Volume of Sales in Thousands of Base Currency */
        public Decimal GetSalesVolume()
        {
            Object bd = Get_Value("SalesVolume");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Send EMail.
        @param SendEMail Enable sending Document EMail */
        public void SetSendEMail(Boolean SendEMail)
        {
            Set_ValueNoCheck("SendEMail", SendEMail);
        }
        /** Get Send EMail.
        @return Enable sending Document EMail */
        public Boolean IsSendEMail()
        {
            Object oo = Get_Value("SendEMail");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Share.
        @param ShareOfCustomer Share of Customer's business as a percentage */
        public void SetShareOfCustomer(int ShareOfCustomer)
        {
            Set_ValueNoCheck("ShareOfCustomer", ShareOfCustomer);
        }
        /** Get Share.
        @return Share of Customer's business as a percentage */
        public int GetShareOfCustomer()
        {
            Object ii = Get_Value("ShareOfCustomer");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Min Shelf Life %.
        @param ShelfLifeMinPct Minimum Shelf Life in percent based on Product Instance Guarantee Date */
        public void SetShelfLifeMinPct(int ShelfLifeMinPct)
        {
            Set_ValueNoCheck("ShelfLifeMinPct", ShelfLifeMinPct);
        }
        /** Get Min Shelf Life %.
        @return Minimum Shelf Life in percent based on Product Instance Guarantee Date */
        public int GetShelfLifeMinPct()
        {
            Object ii = Get_Value("ShelfLifeMinPct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Supervisor_ID AD_Reference_ID=110 */
        public static int SUPERVISOR_ID_AD_Reference_ID = 110;
        /** Set Supervisor.
        @param Supervisor_ID Supervisor for this user/organization - used for escalation and approval */
        public void SetSupervisor_ID(int Supervisor_ID)
        {
            if (Supervisor_ID <= 0) Set_ValueNoCheck("Supervisor_ID", null);
            else
                Set_ValueNoCheck("Supervisor_ID", Supervisor_ID);
        }
        /** Get Supervisor.
        @return Supervisor for this user/organization - used for escalation and approval */
        public int GetSupervisor_ID()
        {
            Object ii = Get_Value("Supervisor_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax ID.
        @param TaxID Tax Identification */
        public void SetTaxID(String TaxID)
        {
            if (TaxID != null && TaxID.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                TaxID = TaxID.Substring(0, 20);
            }
            Set_ValueNoCheck("TaxID", TaxID);
        }
        /** Get Tax ID.
        @return Tax Identification */
        public String GetTaxID()
        {
            return (String)Get_Value("TaxID");
        }
        /** Set Title.
        @param Title Title of the Contact */
        public void SetTitle(String Title)
        {
            if (Title != null && Title.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                Title = Title.Substring(0, 40);
            }
            Set_ValueNoCheck("Title", Title);
        }
        /** Get Title.
        @return Title of the Contact */
        public String GetTitle()
        {
            return (String)Get_Value("Title");
        }
        /** Set Open Balance.
        @param TotalOpenBalance Total Open Balance Amount in primary Accounting Currency */
        public void SetTotalOpenBalance(Decimal? TotalOpenBalance)
        {
            Set_ValueNoCheck("TotalOpenBalance", (Decimal?)TotalOpenBalance);
        }
        /** Get Open Balance.
        @return Total Open Balance Amount in primary Accounting Currency */
        public Decimal GetTotalOpenBalance()
        {
            Object bd = Get_Value("TotalOpenBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set URL.
        @param URL Full URL address - e.g. http://www.viennaadvantage.com */
        public void SetURL(String URL)
        {
            if (URL != null && URL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                URL = URL.Substring(0, 120);
            }
            Set_ValueNoCheck("URL", URL);
        }
        /** Get URL.
        @return Full URL address - e.g. http://www.viennaadvantage.com */
        public String GetURL()
        {
            return (String)Get_Value("URL");
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
            Set_ValueNoCheck("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }
    }

}
