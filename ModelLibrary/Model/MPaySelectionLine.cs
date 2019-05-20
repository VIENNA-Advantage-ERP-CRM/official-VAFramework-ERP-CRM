using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Data;
using java.io;
using System.Data.SqlClient;
using VAdvantage.Utility;
using VAdvantage.Logging;

namespace VAdvantage.Model
{
    public class MPaySelectionLine : X_C_PaySelectionLine
    {
        /**
	     * 	Standard Constructor
	     *	@param ctx context
	     *	@param C_PaySelectionLine_ID id
	     *	@param trxName transaction
	     */
        public MPaySelectionLine(Ctx ctx, int C_PaySelectionLine_ID, Trx trxName) :
            base(ctx, C_PaySelectionLine_ID, trxName)
        {

            if (C_PaySelectionLine_ID == 0)
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
        public MPaySelectionLine(Ctx ctx, DataRow dr, Trx trxName) :
            base(ctx, dr, trxName)
        {

        }

        /**
         * 	Parent Constructor
         *	@param ps parent
         *	@param Line line
         *	@param PaymentRule payment rule
         */
        public MPaySelectionLine(MPaySelection ps, int Line, String PaymentRule)
            : this(ps.GetCtx(), 0, ps.Get_TrxName())
        {

            SetClientOrg(ps);
            SetC_PaySelection_ID(ps.GetC_PaySelection_ID());
            SetLine(Line);
            SetPaymentRule(PaymentRule);
        }

        /**	Invoice					*/
        private MInvoice _invoice = null;

        /**
         * 	Set Invoice Info
         *	@param C_Invoice_ID invoice
         *	@param isSOTrx sales trx
         *	@param PayAmt payment
         *	@param OpenAmt open
         *	@param DiscountAmt discount
         */
        public void SetInvoice(int C_Invoice_ID, Boolean isSOTrx, Decimal OpenAmt,
            Decimal PayAmt, Decimal DiscountAmt)
        {
            SetC_Invoice_ID(C_Invoice_ID);
            SetIsSOTrx(isSOTrx);
            SetOpenAmt(OpenAmt);
            SetPayAmt(PayAmt);
            SetDiscountAmt(DiscountAmt);
            SetDifferenceAmt(Decimal.Subtract(Decimal.Subtract(OpenAmt, PayAmt), DiscountAmt));
        }

        /**
       * 	Set Invoice Info
       *	@param C_Invoice_ID invoice
       *	@param isSOTrx sales trx
       *	@param PayAmt payment
       *	@param OpenAmt open
       *	@param DiscountAmt discount
       */
        public void SetInvoice(int C_Invoice_ID, Boolean isSOTrx, Decimal OpenAmt,
            Decimal PayAmt, Decimal DiscountAmt, Decimal WriteOffAmount)
        {
            SetC_Invoice_ID(C_Invoice_ID);
            SetIsSOTrx(isSOTrx);
            SetOpenAmt(OpenAmt);
            SetPayAmt(PayAmt);
            SetDiscountAmt(DiscountAmt);
            SetDifferenceAmt(WriteOffAmount);
        }

        /**
         * 	Set Invoice - Callout
         *	@param oldC_Invoice_ID old BP
         *	@param newC_Invoice_ID new BP
         *	@param windowNo window no
         */
        //@UICallout 
        public void SetC_Invoice_ID(String oldC_Invoice_ID,
                String newC_Invoice_ID, int windowNo)
        {
            if (newC_Invoice_ID == null || newC_Invoice_ID.Length == 0)
                return;
            int C_Invoice_ID = int.Parse(newC_Invoice_ID);
            //  reSet as dependent fields Get reSet
            SetContext(windowNo, "C_Invoice_ID", C_Invoice_ID.ToString());
            SetC_Invoice_ID(C_Invoice_ID);
            if (C_Invoice_ID == 0)
            {
                SetPayAmt(Env.ZERO);
                SetDiscountAmt(Env.ZERO);
                SetDifferenceAmt(Env.ZERO);
                return;
            }

            int C_BankAccount_ID = GetCtx().GetContextAsInt(windowNo, "C_BankAccount_ID");
            DateTime PayDate = CommonFunctions.CovertMilliToDate(GetCtx().GetContextAsTime(windowNo, "PayDate"));

            Decimal OpenAmt = Env.ZERO;
            Decimal DiscountAmt = Env.ZERO;
            Boolean IsSOTrx = false;
            String sql = "SELECT currencyConvert(invoiceOpen(i.C_Invoice_ID, 0), i.C_Currency_ID,"
                    + "ba.C_Currency_ID, i.DateInvoiced, i.C_ConversionType_ID, i.AD_Client_ID, i.AD_Org_ID),"
                + " paymentTermDiscount(i.GrandTotal,i.C_Currency_ID,i.C_PaymentTer_ID,i.DateInvoiced, @PayDate), i.IsSOTrx "
                + "FROM C_Invoice_v i, C_BankAccount ba "
                + "WHERE i.C_Invoice_ID=@C_Invoice_ID AND ba.C_BankAccount_ID=@C_BankAccount_ID";	//	#1..2

            IDataReader idr = null;
            try
            {

                SqlParameter[] param = new SqlParameter[3];
                param[0] = new SqlParameter("@C_Invoice_ID", C_Invoice_ID);
                param[1] = new SqlParameter("@C_BankAccount_ID", C_BankAccount_ID);
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
            log.Fine(" - OpenAmt=" + OpenAmt + " (Invoice=" + C_Invoice_ID + ",BankAcct=" + C_BankAccount_ID + ")");
            SetInvoice(C_Invoice_ID, IsSOTrx, OpenAmt, Decimal.Subtract(OpenAmt, DiscountAmt), DiscountAmt);
        }	//	SetC_Invoice_ID

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
            int C_Invoice_ID = GetC_Invoice_ID();
            if (C_Invoice_ID == 0)
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
        public MInvoice GetInvoice()
        {
            if (_invoice == null)
                _invoice = new MInvoice(GetCtx(), GetC_Invoice_ID(), Get_TrxName());
            return _invoice;
        }


        /**
         * 	Before Save
         *	@param newRecord new
         *	@return true
         */
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            SetDifferenceAmt(Decimal.Subtract(Decimal.Subtract(GetOpenAmt(), GetPayAmt()), GetDiscountAmt()));
            return true;
        }

        /**
         * 	After Save
         *	@param newRecord new
         *	@param success success
         *	@return success
         */
        protected override Boolean AfterSave(Boolean newRecord, Boolean success)
        {
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
            String sql = "UPDATE C_PaySelection ps "
                + "SET TotalAmt = (SELECT COALESCE(SUM(psl.PayAmt),0) "
                    + "FROM C_PaySelectionLine psl "
                    + "WHERE ps.C_PaySelection_ID=psl.C_PaySelection_ID AND psl.IsActive='Y') "
                + "WHERE C_PaySelection_ID=" + GetC_PaySelection_ID();
            DataBase.DB.ExecuteQuery(sql, null, Get_TrxName());
        }

        /**
         * 	String Representation
         * 	@return info
         */
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MPaySelectionLine[");
            sb.Append(Get_ID()).Append(",C_Invoice_ID=").Append(GetC_Invoice_ID())
                .Append(",PayAmt=").Append(GetPayAmt())
                .Append(",DifferenceAmt=").Append(GetDifferenceAmt())
                .Append("]");
            return sb.ToString();
        }

    }
}
