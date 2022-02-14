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
    /** Generated Model for M_InOutLine
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_M_InOutLine : PO
    {
        public X_M_InOutLine(Context ctx, int M_InOutLine_ID, Trx trxName)
            : base(ctx, M_InOutLine_ID, trxName)
        {
            /** if (M_InOutLine_ID == 0)
            {
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetIsDescription (false);	// N
            SetIsInvoiced (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_InOutLine WHERE M_InOut_ID=@M_InOut_ID@
            SetM_InOutLine_ID (0);
            SetM_InOut_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            }
             */
        }
        public X_M_InOutLine(Ctx ctx, int M_InOutLine_ID, Trx trxName)
            : base(ctx, M_InOutLine_ID, trxName)
        {
            /** if (M_InOutLine_ID == 0)
            {
            SetC_UOM_ID (0);	// @#C_UOM_ID@
            SetIsDescription (false);	// N
            SetIsInvoiced (false);
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM M_InOutLine WHERE M_InOut_ID=@M_InOut_ID@
            SetM_InOutLine_ID (0);
            SetM_InOut_ID (0);
            SetMovementQty (0.0);	// 1
            SetProcessed (false);	// N
            SetQtyEntered (0.0);	// 1
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_M_InOutLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_M_InOutLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27562514379632L;
        /** Last Updated Timestamp 7/29/2010 1:07:42 PM */
        public static long updatedMS = 1280389062843L;
        /** AD_Table_ID=320 */
        public static int Table_ID;
        // =320;

        /** TableName=M_InOutLine */
        public static String Table_Name = "M_InOutLine";

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
            StringBuilder sb = new StringBuilder("X_M_InOutLine[").Append(Get_ID()).Append("]");
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
        /** Set Order Line.
        @param C_OrderLine_ID Order Line */
        public void SetC_OrderLine_ID(int C_OrderLine_ID)
        {
            if (C_OrderLine_ID <= 0) Set_ValueNoCheck("C_OrderLine_ID", null);
            else
                Set_ValueNoCheck("C_OrderLine_ID", C_OrderLine_ID);
        }
        /** Get Order Line.
        @return Order Line */
        public int GetC_OrderLine_ID()
        {
            Object ii = Get_Value("C_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Phase.
        @param C_ProjectPhase_ID Phase of a Project */
        public void SetC_ProjectPhase_ID(int C_ProjectPhase_ID)
        {
            if (C_ProjectPhase_ID <= 0) Set_Value("C_ProjectPhase_ID", null);
            else
                Set_Value("C_ProjectPhase_ID", C_ProjectPhase_ID);
        }
        /** Get Project Phase.
        @return Phase of a Project */
        public int GetC_ProjectPhase_ID()
        {
            Object ii = Get_Value("C_ProjectPhase_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Task.
        @param C_ProjectTask_ID Actual Project Task in a Phase */
        public void SetC_ProjectTask_ID(int C_ProjectTask_ID)
        {
            if (C_ProjectTask_ID <= 0) Set_Value("C_ProjectTask_ID", null);
            else
                Set_Value("C_ProjectTask_ID", C_ProjectTask_ID);
        }
        /** Get Project Task.
        @return Actual Project Task in a Phase */
        public int GetC_ProjectTask_ID()
        {
            Object ii = Get_Value("C_ProjectTask_ID");
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
            Set_ValueNoCheck("C_UOM_ID", C_UOM_ID);
        }
        /** Get UOM.
        @return Unit of Measure */
        public int GetC_UOM_ID()
        {
            Object ii = Get_Value("C_UOM_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Confirmed Quantity.
        @param ConfirmedQty Confirmation of a received quantity */
        public void SetConfirmedQty(Decimal? ConfirmedQty)
        {
            Set_Value("ConfirmedQty", (Decimal?)ConfirmedQty);
        }
        /** Get Confirmed Quantity.
        @return Confirmation of a received quantity */
        public Decimal GetConfirmedQty()
        {
            Object bd = Get_Value("ConfirmedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Attribute Numbers.
        @param DTD001_Attribute Attribute Numbers */
        public void SetDTD001_Attribute(String DTD001_Attribute)
        {
            Set_Value("DTD001_Attribute", DTD001_Attribute);
        }
        /** Get Attribute Numbers.
        @return Attribute Numbers */
        public String GetDTD001_Attribute()
        {
            return (String)Get_Value("DTD001_Attribute");
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
        /** Set Description Only.
        @param IsDescription if true, the line is just description and no transaction */
        public void SetIsDescription(Boolean IsDescription)
        {
            Set_Value("IsDescription", IsDescription);
        }
        /** Get Description Only.
        @return if true, the line is just description and no transaction */
        public Boolean IsDescription()
        {
            Object oo = Get_Value("IsDescription");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Invoiced.
        @param IsInvoiced Is this invoiced? */
        public void SetIsInvoiced(Boolean IsInvoiced)
        {
            Set_Value("IsInvoiced", IsInvoiced);
        }
        /** Get Invoiced.
        @return Is this invoiced? */
        public Boolean IsInvoiced()
        {
            Object oo = Get_Value("IsInvoiced");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Line No.
        @param Line Unique line for this document */
        public void SetLine(int Line)
        {
            Set_Value("Line", Line);
        }
        /** Get Line No.
        @return Unique line for this document */
        public int GetLine()
        {
            Object ii = Get_Value("Line");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetLine().ToString());
        }

        /** LineDocStatus AD_Reference_ID=131 */
        public static int LINEDOCSTATUS_AD_Reference_ID = 131;
        /** Unknown = ?? */
        public static String LINEDOCSTATUS_Unknown = "??";
        /** Approved = AP */
        public static String LINEDOCSTATUS_Approved = "AP";
        /** Closed = CL */
        public static String LINEDOCSTATUS_Closed = "CL";
        /** Completed = CO */
        public static String LINEDOCSTATUS_Completed = "CO";
        /** Drafted = DR */
        public static String LINEDOCSTATUS_Drafted = "DR";
        /** Invalid = IN */
        public static String LINEDOCSTATUS_Invalid = "IN";
        /** In Progress = IP */
        public static String LINEDOCSTATUS_InProgress = "IP";
        /** Not Approved = NA */
        public static String LINEDOCSTATUS_NotApproved = "NA";
        /** Reversed = RE */
        public static String LINEDOCSTATUS_Reversed = "RE";
        /** Voided = VO */
        public static String LINEDOCSTATUS_Voided = "VO";
        /** Waiting Confirmation = WC */
        public static String LINEDOCSTATUS_WaitingConfirmation = "WC";
        /** Waiting Payment = WP */
        public static String LINEDOCSTATUS_WaitingPayment = "WP";
        /** Is test a valid value.
        @param test testvalue
        @returns true if valid **/
        public bool IsLineDocStatusValid(String test)
        {
            return test == null || test.Equals("??") || test.Equals("AP") || test.Equals("CL") || test.Equals("CO") || test.Equals("DR") || test.Equals("IN") || test.Equals("IP") || test.Equals("NA") || test.Equals("RE") || test.Equals("VO") || test.Equals("WC") || test.Equals("WP");
        }
        /** Set Line Document Status.
        @param LineDocStatus The current status of the document line */
        public void SetLineDocStatus(String LineDocStatus)
        {
            if (!IsLineDocStatusValid(LineDocStatus))
                throw new ArgumentException("LineDocStatus Invalid value - " + LineDocStatus + " - Reference_ID=131 - ?? - AP - CL - CO - DR - IN - IP - NA - RE - VO - WC - WP");
            if (LineDocStatus != null && LineDocStatus.Length > 2)
            {
                log.Warning("Length > 2 - truncated");
                LineDocStatus = LineDocStatus.Substring(0, 2);
            }
            Set_Value("LineDocStatus", LineDocStatus);
        }
        /** Get Line Document Status.
        @return The current status of the document line */
        public String GetLineDocStatus()
        {
            return (String)Get_Value("LineDocStatus");
        }
        /** Set Attribute Set Instance.
        @param M_AttributeSetInstance_ID Product Attribute Set Instance */
        public void SetM_AttributeSetInstance_ID(int M_AttributeSetInstance_ID)
        {
            if (M_AttributeSetInstance_ID <= 0) Set_Value("M_AttributeSetInstance_ID", null);
            else
                Set_Value("M_AttributeSetInstance_ID", M_AttributeSetInstance_ID);
        }
        /** Get Attribute Set Instance.
        @return Product Attribute Set Instance */
        public int GetM_AttributeSetInstance_ID()
        {
            Object ii = Get_Value("M_AttributeSetInstance_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Shipment/Receipt Line.
        @param M_InOutLine_ID Line on Shipment or Receipt document */
        public void SetM_InOutLine_ID(int M_InOutLine_ID)
        {
            if (M_InOutLine_ID < 1) throw new ArgumentException("M_InOutLine_ID is mandatory.");
            Set_ValueNoCheck("M_InOutLine_ID", M_InOutLine_ID);
        }
        /** Get Shipment/Receipt Line.
        @return Line on Shipment or Receipt document */
        public int GetM_InOutLine_ID()
        {
            Object ii = Get_Value("M_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Orig_OrderLine_ID AD_Reference_ID=271 */
        public static int ORIG_ORDERLINE_ID_AD_Reference_ID = 271;
        /** Set Orig Sales Order Line.
        @param Orig_OrderLine_ID Original Sales Order Line for Return Material Authorization */
        public void SetOrig_OrderLine_ID(int Orig_OrderLine_ID)
        {
            throw new ArgumentException("Orig_OrderLine_ID Is virtual column");
        }
        /** Get Orig Sales Order Line.
        @return Original Sales Order Line for Return Material Authorization */
        public int GetOrig_OrderLine_ID()
        {
            Object ii = Get_Value("Orig_OrderLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Picked Quantity.
        @param PickedQty Picked Quantity */
        public void SetPickedQty(Decimal? PickedQty)
        {
            Set_Value("PickedQty", (Decimal?)PickedQty);
        }
        /** Get Picked Quantity.
        @return Picked Quantity */
        public Decimal GetPickedQty()
        {
            Object bd = Get_Value("PickedQty");
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
        /** Set Referenced Shipment Line.
        @param Ref_InOutLine_ID Referenced Shipment Line */
        public void SetRef_InOutLine_ID(int Ref_InOutLine_ID)
        {
            if (Ref_InOutLine_ID <= 0) Set_Value("Ref_InOutLine_ID", null);
            else
                Set_Value("Ref_InOutLine_ID", Ref_InOutLine_ID);
        }
        /** Get Referenced Shipment Line.
        @return Referenced Shipment Line */
        public int GetRef_InOutLine_ID()
        {
            Object ii = Get_Value("Ref_InOutLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Scrapped Quantity.
        @param ScrappedQty The Quantity scrapped due to QA issues */
        public void SetScrappedQty(Decimal? ScrappedQty)
        {
            Set_Value("ScrappedQty", (Decimal?)ScrappedQty);
        }
        /** Get Scrapped Quantity.
        @return The Quantity scrapped due to QA issues */
        public Decimal GetScrappedQty()
        {
            Object bd = Get_Value("ScrappedQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Target Quantity.
        @param TargetQty Target Movement Quantity */
        public void SetTargetQty(Decimal? TargetQty)
        {
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
        /** Set Product.
        @param A_Asset_ID Product, Service, Item */
        public void SetA_Asset_ID(int A_Asset_ID)
        {
            if (A_Asset_ID <= 0) Set_Value("A_Asset_ID", null);
            else
                Set_Value("A_Asset_ID", A_Asset_ID);
        }
        /** Get A_Asset_ID.
        @return A_Asset_ID */
        public int GetA_Asset_ID()
        {
            Object ii = Get_Value("A_Asset_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Attribute Number Mandatory.
        @param DTD001_IsAttributeNo Attribute Number Mandatory */
        public void SetDTD001_IsAttributeNo(Boolean DTD001_IsAttributeNo)
        {
            Set_Value("DTD001_IsAttributeNo", DTD001_IsAttributeNo);
        }
        /** Get Attribute Number Mandatory.
        @return Attribute Number Mandatory */
        public Boolean IsDTD001_IsAttributeNo()
        {
            Object oo = Get_Value("DTD001_IsAttributeNo");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }

        // Added By Mohit VAWMS 20-8-2015

        /** Set Quantity Allocated.
        @param QtyAllocated Quantity that has been picked and is awaiting shipment */
        public void SetQtyAllocated(Decimal? QtyAllocated)
        {
            if (QtyAllocated == null) throw new ArgumentException("QtyAllocated is mandatory.");
            Set_Value("QtyAllocated", (Decimal?)QtyAllocated);
        }
        /** Get Quantity Allocated.
        @return Quantity that has been picked and is awaiting shipment */
        public Decimal GetQtyAllocated()
        {
            Object bd = Get_Value("QtyAllocated");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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

        /** Set Cost Immediately.@param IsCostImmediate Update Costs immediately for testing */
        public void SetIsCostImmediate(Boolean IsCostImmediate) { Set_Value("IsCostImmediate", IsCostImmediate); }
        /** Get Cost Immediately.@return Update Costs immediately for testing */
        public Boolean IsCostImmediate() { Object oo = Get_Value("IsCostImmediate"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }


        /** Set Cost Price. @param VA024_CostPrice Cost Price */
        public void SetVA024_CostPrice(Decimal? VA024_CostPrice) { Set_ValueNoCheck("VA024_CostPrice", (Decimal?)VA024_CostPrice); }
        /** Get Cost Price.@return Cost Price */
        public Decimal GetVA024_CostPrice() { Object bd = Get_Value("VA024_CostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Unit Price.@param VA024_UnitPrice Unit Price */
        public void SetVA024_UnitPrice(Decimal? VA024_UnitPrice) { Set_ValueNoCheck("VA024_UnitPrice", (Decimal?)VA024_UnitPrice); }
        /** Get Unit Price.@return Unit Price */
        public Decimal GetVA024_UnitPrice() { Object bd = Get_Value("VA024_UnitPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Current Cost.@param CurrentCostPrice The currently used cost price */
        public void SetCurrentCostPrice(Decimal? CurrentCostPrice) { Set_Value("CurrentCostPrice", (Decimal?)CurrentCostPrice); }
        /** Get Current Cost.@return The currently used cost price */
        public Decimal GetCurrentCostPrice() { Object bd = Get_Value("CurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Post Current Cost Price.@param PostCurrentCostPrice It indicate the cost after cost calculation of current record */
        public void SetPostCurrentCostPrice(Decimal? PostCurrentCostPrice) { Set_Value("PostCurrentCostPrice", (Decimal?)PostCurrentCostPrice); }
        /** Get Post Current Cost Price. @return It indicate the cost after cost calculation of current record */
        public Decimal GetPostCurrentCostPrice() { Object bd = Get_Value("PostCurrentCostPrice"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }

        /** Set Drop Shipment.@param IsDropShip Drop Shipments are sent from the Vendor directly to the Customer */
        public void SetIsDropShip(Boolean IsDropShip) { Set_Value("IsDropShip", IsDropShip); }
        /** Get Drop Shipment.@return Drop Shipments are sent from the Vendor directly to the Customer */
        public Boolean IsDropShip() { Object oo = Get_Value("IsDropShip"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }

        /** Set Product Container.@param M_ProductContainer_ID Product Container */
        public void SetM_ProductContainer_ID(int M_ProductContainer_ID)
        {
            if (M_ProductContainer_ID <= 0) Set_Value("M_ProductContainer_ID", null);
            else
                Set_Value("M_ProductContainer_ID", M_ProductContainer_ID);
        }
        /** Get Product Container.@return Product Container */
        public int GetM_ProductContainer_ID() { Object ii = Get_Value("M_ProductContainer_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }

        /** ReversalDoc_ID AD_Reference_ID=295 */
        public static int REVERSALDOC_ID_AD_Reference_ID = 295;
        /** Set Reversal Document.@param ReversalDoc_ID Reversal Document */
        public void SetReversalDoc_ID(int ReversalDoc_ID)
        {
            if (ReversalDoc_ID <= 0) Set_Value("ReversalDoc_ID", null);
            else
                Set_Value("ReversalDoc_ID", ReversalDoc_ID);
        }
        /** Get Reversal Document.@return Reversal Document */
        public int GetReversalDoc_ID() { Object ii = Get_Value("ReversalDoc_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Asset Value.
@param VAFAM_AssetValue Asset Value */
        public void SetVAFAM_AssetValue(Decimal? VAFAM_AssetValue) { Set_Value("VAFAM_AssetValue", (Decimal?)VAFAM_AssetValue); }/** Get Asset Value.
@return Asset Value */
        public Decimal GetVAFAM_AssetValue() { Object bd = Get_Value("VAFAM_AssetValue"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Depreciation Amount.
@param VAFAM_DepAmount The amount to be depreciated from the Asset. */
        public void SetVAFAM_DepAmount(Decimal? VAFAM_DepAmount) { Set_Value("VAFAM_DepAmount", (Decimal?)VAFAM_DepAmount); }/** Get Depreciation Amount.
@return The amount to be depreciated from the Asset. */
        public Decimal GetVAFAM_DepAmount() { Object bd = Get_Value("VAFAM_DepAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Set Asset Quantity.
@param VAFAM_Quantity Asset Quantity */
        public void SetVAFAM_Quantity(Decimal? VAFAM_Quantity) { Set_Value("VAFAM_Quantity", (Decimal?)VAFAM_Quantity); }/** Get Asset Quantity.
@return Asset Quantity */
        public Decimal GetVAFAM_Quantity() { Object bd = Get_Value("VAFAM_Quantity"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }

}
