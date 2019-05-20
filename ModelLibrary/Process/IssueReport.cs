/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : IssueReport
 * Purpose        : Report System Issue
 * Class Used     : ProcessEngine.SvrProcess
 * Chronological    Development
 * Deepak           28-Jan-2010
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
using VAdvantage.Print;
using System.Drawing;
using javax.swing;
using System.Security.Policy;


using System.Net;
using System.IO;

using VAdvantage.ProcessEngine;namespace VAdvantage.Process
{
    public class IssueReport : ProcessEngine.SvrProcess
    {
        /**	Issue to report			*/
        private int _AD_Issue_ID = 0;

        /// <summary>
        /// perpare
        /// </summary>
        protected override void Prepare()
        {
            _AD_Issue_ID = GetRecord_ID();
        }	//	prepare

        /// <summary>
        /// Doit
        /// </summary>
        /// <returns>message</returns>
        protected override String DoIt()
        {
            log.Info("AD_Issue_ID=" + _AD_Issue_ID);
            if (!MSystem.Get(GetCtx()).IsAutoErrorReport())
            {
                return "NOT reported - Enable Error Reporting in Window System";
            }
            //
            MIssue issue = new MIssue(GetCtx(), _AD_Issue_ID, Get_TrxName());
            if (issue.Get_ID() == 0)
            {
                return "No Issue to report - ID=" + _AD_Issue_ID;
            }
            //
            String error = issue.Report();
            if (error != null)
            {
                throw new SystemException(error);
            }
            if (issue.Save())
                return "Issue Reported: " + issue.GetRequestDocumentNo();
            throw new SystemException("Issue Not Saved");
        }	//	doIt

    }

   
}
