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
    /** Generated Model for C_Project
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_Project : PO
    {
        public X_C_Project(Context ctx, int C_Project_ID, Trx trxName)
            : base(ctx, C_Project_ID, trxName)
        {
            /** if (C_Project_ID == 0)
            {
            SetC_Project_ID (0);
            SetCommittedAmt (0.0);
            SetCommittedQty (0.0);
            SetInvoicedAmt (0.0);
            SetInvoicedQty (0.0);
            SetIsCommitCeiling (false);
            SetIsCommitment (false);
            SetIsSummary (false);
            SetName (null);
            SetPlannedAmt (0.0);
            SetPlannedMarginAmt (0.0);
            SetPlannedQty (0.0);
            SetProcessed (false);	// N
            SetProjInvoiceRule (null);	// -
            SetProjectBalanceAmt (0.0);
            SetProjectLineLevel (null);	// P
            SetValue (null);
            }
             */
        }
        public X_C_Project(Ctx ctx, int C_Project_ID, Trx trxName)
            : base(ctx, C_Project_ID, trxName)
        {
            /** if (C_Project_ID == 0)
            {
            SetC_Project_ID (0);
            SetCommittedAmt (0.0);
            SetCommittedQty (0.0);
            SetInvoicedAmt (0.0);
            SetInvoicedQty (0.0);
            SetIsCommitCeiling (false);
            SetIsCommitment (false);
            SetIsSummary (false);
            SetName (null);
            SetPlannedAmt (0.0);
            SetPlannedMarginAmt (0.0);
            SetPlannedQty (0.0);
            SetProcessed (false);	// N
            SetProjInvoiceRule (null);	// -
            SetProjectBalanceAmt (0.0);
            SetProjectLineLevel (null);	// P
            SetValue (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Project(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Project(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_Project(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_Project()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514374162L;
        /** Last Updated Timestamp 7/29/2010 1:07:37 PM */
        public static long updatedMS = 1280389057373L;
        /** AD_Table_ID=203 */
        public static int Table_ID;
        // =203;

        /** TableName=C_Project */
        public static String Table_Name = "C_Project";

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
            StringBuilder sb = new StringBuilder("X_C_Project[").Append(Get_ID()).Append("]");
            return sb.ToString();
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

        /** C_BPartnerSR_ID AD_Reference_ID=353 */
        public static int C_BPARTNERSR_ID_AD_Reference_ID = 353;
        /** Set BPartner (Agent).
        @param C_BPartnerSR_ID Business Partner (Agent or Sales Rep) */
        public void SetC_BPartnerSR_ID(int C_BPartnerSR_ID)
        {
            if (C_BPartnerSR_ID <= 0) Set_Value("C_BPartnerSR_ID", null);
            else
                Set_Value("C_BPartnerSR_ID", C_BPartnerSR_ID);
        }
        /** Get BPartner (Agent).
        @return Business Partner (Agent or Sales Rep) */
        public int GetC_BPartnerSR_ID()
        {
            Object ii = Get_Value("C_BPartnerSR_ID");
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
        /** Set Standard Phase.
        @param C_Phase_ID Standard Phase of the Project Type */
        public void SetC_Phase_ID(int C_Phase_ID)
        {
            if (C_Phase_ID <= 0) Set_Value("C_Phase_ID", null);
            else
                Set_Value("C_Phase_ID", C_Phase_ID);
        }
        /** Get Standard Phase.
        @return Standard Phase of the Project Type */
        public int GetC_Phase_ID()
        {
            Object ii = Get_Value("C_Phase_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Type.
        @param C_ProjectType_ID Type of the project */
        public void SetC_ProjectType_ID(String C_ProjectType_ID)
        {
            if (C_ProjectType_ID != null && C_ProjectType_ID.Length > 22)
            {
                log.Warning("Length > 22 - truncated");
                C_ProjectType_ID = C_ProjectType_ID.Substring(0, 22);
            }
            Set_Value("C_ProjectType_ID", C_ProjectType_ID);
        }
        /** Get Project Type.
        @return Type of the project */
        public String GetC_ProjectType_ID()
        {
            return (String)Get_Value("C_ProjectType_ID");
        }
        /** Set Project.
        @param C_Project_ID Financial Project */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID < 1) throw new ArgumentException("C_Project_ID is mandatory.");
            Set_ValueNoCheck("C_Project_ID", C_Project_ID);
        }
        /** Get Project.
        @return Financial Project */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Sales Region.
        @param C_SalesRegion_ID Sales coverage region */
        public void SetC_SalesRegion_ID(int C_SalesRegion_ID)
        {
            if (C_SalesRegion_ID <= 0) Set_Value("C_SalesRegion_ID", null);
            else
                Set_Value("C_SalesRegion_ID", C_SalesRegion_ID);
        }
        /** Get Sales Region.
        @return Sales coverage region */
        public int GetC_SalesRegion_ID()
        {
            Object ii = Get_Value("C_SalesRegion_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Committed Amount.
        @param CommittedAmt The (legal) commitment amount */
        public void SetCommittedAmt(Decimal? CommittedAmt)
        {
            if (CommittedAmt == null) throw new ArgumentException("CommittedAmt is mandatory.");
            Set_Value("CommittedAmt", (Decimal?)CommittedAmt);
        }
        /** Get Committed Amount.
        @return The (legal) commitment amount */
        public Decimal GetCommittedAmt()
        {
            Object bd = Get_Value("CommittedAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Committed Quantity.
        @param CommittedQty The (legal) commitment Quantity */
        public void SetCommittedQty(Decimal? CommittedQty)
        {
            if (CommittedQty == null) throw new ArgumentException("CommittedQty is mandatory.");
            Set_Value("CommittedQty", (Decimal?)CommittedQty);
        }
        /** Get Committed Quantity.
        @return The (legal) commitment Quantity */
        public Decimal GetCommittedQty()
        {
            Object bd = Get_Value("CommittedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Copy From.
        @param CopyFrom Copy From Record */
        public void SetCopyFrom(String CopyFrom)
        {
            if (CopyFrom != null && CopyFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                CopyFrom = CopyFrom.Substring(0, 1);
            }
            Set_Value("CopyFrom", CopyFrom);
        }
        /** Get Copy From.
        @return Copy From Record */
        public String GetCopyFrom()
        {
            return (String)Get_Value("CopyFrom");
        }
        /** Set Contract Date.
        @param DateContract The (planned) effective date of this document. */
        public void SetDateContract(DateTime? DateContract)
        {
            Set_Value("DateContract", (DateTime?)DateContract);
        }
        /** Get Contract Date.
        @return The (planned) effective date of this document. */
        public DateTime? GetDateContract()
        {
            return (DateTime?)Get_Value("DateContract");
        }
        /** Set Finish Date.
        @param DateFinish Finish or (planned) completion date */
        public void SetDateFinish(DateTime? DateFinish)
        {
            Set_Value("DateFinish", (DateTime?)DateFinish);
        }

        public void SetIsCampaign(Boolean IsCampaign)
        {
            Set_Value("IsCampaign", IsCampaign);
        }
        /** Get Finish Date.
        @return Finish or (planned) completion date */
        public DateTime? GetDateFinish()
        {
            return (DateTime?)Get_Value("DateFinish");
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
        /** Set Invoiced Amount.
        @param InvoicedAmt The amount invoiced */
        public void SetInvoicedAmt(Decimal? InvoicedAmt)
        {
            if (InvoicedAmt == null) throw new ArgumentException("InvoicedAmt is mandatory.");
            Set_ValueNoCheck("InvoicedAmt", (Decimal?)InvoicedAmt);
        }
        /** Get Invoiced Amount.
        @return The amount invoiced */
        public Decimal GetInvoicedAmt()
        {
            Object bd = Get_Value("InvoicedAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Invoiced.
        @param InvoicedQty The quantity invoiced */
        public void SetInvoicedQty(Decimal? InvoicedQty)
        {
            if (InvoicedQty == null) throw new ArgumentException("InvoicedQty is mandatory.");
            Set_ValueNoCheck("InvoicedQty", (Decimal?)InvoicedQty);
        }
        /** Get Quantity Invoiced.
        @return The quantity invoiced */
        public Decimal GetInvoicedQty()
        {
            Object bd = Get_Value("InvoicedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Commitment is Ceiling.
        @param IsCommitCeiling The commitment amount/quantity is the chargeable ceiling */
        public void SetIsCommitCeiling(Boolean IsCommitCeiling)
        {
            Set_Value("IsCommitCeiling", IsCommitCeiling);
        }
        /** Get Commitment is Ceiling.
        @return The commitment amount/quantity is the chargeable ceiling */
        public Boolean IsCommitCeiling()
        {
            Object oo = Get_Value("IsCommitCeiling");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Commitment.
        @param IsCommitment Is this document a (legal) commitment? */
        public void SetIsCommitment(Boolean IsCommitment)
        {
            Set_Value("IsCommitment", IsCommitment);
        }
        /** Get Commitment.
        @return Is this document a (legal) commitment? */
        public Boolean IsCommitment()
        {
            Object oo = Get_Value("IsCommitment");
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
        /** Set Price List Version.
        @param M_PriceList_Version_ID Identifies a unique instance of a Price List */
        public void SetM_PriceList_Version_ID(int M_PriceList_Version_ID)
        {
            if (M_PriceList_Version_ID <= 0) Set_Value("M_PriceList_Version_ID", null);
            else
                Set_Value("M_PriceList_Version_ID", M_PriceList_Version_ID);
        }
        /** Get Price List Version.
        @return Identifies a unique instance of a Price List */
        public int GetM_PriceList_Version_ID()
        {
            Object ii = Get_Value("M_PriceList_Version_ID");
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
        /** Set Note.
        @param Note Optional additional user defined information */
        public void SetNote(String Note)
        {
            if (Note != null && Note.Length > 2000)
            {
                log.Warning("Length > 2000 - truncated");
                Note = Note.Substring(0, 2000);
            }
            Set_Value("Note", Note);
        }
        /** Get Note.
        @return Optional additional user defined information */
        public String GetNote()
        {
            return (String)Get_Value("Note");
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
        /** Set Planned Amount.
        @param PlannedAmt Planned amount for this project */
        public void SetPlannedAmt(Decimal? PlannedAmt)
        {
            if (PlannedAmt == null) throw new ArgumentException("PlannedAmt is mandatory.");
            Set_Value("PlannedAmt", (Decimal?)PlannedAmt);
        }
        /** Get Planned Amount.
        @return Planned amount for this project */
        public Decimal GetPlannedAmt()
        {
            Object bd = Get_Value("PlannedAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Planned Date.
        @param PlannedDate Date projected */
        public void SetPlannedDate(DateTime? PlannedDate)
        {
            Set_Value("PlannedDate", (DateTime?)PlannedDate);
        }
        /** Get Planned Date.
        @return Date projected */
        public DateTime? GetPlannedDate()
        {
            return (DateTime?)Get_Value("PlannedDate");
        }
        /** Set Planned Margin.
        @param PlannedMarginAmt Project's planned margin amount */
        public void SetPlannedMarginAmt(Decimal? PlannedMarginAmt)
        {
            if (PlannedMarginAmt == null) throw new ArgumentException("PlannedMarginAmt is mandatory.");
            Set_Value("PlannedMarginAmt", (Decimal?)PlannedMarginAmt);
        }
        /** Get Planned Margin.
        @return Project's planned margin amount */
        public Decimal GetPlannedMarginAmt()
        {
            Object bd = Get_Value("PlannedMarginAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Planned Quantity.
        @param PlannedQty Planned quantity for this project */
        public void SetPlannedQty(Decimal? PlannedQty)
        {
            if (PlannedQty == null) throw new ArgumentException("PlannedQty is mandatory.");
            Set_Value("PlannedQty", (Decimal?)PlannedQty);
        }
        /** Get Planned Quantity.
        @return Planned quantity for this project */
        public Decimal GetPlannedQty()
        {
            Object bd = Get_Value("PlannedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Probability.
        @param Probability Probability in Percent */
        public void SetProbability(int Probability)
        {
            Set_Value("Probability", Probability);
        }
        /** Get Probability.
        @return Probability in Percent */
        public int GetProbability()
        {
            Object ii = Get_Value("Probability");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** ProjInvoiceRule AD_Reference_ID=383 */
        public static int PROJINVOICERULE_AD_Reference_ID = 383;
        /** None = - */
        public static String PROJINVOICERULE_None = "-";
        /** Committed Amount = C */
        public static String PROJINVOICERULE_CommittedAmount = "C";
        /** Product  Quantity = P */
        public static String PROJINVOICERULE_ProductQuantity = "P";
        /** Time&Material = T */
        public static String PROJINVOICERULE_TimeMaterial = "T";
        /** Time&Material max Comitted = c */
        public static String PROJINVOICERULE_TimeMaterialMaxComitted = "c";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProjInvoiceRuleValid(String test)
        {
            return test.Equals("-") || test.Equals("C") || test.Equals("P") || test.Equals("T") || test.Equals("c");
        }
        /** Set Invoice Rule.
        @param ProjInvoiceRule Invoice Rule for the project */
        public void SetProjInvoiceRule(String ProjInvoiceRule)
        {
            if (ProjInvoiceRule == null) throw new ArgumentException("ProjInvoiceRule is mandatory");
            if (!IsProjInvoiceRuleValid(ProjInvoiceRule))
                throw new ArgumentException("ProjInvoiceRule Invalid value - " + ProjInvoiceRule + " - Reference_ID=383 - - - C - P - T - c");
            if (ProjInvoiceRule.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProjInvoiceRule = ProjInvoiceRule.Substring(0, 1);
            }
            Set_Value("ProjInvoiceRule", ProjInvoiceRule);
        }
        /** Get Invoice Rule.
        @return Invoice Rule for the project */
        public String GetProjInvoiceRule()
        {
            return (String)Get_Value("ProjInvoiceRule");
        }
        /** Set Project Balance.
        @param ProjectBalanceAmt Total Project Balance */
        public void SetProjectBalanceAmt(Decimal? ProjectBalanceAmt)
        {
            if (ProjectBalanceAmt == null) throw new ArgumentException("ProjectBalanceAmt is mandatory.");
            Set_ValueNoCheck("ProjectBalanceAmt", (Decimal?)ProjectBalanceAmt);
        }
        /** Get Project Balance.
        @return Total Project Balance */
        public Decimal GetProjectBalanceAmt()
        {
            Object bd = Get_Value("ProjectBalanceAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** ProjectCategory AD_Reference_ID=288 */
        public static int PROJECTCATEGORY_AD_Reference_ID = 288;
        /** Asset Project = A */
        public static String PROJECTCATEGORY_AssetProject = "A";
        /** General = N */
        public static String PROJECTCATEGORY_General = "N";
        /** Service (Charge) Project = S */
        public static String PROJECTCATEGORY_ServiceChargeProject = "S";
        /** Work Order (Job) = W */
        public static String PROJECTCATEGORY_WorkOrderJob = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProjectCategoryValid(String test)
        {
            return test == null || test.Equals("A") || test.Equals("N") || test.Equals("S") || test.Equals("W");
        }
        /** Set Project Category.
        @param ProjectCategory Project Category */
        public void SetProjectCategory(String ProjectCategory)
        {
            if (!IsProjectCategoryValid(ProjectCategory))
                throw new ArgumentException("ProjectCategory Invalid value - " + ProjectCategory + " - Reference_ID=288 - A - N - S - W");
            if (ProjectCategory != null && ProjectCategory.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProjectCategory = ProjectCategory.Substring(0, 1);
            }
            Set_Value("ProjectCategory", ProjectCategory);
        }
        /** Get Project Category.
        @return Project Category */
        public String GetProjectCategory()
        {
            return (String)Get_Value("ProjectCategory");
        }

        /** ProjectLineLevel AD_Reference_ID=384 */
        public static int PROJECTLINELEVEL_AD_Reference_ID = 384;
        /** Phase = A */
        public static String PROJECTLINELEVEL_Phase = "A";
        /** Project = P */
        public static String PROJECTLINELEVEL_Project = "P";
        /** Task = T */
        public static String PROJECTLINELEVEL_Task = "T";
        //Added new value taskline in project line level sugested by Mukesh sir
        /** Task Line = Y */
        public static String PROJECTLINELEVEL_TaskLine = "Y";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsProjectLineLevelValid(String test)
        {
            return test.Equals("A") || test.Equals("P") || test.Equals("T") || test.Equals("Y");
        }
        /** Set Line Level.
        @param ProjectLineLevel Project Line Level */
        public void SetProjectLineLevel(String ProjectLineLevel)
        {
            if (ProjectLineLevel == null) throw new ArgumentException("ProjectLineLevel is mandatory");
            if (!IsProjectLineLevelValid(ProjectLineLevel))
                throw new ArgumentException("ProjectLineLevel Invalid value - " + ProjectLineLevel + " - Reference_ID=384 - A - P - T - Y");
            if (ProjectLineLevel.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                ProjectLineLevel = ProjectLineLevel.Substring(0, 1);
            }
            Set_Value("ProjectLineLevel", ProjectLineLevel);
        }
        /** Get Line Level.
        @return Project Line Level */
        public String GetProjectLineLevel()
        {
            return (String)Get_Value("ProjectLineLevel");
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
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetValue());
        }

        /** Set C_Lead_ID.
        @param C_Lead_ID( Identifies a C_Lead_ID */
        public void SetC_Lead_ID(int C_Lead_ID)
        {
            if (C_Lead_ID <= 0) Set_Value("C_Lead_ID", null);
            else
                Set_Value("C_Lead_ID", C_Lead_ID);
        }
        /** Get C_Lead_ID.
        @return Identifies a C_Lead_ID  */
        public int GetC_Lead_ID()
        {
            Object ii = Get_Value("C_Lead_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Is Opportunity.
        @param IsOpportunity Is Opportunity */
        public void SetIsOpportunity(Boolean IsOpportunity)
        {
            Set_Value("IsOpportunity", IsOpportunity);
        }
        /** Get Is Opportunity.
        @return Is Opportunity */
        public Boolean IsOpportunity()
        {
            Object oo = Get_Value("IsOpportunity");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** Ref_Order_ID AD_Reference_ID=290 */
        public static int REF_ORDER_ID_AD_Reference_ID = 290;
        /** Set Referenced Order.
        @param Ref_Order_ID Reference to corresponding Sales/Purchase Order */
        public void SetRef_Order_ID(int Ref_Order_ID)
        {
            if (Ref_Order_ID <= 0) Set_Value("Ref_Order_ID", null);
            else
                Set_Value("Ref_Order_ID", Ref_Order_ID);
        }
        /** Get Referenced Order.
        @return Reference to corresponding Sales/Purchase Order */
        public int GetRef_Order_ID()
        {
            Object ii = Get_Value("Ref_Order_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Generate Quotation.
        @param Generate_Quotation Generate Quotation */
        public void SetGenerate_Quotation(String Generate_Quotation)
        {
            if (Generate_Quotation != null && Generate_Quotation.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Generate_Quotation = Generate_Quotation.Substring(0, 50);
            }
            Set_Value("Generate_Quotation", Generate_Quotation);
        }
        /** Get Generate Quotation.
        @return Generate Quotation */
        public String GetGenerate_Quotation()
        {
            return (String)Get_Value("Generate_Quotation");
        }

        /** C_Order_ID AD_Reference_ID=290 */
        public static int C_ORDER_ID_AD_Reference_ID = 290;
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

        /** Set Generate Order.
        @param Generate_Order Generate Order */
        public void SetGenerate_Order(String Generate_Order)
        {
            if (Generate_Order != null && Generate_Order.Length > 50)
            {
                log.Warning("Length > 50 - truncated");
                Generate_Order = Generate_Order.Substring(0, 50);
            }
            Set_Value("Generate_Order", Generate_Order);
        }
        /** Get Generate Order.
        @return Generate Order */
        public String GetGenerate_Order()
        {
            return (String)Get_Value("Generate_Order");
        }

        /** Set Price List.
        @param M_PriceList_ID Identifies a unique instance of a Price List */
        public void SetM_PriceList_ID(int M_PriceList_ID)
        {
            if (M_PriceList_ID <= 0) Set_Value("M_PriceList_ID", null);
            else
                Set_Value("M_PriceList_ID", M_PriceList_ID);
        }
        /** Get Price List.
        @return Identifies a unique instance of a Price List */
        public int GetM_PriceList_ID()
        {
            Object ii = Get_Value("M_PriceList_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Stage AD_Reference_ID=1000093 */
        public static int STAGE_AD_Reference_ID = 1000093;
        /** Closed/Lost = L */
        public static String STAGE_ClosedLost = "L";
        /** Negotiation = N */
        public static String STAGE_Negotiation = "N";
        /** Open = O */
        public static String STAGE_Open = "O";
        /** Closed/Won = W */
        public static String STAGE_ClosedWon = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsStageValid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("N") || test.Equals("O") || test.Equals("W");
        }
        /** Set Stage.
        @param Stage Stage */
        public void SetStage(String Stage)
        {
            if (!IsStageValid(Stage))
                throw new ArgumentException("Stage Invalid value - " + Stage + " - Reference_ID=1000093 - L - N - O - W");
            if (Stage != null && Stage.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                Stage = Stage.Substring(0, 1);
            }
            Set_Value("Stage", Stage);
        }
        /** Get Stage.
        @return Stage */
        public String GetStage()
        {
            return (String)Get_Value("Stage");
        }

        /** Set Source.
        @param R_Source_ID Source for the Lead or Request */
        public void SetR_Source_ID(int R_Source_ID)
        {
            if (R_Source_ID <= 0) Set_Value("R_Source_ID", null);
            else
                Set_Value("R_Source_ID", R_Source_ID);
        }
        /** Get Source.
        @return Source for the Lead or Request */
        public int GetR_Source_ID()
        {
            Object ii = Get_Value("R_Source_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** OpportunityStatus AD_Reference_ID=1000094 */
        public static int OPPORTUNITYSTATUS_AD_Reference_ID = 1000094;
        /** Lost = L */
        public static String OPPORTUNITYSTATUS_Lost = "L";
        /** New = N */
        public static String OPPORTUNITYSTATUS_New = "N";
        /** Pending = P */
        public static String OPPORTUNITYSTATUS_Pending = "P";
        /** Win = W */
        public static String OPPORTUNITYSTATUS_Win = "W";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsOpportunityStatusValid(String test)
        {
            return test == null || test.Equals("L") || test.Equals("N") || test.Equals("P") || test.Equals("W");
        }
        /** Set Status.
        @param OpportunityStatus Status */
        public void SetOpportunityStatus(String OpportunityStatus)
        {
            if (!IsOpportunityStatusValid(OpportunityStatus))
                throw new ArgumentException("OpportunityStatus Invalid value - " + OpportunityStatus + " - Reference_ID=1000094 - L - N - P - W");
            if (OpportunityStatus != null && OpportunityStatus.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                OpportunityStatus = OpportunityStatus.Substring(0, 1);
            }
            Set_Value("OpportunityStatus", OpportunityStatus);
        }
        /** Get Status.
        @return Status */
        public String GetOpportunityStatus()
        {
            return (String)Get_Value("OpportunityStatus");
        }
        /** Set Enquiry Received Date.
        @param C_EnquiryRdate Enquiry Received Date */
        public void SetC_EnquiryRdate(DateTime? C_EnquiryRdate) { Set_Value("C_EnquiryRdate", (DateTime?)C_EnquiryRdate); }/** Get Enquiry Received Date.
        @return Enquiry Received Date */
        public DateTime? GetC_EnquiryRdate() { return (DateTime?)Get_Value("C_EnquiryRdate"); }

        /** Set Proposal Due Date.
        @param C_ProposalDdate Proposal Due Date */
        public void SetC_ProposalDdate(DateTime? C_ProposalDdate) { Set_Value("C_ProposalDdate", (DateTime?)C_ProposalDdate); }/** Get Proposal Due Date.
        @return Proposal Due Date */
        public DateTime? GetC_ProposalDdate() { return (DateTime?)Get_Value("C_ProposalDdate"); }
        /** Set Prospects.
        @param Ref_BPartner_ID Identifies a Prospect */
        public void SetRef_BPartner_ID(int Ref_BPartner_ID)
        {
            if (Ref_BPartner_ID <= 0)
                Set_Value("Ref_BPartner_ID", null);
            else
                Set_Value("Ref_BPartner_ID", Ref_BPartner_ID);
        }/** Get Prospects.
        @return Identifies a Prospect */
        public int GetRef_BPartner_ID()
        {
            Object ii = Get_Value("Ref_BPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii);
        }

        /** Set Estimated Closing Date.
        @param VA077_EstClosingDate Use to identify the estimated Invoice date*/
        public void SetVA077_EstClosingDate(DateTime? VA077_EstClosingDate) { Set_Value("VA077_EstClosingDate", (DateTime?)VA077_EstClosingDate); }/** Get Estimated Closing Date.
@return Use to identify the estimated Invoice date */
        public DateTime? GetVA077_EstClosingDate() { return (DateTime?)Get_Value("VA077_EstClosingDate"); }/** Set Margin %.
@param VA077_MarginPercent It’s margin amount in percentage */
        public void SetVA077_MarginPercent(Decimal? VA077_MarginPercent) { Set_Value("VA077_MarginPercent", (Decimal?)VA077_MarginPercent); }/** Get Margin %.
@return It’s margin amount in percentage */
        public Decimal GetVA077_MarginPercent() { Object bd = Get_Value("VA077_MarginPercent"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** VA077_ProposalStatus AD_Reference_ID=1001024 */
        public static int VA077_PROPOSALSTATUS_AD_Reference_ID = 1001024;/** Deal Made = DM */
        public static String VA077_PROPOSALSTATUS_DealMade = "DM";/** Not Interested = NI */
        public static String VA077_PROPOSALSTATUS_NotInterested = "NI";/** Is test a valid value.
@param test testvalue
@returns true if valid **/
        public bool IsVA077_ProposalStatusValid(String test) { return test == null || test.Equals("DM") || test.Equals("NI"); }/** Set Proposal Status.
@param VA077_ProposalStatus Identifies the status of proposal */
        public void SetVA077_ProposalStatus(String VA077_ProposalStatus)
        {
            if (!IsVA077_ProposalStatusValid(VA077_ProposalStatus))
                throw new ArgumentException("VA077_ProposalStatus Invalid value - " + VA077_ProposalStatus + " - Reference_ID=1001024 - DM - NI"); if (VA077_ProposalStatus != null && VA077_ProposalStatus.Length > 2) { log.Warning("Length > 2 - truncated"); VA077_ProposalStatus = VA077_ProposalStatus.Substring(0, 2); }
            Set_Value("VA077_ProposalStatus", VA077_ProposalStatus);
        }/** Get Proposal Status.
@return Identifies the status of proposal */
        public String GetVA077_ProposalStatus() { return (String)Get_Value("VA077_ProposalStatus"); }
        /** VA077_SalesCoWorker AD_Reference_ID=1001025 */
        public static int VA077_SALESCOWORKER_AD_Reference_ID = 1001025;/** Set Sales co-worker.
@param VA077_SalesCoWorker Sales co-worker */
        public void SetVA077_SalesCoWorker(int VA077_SalesCoWorker) { Set_Value("VA077_SalesCoWorker", VA077_SalesCoWorker); }/** Get Sales co-worker.
@return Sales co-worker */
        public int GetVA077_SalesCoWorker() { Object ii = Get_Value("VA077_SalesCoWorker"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Sales Co-worker %.
@param VA077_SalesCoWorkerPer Identify the share of second sales person  */
        public void SetVA077_SalesCoWorkerPer(Decimal? VA077_SalesCoWorkerPer) { Set_Value("VA077_SalesCoWorkerPer", (Decimal?)VA077_SalesCoWorkerPer); }/** Get Sales Co-worker %.
@return Identify the share of second sales person  */
        public Decimal GetVA077_SalesCoWorkerPer() { Object bd = Get_Value("VA077_SalesCoWorkerPer"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Margin Amount.
@param VA077_TotalMarginAmt Indicate the total margin amount */
        public void SetVA077_TotalMarginAmt(Decimal? VA077_TotalMarginAmt) { Set_Value("VA077_TotalMarginAmt", (Decimal?)VA077_TotalMarginAmt); }/** Get Total Margin Amount.
@return Indicate the total margin amount */
        public Decimal GetVA077_TotalMarginAmt() { Object bd = Get_Value("VA077_TotalMarginAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Purchase Amount.
@param VA077_TotalPurchaseAmt Total Purchase Amount */
        public void SetVA077_TotalPurchaseAmt(Decimal? VA077_TotalPurchaseAmt) { Set_Value("VA077_TotalPurchaseAmt", (Decimal?)VA077_TotalPurchaseAmt); }/** Get Total Purchase Amount.
@return Total Purchase Amount */
        public Decimal GetVA077_TotalPurchaseAmt() { Object bd = Get_Value("VA077_TotalPurchaseAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Total Sales Amount.
@param VA077_TotalSalesAmt Total Sales Amount */
        public void SetVA077_TotalSalesAmt(Decimal? VA077_TotalSalesAmt) { Set_Value("VA077_TotalSalesAmt", (Decimal?)VA077_TotalSalesAmt); }/** Get Total Sales Amount.
@return Total Sales Amount */
        public Decimal GetVA077_TotalSalesAmt() { Object bd = Get_Value("VA077_TotalSalesAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

    }

}
