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
    /** Generated Model for C_AcctSchema_Default
     *  @author Jagmohan Bhatt (generated) 
     *  @version Vienna Framework 1.1.1 - $Id$ */
    public class X_C_AcctSchema_Default : PO
    {

        public X_C_AcctSchema_Default(Context ctx, int C_AcctSchema_Default_ID, Trx trxName)
            : base(ctx, C_AcctSchema_Default_ID, trxName)
        {
            /** if (C_AcctSchema_Default_ID == 0)
            {
            SetB_Asset_Acct (0);
            SetB_Expense_Acct (0);
            SetB_InTransit_Acct (0);
            SetB_InterestExp_Acct (0);
            SetB_InterestRev_Acct (0);
            SetB_PaymentSelect_Acct (0);
            SetB_RevaluationGain_Acct (0);
            SetB_RevaluationLoss_Acct (0);
            SetB_SettlementGain_Acct (0);
            SetB_SettlementLoss_Acct (0);
            SetB_UnallocatedCash_Acct (0);
            SetB_Unidentified_Acct (0);
            SetCB_Asset_Acct (0);
            SetCB_CashTransfer_Acct (0);
            SetCB_Differences_Acct (0);
            SetCB_Expense_Acct (0);
            SetCB_Receipt_Acct (0);
            SetC_AcctSchema_ID (0);
            SetC_Prepayment_Acct (0);
            SetC_Receivable_Acct (0);
            SetC_Receivable_Services_Acct (0);
            SetCh_Expense_Acct (0);
            SetCh_Revenue_Acct (0);
            SetE_Expense_Acct (0);
            SetE_Prepayment_Acct (0);
            SetNotInvoicedReceipts_Acct (0);
            SetNotInvoicedReceivables_Acct (0);
            SetNotInvoicedRevenue_Acct (0);
            SetPJ_Asset_Acct (0);
            SetPJ_WIP_Acct (0);
            SetP_Asset_Acct (0);
            SetP_COGS_Acct (0);
            SetP_CostAdjustment_Acct (0);
            SetP_Expense_Acct (0);
            SetP_InventoryClearing_Acct (0);
            SetP_InvoicePriceVariance_Acct (0);
            SetP_PurchasePriceVariance_Acct (0);
            SetP_Revenue_Acct (0);
            SetP_TradeDiscountGrant_Acct (0);
            SetP_TradeDiscountRec_Acct (0);
            SetPayDiscount_Exp_Acct (0);
            SetPayDiscount_Rev_Acct (0);
            SetRealizedGain_Acct (0);
            SetRealizedLoss_Acct (0);
            SetT_Credit_Acct (0);
            SetT_Due_Acct (0);
            SetT_Expense_Acct (0);
            SetT_Liability_Acct (0);
            SetT_Receivables_Acct (0);
            SetUnEarnedRevenue_Acct (0);
            SetUnrealizedGain_Acct (0);
            SetUnrealizedLoss_Acct (0);
            SetV_Liability_Acct (0);
            SetV_Liability_Services_Acct (0);
            SetV_Prepayment_Acct (0);
            SetW_Differences_Acct (0);
            SetW_InvActualAdjust_Acct (0);
            SetW_Inventory_Acct (0);
            SetW_Revaluation_Acct (0);
            SetWithholding_Acct (0);
            SetWriteOff_Acct (0);
            }
             */
        }
        public X_C_AcctSchema_Default(Ctx ctx, int C_AcctSchema_Default_ID, Trx trxName)
            : base(ctx, C_AcctSchema_Default_ID, trxName)
        {
            /** if (C_AcctSchema_Default_ID == 0)
            {
            SetB_Asset_Acct (0);
            SetB_Expense_Acct (0);
            SetB_InTransit_Acct (0);
            SetB_InterestExp_Acct (0);
            SetB_InterestRev_Acct (0);
            SetB_PaymentSelect_Acct (0);
            SetB_RevaluationGain_Acct (0);
            SetB_RevaluationLoss_Acct (0);
            SetB_SettlementGain_Acct (0);
            SetB_SettlementLoss_Acct (0);
            SetB_UnallocatedCash_Acct (0);
            SetB_Unidentified_Acct (0);
            SetCB_Asset_Acct (0);
            SetCB_CashTransfer_Acct (0);
            SetCB_Differences_Acct (0);
            SetCB_Expense_Acct (0);
            SetCB_Receipt_Acct (0);
            SetC_AcctSchema_ID (0);
            SetC_Prepayment_Acct (0);
            SetC_Receivable_Acct (0);
            SetC_Receivable_Services_Acct (0);
            SetCh_Expense_Acct (0);
            SetCh_Revenue_Acct (0);
            SetE_Expense_Acct (0);
            SetE_Prepayment_Acct (0);
            SetNotInvoicedReceipts_Acct (0);
            SetNotInvoicedReceivables_Acct (0);
            SetNotInvoicedRevenue_Acct (0);
            SetPJ_Asset_Acct (0);
            SetPJ_WIP_Acct (0);
            SetP_Asset_Acct (0);
            SetP_COGS_Acct (0);
            SetP_CostAdjustment_Acct (0);
            SetP_Expense_Acct (0);
            SetP_InventoryClearing_Acct (0);
            SetP_InvoicePriceVariance_Acct (0);
            SetP_PurchasePriceVariance_Acct (0);
            SetP_Revenue_Acct (0);
            SetP_TradeDiscountGrant_Acct (0);
            SetP_TradeDiscountRec_Acct (0);
            SetPayDiscount_Exp_Acct (0);
            SetPayDiscount_Rev_Acct (0);
            SetRealizedGain_Acct (0);
            SetRealizedLoss_Acct (0);
            SetT_Credit_Acct (0);
            SetT_Due_Acct (0);
            SetT_Expense_Acct (0);
            SetT_Liability_Acct (0);
            SetT_Receivables_Acct (0);
            SetUnEarnedRevenue_Acct (0);
            SetUnrealizedGain_Acct (0);
            SetUnrealizedLoss_Acct (0);
            SetV_Liability_Acct (0);
            SetV_Liability_Services_Acct (0);
            SetV_Prepayment_Acct (0);
            SetW_Differences_Acct (0);
            SetW_InvActualAdjust_Acct (0);
            SetW_Inventory_Acct (0);
            SetW_Revaluation_Acct (0);
            SetWithholding_Acct (0);
            SetWriteOff_Acct (0);
            }
             */
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_Default(Context ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_Default(Ctx ctx, DataRow rs, Trx trxName)
            : base(ctx, rs, trxName)
        {
        }
        /** Load Constructor 
        @param ctx context
        @param rs result set 
        @param trxName transaction
        */
        public X_C_AcctSchema_Default(Ctx ctx, IDataReader dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }
        /** Static Constructor 
         Set Table ID By Table Name
         added by ->Harwinder */
        static X_C_AcctSchema_Default()
        {
            Table_ID = Get_Table_ID(Table_Name);
            model = new KeyNamePair(Table_ID, Table_Name);
        }
        /** Serial Version No */
        //static long serialVersionUID 27590861777519L;
        /** Last Updated Timestamp 6/22/2011 3:24:20 PM */
        public static long updatedMS = 1308736460730L;
        /** AD_Table_ID=315 */
        public static int Table_ID;
        // =315;

        /** TableName=C_AcctSchema_Default */
        public static String Table_Name = "C_AcctSchema_Default";

        protected static KeyNamePair model;
        protected Decimal accessLevel = new Decimal(2);
        /** AccessLevel
        @return 2 - Client 
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
            StringBuilder sb = new StringBuilder("X_C_AcctSchema_Default[").Append(Get_ID()).Append("]");
            return sb.ToString();
        }
        /** Set Bank Asset.
        @param B_Asset_Acct Bank Asset Account */
        public void SetB_Asset_Acct(int B_Asset_Acct)
        {
            Set_Value("B_Asset_Acct", B_Asset_Acct);
        }
        /** Get Bank Asset.
        @return Bank Asset Account */
        public int GetB_Asset_Acct()
        {
            Object ii = Get_Value("B_Asset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Expense.
        @param B_Expense_Acct Bank Expense Account */
        public void SetB_Expense_Acct(int B_Expense_Acct)
        {
            Set_Value("B_Expense_Acct", B_Expense_Acct);
        }
        /** Get Bank Expense.
        @return Bank Expense Account */
        public int GetB_Expense_Acct()
        {
            Object ii = Get_Value("B_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank In Transit.
        @param B_InTransit_Acct Bank In Transit Account */
        public void SetB_InTransit_Acct(int B_InTransit_Acct)
        {
            Set_Value("B_InTransit_Acct", B_InTransit_Acct);
        }
        /** Get Bank In Transit.
        @return Bank In Transit Account */
        public int GetB_InTransit_Acct()
        {
            Object ii = Get_Value("B_InTransit_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Interest Expense.
        @param B_InterestExp_Acct Bank Interest Expense Account */
        public void SetB_InterestExp_Acct(int B_InterestExp_Acct)
        {
            Set_Value("B_InterestExp_Acct", B_InterestExp_Acct);
        }
        /** Get Bank Interest Expense.
        @return Bank Interest Expense Account */
        public int GetB_InterestExp_Acct()
        {
            Object ii = Get_Value("B_InterestExp_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Interest Revenue.
        @param B_InterestRev_Acct Bank Interest Revenue Account */
        public void SetB_InterestRev_Acct(int B_InterestRev_Acct)
        {
            Set_Value("B_InterestRev_Acct", B_InterestRev_Acct);
        }
        /** Get Bank Interest Revenue.
        @return Bank Interest Revenue Account */
        public int GetB_InterestRev_Acct()
        {
            Object ii = Get_Value("B_InterestRev_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Selection.
        @param B_PaymentSelect_Acct AP Payment Selection Clearing Account */
        public void SetB_PaymentSelect_Acct(int B_PaymentSelect_Acct)
        {
            Set_Value("B_PaymentSelect_Acct", B_PaymentSelect_Acct);
        }
        /** Get Payment Selection.
        @return AP Payment Selection Clearing Account */
        public int GetB_PaymentSelect_Acct()
        {
            Object ii = Get_Value("B_PaymentSelect_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Revaluation Gain.
        @param B_RevaluationGain_Acct Bank Revaluation Gain Account */
        public void SetB_RevaluationGain_Acct(int B_RevaluationGain_Acct)
        {
            Set_Value("B_RevaluationGain_Acct", B_RevaluationGain_Acct);
        }
        /** Get Bank Revaluation Gain.
        @return Bank Revaluation Gain Account */
        public int GetB_RevaluationGain_Acct()
        {
            Object ii = Get_Value("B_RevaluationGain_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Revaluation Loss.
        @param B_RevaluationLoss_Acct Bank Revaluation Loss Account */
        public void SetB_RevaluationLoss_Acct(int B_RevaluationLoss_Acct)
        {
            Set_Value("B_RevaluationLoss_Acct", B_RevaluationLoss_Acct);
        }
        /** Get Bank Revaluation Loss.
        @return Bank Revaluation Loss Account */
        public int GetB_RevaluationLoss_Acct()
        {
            Object ii = Get_Value("B_RevaluationLoss_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Settlement Gain.
        @param B_SettlementGain_Acct Bank Settlement Gain Account */
        public void SetB_SettlementGain_Acct(int B_SettlementGain_Acct)
        {
            Set_Value("B_SettlementGain_Acct", B_SettlementGain_Acct);
        }
        /** Get Bank Settlement Gain.
        @return Bank Settlement Gain Account */
        public int GetB_SettlementGain_Acct()
        {
            Object ii = Get_Value("B_SettlementGain_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Settlement Loss.
        @param B_SettlementLoss_Acct Bank Settlement Loss Account */
        public void SetB_SettlementLoss_Acct(int B_SettlementLoss_Acct)
        {
            Set_Value("B_SettlementLoss_Acct", B_SettlementLoss_Acct);
        }
        /** Get Bank Settlement Loss.
        @return Bank Settlement Loss Account */
        public int GetB_SettlementLoss_Acct()
        {
            Object ii = Get_Value("B_SettlementLoss_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unallocated Cash.
        @param B_UnallocatedCash_Acct Unallocated Cash Clearing Account */
        public void SetB_UnallocatedCash_Acct(int B_UnallocatedCash_Acct)
        {
            Set_Value("B_UnallocatedCash_Acct", B_UnallocatedCash_Acct);
        }
        /** Get Unallocated Cash.
        @return Unallocated Cash Clearing Account */
        public int GetB_UnallocatedCash_Acct()
        {
            Object ii = Get_Value("B_UnallocatedCash_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Bank Unidentified Receipts.
        @param B_Unidentified_Acct Bank Unidentified Receipts Account */
        public void SetB_Unidentified_Acct(int B_Unidentified_Acct)
        {
            Set_Value("B_Unidentified_Acct", B_Unidentified_Acct);
        }
        /** Get Bank Unidentified Receipts.
        @return Bank Unidentified Receipts Account */
        public int GetB_Unidentified_Acct()
        {
            Object ii = Get_Value("B_Unidentified_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Book Asset.
        @param CB_Asset_Acct Cash Book Asset Account */
        public void SetCB_Asset_Acct(int CB_Asset_Acct)
        {
            Set_Value("CB_Asset_Acct", CB_Asset_Acct);
        }
        /** Get Cash Book Asset.
        @return Cash Book Asset Account */
        public int GetCB_Asset_Acct()
        {
            Object ii = Get_Value("CB_Asset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Transfer.
        @param CB_CashTransfer_Acct Cash Transfer Clearing Account */
        public void SetCB_CashTransfer_Acct(int CB_CashTransfer_Acct)
        {
            Set_Value("CB_CashTransfer_Acct", CB_CashTransfer_Acct);
        }
        /** Get Cash Transfer.
        @return Cash Transfer Clearing Account */
        public int GetCB_CashTransfer_Acct()
        {
            Object ii = Get_Value("CB_CashTransfer_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Book Differences.
        @param CB_Differences_Acct Cash Book Differences Account */
        public void SetCB_Differences_Acct(int CB_Differences_Acct)
        {
            Set_Value("CB_Differences_Acct", CB_Differences_Acct);
        }
        /** Get Cash Book Differences.
        @return Cash Book Differences Account */
        public int GetCB_Differences_Acct()
        {
            Object ii = Get_Value("CB_Differences_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Book Expense.
        @param CB_Expense_Acct Cash Book Expense Account */
        public void SetCB_Expense_Acct(int CB_Expense_Acct)
        {
            Set_Value("CB_Expense_Acct", CB_Expense_Acct);
        }
        /** Get Cash Book Expense.
        @return Cash Book Expense Account */
        public int GetCB_Expense_Acct()
        {
            Object ii = Get_Value("CB_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cash Book Receipt.
        @param CB_Receipt_Acct Cash Book Receipts Account */
        public void SetCB_Receipt_Acct(int CB_Receipt_Acct)
        {
            Set_Value("CB_Receipt_Acct", CB_Receipt_Acct);
        }
        /** Get Cash Book Receipt.
        @return Cash Book Receipts Account */
        public int GetCB_Receipt_Acct()
        {
            Object ii = Get_Value("CB_Receipt_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Accounting Schema.
        @param C_AcctSchema_ID Rules for accounting */
        public void SetC_AcctSchema_ID(int C_AcctSchema_ID)
        {
            if (C_AcctSchema_ID < 1) throw new ArgumentException("C_AcctSchema_ID is mandatory.");
            Set_ValueNoCheck("C_AcctSchema_ID", C_AcctSchema_ID);
        }
        /** Get Accounting Schema.
        @return Rules for accounting */
        public int GetC_AcctSchema_ID()
        {
            Object ii = Get_Value("C_AcctSchema_ID");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Get Record ID/ColumnName
        @return ID/ColumnName pair */
        public KeyNamePair GetKeyNamePair()
        {
            return new KeyNamePair(Get_ID(), GetC_AcctSchema_ID().ToString());
        }
        /** Set Customer Prepayment.
        @param C_Prepayment_Acct Account for customer prepayments */
        public void SetC_Prepayment_Acct(int C_Prepayment_Acct)
        {
            Set_Value("C_Prepayment_Acct", C_Prepayment_Acct);
        }
        /** Get Customer Prepayment.
        @return Account for customer prepayments */
        public int GetC_Prepayment_Acct()
        {
            Object ii = Get_Value("C_Prepayment_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Customer Receivables.
        @param C_Receivable_Acct Account for Customer Receivables */
        public void SetC_Receivable_Acct(int C_Receivable_Acct)
        {
            Set_Value("C_Receivable_Acct", C_Receivable_Acct);
        }
        /** Get Customer Receivables.
        @return Account for Customer Receivables */
        public int GetC_Receivable_Acct()
        {
            Object ii = Get_Value("C_Receivable_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Receivable Services.
        @param C_Receivable_Services_Acct Customer Accounts Receivables Services Account */
        public void SetC_Receivable_Services_Acct(int C_Receivable_Services_Acct)
        {
            Set_Value("C_Receivable_Services_Acct", C_Receivable_Services_Acct);
        }
        /** Get Receivable Services.
        @return Customer Accounts Receivables Services Account */
        public int GetC_Receivable_Services_Acct()
        {
            Object ii = Get_Value("C_Receivable_Services_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge Expense.
        @param Ch_Expense_Acct Charge Expense Account */
        public void SetCh_Expense_Acct(int Ch_Expense_Acct)
        {
            Set_Value("Ch_Expense_Acct", Ch_Expense_Acct);
        }
        /** Get Charge Expense.
        @return Charge Expense Account */
        public int GetCh_Expense_Acct()
        {
            Object ii = Get_Value("Ch_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Charge Revenue.
        @param Ch_Revenue_Acct Charge Revenue Account */
        public void SetCh_Revenue_Acct(int Ch_Revenue_Acct)
        {
            Set_Value("Ch_Revenue_Acct", Ch_Revenue_Acct);
        }
        /** Get Charge Revenue.
        @return Charge Revenue Account */
        public int GetCh_Revenue_Acct()
        {
            Object ii = Get_Value("Ch_Revenue_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Employee Expense.
        @param E_Expense_Acct Account for Employee Expenses */
        public void SetE_Expense_Acct(int E_Expense_Acct)
        {
            Set_Value("E_Expense_Acct", E_Expense_Acct);
        }
        /** Get Employee Expense.
        @return Account for Employee Expenses */
        public int GetE_Expense_Acct()
        {
            Object ii = Get_Value("E_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Employee Prepayment.
        @param E_Prepayment_Acct Account for Employee Expense Prepayments */
        public void SetE_Prepayment_Acct(int E_Prepayment_Acct)
        {
            Set_Value("E_Prepayment_Acct", E_Prepayment_Acct);
        }
        /** Get Employee Prepayment.
        @return Account for Employee Expense Prepayments */
        public int GetE_Prepayment_Acct()
        {
            Object ii = Get_Value("E_Prepayment_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Not-invoiced Receipts.
        @param NotInvoicedReceipts_Acct Account for not-invoiced Material Receipts */
        public void SetNotInvoicedReceipts_Acct(int NotInvoicedReceipts_Acct)
        {
            Set_Value("NotInvoicedReceipts_Acct", NotInvoicedReceipts_Acct);
        }
        /** Get Not-invoiced Receipts.
        @return Account for not-invoiced Material Receipts */
        public int GetNotInvoicedReceipts_Acct()
        {
            Object ii = Get_Value("NotInvoicedReceipts_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Not-invoiced Receivables.
        @param NotInvoicedReceivables_Acct Account for not invoiced Receivables */
        public void SetNotInvoicedReceivables_Acct(int NotInvoicedReceivables_Acct)
        {
            Set_Value("NotInvoicedReceivables_Acct", NotInvoicedReceivables_Acct);
        }
        /** Get Not-invoiced Receivables.
        @return Account for not invoiced Receivables */
        public int GetNotInvoicedReceivables_Acct()
        {
            Object ii = Get_Value("NotInvoicedReceivables_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Not-invoiced Revenue.
        @param NotInvoicedRevenue_Acct Account for not invoiced Revenue */
        public void SetNotInvoicedRevenue_Acct(int NotInvoicedRevenue_Acct)
        {
            Set_Value("NotInvoicedRevenue_Acct", NotInvoicedRevenue_Acct);
        }
        /** Get Not-invoiced Revenue.
        @return Account for not invoiced Revenue */
        public int GetNotInvoicedRevenue_Acct()
        {
            Object ii = Get_Value("NotInvoicedRevenue_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Overhead Absorption.
        @param Overhead_Absorption_Acct Overhead Absorption Account */
        public void SetOverhead_Absorption_Acct(int Overhead_Absorption_Acct)
        {
            Set_Value("Overhead_Absorption_Acct", Overhead_Absorption_Acct);
        }
        /** Get Overhead Absorption.
        @return Overhead Absorption Account */
        public int GetOverhead_Absorption_Acct()
        {
            Object ii = Get_Value("Overhead_Absorption_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Project Asset.
        @param PJ_Asset_Acct Project Asset Account */
        public void SetPJ_Asset_Acct(int PJ_Asset_Acct)
        {
            Set_Value("PJ_Asset_Acct", PJ_Asset_Acct);
        }
        /** Get Project Asset.
        @return Project Asset Account */
        public int GetPJ_Asset_Acct()
        {
            Object ii = Get_Value("PJ_Asset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work In Progress.
        @param PJ_WIP_Acct Account for Work in Progress */
        public void SetPJ_WIP_Acct(int PJ_WIP_Acct)
        {
            Set_Value("PJ_WIP_Acct", PJ_WIP_Acct);
        }
        /** Get Work In Progress.
        @return Account for Work in Progress */
        public int GetPJ_WIP_Acct()
        {
            Object ii = Get_Value("PJ_WIP_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product Asset.
        @param P_Asset_Acct Account for Product Asset (Inventory) */
        public void SetP_Asset_Acct(int P_Asset_Acct)
        {
            Set_Value("P_Asset_Acct", P_Asset_Acct);
        }
        /** Get Product Asset.
        @return Account for Product Asset (Inventory) */
        public int GetP_Asset_Acct()
        {
            Object ii = Get_Value("P_Asset_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product COGS.
        @param P_COGS_Acct Account for Cost of Goods Sold */
        public void SetP_COGS_Acct(int P_COGS_Acct)
        {
            Set_Value("P_COGS_Acct", P_COGS_Acct);
        }
        /** Get Product COGS.
        @return Account for Cost of Goods Sold */
        public int GetP_COGS_Acct()
        {
            Object ii = Get_Value("P_COGS_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Cost Adjustment.
        @param P_CostAdjustment_Acct Product Cost Adjustment Account */
        public void SetP_CostAdjustment_Acct(int P_CostAdjustment_Acct)
        {
            Set_Value("P_CostAdjustment_Acct", P_CostAdjustment_Acct);
        }
        /** Get Cost Adjustment.
        @return Product Cost Adjustment Account */
        public int GetP_CostAdjustment_Acct()
        {
            Object ii = Get_Value("P_CostAdjustment_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product Expense.
        @param P_Expense_Acct Account for Product Expense */
        public void SetP_Expense_Acct(int P_Expense_Acct)
        {
            Set_Value("P_Expense_Acct", P_Expense_Acct);
        }
        /** Get Product Expense.
        @return Account for Product Expense */
        public int GetP_Expense_Acct()
        {
            Object ii = Get_Value("P_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Clearing.
        @param P_InventoryClearing_Acct Product Inventory Clearing Account */
        public void SetP_InventoryClearing_Acct(int P_InventoryClearing_Acct)
        {
            Set_Value("P_InventoryClearing_Acct", P_InventoryClearing_Acct);
        }
        /** Get Inventory Clearing.
        @return Product Inventory Clearing Account */
        public int GetP_InventoryClearing_Acct()
        {
            Object ii = Get_Value("P_InventoryClearing_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Invoice Price Variance.
        @param P_InvoicePriceVariance_Acct Difference between Costs and Invoice Price (IPV) */
        public void SetP_InvoicePriceVariance_Acct(int P_InvoicePriceVariance_Acct)
        {
            Set_Value("P_InvoicePriceVariance_Acct", P_InvoicePriceVariance_Acct);
        }
        /** Get Invoice Price Variance.
        @return Difference between Costs and Invoice Price (IPV) */
        public int GetP_InvoicePriceVariance_Acct()
        {
            Object ii = Get_Value("P_InvoicePriceVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Material Overhead.
        @param P_MaterialOverhd_Acct Material Overhead Account */
        public void SetP_MaterialOverhd_Acct(int P_MaterialOverhd_Acct)
        {
            Set_Value("P_MaterialOverhd_Acct", P_MaterialOverhd_Acct);
        }
        /** Get Material Overhead.
        @return Material Overhead Account */
        public int GetP_MaterialOverhd_Acct()
        {
            Object ii = Get_Value("P_MaterialOverhd_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Purchase Price Variance.
        @param P_PurchasePriceVariance_Acct Difference between Standard Cost and Purchase Price (PPV) */
        public void SetP_PurchasePriceVariance_Acct(int P_PurchasePriceVariance_Acct)
        {
            Set_Value("P_PurchasePriceVariance_Acct", P_PurchasePriceVariance_Acct);
        }
        /** Get Purchase Price Variance.
        @return Difference between Standard Cost and Purchase Price (PPV) */
        public int GetP_PurchasePriceVariance_Acct()
        {
            Object ii = Get_Value("P_PurchasePriceVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Resource Absorption.
        @param P_Resource_Absorption_Acct Resource Absorption Account */
        public void SetP_Resource_Absorption_Acct(int P_Resource_Absorption_Acct)
        {
            Set_Value("P_Resource_Absorption_Acct", P_Resource_Absorption_Acct);
        }
        /** Get Resource Absorption.
        @return Resource Absorption Account */
        public int GetP_Resource_Absorption_Acct()
        {
            Object ii = Get_Value("P_Resource_Absorption_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Product Revenue.
        @param P_Revenue_Acct Account for Product Revenue (Sales Account) */
        public void SetP_Revenue_Acct(int P_Revenue_Acct)
        {
            Set_Value("P_Revenue_Acct", P_Revenue_Acct);
        }
        /** Get Product Revenue.
        @return Account for Product Revenue (Sales Account) */
        public int GetP_Revenue_Acct()
        {
            Object ii = Get_Value("P_Revenue_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Trade Discount Granted.
        @param P_TradeDiscountGrant_Acct Trade Discount Granted Account */
        public void SetP_TradeDiscountGrant_Acct(int P_TradeDiscountGrant_Acct)
        {
            Set_Value("P_TradeDiscountGrant_Acct", P_TradeDiscountGrant_Acct);
        }
        /** Get Trade Discount Granted.
        @return Trade Discount Granted Account */
        public int GetP_TradeDiscountGrant_Acct()
        {
            Object ii = Get_Value("P_TradeDiscountGrant_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Trade Discount Received.
        @param P_TradeDiscountRec_Acct Trade Discount Receivable Account */
        public void SetP_TradeDiscountRec_Acct(int P_TradeDiscountRec_Acct)
        {
            Set_Value("P_TradeDiscountRec_Acct", P_TradeDiscountRec_Acct);
        }
        /** Get Trade Discount Received.
        @return Trade Discount Receivable Account */
        public int GetP_TradeDiscountRec_Acct()
        {
            Object ii = Get_Value("P_TradeDiscountRec_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Discount Expense.
        @param PayDiscount_Exp_Acct Payment Discount Expense Account */
        public void SetPayDiscount_Exp_Acct(int PayDiscount_Exp_Acct)
        {
            Set_Value("PayDiscount_Exp_Acct", PayDiscount_Exp_Acct);
        }
        /** Get Payment Discount Expense.
        @return Payment Discount Expense Account */
        public int GetPayDiscount_Exp_Acct()
        {
            Object ii = Get_Value("PayDiscount_Exp_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Payment Discount Revenue.
        @param PayDiscount_Rev_Acct Payment Discount Revenue Account */
        public void SetPayDiscount_Rev_Acct(int PayDiscount_Rev_Acct)
        {
            Set_Value("PayDiscount_Rev_Acct", PayDiscount_Rev_Acct);
        }
        /** Get Payment Discount Revenue.
        @return Payment Discount Revenue Account */
        public int GetPayDiscount_Rev_Acct()
        {
            Object ii = Get_Value("PayDiscount_Rev_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
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
        /** Set Realized Gain Acct.
        @param RealizedGain_Acct Realized Gain Account */
        public void SetRealizedGain_Acct(int RealizedGain_Acct)
        {
            Set_Value("RealizedGain_Acct", RealizedGain_Acct);
        }
        /** Get Realized Gain Acct.
        @return Realized Gain Account */
        public int GetRealizedGain_Acct()
        {
            Object ii = Get_Value("RealizedGain_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Realized Loss Acct.
        @param RealizedLoss_Acct Realized Loss Account */
        public void SetRealizedLoss_Acct(int RealizedLoss_Acct)
        {
            Set_Value("RealizedLoss_Acct", RealizedLoss_Acct);
        }
        /** Get Realized Loss Acct.
        @return Realized Loss Account */
        public int GetRealizedLoss_Acct()
        {
            Object ii = Get_Value("RealizedLoss_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Credit.
        @param T_Credit_Acct Account for Tax you can reclaim */
        public void SetT_Credit_Acct(int T_Credit_Acct)
        {
            Set_Value("T_Credit_Acct", T_Credit_Acct);
        }
        /** Get Tax Credit.
        @return Account for Tax you can reclaim */
        public int GetT_Credit_Acct()
        {
            Object ii = Get_Value("T_Credit_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Due.
        @param T_Due_Acct Account for Tax you have to pay */
        public void SetT_Due_Acct(int T_Due_Acct)
        {
            Set_Value("T_Due_Acct", T_Due_Acct);
        }
        /** Get Tax Due.
        @return Account for Tax you have to pay */
        public int GetT_Due_Acct()
        {
            Object ii = Get_Value("T_Due_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Expense.
        @param T_Expense_Acct Account for paid tax you cannot reclaim */
        public void SetT_Expense_Acct(int T_Expense_Acct)
        {
            Set_Value("T_Expense_Acct", T_Expense_Acct);
        }
        /** Get Tax Expense.
        @return Account for paid tax you cannot reclaim */
        public int GetT_Expense_Acct()
        {
            Object ii = Get_Value("T_Expense_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Liability.
        @param T_Liability_Acct Account for Tax declaration liability */
        public void SetT_Liability_Acct(int T_Liability_Acct)
        {
            Set_Value("T_Liability_Acct", T_Liability_Acct);
        }
        /** Get Tax Liability.
        @return Account for Tax declaration liability */
        public int GetT_Liability_Acct()
        {
            Object ii = Get_Value("T_Liability_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Tax Receivables.
        @param T_Receivables_Acct Account for Tax credit after tax declaration */
        public void SetT_Receivables_Acct(int T_Receivables_Acct)
        {
            Set_Value("T_Receivables_Acct", T_Receivables_Acct);
        }
        /** Get Tax Receivables.
        @return Account for Tax credit after tax declaration */
        public int GetT_Receivables_Acct()
        {
            Object ii = Get_Value("T_Receivables_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unearned Revenue.
        @param UnEarnedRevenue_Acct Account for unearned revenue */
        public void SetUnEarnedRevenue_Acct(int UnEarnedRevenue_Acct)
        {
            Set_Value("UnEarnedRevenue_Acct", UnEarnedRevenue_Acct);
        }
        /** Get Unearned Revenue.
        @return Account for unearned revenue */
        public int GetUnEarnedRevenue_Acct()
        {
            Object ii = Get_Value("UnEarnedRevenue_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unrealized Gain Acct.
        @param UnrealizedGain_Acct Unrealized Gain Account for currency revaluation */
        public void SetUnrealizedGain_Acct(int UnrealizedGain_Acct)
        {
            Set_Value("UnrealizedGain_Acct", UnrealizedGain_Acct);
        }
        /** Get Unrealized Gain Acct.
        @return Unrealized Gain Account for currency revaluation */
        public int GetUnrealizedGain_Acct()
        {
            Object ii = Get_Value("UnrealizedGain_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Unrealized Loss Acct.
        @param UnrealizedLoss_Acct Unrealized Loss Account for currency revaluation */
        public void SetUnrealizedLoss_Acct(int UnrealizedLoss_Acct)
        {
            Set_Value("UnrealizedLoss_Acct", UnrealizedLoss_Acct);
        }
        /** Get Unrealized Loss Acct.
        @return Unrealized Loss Account for currency revaluation */
        public int GetUnrealizedLoss_Acct()
        {
            Object ii = Get_Value("UnrealizedLoss_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Vendor Liability.
        @param V_Liability_Acct Account for Vendor Liability */
        public void SetV_Liability_Acct(int V_Liability_Acct)
        {
            Set_Value("V_Liability_Acct", V_Liability_Acct);
        }
        /** Get Vendor Liability.
        @return Account for Vendor Liability */
        public int GetV_Liability_Acct()
        {
            Object ii = Get_Value("V_Liability_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Vendor Service Liability.
        @param V_Liability_Services_Acct Account for Vender Service Liability */
        public void SetV_Liability_Services_Acct(int V_Liability_Services_Acct)
        {
            Set_Value("V_Liability_Services_Acct", V_Liability_Services_Acct);
        }
        /** Get Vendor Service Liability.
        @return Account for Vender Service Liability */
        public int GetV_Liability_Services_Acct()
        {
            Object ii = Get_Value("V_Liability_Services_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Vendor Prepayment.
        @param V_Prepayment_Acct Account for Vendor Prepayments */
        public void SetV_Prepayment_Acct(int V_Prepayment_Acct)
        {
            Set_Value("V_Prepayment_Acct", V_Prepayment_Acct);
        }
        /** Get Vendor Prepayment.
        @return Account for Vendor Prepayments */
        public int GetV_Prepayment_Acct()
        {
            Object ii = Get_Value("V_Prepayment_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Center Overhead.
        @param WC_Overhead_Acct Work Center Overhead Account */
        public void SetWC_Overhead_Acct(int WC_Overhead_Acct)
        {
            Set_Value("WC_Overhead_Acct", WC_Overhead_Acct);
        }
        /** Get Work Center Overhead.
        @return Work Center Overhead Account */
        public int GetWC_Overhead_Acct()
        {
            Object ii = Get_Value("WC_Overhead_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Material Overhead Variance.
        @param WO_MaterialOverhdVariance_Acct Work Order Material Overhead Variance Account */
        public void SetWO_MaterialOverhdVariance_Acct(int WO_MaterialOverhdVariance_Acct)
        {
            Set_Value("WO_MaterialOverhdVariance_Acct", WO_MaterialOverhdVariance_Acct);
        }
        /** Get Work Order Material Overhead Variance.
        @return Work Order Material Overhead Variance Account */
        public int GetWO_MaterialOverhdVariance_Acct()
        {
            Object ii = Get_Value("WO_MaterialOverhdVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Material Overhead.
        @param WO_MaterialOverhd_Acct Work Order Material Overhead Account */
        public void SetWO_MaterialOverhd_Acct(int WO_MaterialOverhd_Acct)
        {
            Set_Value("WO_MaterialOverhd_Acct", WO_MaterialOverhd_Acct);
        }
        /** Get Work Order Material Overhead.
        @return Work Order Material Overhead Account */
        public int GetWO_MaterialOverhd_Acct()
        {
            Object ii = Get_Value("WO_MaterialOverhd_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Material Variance.
        @param WO_MaterialVariance_Acct Work Order Material Variance Account */
        public void SetWO_MaterialVariance_Acct(int WO_MaterialVariance_Acct)
        {
            Set_Value("WO_MaterialVariance_Acct", WO_MaterialVariance_Acct);
        }
        /** Get Work Order Material Variance.
        @return Work Order Material Variance Account */
        public int GetWO_MaterialVariance_Acct()
        {
            Object ii = Get_Value("WO_MaterialVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Material.
        @param WO_Material_Acct Work Order Material Account */
        public void SetWO_Material_Acct(int WO_Material_Acct)
        {
            Set_Value("WO_Material_Acct", WO_Material_Acct);
        }
        /** Get Work Order Material.
        @return Work Order Material Account */
        public int GetWO_Material_Acct()
        {
            Object ii = Get_Value("WO_Material_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Outside Processing Variance.
        @param WO_OSPVariance_Acct Work Order Outside Processing Variance Account */
        public void SetWO_OSPVariance_Acct(int WO_OSPVariance_Acct)
        {
            Set_Value("WO_OSPVariance_Acct", WO_OSPVariance_Acct);
        }
        /** Get Work Order Outside Processing Variance.
        @return Work Order Outside Processing Variance Account */
        public int GetWO_OSPVariance_Acct()
        {
            Object ii = Get_Value("WO_OSPVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Outside Processing.
        @param WO_OSP_Acct Work Order Outside Processing Account */
        public void SetWO_OSP_Acct(int WO_OSP_Acct)
        {
            Set_Value("WO_OSP_Acct", WO_OSP_Acct);
        }
        /** Get Work Order Outside Processing.
        @return Work Order Outside Processing Account */
        public int GetWO_OSP_Acct()
        {
            Object ii = Get_Value("WO_OSP_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Overhead Variance.
        @param WO_OverhdVariance_Acct Work Order Overhead Variance Account */
        public void SetWO_OverhdVariance_Acct(int WO_OverhdVariance_Acct)
        {
            Set_Value("WO_OverhdVariance_Acct", WO_OverhdVariance_Acct);
        }
        /** Get Work Order Overhead Variance.
        @return Work Order Overhead Variance Account */
        public int GetWO_OverhdVariance_Acct()
        {
            Object ii = Get_Value("WO_OverhdVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Resource Variance.
        @param WO_ResourceVariance_Acct Work Order Resource Variance Account */
        public void SetWO_ResourceVariance_Acct(int WO_ResourceVariance_Acct)
        {
            Set_Value("WO_ResourceVariance_Acct", WO_ResourceVariance_Acct);
        }
        /** Get Work Order Resource Variance.
        @return Work Order Resource Variance Account */
        public int GetWO_ResourceVariance_Acct()
        {
            Object ii = Get_Value("WO_ResourceVariance_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Resource.
        @param WO_Resource_Acct Work Order Resource Account */
        public void SetWO_Resource_Acct(int WO_Resource_Acct)
        {
            Set_Value("WO_Resource_Acct", WO_Resource_Acct);
        }
        /** Get Work Order Resource.
        @return Work Order Resource Account */
        public int GetWO_Resource_Acct()
        {
            Object ii = Get_Value("WO_Resource_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Work Order Scrap.
        @param WO_Scrap_Acct Work Order Scrap Account */
        public void SetWO_Scrap_Acct(int WO_Scrap_Acct)
        {
            Set_Value("WO_Scrap_Acct", WO_Scrap_Acct);
        }
        /** Get Work Order Scrap.
        @return Work Order Scrap Account */
        public int GetWO_Scrap_Acct()
        {
            Object ii = Get_Value("WO_Scrap_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Warehouse Differences.
        @param W_Differences_Acct Warehouse Differences Account */
        public void SetW_Differences_Acct(int W_Differences_Acct)
        {
            Set_Value("W_Differences_Acct", W_Differences_Acct);
        }
        /** Get Warehouse Differences.
        @return Warehouse Differences Account */
        public int GetW_Differences_Acct()
        {
            Object ii = Get_Value("W_Differences_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Adjustment.
        @param W_InvActualAdjust_Acct Account for Inventory value adjustments for Actual Costing */
        public void SetW_InvActualAdjust_Acct(int W_InvActualAdjust_Acct)
        {
            Set_Value("W_InvActualAdjust_Acct", W_InvActualAdjust_Acct);
        }
        /** Get Inventory Adjustment.
        @return Account for Inventory value adjustments for Actual Costing */
        public int GetW_InvActualAdjust_Acct()
        {
            Object ii = Get_Value("W_InvActualAdjust_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set (Not Used).
        @param W_Inventory_Acct Warehouse Inventory Asset Account - Currently not used */
        public void SetW_Inventory_Acct(int W_Inventory_Acct)
        {
            Set_Value("W_Inventory_Acct", W_Inventory_Acct);
        }
        /** Get (Not Used).
        @return Warehouse Inventory Asset Account - Currently not used */
        public int GetW_Inventory_Acct()
        {
            Object ii = Get_Value("W_Inventory_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Inventory Revaluation.
        @param W_Revaluation_Acct Account for Inventory Revaluation */
        public void SetW_Revaluation_Acct(int W_Revaluation_Acct)
        {
            Set_Value("W_Revaluation_Acct", W_Revaluation_Acct);
        }
        /** Get Inventory Revaluation.
        @return Account for Inventory Revaluation */
        public int GetW_Revaluation_Acct()
        {
            Object ii = Get_Value("W_Revaluation_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Withholding.
        @param Withholding_Acct Account for Withholdings */
        public void SetWithholding_Acct(int Withholding_Acct)
        {
            Set_Value("Withholding_Acct", Withholding_Acct);
        }
        /** Get Withholding.
        @return Account for Withholdings */
        public int GetWithholding_Acct()
        {
            Object ii = Get_Value("Withholding_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
        /** Set Write-off.
        @param WriteOff_Acct Account for Receivables write-off */
        public void SetWriteOff_Acct(int WriteOff_Acct)
        {
            Set_Value("WriteOff_Acct", WriteOff_Acct);
        }
        /** Get Write-off.
        @return Account for Receivables write-off */
        public int GetWriteOff_Acct()
        {
            Object ii = Get_Value("WriteOff_Acct");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** Set Write-off.
       @param ASSET_DEPRECIATION_ACCT Account for Receivables write-off */
        public void SetASSET_DEPRECIATION_ACCT(int ASSET_DEPRECIATION_ACCT)
        {
            Set_Value("ASSET_DEPRECIATION_ACCT", ASSET_DEPRECIATION_ACCT);
        }
        /** Get Write-off.
        @return Account for Receivables ASSET_DEPRECIATION_ACCT */
        public int GetASSET_DEPRECIATION_ACCT()
        {
            Object ii = Get_Value("ASSET_DEPRECIATION_ACCT");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }


        /** Set Write-off.
      @param ASSET_DISP_REVENUE_ACCT Account for Receivables write-off */
        public void SetASSET_DISP_REVENUE_ACCT(int ASSET_DISP_REVENUE_ACCT)
        {
            Set_Value("ASSET_DISP_REVENUE_ACCT", ASSET_DISP_REVENUE_ACCT);
        }
        /** Get Write-off.
        @return Account for Receivables ASSET_DISP_REVENUE_ACCT */
        public int GetASSET_DISP_REVENUE_ACCT()
        {
            Object ii = Get_Value("ASSET_DISP_REVENUE_ACCT");
            if (ii == null) return 0;
            return Convert.ToInt32(ii);
        }
    }
}
