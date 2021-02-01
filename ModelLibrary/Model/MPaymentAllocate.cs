/********************************************************
 * Module Name    : 
 * Purpose        : 
 * Class Used     : X_VAB_PaymentAllotment
 * Chronological Development
 * Veena Pandey     24-June-2009
 ******************************************************/

using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Classes;
using VAdvantage.Utility;
using VAdvantage.DataBase;
using VAdvantage.Common;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaymentAllocate : X_VAB_PaymentAllotment
    {
        /**	Logger	*/
        private static VLogger _log = VLogger.GetVLogger(typeof(MPaymentAllocate).FullName);
        /**	The Invoice				*/
        private MInvoice _invoice = null;

        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAB_PaymentAllotment_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MPaymentAllocate(Ctx ctx, int VAB_PaymentAllotment_ID, Trx trxName)
            : base(ctx, VAB_PaymentAllotment_ID, trxName)
        {
            if (VAB_PaymentAllotment_ID == 0)
            {
                //	SetVAB_Payment_ID (0);	//	Parent
                //	SetVAB_Invoice_ID (0);
                SetAmount(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetOverUnderAmt(Env.ZERO);
                SetWriteOffAmt(Env.ZERO);
                SetInvoiceAmt(Env.ZERO);
            }
        }

        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">data row</param>
        /// <param name="trxName">transaction</param>
        public MPaymentAllocate(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
        }

        /**
	     * 	Get active Payment Allocation of Payment
	     *	@param parent payment
	     *	@return array of allocations
	     */
        public static MPaymentAllocate[] Get(MPayment parent)
        {
            List<MPaymentAllocate> list = new List<MPaymentAllocate>();
            String sql = "SELECT * FROM VAB_PaymentAllotment WHERE VAB_Payment_ID=" + parent.GetVAB_Payment_ID() + " AND IsActive='Y'";
            try
            {
                //DataSet ds = DataBase.DB.ExecuteDataset(sql, null, null);
                DataSet ds = DataBase.DB.ExecuteDataset(sql, null, parent.Get_TrxName()); //Arpit..Passes transction to get ALL Allocations at transction Level
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        list.Add(new MPaymentAllocate(parent.GetCtx(), dr, parent.Get_TrxName()));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Log(Level.SEVERE, sql, e);
            }

            MPaymentAllocate[] retValue = new MPaymentAllocate[list.Count];
            retValue = list.ToArray();
            return retValue;
        }

        /**
	     * 	Set VAB_Invoice_ID
	     *	@param VAB_Invoice_ID id
	     */
        public new void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            base.SetVAB_Invoice_ID(VAB_Invoice_ID);
            _invoice = null;
        }

        /**
         * 	Get Invoice
         *	@return invoice
         */
        public MInvoice GetInvoice()
        {
            if (_invoice == null && GetVAB_Invoice_ID() != 0)
                _invoice = new MInvoice(GetCtx(), GetVAB_Invoice_ID(), Get_TrxName());
            return _invoice;
        }

        /**
         * 	Get BPartner of Invoice
         *	@return bp
         */
        public int GetVAB_BusinessPartner_ID()
        {
            if (_invoice == null)
                GetInvoice();
            if (_invoice == null)
                return 0;
            return _invoice.GetVAB_BusinessPartner_ID();
        }

        /**
         * 	Set Invoice - Callout
         *	@param oldVAB_Invoice_ID old BP
         *	@param newVAB_Invoice_ID new BP
         *	@param windowNo window no
         */
        //@UICallout
        public void SetVAB_Invoice_ID(String oldVAB_Invoice_ID, String newVAB_Invoice_ID, int windowNo)
        {
            if (newVAB_Invoice_ID == null || newVAB_Invoice_ID.Length == 0)
                return;
            int VAB_Invoice_ID = int.Parse(newVAB_Invoice_ID);
            SetVAB_Invoice_ID(VAB_Invoice_ID);
            if (VAB_Invoice_ID == 0)
                return;
            //	Check Payment
            int VAB_Payment_ID = GetVAB_Payment_ID();
            MPayment payment = new MPayment(GetCtx(), VAB_Payment_ID, null);
            if (payment.GetVAB_Charge_ID() != 0
                || payment.GetVAB_Invoice_ID() != 0
                || payment.GetVAB_Order_ID() != 0)
            {
                //p_changeVO.addError( Msg.GetMsg(GetCtx(),"PaymentIsAllocated"));
                return;
            }

            SetDiscountAmt(Env.ZERO);
            SetWriteOffAmt(Env.ZERO);
            SetOverUnderAmt(Env.ZERO);

            int VAB_sched_InvoicePayment_ID = 0;
            if (GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_Invoice_ID") == VAB_Invoice_ID
                && GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_sched_InvoicePayment_ID") != 0)
                VAB_sched_InvoicePayment_ID = GetCtx().GetContextAsInt(Env.WINDOW_INFO, Env.TAB_INFO, "VAB_sched_InvoicePayment_ID");

            //  Payment Date
            DateTime ts = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime(windowNo, "DateTrx"));
            if (ts == null)
                ts = DateTime.Now;
            //
            String sql = "SELECT VAB_BusinessPartner_ID,VAB_Currency_ID,"		        //	1..2
                + " invoiceOpen(VAB_Invoice_ID, @paysch),"					//	3		#1
                + " invoiceDiscount(VAB_Invoice_ID,@tsdt,@paysch1), IsSOTrx "	//	4..5	#2/3
                + "FROM VAB_Invoice WHERE VAB_Invoice_ID=@invid";			    //			#4
            int VAB_Currency_ID = 0;		//	Invoice Currency
            IDataReader idr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[4];
                param[0] = new SqlParameter("@paysch", VAB_sched_InvoicePayment_ID);
                param[1] = new SqlParameter("@tsdt", (DateTime?)ts);
                param[2] = new SqlParameter("@paysch1", VAB_sched_InvoicePayment_ID);
                param[3] = new SqlParameter("@invid", VAB_Invoice_ID);

                idr = DataBase.DB.ExecuteReader(sql, null, null);
                if (idr.Read())
                {
                    //	SetVAB_BusinessPartner_ID(rs.GetInt(1));
                    VAB_Currency_ID = Utility.Util.GetValueOfInt(idr[1].ToString());	//	Set Invoice Currency
                    //	SetVAB_Currency_ID(VAB_Currency_ID);
                    //
                    Decimal? invoiceOpen = null;
                    if (!idr.IsDBNull(2))
                        invoiceOpen = Utility.Util.GetValueOfDecimal(idr[2]);	//	Set Invoice Open Amount
                    if (invoiceOpen == null)
                        invoiceOpen = Env.ZERO;
                    Decimal? discountAmt = null;
                    if (!idr.IsDBNull(3))
                        discountAmt = Utility.Util.GetValueOfDecimal(idr[3]);	//	Set Discount Amt
                    if (discountAmt == null)
                        discountAmt = Env.ZERO;
                    //
                    SetInvoiceAmt((Decimal)invoiceOpen);
                    SetAmount(Decimal.Subtract((Decimal)invoiceOpen, (Decimal)discountAmt));
                    SetDiscountAmt(Convert.ToDecimal(discountAmt));
                    //  reSet as dependent fields Get reSet
                    GetCtx().SetContext(windowNo, "VAB_Invoice_ID", VAB_Invoice_ID);
                    //IsSOTrx, Project
                }
                idr.Close();
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            //	Check Invoice/Payment Currency - may not be an issue(??)
            if (VAB_Currency_ID != 0)
            {
                int currency_ID = GetCtx().GetContextAsInt(windowNo, "VAB_Currency_ID");
                if (currency_ID != VAB_Currency_ID)
                {
                    String msg = Msg.ParseTranslation(GetCtx(), "@VAB_Currency_ID@: @VAB_Invoice_ID@ <> @VAB_Payment_ID@");
                    //p_changeVO.addError(msg);
                }
            }
        }


        /**
         * 	Set Allocation Amt - Callout
         *	@param oldAmount old value
         *	@param newAmount new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetAmount(String oldAmount, String newAmount, int windowNo)
        {
            if (newAmount == null || newAmount.Length == 0)
                return;
            Decimal amount = (Decimal)PO.ConvertToBigDecimal(newAmount);
            SetAmount(amount);
            CheckAmt(windowNo, "PayAmt");
        }

        /**
         * 	Set Discount - Callout
         *	@param oldDiscountAmt old value
         *	@param newDiscountAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetDiscountAmt(String oldDiscountAmt, String newDiscountAmt, int windowNo)
        {
            if (newDiscountAmt == null || newDiscountAmt.Length == 0)
                return;
            Decimal discountAmt = (Decimal)PO.ConvertToBigDecimal(newDiscountAmt);
            SetDiscountAmt(discountAmt);
            CheckAmt(windowNo, "DiscountAmt");
        }

        /**
         * 	Set Over Under Amt - Callout
         *	@param oldOverUnderAmt old value
         *	@param newOverUnderAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout
        public void SetOverUnderAmt(String oldOverUnderAmt, String newOverUnderAmt, int windowNo)
        {
            if (newOverUnderAmt == null || newOverUnderAmt.Length == 0)
                return;
            Decimal overUnderAmt = (Decimal)PO.ConvertToBigDecimal(newOverUnderAmt);
            SetOverUnderAmt(overUnderAmt);
            CheckAmt(windowNo, "OverUnderAmt");
        }

        /**
         * 	Set WriteOff Amt - Callout
         *	@param oldWriteOffAmt old value
         *	@param newWriteOffAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        ////@UICallout
        public void SetWriteOffAmt(String oldWriteOffAmt, String newWriteOffAmt, int windowNo)
        {
            if (newWriteOffAmt == null || newWriteOffAmt.Length == 0)
                return;
            Decimal writeOffAmt = (Decimal)PO.ConvertToBigDecimal(newWriteOffAmt);
            SetWriteOffAmt(writeOffAmt);
            CheckAmt(windowNo, "WriteOffAmt");
        }

        /**
         * 	Check amount (Callout)
         *	@param windowNo window
         *	@param columnName columnName
         */
        private void CheckAmt(int windowNo, String columnName)
        {
            int VAB_Invoice_ID = GetVAB_Invoice_ID();
            //	No Payment
            if (VAB_Invoice_ID == 0)
                return;

            //	Get Info from Tab
            Decimal amount = GetAmount();
            Decimal discountAmt = GetDiscountAmt();
            Decimal writeOffAmt = GetWriteOffAmt();
            Decimal overUnderAmt = GetOverUnderAmt();
            Decimal invoiceAmt = GetInvoiceAmt();
            log.Fine("Amt=" + amount + ", Discount=" + discountAmt
                + ", WriteOff=" + writeOffAmt + ", OverUnder=" + overUnderAmt
                + ", Invoice=" + invoiceAmt);

            //  PayAmt - calculate write off
            if (columnName.Equals("Amount"))
            {
                writeOffAmt = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, amount), discountAmt), overUnderAmt);
                SetWriteOffAmt(writeOffAmt);
            }
            else    //  calculate Amount
            {
                amount = Decimal.Subtract(Decimal.Subtract(Decimal.Subtract(invoiceAmt, discountAmt), writeOffAmt), overUnderAmt);
                SetAmount(amount);
            }
        }

        /// <summary>
        ///  Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true, on Save</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            MPayment payment = new MPayment(GetCtx(), GetVAB_Payment_ID(), Get_TrxName());
            if ((newRecord || Is_ValueChanged("VAB_Invoice_ID"))
                && (payment.GetVAB_Charge_ID() != 0
                    || payment.GetVAB_Invoice_ID() != 0
                    || payment.GetVAB_Order_ID() != 0))
            {
                log.SaveError("PaymentIsAllocated", "");
                return false;
            }

            // during saving a new record, system will check same invoice schedule reference exist on same payment or not
            if (newRecord && Get_ColumnIndex("VAB_sched_InvoicePayment_ID") >= 0 && GetVAB_sched_InvoicePayment_ID() > 0)
            {
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(*) FROM VAB_PaymentAllotment   WHERE VAB_Payment_ID = " + GetVAB_Payment_ID() +
                          @" AND IsActive = 'Y' AND VAB_sched_InvoicePayment_ID = " + GetVAB_sched_InvoicePayment_ID(), null, Get_Trx())) > 0)
                {
                    //"Error" not required
                    //log.SaveError("Error", Msg.GetMsg(GetCtx(), "VIS_NotSaveDuplicateRecord"));
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_NotSaveDuplicateRecord"));
                    return false;
                }
            }

            // check schedule is hold or not, if hold then no to save record
            if (Get_ColumnIndex("VAB_sched_InvoicePayment_ID") >= 0 && GetVAB_sched_InvoicePayment_ID() > 0)
            {
                if (payment.IsHoldpaymentSchedule(GetVAB_sched_InvoicePayment_ID()))
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_PaymentisHold"));
                    return false;
                }
            }

            if (!Env.IsModuleInstalled("VA009_"))
            {
                Decimal check = Decimal.Add(Decimal.Add(Decimal.Add(GetAmount(), GetDiscountAmt()), GetWriteOffAmt()), GetOverUnderAmt());
                if (check.CompareTo(GetInvoiceAmt()) != 0)
                {
                    log.SaveError("Error", Msg.ParseTranslation(GetCtx(),
                         "@InvoiceAmt@(" + GetInvoiceAmt()
                       + ") <> @Totals@(" + check + ")"));
                    return false;
                }
            }

            //	Org
            if (newRecord || Is_ValueChanged("VAB_Invoice_ID"))
            {
                GetInvoice();
                if (_invoice != null)
                    SetVAF_Org_ID(_invoice.GetVAF_Org_ID());
            }

            return true;
        }

        /// <summary>
        /// Added by Bharat to Reset IsPaymentAllocate to false on Deletion of Record..
        /// </summary>
        /// <param name="success"></param>
        /// <returns></returns>
        protected override bool AfterDelete(bool success)
        {
            if (!success)
                return success;
            MPayment pay = new MPayment(GetCtx(), GetVAB_Payment_ID(), Get_TrxName());
            if (pay.Get_ColumnIndex("IsPaymentAllocate") > 0)
            {
                string sql = "SELECT Count(VAB_PaymentAllotment_ID) FROM VAB_PaymentAllotment WHERE VAB_Payment_ID = " + GetVAB_Payment_ID();
                int count = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (count == 0)
                {
                    String qry = "UPDATE VAB_Payment SET IsPaymentAllocate = 'N' WHERE VAB_Payment_ID=" + GetVAB_Payment_ID();
                    int no = DataBase.DB.ExecuteQuery(qry, null, Get_TrxName());
                    if (no != 1)
                    {
                        log.Warning("Payment Allocate not set");
                    }
                }
            }

            // Added bby Bharat on 31 July 2017 as Issue given by Ravikant
            //int _CountVA009 = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(VAF_MODULEINFO_ID) FROM VAF_MODULEINFO WHERE PREFIX='VA009_'  AND IsActive = 'Y'"));
            if (Env.IsModuleInstalled("VA009_"))
            {
                // consider record which are active
                String sql = "UPDATE VAB_Payment i"
                    + " SET PayAmt= (SELECT COALESCE(SUM(Amount),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID) ,  "
                    + "     DiscountAmt= (SELECT COALESCE(SUM(DiscountAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID) ,  "
                    + "     WriteOffAmt= (SELECT COALESCE(SUM(WriteOffAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID) , "
                    + "     OverUnderAmt= (SELECT COALESCE(SUM(OverUnderAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID)  "
                    + (pay.Get_ColumnIndex("PaymentAmount") > 0 ? ", PaymentAmount =  (SELECT COALESCE(SUM(Amount),0) FROM VAB_PaymentAllotment il WHERE il.IsActive = 'Y' AND i.VAB_Payment_ID=il.VAB_Payment_ID)" : "")
                    + "WHERE VAB_Payment_ID=" + GetVAB_Payment_ID();
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                if (no != 1)
                {
                    log.Warning("(1) #" + no);
                }
            }

            // calculate Withholding
            if (!pay.VerifyAndCalculateWithholding(true))
            {
                log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "WrongBackupWithholding"));
            }
            else
            {
                pay._isWithholdingFromPaymentAllocate = true;
                pay.Save();
            }
            return true;
        }

        protected override bool AfterSave(bool newRecord, bool success)
        {
            // Added by Bharat on 27 june 2017 to handle Unique constraint issue
            if (!success)
                return success;

            // Added by Bharat on 25 July 2017 to set IsPaymentAllocate to True.
            MPayment pay = new MPayment(GetCtx(), GetVAB_Payment_ID(), Get_TrxName());
            if (pay.Get_ColumnIndex("IsPaymentAllocate") > 0)
            {
                String qry = "UPDATE VAB_Payment SET IsPaymentAllocate = 'Y' WHERE VAB_Payment_ID=" + GetVAB_Payment_ID();
                int no = DataBase.DB.ExecuteQuery(qry, null, Get_TrxName());
                if (no != 1)
                {
                    log.Warning("Payment Allocate not set");
                }
            }

            if (Env.IsModuleInstalled("VA009_"))
            {
                //changes done to avoid null Exception
                // consider record which are active
                String sql = "UPDATE VAB_Payment i"
                    + " SET PayAmt= ((SELECT COALESCE(SUM(Amount),0) FROM VAB_PaymentAllotment il WHERE il.IsActive = 'Y' AND i.VAB_Payment_ID=il.VAB_Payment_ID) "
                    + (pay.Get_ColumnIndex("PaymentAmount") > 0 ? " - (NVL(BackupWithholdingAmount,0) + NVL(WithholdingAmt,0))),  " : "), ")
                    + "     DiscountAmt= (SELECT COALESCE(SUM(DiscountAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID) ,  "
                    + "     WriteOffAmt= (SELECT COALESCE(SUM(WriteOffAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID) , "
                    + "     OverUnderAmt= (SELECT COALESCE(SUM(OverUnderAmt),0) FROM VAB_PaymentAllotment il WHERE  il.IsActive = 'Y' AND  i.VAB_Payment_ID=il.VAB_Payment_ID)  "
                    + (pay.Get_ColumnIndex("PaymentAmount") > 0 ? ", PaymentAmount =  (SELECT COALESCE(SUM(Amount),0) FROM VAB_PaymentAllotment il WHERE il.IsActive = 'Y' AND i.VAB_Payment_ID=il.VAB_Payment_ID)" : "")
                    + "WHERE VAB_Payment_ID=" + GetVAB_Payment_ID();
                int no = DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
                if (no != 1)
                {
                    log.Warning("(1) #" + no);
                }
            }

            // calculate Withholding
            if (!pay.VerifyAndCalculateWithholding(true))
            {
                log.SaveWarning("Warning", Msg.GetMsg(GetCtx(), "WrongBackupWithholding"));
            }
            else
            {
                pay._isWithholdingFromPaymentAllocate = true;
                pay.Save();// if not saved, then try to calculate on prepare
            }
            return true;
        }

    }
}