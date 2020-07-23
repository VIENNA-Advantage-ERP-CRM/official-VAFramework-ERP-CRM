/********************************************************
    * Project Name   : VAdvantage
    * Interface Name : BankStatementMatcherInterface
    * Purpose        : Bank Statement Matcher Algorithm Interface
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
//using VAdvantage.Common;
using VAdvantage.Process;
using VAdvantage.Model;
using VAdvantage.DataBase;
using VAdvantage.SqlExec;
using VAdvantage.Utility;
using System.Windows.Forms;
//////using VAdvantage.Controls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using VAdvantage.Logging;

namespace VAdvantage.Interface
{
    public interface BankStatementMatcherInterface
    {
        /// <summary>
        /// Match Bank Statement Line
        /// </summary>
        /// <param name="bsl">bank statement line</param>
        /// <returns>found matches or null</returns>
        BankStatementMatchInfo FindMatch(MBankStatementLine bsl);


        /// <summary>
        /// Match Bank Statement Import Line
        /// </summary>
        /// <param name="ibs">bank statement import line</param>
        /// <returns>found matches or null</returns>
         BankStatementMatchInfo FindMatch(X_I_BankStatement ibs);

    }
}
