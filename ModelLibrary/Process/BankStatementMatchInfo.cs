/********************************************************
    * Project Name   : VAdvantage
    * Class Name     : BankStatementMatchInfo
    * Purpose        : Bank Statement Match Information.
    *	               Returned by Bank Statement Matcher	
    * Class Used     : 
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
    public class BankStatementMatchInfo
    {
        #region
        private int _C_BPartner_ID = 0;
        private int _C_Payment_ID = 0;
        private int _C_Invoice_ID = 0;
        #endregion

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public BankStatementMatchInfo()
        {
            //base();//Super();
        }

        /// <summary>
        /// Do we have a match?
        /// </summary>
        /// <returns>true if something could be matched</returns>
        public bool IsMatched()
        {
            return _C_BPartner_ID > 0 || _C_Payment_ID > 0 || _C_Invoice_ID > 0;
        }

        /// <summary>
        /// Get matched BPartner
        /// </summary>
        /// <returns>BPartner</returns>
        public int GetC_BPartner_ID()
        {
            return _C_BPartner_ID;
        }

        /// <summary>
        /// Set matched BPartner
        /// </summary>
        /// <param name="C_BPartner_ID">BPartner</param>
        public void SetC_BPartner_ID(int C_BPartner_ID)
        {
            _C_BPartner_ID = C_BPartner_ID;
        }

        /// <summary>
        /// Get matched Payment
        /// </summary>
        /// <returns>Payment</returns>
        public int GetC_Payment_ID()
        {
            return _C_Payment_ID;
        }

        /// <summary>
        /// Set matched Payment
        /// </summary>
        /// <param name="C_Payment_ID">payment</param>
        public void SetC_Payment_ID(int C_Payment_ID)
        {
            _C_Payment_ID = C_Payment_ID;
        }

        /// <summary>
        /// Get matched Invoice
        /// </summary>
        /// <returns>invoice</returns>
        public int GetC_Invoice_ID()
        {
            return _C_Invoice_ID;
        }

        /// <summary>
        /// Set matched Invoice
        /// </summary>
        /// <param name="C_Invoice_ID">invoice</param>

        public void SetC_Invoice_ID(int C_Invoice_ID)
        {
            _C_Invoice_ID = C_Invoice_ID;
        }
    }
}
