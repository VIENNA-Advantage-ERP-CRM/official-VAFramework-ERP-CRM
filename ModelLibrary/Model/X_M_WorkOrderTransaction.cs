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
    /** Generated Model for M_WorkOrderTransaction
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_WorkOrderTransaction : PO
    {
        public X_M_WorkOrderTransaction(Context ctx, int M_WorkOrderTransaction_ID, Trx trxName)
            : base(ctx, M_WorkOrderTransaction_ID, trxName)
        {
            /** if (M_WorkOrderTransaction_ID == 0)
            {
            SetC_DocType_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);	// @IsApproved@
            SetIsOptionalFrom (false);	// N
            SetIsOptionalTo (false);	// N
            SetM_Product_ID (0);
            SetM_WorkOrderTransaction_ID (0);
            SetM_WorkOrder_ID (0);
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            SetWOComplete (false);	// N
            SetWOTxnSource (null);	// M
            SetWorkOrderTxnType (null);
            }
             */
        }
        public X_M_WorkOrderTransaction(Ctx ctx, int M_WorkOrderTransaction_ID, Trx trxName)
            : base(ctx, M_WorkOrderTransaction_ID, trxName)
        {
            /** if (M_WorkOrderTransaction_ID == 0)
            {
            SetC_DocType_ID (0);
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetDateAcct (DateTime.Now);	// @#Date@
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);	// @IsApproved@
            SetIsOptionalFrom (false);	// N
            SetIsOptionalTo (false);	// N
            SetM_Product_ID (0);
            SetM_WorkOrderTransaction_ID (0);
            SetM_WorkOrder_ID (0);
            SetPosted (false);	// N
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            SetWOComplete (false);	// N
            SetWOTxnSource (null);	// M
            SetWorkOrderTxnType (null);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_WorkOrderTransaction(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_WorkOrderTransaction(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_WorkOrderTransaction(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_WorkOrderTransaction()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27581088222639L;
        /** Last Updated Timestamp 3/1/2011 12:31:45 PM */
        public static long updatedMS = 1298962905850L;
        /** AD_Table_ID=1030 */
        public static int Table_ID;
        // =1030;

        /** TableName=M_WorkOrderTransaction */
        public static String Table_Name = "M_WorkOrderTransaction";

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
            StringBuilder sb = new StringBuilder("X_M_WorkOrderTransaction[").Append(Get_ID()).Append("]");
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
        /** Set UOM.
        @param C_UOM_ID Unit of Measure */
        public void SetC_UOM_ID(int C_UOM_ID)
        {
            if (C_UOM_ID < 1) throw new ArgumentException("C_UOM_ID is mandatory.");
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
        /** Set Transaction Date.
        @param DateTrx Transaction Date */
        public void SetDateTrx(DateTime? DateTrx)
        {
            Set_ValueNoCheck("DateTrx", (DateTime?)DateTrx);
        }
        /** Get Transaction Date.
        @return Transaction Date */
        public DateTime? GetDateTrx()
        {
            return (DateTime?)Get_Value("DateTrx");
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
        /** Set Create Component Txn Lines.
        @param GenerateLines Generate component lines for Push supply type components  */
        public void SetGenerateLines(String GenerateLines)
        {
            if (GenerateLines != null && GenerateLines.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                GenerateLines = GenerateLines.Substring(0, 1);
            }
            Set_Value("GenerateLines", GenerateLines);
        }
        /** Get Create Component Txn Lines.
        @return Generate component lines for Push supply type components  */
        public String GetGenerateLines()
        {
            return (String)Get_Value("GenerateLines");
        }
        /** Set Generate Resource Usage Lines.
        @param GenerateResourceLines Generate resource usage lines for manually charged resources */
        public void SetGenerateResourceLines(String GenerateResourceLines)
        {
            if (GenerateResourceLines != null && GenerateResourceLines.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                GenerateResourceLines = GenerateResourceLines.Substring(0, 1);
            }
            Set_Value("GenerateResourceLines", GenerateResourceLines);
        }
        /** Get Generate Resource Usage Lines.
        @return Generate resource usage lines for manually charged resources */
        public String GetGenerateResourceLines()
        {
            return (String)Get_Value("GenerateResourceLines");
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
        /** Set Optional.
        @param IsOptionalFrom Indicates if the Operation From in the Work Order Move Transaction is an optional operation */
        public void SetIsOptionalFrom(Boolean IsOptionalFrom)
        {
            Set_Value("IsOptionalFrom", IsOptionalFrom);
        }
        /** Get Optional.
        @return Indicates if the Operation From in the Work Order Move Transaction is an optional operation */
        public Boolean IsOptionalFrom()
        {
            Object oo = Get_Value("IsOptionalFrom");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Optional.
        @param IsOptionalTo Indicates if the Operation To in Work Order Move Transaction is an optional operation */
        public void SetIsOptionalTo(Boolean IsOptionalTo)
        {
            Set_Value("IsOptionalTo", IsOptionalTo);
        }
        /** Get Optional.
        @return Indicates if the Operation To in Work Order Move Transaction is an optional operation */
        public Boolean IsOptionalTo()
        {
            Object oo = Get_Value("IsOptionalTo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** M_Locator_ID AD_Reference_ID=446 */
        public static int M_LOCATOR_ID_AD_Reference_ID = 446;
        /** Set Locator.
        @param M_Locator_ID Warehouse Locator */
        public void SetM_Locator_ID(int M_Locator_ID)
        {
            if (M_Locator_ID <= 0) Set_Value("M_Locator_ID", null);
            else
                Set_Value("M_Locator_ID", M_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetM_Locator_ID()
        {
            Object ii = Get_Value("M_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product.
        @param M_Product_ID Product, Service, Item */
        public void SetM_Product_ID(int M_Product_ID)
        {
            if (M_Product_ID < 1) throw new ArgumentException("M_Product_ID is mandatory.");
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
        /** Set Work Order Transaction.
        @param M_WorkOrderTransaction_ID Work Order Transaction */
        public void SetM_WorkOrderTransaction_ID(int M_WorkOrderTransaction_ID)
        {
            if (M_WorkOrderTransaction_ID < 1) throw new ArgumentException("M_WorkOrderTransaction_ID is mandatory.");
            Set_ValueNoCheck("M_WorkOrderTransaction_ID", M_WorkOrderTransaction_ID);
        }
        /** Get Work Order Transaction.
        @return Work Order Transaction */
        public int GetM_WorkOrderTransaction_ID()
        {
            Object ii = Get_Value("M_WorkOrderTransaction_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order.
        @param M_WorkOrder_ID Work Order */
        public void SetM_WorkOrder_ID(int M_WorkOrder_ID)
        {
            if (M_WorkOrder_ID < 1) throw new ArgumentException("M_WorkOrder_ID is mandatory.");
            Set_ValueNoCheck("M_WorkOrder_ID", M_WorkOrder_ID);
        }
        /** Get Work Order.
        @return Work Order */
        public int GetM_WorkOrder_ID()
        {
            Object ii = Get_Value("M_WorkOrder_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** OperationFrom_ID AD_Reference_ID=451 */
        public static int OPERATIONFROM_ID_AD_Reference_ID = 451;
        /** Set Operation From.
        @param OperationFrom_ID Process the operations in a work order transaction starting at this one. */
        public void SetOperationFrom_ID(int OperationFrom_ID)
        {
            if (OperationFrom_ID <= 0) Set_Value("OperationFrom_ID", null);
            else
                Set_Value("OperationFrom_ID", OperationFrom_ID);
        }
        /** Get Operation From.
        @return Process the operations in a work order transaction starting at this one. */
        public int GetOperationFrom_ID()
        {
            Object ii = Get_Value("OperationFrom_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** OperationTo_ID AD_Reference_ID=451 */
        public static int OPERATIONTO_ID_AD_Reference_ID = 451;
        /** Set Operation To.
        @param OperationTo_ID Process the operations in a work order transaction ending at this one (inclusive). */
        public void SetOperationTo_ID(int OperationTo_ID)
        {
            if (OperationTo_ID <= 0) Set_Value("OperationTo_ID", null);
            else
                Set_Value("OperationTo_ID", OperationTo_ID);
        }
        /** Get Operation To.
        @return Process the operations in a work order transaction ending at this one (inclusive). */
        public int GetOperationTo_ID()
        {
            Object ii = Get_Value("OperationTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** ParentWorkOrderTxn_ID AD_Reference_ID=452 */
        public static int PARENTWORKORDERTXN_ID_AD_Reference_ID = 452;
        /** Set Parent Work Order Transaction.
        @param ParentWorkOrderTxn_ID Work Order Transaction that created this Work Order Transaction */
        public void SetParentWorkOrderTxn_ID(int ParentWorkOrderTxn_ID)
        {
            if (ParentWorkOrderTxn_ID <= 0) Set_ValueNoCheck("ParentWorkOrderTxn_ID", null);
            else
                Set_ValueNoCheck("ParentWorkOrderTxn_ID", ParentWorkOrderTxn_ID);
        }
        /** Get Parent Work Order Transaction.
        @return Work Order Transaction that created this Work Order Transaction */
        public int GetParentWorkOrderTxn_ID()
        {
            Object ii = Get_Value("ParentWorkOrderTxn_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Posted.
        @param Posted Posting status */
        public void SetPosted(Boolean Posted)
        {
            Set_ValueNoCheck("Posted", Posted);
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
        /** Set Processed.
        @param Processed The document has been processed */
        public void SetProcessed(Boolean Processed)
        {
            Set_ValueNoCheck("Processed", Processed);
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
        /** Set Quantity.
        @param QtyEntered The Quantity Entered is based on the selected UoM */
        public void SetQtyEntered(Decimal? QtyEntered)
        {
            if (QtyEntered == null) throw new ArgumentException("QtyEntered is mandatory.");
            Set_Value("QtyEntered", (Decimal?)QtyEntered);
        }
        /** Get Quantity.
        @return The Quantity Entered is based on the selected UoM */
        public Decimal GetQtyEntered()
        {
            Object bd = Get_Value("QtyEntered");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** StepFrom AD_Reference_ID=499 */
        public static int STEPFROM_AD_Reference_ID = 499;
        /** Queue = Q */
        public static String STEPFROM_Queue = "Q";
        /** Run = R */
        public static String STEPFROM_Run = "R";
        /** To Move = T */
        public static String STEPFROM_ToMove = "T";
        /** Scrap = X */
        public static String STEPFROM_Scrap = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsStepFromValid(String test)
        {
            return test == null || test.Equals("Q") || test.Equals("R") || test.Equals("T") || test.Equals("X");
        }
        /** Set Step From.
        @param StepFrom The source intra-operation step from which the work order movement is being made. */
        public void SetStepFrom(String StepFrom)
        {
            if (!IsStepFromValid(StepFrom))
                throw new ArgumentException("StepFrom Invalid value - " + StepFrom + " - Reference_ID=499 - Q - R - T - X");
            if (StepFrom != null && StepFrom.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                StepFrom = StepFrom.Substring(0, 1);
            }
            Set_Value("StepFrom", StepFrom);
        }
        /** Get Step From.
        @return The source intra-operation step from which the work order movement is being made. */
        public String GetStepFrom()
        {
            return (String)Get_Value("StepFrom");
        }

        /** StepTo AD_Reference_ID=499 */
        public static int STEPTO_AD_Reference_ID = 499;
        /** Queue = Q */
        public static String STEPTO_Queue = "Q";
        /** Run = R */
        public static String STEPTO_Run = "R";
        /** To Move = T */
        public static String STEPTO_ToMove = "T";
        /** Scrap = X */
        public static String STEPTO_Scrap = "X";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsStepToValid(String test)
        {
            return test == null || test.Equals("Q") || test.Equals("R") || test.Equals("T") || test.Equals("X");
        }
        /** Set Step To.
        @param StepTo The destination intra-operation step to which the work order movement is being done. */
        public void SetStepTo(String StepTo)
        {
            if (!IsStepToValid(StepTo))
                throw new ArgumentException("StepTo Invalid value - " + StepTo + " - Reference_ID=499 - Q - R - T - X");
            if (StepTo != null && StepTo.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                StepTo = StepTo.Substring(0, 1);
            }
            Set_Value("StepTo", StepTo);
        }
        /** Get Step To.
        @return The destination intra-operation step to which the work order movement is being done. */
        public String GetStepTo()
        {
            return (String)Get_Value("StepTo");
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
        /** Set Complete this Assembly.
        @param WOComplete Indicates that a move transaction should include a completion for the assembly. */
        public void SetWOComplete(Boolean WOComplete)
        {
            Set_Value("WOComplete", WOComplete);
        }
        /** Get Complete this Assembly.
        @return Indicates that a move transaction should include a completion for the assembly. */
        public Boolean IsWOComplete()
        {
            Object oo = Get_Value("WOComplete");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        /** WOTxnSource AD_Reference_ID=453 */
        public static int WOTXNSOURCE_AD_Reference_ID = 453;
        /** Generated = G */
        public static String WOTXNSOURCE_Generated = "G";
        /** Manually Entered = M */
        public static String WOTXNSOURCE_ManuallyEntered = "M";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWOTxnSourceValid(String test)
        {
            return test.Equals("G") || test.Equals("M");
        }
        /** Set Transaction Source.
        @param WOTxnSource Indicates where the work order transaction originated. */
        public void SetWOTxnSource(String WOTxnSource)
        {
            if (WOTxnSource == null) throw new ArgumentException("WOTxnSource is mandatory");
            if (!IsWOTxnSourceValid(WOTxnSource))
                throw new ArgumentException("WOTxnSource Invalid value - " + WOTxnSource + " - Reference_ID=453 - G - M");
            if (WOTxnSource.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                WOTxnSource = WOTxnSource.Substring(0, 1);
            }
            Set_ValueNoCheck("WOTxnSource", WOTxnSource);
        }
        /** Get Transaction Source.
        @return Indicates where the work order transaction originated. */
        public String GetWOTxnSource()
        {
            return (String)Get_Value("WOTxnSource");
        }

        /** WorkOrderTxnType AD_Reference_ID=454 */
        public static int WORKORDERTXNTYPE_AD_Reference_ID = 454;
        /** Assembly Completion to Inventory = AI */
        public static String WORKORDERTXNTYPE_AssemblyCompletionToInventory = "AI";
        /** Assembly Return from Inventory = AR */
        public static String WORKORDERTXNTYPE_AssemblyReturnFromInventory = "AR";
        /** Component Issue to Work Order = CI */
        public static String WORKORDERTXNTYPE_ComponentIssueToWorkOrder = "CI";
        /** Component Return from Work Order = CR */
        public static String WORKORDERTXNTYPE_ComponentReturnFromWorkOrder = "CR";
        /** Resource Usage = RU */
        public static String WORKORDERTXNTYPE_ResourceUsage = "RU";
        /** Work Order Move = WM */
        public static String WORKORDERTXNTYPE_WorkOrderMove = "WM";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsWorkOrderTxnTypeValid(String test)
        {
            return test.Equals("AI") || test.Equals("AR") || test.Equals("CI") || test.Equals("CR") || test.Equals("RU") || test.Equals("WM");
        }
        /** Set Transaction Type.
        @param WorkOrderTxnType Transaction Type */
        public void SetWorkOrderTxnType(String WorkOrderTxnType)
        {
            if (WorkOrderTxnType == null) throw new ArgumentException("WorkOrderTxnType is mandatory");
            if (!IsWorkOrderTxnTypeValid(WorkOrderTxnType))
                throw new ArgumentException("WorkOrderTxnType Invalid value - " + WorkOrderTxnType + " - Reference_ID=454 - AI - AR - CI - CR - RU - WM");
            if (WorkOrderTxnType.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                WorkOrderTxnType = WorkOrderTxnType.Substring(0, 2);
            }
            Set_ValueNoCheck("WorkOrderTxnType", WorkOrderTxnType);
        }
        /** Get Transaction Type.
        @return Transaction Type */
        public String GetWorkOrderTxnType()
        {
            return (String)Get_Value("WorkOrderTxnType");
        }
    }

}
