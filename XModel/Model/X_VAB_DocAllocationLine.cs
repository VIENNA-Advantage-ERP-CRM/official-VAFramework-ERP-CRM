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
    using System.Data;/** Generated Model for VAB_DocAllocationLine
 *  @author Raghu (Updated) 
 *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_VAB_DocAllocationLine : PO
    {
        public X_VAB_DocAllocationLine(Context ctx, int VAB_DocAllocationLine_ID, Trx trxName) : base(ctx, VAB_DocAllocationLine_ID, trxName)
        {/** if (VAB_DocAllocationLine_ID == 0){SetAmount (0.0);SetVAB_DocAllocation_ID (0);SetVAB_DocAllocationLine_ID (0);SetDiscountAmt (0.0);SetWriteOffAmt (0.0);} */
        }
        public X_VAB_DocAllocationLine(Ctx ctx, int VAB_DocAllocationLine_ID, Trx trxName) : base(ctx, VAB_DocAllocationLine_ID, trxName)
        {/** if (VAB_DocAllocationLine_ID == 0){SetAmount (0.0);SetVAB_DocAllocation_ID (0);SetVAB_DocAllocationLine_ID (0);SetDiscountAmt (0.0);SetWriteOffAmt (0.0);} */
        }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_DocAllocationLine(Context ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_DocAllocationLine(Ctx ctx, DataRow rs, Trx trxName) : base(ctx, rs, trxName) { }/** Load Constructor 
@param ctx context
@param rs result set 
@param trxName transaction
*/
        public X_VAB_DocAllocationLine(Ctx ctx, IDataReader dr, Trx trxName) : base(ctx, dr, trxName) { }/** Static Constructor 
 Set Table ID By Table Name
 added by ->Harwinder */
        static X_VAB_DocAllocationLine() { Table_ID = Get_Table_ID(Table_Name); model = new KeyNamePair(Table_ID, Table_Name); }/** Serial Version No */
        static long serialVersionUID = 27884379430766L;/** Last Updated Timestamp 10/9/2020 2:35:14 PM */
        public static long updatedMS = 1602254113977L;/** VAF_TableView_ID=390 */
        public static int Table_ID; // =390;
        /** TableName=VAB_DocAllocationLine */
        public static String Table_Name = "VAB_DocAllocationLine";
        protected static KeyNamePair model; protected Decimal accessLevel = new Decimal(1);/** AccessLevel
@return 1 - Org 
*/
        protected override int Get_AccessLevel() { return Convert.ToInt32(accessLevel.ToString()); }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Context ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Load Meta Data
@param ctx context
@return PO Info
*/
        protected override POInfo InitPO(Ctx ctx) { POInfo poi = POInfo.GetPOInfo(ctx, Table_ID); return poi; }/** Info
@return info
*/
        public override String ToString() { StringBuilder sb = new StringBuilder("X_VAB_DocAllocationLine[").Append(Get_ID()).Append("]"); return sb.ToString(); }/** Set Amount.
@param Amount Amount in a defined currency */
        public void SetAmount(Decimal? Amount) { if (Amount == null) throw new ArgumentException("Amount is mandatory."); Set_ValueNoCheck("Amount", (Decimal?)Amount); }/** Get Amount.
@return Amount in a defined currency */
        public Decimal GetAmount() { Object bd = Get_Value("Amount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Backup Withholding Amount.
@param BackupWithholdingAmount Backup Withholding Amount */
        public void SetBackupWithholdingAmount(Decimal? BackupWithholdingAmount) { Set_Value("BackupWithholdingAmount", (Decimal?)BackupWithholdingAmount); }/** Get Backup Withholding Amount.
@return Backup Withholding Amount */
        public Decimal GetBackupWithholdingAmount() { Object bd = Get_Value("BackupWithholdingAmount"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Allocation.
@param VAB_DocAllocation_ID Payment allocation */
        public void SetVAB_DocAllocation_ID(int VAB_DocAllocation_ID) { if (VAB_DocAllocation_ID < 1) throw new ArgumentException("VAB_DocAllocation_ID is mandatory."); Set_ValueNoCheck("VAB_DocAllocation_ID", VAB_DocAllocation_ID); }/** Get Allocation.
@return Payment allocation */
        public int GetVAB_DocAllocation_ID() { Object ii = Get_Value("VAB_DocAllocation_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Allocation Line.
@param VAB_DocAllocationLine_ID Allocation Line */
        public void SetVAB_DocAllocationLine_ID(int VAB_DocAllocationLine_ID) { if (VAB_DocAllocationLine_ID < 1) throw new ArgumentException("VAB_DocAllocationLine_ID is mandatory."); Set_ValueNoCheck("VAB_DocAllocationLine_ID", VAB_DocAllocationLine_ID); }/** Get Allocation Line.
@return Allocation Line */
        public int GetVAB_DocAllocationLine_ID() { Object ii = Get_Value("VAB_DocAllocationLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Business Partner.
@param VAB_BusinessPartner_ID Identifies a Customer/Prospect */
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            if (VAB_BusinessPartner_ID <= 0) Set_ValueNoCheck("VAB_BusinessPartner_ID", null);
            else
                Set_ValueNoCheck("VAB_BusinessPartner_ID", VAB_BusinessPartner_ID);
        }/** Get Business Partner.
@return Identifies a Customer/Prospect */
        public int GetVAB_BusinessPartner_ID() { Object ii = Get_Value("VAB_BusinessPartner_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cash Book.
@param VAB_CashBook_ID Cash Book for recording petty cash transactions */
        public void SetVAB_CashBook_ID(int VAB_CashBook_ID) { throw new ArgumentException("VAB_CashBook_ID Is virtual column"); }/** Get Cash Book.
@return Cash Book for recording petty cash transactions */
        public int GetVAB_CashBook_ID() { Object ii = Get_Value("VAB_CashBook_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Cash Journal Line.
@param VAB_CashJRNLLine_ID Cash Journal Line */
        public void SetVAB_CashJRNLLine_ID(int VAB_CashJRNLLine_ID)
        {
            if (VAB_CashJRNLLine_ID <= 0) Set_ValueNoCheck("VAB_CashJRNLLine_ID", null);
            else
                Set_ValueNoCheck("VAB_CashJRNLLine_ID", VAB_CashJRNLLine_ID);
        }/** Get Cash Journal Line.
@return Cash Journal Line */
        public int GetVAB_CashJRNLLine_ID() { Object ii = Get_Value("VAB_CashJRNLLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice Payment Schedule.
@param VAB_sched_InvoicePayment_ID Invoice Payment Schedule */
        public void SetVAB_sched_InvoicePayment_ID(int VAB_sched_InvoicePayment_ID)
        {
            if (VAB_sched_InvoicePayment_ID <= 0) Set_Value("VAB_sched_InvoicePayment_ID", null);
            else
                Set_Value("VAB_sched_InvoicePayment_ID", VAB_sched_InvoicePayment_ID);
        }/** Get Invoice Payment Schedule.
@return Invoice Payment Schedule */
        public int GetVAB_sched_InvoicePayment_ID() { Object ii = Get_Value("VAB_sched_InvoicePayment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Invoice.
@param VAB_Invoice_ID Invoice Identifier */
        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            if (VAB_Invoice_ID <= 0) Set_ValueNoCheck("VAB_Invoice_ID", null);
            else
                Set_ValueNoCheck("VAB_Invoice_ID", VAB_Invoice_ID);
        }/** Get Invoice.
@return Invoice Identifier */
        public int GetVAB_Invoice_ID() { Object ii = Get_Value("VAB_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Get Record ID/ColumnName
@return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair() { return new KeyNamePair(Get_ID(), GetVAB_Invoice_ID().ToString()); }/** Set Order.
@param VAB_Order_ID Sales Order */
        public void SetVAB_Order_ID(int VAB_Order_ID)
        {
            if (VAB_Order_ID <= 0) Set_ValueNoCheck("VAB_Order_ID", null);
            else
                Set_ValueNoCheck("VAB_Order_ID", VAB_Order_ID);
        }/** Get Order.
@return Sales Order */
        public int GetVAB_Order_ID() { Object ii = Get_Value("VAB_Order_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Payment.
@param VAB_Payment_ID Payment identifier */
        public void SetVAB_Payment_ID(int VAB_Payment_ID)
        {
            if (VAB_Payment_ID <= 0) Set_ValueNoCheck("VAB_Payment_ID", null);
            else
                Set_ValueNoCheck("VAB_Payment_ID", VAB_Payment_ID);
        }/** Get Payment.
@return Payment identifier */
        public int GetVAB_Payment_ID() { Object ii = Get_Value("VAB_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Transaction Date.
@param DateTrx Transaction Date */
        public void SetDateTrx(DateTime? DateTrx) { Set_ValueNoCheck("DateTrx", (DateTime?)DateTrx); }/** Get Transaction Date.
@return Transaction Date */
        public DateTime? GetDateTrx() { return (DateTime?)Get_Value("DateTrx"); }/** Set Discount Amount.
@param DiscountAmt Calculated amount of discount */
        public void SetDiscountAmt(Decimal? DiscountAmt) { if (DiscountAmt == null) throw new ArgumentException("DiscountAmt is mandatory."); Set_ValueNoCheck("DiscountAmt", (Decimal?)DiscountAmt); }/** Get Discount Amount.
@return Calculated amount of discount */
        public Decimal GetDiscountAmt() { Object bd = Get_Value("DiscountAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Export.
@param Export_ID Export */
        public void SetExport_ID(String Export_ID) { if (Export_ID != null && Export_ID.Length > 50) { log.Warning("Length > 50 - truncated"); Export_ID = Export_ID.Substring(0, 50); } Set_ValueNoCheck("Export_ID", Export_ID); }/** Get Export.
@return Export */
        public String GetExport_ID() { return (String)Get_Value("Export_ID"); }/** Set Total Amount.
@param FRPT_TotalAmt Total Amount */
        public void SetFRPT_TotalAmt(Decimal? FRPT_TotalAmt) { throw new ArgumentException("FRPT_TotalAmt Is virtual column"); }/** Get Total Amount.
@return Total Amount */
        public Decimal GetFRPT_TotalAmt() { Object bd = Get_Value("FRPT_TotalAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Journal Line.
@param VAGL_JRNLLine_ID General Ledger Journal Line */
        public void SetVAGL_JRNLLine_ID(int VAGL_JRNLLine_ID)
        {
            if (VAGL_JRNLLine_ID <= 0) Set_Value("VAGL_JRNLLine_ID", null);
            else
                Set_Value("VAGL_JRNLLine_ID", VAGL_JRNLLine_ID);
        }/** Get Journal Line.
@return General Ledger Journal Line */
        public int GetVAGL_JRNLLine_ID() { Object ii = Get_Value("VAGL_JRNLLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Generate Allocation.
@param GenerateAllocation Generate Allocation */
        public void SetGenerateAllocation(String GenerateAllocation) { if (GenerateAllocation != null && GenerateAllocation.Length > 10) { log.Warning("Length > 10 - truncated"); GenerateAllocation = GenerateAllocation.Substring(0, 10); } Set_Value("GenerateAllocation", GenerateAllocation); }/** Get Generate Allocation.
@return Generate Allocation */
        public String GetGenerateAllocation() { return (String)Get_Value("GenerateAllocation"); }/** Set Manual.
@param IsManual This is a manual process */
        public void SetIsManual(Boolean IsManual) { Set_ValueNoCheck("IsManual", IsManual); }/** Get Manual.
@return This is a manual process */
        public Boolean IsManual() { Object oo = Get_Value("IsManual"); if (oo != null) { if (oo.GetType() == typeof(bool)) return Convert.ToBoolean(oo); return "Y".Equals(oo); } return false; }/** Set VAM_ProductCostAllocation_ID.
@param VAM_ProductCostAllocation_ID VAM_ProductCostAllocation_ID */
        public void SetVAM_ProductCostAllocation_ID(int VAM_ProductCostAllocation_ID)
        {
            if (VAM_ProductCostAllocation_ID <= 0) Set_Value("VAM_ProductCostAllocation_ID", null);
            else
                Set_Value("VAM_ProductCostAllocation_ID", VAM_ProductCostAllocation_ID);
        }/** Get VAM_ProductCostAllocation_ID.
@return VAM_ProductCostAllocation_ID */
        public int GetVAM_ProductCostAllocation_ID() { Object ii = Get_Value("VAM_ProductCostAllocation_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Over/Under Payment.
@param OverUnderAmt Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
        public void SetOverUnderAmt(Decimal? OverUnderAmt) { Set_Value("OverUnderAmt", (Decimal?)OverUnderAmt); }/** Get Over/Under Payment.
@return Over-Payment (unallocated) or Under-Payment (partial payment) Amount */
        public Decimal GetOverUnderAmt() { Object bd = Get_Value("OverUnderAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
        /** Ref_VAB_Invoice_ID VAF_Control_Ref_ID=336 */
        public static int REF_VAB_INVOICE_ID_VAF_Control_Ref_ID = 336;/** Set Invoice Ref.
@param Ref_VAB_Invoice_ID Invoice Ref */
        public void SetRef_VAB_Invoice_ID(int Ref_VAB_Invoice_ID)
        {
            if (Ref_VAB_Invoice_ID <= 0) Set_Value("Ref_VAB_Invoice_ID", null);
            else
                Set_Value("Ref_VAB_Invoice_ID", Ref_VAB_Invoice_ID);
        }/** Get Invoice Ref.
@return Invoice Ref */
        public int GetRef_VAB_Invoice_ID() { Object ii = Get_Value("Ref_VAB_Invoice_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Ref_CashLine_ID VAF_Control_Ref_ID=1000602 */
        public static int REF_CASHLINE_ID_VAF_Control_Ref_ID = 1000602;/** Set Ref Cash Journal.
@param Ref_CashLine_ID Ref Cash Journal */
        public void SetRef_CashLine_ID(int Ref_CashLine_ID)
        {
            if (Ref_CashLine_ID <= 0) Set_Value("Ref_CashLine_ID", null);
            else
                Set_Value("Ref_CashLine_ID", Ref_CashLine_ID);
        }/** Get Ref Cash Journal.
@return Ref Cash Journal */
        public int GetRef_CashLine_ID() { Object ii = Get_Value("Ref_CashLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Ref_GLJournalLine_ID VAF_Control_Ref_ID=1000314 */
        public static int REF_GLJOURNALLINE_ID_VAF_Control_Ref_ID = 1000314;/** Set Ref GL Journal Line.
@param Ref_GLJournalLine_ID General Ledger Journal Line */
        public void SetRef_GLJournalLine_ID(int Ref_GLJournalLine_ID)
        {
            if (Ref_GLJournalLine_ID <= 0) Set_Value("Ref_GLJournalLine_ID", null);
            else
                Set_Value("Ref_GLJournalLine_ID", Ref_GLJournalLine_ID);
        }/** Get Ref GL Journal Line.
@return General Ledger Journal Line */
        public int GetRef_GLJournalLine_ID() { Object ii = Get_Value("Ref_GLJournalLine_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Ref_Invoiceschedule_ID VAF_Control_Ref_ID=1000603 */
        public static int REF_INVOICESCHEDULE_ID_VAF_Control_Ref_ID = 1000603;/** Set Ref Invoice Schedule.
@param Ref_Invoiceschedule_ID Ref Invoice Schedule */
        public void SetRef_Invoiceschedule_ID(int Ref_Invoiceschedule_ID)
        {
            if (Ref_Invoiceschedule_ID <= 0) Set_Value("Ref_Invoiceschedule_ID", null);
            else
                Set_Value("Ref_Invoiceschedule_ID", Ref_Invoiceschedule_ID);
        }/** Get Ref Invoice Schedule.
@return Ref Invoice Schedule */
        public int GetRef_Invoiceschedule_ID() { Object ii = Get_Value("Ref_Invoiceschedule_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }
        /** Ref_Payment_ID VAF_Control_Ref_ID=343 */
        public static int REF_PAYMENT_ID_VAF_Control_Ref_ID = 343;/** Set Referenced Payment.
@param Ref_Payment_ID Referenced Payment */
        public void SetRef_Payment_ID(int Ref_Payment_ID)
        {
            if (Ref_Payment_ID <= 0) Set_Value("Ref_Payment_ID", null);
            else
                Set_Value("Ref_Payment_ID", Ref_Payment_ID);
        }/** Get Referenced Payment.
@return Referenced Payment */
        public int GetRef_Payment_ID() { Object ii = Get_Value("Ref_Payment_ID"); if (ii == null) return 0; return Convert.ToInt32(ii); }/** Set Withholding Amount.
@param WithholdingAmt This field represents the calculated withholding amount */
        public void SetWithholdingAmt(Decimal? WithholdingAmt) { Set_Value("WithholdingAmt", (Decimal?)WithholdingAmt); }/** Get Withholding Amount.
@return This field represents the calculated withholding amount */
        public Decimal GetWithholdingAmt() { Object bd = Get_Value("WithholdingAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }/** Set Write-off Amount.
@param WriteOffAmt Amount to write-off */
        public void SetWriteOffAmt(Decimal? WriteOffAmt) { if (WriteOffAmt == null) throw new ArgumentException("WriteOffAmt is mandatory."); Set_Value("WriteOffAmt", (Decimal?)WriteOffAmt); }/** Get Write-off Amount.
@return Amount to write-off */
        public Decimal GetWriteOffAmt() { Object bd = Get_Value("WriteOffAmt"); if (bd == null) return Env.ZERO; return Convert.ToDecimal(bd); }
    }
}