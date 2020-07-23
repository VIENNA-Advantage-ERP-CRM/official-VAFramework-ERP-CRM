/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : DocLine_Bank
 * Purpose        :  Bank Statement Line
 * Class Used     : DocLine
 * Chronological    Development
 * Raghunandan      19-Jan-2010
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
using VAdvantage.Report;
using System.Data.SqlClient;

namespace VAdvantage.Acct
{
    /// <summary>
    /// Bank Statement Line
    /// </summary>
    public class DocLine_Bank : DocLine
    {
        //Reversal Flag		
        private bool _IsReversal = false;
        //Payment					
        private int _C_Payment_ID = 0;

        private Decimal _TrxAmt = Env.ZERO;
        private Decimal _StmtAmt = Env.ZERO;
        private Decimal _InterestAmt = Env.ZERO;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="line"></param>
        /// <param name="doc"></param>
        public DocLine_Bank(MBankStatementLine line, Doc_Bank doc)
            : base(line, doc)
        {

            _C_Payment_ID = line.GetC_Payment_ID();
            _IsReversal = line.IsReversal();
            //
            _StmtAmt = line.GetStmtAmt();
            _InterestAmt = line.GetInterestAmt();
            _TrxAmt = line.GetTrxAmt();
            //
            SetDateDoc(line.GetValutaDate());
            SetC_BPartner_ID(line.GetC_BPartner_ID());
        }



        /// <summary>
        ///  Get Payment
        /// </summary>
        /// <returns>C_Paymnet_ID</returns>
        public int GetC_Payment_ID()
        {
            return _C_Payment_ID;
        }

        /// <summary>
        /// Get AD_Org_ID
        /// </summary>
        /// <param name="payment">if true get Org from payment</param>
        /// <returns>org</returns>
        public int GetAD_Org_ID(bool payment)
        {
            if (payment && GetC_Payment_ID() != 0)
            {
                String sql = "SELECT AD_Org_ID FROM C_Payment WHERE C_Payment_ID=@param1";
                int id = DataBase.DB.GetSQLValue(null, sql, GetC_Payment_ID());
                if (id > 0)
                {
                    return id;
                }
            }
            return base.GetAD_Org_ID();
        }

        /// <summary>
        /// Is Reversal
        /// </summary>
        /// <returns>true if reversal</returns>
        public bool IsReversal()
        {
            return _IsReversal;
        }

        /// <summary>
        /// Get Interest
        /// </summary>
        /// <returns>InterestAmount</returns>
        public Decimal GetInterestAmt()
        {
            return _InterestAmt;
        }

        /// <summary>
        /// Get Statement
        /// </summary>
        /// <returns>Starement Amount</returns>
        public Decimal GetStmtAmt()
        {
            return _StmtAmt;
        }

        /// <summary>
        /// Get Transaction
        /// </summary>
        /// <returns>transaction amount</returns>
        public Decimal GetTrxAmt()
        {
            return _TrxAmt;
        }

    }
}
