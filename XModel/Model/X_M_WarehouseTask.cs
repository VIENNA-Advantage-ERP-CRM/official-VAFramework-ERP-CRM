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
    /** Generated Model for VAM_WarehouseTask
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAM_WarehouseTask : PO
    {
        public X_VAM_WarehouseTask(Context ctx, int VAM_WarehouseTask_ID, Trx trxName)
            : base(ctx, VAM_WarehouseTask_ID, trxName)
        {
            /** if (VAM_WarehouseTask_ID == 0)
            {
            SetVAB_DocTypes_ID (0);
            SetVAB_UOM_ID (0);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);	// N
            SetVAM_LocatorTo_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_WarehouseTask_ID (0);
            SetVAM_Warehouse_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetMovementQty (0.0);
            SetQtyDedicated (0.0);	// 0
            SetQtyEntered (0.0);
            SetQtySuggested (0.0);
            SetTargetQty (0.0);
            }
             */
        }
        public X_VAM_WarehouseTask(Ctx ctx, int VAM_WarehouseTask_ID, Trx trxName)
            : base(ctx, VAM_WarehouseTask_ID, trxName)
        {
            /** if (VAM_WarehouseTask_ID == 0)
            {
            SetVAB_DocTypes_ID (0);
            SetVAB_UOM_ID (0);
            SetDocAction (null);	// CO
            SetDocStatus (null);	// DR
            SetDocumentNo (null);
            SetIsApproved (false);	// N
            SetVAM_LocatorTo_ID (0);
            SetVAM_Locator_ID (0);
            SetVAM_Product_ID (0);
            SetVAM_WarehouseTask_ID (0);
            SetVAM_Warehouse_ID (0);
            SetMovementDate (DateTime.Now);	// @#Date@
            SetMovementQty (0.0);
            SetQtyDedicated (0.0);	// 0
            SetQtyEntered (0.0);
            SetQtySuggested (0.0);
            SetTargetQty (0.0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_WarehouseTask(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_WarehouseTask(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_VAM_WarehouseTask(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_VAM_WarehouseTask()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27581088222092L;
        /** Last Updated Timestamp 3/1/2011 12:31:45 PM */
        public static long updatedMS = 1298962905303L;
        /** VAF_TableView_ID=1018 */
        public static int Table_ID;
        // =1018;

        /** TableName=VAM_WarehouseTask */
        public static String Table_Name = "VAM_WarehouseTask";

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
            StringBuilder sb = new StringBuilder("X_VAM_WarehouseTask[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }

        /** VAF_UserContact_ID VAF_Control_Ref_ID=286 */
        public static int VAF_USERCONTACT_ID_VAF_Control_Ref_ID = 286;
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

        /** VAB_DocTypes_ID VAF_Control_Ref_ID=170 */
        public static int VAB_DocTypes_ID_VAF_Control_Ref_ID = 170;
        /** Set Document Type.
        @param VAB_DocTypes_ID Document type or rules */
        public void SetVAB_DocTypes_ID(int VAB_DocTypes_ID)
        {
            if (VAB_DocTypes_ID < 0) throw new ArgumentException("VAB_DocTypes_ID is mandatory.");
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
        /** Set Order Line.
        @param VAB_OrderLine_ID Order Line */
        public void SetVAB_OrderLine_ID(int VAB_OrderLine_ID)
        {
            if (VAB_OrderLine_ID <= 0) Set_Value("VAB_OrderLine_ID", null);
            else
                Set_Value("VAB_OrderLine_ID", VAB_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetVAB_OrderLine_ID()
        {
            Object ii = Get_Value("VAB_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set UOM.
        @param VAB_UOM_ID Unit of Measure */
        public void SetVAB_UOM_ID(int VAB_UOM_ID)
        {
            if (VAB_UOM_ID < 1) throw new ArgumentException("VAB_UOM_ID is mandatory.");
            Set_Value("VAB_UOM_ID", VAB_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetVAB_UOM_ID()
        {
            Object ii = Get_Value("VAB_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Wave Line.
        @param C_WaveLine_ID Selected order lines for which there is sufficient onhand quantity in the warehouse */
        public void SetC_WaveLine_ID(int C_WaveLine_ID)
        {
            if (C_WaveLine_ID <= 0) Set_Value("C_WaveLine_ID", null);
            else
                Set_Value("C_WaveLine_ID", C_WaveLine_ID);
        }
        /** Get Wave Line.
        @return Selected order lines for which there is sufficient onhand quantity in the warehouse */
        public int GetC_WaveLine_ID()
        {
            Object ii = Get_Value("C_WaveLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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

        /** DocAction VAF_Control_Ref_ID=135 */
        public static int DOCACTION_VAF_Control_Ref_ID = 135;
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

        /** DocStatus VAF_Control_Ref_ID=131 */
        public static int DOCSTATUS_VAF_Control_Ref_ID = 131;
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
        /** Set Actual Attribute Set Instance.
        @param M_ActualASI_ID Product Attribute Set Instance actually used for the warehouse task */
        public void SetM_ActualASI_ID(int M_ActualASI_ID)
        {
            if (M_ActualASI_ID <= 0) Set_Value("M_ActualASI_ID", null);
            else
                Set_Value("M_ActualASI_ID", M_ActualASI_ID);
        }
        /** Get Actual Attribute Set Instance.
        @return Product Attribute Set Instance actually used for the warehouse task */
        public int GetM_ActualASI_ID()
        {
            Object ii = Get_Value("M_ActualASI_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_ActualLocatorTo_ID VAF_Control_Ref_ID=446 */
        public static int M_ACTUALLOCATORTO_ID_VAF_Control_Ref_ID = 446;
        /** Set Actual Destination Locator.
        @param M_ActualLocatorTo_ID Actual locator where the stock was moved to */
        public void SetM_ActualLocatorTo_ID(int M_ActualLocatorTo_ID)
        {
            if (M_ActualLocatorTo_ID <= 0) Set_Value("M_ActualLocatorTo_ID", null);
            else
                Set_Value("M_ActualLocatorTo_ID", M_ActualLocatorTo_ID);
        }
        /** Get Actual Destination Locator.
        @return Actual locator where the stock was moved to */
        public int GetM_ActualLocatorTo_ID()
        {
            Object ii = Get_Value("M_ActualLocatorTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** M_ActualLocator_ID VAF_Control_Ref_ID=446 */
        public static int M_ACTUALLOCATOR_ID_VAF_Control_Ref_ID = 446;
        /** Set Actual Source Locator.
        @param M_ActualLocator_ID Actual locator from where the stock was moved */
        public void SetM_ActualLocator_ID(int M_ActualLocator_ID)
        {
            if (M_ActualLocator_ID <= 0) Set_Value("M_ActualLocator_ID", null);
            else
                Set_Value("M_ActualLocator_ID", M_ActualLocator_ID);
        }
        /** Get Actual Source Locator.
        @return Actual locator from where the stock was moved */
        public int GetM_ActualLocator_ID()
        {
            Object ii = Get_Value("M_ActualLocator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Set Instance.
        @param VAM_PFeature_SetInstance_ID Product Attribute Set Instance */
        public void SetVAM_PFeature_SetInstance_ID(int VAM_PFeature_SetInstance_ID)
        {
            if (VAM_PFeature_SetInstance_ID <= 0) Set_Value("VAM_PFeature_SetInstance_ID", null);
            else
                Set_Value("VAM_PFeature_SetInstance_ID", VAM_PFeature_SetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetVAM_PFeature_SetInstance_ID()
        {
            Object ii = Get_Value("VAM_PFeature_SetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param VAM_Inv_InOutLine_ID Line on Shipment or Receipt document */
        public void SetVAM_Inv_InOutLine_ID(int VAM_Inv_InOutLine_ID)
        {
            if (VAM_Inv_InOutLine_ID <= 0) Set_ValueNoCheck("VAM_Inv_InOutLine_ID", null);
            else
                Set_ValueNoCheck("VAM_Inv_InOutLine_ID", VAM_Inv_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetVAM_Inv_InOutLine_ID()
        {
            Object ii = Get_Value("VAM_Inv_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAM_LocatorTo_ID VAF_Control_Ref_ID=446 */
        public static int VAM_LocatorTO_ID_VAF_Control_Ref_ID = 446;
        /** Set Locator To.
        @param VAM_LocatorTo_ID Location inventory is moved to */
        public void SetVAM_LocatorTo_ID(int VAM_LocatorTo_ID)
        {
            if (VAM_LocatorTo_ID < 1) throw new ArgumentException("VAM_LocatorTo_ID is mandatory.");
            Set_Value("VAM_LocatorTo_ID", VAM_LocatorTo_ID);
        }
        /** Get Locator To.
        @return Location inventory is moved to */
        public int GetVAM_LocatorTo_ID()
        {
            Object ii = Get_Value("VAM_LocatorTo_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAM_Locator_ID VAF_Control_Ref_ID=446 */
        public static int VAM_Locator_ID_VAF_Control_Ref_ID = 446;
        /** Set Locator.
        @param VAM_Locator_ID Warehouse Locator */
        public void SetVAM_Locator_ID(int VAM_Locator_ID)
        {
            if (VAM_Locator_ID < 1) throw new ArgumentException("VAM_Locator_ID is mandatory.");
            Set_Value("VAM_Locator_ID", VAM_Locator_ID);
        }
        /** Get Locator.
        @return Warehouse Locator */
        public int GetVAM_Locator_ID()
        {
            Object ii = Get_Value("VAM_Locator_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** VAM_Product_ID VAF_Control_Ref_ID=162 */
        public static int VAM_Product_ID_VAF_Control_Ref_ID = 162;
        /** Set Product.
        @param VAM_Product_ID Product, Service, Item */
        public void SetVAM_Product_ID(int VAM_Product_ID)
        {
            if (VAM_Product_ID < 1) throw new ArgumentException("VAM_Product_ID is mandatory.");
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

        /** M_SplitWarehouseTask_ID VAF_Control_Ref_ID=477 */
        public static int M_SPLITWAREHOUSETASK_ID_VAF_Control_Ref_ID = 477;
        /** Set Split Warehouse Task.
        @param M_SplitWarehouseTask_ID Warehouse Task that this task was split from */
        public void SetM_SplitWarehouseTask_ID(int M_SplitWarehouseTask_ID)
        {
            if (M_SplitWarehouseTask_ID <= 0) Set_ValueNoCheck("M_SplitWarehouseTask_ID", null);
            else
                Set_ValueNoCheck("M_SplitWarehouseTask_ID", M_SplitWarehouseTask_ID);
        }
        /** Get Split Warehouse Task.
        @return Warehouse Task that this task was split from */
        public int GetM_SplitWarehouseTask_ID()
        {
            Object ii = Get_Value("M_SplitWarehouseTask_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse Task.
        @param VAM_WarehouseTask_ID A Warehouse Task represents a basic warehouse operation such as putaway, picking or replenishment. */
        public void SetVAM_WarehouseTask_ID(int VAM_WarehouseTask_ID)
        {
            if (VAM_WarehouseTask_ID < 1) throw new ArgumentException("VAM_WarehouseTask_ID is mandatory.");
            Set_ValueNoCheck("VAM_WarehouseTask_ID", VAM_WarehouseTask_ID);
        }
        /** Get Warehouse Task.
        @return A Warehouse Task represents a basic warehouse operation such as putaway, picking or replenishment. */
        public int GetVAM_WarehouseTask_ID()
        {
            Object ii = Get_Value("VAM_WarehouseTask_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse.
        @param VAM_Warehouse_ID Storage Warehouse and Service Point */
        public void SetVAM_Warehouse_ID(int VAM_Warehouse_ID)
        {
            if (VAM_Warehouse_ID < 1) throw new ArgumentException("VAM_Warehouse_ID is mandatory.");
            Set_Value("VAM_Warehouse_ID", VAM_Warehouse_ID);
        }
        /** Get Warehouse.
        @return Storage Warehouse and Service Point */
        public int GetVAM_Warehouse_ID()
        {
            Object ii = Get_Value("VAM_Warehouse_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Component.
        @param VAM_WorkOrderComponent_ID Work Order Component */
        public void SetVAM_WorkOrderComponent_ID(int VAM_WorkOrderComponent_ID)
        {
            if (VAM_WorkOrderComponent_ID <= 0) Set_Value("VAM_WorkOrderComponent_ID", null);
            else
                Set_Value("VAM_WorkOrderComponent_ID", VAM_WorkOrderComponent_ID);
        }
        /** Get Work Order Component.
        @return Work Order Component */
        public int GetVAM_WorkOrderComponent_ID()
        {
            Object ii = Get_Value("VAM_WorkOrderComponent_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Transaction Line.
        @param VAM_WorkOrderTransactionLine_ID Work Order Transaction Line */
        public void SetVAM_WorkOrderTransactionLine_ID(int VAM_WorkOrderTransactionLine_ID)
        {
            if (VAM_WorkOrderTransactionLine_ID <= 0) Set_Value("VAM_WorkOrderTransactionLine_ID", null);
            else
                Set_Value("VAM_WorkOrderTransactionLine_ID", VAM_WorkOrderTransactionLine_ID);
        }
        /** Get Work Order Transaction Line.
        @return Work Order Transaction Line */
        public int GetVAM_WorkOrderTransactionLine_ID()
        {
            Object ii = Get_Value("VAM_WorkOrderTransactionLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order.
        @param VAM_WorkOrder_ID Work Order */
        public void SetVAM_WorkOrder_ID(int VAM_WorkOrder_ID)
        {
            throw new ArgumentException("VAM_WorkOrder_ID Is virtual column");
        }
        /** Get Work Order.
        @return Work Order */
        public int GetVAM_WorkOrder_ID()
        {
            Object ii = Get_Value("VAM_WorkOrder_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Movement Date.
        @param MovementDate Date a product was moved in or out of inventory */
        public void SetMovementDate(DateTime? MovementDate)
        {
            if (MovementDate == null) throw new ArgumentException("MovementDate is mandatory.");
            Set_ValueNoCheck("MovementDate", (DateTime?)MovementDate);
        }
        /** Get Movement Date.
        @return Date a product was moved in or out of inventory */
        public DateTime? GetMovementDate()
        {
            return (DateTime?)Get_Value("MovementDate");
        }
        /** Set Movement Quantity.
        @param MovementQty Quantity of a product moved. */
        public void SetMovementQty(Decimal? MovementQty)
        {
            if (MovementQty == null) throw new ArgumentException("MovementQty is mandatory.");
            Set_Value("MovementQty", (Decimal?)MovementQty);
        }
        /** Get Movement Quantity.
        @return Quantity of a product moved. */
        public Decimal GetMovementQty()
        {
            Object bd = Get_Value("MovementQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Quantity Dedicated.
        @param QtyDedicated Quantity for which there is a pending Warehouse Task */
        public void SetQtyDedicated(Decimal? QtyDedicated)
        {
            if (QtyDedicated == null) throw new ArgumentException("QtyDedicated is mandatory.");
            Set_Value("QtyDedicated", (Decimal?)QtyDedicated);
        }
        /** Get Quantity Dedicated.
        @return Quantity for which there is a pending Warehouse Task */
        public Decimal GetQtyDedicated()
        {
            Object bd = Get_Value("QtyDedicated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Suggested Quantity.
        @param QtySuggested Quantity suggested for Pick or Putaway by the Putaway or Pick process */
        public void SetQtySuggested(Decimal? QtySuggested)
        {
            if (QtySuggested == null) throw new ArgumentException("QtySuggested is mandatory.");
            Set_Value("QtySuggested", (Decimal?)QtySuggested);
        }
        /** Get Suggested Quantity.
        @return Quantity suggested for Pick or Putaway by the Putaway or Pick process */
        public Decimal GetQtySuggested()
        {
            Object bd = Get_Value("QtySuggested");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Split Task.
        @param SplitTask Split Warehouse Task into two tasks */
        public void SetSplitTask(String SplitTask)
        {
            if (SplitTask != null && SplitTask.Length > 1)
            {
                log.Warning("Length > 1 - truncated");
                SplitTask = SplitTask.Substring(0, 1);
            }
            Set_Value("SplitTask", SplitTask);
        }
        /** Get Split Task.
        @return Split Warehouse Task into two tasks */
        public String GetSplitTask()
        {
            return (String)Get_Value("SplitTask");
        }
        /** Set Target Quantity.
        @param TargetQty Target Movement Quantity */
        public void SetTargetQty(Decimal? TargetQty)
        {
            if (TargetQty == null) throw new ArgumentException("TargetQty is mandatory.");
            Set_Value("TargetQty", (Decimal?)TargetQty);
        }
        /** Get Target Quantity.
        @return Target Movement Quantity */
        public Decimal GetTargetQty()
        {
            Object bd = Get_Value("TargetQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
