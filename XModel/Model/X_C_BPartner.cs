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
    /** Generated Model for C_BPartner
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_BPartner : PO
    {
        public X_C_BPartner(Context ctx, int C_BPartner_ID, Trx trxName)
            : base(ctx, C_BPartner_ID, trxName)
        {
            /** if (C_BPartner_ID == 0)
            {
            SetC_BP_Group_ID (0);
            SetC_BPartner_ID (0);
            SetEMPLOYEE_FILTER (false);	// N
            SetIsCustomer (false);
            SetIsEmployee (false);
            SetIsOneTime (false);
            SetIsProspect (false);
            SetIsSalesRep (false);
            SetIsSummary (false);
            SetIsVendor (false);
            SetName (null);
            SetSO_CreditLimit (0.0);
            SetSO_CreditUsed (0.0);
            SetSendEMail (false);
            SetVAHRUAE_SalaryType (null);	// MO
            SetValue (null);
            }
             */
        }
        public X_C_BPartner(Ctx ctx, int C_BPartner_ID, Trx trxName)
            : base(ctx, C_BPartner_ID, trxName)
        {
            /** if (C_BPartner_ID == 0)
            {
            SetC_BP_Group_ID (0);
            SetC_BPartner_ID (0);
            SetEMPLOYEE_FILTER (false);	// N
            SetIsCustomer (false);
            SetIsEmployee (false);
            SetIsOneTime (false);
            SetIsProspect (false);
            SetIsSalesRep (false);
            SetIsSummary (false);
            SetIsVendor (false);
            SetName (null);
            SetSO_CreditLimit (0.0);
            SetSO_CreditUsed (0.0);
            SetSendEMail (false);
            SetVAHRUAE_SalaryType (null);	// MO
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BPartner(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BPartner(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_BPartner(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_BPartner()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        static long serialVersionUID = 27721587398306L;
        /** Last Updated Timestamp 8/13/2015 4:04:42 PM */
        public static long updatedMS = 1439462081517L;
        /** AD_Table_ID=291 */
        public static int Table_ID;
        // =291;

        /** TableName=C_BPartner */
        public static String Table_Name = "C_BPartner";

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
            StringBuilder sb = new StringBuilder("X_C_BPartner[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** AD_Language AD_Reference_ID=327 */
        public static int AD_LANGUAGE_AD_Reference_ID = 327;
        /** Set Language.
        @param AD_Language Language for this entity */
        public void SetAD_Language(String AD_Language)
        {
            if (AD_Language != null && AD_Language.Length > 5)
            {
                log.Warning("Length > 5 - truncated");
                AD_Language = AD_Language.Substring(0, 5);
            }
            Set_Value("AD_Language", AD_Language);
        }
        /** Get Language.
        @return Language for this entity */
        public String GetAD_Language()
        {
            return (String)Get_Value("AD_Language");
        }

        /** AD_OrgBP_ID AD_Reference_ID=417 */
        public static int AD_ORGBP_ID_AD_Reference_ID = 417;
        /** Set Linked Organization.
        @param AD_OrgBP_ID The Business Partner is another Organization for explicit Inter-Org transactions */
        public void SetAD_OrgBP_ID(String AD_OrgBP_ID)
        {
            if (AD_OrgBP_ID != null && AD_OrgBP_ID.Length > 22)
            {
                log.Warning("Length > 22 - truncated");
                AD_OrgBP_ID = AD_OrgBP_ID.Substring(0, 22);
            }
            Set_Value("AD_OrgBP_ID", AD_OrgBP_ID);
        }
        /** Get Linked Organization.
        @return The Business Partner is another Organization for explicit Inter-Org transactions */
        public String GetAD_OrgBP_ID()
        {
            return (String)Get_Value("AD_OrgBP_ID");
        }
        /** Set Acquisition Cost.
        @param AcqusitionCost The cost of gaining the prospect as a customer */
        public void SetAcqusitionCost(Decimal? AcqusitionCost)
        {
            Set_Value("AcqusitionCost", (Decimal?)AcqusitionCost);
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
            Set_Value("ActualLifeTimeValue", (Decimal?)ActualLifeTimeValue);
        }
        /** Get Life Time Value.
        @return Actual Life Time Revenue */
        public Decimal GetActualLifeTimeValue()
        {
            Object bd = Get_Value("ActualLifeTimeValue");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set BPCODE.
        @param BPCODE BPCODE */
        public void SetBPCODE(String BPCODE)
        {
            if (BPCODE != null && BPCODE.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                BPCODE = BPCODE.Substring(0, 20);
            }
            Set_Value("BPCODE", BPCODE);
        }
        /** Get BPCODE.
        @return BPCODE */
        public String GetBPCODE()
        {
            return (String)Get_Value("BPCODE");
        }
        /** Set Partner Parent.
        @param BPartner_Parent_ID Business Partner Parent */
        public void SetBPartner_Parent_ID(int BPartner_Parent_ID)
        {
            if (BPartner_Parent_ID <= 0) Set_Value("BPartner_Parent_ID", null);
            else
                Set_Value("BPartner_Parent_ID", BPartner_Parent_ID);
        }
        /** Get Partner Parent.
        @return Business Partner Parent */
        public int GetBPartner_Parent_ID()
        {
            Object ii = Get_Value("BPartner_Parent_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Group.
        @param C_BP_Group_ID Customer/Prospect/Employee Group */
        public void SetC_BP_Group_ID(int C_BP_Group_ID)
        {
            if (C_BP_Group_ID < 1) throw new ArgumentException("C_BP_Group_ID is mandatory.");
            Set_Value("C_BP_Group_ID", C_BP_Group_ID);
        }
        /** Get Group.
        @return Customer/Prospect/Employee Group */
        public int GetC_BP_Group_ID()
        {
            Object ii = Get_Value("C_BP_Group_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BP Size.
        @param C_BP_Size_ID Business Partner Size */
        public void SetC_BP_Size_ID(int C_BP_Size_ID)
        {
            if (C_BP_Size_ID <= 0) Set_Value("C_BP_Size_ID", null);
            else
                Set_Value("C_BP_Size_ID", C_BP_Size_ID);
        }
        /** Get BP Size.
        @return Business Partner Size */
        public int GetC_BP_Size_ID()
        {
            Object ii = Get_Value("C_BP_Size_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BP Status.
        @param C_BP_Status_ID Business Partner Status */
        public void SetC_BP_Status_ID(int C_BP_Status_ID)
        {
            if (C_BP_Status_ID <= 0) Set_Value("C_BP_Status_ID", null);
            else
                Set_Value("C_BP_Status_ID", C_BP_Status_ID);
        }
        /** Get BP Status.
        @return Business Partner Status */
        public int GetC_BP_Status_ID()
        {
            Object ii = Get_Value("C_BP_Status_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Business Partner.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
            Set_ValueNoCheck("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Business Partner.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Consolidation Reference.
        @param C_CONSOLIDATIONREFERENCE_ID Consolidation Reference */
        public void SetC_CONSOLIDATIONREFERENCE_ID(int C_CONSOLIDATIONREFERENCE_ID)
        {
            if (C_CONSOLIDATIONREFERENCE_ID <= 0) Set_Value("C_CONSOLIDATIONREFERENCE_ID", null);
            else
                Set_Value("C_CONSOLIDATIONREFERENCE_ID", C_CONSOLIDATIONREFERENCE_ID);
        }
        /** Get Consolidation Reference.
        @return Consolidation Reference */
        public int GetC_CONSOLIDATIONREFERENCE_ID()
        {
            Object ii = Get_Value("C_CONSOLIDATIONREFERENCE_ID");
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
        @param C_Country_ID Country  */
        public void SetC_Country_ID(int C_Country_ID)
        {
            if (C_Country_ID <= 0) Set_Value("C_Country_ID", null);
            else
                Set_Value("C_Country_ID", C_Country_ID);
        }
        /** Get Country.
        @return Country  */
        public int GetC_Country_ID()
        {
            Object ii = Get_Value("C_Country_ID");
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
        /** Set Dunning.
        @param C_Dunning_ID Dunning Rules for overdue invoices */
        public void SetC_Dunning_ID(int C_Dunning_ID)
        {
            if (C_Dunning_ID <= 0) Set_Value("C_Dunning_ID", null);
            else
                Set_Value("C_Dunning_ID", C_Dunning_ID);
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
            if (C_Greeting_ID <= 0) Set_Value("C_Greeting_ID", null);
            else
                Set_Value("C_Greeting_ID", C_Greeting_ID);
        }
        /** Get Greeting.
        @return Greeting to print on correspondence */
        public int GetC_Greeting_ID()
        {
            Object ii = Get_Value("C_Greeting_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Industry Code.
        @param C_IndustryCode_ID Partner's Industry Classification */
        public void SetC_IndustryCode_ID(int C_IndustryCode_ID)
        {
            if (C_IndustryCode_ID <= 0) Set_Value("C_IndustryCode_ID", null);
            else
                Set_Value("C_IndustryCode_ID", C_IndustryCode_ID);
        }
        /** Get Industry Code.
        @return Partner's Industry Classification */
        public int GetC_IndustryCode_ID()
        {
            Object ii = Get_Value("C_IndustryCode_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Schedule.
        @param C_InvoiceSchedule_ID Schedule for generating Invoices */
        public void SetC_InvoiceSchedule_ID(int C_InvoiceSchedule_ID)
        {
            if (C_InvoiceSchedule_ID <= 0) Set_Value("C_InvoiceSchedule_ID", null);
            else
                Set_Value("C_InvoiceSchedule_ID", C_InvoiceSchedule_ID);
        }
        /** Get Invoice Schedule.
        @return Schedule for generating Invoices */
        public int GetC_InvoiceSchedule_ID()
        {
            Object ii = Get_Value("C_InvoiceSchedule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Position Category.
        @param C_JobCategory_ID Job Position Category */
        public void SetC_JobCategory_ID(int C_JobCategory_ID)
        {
            if (C_JobCategory_ID <= 0) Set_Value("C_JobCategory_ID", null);
            else
                Set_Value("C_JobCategory_ID", C_JobCategory_ID);
        }
        /** Get Position Category.
        @return Job Position Category */
        public int GetC_JobCategory_ID()
        {
            Object ii = Get_Value("C_JobCategory_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Position.
        @param C_Job_ID Job Position */
        public void SetC_Job_ID(int C_Job_ID)
        {
            if (C_Job_ID <= 0) Set_Value("C_Job_ID", null);
            else
                Set_Value("C_Job_ID", C_Job_ID);
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
        /** Set City.
        @param City Identifies a City */
        public void SetCity(String City)
        {
            if (City != null && City.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                City = City.Substring(0, 50);
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
            if (ContactName != null && ContactName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ContactName = ContactName.Substring(0, 50);
            }
            Set_Value("ContactName", ContactName);
        }
        /** Get Contact Name.
        @return Business Partner Contact Name */
        public String GetContactName()
        {
            return (String)Get_Value("ContactName");
        }
        /** Set Add To Target List.
        @param CreateTargetList Add your customers to campaign target list */
        public void SetCreateTargetList(String CreateTargetList)
        {
            if (CreateTargetList != null && CreateTargetList.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                CreateTargetList = CreateTargetList.Substring(0, 50);
            }
            Set_Value("CreateTargetList", CreateTargetList);
        }
        /** Get Add To Target List.
        @return Add your customers to campaign target list */
        public String GetCreateTargetList()
        {
            return (String)Get_Value("CreateTargetList");
        }
        /** CreditStatusSettingOn AD_Reference_ID=1000178 */
        public static int CREDITSTATUSSETTINGON_AD_Reference_ID = 1000178;
        /** Customer Header = CH */
        public static String CREDITSTATUSSETTINGON_CustomerHeader = "CH";
        /** Customer Location = CL */
        public static String CREDITSTATUSSETTINGON_CustomerLocation = "CL";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCreditStatusSettingOnValid(String test)
        {
            return test == null || test.Equals("CH") || test.Equals("CL");
        }
        /** Set Credit Status Setting On.
        @param CreditStatusSettingOn Credit Status Setting on represents that on which level needs to define the credit details of the customer. */
        public void SetCreditStatusSettingOn(String CreditStatusSettingOn)
        {
            if (!IsCreditStatusSettingOnValid(CreditStatusSettingOn))
                throw new ArgumentException("CreditStatusSettingOn Invalid value - " + CreditStatusSettingOn + " - Reference_ID=1000178 - CH - CL");
            if (CreditStatusSettingOn != null && CreditStatusSettingOn.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                CreditStatusSettingOn = CreditStatusSettingOn.Substring(0, 2);
            }
            Set_Value("CreditStatusSettingOn", CreditStatusSettingOn);
        }
        /** Get Credit Status Setting On.
        @return Credit Status Setting on represents that on which level needs to define the credit details of the customer. */
        public String GetCreditStatusSettingOn()
        {
            return (String)Get_Value("CreditStatusSettingOn");
        }

        /** CreditValidation AD_Reference_ID=1000179 */
        public static int CREDITVALIDATION_AD_Reference_ID = 1000179;
        /** Stop SO = A */
        public static String CREDITVALIDATION_StopSO = "A";
        /** Stop Shipment = B */
        public static String CREDITVALIDATION_StopShipment = "B";
        /** Stop Invoice = C */
        public static String CREDITVALIDATION_StopInvoice = "C";
        /** Stop All = D */
        public static String CREDITVALIDATION_StopAll = "D";
        /** Stop SO & Shipment = E */
        public static String CREDITVALIDATION_StopSOShipment = "E";
        /** Stop Shipment & Invoice = F */
        public static String CREDITVALIDATION_StopShipmentInvoice = "F";
        /** Warning on SO = G */
        public static String CREDITVALIDATION_WarningOnSO = "G";
        /** Warning on Shipment = H */
        public static String CREDITVALIDATION_WarningOnShipment = "H";
        /** Warning on Invoice = I */
        public static String CREDITVALIDATION_WarningOnInvoice = "I";
        /** Warning on All = J */
        public static String CREDITVALIDATION_WarningOnAll = "J";
        /** Warning on SO & Shipment = K */
        public static String CREDITVALIDATION_WarningOnSOShipment = "K";
        /** Warning on Shipment & Invoice = L */
        public static String CREDITVALIDATION_WarningOnShipmentInvoice = "L";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsCreditValidationValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("B") || test.Equals("C") || test.Equals("D") || test.Equals("E") || test.Equals("F") || test.Equals("G") || test.Equals("H") || test.Equals("I") || test.Equals("J") || test.Equals("K") || test.Equals("L");
        }
        /** Set Credit Validation.
        @param CreditValidation Credit Validation field indicates to raise either the warning or to stop generating the transactions (Sales Order & Invoices) */
        public void SetCreditValidation(String CreditValidation)
        {
            if (!IsCreditValidationValid(CreditValidation))
                throw new ArgumentException("CreditValidation Invalid value - " + CreditValidation + " - Reference_ID=1000179 - A - B - C - D - E - F - G - H - I - J - K - L");
            if (CreditValidation != null && CreditValidation.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreditValidation = CreditValidation.Substring(0, 1);
            }
            Set_Value("CreditValidation", CreditValidation);
        }
        /** Get Credit Validation.
        @return Credit Validation field indicates to raise either the warning or to stop generating the transactions (Sales Order & Invoices) */
        public String GetCreditValidation()
        {
            return (String)Get_Value("CreditValidation");
        }
        /** Set D-U-N-S.
        @param DUNS Creditor Check (Dun & Bradstreet) Number */
        public void SetDUNS(String DUNS)
        {
            if (DUNS != null && DUNS.Length > 12)
            {
                log.Warning("Length > 12 - truncated");
                DUNS = DUNS.Substring(0, 12);
            }
            Set_Value("DUNS", DUNS);
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
            Set_Value("DeliveryRule", DeliveryRule);
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
            Set_Value("DeliveryViaRule", DeliveryViaRule);
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
            Set_Value("Description", Description);
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
            Set_Value("DocumentCopies", DocumentCopies);
        }
        /** Get Document Copies.
        @return Number of copies to be printed */
        public int GetDocumentCopies()
        {
            Object ii = Get_Value("DocumentCopies");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set EMPLOYEE_FILTER.
        @param EMPLOYEE_FILTER EMPLOYEE_FILTER */
        public void SetEMPLOYEE_FILTER(Boolean EMPLOYEE_FILTER)
        {
            Set_Value("EMPLOYEE_FILTER", EMPLOYEE_FILTER);
        }
        /** Get EMPLOYEE_FILTER.
        @return EMPLOYEE_FILTER */
        public Boolean IsEMPLOYEE_FILTER()
        {
            Object oo = Get_Value("EMPLOYEE_FILTER");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set EMPLOYEE_GRADE.
        @param EMPLOYEE_GRADE EMPLOYEE_GRADE */
        public void SetEMPLOYEE_GRADE(String EMPLOYEE_GRADE)
        {
            if (EMPLOYEE_GRADE != null && EMPLOYEE_GRADE.Length > 5)
            {
                log.Warning("Length > 5 - truncated");
                EMPLOYEE_GRADE = EMPLOYEE_GRADE.Substring(0, 5);
            }
            Set_Value("EMPLOYEE_GRADE", EMPLOYEE_GRADE);
        }
        /** Get EMPLOYEE_GRADE.
        @return EMPLOYEE_GRADE */
        public String GetEMPLOYEE_GRADE()
        {
            return (String)Get_Value("EMPLOYEE_GRADE");
        }

        /** EMPLOYEE_STATUS AD_Reference_ID=1000009 */
        public static int EMPLOYEE_STATUS_AD_Reference_ID = 1000009;
        /** Permanent = P */
        public static String EMPLOYEE_STATUS_Permanent = "P";
        /** Temporary = T */
        public static String EMPLOYEE_STATUS_Temporary = "T";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsEMPLOYEE_STATUSValid(String test)
        {
            return test == null || test.Equals("P") || test.Equals("T");
        }
        /** Set Employee Status.
        @param EMPLOYEE_STATUS Employee Status */
        public void SetEMPLOYEE_STATUS(String EMPLOYEE_STATUS)
        {
            if (!IsEMPLOYEE_STATUSValid(EMPLOYEE_STATUS))
                throw new ArgumentException("EMPLOYEE_STATUS Invalid value - " + EMPLOYEE_STATUS + " - Reference_ID=1000009 - P - T");
            if (EMPLOYEE_STATUS != null && EMPLOYEE_STATUS.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                EMPLOYEE_STATUS = EMPLOYEE_STATUS.Substring(0, 1);
            }
            Set_Value("EMPLOYEE_STATUS", EMPLOYEE_STATUS);
        }
        /** Get Employee Status.
        @return Employee Status */
        public String GetEMPLOYEE_STATUS()
        {
            return (String)Get_Value("EMPLOYEE_STATUS");
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
        /** Set Enroll_ID.
        @param Enroll_ID Enroll_ID */
        public void SetEnroll_ID(int Enroll_ID)
        {
            if (Enroll_ID <= 0) Set_Value("Enroll_ID", null);
            else
                Set_Value("Enroll_ID", Enroll_ID);
        }
        /** Get Enroll_ID.
        @return Enroll_ID */
        public int GetEnroll_ID()
        {
            Object ii = Get_Value("Enroll_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Fax.
        @param Fax Facsimile number */
        public void SetFax(String Fax)
        {
            if (Fax != null && Fax.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Fax = Fax.Substring(0, 50);
            }
            Set_Value("Fax", Fax);
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
            Set_Value("FirstSale", (DateTime?)FirstSale);
        }
        /** Get First Sale.
        @return Date of First Sale */
        public DateTime? GetFirstSale()
        {
            return (DateTime?)Get_Value("FirstSale");
        }
        /** Set Flat Discount %.
        @param FlatDiscount Flat discount percentage  */
        public void SetFlatDiscount(Decimal? FlatDiscount)
        {
            Set_Value("FlatDiscount", (Decimal?)FlatDiscount);
        }
        /** Get Flat Discount %.
        @return Flat discount percentage  */
        public Decimal GetFlatDiscount()
        {
            Object bd = Get_Value("FlatDiscount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Get Apply Discount Calculation.
       @return Apply Discount Calculation */
        public String GetED007_DiscountCalculation()
        {
            return (String)Get_Value("ED007_DiscountCalculation");
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
        /** Freight Excluded = X */
        public static String FREIGHTCOSTRULE_FreightExcluded = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsFreightCostRuleValid(String test)
        {
            return test == null || test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L") || test.Equals("X");
        }
        /** Set Freight Cost Rule.
        @param FreightCostRule Method for charging Freight */
        public void SetFreightCostRule(String FreightCostRule)
        {
            if (!IsFreightCostRuleValid(FreightCostRule))
                throw new ArgumentException("FreightCostRule Invalid value - " + FreightCostRule + " - Reference_ID=153 - C - F - I - L - X");
            if (FreightCostRule != null && FreightCostRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                FreightCostRule = FreightCostRule.Substring(0, 1);
            }
            Set_Value("FreightCostRule", FreightCostRule);
        }
        /** Get Freight Cost Rule.
        @return Method for charging Freight */
        public String GetFreightCostRule()
        {
            return (String)Get_Value("FreightCostRule");
        }

        /** Gender AD_Reference_ID=1000020 */
        public static int GENDER_AD_Reference_ID = 1000020;
        /** Female = F */
        public static String GENDER_Female = "F";
        /** Male = M */
        public static String GENDER_Male = "M";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsGenderValid(String test)
        {
            return test == null || test.Equals("F") || test.Equals("M");
        }
        /** Set Gender.
        @param Gender Gender */
        public void SetGender(String Gender)
        {
            if (!IsGenderValid(Gender))
                throw new ArgumentException("Gender Invalid value - " + Gender + " - Reference_ID=1000020 - F - M");
            if (Gender != null && Gender.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Gender = Gender.Substring(0, 1);
            }
            Set_Value("Gender", Gender);
        }
        /** Get Gender.
        @return Gender */
        public String GetGender()
        {
            return (String)Get_Value("Gender");
        }
        /** Set Generate Customer.
        @param GenerateAccount Convert prospects to customers */
        public void SetGenerateAccount(String GenerateAccount)
        {
            if (GenerateAccount != null && GenerateAccount.Length > 22)
            {
                log.Warning("Length > 22 - truncated");
                GenerateAccount = GenerateAccount.Substring(0, 22);
            }
            Set_Value("GenerateAccount", GenerateAccount);
        }
        /** Get Generate Customer.
        @return Convert prospects to customers */
        public String GetGenerateAccount()
        {
            return (String)Get_Value("GenerateAccount");
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
        @param InvoiceRule Frequency and method of invoicing  */
        public void SetInvoiceRule(String InvoiceRule)
        {
            if (!IsInvoiceRuleValid(InvoiceRule))
                throw new ArgumentException("InvoiceRule Invalid value - " + InvoiceRule + " - Reference_ID=150 - D - I - O - S");
            if (InvoiceRule != null && InvoiceRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                InvoiceRule = InvoiceRule.Substring(0, 1);
            }
            Set_Value("InvoiceRule", InvoiceRule);
        }
        /** Get Invoicing Rule.
        @return Frequency and method of invoicing  */
        public String GetInvoiceRule()
        {
            return (String)Get_Value("InvoiceRule");
        }

        /** Invoice_PrintFormat_ID AD_Reference_ID=261 */
        public static int INVOICE_PRINTFORMAT_ID_AD_Reference_ID = 261;
        /** Set Invoice Print Format.
        @param Invoice_PrintFormat_ID Print Format for printing Invoices */
        public void SetInvoice_PrintFormat_ID(int Invoice_PrintFormat_ID)
        {
            if (Invoice_PrintFormat_ID <= 0) Set_Value("Invoice_PrintFormat_ID", null);
            else
                Set_Value("Invoice_PrintFormat_ID", Invoice_PrintFormat_ID);
        }
        /** Get Invoice Print Format.
        @return Print Format for printing Invoices */
        public int GetInvoice_PrintFormat_ID()
        {
            Object ii = Get_Value("Invoice_PrintFormat_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Credit Watch %.
@param CreditWatchPercent Credit Watch - Percent of Credit Limit when OK switches to Watch */
        public void SetCreditWatchPercent(Decimal? CreditWatchPercent) { Set_Value("CreditWatchPercent", (Decimal?)CreditWatchPercent); }/** Get Credit Watch %.
@return Credit Watch - Percent of Credit Limit when OK switches to Watch */
        public Decimal GetCreditWatchPercent() { Object bd = Get_Value("CreditWatchPercent"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Customer.
        @param IsCustomer Indicates if this Business Partner is a Customer */
        public void SetIsCustomer(Boolean IsCustomer)
        {
            Set_Value("IsCustomer", IsCustomer);
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
            Set_Value("IsDiscountPrinted", IsDiscountPrinted);
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
            Set_Value("IsEmployee", IsEmployee);
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
        /** Set One time transaction.
        @param IsOneTime One time transaction */
        public void SetIsOneTime(Boolean IsOneTime)
        {
            Set_Value("IsOneTime", IsOneTime);
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
            Set_Value("IsProspect", IsProspect);
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
            Set_Value("IsSalesRep", IsSalesRep);
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
            Set_Value("IsSummary", IsSummary);
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
            Set_Value("IsTaxExempt", IsTaxExempt);
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
            Set_Value("IsVendor", IsVendor);
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
        /** Set Last Gmail Updated.
        @param LastGmailUpdated Last Gmail Updated */
        public void SetLastGmailUpdated(DateTime? LastGmailUpdated)
        {
            Set_Value("LastGmailUpdated", (DateTime?)LastGmailUpdated);
        }
        /** Get Last Gmail Updated.
        @return Last Gmail Updated */
        public DateTime? GetLastGmailUpdated()
        {
            return (DateTime?)Get_Value("LastGmailUpdated");
        }
        /** Set Last Local Updated.
        @param LastLocalUpdated Last Local Updated */
        public void SetLastLocalUpdated(DateTime? LastLocalUpdated)
        {
            Set_Value("LastLocalUpdated", (DateTime?)LastLocalUpdated);
        }
        /** Get Last Local Updated.
        @return Last Local Updated */
        public DateTime? GetLastLocalUpdated()
        {
            return (DateTime?)Get_Value("LastLocalUpdated");
        }

        /** M_DiscountSchema_ID AD_Reference_ID=325 */
        public static int M_DISCOUNTSCHEMA_ID_AD_Reference_ID = 325;
        /** Set Discount Schema.
        @param M_DiscountSchema_ID Schema to calculate price lists or the trade discount percentage */
        public void SetM_DiscountSchema_ID(int M_DiscountSchema_ID)
        {
            if (M_DiscountSchema_ID <= 0) Set_Value("M_DiscountSchema_ID", null);
            else
                Set_Value("M_DiscountSchema_ID", M_DiscountSchema_ID);
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
        /** Set Return Policy.
        @param M_ReturnPolicy_ID The Return Policy dictates the timeframe within which goods can be returned. */
        public void SetM_ReturnPolicy_ID(int M_ReturnPolicy_ID)
        {
            if (M_ReturnPolicy_ID <= 0) Set_Value("M_ReturnPolicy_ID", null);
            else
                Set_Value("M_ReturnPolicy_ID", M_ReturnPolicy_ID);
        }
        /** Get Return Policy.
        @return The Return Policy dictates the timeframe within which goods can be returned. */
        public int GetM_ReturnPolicy_ID()
        {
            Object ii = Get_Value("M_ReturnPolicy_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** MaritalStatus AD_Reference_ID=1000021 */
        public static int MARITALSTATUS_AD_Reference_ID = 1000021;
        /** Married = M */
        public static String MARITALSTATUS_Married = "M";
        /** Single = S */
        public static String MARITALSTATUS_Single = "S";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMaritalStatusValid(String test)
        {
            return test == null || test.Equals("M") || test.Equals("S");
        }
        /** Set MaritalStatus.
        @param MaritalStatus MaritalStatus */
        public void SetMaritalStatus(String MaritalStatus)
        {
            if (!IsMaritalStatusValid(MaritalStatus))
                throw new ArgumentException("MaritalStatus Invalid value - " + MaritalStatus + " - Reference_ID=1000021 - M - S");
            if (MaritalStatus != null && MaritalStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MaritalStatus = MaritalStatus.Substring(0, 1);
            }
            Set_Value("MaritalStatus", MaritalStatus);
        }
        /** Get MaritalStatus.
        @return MaritalStatus */
        public String GetMaritalStatus()
        {
            return (String)Get_Value("MaritalStatus");
        }
        /** Set Mobile.
        @param Mobile Identifies the mobile number. */
        public void SetMobile(String Mobile)
        {
            if (Mobile != null && Mobile.Length > 25)
            {
                log.Warning("Length > 25 - truncated");
                Mobile = Mobile.Substring(0, 25);
            }
            Set_Value("Mobile", Mobile);
        }
        /** Get Mobile.
        @return Identifies the mobile number. */
        public String GetMobile()
        {
            return (String)Get_Value("Mobile");
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
            Set_Value("NAICS", NAICS);
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
            if (Name.Length > 80)
            {
                log.Warning("Length > 80 - truncated");
                Name = Name.Substring(0, 80);
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
        /** Set Name 2.
        @param Name2 Additional Name */
        public void SetName2(String Name2)
        {
            if (Name2 != null && Name2.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                Name2 = Name2.Substring(0, 60);
            }
            Set_Value("Name2", Name2);
        }
        /** Get Name 2.
        @return Additional Name */
        public String GetName2()
        {
            return (String)Get_Value("Name2");
        }
        /** Set Nationality.
        @param Nationality Nationality */
        public void SetNationality(String Nationality)
        {
            if (Nationality != null && Nationality.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Nationality = Nationality.Substring(0, 50);
            }
            Set_Value("Nationality", Nationality);
        }
        /** Get Nationality.
        @return Nationality */
        public String GetNationality()
        {
            return (String)Get_Value("Nationality");
        }
        /** Set Employees.
        @param NumberEmployees Number of employees */
        public void SetNumberEmployees(int NumberEmployees)
        {
            Set_Value("NumberEmployees", NumberEmployees);
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
        @param POReference Transaction Reference Number  of your Customer/Prospect */
        public void SetPOReference(String POReference)
        {
            if (POReference != null && POReference.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                POReference = POReference.Substring(0, 20);
            }
            Set_Value("POReference", POReference);
        }
        /** Get Order Reference.
        @return Transaction Reference Number  of your Customer/Prospect */
        public String GetPOReference()
        {
            return (String)Get_Value("POReference");
        }

        /** PO_DiscountSchema_ID AD_Reference_ID=325 */
        public static int PO_DISCOUNTSCHEMA_ID_AD_Reference_ID = 325;
        /** Set PO Discount Schema.
        @param PO_DiscountSchema_ID Schema to calculate the purchase trade discount percentage */
        public void SetPO_DiscountSchema_ID(int PO_DiscountSchema_ID)
        {
            if (PO_DiscountSchema_ID <= 0) Set_Value("PO_DiscountSchema_ID", null);
            else
                Set_Value("PO_DiscountSchema_ID", PO_DiscountSchema_ID);
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
            if (PO_PaymentTerm_ID <= 0) Set_Value("PO_PaymentTerm_ID", null);
            else
                Set_Value("PO_PaymentTerm_ID", PO_PaymentTerm_ID);
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
            if (PO_PriceList_ID <= 0) Set_Value("PO_PriceList_ID", null);
            else
                Set_Value("PO_PriceList_ID", PO_PriceList_ID);
        }
        /** Get Purchase Pricelist.
        @return Price List used by this Business Partner */
        public int GetPO_PriceList_ID()
        {
            Object ii = Get_Value("PO_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PO_ReturnPolicy_ID AD_Reference_ID=431 */
        public static int PO_RETURNPOLICY_ID_AD_Reference_ID = 431;
        /** Set Vendor Return Policy.
        @param PO_ReturnPolicy_ID Vendor Return Policy */
        public void SetPO_ReturnPolicy_ID(int PO_ReturnPolicy_ID)
        {
            if (PO_ReturnPolicy_ID <= 0) Set_Value("PO_ReturnPolicy_ID", null);
            else
                Set_Value("PO_ReturnPolicy_ID", PO_ReturnPolicy_ID);
        }
        /** Get Vendor Return Policy.
        @return Vendor Return Policy */
        public int GetPO_ReturnPolicy_ID()
        {
            Object ii = Get_Value("PO_ReturnPolicy_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** PaymentRule AD_Reference_ID=195 */
        public static int PAYMENTRULE_AD_Reference_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULE_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULE_DirectDebit = "D";
        /** Letter of Credit = L */
        public static String PAYMENTRULE_LetterOfCredit = "L";
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
            return test == null || test.Equals("B") || test.Equals("D") || test.Equals("L") || test.Equals("K") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Method.
        @param PaymentRule How you pay the invoice */
        public void SetPaymentRule(String PaymentRule)
        {
            //if (!IsPaymentRuleValid(PaymentRule))
            //   throw new ArgumentException("PaymentRule Invalid value - " + PaymentRule + " - Reference_ID=195 - B - D - K - L - P - S - T");
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

        /** PaymentRulePO AD_Reference_ID=195 */
        public static int PAYMENTRULEPO_AD_Reference_ID = 195;
        /** Cash = B */
        public static String PAYMENTRULEPO_Cash = "B";
        /** Direct Debit = D */
        public static String PAYMENTRULEPO_DirectDebit = "D";
        /** Credit Card = K */
        public static String PAYMENTRULEPO_CreditCard = "K";
        /** Letter of Credit = L */
        public static String PAYMENTRULEPO_LetterOfCredit = "L";
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
            return test == null || test.Equals("B") || test.Equals("D") || test.Equals("K") || test.Equals("L") || test.Equals("P") || test.Equals("S") || test.Equals("T");
        }
        /** Set Payment Rule.
        @param PaymentRulePO Purchase payment option */
        public void SetPaymentRulePO(String PaymentRulePO)
        {
            //if (!IsPaymentRulePOValid(PaymentRulePO))
            //   throw new ArgumentException("PaymentRulePO Invalid value - " + PaymentRulePO + " - Reference_ID=195 - B - D - K - L - P - S - T");
            if (PaymentRulePO != null && PaymentRulePO.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PaymentRulePO = PaymentRulePO.Substring(0, 1);
            }
            Set_Value("PaymentRulePO", PaymentRulePO);
        }
        /** Get Payment Rule.
        @return Purchase payment option */
        public String GetPaymentRulePO()
        {
            return (String)Get_Value("PaymentRulePO");
        }
        /** Set Pic.
        @param Pic Pic */
        public void SetPic(int Pic)
        {
            Set_Value("Pic", Pic);
        }
        /** Get Pic.
        @return Pic */
        public int GetPic()
        {
            Object ii = Get_Value("Pic");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Potential Life Time Value.
        @param PotentialLifeTimeValue Total Revenue expected */
        public void SetPotentialLifeTimeValue(Decimal? PotentialLifeTimeValue)
        {
            Set_Value("PotentialLifeTimeValue", (Decimal?)PotentialLifeTimeValue);
        }
        /** Get Potential Life Time Value.
        @return Total Revenue expected */
        public Decimal GetPotentialLifeTimeValue()
        {
            Object bd = Get_Value("PotentialLifeTimeValue");
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
        /** Set Add To Target List.
        @param ProspectTargetList Add prospect to your campaign target list */
        public void SetProspectTargetList(String ProspectTargetList)
        {
            if (ProspectTargetList != null && ProspectTargetList.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ProspectTargetList = ProspectTargetList.Substring(0, 50);
            }
            Set_Value("ProspectTargetList", ProspectTargetList);
        }
        /** Get Add To Target List.
        @return Add prospect to your campaign target list */
        public String GetProspectTargetList()
        {
            return (String)Get_Value("ProspectTargetList");
        }

        /** Rating AD_Reference_ID=419 */
        public static int RATING_AD_Reference_ID = 419;
        /** Not Rated = - */
        public static String RATING_NotRated = "-";
        /** A = A */
        public static String RATING_A = "A";
        /** B = B */
        public static String RATING_B = "B";
        /** C = C */
        public static String RATING_C = "C";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsRatingValid(String test)
        {
            return test == null || test.Equals("-") || test.Equals("A") || test.Equals("B") || test.Equals("C");
        }
        /** Set Rating.
        @param Rating Classification or Importance */
        public void SetRating(String Rating)
        {
            if (!IsRatingValid(Rating))
                throw new ArgumentException("Rating Invalid value - " + Rating + " - Reference_ID=419 - - - A - B - C");
            if (Rating != null && Rating.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Rating = Rating.Substring(0, 1);
            }
            Set_Value("Rating", Rating);
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
            Set_Value("ReferenceNo", ReferenceNo);
        }
        /** Get Reference No.
        @return Your customer or vendor number at the Business Partner's site */
        public String GetReferenceNo()
        {
            return (String)Get_Value("ReferenceNo");
        }

        /** SALARYPROCESS AD_Reference_ID=1000010 */
        public static int SALARYPROCESS_AD_Reference_ID = 1000010;
        /** Direct Deposit = A */
        public static String SALARYPROCESS_DirectDeposit = "A";
        /** Cash = C */
        public static String SALARYPROCESS_Cash = "C";
        /** Cheque = K */
        public static String SALARYPROCESS_Cheque = "K";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsSALARYPROCESSValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("C") || test.Equals("K");
        }
        /** Set Salary Process.
        @param SALARYPROCESS Salary Process */
        public void SetSALARYPROCESS(String SALARYPROCESS)
        {
            if (!IsSALARYPROCESSValid(SALARYPROCESS))
                throw new ArgumentException("SALARYPROCESS Invalid value - " + SALARYPROCESS + " - Reference_ID=1000010 - A - C - K");
            if (SALARYPROCESS != null && SALARYPROCESS.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SALARYPROCESS = SALARYPROCESS.Substring(0, 1);
            }
            Set_Value("SALARYPROCESS", SALARYPROCESS);
        }
        /** Get Salary Process.
        @return Salary Process */
        public String GetSALARYPROCESS()
        {
            return (String)Get_Value("SALARYPROCESS");
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
        @param SOCreditStatus Partner's Credit Status */
        public void SetSOCreditStatus(String SOCreditStatus)
        {
            if (!IsSOCreditStatusValid(SOCreditStatus))
                throw new ArgumentException("SOCreditStatus Invalid value - " + SOCreditStatus + " - Reference_ID=289 - H - O - S - W - X");
            if (SOCreditStatus != null && SOCreditStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SOCreditStatus = SOCreditStatus.Substring(0, 1);
            }
            Set_Value("SOCreditStatus", SOCreditStatus);
        }
        /** Get Credit Status.
        @return Partner's Credit Status */
        public String GetSOCreditStatus()
        {
            return (String)Get_Value("SOCreditStatus");
        }
        /** Set Credit Limit.
        @param SO_CreditLimit Total outstanding invoice amounts allowed */
        public void SetSO_CreditLimit(Decimal? SO_CreditLimit)
        {
            if (SO_CreditLimit == null) throw new ArgumentException("SO_CreditLimit is mandatory.");
            Set_Value("SO_CreditLimit", (Decimal?)SO_CreditLimit);
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
            if (SO_CreditUsed == null) throw new ArgumentException("SO_CreditUsed is mandatory.");
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
            Set_Value("SO_Description", SO_Description);
        }
        /** Get Order Description.
        @return Description to be used on orders */
        public String GetSO_Description()
        {
            return (String)Get_Value("SO_Description");
        }

        /** SalesRep_ID AD_Reference_ID=190 */
        public static int SALESREP_ID_AD_Reference_ID = 190;
        /** Set Sales Rep.
        @param SalesRep_ID Company Agent like Sales Representitive, Customer Service Representative, ... */
        public void SetSalesRep_ID(int SalesRep_ID)
        {
            if (SalesRep_ID <= 0) Set_Value("SalesRep_ID", null);
            else
                Set_Value("SalesRep_ID", SalesRep_ID);
        }
        /** Get Sales Rep.
        @return Company Agent like Sales Representitive, Customer Service Representative, ... */
        public int GetSalesRep_ID()
        {
            Object ii = Get_Value("SalesRep_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sales Volume.
        @param SalesVolume Total Volume of Sales in Thousands of Base Currency */
        public void SetSalesVolume(int SalesVolume)
        {
            Set_Value("SalesVolume", SalesVolume);
        }
        /** Get Sales Volume.
        @return Total Volume of Sales in Thousands of Base Currency */
        public int GetSalesVolume()
        {
            Object ii = Get_Value("SalesVolume");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Send EMail.
        @param SendEMail Enable sending Document EMail */
        public void SetSendEMail(Boolean SendEMail)
        {
            Set_Value("SendEMail", SendEMail);
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
            Set_Value("ShareOfCustomer", ShareOfCustomer);
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
            Set_Value("ShelfLifeMinPct", ShelfLifeMinPct);
        }
        /** Get Min Shelf Life %.
        @return Minimum Shelf Life in percent based on Product Instance Guarantee Date */
        public int GetShelfLifeMinPct()
        {
            Object ii = Get_Value("ShelfLifeMinPct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Street.
       @param Street Street */
        public void SetStreet(String Street)
        {
            if (Street != null && Street.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Street = Street.Substring(0, 50);
            }
            Set_Value("Street", Street);
        }
        /** Get Street.
        @return Street */
        public String GetStreet()
        {
            return (String)Get_Value("Street");
        }
        /** Set Add To Interest Area.
        @param SubscribeInterestArea Subscribe Customer to interest area */
        public void SetSubscribeInterestArea(String SubscribeInterestArea)
        {
            if (SubscribeInterestArea != null && SubscribeInterestArea.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                SubscribeInterestArea = SubscribeInterestArea.Substring(0, 50);
            }
            Set_Value("SubscribeInterestArea", SubscribeInterestArea);
        }
        /** Get Add To Interest Area.
        @return Subscribe Customer to interest area */
        public String GetSubscribeInterestArea()
        {
            return (String)Get_Value("SubscribeInterestArea");
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
            Set_Value("TaxID", TaxID);
        }
        /** Get Tax ID.
        @return Tax Identification */
        public String GetTaxID()
        {
            return (String)Get_Value("TaxID");
        }
        /** Set Open Balance.
        @param TotalOpenBalance Total Open Balance Amount in primary Accounting Currency */
        public void SetTotalOpenBalance(Decimal? TotalOpenBalance)
        {
            Set_Value("TotalOpenBalance", (Decimal?)TotalOpenBalance);
        }
        /** Get Open Balance.
        @return Total Open Balance Amount in primary Accounting Currency */
        public Decimal GetTotalOpenBalance()
        {
            Object bd = Get_Value("TotalOpenBalance");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set UNIQUEEMPCODE.
        @param UNIQUEEMPCODE UNIQUEEMPCODE */
        public void SetUNIQUEEMPCODE(int UNIQUEEMPCODE)
        {
            Set_Value("UNIQUEEMPCODE", UNIQUEEMPCODE);
        }
        /** Get UNIQUEEMPCODE.
        @return UNIQUEEMPCODE */
        public int GetUNIQUEEMPCODE()
        {
            Object ii = Get_Value("UNIQUEEMPCODE");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set URL.
        @param URL Full URL address - e.g. http://www.viennasolutions.com */
        public void SetURL(String URL)
        {
            if (URL != null && URL.Length > 120)
            {
                log.Warning("Length > 120 - truncated");
                URL = URL.Substring(0, 120);
            }
            Set_Value("URL", URL);
        }
        /** Get URL.
        @return Full URL address - e.g. http://www.viennasolutions.com */
        public String GetURL()
        {
            return (String)Get_Value("URL");
        }
        /** Set Reward Card Number.
        @param VA204_RewardCardM_ID Reward Card Number */
        public void SetVA204_RewardCardM_ID(int VA204_RewardCardM_ID)
        {
            if (VA204_RewardCardM_ID <= 0) Set_Value("VA204_RewardCardM_ID", null);
            else
                Set_Value("VA204_RewardCardM_ID", VA204_RewardCardM_ID);
        }
        /** Get Reward Card Number.
        @return Reward Card Number */
        public int GetVA204_RewardCardM_ID()
        {
            Object ii = Get_Value("VA204_RewardCardM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAHRUAE_AD_User_ID AD_Reference_ID=110 */
        public static int VAHRUAE_AD_USER_ID_AD_Reference_ID = 110;
        /** Set Report To.
        @param VAHRUAE_AD_User_ID Report To */
        public void SetVAHRUAE_AD_User_ID(int VAHRUAE_AD_User_ID)
        {
            if (VAHRUAE_AD_User_ID <= 0) Set_Value("VAHRUAE_AD_User_ID", null);
            else
                Set_Value("VAHRUAE_AD_User_ID", VAHRUAE_AD_User_ID);
        }
        /** Get Report To.
        @return Report To */
        public int GetVAHRUAE_AD_User_ID()
        {
            Object ii = Get_Value("VAHRUAE_AD_User_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Add To EmployeeRoster.
        @param VAHRUAE_AddToEmployeeRoster Add To EmployeeRoster */
        public void SetVAHRUAE_AddToEmployeeRoster(String VAHRUAE_AddToEmployeeRoster)
        {
            if (VAHRUAE_AddToEmployeeRoster != null && VAHRUAE_AddToEmployeeRoster.Length > 10)
            {
                log.Warning("Length > 10 - truncated");
                VAHRUAE_AddToEmployeeRoster = VAHRUAE_AddToEmployeeRoster.Substring(0, 10);
            }
            Set_Value("VAHRUAE_AddToEmployeeRoster", VAHRUAE_AddToEmployeeRoster);
        }
        /** Get Add To EmployeeRoster.
        @return Add To EmployeeRoster */
        public String GetVAHRUAE_AddToEmployeeRoster()
        {
            return (String)Get_Value("VAHRUAE_AddToEmployeeRoster");
        }
        /** Set ZIP.
       @param ZIP ZIP */
        public void SetZIP(String ZIP)
        {
            if (ZIP != null && ZIP.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                ZIP = ZIP.Substring(0, 50);
            }
            Set_Value("ZIP", ZIP);
        }
        /** Get ZIP.
        @return ZIP */
        public String GetZIP()
        {
            return (String)Get_Value("ZIP");
        }

        /** Set Advance Amount.
        @param VAHRUAE_AdvanceAmount Advance Amount */
        public void SetVAHRUAE_AdvanceAmount(Decimal? VAHRUAE_AdvanceAmount)
        {
            Set_Value("VAHRUAE_AdvanceAmount", (Decimal?)VAHRUAE_AdvanceAmount);
        }
        /** Get Advance Amount.
        @return Advance Amount */
        public Decimal GetVAHRUAE_AdvanceAmount()
        {
            Object bd = Get_Value("VAHRUAE_AdvanceAmount");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Advance Frequency.
        @param VAHRUAE_AdvanceFrequency Advance Frequency */
        public void SetVAHRUAE_AdvanceFrequency(int VAHRUAE_AdvanceFrequency)
        {
            Set_Value("VAHRUAE_AdvanceFrequency", VAHRUAE_AdvanceFrequency);
        }
        /** Get Advance Frequency.
        @return Advance Frequency */
        public int GetVAHRUAE_AdvanceFrequency()
        {
            Object ii = Get_Value("VAHRUAE_AdvanceFrequency");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAHRUAE_BLOOD_GROUP AD_Reference_ID=1000158 */
        public static int VAHRUAE_BLOOD_GROUP_AD_Reference_ID = 1000158;
        /** O+ = 01 */
        public static String VAHRUAE_BLOOD_GROUP_OPlus = "01";
        /** O- = 02 */
        public static String VAHRUAE_BLOOD_GROUP_O_ = "02";
        /** A+ = 03 */
        public static String VAHRUAE_BLOOD_GROUP_APlus = "03";
        /** A- = 04 */
        public static String VAHRUAE_BLOOD_GROUP_A_ = "04";
        /** B+ = 05 */
        public static String VAHRUAE_BLOOD_GROUP_BPlus = "05";
        /** B- = 06 */
        public static String VAHRUAE_BLOOD_GROUP_B_ = "06";
        /** AB+ = 07 */
        public static String VAHRUAE_BLOOD_GROUP_ABPlus = "07";
        /** AB- = 08 */
        public static String VAHRUAE_BLOOD_GROUP_AB_ = "08";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAHRUAE_BLOOD_GROUPValid(String test)
        {
            return test == null || test.Equals("01") || test.Equals("02") || test.Equals("03") || test.Equals("04") || test.Equals("05") || test.Equals("06") || test.Equals("07") || test.Equals("08");
        }
        /** Set Blood Group.
        @param VAHRUAE_BLOOD_GROUP Blood Group */
        public void SetVAHRUAE_BLOOD_GROUP(String VAHRUAE_BLOOD_GROUP)
        {
            if (!IsVAHRUAE_BLOOD_GROUPValid(VAHRUAE_BLOOD_GROUP))
                throw new ArgumentException("VAHRUAE_BLOOD_GROUP Invalid value - " + VAHRUAE_BLOOD_GROUP + " - Reference_ID=1000158 - 01 - 02 - 03 - 04 - 05 - 06 - 07 - 08");
            if (VAHRUAE_BLOOD_GROUP != null && VAHRUAE_BLOOD_GROUP.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VAHRUAE_BLOOD_GROUP = VAHRUAE_BLOOD_GROUP.Substring(0, 2);
            }
            Set_Value("VAHRUAE_BLOOD_GROUP", VAHRUAE_BLOOD_GROUP);
        }
        /** Get Blood Group.
        @return Blood Group */
        public String GetVAHRUAE_BLOOD_GROUP()
        {
            return (String)Get_Value("VAHRUAE_BLOOD_GROUP");
        }
        /** Set Birthday.
        @param VAHRUAE_Birthday Birthday or Anniversary day */
        public void SetVAHRUAE_Birthday(DateTime? VAHRUAE_Birthday)
        {
            Set_Value("VAHRUAE_Birthday", (DateTime?)VAHRUAE_Birthday);
        }
        /** Get Birthday.
        @return Birthday or Anniversary day */
        public DateTime? GetVAHRUAE_Birthday()
        {
            return (DateTime?)Get_Value("VAHRUAE_Birthday");
        }
        /** Set Joining Date.
        @param VAHRUAE_DATE_JOINING Joining Date */
        public void SetVAHRUAE_DATE_JOINING(DateTime? VAHRUAE_DATE_JOINING)
        {
            Set_Value("VAHRUAE_DATE_JOINING", (DateTime?)VAHRUAE_DATE_JOINING);
        }
        /** Get Joining Date.
        @return Joining Date */
        public DateTime? GetVAHRUAE_DATE_JOINING()
        {
            return (DateTime?)Get_Value("VAHRUAE_DATE_JOINING");
        }
        /** Set DateRelieved.
        @param VAHRUAE_DateRelieved DateRelieved */
        public void SetVAHRUAE_DateRelieved(DateTime? VAHRUAE_DateRelieved)
        {
            Set_Value("VAHRUAE_DateRelieved", (DateTime?)VAHRUAE_DateRelieved);
        }
        /** Get DateRelieved.
        @return DateRelieved */
        public DateTime? GetVAHRUAE_DateRelieved()
        {
            return (DateTime?)Get_Value("VAHRUAE_DateRelieved");
        }
        /** Set Department.
        @param VAHRUAE_DepartmentMaster_ID Department */
        public void SetVAHRUAE_DepartmentMaster_ID(int VAHRUAE_DepartmentMaster_ID)
        {
            if (VAHRUAE_DepartmentMaster_ID <= 0) Set_Value("VAHRUAE_DepartmentMaster_ID", null);
            else
                Set_Value("VAHRUAE_DepartmentMaster_ID", VAHRUAE_DepartmentMaster_ID);
        }
        /** Get Department.
        @return Department */
        public int GetVAHRUAE_DepartmentMaster_ID()
        {
            Object ii = Get_Value("VAHRUAE_DepartmentMaster_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Father Name.
        @param VAHRUAE_FatherName Father Name */
        public void SetVAHRUAE_FatherName(String VAHRUAE_FatherName)
        {
            if (VAHRUAE_FatherName != null && VAHRUAE_FatherName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VAHRUAE_FatherName = VAHRUAE_FatherName.Substring(0, 50);
            }
            Set_Value("VAHRUAE_FatherName", VAHRUAE_FatherName);
        }
        /** Get Father Name.
        @return Father Name */
        public String GetVAHRUAE_FatherName()
        {
            return (String)Get_Value("VAHRUAE_FatherName");
        }

        /** VAHRUAE_FrequencyType AD_Reference_ID=1000190 */
        public static int VAHRUAE_FREQUENCYTYPE_AD_Reference_ID = 1000190;
        /** Month = M */
        public static String VAHRUAE_FREQUENCYTYPE_Month = "M";
        /** Quater = Q */
        public static String VAHRUAE_FREQUENCYTYPE_Quater = "Q";
        /** Half Year = S */
        public static String VAHRUAE_FREQUENCYTYPE_HalfYear = "S";
        /** Year = Y */
        public static String VAHRUAE_FREQUENCYTYPE_Year = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAHRUAE_FrequencyTypeValid(String test)
        {
            return test == null || test.Equals("M") || test.Equals("Q") || test.Equals("S") || test.Equals("Y");
        }
        /** Set Frequency Type.
        @param VAHRUAE_FrequencyType Frequency Type */
        public void SetVAHRUAE_FrequencyType(String VAHRUAE_FrequencyType)
        {
            if (!IsVAHRUAE_FrequencyTypeValid(VAHRUAE_FrequencyType))
                throw new ArgumentException("VAHRUAE_FrequencyType Invalid value - " + VAHRUAE_FrequencyType + " - Reference_ID=1000190 - M - Q - S - Y");
            if (VAHRUAE_FrequencyType != null && VAHRUAE_FrequencyType.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                VAHRUAE_FrequencyType = VAHRUAE_FrequencyType.Substring(0, 1);
            }
            Set_Value("VAHRUAE_FrequencyType", VAHRUAE_FrequencyType);
        }
        /** Get Frequency Type.
        @return Frequency Type */
        public String GetVAHRUAE_FrequencyType()
        {
            return (String)Get_Value("VAHRUAE_FrequencyType");
        }

        /** VAHRUAE_GOSIRule_ID AD_Reference_ID=1000195 */
        public static int VAHRUAE_GOSIRULE_ID_AD_Reference_ID = 1000195;
        /** Set GOSI Rule.
        @param VAHRUAE_GOSIRule_ID GOSI Rule */
        public void SetVAHRUAE_GOSIRule_ID(int VAHRUAE_GOSIRule_ID)
        {
            if (VAHRUAE_GOSIRule_ID <= 0) Set_Value("VAHRUAE_GOSIRule_ID", null);
            else
                Set_Value("VAHRUAE_GOSIRule_ID", VAHRUAE_GOSIRule_ID);
        }
        /** Get GOSI Rule.
        @return GOSI Rule */
        public int GetVAHRUAE_GOSIRule_ID()
        {
            Object ii = Get_Value("VAHRUAE_GOSIRule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shift.
        @param VAHRUAE_HR_AttendanceRule_ID Shift */
        public void SetVAHRUAE_HR_AttendanceRule_ID(int VAHRUAE_HR_AttendanceRule_ID)
        {
            if (VAHRUAE_HR_AttendanceRule_ID <= 0) Set_Value("VAHRUAE_HR_AttendanceRule_ID", null);
            else
                Set_Value("VAHRUAE_HR_AttendanceRule_ID", VAHRUAE_HR_AttendanceRule_ID);
        }
        /** Get Shift.
        @return Shift */
        public int GetVAHRUAE_HR_AttendanceRule_ID()
        {
            Object ii = Get_Value("VAHRUAE_HR_AttendanceRule_ID");
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
        /** Set C_PERSONALINFO_ID.
       @param C_PERSONALINFO_ID C_PERSONALINFO_ID */
        public void SetC_PERSONALINFO_ID(int C_PERSONALINFO_ID)
        {
            if (C_PERSONALINFO_ID <= 0) Set_Value("C_PERSONALINFO_ID", null);
            else
                Set_Value("C_PERSONALINFO_ID", C_PERSONALINFO_ID);
        }
        /** Get C_PERSONALINFO_ID.
        @return C_PERSONALINFO_ID */
        public int GetC_PERSONALINFO_ID()
        {
            Object ii = Get_Value("C_PERSONALINFO_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Penalty.
        @param VAHRUAE_HR_Penality_ID Penalty */
        public void SetVAHRUAE_HR_Penality_ID(int VAHRUAE_HR_Penality_ID)
        {
            if (VAHRUAE_HR_Penality_ID <= 0) Set_Value("VAHRUAE_HR_Penality_ID", null);
            else
                Set_Value("VAHRUAE_HR_Penality_ID", VAHRUAE_HR_Penality_ID);
        }
        /** Get Penalty.
        @return Penalty */
        public int GetVAHRUAE_HR_Penality_ID()
        {
            Object ii = Get_Value("VAHRUAE_HR_Penality_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAHRUAE_IndeminityRule_ID AD_Reference_ID=1000196 */
        public static int VAHRUAE_INDEMINITYRULE_ID_AD_Reference_ID = 1000196;
        /** Set Indemnity Rule.
        @param VAHRUAE_IndeminityRule_ID Indemnity Rule */
        public void SetVAHRUAE_IndeminityRule_ID(int VAHRUAE_IndeminityRule_ID)
        {
            if (VAHRUAE_IndeminityRule_ID <= 0) Set_Value("VAHRUAE_IndeminityRule_ID", null);
            else
                Set_Value("VAHRUAE_IndeminityRule_ID", VAHRUAE_IndeminityRule_ID);
        }
        /** Get Indemnity Rule.
        @return Indemnity Rule */
        public int GetVAHRUAE_IndeminityRule_ID()
        {
            Object ii = Get_Value("VAHRUAE_IndeminityRule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Eligible For Advance.
        @param VAHRUAE_IsEligibleForAdvance Eligible For Advance */
        public void SetVAHRUAE_IsEligibleForAdvance(Boolean VAHRUAE_IsEligibleForAdvance)
        {
            Set_Value("VAHRUAE_IsEligibleForAdvance", VAHRUAE_IsEligibleForAdvance);
        }
        /** Get Eligible For Advance.
        @return Eligible For Advance */
        public Boolean IsVAHRUAE_IsEligibleForAdvance()
        {
            Object oo = Get_Value("VAHRUAE_IsEligibleForAdvance");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Leave Salary Rule.
        @param VAHRUAE_LeaveSalaryRule_ID Leave Salary Rule */
        public void SetVAHRUAE_LeaveSalaryRule_ID(int VAHRUAE_LeaveSalaryRule_ID)
        {
            if (VAHRUAE_LeaveSalaryRule_ID <= 0) Set_Value("VAHRUAE_LeaveSalaryRule_ID", null);
            else
                Set_Value("VAHRUAE_LeaveSalaryRule_ID", VAHRUAE_LeaveSalaryRule_ID);
        }
        /** Get Leave Salary Rule.
        @return Leave Salary Rule */
        public int GetVAHRUAE_LeaveSalaryRule_ID()
        {
            Object ii = Get_Value("VAHRUAE_LeaveSalaryRule_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Mother Name.
        @param VAHRUAE_MotherName Mother Name */
        public void SetVAHRUAE_MotherName(String VAHRUAE_MotherName)
        {
            if (VAHRUAE_MotherName != null && VAHRUAE_MotherName.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VAHRUAE_MotherName = VAHRUAE_MotherName.Substring(0, 50);
            }
            Set_Value("VAHRUAE_MotherName", VAHRUAE_MotherName);
        }
        /** Get Mother Name.
        @return Mother Name */
        public String GetVAHRUAE_MotherName()
        {
            return (String)Get_Value("VAHRUAE_MotherName");
        }
        /** Set Nationality.
        @param VAHRUAE_Nationality_ID Nationality */
        public void SetVAHRUAE_Nationality_ID(int VAHRUAE_Nationality_ID)
        {
            if (VAHRUAE_Nationality_ID <= 0) Set_Value("VAHRUAE_Nationality_ID", null);
            else
                Set_Value("VAHRUAE_Nationality_ID", VAHRUAE_Nationality_ID);
        }
        /** Get Nationality.
        @return Nationality */
        public int GetVAHRUAE_Nationality_ID()
        {
            Object ii = Get_Value("VAHRUAE_Nationality_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set RelivingReason.
        @param VAHRUAE_RelivingReason RelivingReason */
        public void SetVAHRUAE_RelivingReason(String VAHRUAE_RelivingReason)
        {
            if (VAHRUAE_RelivingReason != null && VAHRUAE_RelivingReason.Length > 25)
            {
                log.Warning("Length > 25 - truncated");
                VAHRUAE_RelivingReason = VAHRUAE_RelivingReason.Substring(0, 25);
            }
            Set_Value("VAHRUAE_RelivingReason", VAHRUAE_RelivingReason);
        }
        /** Get RelivingReason.
        @return RelivingReason */
        public String GetVAHRUAE_RelivingReason()
        {
            return (String)Get_Value("VAHRUAE_RelivingReason");
        }
        /** Set Remarks.
        @param VAHRUAE_Remarks Remarks */
        public void SetVAHRUAE_Remarks(String VAHRUAE_Remarks)
        {
            if (VAHRUAE_Remarks != null && VAHRUAE_Remarks.Length > 500)
            {
                log.Warning("Length > 500 - truncated");
                VAHRUAE_Remarks = VAHRUAE_Remarks.Substring(0, 500);
            }
            Set_Value("VAHRUAE_Remarks", VAHRUAE_Remarks);
        }
        /** Get Remarks.
        @return Remarks */
        public String GetVAHRUAE_Remarks()
        {
            return (String)Get_Value("VAHRUAE_Remarks");
        }

        /** VAHRUAE_SalaryType AD_Reference_ID=1000230 */
        public static int VAHRUAE_SALARYTYPE_AD_Reference_ID = 1000230;
        /** Daily = DA */
        public static String VAHRUAE_SALARYTYPE_Daily = "DA";
        /** Fort Night = FN */
        public static String VAHRUAE_SALARYTYPE_FortNight = "FN";
        /** Monthly = MO */
        public static String VAHRUAE_SALARYTYPE_Monthly = "MO";
        /** Weekly = WE */
        public static String VAHRUAE_SALARYTYPE_Weekly = "WE";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVAHRUAE_SalaryTypeValid(String test)
        {
            return test.Equals("DA") || test.Equals("FN") || test.Equals("MO") || test.Equals("WE");
        }
        /** Set Salary Type.
        @param VAHRUAE_SalaryType Salary Type */
        public void SetVAHRUAE_SalaryType(String VAHRUAE_SalaryType)
        {
            if (VAHRUAE_SalaryType == null) throw new ArgumentException("VAHRUAE_SalaryType is mandatory");
            if (!IsVAHRUAE_SalaryTypeValid(VAHRUAE_SalaryType))
                throw new ArgumentException("VAHRUAE_SalaryType Invalid value - " + VAHRUAE_SalaryType + " - Reference_ID=1000230 - DA - FN - MO - WE");
            if (VAHRUAE_SalaryType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                VAHRUAE_SalaryType = VAHRUAE_SalaryType.Substring(0, 2);
            }
            Set_Value("VAHRUAE_SalaryType", VAHRUAE_SalaryType);
        }
        /** Get Salary Type.
        @return Salary Type */
        public String GetVAHRUAE_SalaryType()
        {
            return (String)Get_Value("VAHRUAE_SalaryType");
        }
        /** Set Shift Compensation.
        @param VAHRUAE_ShiftCompensation Shift Compensation */
        public void SetVAHRUAE_ShiftCompensation(Boolean VAHRUAE_ShiftCompensation)
        {
            Set_Value("VAHRUAE_ShiftCompensation", VAHRUAE_ShiftCompensation);
        }
        /** Set UnivRegNo.
       @param UNIVREGNO UnivRegNo */
        public void SetUNIVREGNO(String UNIVREGNO)
        {
            if (UNIVREGNO != null && UNIVREGNO.Length > 40)
            {
                log.Warning("Length > 40 - truncated");
                UNIVREGNO = UNIVREGNO.Substring(0, 40);
            }
            Set_Value("UNIVREGNO", UNIVREGNO);
        }
        /** Get UnivRegNo.
        @return UnivRegNo */
        public String GetUNIVREGNO()
        {
            return (String)Get_Value("UNIVREGNO");
        }

        /** VENDOR_NAME AD_Reference_ID=192 */
        public static int VENDOR_NAME_AD_Reference_ID = 192;
        /** Set VENDOR_NAME.
        @param VENDOR_NAME VENDOR_NAME */
        public void SetVENDOR_NAME(int VENDOR_NAME)
        {
            Set_Value("VENDOR_NAME", VENDOR_NAME);
        }
        /** Get VENDOR_NAME.
        @return VENDOR_NAME */
        public int GetVENDOR_NAME()
        {
            Object ii = Get_Value("VENDOR_NAME");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set BP Ref Code.
       @param VA008_CBPartnerRefCode This indicates Business Partner Identification code which is generated through Generate Ref Code Process */
        public void SetVA008_CBPartnerRefCode(String VA008_CBPartnerRefCode)
        {
            if (VA008_CBPartnerRefCode != null && VA008_CBPartnerRefCode.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VA008_CBPartnerRefCode = VA008_CBPartnerRefCode.Substring(0, 50);
            }
            Set_Value("VA008_CBPartnerRefCode", VA008_CBPartnerRefCode);
        }
        /** Get BP Ref Code.
        @return This indicates Business Partner Identification code which is generated through Generate Ref Code Process */
        public String GetVA008_CBPartnerRefCode()
        {
            return (String)Get_Value("VA008_CBPartnerRefCode");
        }


        /** Set Generate Ref Code.
        @param VA008_Generaterefcode This button is link with process to generate a Ref Code */
        public void SetVA008_Generaterefcode(String VA008_Generaterefcode)
        {
            if (VA008_Generaterefcode != null && VA008_Generaterefcode.Length > 3)
            {
                log.Warning("Length > 3 - truncated");
                VA008_Generaterefcode = VA008_Generaterefcode.Substring(0, 3);
            }
            Set_Value("VA008_Generaterefcode", VA008_Generaterefcode);
        }
        /** Get Generate Ref Code.
        @return This button is link with process to generate a Ref Code */
        public String GetVA008_Generaterefcode()
        {
            return (String)Get_Value("VA008_Generaterefcode");
        }
        /** Set Validated.
      @param VA008_IsValidated This check box become True when reference code is validated  */
        public void SetVA008_IsValidated(Boolean VA008_IsValidated)
        {
            Set_Value("VA008_IsValidated", VA008_IsValidated);
        }
        /** Get Validated.
        @return This check box become True when reference code is validated  */
        public Boolean IsVA008_IsValidated()
        {
            Object oo = Get_Value("VA008_IsValidated");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Set Reference Code.
        @param VA008_IsReferCode Reference Code */
        public void SetVA008_IsReferCode(Boolean VA008_IsReferCode)
        {
            Set_Value("VA008_IsReferCode", VA008_IsReferCode);
        }
        /** Get Reference Code.
        @return Reference Code */
        public Boolean IsVA008_IsReferCode()
        {
            Object oo = Get_Value("VA008_IsReferCode");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Get Shift Compensation.
        @return Shift Compensation */
        public Boolean IsVAHRUAE_ShiftCompensation()
        {
            Object oo = Get_Value("VAHRUAE_ShiftCompensation");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Reward Card Number.
        @param VAPOS_RewardCardM_ID Reward Card Number */
        public void SetVAPOS_RewardCardM_ID(int VAPOS_RewardCardM_ID)
        {
            if (VAPOS_RewardCardM_ID <= 0) Set_Value("VAPOS_RewardCardM_ID", null);
            else
                Set_Value("VAPOS_RewardCardM_ID", VAPOS_RewardCardM_ID);
        }
        /** Get Reward Card Number.
        @return Reward Card Number */
        public int GetVAPOS_RewardCardM_ID()
        {
            Object ii = Get_Value("VAPOS_RewardCardM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Type.
        @param VATAX_TaxType_ID Tax Type */
        public void SetVATAX_TaxType_ID(int VATAX_TaxType_ID)
        {
            if (VATAX_TaxType_ID <= 0) Set_Value("VATAX_TaxType_ID", null);
            else
                Set_Value("VATAX_TaxType_ID", VATAX_TaxType_ID);
        }
        /** Get Tax Type.
        @return Tax Type */
        public int GetVATAX_TaxType_ID()
        {
            Object ii = Get_Value("VATAX_TaxType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set POS ExportID.
        @param VAPOS_ExportID POS ExportID */
        public void SetVAPOS_ExportID(String VAPOS_ExportID)
        {
            if (VAPOS_ExportID != null && VAPOS_ExportID.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                VAPOS_ExportID = VAPOS_ExportID.Substring(0, 50);
            }
            Set_Value("VAPOS_ExportID", VAPOS_ExportID);
        }
        /** Get POS ExportID.
        @return POS ExportID */
        public String GetVAPOS_ExportID()
        {
            return (String)Get_Value("VAPOS_ExportID");
        }

        /** Set General Attribute Set Instance.
@param C_GenAttributeSetInstance_ID General Attribute Set Instance */
        public void SetC_GenAttributeSetInstance_ID(Object C_GenAttributeSetInstance_ID)
        {
            Set_Value("C_GenAttributeSetInstance_ID", C_GenAttributeSetInstance_ID);
        }
        /** Get General Attribute Set Instance.
        @return General Attribute Set Instance */
        public Object GetC_GenAttributeSetInstance_ID()
        {
            return Get_Value("C_GenAttributeSetInstance_ID");
        }
        /** Set General Attribute Set.
        @param C_GenAttributeSet_ID General Attribute Set */
        public void SetC_GenAttributeSet_ID(int C_GenAttributeSet_ID)
        {
            if (C_GenAttributeSet_ID <= 0) Set_Value("C_GenAttributeSet_ID", null);
            else
                Set_Value("C_GenAttributeSet_ID", C_GenAttributeSet_ID);
        }
        /** Get General Attribute Set.
        @return General Attribute Set */
        public int GetC_GenAttributeSet_ID()
        {
            Object ii = Get_Value("C_GenAttributeSet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        public void SetED004_CUSTOMERTXCODE(String TaxID)
        {
            if (TaxID != null && TaxID.Length > 20)
            {
                log.Warning("Length > 20 - truncated");
                TaxID = TaxID.Substring(0, 20);
            }
            Set_Value("ED004_CustomerTaxCode", TaxID);
        }
        public void SetED004_TAXPAYERCODE(int taxDesc)
        {
            Set_Value("ED004_TaxpayerCode", taxDesc);
        }

        /** Set ConsolidationRef.
        @param VAWMS_C_ConsolidationRef_ID ConsolidationRef */
        public void SetVAWMS_C_ConsolidationRef_ID(int VAWMS_C_ConsolidationRef_ID)
        {
            if (VAWMS_C_ConsolidationRef_ID <= 0) Set_Value("VAWMS_C_ConsolidationRef_ID", null);
            else
                Set_Value("VAWMS_C_ConsolidationRef_ID", VAWMS_C_ConsolidationRef_ID);
        }
        /** Get ConsolidationRef.
        @return ConsolidationRef */
        public int GetVAWMS_C_ConsolidationRef_ID()
        {
            Object ii = Get_Value("VAWMS_C_ConsolidationRef_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set VSS_DEPARTMENTMASTER_ID.
        @param VSS_DEPARTMENTMASTER_ID VSS_DEPARTMENTMASTER_ID */
        public void SetVSS_DEPARTMENTMASTER_ID(int VSS_DEPARTMENTMASTER_ID)
        {
            if (VSS_DEPARTMENTMASTER_ID <= 0) Set_Value("VSS_DEPARTMENTMASTER_ID", null);
            else
                Set_Value("VSS_DEPARTMENTMASTER_ID", VSS_DEPARTMENTMASTER_ID);
        }
        /** Get VSS_DEPARTMENTMASTER_ID.
        @return VSS_DEPARTMENTMASTER_ID */
        public int GetVSS_DEPARTMENTMASTER_ID()
        {
            Object ii = Get_Value("VSS_DEPARTMENTMASTER_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Search Key.
        @param Value Search key for the record in the format required - must be unique */
        public void SetValue(String Value)
        {
            if (Value == null) throw new ArgumentException("Value is mandatory.");
            if (Value.Length > 80)
            {
                log.Warning("Length > 80 - truncated");
                Value = Value.Substring(0, 80);
            }
            Set_Value("Value", Value);
        }
        /** Get Search Key.
        @return Search key for the record in the format required - must be unique */
        public String GetValue()
        {
            return (String)Get_Value("Value");
        }

        //-------------------------Column Added by Anuj------------------------------
        /** Set BP Mandate.
       @param VA009_BPMandate_ID BP Mandate */
        public void SetVA009_BPMandate_ID(int VA009_BPMandate_ID)
        {
            if (VA009_BPMandate_ID <= 0) Set_Value("VA009_BPMandate_ID", null);
            else
                Set_Value("VA009_BPMandate_ID", VA009_BPMandate_ID);
        }
        /** Get BP Mandate.
        @return BP Mandate */
        public int GetVA009_BPMandate_ID()
        {
            Object ii = Get_Value("VA009_BPMandate_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Method.
        @param VA009_PaymentMethod_ID Payment Method */
        public void SetVA009_PaymentMethod_ID(int VA009_PaymentMethod_ID)
        {
            if (VA009_PaymentMethod_ID <= 0) Set_Value("VA009_PaymentMethod_ID", null);
            else
                Set_Value("VA009_PaymentMethod_ID", VA009_PaymentMethod_ID);
        }
        /** Get Payment Method.
        @return Payment Method */
        public int GetVA009_PaymentMethod_ID()
        {
            Object ii = Get_Value("VA009_PaymentMethod_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VA009_PO_PaymentMethod_ID AD_Reference_ID=1000613 */
        public static int VA009_PO_PAYMENTMETHOD_ID_AD_Reference_ID = 1000613;
        /** Set PO Payment Method.
            @param VA009_PO_PaymentMethod_ID The way that a buyer choose to pay the seller of a good or service. */
        public void SetVA009_PO_PaymentMethod_ID(int VA009_PO_PaymentMethod_ID)
        {
            if (VA009_PO_PaymentMethod_ID <= 0)
                Set_Value("VA009_PO_PaymentMethod_ID", null);
            else
                Set_Value("VA009_PO_PaymentMethod_ID", VA009_PO_PaymentMethod_ID);
        }
        /** Get PO Payment Method.
            @return The way that a buyer choose to pay the seller of a good or service. */
        public int GetVA009_PO_PaymentMethod_ID()
        {
            Object ii = Get_Value("VA009_PO_PaymentMethod_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }
        //----------------------------------------------------------

        /** Set Generate Opportunity.
       @param CreateProject Generate opportunity from lead */
        public void SetCreateProject(String CreateProject)
        {
            if (CreateProject != null && CreateProject.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreateProject = CreateProject.Substring(0, 1);
            }
            Set_Value("CreateProject", CreateProject);
        }
        /** Get Generate Opportunity.
        @return Generate opportunity from lead */
        public String GetCreateProject()
        {
            return (String)Get_Value("CreateProject");
        }
        //----------------------------------------------------------

        /** VA025_DiscountCalculation AD_Reference_ID=1000487 */
        public static int VA025_DISCOUNTCALCULATION_AD_Reference_ID = 1000487;/** Combination and Value = C1 */
        public static String VA025_DISCOUNTCALCULATION_CombinationAndValue = "C1";/** Value and Combination = C2 */
        public static String VA025_DISCOUNTCALCULATION_ValueAndCombination = "C2";/** Combination = C3 */
        public static String VA025_DISCOUNTCALCULATION_Combination = "C3";/** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsVA025_DiscountCalculationValid(String test) { return test == null || test.Equals("C1") || test.Equals("C2") || test.Equals("C3"); }/** Set Apply Discount Calculation.
        @param VA025_DiscountCalculation Apply Discount Calculation */
        public void SetVA025_DiscountCalculation(String VA025_DiscountCalculation)
        {
            if (!IsVA025_DiscountCalculationValid(VA025_DiscountCalculation))
                throw new ArgumentException("VA025_DiscountCalculation Invalid value - " + VA025_DiscountCalculation + " - Reference_ID=1000487 - C1 - C2 - C3"); if (VA025_DiscountCalculation != null && VA025_DiscountCalculation.Length > 2) { log.Warning("Length > 2 - truncated"); VA025_DiscountCalculation = VA025_DiscountCalculation.Substring(0, 2); }
            Set_Value("VA025_DiscountCalculation", VA025_DiscountCalculation);
        } /** Get Apply Discount Calculation.
        @return Apply Discount Calculation */
        public String GetVA025_DiscountCalculation() { return (String)Get_Value("VA025_DiscountCalculation"); }/** Set Apply Discount Per Unit.
        @param VA025_IsDiscountPerUnit Apply Discount Per Unit */
        public void SetVA025_IsDiscountPerUnit(Boolean VA025_IsDiscountPerUnit) { Set_Value("VA025_IsDiscountPerUnit", VA025_IsDiscountPerUnit); }/** Get Apply Discount Per Unit.
        @return Apply Discount Per Unit */
        public Boolean IsVA025_IsDiscountPerUnit() { Object oo = Get_Value("VA025_IsDiscountPerUnit"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** VA025_PromotionalDiscount AD_Reference_ID=325 */
        public static int VA025_PROMOTIONALDISCOUNT_AD_Reference_ID = 325;/** Set Promotional Discount Schema.
        @param VA025_PromotionalDiscount Promotional Discount Schema */
        public void SetVA025_PromotionalDiscount(int VA025_PromotionalDiscount) { Set_Value("VA025_PromotionalDiscount", VA025_PromotionalDiscount); }/** Get Promotional Discount Schema.
        @return Promotional Discount Schema */
        public int GetVA025_PromotionalDiscount() { Object ii = Get_Value("VA025_PromotionalDiscount"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /// <summary>
        /// Set Withholding Tax.
        /// </summary>
        /// <param name="C_Withholding_ID">Withholding type defined</param>
        public void SetC_Withholding_ID(int C_Withholding_ID)
        {
            if (C_Withholding_ID <= 0) Set_Value("C_Withholding_ID", null);
            else
                Set_Value("C_Withholding_ID", C_Withholding_ID);
        }
        /// <summary>
        /// Get Withholding Tax.
        /// </summary>
        /// <returns>Withholding type defined</returns>
        public int GetC_Withholding_ID() { Object ii = Get_Value("C_Withholding_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** Set Withholding applicable on AP Invoice.
@param IsApplicableonAPInvoice This field represents that withholding tax to be calculated on the AP Invoices. */
        public void SetIsApplicableonAPInvoice(Boolean IsApplicableonAPInvoice) { Set_Value("IsApplicableonAPInvoice", IsApplicableonAPInvoice); }/** Get Withholding applicable on AP Invoice.
@return This field represents that withholding tax to be calculated on the AP Invoices. */
        public Boolean IsApplicableonAPInvoice() { Object oo = Get_Value("IsApplicableonAPInvoice"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Withholding applicable on AP Payment.
@param IsApplicableonAPPayment This field represents that withholding tax to be calculated on the AP Payment */
        public void SetIsApplicableonAPPayment(Boolean IsApplicableonAPPayment) { Set_Value("IsApplicableonAPPayment", IsApplicableonAPPayment); }/** Get Withholding applicable on AP Payment.
@return This field represents that withholding tax to be calculated on the AP Payment */
        public Boolean IsApplicableonAPPayment() { Object oo = Get_Value("IsApplicableonAPPayment"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Withholding applicable on AR Invoice.
@param IsApplicableonARInvoice This field represents that withholding tax to be calculated on the AR Invoices. */
        public void SetIsApplicableonARInvoice(Boolean IsApplicableonARInvoice) { Set_Value("IsApplicableonARInvoice", IsApplicableonARInvoice); }/** Get Withholding applicable on AR Invoice.
@return This field represents that withholding tax to be calculated on the AR Invoices. */
        public Boolean IsApplicableonARInvoice() { Object oo = Get_Value("IsApplicableonARInvoice"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set Withholding applicable on AR Receipt.
@param IsApplicableonARReceipt This field represents that withholding tax to be calculated on the AR Receipt. */
        public void SetIsApplicableonARReceipt(Boolean IsApplicableonARReceipt) { Set_Value("IsApplicableonARReceipt", IsApplicableonARReceipt); }/** Get Withholding applicable on AR Receipt.
@return This field represents that withholding tax to be calculated on the AR Receipt. */
        public Boolean IsApplicableonARReceipt() { Object oo = Get_Value("IsApplicableonARReceipt"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }
        /** AP_WithholdingTax_ID AD_Reference_ID=1000228 */
        public static int AP_WITHHOLDINGTAX_ID_AD_Reference_ID = 1000228;/** Set AP Withholding Tax.
@param AP_WithholdingTax_ID AP Withholding Tax */
        public void SetAP_WithholdingTax_ID(int AP_WithholdingTax_ID)
        {
            if (AP_WithholdingTax_ID <= 0) Set_Value("AP_WithholdingTax_ID", null);
            else
                Set_Value("AP_WithholdingTax_ID", AP_WithholdingTax_ID);
        }/** Get AP Withholding Tax.
@return AP Withholding Tax */
        public int GetAP_WithholdingTax_ID() { Object ii = Get_Value("AP_WithholdingTax_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
    }

}
