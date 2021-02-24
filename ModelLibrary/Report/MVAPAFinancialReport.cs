/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReport
 * Purpose        : Report Model
 * Class Used     : X_VAPA_FinancialReport
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
    public class MVAPAFinancialReport : X_VAPA_FinancialReport
    {
        /// <summary>
        ///  Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAPA_FinancialReport_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFinancialReport(Ctx ctx, int VAPA_FinancialReport_ID, Trx trxName)
            : base(ctx, VAPA_FinancialReport_ID, trxName)
        {

            if (VAPA_FinancialReport_ID == 0)
            {
                //	setName (null);
                //	setVAPA_FR_RowSet_ID (0);
                //	setVAPA_FR_ColumnSet_ID (0);
                SetListSources(false);
                SetListTrx(false);
            }
            else
            {
                _columnSet = new MVAPAFRColumnSet(ctx, GetVAPA_FR_ColumnSet_ID(), trxName);
                _lineSet = new MVAPAFRRowSet(ctx, GetVAPA_FR_RowSet_ID(), trxName);
            }
        }	//	MReport

        /// <summary>
        /// Load Constructor 
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAPAFinancialReport(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {            
            _columnSet = new MVAPAFRColumnSet(ctx, GetVAPA_FR_ColumnSet_ID(), trxName);
            _lineSet = new MVAPAFRRowSet(ctx, GetVAPA_FR_RowSet_ID(), trxName);
        }	//	MReport

        private MVAPAFRColumnSet _columnSet = null;
        private MVAPAFRRowSet _lineSet = null;

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
            //	VAF_Client indirectly via AcctSchema
            StringBuilder sb = new StringBuilder();
            //	Mandatory 	AcctSchema
            sb.Append("VAB_AccountBook_ID=").Append(GetVAB_AccountBook_ID());
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
            sb.Append(" - VAB_AccountBook_ID=").Append(GetVAB_AccountBook_ID())
                .Append(", VAB_Calender_ID=").Append(GetVAB_Calender_ID());
            sb.Append("]");
            return sb.ToString();
        }	//	toString

       /// <summary>
       /// 	Get Column Set
       /// </summary>
        /// <returns> Column Set</returns>
        public MVAPAFRColumnSet GetColumnSet()
        {
            return _columnSet;
        }

        /// <summary>
        /// Get Line Set
        /// </summary>
        /// <returns>Line Set</returns>
        public MVAPAFRRowSet GetLineSet()
        {
            return _lineSet;
        }

    }	//	MReport

}
