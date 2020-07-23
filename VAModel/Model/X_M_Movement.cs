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
    /** Generated Model for M_Movement
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_Movement : PO
    {
        public X_M_Movement(Context ctx, int M_Movement_ID, Trx trxName)
            : base(ctx, M_Movement_ID, trxName)
        {
            /** if (M_Movement_ID == 0)
            {
            SetC_DocType_ID (0);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);
            SetIsInTransit (false);
            SetM_Movement_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetPosted (false);
            SetProcessed (false);	// N
            }
             */
        }
        public X_M_Movement(Ctx ctx, int M_Movement_ID, Trx trxName)
            : base(ctx, M_Movement_ID, trxName)
        {
            /** if (M_Movement_ID == 0)
            {
            SetC_DocType_ID (0);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);
            SetIsInTransit (false);
            SetM_Movement_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetPosted (false);
            SetProcessed (false);	// N
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Movement(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Movement(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_Movement(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_Movement()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514380024L;
        /** Last Updated Timestamp 7/29/2010 1:07:43 PM */
        public static long updatedMS = 1280389063235L;
        /** AD_Table_ID=323 */
        public static int Table_ID;
        // =323;

        /** TableName=M_Movement */
        public static String Table_Name = "M_Movement";

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
            StringBuilder sb = new StringBuilder("X_M_Movement[").Append(Get_ID()).Append("]");
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
        /** Set Approval Amount.
        @param ApprovalAmt Document Approval Amount */
        public void SetApprovalAmt(Decimal? ApprovalAmt)
        {
            Set_Value("ApprovalAmt", (Decimal?)ApprovalAmt);
        }
        /** Get Approval Amount.
        @return Document Approval Amount */
        public Decimal GetApprovalAmt()
        {
            Object bd = Get_Value("ApprovalAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Document Type.
        @param C_DocType_ID Document type or rules */
        public void SetC_DocType_ID(int C_DocType_ID)
        {
            if (C_DocType_ID < 0) throw new ArgumentException("C_DocType_ID is mandatory.");
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
            Set_Value("DocumentNo", DocumentNo);
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
        /** Set Inventory Move.
        @param M_Movement_ID Movement of Inventory */
        public void SetM_Movement_ID(int M_Movement_ID)
        {
            if (M_Movement_ID < 1) throw new ArgumentException("M_Movement_ID is mandatory.");
            Set_ValueNoCheck("M_Movement_ID", M_Movement_ID);
        }
        /** Get Inventory Move.
        @return Movement of Inventory */
        public int GetM_Movement_ID()
        {
            Object ii = Get_Value("M_Movement_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** DTD001_MWarehouseSource_ID AD_Reference_ID=1000313 */
        public static int DTD001_MWAREHOUSESOURCE_ID_AD_Reference_ID = 1000313;
        /** Set Source Warehouse.
        @param DTD001_MWarehouseSource_ID Source Warehouse */
        public void SetDTD001_MWarehouseSource_ID(int DTD001_MWarehouseSource_ID)
        {
            if (DTD001_MWarehouseSource_ID <= 0) Set_Value("DTD001_MWarehouseSource_ID", null);
            else
                Set_Value("DTD001_MWarehouseSource_ID", DTD001_MWarehouseSource_ID);
        }
        /** Get Source Warehouse.
        @return Source Warehouse */
        public int GetDTD001_MWarehouseSource_ID()
        {
            Object ii = Get_Value("DTD001_MWarehouseSource_ID");
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
        /** Set Packing.
        @param DTD001_Packing_ID Packing */
        public void SetDTD001_Packing_ID(int DTD001_Packing_ID)
        {
            if (DTD001_Packing_ID <= 0) Set_Value("DTD001_Packing_ID", null);
            else
                Set_Value("DTD001_Packing_ID", DTD001_Packing_ID);
        }
        /** Get Packing.
        @return Packing */
        public int GetDTD001_Packing_ID()
        {
            Object ii = Get_Value("DTD001_Packing_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Cost Calculated.@param IsCostCalculated Cost Calculated */
        public void SetIsCostCalculated(Boolean IsCostCalculated) { Set_Value("IsCostCalculated", IsCostCalculated); }
        /** Get Cost Calculated. @return Cost Calculated */
        public Boolean IsCostCalculated() { Object oo = Get_Value("IsCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Reversed Cost Calculated.@param IsReversedCostCalculated Reversed Cost Calculated */
        public void SetIsReversedCostCalculated(Boolean IsReversedCostCalculated) { Set_Value("IsReversedCostCalculated", IsReversedCostCalculated); }
        /** Get Reversed Cost Calculated. @return Reversed Cost Calculated */
        public Boolean IsReversedCostCalculated() { Object oo = Get_Value("IsReversedCostCalculated"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** ReversalDoc_ID AD_Reference_ID=1000206 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 1000206;
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

        /** Set From Path.@param FromPath This field is useful while Container Movement in Order to keep the references of container linkage in From Locator . At the Reverse of inventory move, it helps to find the path of container as was on completion of move. */
        public void SetFromPath(String FromPath) { if (FromPath != null && FromPath.Length > 500) { log.Warning("Length > 500 - truncated"); FromPath = FromPath.Substring(0, 500); } Set_Value("FromPath", FromPath); }
        /** Get From Path.@return This field is useful while Container Movement in Order to keep the references of container linkage in From Locator . At the Reverse of inventory move, it helps to find the path of container as was on completion of move. */
        public String GetFromPath() { return (String)Get_Value("FromPath"); }

        /** Set To Path.@param ToPath This field is useful while Container Movement in Order to keep the references of container linkage in To Locator . At the Reverse of inventory move, it helps to find the path of container as was on completion of move. */
        public void SetToPath(String ToPath) { if (ToPath != null && ToPath.Length > 500) { log.Warning("Length > 500 - truncated"); ToPath = ToPath.Substring(0, 500); } Set_Value("ToPath", ToPath); }
        /** Get To Path.@return This field is useful while Container Movement in Order to keep the references of container linkage in To Locator . At the Reverse of inventory move, it helps to find the path of container as was on completion of move. */
        public String GetToPath() { return (String)Get_Value("ToPath"); }

        /** Set Drop All Lines.@param DropAllLines Is Used to drop all movement lines. */
        public void SetDropAllLines(String DropAllLines) { if (DropAllLines != null && DropAllLines.Length > 1) { log.Warning("Length > 1 - truncated"); DropAllLines = DropAllLines.Substring(0, 1); } Set_Value("DropAllLines", DropAllLines); }
        /** Get Drop All Lines.@return Is Used to drop all movement lines. */
        public String GetDropAllLines() { return (String)Get_Value("DropAllLines"); }

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
