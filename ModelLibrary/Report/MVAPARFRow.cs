/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : MReportLine
 * Purpose        : Report Line Model
 * Class Used     : X_VAPA_FR_Row
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
    public class MVAPARFRow : X_VAPA_FR_Row
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ctx">context </param>
        /// <param name="VAPA_FR_Row_ID">id</param>
        /// <param name="trxName">transaction</param>
        public MVAPARFRow(Ctx ctx, int VAPA_FR_Row_ID, Trx trxName)
            : base(ctx, VAPA_FR_Row_ID, trxName)
        {

            if (VAPA_FR_Row_ID == 0)
            {
                SetSeqNo(0);
                //	setIsSummary (false);		//	not active in DD 
                SetIsPrinted(false);
            }
            else
            {
                LoadSources();
            }
        }	//	MReportLine

        /// <summary>
        ///	Constructor
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="rs">datarow</param>
        /// <param name="trxName">transaction</param>
        public MVAPARFRow(Ctx ctx, DataRow dr, Trx trxName)
            : base(ctx, dr, trxName)
        {

            LoadSources();
        }	//	MReportLine

        /**	Containt Sources				*/
        private MVAPAFVARSource[] _sources = null;
        /** Cache result					*/
        private String _whereClause = null;

        /// <summary>
        ///	Load contained Sources
        /// </summary>
        //private void LoadSources()
        //{
        //    List<MReportSource> list = new List<MReportSource>();
        //    String sql = "SELECT * FROM VAPA_FVAR_Source WHERE VAPA_FR_Row_ID=@param AND IsActive='Y'";
        //    SqlParameter[] param=new SqlParameter[1];
        //    IDataReader idr=null;
        //    DataTable dt = null;
        //    try
        //    {
        //        param[0] = new SqlParameter("@param", GetVAPA_FR_Row_ID());
        //        idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
        //        dt = new DataTable();
        //        dt.Load(idr);
        //        idr.Close();
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new MReportSource(GetCtx(), dr, null));
        //        }
        //        dt = null;
        //    }
        //    catch (Exception e)
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //        }
        //        if (dt != null)
        //        {
        //            dt = null;
        //        }
        //        log.Log(Level.SEVERE, null, e);
        //    }
        //    finally
        //    {
        //        if (idr != null)
        //        {
        //            idr.Close();
        //        }
        //        if (dt != null)
        //        {
        //            dt = null;
        //        }
        //    }
        //            //
        //    _sources = new MReportSource[list.Count];
        //    _sources=list.ToArray();
        //    log.Finest("ID=" + GetVAPA_FR_Row_ID()
        //        + " - Size=" + list.Count);
        //}	//	loadSources



        private void LoadSources()
        {
            List<MVAPAFVARSource> list = new List<MVAPAFVARSource>();
            String sql = "";

            //if (GetFRPT_LinkWith() == FRPT_LINKWITH_LedgerCode)
            //{
                sql = "SELECT * FROM VAPA_FVAR_Source WHERE VAPA_FR_Row_ID=@param AND IsActive='Y'";
            //}
            //else if (GetFRPT_LinkWith() == FRPT_LINKWITH_LedgerGroup)
            //{
            //    sql = "";
            //}
            //else
            //{
            //    log.Warning("ID=" + GetVAPA_FR_Row_ID()
            //            + "Not Link With ledger AND Subgroup");
            //    return;
            //}

            SqlParameter[] param = new SqlParameter[1];
            IDataReader idr = null;
            DataTable dt = null;
            try
            {
                param[0] = new SqlParameter("@param", GetVAPA_FR_Row_ID());
                idr = DataBase.DB.ExecuteReader(sql, param, Get_TrxName());
                dt = new DataTable();
                dt.Load(idr);
                idr.Close();
                foreach (DataRow dr in dt.Rows)
                {
                    list.Add(new MVAPAFVARSource(GetCtx(), dr, null));
                }
                dt = null;
            }
            catch (Exception e)
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
                log.Log(Level.SEVERE, null, e);
            }
            finally
            {
                if (idr != null)
                {
                    idr.Close();
                }
                if (dt != null)
                {
                    dt = null;
                }
            }
            //
            _sources = new MVAPAFVARSource[list.Count];
            _sources = list.ToArray();
            log.Finest("ID=" + GetVAPA_FR_Row_ID()
                + " - Size=" + list.Count);
        }	//	loadSources





        /// <summary>
        ///	Get Sources
        /// </summary>
        /// <returns>sources</returns>
        public MVAPAFVARSource[] GetSources()
        {
            if (LINETYPE_SegmentValue.Equals(GetLineType()))
            {
                return _sources;
            }
            return new MVAPAFVARSource[0];
        }	//	getSources

        /// <summary>
        ///	List Info to System.out
        /// </summary>
        public void List()
        {
            //System.out.println();
            System.Console.WriteLine("- " + ToString());
            if (_sources == null)
            {
                return;
            }
            for (int i = 0; i < _sources.Length; i++)
            {
                System.Console.WriteLine("  - " + _sources[i].ToString());
            }
        }	//	list


        /// <summary>
        ///	Get Source Column Name
        /// </summary>
        /// <returns>Source ColumnName</returns>
        public String GetSourceColumnName()
        {
            String ColumnName = null;
            for (int i = 0; i < _sources.Length; i++)
            {
                String col = MVABAccountBookElement.GetColumnName(_sources[i].GetElementType());
                if (ColumnName == null || ColumnName.Length == 0)
                {
                    ColumnName = col;
                }
                else if (!ColumnName.Equals(col))
                {
                    log.Config("More than one: " + ColumnName + " - " + col);
                    return null;
                }
            }
            return ColumnName;
        }	//	getColumnName

        /// <summary>
        /// Get Value Query for Segment Type
        /// </summary>
        /// <returns>Query for first source element or null</returns>
        public String GetSourceValueQuery()
        {
            if (_sources != null && _sources.Length > 0)
            {
                return MVABAccountBookElement.GetValueQuery(_sources[0].GetElementType());
            }
            return null;
        }	//


        /// <summary>
        ///	Get SQL Select Clause.
        /// </summary>
        /// <param name="withSum">withSum with SUM() function</param>
        /// <returns>select clause - AmtAcctCR+AmtAcctDR/etc or "null" if not defined</returns>
        public String GetSelectClause(Boolean withSum)
        {
            String at = GetAmountType().Substring(0, 1);	//	first letter
            StringBuilder sb = new StringBuilder();
            if (withSum)
            {
                sb.Append("SUM(");
            }
            if (AmountType_Balance.Equals(at))
            {
                //	sb.append("AmtAcctDr-AmtAcctCr");
                sb.Append("acctBalance(Account_ID,AmtAcctDr,AmtAcctCr)");
            }
            else if (AmountType_CR.Equals(at))
            {
                sb.Append("AmtAcctCr");
            }
            else if (AmountType_DR.Equals(at))
            {
                sb.Append("AmtAcctDr");
            }
            else if (AmountType_Qty.Equals(at))
            {
                sb.Append("Qty");
            }
            else
            {
                log.Log(Level.SEVERE, "AmountType=" + GetAmountType() + ", at=" + at);
                return "NULL";
            }
            if (withSum)
            {
                sb.Append(")");
            }
            return sb.ToString();
        }	//	getSelectClause

        /// <summary>
        ///	Is it Period ?
        /// </summary>
        /// <returns>true if Period Amount Type</returns>
        public Boolean IsPeriod()
        {
            String at = GetAmountType();
            if (at == null)
            {
                return false;
            }
            return AMOUNTTYPE_PeriodBalance.Equals(at)
                || AMOUNTTYPE_PeriodCreditOnly.Equals(at)
                || AMOUNTTYPE_PeriodDebitOnly.Equals(at)
                || AMOUNTTYPE_PeriodQuantity.Equals(at);
        }	//	isPeriod

        /// <summary>
        ///	Is it Year ?
        /// </summary>
        /// <returns>true if Year Amount Type</returns>
        public Boolean IsYear()
        {
            String at = GetAmountType();
            if (at == null)
            {
                return false;
            }
            return AMOUNTTYPE_YearBalance.Equals(at)
                || AMOUNTTYPE_YearCreditOnly.Equals(at)
                || AMOUNTTYPE_YearDebitOnly.Equals(at)
                || AMOUNTTYPE_YearQuantity.Equals(at);
        }	//	isYear

        /// <summary>
        ///	Is it Total ?
        /// </summary>
        /// <returns>true if Year Amount Type</returns>
        public Boolean IsTotal()
        {
            String at = GetAmountType();
            if (at == null)
                return false;
            return AMOUNTTYPE_TotalBalance.Equals(at)
                || AMOUNTTYPE_TotalCreditOnly.Equals(at)
                || AMOUNTTYPE_TotalDebitOnly.Equals(at)
                || AMOUNTTYPE_TotalQuantity.Equals(at);
        }	//	isTotal

        /// <summary>
        ///	Get SQL where clause (sources, posting type)
        /// </summary>
        /// <param name="VAPA_FinancialReportingOrder_ID">hierarchy</param>
        /// <returns>where clause</returns>
        public String GetWhereClause(int VAPA_FinancialReportingOrder_ID)
        {
            if (_sources == null)
            {
                return "";
            }
            if (_whereClause == null)
            {
                //	Only one
                if (_sources.Length == 0)
                {
                    _whereClause = "";
                }
                else if (_sources.Length == 1)
                {
                    _whereClause = _sources[0].GetWhereClause(VAPA_FinancialReportingOrder_ID);
                }
                else
                {
                    //	Multiple
                    StringBuilder sb = new StringBuilder("(");
                    for (int i = 0; i < _sources.Length; i++)
                    {
                        if (i > 0)
                        {
                            sb.Append(" OR ");
                        }
                        sb.Append(_sources[i].GetWhereClause(VAPA_FinancialReportingOrder_ID));
                    }
                    sb.Append(")");
                    _whereClause = sb.ToString();
                }
                //	Posting Type
                String PostingType = GetPostingType();
                if (PostingType != null && PostingType.Length > 0)
                {
                    if (_whereClause.Length > 0)
                    {
                        _whereClause += " AND ";
                    }
                    _whereClause += "PostingType='" + PostingType + "'";
                }
                log.Fine(_whereClause);
            }
            return _whereClause;
        }	//	getWhereClause

        /// <summary>
        ///	Has Posting Type
        /// </summary>
        /// <returns>true if posting</returns>
        public Boolean IsPostingType()
        {
            String PostingType = GetPostingType();
            return (PostingType != null && PostingType.Length > 0);
        }	//	isPostingType

        /// <summary>
        ///	String Representation
        /// </summary>
        /// <returns>info</returns>
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder("MReportLine[")
                .Append(Get_ID()).Append(" - ").Append(GetName()).Append(" - ").Append(GetDescription())
                .Append(", SeqNo=").Append(GetSeqNo()).Append(", AmountType=").Append(GetAmountType())
                .Append(" - LineType=").Append(GetLineType());
            if (IsLineTypeCalculation())
            {
                sb.Append(" - Calculation=").Append(GetCalculationType())
                    .Append(" - ").Append(GetOper_1_ID()).Append(" - ").Append(GetOper_2_ID());
            }
            else	//	SegmentValue
            {
                sb.Append(" - SegmentValue - PostingType=").Append(GetPostingType())
                    .Append(", AmountType=").Append(GetAmountType());
            }
            sb.Append("]");
            return sb.ToString();
        }	//	toString

        //	First Letter
        static String AmountType_Balance = "B";
        static String AmountType_CR = "C";
        static String AmountType_DR = "D";
        static String AmountType_Qty = "Q";
        //	Second Letter
        //static  String		AmountType_Period = "P";
        //static  String		AmountType_Year = "Y";
        //static  String		AmountType_Total = "T";

        /// <summary>
        ///	Line Type Calculation
        /// </summary>
        /// <returns>true if calculation</returns>
        public Boolean IsLineTypeCalculation()
        {
            return LINETYPE_Calculation.Equals(GetLineType());
        }
        /// <summary>
        ///	Line Type Segment Value
        /// </summary>
        /// <returns> true if segment value</returns>
        public Boolean IsLineTypeSegmentValue()
        {
            return LINETYPE_SegmentValue.Equals(GetLineType());
        }

        /// <summary>
        ///	Calculation Type Range
        /// </summary>
        /// <returns>true if range</returns>
        public Boolean IsCalculationTypeRange()
        {
            return CALCULATIONTYPE_AddRangeOp1ToOp2.Equals(GetCalculationType());
        }
        /// <summary>
        ///	Calculation Type Add
        /// </summary>
        /// <returns>true if add</returns>
        public Boolean IsCalculationTypeAdd()
        {
            return CALCULATIONTYPE_AddOp1PlusOp2.Equals(GetCalculationType());
        }
        /// <summary>
        ///	Calculation Type Subtract
        /// </summary>
        /// <returns>true if subtract</returns>
        public Boolean IsCalculationTypeSubtract()
        {
            return CALCULATIONTYPE_SubtractOp1_Op2.Equals(GetCalculationType());
        }
        /// <summary>
        ///	Calculation Type Percent
        /// </summary>
        /// <returns>true if percent</returns>
        public Boolean IsCalculationTypePercent()
        {
            return CALCULATIONTYPE_PercentageOp1OfOp2.Equals(GetCalculationType());
        }

        /// <summary>
        /// Before Save
        /// </summary>
        /// <param name="newRecord">new</param>
        /// <returns>true</returns>
        protected override Boolean BeforeSave(Boolean newRecord)
        {
            if (LINETYPE_SegmentValue.Equals(GetLineType()))
            {
                if (GetCalculationType() != null)
                {
                    SetCalculationType(null);
                }
                if (GetOper_1_ID() != 0)
                {
                    SetOper_1_ID(0);
                }
                if (GetOper_2_ID() != 0)
                {
                    SetOper_2_ID(0);
                }
            }
            return true;
        }	//	beforeSave


        /// <summary>
        ///	Copy
        /// </summary>
        /// <param name="ctx">context</param>
        /// <param name="VAF_Client_ID">parent</param>
        /// <param name="VAF_Org_ID">parent</param>
        /// <param name="VAPA_FR_RowSet_ID">parent</param>
        /// <param name="source">copy source</param>
        /// <param name="trxName">transaction</param>
        /// <returns>report line</returns>
        public static MVAPARFRow Copy(Ctx ctx, int VAF_Client_ID, int VAF_Org_ID,
            int VAPA_FR_RowSet_ID, MVAPARFRow source, Trx trxName)
        {
            MVAPARFRow retValue = new MVAPARFRow(ctx, 0, trxName);
            MVAPARFRow.CopyValues(source, retValue, VAF_Client_ID, VAF_Org_ID);
            //
            retValue.SetVAPA_FR_RowSet_ID(VAPA_FR_RowSet_ID);
            retValue.SetOper_1_ID(0);
            retValue.SetOper_2_ID(0);
            return retValue;
        }	//	copy


    }	//	MReportLine

}
