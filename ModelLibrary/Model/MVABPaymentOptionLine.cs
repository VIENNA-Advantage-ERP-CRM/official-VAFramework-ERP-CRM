using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
//////using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using java.io;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MVABPaymentOptionLine : X_VAB_PaymentOptionLine
    {
        /**
	     * 	Standard Constructor
	     *	@param ctx context
	     *	@param VAB_PaymentOptionLine_ID id
	     *	@param trxName transaction
	     */
        public MVABPaymentOptionLine(Ctx ctx, int VAB_PaymentOptionLine_ID, Trx trxName) :
            base(ctx, VAB_PaymentOptionLine_ID, trxName)
        {

            if (VAB_PaymentOptionLine_ID == 0)
            {
                SetIsSOTrx(false);
                SetOpenAmt(Env.ZERO);
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetDifferenceAmt(Env.ZERO);
                SetIsManual(false);
            }
        }

        /**
         * 	Load Constructor
         *	@param ctx context
         *	@param rs result Set
         *	@param trxName transaction
         */
        public MVABPaymentOptionLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param ps parent
         *	@param Line line
         *	@param PaymentRule payment rule
         */
        public MVABPaymentOptionLine(MVABPaymentOption ps, int Line, String PaymentRule)
            : this(ps.GetCtx(), 0, ps.Get_TrxName())
        {

            SetClientOrg(ps);
            SetVAB_PaymentOption_ID(ps.GetVAB_PaymentOption_ID());
            SetLine(Line);
            SetPaymentRule(PaymentRule);
        }

        /**	Invoice					*/
        private MVABInvoice _invoice = null;

        /**
         * 	Set Invoice Info
         *	@param VAB_Invoice_ID invoice
         *	@param isSOTrx sales trx
         *	@param PayAmt payment
         *	@param OpenAmt open
         *	@param DiscountAmt discount
         */
        public void SetInvoice(int VAB_Invoice_ID, Boolean isSOTrx, Decimal OpenAmt,
            Decimal PayAmt, Decimal DiscountAmt)
        {
            SetVAB_Invoice_ID(VAB_Invoice_ID);
            SetIsSOTrx(isSOTrx);
            SetOpenAmt(OpenAmt);
            SetPayAmt(PayAmt);
            SetDiscountAmt(DiscountAmt);
            SetDifferenceAmt(Decimal.Subtract(Decimal.Subtract(OpenAmt, PayAmt), DiscountAmt));
        }

        /**
       * 	Set Invoice Info
       *	@param VAB_Invoice_ID invoice
       *	@param isSOTrx sales trx
       *	@param PayAmt payment
       *	@param OpenAmt open
       *	@param DiscountAmt discount
       */
        public void SetInvoice(int VAB_Invoice_ID, Boolean isSOTrx, Decimal OpenAmt,
            Decimal PayAmt, Decimal DiscountAmt, Decimal WriteOffAmount)
        {
            SetVAB_Invoice_ID(VAB_Invoice_ID);
            SetIsSOTrx(isSOTrx);
            SetOpenAmt(OpenAmt);
            SetPayAmt(PayAmt);
            SetDiscountAmt(DiscountAmt);
            SetDifferenceAmt(WriteOffAmount);
        }

        /**
         * 	Set Invoice - Callout
         *	@param oldVAB_Invoice_ID old BP
         *	@param newVAB_Invoice_ID new BP
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetVAB_Invoice_ID(String oldVAB_Invoice_ID,
                String newVAB_Invoice_ID, int windowNo)
        {
            if (newVAB_Invoice_ID == null || newVAB_Invoice_ID.Length == 0)
                return;
            int VAB_Invoice_ID = int.Parse(newVAB_Invoice_ID);
            //  reSet as dependent fields Get reSet
            SetContext(windowNo, "VAB_Invoice_ID", VAB_Invoice_ID.ToString());
            SetVAB_Invoice_ID(VAB_Invoice_ID);
            if (VAB_Invoice_ID == 0)
            {
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetDifferenceAmt(Env.ZERO);
                return;
            }

            int VAB_Bank_Acct_ID = GetCtx().GetContextAsInt(windowNo, "VAB_Bank_Acct_ID");
            DateTime PayDate = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime(windowNo, "PayDate"));

            Decimal OpenAmt = Env.ZERO;
            Decimal DiscountAmt = Env.ZERO;
            Boolean IsSOTrx = false;
            String sql = "SELECT currencyConvert(invoiceOpen(i.VAB_Invoice_ID, 0), i.VAB_Currency_ID,"
                    + "ba.VAB_Currency_ID, i.DateInvoiced, i.VAB_CurrencyType_ID, i.VAF_Client_ID, i.VAF_Org_ID),"
                + " paymentTermDiscount(i.GrandTotal,i.VAB_Currency_ID,i.VAB_PaymentTer_ID,i.DateInvoiced, @PayDate), i.IsSOTrx "
                + "FROM VAB_Invoice_v i, VAB_Bank_Acct ba "
                + "WHERE i.VAB_Invoice_ID=@VAB_Invoice_ID AND ba.VAB_Bank_Acct_ID=@VAB_Bank_Acct_ID";	//	#1..2

            IDataReader idr = null;
            try
            {

                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@VAB_Invoice_ID", VAB_Invoice_ID);
                param[1] = new SqlParameter("@VAB_Bank_Acct_ID", VAB_Bank_Acct_ID);
                param[2] = new SqlParameter("@PayDate", PayDate);

                idr = DataBase.DB.ExecuteReader(sql, param);
                //ResultSet rs = pstmt.executeQuery();

                if (idr.Read())
                {
                    OpenAmt = Utility.Util.GetValueOfDecimal(idr[1]);
                    DiscountAmt = Utility.Util.GetValueOfDecimal(idr[2]);
                    IsSOTrx = "Y".Equals(idr.GetString(3));
                }
                idr.Close();

            }
            catch (SqlException e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                log.Log(Level.SEVERE, sql, e);
            }
            log.Fine(" - OpenAmt=" + OpenAmt + " (Invoice=" + VAB_Invoice_ID + ",BankAcct=" + VAB_Bank_Acct_ID + ")");
            SetInvoice(VAB_Invoice_ID, IsSOTrx, OpenAmt, Decimal.Subtract(OpenAmt, DiscountAmt), DiscountAmt);
        }	//	SetVAB_Invoice_ID

        /**
         * 	Set Pay Amt - Callout
         *	@param oldPayAmt old value
         *	@param newPayAmt new value
         *	@param windowNo window
         *	@throws Exception
         */
        //@UICallout 
        public void SetPayAmt(String oldPayAmt, String newPayAmt, int windowNo)
        {
            if (newPayAmt == null || newPayAmt.Length == 0)
                return;
            Decimal PayAmt = (Decimal)PO.ConvertToBigDecimal(newPayAmt);

            Decimal OpenAmt = GetOpenAmt();
            Decimal DiscountAmt = GetDiscountAmt();
            Decimal DifferenceAmt = Decimal.Subtract(Decimal.Subtract(OpenAmt, PayAmt), DiscountAmt);

            //	Get invoice info
            int VAB_Invoice_ID = GetVAB_Invoice_ID();
            if (VAB_Invoice_ID == 0)
            {
                PayAmt = Env.ZERO;
                DifferenceAmt = Env.ZERO;
                DiscountAmt = Env.ZERO;
                SetDiscountAmt(DiscountAmt);
            }
            log.Fine("OpenAmt=" + OpenAmt + " - PayAmt=" + PayAmt
                + ", Discount=" + DiscountAmt + ", Difference=" + DifferenceAmt);

            SetPayAmt(PayAmt);
            SetDifferenceAmt(DifferenceAmt);
        }


        /**
         * 	Get Invoice
         *	@return invoice
         */
        public MVABInvoice GetInvoice()
        {
            if (_invoice == null)
                _invoice = new MVABInvoice(GetCtx(), GetVAB_Invoice_ID(), Get_TrxName());
            return _invoice;
        }


        /// <summary>
        ///  Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true, on Save</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            // check schedule is hold or not, if hold then do not save record
            if (GetVAB_sched_InvoicePayment_ID() > 0)
            {
                if (IsHoldpaymentSchedule(GetVAB_sched_InvoicePayment_ID()))
                {
                    log.SaveError("", Msg.GetMsg(GetCtx(), "VIS_PaymentisHold"));
                    return false;
                }
            }

            SetDifferenceAmt(Decimal.Subtract(Decimal.Subtract(GetOpenAmt(), GetPayAmt()), GetDiscountAmt()));
            return true;
        }

        /// <summary>
        /// Is used to get Invoice payment schedule is Hold Payment or not
        /// </summary>
        /// <param name="VAB_sched_InvoicePayment_ID">Invoice payment schedule reference</param>
        /// <returns>TRUE, if hold payment</returns>
        public bool IsHoldpaymentSchedule(int VAB_sched_InvoicePayment_ID)
        {
            try
            {
                String sql = "SELECT IsHoldPayment FROM VAB_sched_InvoicePayment WHERE VAB_sched_InvoicePayment_ID = " + VAB_sched_InvoicePayment_ID;
                String IsHoldPayment = Util.GetValueOfString(DB.ExecuteScalar(sql, null, Get_Trx()));
                return IsHoldPayment.Equals("Y");
            }
            catch
            {
                // when column not found, mean hold payment functionlity not in system
                return false;
            }
        }

        /// <summary>
        ///  After Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true, After Save</returns>
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
            if (!success)
            {
                return success;
            }
            SetHeader();
            return success;
        }

        /**
         * 	After Delete
         *	@param success success
         *	@return sucess
         */
        protected override Boolean AfterDelete(Boolean success)
        {
            SetHeader();
            return success;
        }

        /**
         * 	Recalculate Header Sum
         */
        private void SetHeader()
        {
            //	Update Header
            String sql = "UPDATE VAB_PaymentOption ps "
                + "SET TotalAmt = (SELECT COALESCE(SUM(psl.PayAmt),0) "
                    + "FROM VAB_PaymentOptionLine psl "
                    + "WHERE ps.VAB_PaymentOption_ID=psl.VAB_PaymentOption_ID AND psl.IsActive='Y') "
                + "WHERE VAB_PaymentOption_ID=" + GetVAB_PaymentOption_ID();
            DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
        }

        /**
         * 	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaySelectionLine[");
            sb.Append(Get_ID()).Append(",VAB_Invoice_ID=").Append(GetVAB_Invoice_ID())
                .Append(",PayAmt=").Append(GetPayAmt())
                .Append(",DifferenceAmt=").Append(GetDifferenceAmt())
                .Append("]");
            return sb.ToString();
        }

    }
}
