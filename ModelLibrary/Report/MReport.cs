/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReport
 * Purpose        : Report Model
 * Class Used     : X_PA_Report
 * Chronological    Development
 * Deepak           18-Jan-2010
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


namespace VAdvantage.Report
{
    public class MReport : X_PA_Report
    {
        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="PA_Report_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MReport(Ctx ctx, int PA_Report_ID, Trx trxName)
            : base(ctx, PA_Report_ID, trxName)
        {

            if (PA_Report_ID == 0)
            {
                //	setName (null);
                //	setPA_ReportLineSet_ID (0);
                //	setPA_ReportColumnSet_ID (0);
                SetListSources(false);
                SetListTrx(false);
            }
            else
            {
                _columnSet = new MReportColumnSet(ctx, GetPA_ReportColumnSet_ID(), trxName);
                _lineSet = new MReportLineSet(ctx, GetPA_ReportLineSet_ID(), trxName);
            }
        }	//	MReport

        /// <summary>
        /// Load Constructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MReport(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {            
            _columnSet = new MReportColumnSet(ctx, GetPA_ReportColumnSet_ID(), trxName);
            _lineSet = new MReportLineSet(ctx, GetPA_ReportLineSet_ID(), trxName);
        }	//	MReport

        private MReportColumnSet _columnSet = null;
        private MReportLineSet _lineSet = null;

        /// <summary>
        ///	List Info
        /// </summary>
        public void List()
    {
        //System.out.println(toString());
            System.Console.WriteLine(ToString());
        if (_columnSet != null)
        {
            _columnSet.List();
        }
        //System.out.println();
        if (_lineSet != null)
        {
            _lineSet.List();
        }
    }	//	dump

        /// <summary>
        ///	Get Where Clause for Report
        /// </summary>
        /// <returns>Where Clause for Report</returns>
        public String GetWhereClause()
        {
            //	AD_Client indirectly via AcctSchema
            StringBuilder sb = new StringBuilder();
            //	Mandatory 	AcctSchema
            sb.Append("C_AcctSchema_ID=").Append(GetC_AcctSchema_ID());
            //
            return sb.ToString();
        }	//	getWhereClause

        /*************************************************************************/

        /// <summary>
        /// String Representation
        /// </summary>
        /// <returns>Info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReport[")
                .Append(Get_ID()).Append(" - ").Append(GetName());
            if (GetDescription() != null)
             sb.Append("(").Append(GetDescription()).Append(")");
            sb.Append(" - C_AcctSchema_ID=").Append(GetC_AcctSchema_ID())
                .Append(", C_Calendar_ID=").Append(GetC_Calendar_ID());
            sb.Append("]");
            return sb.ToString();
        }	//	toString

       /// <summary>
       /// 	Get Column Set
       /// </summary>
        /// <returns> Column Set</returns>
        public MReportColumnSet GetColumnSet()
        {
            return _columnSet;
        }

        /// <summary>
        /// Get Line Set
        /// </summary>
        /// <returns>Line Set</returns>
        public MReportLineSet GetLineSet()
        {
            return _lineSet;
        }

    }	//	MReport

}
