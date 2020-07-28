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
    /** Generated Model for M_InOut
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_InOut : PO
    {
        public X_M_InOut(Context ctx, int M_InOut_ID, Trx trxName)
            : base(ctx, M_InOut_ID, trxName)
        {
            /** if (M_InOut_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_DocType_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDeliveryRule (null);	// A
            SetDeliveryViaRule (null);	// P
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetFreightCostRule (null);	// I
            SetIsApproved (false);
            SetIsInDispute (false);
            SetIsInTransit (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetM_InOut_ID (0);
            SetM_Warehouse_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetMovementType (null);
            SetPosted (false);
            SetPriorityRule (null);	// 5
            SetProcessed (false);	// N
            SetSendEMail (false);
            }
             */
        }
        public X_M_InOut(Ctx ctx, int M_InOut_ID, Trx trxName)
            : base(ctx, M_InOut_ID, trxName)
        {
            /** if (M_InOut_ID == 0)
            {
            SetC_BPartner_ID (0);
            SetC_BPartner_Location_ID (0);
            SetC_DocType_ID (0);
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDeliveryRule (null);	// A
            SetDeliveryViaRule (null);	// P
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetFreightCostRule (null);	// I
            SetIsApproved (false);
            SetIsInDispute (false);
            SetIsInTransit (false);
            SetIsPrinted (false);
            SetIsReturnTrx (false);	// N
            SetIsSOTrx (false);	// @IsSOTrx@
            SetM_InOut_ID (0);
            SetM_Warehouse_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetMovementType (null);
            SetPosted (false);
            SetPriorityRule (null);	// 5
            SetProcessed (false);	// N
            SetSendEMail (false);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOut(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOut(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOut(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_InOut()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379459L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062670L;
        /** AD_Table_ID=319 */
        public static int Table_ID;
        // =319;

        /** TableName=M_InOut */
        public static String Table_Name = "M_InOut";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(1);
        /** AccessLevel
        @return 1 - Org 
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
            StringBuilder sb = new StringBuilder("X_M_InOut[").Append(Get_ID()).Append("]");
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
            if (C_BPartner_ID < 1) throw new ArgumentException("C_BPartner_ID is mandatory.");
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
            if (C_BPartner_Location_ID < 1) throw new ArgumentException("C_BPartner_Location_ID is mandatory.");
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

        /** C_Charge_ID AD_Reference_ID=200 */
        public static int C_CHARGE_ID_AD_Reference_ID = 200;
        /** Set Charge.
        @param C_Charge_ID Additional document charges */
        public void SetC_Charge_ID(int C_Charge_ID)
        {
            if (C_Charge_ID <= 0) Set_Value("C_Charge_ID", null);
            else
                Set_Value("C_Charge_ID", C_Charge_ID);
        }
        /** Get Charge.
        @return Additional document charges */
        public int GetC_Charge_ID()
        {
            Object ii = Get_Value("C_Charge_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** C_DocType_ID AD_Reference_ID=170 */
        public static int C_DOCTYPE_ID_AD_Reference_ID = 170;
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
            Set_ValueNoCheck("C_DocType_ID", C_DocType_ID);
        }
        /** Get Document Type.
        @return Document type or rules */
        public int GetC_DocType_ID()
        {
            Object ii = Get_Value("C_DocType_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_ValueNoCheck("C_Invoice_ID", null);
            else
                Set_ValueNoCheck("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Order.
        @param C_Order_ID Order */
        public void SetC_Order_ID(int C_Order_ID)
        {
            if (C_Order_ID <= 0) Set_ValueNoCheck("C_Order_ID", null);
            else
                Set_ValueNoCheck("C_Order_ID", C_Order_ID);
        }
        /** Get Order.
        @return Order */
        public int GetC_Order_ID()
        {
            Object ii = Get_Value("C_Order_ID");
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
        /** Set Charge amount.
        @param ChargeAmt Charge Amount */
        public void SetChargeAmt(Decimal? ChargeAmt)
        {
            Set_Value("ChargeAmt", (Decimal?)ChargeAmt);
        }
        /** Get Charge amount.
        @return Charge Amount */
        public Decimal GetChargeAmt()
        {
            Object bd = Get_Value("ChargeAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Create Confirm.
        @param CreateConfirm Create Confirm */
        public void SetCreateConfirm(String CreateConfirm)
        {
            if (CreateConfirm != null && CreateConfirm.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreateConfirm = CreateConfirm.Substring(0, 1);
            }
            Set_Value("CreateConfirm", CreateConfirm);
        }
        /** Get Create Confirm.
        @return Create Confirm */
        public String GetCreateConfirm()
        {
            return (String)Get_Value("CreateConfirm");
        }
        /** Set Create lines from.
        @param CreateFrom Process which will generate a new document lines based on an existing document */
        public void SetCreateFrom(String CreateFrom)
        {
            if (CreateFrom != null && CreateFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreateFrom = CreateFrom.Substring(0, 1);
            }
            Set_Value("CreateFrom", CreateFrom);
        }
        /** Get Create lines from.
        @return Process which will generate a new document lines based on an existing document */
        public String GetCreateFrom()
        {
            return (String)Get_Value("CreateFrom");
        }
        /** Set Create Package.
        @param CreatePackage Create Package */
        public void SetCreatePackage(String CreatePackage)
        {
            if (CreatePackage != null && CreatePackage.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CreatePackage = CreatePackage.Substring(0, 1);
            }
            Set_Value("CreatePackage", CreatePackage);
        }
        /** Get Create Package.
        @return Create Package */
        public String GetCreatePackage()
        {
            return (String)Get_Value("CreatePackage");
        }
        /** Set Account Date.
        @param DateAcct General Ledger Date */
        public void SetDateAcct(DateTime? DateAcct)
        {
            if (DateAcct == null) throw new ArgumentException("DateAcct is mandatory.");
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
            Set_ValueNoCheck("DateOrdered", (DateTime?)DateOrdered);
        }
        /** Get Date Ordered.
        @return Date of Order */
        public DateTime? GetDateOrdered()
        {
            return (DateTime?)Get_Value("DateOrdered");
        }
        /** Set Date printed.
        @param DatePrinted Date the document was printed. */
        public void SetDatePrinted(DateTime? DatePrinted)
        {
            Set_Value("DatePrinted", (DateTime?)DatePrinted);
        }
        /** Get Date printed.
        @return Date the document was printed. */
        public DateTime? GetDatePrinted()
        {
            return (DateTime?)Get_Value("DatePrinted");
        }
        /** Set Date received.
        @param DateReceived Date a product was received */
        public void SetDateReceived(DateTime? DateReceived)
        {
            Set_Value("DateReceived", (DateTime?)DateReceived);
        }
        /** Get Date received.
        @return Date a product was received */
        public DateTime? GetDateReceived()
        {
            return (DateTime?)Get_Value("DateReceived");
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
            return test.Equals("A") || test.Equals("F") || test.Equals("L") || test.Equals("M") || test.Equals("O") || test.Equals("R");
        }
        /** Set Shipping Rule.
        @param DeliveryRule Defines the timing of Shipping */
        public void SetDeliveryRule(String DeliveryRule)
        {
            if (DeliveryRule == null) throw new ArgumentException("DeliveryRule is mandatory");
            if (!IsDeliveryRuleValid(DeliveryRule))
                throw new ArgumentException("DeliveryRule Invalid value - " + DeliveryRule + " - Reference_ID=151 - A - F - L - M - O - R");
            if (DeliveryRule.Length > 1)
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
            return test.Equals("D") || test.Equals("P") || test.Equals("S");
        }
        /** Set Shipping Method.
        @param DeliveryViaRule How the order will be delivered */
        public void SetDeliveryViaRule(String DeliveryViaRule)
        {
            if (DeliveryViaRule == null) throw new ArgumentException("DeliveryViaRule is mandatory");
            if (!IsDeliveryViaRuleValid(DeliveryViaRule))
                throw new ArgumentException("DeliveryViaRule Invalid value - " + DeliveryViaRule + " - Reference_ID=152 - D - P - S");
            if (DeliveryViaRule.Length > 1)
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

        /** DocAction AD_Reference_ID=135 */
        public static int DOCACTION_AD_Reference_ID = 135;
        /** <None> = -- */
        public static String DOCACTION_None = "--";
        /** Approve = AP */
        public static String DOCACTION_Approve = "AP";
        /** Close = CL */
        public static String DOCACTION_Close = "CL";
        /** Complete = CO */
        public static String DOCACTION_Complete = "CO";
        /** Invalidate = IN */
        public static String DOCACTION_Invalidate = "IN";
        /** Post = PO */
        public static String DOCACTION_Post = "PO";
        /** Prepare = PR */
        public static String DOCACTION_Prepare = "PR";
        /** Reverse - Accrual = RA */
        public static String DOCACTION_Reverse_Accrual = "RA";
        /** Reverse - Correct = RC */
        public static String DOCACTION_Reverse_Correct = "RC";
        /** Re-activate = RE */
        public static String DOCACTION_Re_Activate = "RE";
        /** Reject = RJ */
        public static String DOCACTION_Reject = "RJ";
        /** Void = VO */
        public static String DOCACTION_Void = "VO";
        /** Wait Complete = WC */
        public static String DOCACTION_WaitComplete = "WC";
        /** Unlock = XL */
        public static String DOCACTION_Unlock = "XL";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocActionValid(String test)
        {
            return test.Equals("--") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("IN") || test.Equals("PO") || test.Equals("PR") || test.Equals("RA") || test.Equals("RC") || test.Equals("RE") || test.Equals("RJ") || test.Equals("VO") || test.Equals("WC") || test.Equals("XL");
        }
        /** Set Document Action.
        @param DocAction The targeted status of the document */
        public void SetDocAction(String DocAction)
        {
            if (DocAction == null) throw new ArgumentException("DocAction is mandatory");
            if (!IsDocActionValid(DocAction))
                throw new ArgumentException("DocAction Invalid value - " + DocAction + " - Reference_ID=135 - -- - AP - CL - CO - IN - PO - PR - RA - RC - RE - RJ - VO - WC - XL");
            if (DocAction.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocAction = DocAction.Substring(0, 2);
            }
            Set_Value("DocAction", DocAction);
        }
        /** Get Document Action.
        @return The targeted status of the document */
        public String GetDocAction()
        {
            return (String)Get_Value("DocAction");
        }

        /** DocStatus AD_Reference_ID=131 */
        public static int DOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String DOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String DOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String DOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String DOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String DOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String DOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String DOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String DOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String DOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String DOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String DOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String DOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsDocStatusValid(String test)
        {
            return test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Document Status.
        @param DocStatus The current status of the document */
        public void SetDocStatus(String DocStatus)
        {
            if (DocStatus == null) throw new ArgumentException("DocStatus is mandatory");
            if (!IsDocStatusValid(DocStatus))
                throw new ArgumentException("DocStatus Invalid value - " + DocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (DocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                DocStatus = DocStatus.Substring(0, 2);
            }
            Set_Value("DocStatus", DocStatus);
        }
        /** Get Document Status.
        @return The current status of the document */
        public String GetDocStatus()
        {
            return (String)Get_Value("DocStatus");
        }
        /** Set Document No.
        @param DocumentNo Document sequence number of the document */
        public void SetDocumentNo(String DocumentNo)
        {
            if (DocumentNo == null) throw new ArgumentException("DocumentNo is mandatory.");
            if (DocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                DocumentNo = DocumentNo.Substring(0, 30);
            }
            Set_ValueNoCheck("DocumentNo", DocumentNo);
        }
        /** Get Document No.
        @return Document sequence number of the document */
        public String GetDocumentNo()
        {
            return (String)Get_Value("DocumentNo");
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetDocumentNo());
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
            return test.Equals("C") || test.Equals("F") || test.Equals("I") || test.Equals("L");
        }
        /** Set Freight Cost Rule.
        @param FreightCostRule Method for charging Freight */
        public void SetFreightCostRule(String FreightCostRule)
        {
            if (FreightCostRule == null) throw new ArgumentException("FreightCostRule is mandatory");
            if (!IsFreightCostRuleValid(FreightCostRule))
                throw new ArgumentException("FreightCostRule Invalid value - " + FreightCostRule + " - Reference_ID=153 - C - F - I - L");
            if (FreightCostRule.Length > 1)
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
        /** Set Generate To.
        @param GenerateTo Generate To */
        public void SetGenerateTo(String GenerateTo)
        {
            if (GenerateTo != null && GenerateTo.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                GenerateTo = GenerateTo.Substring(0, 1);
            }
            Set_Value("GenerateTo", GenerateTo);
        }
        /** Get Generate To.
        @return Generate To */
        public String GetGenerateTo()
        {
            return (String)Get_Value("GenerateTo");
        }
        /** Set Approved.
        @param IsApproved Indicates if this document requires approval */
        public void SetIsApproved(Boolean IsApproved)
        {
            Set_Value("IsApproved", IsApproved);
        }
        /** Get Approved.
        @return Indicates if this document requires approval */
        public Boolean IsApproved()
        {
            Object oo = Get_Value("IsApproved");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set In Dispute.
        @param IsInDispute Document is in dispute */
        public void SetIsInDispute(Boolean IsInDispute)
        {
            Set_Value("IsInDispute", IsInDispute);
        }
        /** Get In Dispute.
        @return Document is in dispute */
        public Boolean IsInDispute()
        {
            Object oo = Get_Value("IsInDispute");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set In Transit.
        @param IsInTransit Movement is in transit */
        public void SetIsInTransit(Boolean IsInTransit)
        {
            Set_Value("IsInTransit", IsInTransit);
        }
        /** Get In Transit.
        @return Movement is in transit */
        public Boolean IsInTransit()
        {
            Object oo = Get_Value("IsInTransit");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Printed.
        @param IsPrinted Indicates if this document / line is printed */
        public void SetIsPrinted(Boolean IsPrinted)
        {
            Set_Value("IsPrinted", IsPrinted);
        }
        /** Get Printed.
        @return Indicates if this document / line is printed */
        public Boolean IsPrinted()
        {
            Object oo = Get_Value("IsPrinted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Return Transaction.
        @param IsReturnTrx This is a return transaction */
        public void SetIsReturnTrx(Boolean IsReturnTrx)
        {
            Set_Value("IsReturnTrx", IsReturnTrx);
        }
        /** Get Return Transaction.
        @return This is a return transaction */
        public Boolean IsReturnTrx()
        {
            Object oo = Get_Value("IsReturnTrx");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Shipment/Receipt.
        @param M_InOut_ID Material Shipment Document */
        public void SetM_InOut_ID(int M_InOut_ID)
        {
            if (M_InOut_ID < 1) throw new ArgumentException("M_InOut_ID is mandatory.");
            Set_ValueNoCheck("M_InOut_ID", M_InOut_ID);
        }
        /** Get Shipment/Receipt.
        @return Material Shipment Document */
        public int GetM_InOut_ID()
        {
            Object ii = Get_Value("M_InOut_ID");
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
            if (M_Warehouse_ID < 1) throw new ArgumentException("M_Warehouse_ID is mandatory.");
            Set_ValueNoCheck("M_Warehouse_ID", M_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetM_Warehouse_ID()
        {
            Object ii = Get_Value("M_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** MatchRequirementR AD_Reference_ID=410 */
        public static int MATCHREQUIREMENTR_AD_Reference_ID = 410;
        /** Purchase Order and Invoice = B */
        public static String MATCHREQUIREMENTR_PurchaseOrderAndInvoice = "B";
        /** Invoice = I */
        public static String MATCHREQUIREMENTR_Invoice = "I";
        /** None = N */
        public static String MATCHREQUIREMENTR_None = "N";
        /** Purchase Order = P */
        public static String MATCHREQUIREMENTR_PurchaseOrder = "P";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMatchRequirementRValid(String test)
        {
            return test == null || test.Equals("B") || test.Equals("I") || test.Equals("N") || test.Equals("P");
        }
        /** Set Receipt Match Requirement.
        @param MatchRequirementR Matching Requirement for Receipts */
        public void SetMatchRequirementR(String MatchRequirementR)
        {
            if (!IsMatchRequirementRValid(MatchRequirementR))
                throw new ArgumentException("MatchRequirementR Invalid value - " + MatchRequirementR + " - Reference_ID=410 - B - I - N - P");
            if (MatchRequirementR != null && MatchRequirementR.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                MatchRequirementR = MatchRequirementR.Substring(0, 1);
            }
            Set_Value("MatchRequirementR", MatchRequirementR);
        }
        /** Get Receipt Match Requirement.
        @return Matching Requirement for Receipts */
        public String GetMatchRequirementR()
        {
            return (String)Get_Value("MatchRequirementR");
        }
        /** Set Movement Date.
        @param MovementDate Date a product was moved in or out of inventory */
        public void SetMovementDate(DateTime? MovementDate)
        {
            if (MovementDate == null) throw new ArgumentException("MovementDate is mandatory.");
            Set_Value("MovementDate", (DateTime?)MovementDate.Value.Date);
        }
        /** Get Movement Date.
        @return Date a product was moved in or out of inventory */
        public DateTime? GetMovementDate()
        {
            return (DateTime?)Get_Value("MovementDate");
        }

        /** MovementType AD_Reference_ID=189 */
        public static int MOVEMENTTYPE_AD_Reference_ID = 189;
        /** Customer Returns = C+ */
        public static String MOVEMENTTYPE_CustomerReturns = "C+";
        /** Customer Shipment = C- */
        public static String MOVEMENTTYPE_CustomerShipment = "C-";
        /** Inventory In = I+ */
        public static String MOVEMENTTYPE_InventoryIn = "I+";
        /** Inventory Out = I- */
        public static String MOVEMENTTYPE_InventoryOut = "I-";
        /** Movement To = M+ */
        public static String MOVEMENTTYPE_MovementTo = "M+";
        /** Movement From = M- */
        public static String MOVEMENTTYPE_MovementFrom = "M-";
        /** Production + = P+ */
        public static String MOVEMENTTYPE_ProductionPlus = "P+";
        /** Production - = P- */
        public static String MOVEMENTTYPE_Production_ = "P-";
        /** Vendor Receipts = V+ */
        public static String MOVEMENTTYPE_VendorReceipts = "V+";
        /** Vendor Returns = V- */
        public static String MOVEMENTTYPE_VendorReturns = "V-";
        /** Work Order + = W+ */
        public static String MOVEMENTTYPE_WorkOrderPlus = "W+";
        /** Work Order - = W- */
        public static String MOVEMENTTYPE_WorkOrder_ = "W-";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsMovementTypeValid(String test)
        {
            return test.Equals("C+") || test.Equals("C-") || test.Equals("I+") || test.Equals("I-") || test.Equals("M+") || test.Equals("M-") || test.Equals("P+") || test.Equals("P-") || test.Equals("V+") || test.Equals("V-") || test.Equals("W+") || test.Equals("W-");
        }
        /** Set Movement Type.
        @param MovementType Method of moving the inventory */
        public void SetMovementType(String MovementType)
        {
            if (MovementType == null) throw new ArgumentException("MovementType is mandatory");
            if (!IsMovementTypeValid(MovementType))
                throw new ArgumentException("MovementType Invalid value - " + MovementType + " - Reference_ID=189 - C+ - C- - I+ - I- - M+ - M- - P+ - P- - V+ - V- - W+ - W-");
            if (MovementType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                MovementType = MovementType.Substring(0, 2);
            }
            Set_ValueNoCheck("MovementType", MovementType);
        }
        /** Get Movement Type.
        @return Method of moving the inventory */
        public String GetMovementType()
        {
            return (String)Get_Value("MovementType");
        }
        /** Set No Packages.
        @param NoPackages Number of packages shipped */
        public void SetNoPackages(int NoPackages)
        {
            Set_Value("NoPackages", NoPackages);
        }
        /** Get No Packages.
        @return Number of packages shipped */
        public int GetNoPackages()
        {
            Object ii = Get_Value("NoPackages");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_InOut_ID AD_Reference_ID=337 */
        public static int ORIG_INOUT_ID_AD_Reference_ID = 337;
        /** Set Orig Shipment.
        @param Orig_InOut_ID Original shipment of the RMA */
        public void SetOrig_InOut_ID(int Orig_InOut_ID)
        {
            throw new ArgumentException("Orig_InOut_ID Is virtual column");
        }
        /** Get Orig Shipment.
        @return Original shipment of the RMA */
        public int GetOrig_InOut_ID()
        {
            Object ii = Get_Value("Orig_InOut_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Orig_Order_ID AD_Reference_ID=290 */
        public static int ORIG_ORDER_ID_AD_Reference_ID = 290;
        /** Set Orig Sales Order.
        @param Orig_Order_ID Original Sales Order for Return Material Authorization */
        public void SetOrig_Order_ID(int Orig_Order_ID)
        {
            throw new ArgumentException("Orig_Order_ID Is virtual column");
        }
        /** Get Orig Sales Order.
        @return Original Sales Order for Return Material Authorization */
        public int GetOrig_Order_ID()
        {
            Object ii = Get_Value("Orig_Order_ID");
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
            Set_Value("POReference", POReference);
        }
        /** Get Order Reference.
        @return Transaction Reference Number (Sales Order, Purchase Order) of your Business Partner */
        public String GetPOReference()
        {
            return (String)Get_Value("POReference");
        }
        /** Set Pick Date.
        @param PickDate Date/Time when picked for Shipment */
        public void SetPickDate(DateTime? PickDate)
        {
            Set_Value("PickDate", (DateTime?)PickDate);
        }
        /** Get Pick Date.
        @return Date/Time when picked for Shipment */
        public DateTime? GetPickDate()
        {
            return (DateTime?)Get_Value("PickDate");
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_Value("Posted", Posted);
        }
        /** Get Posted.
        @return Posting status */
        public Boolean IsPosted()
        {
            Object oo = Get_Value("Posted");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** PriorityRule AD_Reference_ID=154 */
        public static int PRIORITYRULE_AD_Reference_ID = 154;
        /** Urgent = 1 */
        public static String PRIORITYRULE_Urgent = "1";
        /** High = 3 */
        public static String PRIORITYRULE_High = "3";
        /** Medium = 5 */
        public static String PRIORITYRULE_Medium = "5";
        /** Low = 7 */
        public static String PRIORITYRULE_Low = "7";
        /** Minor = 9 */
        public static String PRIORITYRULE_Minor = "9";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsPriorityRuleValid(String test)
        {
            return test.Equals("1") || test.Equals("3") || test.Equals("5") || test.Equals("7") || test.Equals("9");
        }
        /** Set Priority.
        @param PriorityRule Priority of a document */
        public void SetPriorityRule(String PriorityRule)
        {
            if (PriorityRule == null) throw new ArgumentException("PriorityRule is mandatory");
            if (!IsPriorityRuleValid(PriorityRule))
                throw new ArgumentException("PriorityRule Invalid value - " + PriorityRule + " - Reference_ID=154 - 1 - 3 - 5 - 7 - 9");
            if (PriorityRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                PriorityRule = PriorityRule.Substring(0, 1);
            }
            Set_Value("PriorityRule", PriorityRule);
        }
        /** Get Priority.
        @return Priority of a document */
        public String GetPriorityRule()
        {
            return (String)Get_Value("PriorityRule");
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
        /** Set Referenced Shipment.
        @param Ref_InOut_ID Referenced Shipment */
        public void SetRef_InOut_ID(int Ref_InOut_ID)
        {
            if (Ref_InOut_ID <= 0) Set_Value("Ref_InOut_ID", null);
            else
                Set_Value("Ref_InOut_ID", Ref_InOut_ID);
        }
        /** Get Referenced Shipment.
        @return Referenced Shipment */
        public int GetRef_InOut_ID()
        {
            Object ii = Get_Value("Ref_InOut_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Ship Date.
        @param ShipDate Shipment Date/Time */
        public void SetShipDate(DateTime? ShipDate)
        {
            Set_Value("ShipDate", (DateTime?)ShipDate);
        }
        /** Get Ship Date.
        @return Shipment Date/Time */
        public DateTime? GetShipDate()
        {
            return (DateTime?)Get_Value("ShipDate");
        }
        /** Set Tracking No.
        @param TrackingNo Number to track the shipment */
        public void SetTrackingNo(String TrackingNo)
        {
            if (TrackingNo != null && TrackingNo.Length > 60)
            {
                log.Warning("Length > 60 - truncated");
                TrackingNo = TrackingNo.Substring(0, 60);
            }
            Set_Value("TrackingNo", TrackingNo);
        }
        /** Get Tracking No.
        @return Number to track the shipment */
        public String GetTrackingNo()
        {
            return (String)Get_Value("TrackingNo");
        }

        /** User1_ID AD_Reference_ID=134 */
        public static int USER1_ID_AD_Reference_ID = 134;
        /** Set User List 1.
        @param User1_ID User defined list element #1 */
        public void SetUser1_ID(int User1_ID)
        {
            if (User1_ID <= 0) Set_Value("User1_ID", null);
            else
                Set_Value("User1_ID", User1_ID);
        }
        /** Get User List 1.
        @return User defined list element #1 */
        public int GetUser1_ID()
        {
            Object ii = Get_Value("User1_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** User2_ID AD_Reference_ID=137 */
        public static int USER2_ID_AD_Reference_ID = 137;
        /** Set User List 2.
        @param User2_ID User defined list element #2 */
        public void SetUser2_ID(int User2_ID)
        {
            if (User2_ID <= 0) Set_Value("User2_ID", null);
            else
                Set_Value("User2_ID", User2_ID);
        }
        /** Get User List 2.
        @return User defined list element #2 */
        public int GetUser2_ID()
        {
            Object ii = Get_Value("User2_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Volume.
        @param Volume Volume of a product */
        public void SetVolume(Decimal? Volume)
        {
            Set_Value("Volume", (Decimal?)Volume);
        }
        /** Get Volume.
        @return Volume of a product */
        public Decimal GetVolume()
        {
            Object bd = Get_Value("Volume");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Weight.
        @param Weight Weight of a product */
        public void SetWeight(Decimal? Weight)
        {
            Set_Value("Weight", (Decimal?)Weight);
        }
        /** Get Weight.
        @return Weight of a product */
        public Decimal GetWeight()
        {
            Object bd = Get_Value("Weight");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Package.
        @param M_Package_ID Shipment Package */
        public void SetM_Package_ID(int M_Package_ID)
        {
            if (M_Package_ID <= 0) Set_Value("M_Package_ID", null);
            else
                Set_Value("M_Package_ID", M_Package_ID);
        }
        /** Get Package.
        @return Shipment Package */
        public int GetM_Package_ID()
        {
            Object ii = Get_Value("M_Package_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** SAP001_ShipToParty AD_Reference_ID=1000197 */
        public static int SAP001_SHIPTOPARTY_AD_Reference_ID = 1000197;
        /** Set Ship-To-Party.
        @param SAP001_ShipToParty Ship-To-Party */
        public void SetSAP001_ShipToParty(int SAP001_ShipToParty)
        {
            Set_Value("SAP001_ShipToParty", SAP001_ShipToParty);
        }
        /** Get Ship-To-Party.
        @return Ship-To-Party */
        public int GetSAP001_ShipToParty()
        {
            Object ii = Get_Value("SAP001_ShipToParty");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        //Added By Amit 3-8-2015 VAMRP
        /** Set DS_AREA_ID.
        @param DS_AREA_ID DS_AREA_ID */
        public void SetDS_AREA_ID(int DS_AREA_ID)
        {
            if (DS_AREA_ID <= 0) Set_Value("DS_AREA_ID", null);
            else
                Set_Value("DS_AREA_ID", DS_AREA_ID);
        }
        /** Get DS_AREA_ID.
        @return DS_AREA_ID */
        public int GetDS_AREA_ID()
        {
            Object ii = Get_Value("DS_AREA_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set DS_SUBLOCATION_ID.
        @param DS_SUBLOCATION_ID DS_SUBLOCATION_ID */
        public void SetDS_SUBLOCATION_ID(int DS_SUBLOCATION_ID)
        {
            if (DS_SUBLOCATION_ID <= 0) Set_Value("DS_SUBLOCATION_ID", null);
            else
                Set_Value("DS_SUBLOCATION_ID", DS_SUBLOCATION_ID);
        }
        /** Get DS_SUBLOCATION_ID.
        @return DS_SUBLOCATION_ID */
        public int GetDS_SUBLOCATION_ID()
        {
            Object ii = Get_Value("DS_SUBLOCATION_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        //END

        /** Set Cost Calculated.@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }
        /** Get Cost Calculated. @return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Reversed Cost Calculated.@param IsReversedCostCalculated Reversed Cost Calculated */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }
        /** Get Reversed Cost Calculated. @return Reversed Cost Calculated */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Future Cost Calculated.@param IsFutureCostCalculated Future Cost Calculated */
        public void SetIsFutureCostCalculated(Boolean IsFutureCostCalculated) { Set_Value("IsFutureCostCalculated", IsFutureCostCalculated); }
        /** Get Future Cost Calculated.@return Future Cost Calculated */
        public Boolean IsFutureCostCalculated() { Object oo = Get_Value("IsFutureCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Cost Sheet.
    @param VA033_CostSheet_ID Cost Sheet */
        public void SetVA033_CostSheet_ID(int VA033_CostSheet_ID)
        {
            if (VA033_CostSheet_ID <= 0) Set_Value("VA033_CostSheet_ID", null);
            else
                Set_Value("VA033_CostSheet_ID", VA033_CostSheet_ID);
        }
        /** Get Cost Sheet.
        @return Cost Sheet */
        public int GetVA033_CostSheet_ID()
        {
            Object ii = Get_Value("VA033_CostSheet_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Is CostSheet.
        @param VA033_IsCostSheet Is CostSheet */
        public void SetVA033_IsCostSheet(Boolean VA033_IsCostSheet)
        {
            Set_Value("VA033_IsCostSheet", VA033_IsCostSheet);
        }
        /** Get Is CostSheet.
        @return Is CostSheet */
        public Boolean IsVA033_IsCostSheet()
        {
            Object oo = Get_Value("VA033_IsCostSheet");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Drop Shipment.
        @param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip)
        {
            Set_Value("IsDropShip", IsDropShip);
        }
        /** Get Drop Shipment.
        @return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip()
        {
            Object oo = Get_Value("IsDropShip");
            if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); }
            return false;
        }
        /** Set Shipment & MR References.
        @param Ref_ShipMR_ID Shipment & MR References */
        public void SetRef_ShipMR_ID(int Ref_ShipMR_ID)
        {
            if (Ref_ShipMR_ID <= 0) Set_Value("Ref_ShipMR_ID", null);
            else
                Set_Value("Ref_ShipMR_ID", Ref_ShipMR_ID);
        }/** Get Shipment & MR References.
        @return Shipment & MR References */
        public int GetRef_ShipMR_ID()
        {
            Object ii = Get_Value("Ref_ShipMR_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Inco Term.
        @param C_IncoTerm_ID Inco term will be used to create or define the Inco term based on client requirement */
        public void SetC_IncoTerm_ID(int C_IncoTerm_ID)
        {
            if (C_IncoTerm_ID <= 0)
                Set_Value("C_IncoTerm_ID", null);
            else
                Set_Value("C_IncoTerm_ID", C_IncoTerm_ID);
        }
        /** Get Inco Term.
        @return Inco term will be used to create or define the Inco term based on client requirement */
        public int GetC_IncoTerm_ID()
        {
            Object ii = Get_Value("C_IncoTerm_ID");
            if (ii == null)
                return 0;
            return Convert.ToInt32(ii);
        }

        /** ReversalDoc_ID AD_Reference_ID=337 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 337;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }


        /** Set Reversal.@param IsReversal This is a reversing transaction */
        public void SetIsReversal(Boolean IsReversal) { Set_Value("IsReversal", IsReversal); }
        /** Get Reversal.@return This is a reversing transaction */
        public Boolean IsReversal() { Object oo = Get_Value("IsReversal"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Temp Document No.
        @param TempDocumentNo Temp Document No for this Document */
        public void SetTempDocumentNo(String TempDocumentNo)
        {
            if (TempDocumentNo != null && TempDocumentNo.Length > 30)
            {
                log.Warning("Length > 30 - truncated");
                TempDocumentNo = TempDocumentNo.Substring(0, 30);
            }
            Set_Value("TempDocumentNo", TempDocumentNo);
        }
        /** Get Temp Document No.
        @return Temp Document No for this Document */
        public String GetTempDocumentNo()
        {
            return (String)Get_Value("TempDocumentNo");
        }
    }

}
