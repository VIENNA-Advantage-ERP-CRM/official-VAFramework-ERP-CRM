/********************************************************
 * Module Name    : InvoicePayScheduleValidate
 * Purpose        : To display message on third tab when
 *                  user set amount and discount in Invoice Tab
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological Development
 * Raghunandan     27-july-2009
 ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VAdvantage.Classes;

using VAdvantage.Common;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.WF;
using VAdvantage.Process;
using VAdvantage.Utility;


using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class InvoicePayScheduleValidate : ProcessEngine.SvrProcess
    {
        /// <summary>
        /// Prepare - e.g., get Parameters.
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                    ;
                }
                else
                {
                    //log.log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                    //////ErrorLog.FillErrorLog("", "", "prepare - Unknown Parameter: " + name,VAdvantage.Framework.Message.MessageType.INFORMATION);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message (clear text)</returns>
        protected override String DoIt()
        {
            //log.info ("C_InvoicePaySchedule_ID=" + getRecord_ID());
            MInvoicePaySchedule[] schedule = MInvoicePaySchedule.GetInvoicePaySchedule(GetCtx(), 0, GetRecord_ID(), null);
            if (schedule.Length == 0)
            {
                throw new ArgumentException("InvoicePayScheduleValidate - No Schedule");
            }
            //	Get Invoice
            MInvoice invoice = new MInvoice(GetCtx(), schedule[0].GetC_Invoice_ID(), null);
            if (invoice.Get_ID() == 0)
            {
                throw new ArgumentException("InvoicePayScheduleValidate - No Invoice");
            }
            //
            Decimal total = Env.ZERO;
            for (int i = 0; i < schedule.Length; i++)
            {
                Decimal due = schedule[i].GetDueAmt();
                if (due != 0)
                {
                    total = Decimal.Add(total, due);
                }
            }
            bool valid = invoice.GetGrandTotal().CompareTo(total) == 0;
            invoice.SetIsPayScheduleValid(valid);
            invoice.Save();
            //	Schedule
            for (int i = 0; i < schedule.Length; i++)
            {
                if (schedule[i].IsValid() != valid)
                {
                    schedule[i].SetIsValid(valid);
                    schedule[i].Save();
                }
            }
            String msg = "@OK@";
            if (!valid)
            {
                msg = "@GrandTotal@ = " + invoice.GetGrandTotal()
                    + " <> @Total@ = " + total
                    + "  - @Difference@ = " + Decimal.Subtract(invoice.GetGrandTotal(), total);
            }
            return Msg.ParseTranslation(GetCtx(), msg);
        }

    }
}
