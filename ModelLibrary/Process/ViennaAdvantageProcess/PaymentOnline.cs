/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : PaymentOnline
 * Purpose        : Online Payment Process
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Raghunandan      16-July-2009
  ******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using System.Windows.Forms;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Logging;
using System.Data.SqlClient;


using VAdvantage.ProcessEngine;

namespace ViennaAdvantage.Process
{
    public class PaymentOnline : VAdvantage.ProcessEngine.SvrProcess
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
                    log.Log(Level.SEVERE, "prepare - Unknown Parameter: " + name);
                }
            }
        }

        /// <summary>
        /// Perrform Process.
        /// </summary>
        /// <returns>Message</returns>
        protected override String DoIt()
        {
            log.Info("Record_ID=" + GetRecord_ID());
            //	get Payment
            MPayment pp = new MPayment(GetCtx(), GetRecord_ID(), Get_TrxName());
            //	Validate Number
            String msg = MPaymentValidate.ValidateCreditCardNumber(pp.GetCreditCardNumber(), pp.GetCreditCardType());
            if (msg != null && msg.Length > 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), msg));
            }
            msg = MPaymentValidate.ValidateCreditCardExp(pp.GetCreditCardExpMM(), pp.GetCreditCardExpYY());
            if (msg != null && msg.Length > 0)
            {
                throw new ArgumentException(Msg.GetMsg(GetCtx(), msg));
            }
            if (pp.GetCreditCardVV().Length > 0)
            {
                msg = MPaymentValidate.ValidateCreditCardVV(pp.GetCreditCardVV(), pp.GetCreditCardType());
                if (msg != null && msg.Length > 0)
                {
                    throw new ArgumentException(Msg.GetMsg(GetCtx(), msg));
                }
            }

            //  Process it
            bool ok = pp.ProcessOnline();
            pp.Save();
            if (!ok)
            {
                throw new Exception(pp.GetErrorMessage());
            }
            return "OK";
        }

    }
}
