/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MVATAging
 * Purpose        : Aging Model
 * Class Used     : X_VAT_Aging
 * Chronological    Development
 * Deepak           14-Jan-2010
  ******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Process;
using VAdvantage.Classes;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using System.Data;
using System.Data.SqlClient;
using VAdvantage.Logging;
using VAdvantage.Utility;
using System.Drawing;

namespace VAdvantage.Model
{
    public class MVATAging : X_VAT_Aging
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAT_Aging_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVATAging(Ctx ctx, int VAT_Aging_ID, Trx trxName)
            : base(ctx, VAT_Aging_ID, trxName)
        {
            //base(ctx, VAT_Aging_ID, trxName);
            if (VAT_Aging_ID == 0)
            {
                //	setVAF_JInstance_ID (0);
                //	setVAB_BPart_Category_ID (0);
                //	setVAB_BusinessPartner_ID (0);
                //	setVAB_Currency_ID (0);
                //
                SetDueAmt(Env.ZERO);
                SetDue0(Env.ZERO);
                SetDue0_7(Env.ZERO);
                SetDue0_30(Env.ZERO);
                SetDue1_7(Env.ZERO);
                SetDue31_60(Env.ZERO);
                SetDue31_Plus(Env.ZERO);
                SetDue61_90(Env.ZERO);
                SetDue61_Plus(Env.ZERO);
                SetDue8_30(Env.ZERO);
                SetDue91_Plus(Env.ZERO);
                //
                SetPastDueAmt(Env.ZERO);
                SetPastDue1_7(Env.ZERO);
                SetPastDue1_30(Env.ZERO);
                SetPastDue31_60(Env.ZERO);
                SetPastDue31_Plus(Env.ZERO);
                SetPastDue61_90(Env.ZERO);
                SetPastDue61_Plus(Env.ZERO);
                SetPastDue8_30(Env.ZERO);
                SetPastDue91_Plus(Env.ZERO);
                //
                SetOpenAmt(Env.ZERO);
                SetInvoicedAmt(Env.ZERO);
                //
                SetIsListInvoices(false);
                SetIsSOTrx(false);
                //	setDueDate (new Timestamp(System.currentTimeMillis()));
                //	setStatementDate (new Timestamp(System.currentTimeMillis()));
            }
        }	//	VAT_Aging

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_JInstance_ID">instance</param>
        /// <param name="StatementDate">statement date</param>
        /// <param name="VAB_BusinessPartner_ID">bpartner</param>
        /// <param name="VAB_Currency_ID">currency</param>
        /// <param name="VAB_Invoice_ID">invoice</param>
        /// <param name="VAB_sched_InvoicePayment_ID">invoice schedule</param>
        /// <param name="VAB_BPart_Category_ID">group</param>
        /// <param name="DueDate">due date</param>
        /// <param name="IsSOTrx">so trx</param>
        /// <param name="trxName">transaction</param>
        public MVATAging(Ctx ctx, int VAF_JInstance_ID, DateTime? StatementDate,
            int VAB_BusinessPartner_ID, int VAB_Currency_ID,
            int VAB_Invoice_ID, int VAB_sched_InvoicePayment_ID,
            int VAB_BPart_Category_ID, DateTime? DueDate, Boolean IsSOTrx, Trx trxName)
            : this(ctx, 0, trxName)
        {
            //this(ctx, 0, trxName);
            SetVAF_JInstance_ID(VAF_JInstance_ID);
            SetStatementDate(StatementDate);
            //
            SetVAB_BusinessPartner_ID(VAB_BusinessPartner_ID);
            SetVAB_Currency_ID(VAB_Currency_ID);
            SetVAB_BPart_Category_ID(VAB_BPart_Category_ID);
            SetIsSOTrx(IsSOTrx);

            //	Optional
            //	setVAB_Invoice_ID (VAB_Invoice_ID);		// may be zero
            Set_ValueNoCheck("VAB_Invoice_ID", Utility.Util.GetValueOfInt(VAB_Invoice_ID));
            //	setVAB_sched_InvoicePayment_ID(VAB_sched_InvoicePayment_ID);	//	may be zero
            Set_ValueNoCheck("VAB_sched_InvoicePayment_ID", Utility.Util.GetValueOfInt(VAB_sched_InvoicePayment_ID));
            SetIsListInvoices(VAB_Invoice_ID != 0);
            //
            SetDueDate(DueDate);		//	only sensible if List invoices
        }	//	MVATAging


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVATAging(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //base(ctx, rs, trxName);
        }	//	MVATAging

        /** Number of items 		*/
        private int _noItems = 0;
        /** Sum of Due Days			*/
        private int _daysDueSum = 0;

