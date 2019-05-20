/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : BankStatementMatcher
    * Purpose        : Bank Statement Matching
    * Class Used     : ProcessEngine.SvrProcess
    * Chronological    Development
    * Raghunandan     26-Nov-2009
******************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.Classes;
using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BankStatementMatcher : ProcessEngine.SvrProcess
    {
        //Matchers					
        MBankStatementMatcher[] _matchers = null;

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
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
                }
            }
            _matchers = MBankStatementMatcher.GetMatchers(GetCtx(), Get_Trx());
        }

        /// <summary>
        /// Perform Process.
        /// </summary>
        /// <returns>Message </returns>
        protected override String DoIt()
        {
            int Table_ID = GetTable_ID();
            int Record_ID = GetRecord_ID();
            if (_matchers == null || _matchers.Length == 0)
            {
                throw new Exception("No Matchers found");
            }
            //
            log.Info("doIt - Table_ID=" + Table_ID + ", Record_ID=" + Record_ID
                + ", Matchers=" + _matchers.Length);

            if (Table_ID == X_I_BankStatement.Table_ID)
            {
                return Match(new X_I_BankStatement(GetCtx(), Record_ID, Get_Trx()));
            }
            else if (Table_ID == MBankStatement.Table_ID)
            {
                return Match(new MBankStatement(GetCtx(), Record_ID, Get_Trx()));
            }
            else if (Table_ID == MBankStatementLine.Table_ID)
            {
                return Match(new MBankStatementLine(GetCtx(), Record_ID, Get_Trx()));
            }

            return "??";
        }

        /// <summary>
        /// Perform Match
        /// </summary>
        /// <param name="ibs">import bank statement line</param>
        /// <returns>Message</returns>
        private String Match(X_I_BankStatement ibs)
        {
            if (_matchers == null || ibs == null || ibs.GetC_Payment_ID() != 0)
            {
                return "--";
            }

            log.Fine("" + ibs);
            BankStatementMatchInfo info = null;
            for (int i = 0; i < _matchers.Length; i++)
            {
                if (_matchers[i].IsMatcherValid())
                {
                    info = _matchers[i].GetMatcher().FindMatch(ibs);
                    if (info != null && info.IsMatched())
                    {
                        if (info.GetC_Payment_ID() > 0)
                        {
                            ibs.SetC_Payment_ID(info.GetC_Payment_ID());
                        }
                        if (info.GetC_Invoice_ID() > 0)
                        {
                            ibs.SetC_Invoice_ID(info.GetC_Invoice_ID());
                        }
                        if (info.GetC_BPartner_ID() > 0)
                        {
                            ibs.SetC_BPartner_ID(info.GetC_BPartner_ID());
                        }
                        ibs.Save();
                        return "OK";
                    }
                }
            }	//	for all matchers
            return "--";
        }


        /// <summary>
        /// Perform Match
        /// </summary>
        /// <param name="bsl">bank statement line</param>
        /// <returns>Message</returns>
        private String Match(MBankStatementLine bsl)
        {
            if (_matchers == null || bsl == null || bsl.GetC_Payment_ID() != 0)
            {
                return "--";
            }

            log.Fine("match - " + bsl);
            BankStatementMatchInfo info = null;
            for (int i = 0; i < _matchers.Length; i++)
            {
                if (_matchers[i].IsMatcherValid())
                {
                    info = _matchers[i].GetMatcher().FindMatch(bsl);
                    if (info != null && info.IsMatched())
                    {
                        if (info.GetC_Payment_ID() > 0)
                        {
                            bsl.SetC_Payment_ID(info.GetC_Payment_ID());
                        }
                        if (info.GetC_Invoice_ID() > 0)
                        {
                            bsl.SetC_Invoice_ID(info.GetC_Invoice_ID());
                        }
                        if (info.GetC_BPartner_ID() > 0)
                        {
                            bsl.SetC_BPartner_ID(info.GetC_BPartner_ID());
                        }
                        bsl.Save();
                        return "OK";
                    }
                }
            }	//	for all matchers
            return "--";
        }

        /// <summary>
        /// Perform Match
        /// </summary>
        /// <param name="bs">bank statement</param>
        /// <returns>Message</returns>
        private String Match(MBankStatement bs)
        {
            if (_matchers == null || bs == null)
            {
                return "--";
            }
            log.Fine("match - " + bs);
            int count = 0;
            MBankStatementLine[] lines = bs.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].GetC_Payment_ID() == 0)
                {
                    Match(lines[i]);
                    count++;
                }
            }
            //return String.valueOf(count);
            return count.ToString();
        }
    }
}
