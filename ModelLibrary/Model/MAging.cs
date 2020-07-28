/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MAging
 * Purpose        : Aging Model
 * Class Used     : X_T_Aging
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
    public class MAging : X_T_Aging
    {
        /// <summary>
        /// Standard Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="T_Aging_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MAging(Ctx ctx, int T_Aging_ID, Trx trxName)
            : base(ctx, T_Aging_ID, trxName)
        {
            //base(ctx, T_Aging_ID, trxName);
            if (T_Aging_ID == 0)
            {
                //	setAD_PInstance_ID (0);
                //	setC_BP_Group_ID (0);
                //	setC_BPartner_ID (0);
                //	setC_Currency_ID (0);
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
        }	//	T_Aging

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="AD_PInstance_ID">instance</param>
        /// <param name="StatementDate">statement date</param>
        /// <param name="C_BPartner_ID">bpartner</param>
        /// <param name="C_Currency_ID">currency</param>
        /// <param name="C_Invoice_ID">invoice</param>
        /// <param name="C_InvoicePaySchedule_ID">invoice schedule</param>
        /// <param name="C_BP_Group_ID">group</param>
        /// <param name="DueDate">due date</param>
        /// <param name="IsSOTrx">so trx</param>
        /// <param name="trxName">transaction</param>
        public MAging(Ctx ctx, int AD_PInstance_ID, DateTime? StatementDate,
            int C_BPartner_ID, int C_Currency_ID,
            int C_Invoice_ID, int C_InvoicePaySchedule_ID,
            int C_BP_Group_ID, DateTime? DueDate, Boolean IsSOTrx, Trx trxName)
            : this(ctx, 0, trxName)
        {
            //this(ctx, 0, trxName);
            SetAD_PInstance_ID(AD_PInstance_ID);
            SetStatementDate(StatementDate);
            //
            SetC_BPartner_ID(C_BPartner_ID);
            SetC_Currency_ID(C_Currency_ID);
            SetC_BP_Group_ID(C_BP_Group_ID);
            SetIsSOTrx(IsSOTrx);

            //	Optional
            //	setC_Invoice_ID (C_Invoice_ID);		// may be zero
            Set_ValueNoCheck("C_Invoice_ID", Utility.Util.GetValueOfInt(C_Invoice_ID));
            //	setC_InvoicePaySchedule_ID(C_InvoicePaySchedule_ID);	//	may be zero
            Set_ValueNoCheck("C_InvoicePaySchedule_ID", Utility.Util.GetValueOfInt(C_InvoicePaySchedule_ID));
            SetIsListInvoices(C_Invoice_ID != 0);
            //
            SetDueDate(DueDate);		//	only sensible if List invoices
        }	//	MAging


        /// <summary>
        /// Load Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="dr">datarow</param>
        /// <param name="trxName">transaction</param>
        public MAging(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {
            //base(ctx, rs, trxName);
        }	//	MAging

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
            StringBuilder sb = new StringBuilder("MAging[");
            sb.Append("AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                .Append(",C_BPartner_ID=").Append(GetC_BPartner_ID())
                .Append(",C_Currency_ID=").Append(GetC_Currency_ID())
                .Append(",C_Invoice_ID=").Append(GetC_Invoice_ID());
            sb.Append("]");
            return sb.ToString();
        } //	toString

    }	//	MAging
}