        /// <summary>
        /// Add Amount to Buckets
        /// </summary>
        /// <param name="DueDate">due date </param>
        /// <param name="daysDue">positive due - negative not due</param>
        /// <param name="invoicedAmt">invoiced amount</param>
        /// <param name="openAmt">open amount</param>
        public void Add(DateTime? DueDate, int? daysDue, Decimal? invoicedAmt, Decimal? openAmt)
        {
            if (invoicedAmt == null)
            {
                invoicedAmt = Env.ZERO;
            }
            //SetInvoicedAmt(GetInvoicedAmt().add(invoicedAmt));
            SetInvoicedAmt(Decimal.Add(GetInvoicedAmt(), invoicedAmt.Value));
            if (openAmt == null)
            {
                openAmt = Env.ZERO;
            }
            //SetOpenAmt(getOpenAmt().add(openAmt));
            SetOpenAmt(Decimal.Add(GetOpenAmt(), openAmt.Value));
            //	Days Due
            _noItems++;
            _daysDueSum += daysDue.Value;
            SetDaysDue(_daysDueSum / _noItems);
            //	Due Date
            //if (getDueDate().after(DueDate))
            if (GetDueDate() > DueDate.Value)
            {
                SetDueDate(DueDate.Value);		//	earliest
            }
            //
            Decimal? amt = openAmt;
            //	Not due - negative
            if (daysDue.Value <= 0)
            {
                //SetDueAmt(getDueAmt().add(amt));
                SetDueAmt(Decimal.Add(GetDueAmt(), amt.Value));
                if (daysDue == 0)
                {
                    SetDue0(Decimal.Add(GetDue0(), amt.Value));
                    //SetDue0(getDue0().add(amt));
                }

                if (daysDue >= -7)
                {
                    // setDue0_7(getDue0_7().add(amt));
                    SetDue0_7(Decimal.Add(GetDue0_7(), amt.Value));
                }

                if (daysDue >= -30)
                {
                    //SetDue0_30(getDue0_30().add(amt));
                    SetDue0_30(Decimal.Add(GetDue0_30(), amt.Value));
                }

                if (daysDue <= -1 && daysDue >= -7)
                {
                    // setDue1_7(getDue1_7().add(amt));
                    SetDue1_7(Decimal.Add(GetDue1_7(), amt.Value));
                }

                if (daysDue <= -8 && daysDue >= -30)
                {
                    // setDue8_30(getDue8_30().add(amt));
                    SetDue8_30(Decimal.Add(GetDue8_30(), amt.Value));
                }

                if (daysDue <= -31 && daysDue >= -60)
                {
                    SetDue31_60(Decimal.Add(GetDue31_60(), amt.Value));
                }

                if (daysDue <= -31)
                {
                    //setDue31_Plus(getDue31_Plus().add(amt));
                    SetDue31_Plus(Decimal.Add(GetDue31_Plus(), amt.Value));
                }

                if (daysDue <= -61 && daysDue >= -90)
                {
                    // setDue61_90(getDue61_90().add(amt));
                    SetDue61_90(Decimal.Add(GetDue61_90(), amt.Value));
                }

                if (daysDue <= -61)
                {
                    //SetDue61_Plus(getDue61_Plus().add(amt));
                    SetDue61_Plus(Decimal.Add(GetDue61_Plus(), amt.Value));
                }

                if (daysDue <= -91)
                {
                    //setDue91_Plus(getDue91_Plus().add(amt));
                    SetDue91_Plus(Decimal.Add(GetDue91_Plus(), amt.Value));
                }
            }
            else	//	Due = positive (> 1)
            {
                //setPastDueAmt(getPastDueAmt().add(amt));
                SetPastDueAmt(Decimal.Add(GetPastDueAmt(), amt.Value));
                if (daysDue <= 7)
                {
                    // setPastDue1_7(getPastDue1_7().add(amt));
                    SetPastDue1_7(Decimal.Add(GetPastDue1_7(), amt.Value));
                }

                if (daysDue <= 30)
                {
                    // setPastDue1_30(getPastDue1_30().add(amt));
                    SetPastDue1_30(Decimal.Add(GetPastDue1_30(), amt.Value));
                }

                if (daysDue >= 8 && daysDue <= 30)
                {
                    //setPastDue8_30(getPastDue8_30().add(amt));
                    SetPastDue8_30(Decimal.Add(GetPastDue8_30(), amt.Value));
                }

                if (daysDue >= 31 && daysDue <= 60)
                {
                    //setPastDue31_60(getPastDue31_60().add(amt));
                    SetPastDue31_60(Decimal.Add(GetPastDue31_60(), amt.Value));
                }

                if (daysDue >= 31)
                {
                    //setPastDue31_Plus(getPastDue31_Plus().add(amt));
                    SetPastDue31_Plus(Decimal.Add(GetPastDue31_Plus(), amt.Value));
                }

                if (daysDue >= 61 && daysDue <= 90)
                {
                    //SetPastDue61_90(getPastDue61_90().add(amt));
                    SetPastDue61_90(Decimal.Add(GetPastDue61_90(), amt.Value));
                }

                if (daysDue >= 61)
                {
                    //setPastDue61_Plus(getPastDue61_Plus().add(amt));
                    SetPastDue61_Plus(Decimal.Add(GetPastDue61_Plus(), amt.Value));
                }

                if (daysDue >= 91)
                {
                    // setPastDue91_Plus(getPastDue91_Plus().add(amt));
                    SetPastDue91_Plus(Decimal.Add(GetPastDue91_Plus(), amt.Value));
                }
            }
        }	//	add

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MVATAging[");
            sb.Append("VAF_JInstance_ID=").Append(GetVAF_JInstance_ID())
                .Append(",VAB_BusinessPartner_ID=").Append(GetVAB_BusinessPartner_ID())
                .Append(",VAB_Currency_ID=").Append(GetVAB_Currency_ID())
                .Append(",VAB_Invoice_ID=").Append(GetVAB_Invoice_ID());
            sb.Append("]");
            return sb.ToString();
        } //	toString

    }	//	MVATAging
}
