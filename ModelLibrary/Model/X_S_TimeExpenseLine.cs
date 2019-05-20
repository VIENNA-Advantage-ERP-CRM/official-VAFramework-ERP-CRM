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
    /** Generated Model for S_TimeExpenseLine
         *  @author Jagmohan Bhatt (generated) 
         *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_S_TimeExpenseLine : PO
    {
        public X_S_TimeExpenseLine(Context ctx, int S_TimeExpenseLine_ID, Trx trxName)
            : base(ctx, S_TimeExpenseLine_ID, trxName)
        {
            /** if (S_TimeExpenseLine_ID == 0)
            {
            SetIsInvoiced (false);
            SetIsTimeReport (true);	// Y
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM S_TimeExpenseLine WHERE S_TimeExpense_ID=@S_TimeExpense_ID@
            SetProcessed (false);	// N
            SetS_TimeExpenseLine_ID (0);
            SetS_TimeExpense_ID (0);
            }
             */
        }
        public X_S_TimeExpenseLine(Ctx ctx, int S_TimeExpenseLine_ID, Trx trxName)
            : base(ctx, S_TimeExpenseLine_ID, trxName)
        {
            /** if (S_TimeExpenseLine_ID == 0)
            {
            SetIsInvoiced (false);
            SetIsTimeReport (true);	// Y
            SetLine (0);	// @SQL=SELECT NVL(MAX(Line),0)+10 AS DefaultValue FROM S_TimeExpenseLine WHERE S_TimeExpense_ID=@S_TimeExpense_ID@
            SetProcessed (false);	// N
            SetS_TimeExpenseLine_ID (0);
            SetS_TimeExpense_ID (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_TimeExpenseLine(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_TimeExpenseLine(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_S_TimeExpenseLine(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_S_TimeExpenseLine()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27617992269466L;
        /** Last Updated Timestamp 5/1/2012 3:39:12 PM */
        public static long updatedMS = 1335866952677L;
        /** AD_Table_ID=488 */
        public static int Table_ID;
        // =488;

        /** TableName=S_TimeExpenseLine */
        public static String Table_Name = "S_TimeExpenseLine";

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
            StringBuilder sb = new StringBuilder("X_S_TimeExpenseLine[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set AP Actual Hrs.
        @param APActualHrs AP Actual Hrs */
        public void SetAPActualHrs(Decimal? APActualHrs)
        {
            Set_Value("APActualHrs", (Decimal?)APActualHrs);
        }
        /** Get AP Actual Hrs.
        @return AP Actual Hrs */
        public Decimal GetAPActualHrs()
        {
            Object bd = Get_Value("APActualHrs");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set AP Approved Hrs.
        @param APApprovedHrs AP Approved Hrs */
        public void SetAPApprovedHrs(Decimal? APApprovedHrs)
        {
            Set_Value("APApprovedHrs", (Decimal?)APApprovedHrs);
        }
        /** Get AP Approved Hrs.
        @return AP Approved Hrs */
        public Decimal GetAPApprovedHrs()
        {
            Object bd = Get_Value("APApprovedHrs");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set AP Invoice.
        @param APInvoice AP Invoice */
        public void SetAPInvoice(Boolean APInvoice)
        {
            Set_Value("APInvoice", APInvoice);
        }
        /** Get AP Invoice.
        @return AP Invoice */
        public Boolean IsAPInvoice()
        {
            Object oo = Get_Value("APInvoice");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set AR Actual Hrs.
        @param ARActualHrs AR Actual Hrs */
        public void SetARActualHrs(Decimal? ARActualHrs)
        {
            Set_Value("ARActualHrs", (Decimal?)ARActualHrs);
        }
        /** Get AR Actual Hrs.
        @return AR Actual Hrs */
        public Decimal GetARActualHrs()
        {
            Object bd = Get_Value("ARActualHrs");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set AR Approved Hrs.
        @param ARApprovedHrs AR Approved Hrs */
        public void SetARApprovedHrs(Decimal? ARApprovedHrs)
        {
            Set_Value("ARApprovedHrs", (Decimal?)ARApprovedHrs);
        }
        /** Get AR Approved Hrs.
        @return AR Approved Hrs */
        public Decimal GetARApprovedHrs()
        {
            Object bd = Get_Value("ARApprovedHrs");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set AR Invoice.
        @param ARInvoice AR Invoice */
        public void SetARInvoice(Boolean ARInvoice)
        {
            Set_Value("ARInvoice", ARInvoice);
        }
        /** Get AR Invoice.
        @return AR Invoice */
        public Boolean IsARInvoice()
        {
            Object oo = Get_Value("ARInvoice");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Actual Expense Amt.
        @param ActualExpenseAmt Actual Expense Amt */
        public void SetActualExpenseAmt(Decimal? ActualExpenseAmt)
        {
            Set_Value("ActualExpenseAmt", (Decimal?)ActualExpenseAmt);
        }
        /** Get Actual Expense Amt.
        @return Actual Expense Amt */
        public Decimal GetActualExpenseAmt()
        {
            Object bd = Get_Value("ActualExpenseAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Actual Quantity.
        @param ActualQty The actual quantity */
        public void SetActualQty(Decimal? ActualQty)
        {
            Set_Value("ActualQty", (Decimal?)ActualQty);
        }
        /** Get Actual Quantity.
        @return The actual quantity */
        public Decimal GetActualQty()
        {
            Object bd = Get_Value("ActualQty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Approved Expense Amt.
        @param ApprovedExpenseAmt Approved Expense Amt */
        public void SetApprovedExpenseAmt(Decimal? ApprovedExpenseAmt)
        {
            Set_Value("ApprovedExpenseAmt", (Decimal?)ApprovedExpenseAmt);
        }
        /** Get Approved Expense Amt.
        @return Approved Expense Amt */
        public Decimal GetApprovedExpenseAmt()
        {
            Object bd = Get_Value("ApprovedExpenseAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Bill To Customer.
        @param BillToCustomer Bill To Customer */
        public void SetBillToCustomer(Boolean BillToCustomer)
        {
            Set_Value("BillToCustomer", BillToCustomer);
        }
        /** Get Bill To Customer.
        @return Bill To Customer */
        public Boolean IsBillToCustomer()
        {
            Object oo = Get_Value("BillToCustomer");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
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
        /** Set Customer/Prospect.
        @param C_BPartner_ID Identifies a Customer/Prospect */
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            if (C_BPartner_ID <= 0) Set_Value("C_BPartner_ID", null);
            else
                Set_Value("C_BPartner_ID", C_BPartner_ID);
        }
        /** Get Customer/Prospect.
        @return Identifies a Customer/Prospect */
        public int GetC_BPartner_ID()
        {
            Object ii = Get_Value("C_BPartner_ID");
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
        /** Set Invoice Line.
        @param C_InvoiceLine_ID Invoice Detail Line */
        public void SetC_InvoiceLine_ID(int C_InvoiceLine_ID)
        {
            if (C_InvoiceLine_ID <= 0) Set_ValueNoCheck("C_InvoiceLine_ID", null);
            else
                Set_ValueNoCheck("C_InvoiceLine_ID", C_InvoiceLine_ID);
        }
        /** Get Invoice Line.
        @return Invoice Detail Line */
        public int GetC_InvoiceLine_ID()
        {
            Object ii = Get_Value("C_InvoiceLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice.
        @param C_Invoice_ID Invoice Identifier */
        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            if (C_Invoice_ID <= 0) Set_Value("C_Invoice_ID", null);
            else
                Set_Value("C_Invoice_ID", C_Invoice_ID);
        }
        /** Get Invoice.
        @return Invoice Identifier */
        public int GetC_Invoice_ID()
        {
            Object ii = Get_Value("C_Invoice_ID");
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
        @param C_ProjectTask_ID Actual Plan Task in a Phase */
        public void SetC_ProjectTask_ID(int C_ProjectTask_ID)
        {
            if (C_ProjectTask_ID <= 0) Set_Value("C_ProjectTask_ID", null);
            else
                Set_Value("C_ProjectTask_ID", C_ProjectTask_ID);
        }
        /** Get Project Task.
        @return Actual Plan Task in a Phase */
        public int GetC_ProjectTask_ID()
        {
            Object ii = Get_Value("C_ProjectTask_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Opportunity.
        @param C_Project_ID Business Opportunity */
        public void SetC_Project_ID(int C_Project_ID)
        {
            if (C_Project_ID <= 0) Set_Value("C_Project_ID", null);
            else
                Set_Value("C_Project_ID", C_Project_ID);
        }
        /** Get Opportunity.
        @return Business Opportunity */
        public int GetC_Project_ID()
        {
            Object ii = Get_Value("C_Project_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set C_ResourceTime_ID.
        @param C_ResourceTime_ID C_ResourceTime_ID */
        public void SetC_ResourceTime_ID(int C_ResourceTime_ID)
        {
            if (C_ResourceTime_ID <= 0) Set_Value("C_ResourceTime_ID", null);
            else
                Set_Value("C_ResourceTime_ID", C_ResourceTime_ID);
        }
        /** Get C_ResourceTime_ID.
        @return C_ResourceTime_ID */
        public int GetC_ResourceTime_ID()
        {
            Object ii = Get_Value("C_ResourceTime_ID");
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
        /** Set Converted Amount.
        @param ConvertedAmt Converted Amount */
        public void SetConvertedAmt(Decimal? ConvertedAmt)
        {
            Set_Value("ConvertedAmt", (Decimal?)ConvertedAmt);
        }
        /** Get Converted Amount.
        @return Converted Amount */
        public Decimal GetConvertedAmt()
        {
            Object bd = Get_Value("ConvertedAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Expense Date.
        @param DateExpense Date of expense */
        public void SetDateExpense(DateTime? DateExpense)
        {
            Set_Value("DateExpense", (DateTime?)DateExpense);
        }
        /** Get Expense Date.
        @return Date of expense */
        public DateTime? GetDateExpense()
        {
            return (DateTime?)Get_Value("DateExpense");
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
        /** Set Expense Amount.
        @param ExpenseAmt Amount for this expense */
        public void SetExpenseAmt(Decimal? ExpenseAmt)
        {
            Set_Value("ExpenseAmt", (Decimal?)ExpenseAmt);
        }
        /** Get Expense Amount.
        @return Amount for this expense */
        public Decimal GetExpenseAmt()
        {
            Object bd = Get_Value("ExpenseAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Expense Invoice.
        @param ExpenseInvoice Expense Invoice */
        public void SetExpenseInvoice(Boolean ExpenseInvoice)
        {
            Set_Value("ExpenseInvoice", ExpenseInvoice);
        }
        /** Get Expense Invoice.
        @return Expense Invoice */
        public Boolean IsExpenseInvoice()
        {
            Object oo = Get_Value("ExpenseInvoice");
            if (oo != null)
            {
                if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo);
                return "Y".Equals(oo);
            }
            return false;
        }
        /** Set Invoice Price.
        @param InvoicePrice Unit price to be invoiced or 0 for default price */
        public void SetInvoicePrice(Decimal? InvoicePrice)
        {
            Set_Value("InvoicePrice", (Decimal?)InvoicePrice);
        }
        /** Get Invoice Price.
        @return Unit price to be invoiced or 0 for default price */
        public Decimal GetInvoicePrice()
        {
            Object bd = Get_Value("InvoicePrice");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
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
        /** Set Time Report.
        @param IsTimeReport Line is a time report only (no expense) */
        public void SetIsTimeReport(Boolean IsTimeReport)
        {
            Set_Value("IsTimeReport", IsTimeReport);
        }
        /** Get Time Report.
        @return Line is a time report only (no expense) */
        public Boolean IsTimeReport()
        {
            Object oo = Get_Value("IsTimeReport");
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
        /** Set Note.
        @param Note Optional additional user defined information */
        public void SetNote(String Note)
        {
            if (Note != null && Note.Length > 255)
            {
                log.Warning("Length > 255 - truncated");
                Note = Note.Substring(0, 255);
            }
            Set_Value("Note", Note);
        }
        /** Get Note.
        @return Optional additional user defined information */
        public String GetNote()
        {
            return (String)Get_Value("Note");
        }
        /** Set Price Invoiced.
        @param PriceInvoiced The priced invoiced to the customer (in the currency of the customer's AR price list) - 0 for default price */
        public void SetPriceInvoiced(Decimal? PriceInvoiced)
        {
            Set_Value("PriceInvoiced", (Decimal?)PriceInvoiced);
        }
        /** Get Price Invoiced.
        @return The priced invoiced to the customer (in the currency of the customer's AR price list) - 0 for default price */
        public Decimal GetPriceInvoiced()
        {
            Object bd = Get_Value("PriceInvoiced");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Price Reimbursed.
        @param PriceReimbursed The reimbursed price (in currency of the employee's AP price list) */
        public void SetPriceReimbursed(Decimal? PriceReimbursed)
        {
            Set_Value("PriceReimbursed", (Decimal?)PriceReimbursed);
        }
        /** Get Price Reimbursed.
        @return The reimbursed price (in currency of the employee's AP price list) */
        public Decimal GetPriceReimbursed()
        {
            Object bd = Get_Value("PriceReimbursed");
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
        @param Qty Quantity */
        public void SetQty(Decimal? Qty)
        {
            Set_Value("Qty", (Decimal?)Qty);
        }
        /** Get Quantity.
        @return Quantity */
        public Decimal GetQty()
        {
            Object bd = Get_Value("Qty");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Invoiced.
        @param QtyInvoiced Invoiced Quantity */
        public void SetQtyInvoiced(Decimal? QtyInvoiced)
        {
            Set_Value("QtyInvoiced", (Decimal?)QtyInvoiced);
        }
        /** Get Quantity Invoiced.
        @return Invoiced Quantity */
        public Decimal GetQtyInvoiced()
        {
            Object bd = Get_Value("QtyInvoiced");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set Quantity Reimbursed.
        @param QtyReimbursed The reimbursed quantity */
        public void SetQtyReimbursed(Decimal? QtyReimbursed)
        {
            Set_Value("QtyReimbursed", (Decimal?)QtyReimbursed);
        }
        /** Get Quantity Reimbursed.
        @return The reimbursed quantity */
        public Decimal GetQtyReimbursed()
        {
            Object bd = Get_Value("QtyReimbursed");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
        /** Set AR Invoice.
        @param Ref_C_Invoice_ID AR Invoice */
        public void SetRef_C_Invoice_ID(int Ref_C_Invoice_ID)
        {
            if (Ref_C_Invoice_ID <= 0) Set_Value("Ref_C_Invoice_ID", null);
            else
                Set_Value("Ref_C_Invoice_ID", Ref_C_Invoice_ID);
        }
        /** Get AR Invoice.
        @return AR Invoice */
        public int GetRef_C_Invoice_ID()
        {
            Object ii = Get_Value("Ref_C_Invoice_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Assigned Resource.
        @param S_ResourceAssignment_ID Assigned Resource */
        public void SetS_ResourceAssignment_ID(int S_ResourceAssignment_ID)
        {
            if (S_ResourceAssignment_ID <= 0) Set_Value("S_ResourceAssignment_ID", null);
            else
                Set_Value("S_ResourceAssignment_ID", S_ResourceAssignment_ID);
        }
        /** Get Assigned Resource.
        @return Assigned Resource */
        public int GetS_ResourceAssignment_ID()
        {
            Object ii = Get_Value("S_ResourceAssignment_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Expense Line.
        @param S_TimeExpenseLine_ID Time and Expense Report Line */
        public void SetS_TimeExpenseLine_ID(int S_TimeExpenseLine_ID)
        {
            if (S_TimeExpenseLine_ID < 1) throw new ArgumentException("S_TimeExpenseLine_ID is mandatory.");
            Set_ValueNoCheck("S_TimeExpenseLine_ID", S_TimeExpenseLine_ID);
        }
        /** Get Expense Line.
        @return Time and Expense Report Line */
        public int GetS_TimeExpenseLine_ID()
        {
            Object ii = Get_Value("S_TimeExpenseLine_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Expense Report.
        @param S_TimeExpense_ID Time and Expense Report */
        public void SetS_TimeExpense_ID(int S_TimeExpense_ID)
        {
            if (S_TimeExpense_ID < 1) throw new ArgumentException("S_TimeExpense_ID is mandatory.");
            Set_ValueNoCheck("S_TimeExpense_ID", S_TimeExpense_ID);
        }
        /** Get Expense Report.
        @return Time and Expense Report */
        public int GetS_TimeExpense_ID()
        {
            Object ii = Get_Value("S_TimeExpense_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Time Type.
        @param S_TimeType_ID Type of time recorded */
        public void SetS_TimeType_ID(int S_TimeType_ID)
        {
            if (S_TimeType_ID <= 0) Set_Value("S_TimeType_ID", null);
            else
                Set_Value("S_TimeType_ID", S_TimeType_ID);
        }
        /** Get Time Type.
        @return Type of time recorded */
        public int GetS_TimeType_ID()
        {
            Object ii = Get_Value("S_TimeType_ID");
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

        /** Set Resource.
    @param S_Resource_ID Resource */
        public void SetS_Resource_ID(int S_Resource_ID)
        {
            if (S_Resource_ID <= 0) Set_Value("S_Resource_ID", null);
            else
                Set_Value("S_Resource_ID", S_Resource_ID);
        }
        /** Get Resource.
        @return Resource */
        public int GetS_Resource_ID()
        {
            Object ii = Get_Value("S_Resource_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }

        /** Set Planned Hours.
       @param PlannedHours Planned Hours */
        public void SetPlannedHours(Decimal? PlannedHours)
        {
            Set_Value("PlannedHours", (Decimal?)PlannedHours);
        }
        /** Get Planned Hours.
        @return Planned Hours */
        public Decimal GetPlannedHours()
        {
            Object bd = Get_Value("PlannedHours");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }

        /** Set Approved Expense Amt.
       @param ApprovedExpenseAmt Approved Expense Amt */
        public void SetApprovedARExpenseAmt(Decimal? ApprovedARExpenseAmt)
        {
            Set_Value("ApprovedARExpenseAmt", (Decimal?)ApprovedARExpenseAmt);
        }
        /** Get Approved Expense Amt.
        @return Approved Expense Amt */
        public Decimal GetApprovedARExpenseAmt()
        {
            Object bd = Get_Value("ApprovedARExpenseAmt");
            if (bd == null) return Env.ZERO;
            return Convert.ToDecimal(bd);
        }
    }

}
