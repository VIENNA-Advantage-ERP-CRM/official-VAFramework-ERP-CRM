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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BankStatementMatcher : ProcessEngine.SvrProcess
    {
        //Matchers					
        MVABBankingJRNLMatcher[] _matchers = null;

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
            _matchers = MVABBankingJRNLMatcher.GetMatchers(GetCtx(), Get_Trx());
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

            if (Table_ID == X_VAI_BankJRNL.Table_ID)
            {
                return Match(new X_VAI_BankJRNL(GetCtx(), Record_ID, Get_Trx()));
            }
            else if (Table_ID == MVABBankingJRNL.Table_ID)
            {
                return Match(new MVABBankingJRNL(GetCtx(), Record_ID, Get_Trx()));
            }
            else if (Table_ID == MVABBankingJRNLLine.Table_ID)
            {
                return Match(new MVABBankingJRNLLine(GetCtx(), Record_ID, Get_Trx()));
            }

            return "??";
        }

        /// <summary>
        /// Perform Match
        /// </summary>
        /// <param name="ibs">import bank statement line</param>
        /// <returns>Message</returns>
        private String Match(X_VAI_BankJRNL ibs)
        {
            if (_matchers == null || ibs == null || ibs.GetVAB_Payment_ID() != 0)
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
                        if (info.GetVAB_Payment_ID() > 0)
                        {
                            ibs.SetVAB_Payment_ID(info.GetVAB_Payment_ID());
                        }
                        if (info.GetVAB_Invoice_ID() > 0)
                        {
                            ibs.SetVAB_Invoice_ID(info.GetVAB_Invoice_ID());
                        }
                        if (info.GetVAB_BusinessPartner_ID() > 0)
                        {
                            ibs.SetVAB_BusinessPartner_ID(info.GetVAB_BusinessPartner_ID());
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
        private String Match(MVABBankingJRNLLine bsl)
        {
            if (_matchers == null || bsl == null || bsl.GetVAB_Payment_ID() != 0)
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
                        if (info.GetVAB_Payment_ID() > 0)
                        {
                            bsl.SetVAB_Payment_ID(info.GetVAB_Payment_ID());
                        }
                        if (info.GetVAB_Invoice_ID() > 0)
                        {
                            bsl.SetVAB_Invoice_ID(info.GetVAB_Invoice_ID());
                        }
                        if (info.GetVAB_BusinessPartner_ID() > 0)
                        {
                            bsl.SetVAB_BusinessPartner_ID(info.GetVAB_BusinessPartner_ID());
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
        private String Match(MVABBankingJRNL bs)
        {
            if (_matchers == null || bs == null)
            {
                return "--";
            }
            log.Fine("match - " + bs);
            int count = 0;
            MVABBankingJRNLLine[] lines = bs.GetLines(false);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].GetVAB_Payment_ID() == 0)
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
