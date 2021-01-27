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
//using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class BankStatementMatchInfo
    {
        #region
        private int _VAB_BusinessPartner_ID = 0;
        private int _VAB_Payment_ID = 0;
        private int _VAB_Invoice_ID = 0;
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
            return _VAB_BusinessPartner_ID > 0 || _VAB_Payment_ID > 0 || _VAB_Invoice_ID > 0;
        }

        /// <summary>
        /// Get matched BPartner
        /// </summary>
        /// <returns>BPartner</returns>
        public int GetVAB_BusinessPartner_ID()
        {
            return _VAB_BusinessPartner_ID;
        }

        /// <summary>
        /// Set matched BPartner
        /// </summary>
        /// <param name="VAB_BusinessPartner_ID">BPartner</param>
        public void SetVAB_BusinessPartner_ID(int VAB_BusinessPartner_ID)
        {
            _VAB_BusinessPartner_ID = VAB_BusinessPartner_ID;
        }

        /// <summary>
        /// Get matched Payment
        /// </summary>
        /// <returns>Payment</returns>
        public int GetVAB_Payment_ID()
        {
            return _VAB_Payment_ID;
        }

        /// <summary>
        /// Set matched Payment
        /// </summary>
        /// <param name="VAB_Payment_ID">payment</param>
        public void SetVAB_Payment_ID(int VAB_Payment_ID)
        {
            _VAB_Payment_ID = VAB_Payment_ID;
        }

        /// <summary>
        /// Get matched Invoice
        /// </summary>
        /// <returns>invoice</returns>
        public int GetVAB_Invoice_ID()
        {
            return _VAB_Invoice_ID;
        }

        /// <summary>
        /// Set matched Invoice
        /// </summary>
        /// <param name="VAB_Invoice_ID">invoice</param>

        public void SetVAB_Invoice_ID(int VAB_Invoice_ID)
        {
            _VAB_Invoice_ID = VAB_Invoice_ID;
        }
    }
}
