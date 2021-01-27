/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MInvoicePaySchedule
 * Purpose        : Invoice payment shedule calculations 
 * Class Used     : X_VAB_sched_InvoicePayment
 * Chronological    Development
 * Raghunandan     22-Jun-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
//using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MInvoicePaySchedule : X_VAB_sched_InvoicePayment
    {
        // Static Logger					
        private static VLogger _log = VLogger.GetVLogger(typeof(MInvoicePaySchedule).FullName);
        // 100								
        private static Decimal HUNDRED = 100.0M;
        /**	Parent						*/
        private MInvoice _parent = null;

        private decimal oldDueAmt = 0;

        /**
         * 	Get Payment Schedule of the invoice
         * 	@param Ctx context
         * 	@param VAB_Invoice_ID invoice id (direct)
         * 	@param VAB_sched_InvoicePayment_ID id (indirect)
         *	@param trxName transaction
         *	@return array of schedule
         */
        public static MInvoicePaySchedule[] GetInvoicePaySchedule(Ctx Ctx,
            int VAB_Invoice_ID, int VAB_sched_InvoicePayment_ID, Trx trxName)
        {
            String sql = "SELECT * FROM VAB_sched_InvoicePayment ips ";
            if (VAB_Invoice_ID != 0)
            {
                sql += "WHERE VAB_Invoice_ID=" + VAB_Invoice_ID;
            }
            else
            {
                sql += "WHERE EXISTS (SELECT * FROM VAB_sched_InvoicePayment xps"
                + " WHERE xps.VAB_sched_InvoicePayment_id=" + VAB_sched_InvoicePayment_ID + " AND ips.VAB_Invoice_ID=xps.VAB_Invoice_ID) ";
            }
            sql += "ORDER BY duedate";

            //
            List<MInvoicePaySchedule> list = new List<MInvoicePaySchedule>();
            DataTable dt = null;
            IDataReader idr = null;
            try
            {
                idr = DataBase.DB.ExecuteReader(sql, null, trxName);
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MInvoicePaySchedule(Ctx, dr, trxName));
                }
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                _log.Log(Level.SEVERE, "getInvoicePaySchedule", e);
            }
            finally
            {
                dt = null;
            }

            MInvoicePaySchedule[] retValue = new MInvoicePaySchedule[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /***
         * 	Standard Constructor
         *	@param Ctx context
         *	@param VAB_sched_InvoicePayment_ID id
         *	@param trxName transaction
         */
        public MInvoicePaySchedule(Ctx Ctx, int VAB_sched_InvoicePayment_ID, Trx trxName)
            : base(Ctx, VAB_sched_InvoicePayment_ID, trxName)
        {
            if (VAB_sched_InvoicePayment_ID == 0)
            {
                //	setVAB_Invoice_ID (0);
                //	setDiscountAmt (Env.ZERO);
                //	setDiscountDate (new Datetime(System.currentTimeMillis()));
                //	setDueAmt (Env.ZERO);
                //	setDueDate (new Datetime(System.currentTimeMillis()));
                SetIsValid(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param Ctx context
         *	@param dr result set
         *	@param trxName transaction
         */
        public MInvoicePaySchedule(Ctx Ctx, DataRow dr, Trx trxName)
            : base(Ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param invoice invoice
         *	@param paySchedule payment schedule
         */
        public MInvoicePaySchedule(MInvoice invoice, MPaySchedule paySchedule)
            : base(invoice.GetCtx(), 0, invoice.Get_TrxName())
        {

            _parent = invoice;
            SetClientOrg(invoice);
            SetVAB_Invoice_ID(invoice.GetVAB_Invoice_ID());
            SetVAB_PaymentSchedule_ID(paySchedule.GetVAB_PaymentSchedule_ID());

            //	Amounts
            int scale = MCurrency.GetStdPrecision(GetCtx(), invoice.GetVAB_Currency_ID());
            // distribute schedule based on GrandTotalAfterWithholding which is (GrandTotal – WithholdingAmount)
            Decimal due = (invoice.Get_ColumnIndex("GrandTotalAfterWithholding") > 0
                            && invoice.GetGrandTotalAfterWithholding() != 0 ? invoice.GetGrandTotalAfterWithholding() : invoice.GetGrandTotal());
            if (due.CompareTo(Env.ZERO) == 0)
            {
                SetDueAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetIsValid(false);
            }
            else
            {
                //due = due.multiply(paySchedule.getPercentage()).divide(HUNDRED, scale, Decimal.ROUND_HALF_UP);
                due = Decimal.Multiply(due, Decimal.Divide(paySchedule.GetPercentage(),
                    Decimal.Round(HUNDRED, scale, MidpointRounding.AwayFromZero)));
                SetDueAmt(due);
                Decimal discount = Decimal.Multiply(due, Decimal.Divide(paySchedule.GetDiscount(),
                    Decimal.Round(HUNDRED, scale, MidpointRounding.AwayFromZero)));
                SetDiscountAmt(discount);
                SetIsValid(true);
            }

            //	Dates		
            DateTime dueDate = TimeUtil.AddDays(invoice.GetDateInvoiced(), paySchedule.GetNetDays());
            SetDueDate(dueDate);
            DateTime discountDate = TimeUtil.AddDays(invoice.GetDateInvoiced(), paySchedule.GetDiscountDays());
            SetDiscountDate(discountDate);
        }



        /**
         * @return Returns the parent.
         */
        public MInvoice GetParent()
        {
            if (_parent == null)
                _parent = new MInvoice(GetCtx(), GetVAB_Invoice_ID(), Get_TrxName());
            return _parent;
        }

        /**
         * @param parent The parent to set.
         */
        public void SetParent(MInvoice parent)
        {
            _parent = parent;
        }

        /**
         * 	String Representation
         *	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MInvoicePaySchedule[");
            sb.Append(Get_ID()).Append("-Due=" + GetDueDate() + "/" + GetDueAmt())
                .Append(";Discount=").Append(GetDiscountDate() + "/" + GetDiscountAmt())
                .Append("]");
            return sb.ToString();
        }



        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override bool BeforeSave(bool newRecord)
        {
            if (Is_ValueChanged("DueAmt"))
            {
                log.Fine("beforeSave");
                SetIsValid(false);
            }
            oldDueAmt = Util.GetValueOfDecimal(Get_ValueOld("DueAmt"));

            if (Env.IsModuleInstalled("VA009_"))
            {
                // get invoice currency for rounding
                MCurrency currency = MCurrency.Get(GetCtx(), GetVAB_Currency_ID());
                SetDueAmt(Decimal.Round(GetDueAmt(), currency.GetStdPrecision()));
                SetVA009_PaidAmntInvce(Decimal.Round(GetVA009_PaidAmntInvce(), currency.GetStdPrecision()));

                // when invoice schedule have payment reference then need to check payment mode on payment window & update here
                if (GetVAB_Payment_ID() > 0)
                {
                    #region for payment
                    MPayment payment = new MPayment(GetCtx(), GetVAB_Payment_ID(), Get_Trx());
                    SetVA009_PaymentMethod_ID(payment.GetVA009_PaymentMethod_ID());

                    // get payment method detail -- update to here 
                    DataSet dsPaymentMethod = DB.ExecuteDataset(@"SELECT VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger FROM VA009_PaymentMethod
                                          WHERE VA009_PaymentMethod_ID = " + payment.GetVA009_PaymentMethod_ID(), null, Get_Trx());
                    if (dsPaymentMethod != null && dsPaymentMethod.Tables.Count > 0 && dsPaymentMethod.Tables[0].Rows.Count > 0)
                    {
                        if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"])))
                            SetVA009_PaymentMode(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"]));
                        if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                            SetVA009_PaymentType(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"]));
                        SetVA009_PaymentTrigger(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentTrigger"]));
                    }
                    #endregion
                }
                else if (GetVAB_CashJRNLLine_ID() > 0)
                {
                    #region For Cash
                    // when invoice schedule have cashline reference then need to check 
                    // payment mode of "Cash" type having currency is null on "Payment Method" window & update here
                    DataSet dsPaymentMethod = (DB.ExecuteDataset(@"SELECT VA009_PaymentMethod_ID, VA009_PaymentMode, VA009_PaymentType, VA009_PaymentTrigger 
                                       FROM VA009_PaymentMethod WHERE IsActive = 'Y' 
                                       AND VAF_Client_ID = " + GetVAF_Client_ID() + @" AND VA009_PaymentBaseType = 'B' AND NVL(VAB_Currency_ID , 0) = 0", null, Get_Trx()));
                    if (dsPaymentMethod != null && dsPaymentMethod.Tables.Count > 0 && dsPaymentMethod.Tables[0].Rows.Count > 0)
                    {
                        SetVA009_PaymentMethod_ID(Util.GetValueOfInt(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMethod_ID"]));
                        if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"])))
                            SetVA009_PaymentMode(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentMode"]));
                        if (!String.IsNullOrEmpty(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"])))
                            SetVA009_PaymentType(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentType"]));
                        SetVA009_PaymentTrigger(Util.GetValueOfString(dsPaymentMethod.Tables[0].Rows[0]["VA009_PaymentTrigger"]));
                    }
                    else
                    {
                        #region when we not found record of "Cash" then we will create a new rcord on Payment Method for Cash
                        string sql = @"SELECT VAF_TABLEVIEW_ID  FROM VAF_TABLEVIEW WHERE tablename LIKE 'VA009_PaymentMethod' AND IsActive = 'Y'";
                        int tableId = Util.GetValueOfInt(DB.ExecuteScalar(sql, null, null));
                        MTable tbl = new MTable(GetCtx(), tableId, Get_Trx());
                        PO po = tbl.GetPO(GetCtx(), 0, Get_Trx());
                        po.SetVAF_Client_ID(GetVAF_Client_ID());
                        po.SetVAF_Org_ID(0); // Recod will be created in (*) Organization
                        po.Set_Value("Value", "By Cash");
                        po.Set_Value("VA009_Name", "By Cash");
                        po.Set_Value("IsActive", true);
                        po.Set_Value("VA009_PaymentBaseType", "B");
                        po.Set_Value("VA009_PaymentRule", "M");
                        po.Set_Value("VA009_PaymentMode", "C");
                        po.Set_Value("VA009_PaymentType", "S");
                        po.Set_Value("VA009_PaymentTrigger", "S");
                        po.Set_Value("VAB_Currency_ID", null);
                        po.Set_Value("VA009_InitiatePay", false);
                        if (!po.Save(Get_Trx()))
                        {
                            ValueNamePair pp = VLogger.RetrieveError();
                            log.Info("Error Occured when try to save record on Payment Method for Cash. Error Type : " + pp.GetValue());
                        }
                        else
                        {
                            SetVA009_PaymentMethod_ID(Util.GetValueOfInt(po.Get_Value("VA009_PaymentMethod_ID")));
                            SetVA009_PaymentMode("C");
                            SetVA009_PaymentType("S");
                            SetVA009_PaymentTrigger("S");
                        }
                        #endregion
                    }
                    #endregion
                }
            }
            // to set processing false because in case of new schedule processing is set to be false
            if (newRecord)
            {
                SetProcessing(false);

                // when schedile is not paid and invoice hedaer having "Hold Payment", then set "Hold payment" on schedule also
                if (Get_ColumnIndex("IsHoldPayment") > 0 && (GetVAB_Payment_ID() == 0 && GetVAB_CashJRNLLine_ID() == 0))
                {
                    String sql = "SELECT IsHoldPayment FROM VAB_Invoice WHERE VAB_Invoice_ID = " + GetVAB_Invoice_ID();
                    String IsHoldPayment = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                    SetIsHoldPayment(IsHoldPayment.Equals("Y"));
                }
            }
            // if payment refrence not found on schedule the set withholdimh amount as ZERO
            if (GetVAB_Payment_ID() <= 0)
            {
                SetBackupWithholdingAmount(0);
                SetWithholdingAmt(0);
            }
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override bool AfterSave(bool newRecord, bool success)
        {
            if (newRecord || Is_ValueChanged("DueAmt"))
            {
                log.Fine("afterSave");
                GetParent();
                _parent.ValidatePaySchedule();
                _parent.Save();
            }
            if (Env.IsModuleInstalled("VA009_"))
            {
                if (!newRecord && oldDueAmt != GetDueAmt())
                {
                    log.SaveWarning("Message", Msg.GetMsg(GetCtx(), "VA009_ChangeOtherSchedule"));
                }
                if (GetDueAmt() <= Decimal.Add(GetVA009_PaidAmntInvce(), GetVA009_Variance()))
                {
                    DB.ExecuteQuery("UPDATE VAB_sched_InvoicePayment SET VA009_IsPaid = 'Y' WHERE VAB_sched_InvoicePayment_ID = " + GetVAB_sched_InvoicePayment_ID(), null, Get_Trx());
                    // if payment is done against invioce for advanced schedules then update payment reference at order schedule tab
                    if (GetVA009_OrderPaySchedule_ID() > 0 && GetVAB_Payment_ID() > 0)
                    {
                        DB.ExecuteQuery("Update VA009_OrderPaySchedule set VA009_ISPAID='Y', VAB_Payment_ID=" + GetVAB_Payment_ID() +
                                        @" where VA009_OrderPaySchedule_ID=" + GetVA009_OrderPaySchedule_ID(), null, Get_Trx());
                    }
                }
                else
                {
                    // Check if order schedule found at invoice schedule then it will not set ispaid to false
                    if (GetVA009_OrderPaySchedule_ID() == 0)
                    {
                        DB.ExecuteQuery("UPDATE VAB_sched_InvoicePayment SET VA009_IsPaid = 'N' WHERE VAB_sched_InvoicePayment_ID = " + GetVAB_sched_InvoicePayment_ID(), null, Get_Trx());
                    }
                    // if payment is done against invioce for advanced schedules then update payment reference at order schedule tab
                    else if (GetVA009_OrderPaySchedule_ID() != 0)
                    {
                        if (GetVAB_Payment_ID() > 0)
                        {
                            DB.ExecuteQuery("Update VA009_OrderPaySchedule set VA009_ISPAID='Y', VAB_Payment_ID=" + GetVAB_Payment_ID() + " where VA009_OrderPaySchedule_ID=" + GetVA009_OrderPaySchedule_ID(), null, Get_Trx());
                        }
                    }
                }
                DB.ExecuteQuery(@"UPDATE VAB_Invoice inv SET VA009_PaidAmount = 
                              (SELECT SUM(VA009_PaidAmntInvce) FROM VAB_sched_InvoicePayment isch WHERE isch.VAB_Invoice_ID = inv.VAB_Invoice_ID AND isch.IsActive = 'Y')
                             WHERE inv.IsActive = 'Y' AND inv.VAB_Invoice_ID = " + GetVAB_Invoice_ID(), null, Get_Trx());
            }
            return success;
        }

    }
}
