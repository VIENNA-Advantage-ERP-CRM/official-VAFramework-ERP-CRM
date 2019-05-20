/********************************************************
 * Project Name   : VAdvantage
 * Class Name     : FinReport
 * Purpose        : Financial Report Engine
 * Class Used     : Svrprocess
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
using VAdvantage.Print;
using VAdvantage.ProcessEngine;

namespace VAdvantage.Report
{
    public class FinReport:ProcessEngine.SvrProcess
    {
    /**	Period Parameter				*/
    private int					_C_Period_ID = 0;
    /**	Org Parameter					*/
    private int					_Org_ID = 0;
    /**	BPartner Parameter				*/
    private int					_C_BPartner_ID = 0;
    /**	Product Parameter				*/
    private int					_M_Product_ID = 0;
    /**	Project Parameter				*/
    private int					_C_Project_ID = 0;
    /**	Activity Parameter				*/
    private int					_C_Activity_ID = 0;
    /**	SalesRegion Parameter			*/
    private int					_C_SalesRegion_ID = 0;
    /**	Campaign Parameter				*/
    private int					_C_Campaign_ID = 0;
    /** Update Balances Parameter		*/
    private bool				_UpdateBalances = true;
    /** Details before Lines			*/
    private bool				_DetailsSourceFirst = false;
    /** Hierarchy						*/
    private int					_PA_Hierarchy_ID = 0;

    /**	Start Time						*/
    private long 				_start =CommonFunctions.CurrentTimeMillis();//  System.currentTimeMillis();

    /**	Report Definition				*/
    private MReport				_report = null;
    /**	Periods in Calendar				*/
    private FinReportPeriod[]	_periods = null;
    /**	Index of m_C_Period_ID in _periods		**/
    private int					_reportPeriod = -1;
    /**	Parameter Where Clause			*/
    private StringBuilder		m_parameterWhere = new StringBuilder();
    /**	The Report Columns				*/
    private MReportColumn[] 	_columns;
    /** The Report Lines				*/
    private MReportLine[] 		_lines;
    /** Balance Aggregation             */
    private int p_Fact_Accumulation_ID;

    /// <summary>
    /// Prepare - e.g., get Parameters.
    /// </summary>
    protected override void Prepare()
    {
        StringBuilder sb = new StringBuilder ("Record_ID=")
            .Append(GetRecord_ID());
        //	Parameter
        ProcessInfoParameter[] para = GetParameter();
        for (int i = 0; i < para.Length; i++)
        {
            String name = para[i].GetParameterName();
            if (para[i].GetParameter() == null)
            {
                ;
            }
            else if (name.Equals("C_Period_ID"))
            {
                _C_Period_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("PA_Hierarchy_ID"))
            {
                _PA_Hierarchy_ID = para[i].GetParameterAsInt();
            }
            else if (name.Equals("Org_ID"))
            {
                _Org_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
            }
            else if (name.Equals("C_BPartner_ID"))
            {
                _C_BPartner_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("M_Product_ID"))
            {
                _M_Product_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("C_Project_ID"))
            {
                _C_Project_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("C_Activity_ID"))
            {
                _C_Activity_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("C_SalesRegion_ID"))
            {
                _C_SalesRegion_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("C_Campaign_ID"))
            {
                _C_Campaign_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());//.intValue();
            }
            else if (name.Equals("UpdateBalances"))
            {
                _UpdateBalances = "Y".Equals(para[i].GetParameter());
            }
            else if (name.Equals("DetailsSourceFirst"))
            {
                _DetailsSourceFirst = "Y".Equals(para[i].GetParameter());
            }
            else if (name.Equals("Fact_Accumulation_ID"))
            {
                p_Fact_Accumulation_ID = Util.GetValueOfInt(para[i].GetParameter());
            }
            else
            {
                log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }
        //	Optional Org
        if (_Org_ID != 0 && _Org_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Organization, _Org_ID));
        }
        //	Optional BPartner
        if (_C_BPartner_ID != 0 && _C_BPartner_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_BPartner, _C_BPartner_ID));
        }
        //	Optional Product
        if (_M_Product_ID != 0 && _M_Product_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Product, _M_Product_ID));
        }
        //	Optional Project
        if (_C_Project_ID != 0 && _C_Project_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Project, _C_Project_ID));
        }
        //	Optional Activity
        if (_C_Activity_ID != 0 && _C_Activity_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Activity, _C_Activity_ID));
        }
        //	Optional Campaign
        if (_C_Campaign_ID != 0 && _C_Campaign_ID!=-1)
        {
            m_parameterWhere.Append(" AND C_Campaign_ID=").Append(_C_Campaign_ID);
        }
        //	m_parameterWhere.Append(" AND ").Append(MReportTree.getWhereClause(getCtx(), 
        //		MAcctSchemaElement.ELEMENTTYPE_Campaign, _C_Campaign_ID));
        //	Optional Sales Region
        if (_C_SalesRegion_ID != 0 && _C_SalesRegion_ID!=-1)
        {
            m_parameterWhere.Append(" AND ").Append(MReportTree.GetWhereClause(GetCtx(), 
                _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_SalesRegion, _C_SalesRegion_ID));
        }

        //	Load Report Definition
        _report = new MReport (GetCtx(), GetRecord_ID(), null);
        sb.Append(" - ").Append(_report);
        //
        SetPeriods();
        sb.Append(" - C_Period_ID=").Append(_C_Period_ID)
            .Append(" - ").Append(m_parameterWhere);
        //
        log.Info(sb.ToString());
    //	_report.list();
    }	//	prepare

    /// <summary>
    ///	Set Periods
    /// </summary>
    private void SetPeriods()
    {
        log.Info("C_Calendar_ID=" + _report.GetC_Calendar_ID());
        DateTime today = TimeUtil.GetDay(DateTime.Now);// TimeUtil.getDay(System.currentTimeMillis());
        List<FinReportPeriod> list = new List<FinReportPeriod>();

        String sql = "SELECT p.C_Period_ID, p.Name, p.StartDate, p.EndDate, MIN(p1.StartDate) "
            + "FROM C_Period p "
            + " INNER JOIN C_Year y ON (p.C_Year_ID=y.C_Year_ID),"
            + " C_Period p1 "
            + "WHERE y.C_Calendar_ID=@param"
            + " AND p.PeriodType='S' "
            + " AND p1.C_Year_ID=y.C_Year_ID AND p1.PeriodType='S' "
            + "GROUP BY p.C_Period_ID, p.Name, p.StartDate, p.EndDate "
            + "ORDER BY p.StartDate";
        SqlParameter[] param=new SqlParameter[1];
      IDataReader idr=null;
        DataTable dt=null;
        try
        {
            //pstmt = DataBase.prepareStatement(sql, null);
            //pstmt.setInt(1, _report.getC_Calendar_ID());
            param[0] = new SqlParameter("@param", _report.GetC_Calendar_ID());
            idr=DataBase.DB.ExecuteReader(sql,param,null);
            dt=new DataTable();
            dt.Load(idr);
            idr.Close();
            foreach(DataRow dr in dt.Rows)
            {
                FinReportPeriod frp = new FinReportPeriod (Utility.Util.GetValueOfInt(dr[0]),Utility.Util.GetValueOfString(dr[1]),
                Utility.Util.GetValueOfDateTime(dr[2]),Utility.Util.GetValueOfDateTime(dr[3]),Utility.Util.GetValueOfDateTime(dr[4]));
                list.Add(frp);
                if (_C_Period_ID == 0 || _C_Period_ID==-1 && frp.InPeriod(today))
                {
                    _C_Period_ID = frp.GetC_Period_ID();
                }
            }
            dt=null;
        }
        catch (Exception e)
        {
            if(idr!=null)
            {
                idr.Close();
            }
             if(dt!=null)
            {
                dt=null;
            }
            log.Log(Level.SEVERE, sql, e);
        }
        finally
        {
            if(idr!=null)
            {
                idr.Close();
            }
             if(dt!=null)
            {
                dt=null;
            }
        }
        //	convert to Array
        _periods = new FinReportPeriod[list.Count];
        _periods=list.ToArray();
        //	today after latest period
        if (_C_Period_ID == 0 || _C_Period_ID==-1)
        {
            _reportPeriod = _periods.Length - 1;
            _C_Period_ID = Utility.Util.GetValueOfInt(_periods[_reportPeriod].GetC_Period_ID ());
        }
    }	//	setPeriods


    /// <summary>
    /// Perform Process.
    /// </summary>
    /// <returns>Message to be translated</returns>
    protected override String DoIt()
    {
        log.Info("AD_PInstance_ID=" + GetAD_PInstance_ID());
        //	** Create Temporary and empty Report Lines from PA_ReportLine
        //	- AD_PInstance_ID, PA_ReportLine_ID, 0, 0
        int PA_ReportLineSet_ID = _report.GetLineSet().GetPA_ReportLineSet_ID();
        StringBuilder sql = new StringBuilder ("INSERT INTO T_Report "
            + "(AD_PInstance_ID, PA_ReportLine_ID, Record_ID,Fact_Acct_ID, SeqNo,LevelNo, Name,Description) "
            + "SELECT ").Append(GetAD_PInstance_ID()).Append(", PA_ReportLine_ID, 0,0, SeqNo,0, Name,Description "
            + "FROM PA_ReportLine "
            + "WHERE IsActive='Y' AND PA_ReportLineSet_ID=").Append(PA_ReportLineSet_ID);

        int no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
        log.Fine("Report Lines = " + no);

        //	Update AcctSchema Balances
        if (_UpdateBalances)
        {
           // FinBalance.UpdateBalance(_report.GetC_AcctSchema_ID(), false);
            FinBalance.UpdateBalance(GetCtx(),
               _report.GetC_AcctSchema_ID(), null, Get_TrxName(), p_Fact_Accumulation_ID, this);
        }

        //	** Get Data	** Segment Values
        _columns = _report.GetColumnSet().GetColumns();
        if (_columns.Length == 0)
        {
            throw new Exception("@No@ @PA_ReportColumn_ID@");
        }
        _lines = _report.GetLineSet().GetLiness();
        if (_lines.Length == 0)
        {
            throw new Exception("@No@ @PA_ReportLine_ID@");
        }
        //	for all lines
        for (int line = 0; line < _lines.Length; line++)
        {
            //	Line Segment Value (i.e. not calculation)
            if (_lines[line].IsLineTypeSegmentValue())
            {
                InsertLine(line);
            }
        }	//	for all lines

        InsertLineDetail();		//	also clean up
        DoCalculations();

        DeleteUnprintedLines();

        //	Create Report
        if (Ini.IsClient())
        {
            GetProcessInfo().SetTransientObject(GetPrintFormat());
        }
        else
        {
            GetProcessInfo().SetSerializableObject(GetPrintFormat());
        }
        log.Fine((CommonFunctions.CurrentTimeMillis() - _start) + " ms");
        return "";
    }	//	doIt


    /// <summary>
    ///	For all columns (in a line) with relative period access
    /// </summary>
    /// <param name="line">line</param>
    private void InsertLine (int? line)
    {
        log.Info("" + _lines[line.Value]);

        //	No source lines - Headings
        if (_lines[line.Value] == null || _lines[line.Value].GetSources().Length == 0)
        {
            log.Warning ("No Source lines: " + _lines[line.Value]);
            return;
        }

        StringBuilder update = new StringBuilder();
        //	for all columns
        for (int col = 0; col < _columns.Length; col++)
        {
            //	Ignore calculation columns
            if (_columns[col].IsColumnTypeCalculation())
            {
                continue;
            }
            StringBuilder info = new StringBuilder();
            info.Append("Line=").Append(line).Append(",Col=").Append(col);

            //	SELECT SUM()
            StringBuilder select = new StringBuilder ("SELECT ");
            if (_lines[line.Value].GetAmountType() != null)				//	line amount type overwrites column
            {
                String sql = _lines[line.Value].GetSelectClause (true);
                select.Append (sql);
                info.Append(": LineAmtType=").Append(_lines[line.Value].GetAmountType());
            }
            else if (_columns[col].GetAmountType() != null)
            {
                String sql = _columns[col].GetSelectClause (true);
                select.Append (sql);
                info.Append(": ColumnAmtType=").Append(_columns[col].GetAmountType());
            }
            else
            {
                log.Warning("No Amount Type in line: " + _lines[line.Value] + " or column: " + _columns[col]);
                continue;
            }

            //	Get Period/Date info
            select.Append(" FROM Fact_Acct_Balance WHERE DateAcct ");
            Decimal? relativeOffset = null;	//	current
            if (_columns[col].IsColumnTypeRelativePeriod())
            {
                relativeOffset = _columns[col].GetRelativePeriod();
            }
            FinReportPeriod frp = GetPeriod (relativeOffset);
            if (_lines[line.Value].GetAmountType() != null)			//	line amount type overwrites column
            {
                info.Append(" - LineDateAcct=");
                if (_lines[line.Value].IsPeriod())
                {
                    String sql = frp.GetPeriodWhere();
                    info.Append("Period");
                    select.Append(sql);
                }
                else if (_lines[line.Value].IsYear())
                {
                    String sql = frp.GetYearWhere();
                    info.Append("Year");
                    select.Append(sql);
                }
                else if (_lines[line.Value].IsTotal())
                {
                    String sql = frp.GetTotalWhere();
                    info.Append("Total");
                    select.Append(sql);
                }
                else
                {
                    log.Log(Level.SEVERE, "No valid Line AmountType");
                    select.Append("=0");	// valid sql	
                }
            }
            else if (_columns[col].GetAmountType() != null)
            {
                info.Append(" - ColumnDateAcct=");
                if (_columns[col].IsPeriod())
                {
                    String sql = frp.GetPeriodWhere();
                    info.Append("Period");
                    select.Append(sql);
                }
                else if (_columns[col].IsYear())
                {
                    String sql = frp.GetYearWhere();
                    info.Append("Year");
                    select.Append(sql);
                }
                else if (_columns[col].IsTotal())
                {
                    String sql = frp.GetTotalWhere();
                    info.Append("Total");
                    select.Append(sql);
                }
                else
                {
                    log.Log(Level.SEVERE, "No valid Column AmountType");
                    select.Append("=0");	// valid sql	
                }
            }

            //	Line Where
            String s = _lines[line.Value].GetWhereClause(_PA_Hierarchy_ID);	//	(sources, posting type)
            if (s != null && s.Length > 0)
                select.Append(" AND ").Append(s);

            //	Report Where
            s = _report.GetWhereClause();
            if (s != null && s.Length > 0)
                select.Append(" AND ").Append(s);

            //	PostingType
            if (!_lines[line.Value].IsPostingType())		//	only if not defined on line
            {
                String PostingType = _columns[col].GetPostingType();
                if (PostingType != null && PostingType.Length > 0)
                    select.Append(" AND PostingType='").Append(PostingType).Append("'");
            }

            if (_columns[col].IsColumnTypeSegmentValue())
                select.Append(_columns[col].GetWhereClause(_PA_Hierarchy_ID));

            //	Parameter Where
            select.Append(m_parameterWhere);
            log.Finest("Line=" + line + ",Col=" + line + ": " + select);

            //	Update SET portion
            if (update.Length > 0)
                update.Append(", ");
            update.Append("Col_").Append(col)
                .Append(" = (").Append(select).Append(")");
            //
            log.Finest(info.ToString());
        }
        //	Update Line Values
        if (update.Length> 0)
        {
            update.Insert (0, "UPDATE T_Report SET ");
            update.Append(" WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                .Append(" AND PA_ReportLine_ID=").Append(_lines[line.Value].GetPA_ReportLine_ID())
                .Append(" AND ABS(LevelNo)<2");		//	0=Line 1=Acct
            int no = DataBase.DB.ExecuteQuery(update.ToString(),null, Get_TrxName());
            if (no != 1)
            {
                log.Log(Level.SEVERE, "#=" + no + " for " + update);
            }
            log.Finest(update.ToString());
        }
    }	//	insertLine


    /// <summary>
    ///	Line + Column calculation
    /// </summary>
    private void DoCalculations()
    {
        //	for all lines	***************************************************
        for (int line = 0; line < _lines.Length; line++)
        {
            if (!_lines[line].IsLineTypeCalculation())
            {
                continue;
            }
            int oper_1 = _lines[line].GetOper_1_ID();
            int oper_2 = _lines[line].GetOper_2_ID();

            log.Fine("Line " + line + " = #" + oper_1 + " "
                + _lines[line].GetCalculationType() + " #" + oper_2);

            //	Adding
            if (_lines[line].IsCalculationTypeAdd()
                || _lines[line].IsCalculationTypeRange())
            {
                //	Reverse range
                if (oper_1 > oper_2)
                {
                    int temp = oper_1;
                    oper_1 = oper_2;
                    oper_2 = temp;
                }
                StringBuilder sb = new StringBuilder("UPDATE T_Report SET ");
                for (int col = 0; col < _columns.Length; col++)
                {
                    //if (col > 0)
                       // sb.Append(",");
                    sb.Append("Col_").Append(col);
                    sb.Append(" = (SELECT ");
                //for (int col = 0; col < _columns.Length; col++)
                //{
                   // if (col > 0)
                        //jz for update sql translating sb.Append(",");
                      //  sb.Append(", ");
                    sb.Append("COALESCE(SUM(Col_").Append(col).Append("),0)");
                //}
                    sb.Append(" FROM T_Report WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                    .Append(" AND PA_ReportLine_ID IN (");
                if (_lines[line].IsCalculationTypeAdd())
                {
                    sb.Append(oper_1).Append(",").Append(oper_2);
                }
                else
                {
                    sb.Append(GetLineIDs(oper_1, oper_2));		//	list of columns to add up
                }
                sb.Append(") AND ABS(LevelNo)<1)");
                //if (col > 0)
                    if(col < (_columns.Length-1))
                {
                    sb.Append(",");
                }
                }
                	//	0=Line 1=Acct

                    sb.Append(" WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                    .Append(" AND PA_ReportLine_ID=").Append(_lines[line].GetPA_ReportLine_ID())
                    .Append(" AND ABS(LevelNo)<1");		//	not trx
                int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
                if (no != 1)
                {
                    log.Log(Level.SEVERE, "(+) #=" + no + " for " + _lines[line] + " - " + sb.ToString());
                }
                else
                {
                    log.Fine("(+) Line=" + line + " - " + _lines[line]);
                    log.Finest("(+) " + sb.ToString());
                }
            }
            else	//	No Add (subtract, percent)
            {
                //	Step 1 - get First Value or 0 in there
                StringBuilder sb = new StringBuilder("UPDATE T_Report SET ");
                for (int col = 0; col < _columns.Length; col++)
                {
                  // if (col > 0)
                    //{
                      //  sb.Append(",");
                    //}
                    sb.Append("Col_").Append(col);
                    sb.Append(" = (SELECT ");
                    sb.Append("COALESCE(r2.Col_").Append(col).Append(",0)");
                    sb.Append(" FROM T_Report r2 WHERE r2.AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                    .Append(" AND r2.PA_ReportLine_ID=").Append(oper_1)
                    .Append(" AND r2.Record_ID=0 AND r2.Fact_Acct_ID=0)");
                    if (col < (_columns.Length - 1))
                    {
                        sb.Append(",");
                    }
                
                }
               
                //for (int col = 0; col < _columns.Length; col++)
                //{
                  //  if (col > 0)
                    //{
                      //  sb.Append(", "); //jz ", "
                   // }
                    
                //}
                    //
                    sb.Append(" WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                       .Append(" AND PA_ReportLine_ID=").Append(_lines[line].GetPA_ReportLine_ID())
                    .Append(" AND ABS(LevelNo)<1");			//	0=Line 1=Acct
                int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
                if (no != 1)
                {
                    log.Severe("(x) #=" + no + " for " + _lines[line] + " - " + sb.ToString());
                    continue;
                }

                //	Step 2 - do Calculation with Second Value
                sb = new StringBuilder("UPDATE T_Report r1 SET ");
                for (int col = 0; col < _columns.Length; col++)
                {
                    //if (col > 0)
                    //{
                      //  sb.Append(",");
                    //}
                    sb.Append("Col_").Append(col);
                    sb.Append(" = (SELECT ");
                    sb.Append("COALESCE(r1.Col_").Append(col).Append(",0)");
                    if (_lines[line].IsCalculationTypeSubtract())
                    {
                        sb.Append(" - ");
                    }
                    else
                    {
                        sb.Append("/");
                    }
                    sb.Append("COALESCE(r2.Col_").Append(col).Append(",0.000000001)");
                    if (_lines[line].IsCalculationTypePercent())
                    {
                        sb.Append(" *100");
                    }
                     sb.Append(" FROM T_Report r2 WHERE r2.AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                    .Append(" AND r2.PA_ReportLine_ID=").Append(oper_2)
                    .Append(" AND r2.Record_ID=0 AND r2.Fact_Acct_ID=0)");
                     if (col < (_columns.Length - 1))
                     {
                         sb.Append(",");
                     }
                }
               
                //for (int col = 0; col < _columns.Length; col++)
                //{
                  //  if (col > 0)
                    //    sb.Append(",  ");//jz hard coded ", "
                   
                //}
               
                    //
                    sb.Append( "WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                       .Append(" AND PA_ReportLine_ID=").Append(_lines[line].GetPA_ReportLine_ID())
                    .Append(" AND ABS(LevelNo)<1");			//	0=Line 1=Acct
                no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
                if (no != 1)
                {
                    log.Severe("(x) #=" + no + " for " + _lines[line] + " - " + sb.ToString());
                }
                else
                {
                    log.Fine("(x) Line=" + line + " - " + _lines[line]);
                    log.Finest(sb.ToString());
                }
            }
        }	//	for all lines


        //	for all columns		***********************************************
        for (int col = 0; col < _columns.Length; col++)
        {
            //	Only Calculations
            if (!_columns[col].IsColumnTypeCalculation())
                continue;

            StringBuilder sb = new StringBuilder("UPDATE T_Report SET ");
            //	Column to set
            sb.Append("Col_").Append(col).Append("=");
            //	First Operand
            int ii_1 = GetColumnIndex(_columns[col].GetOper_1_ID());
            if (ii_1 < 0)
            {
                log.Log(Level.SEVERE, "Column Index for Operator 1 not found - " + _columns[col]);
                continue;
            }
            //	Second Operand
            int ii_2 = GetColumnIndex(_columns[col].GetOper_2_ID());
            if (ii_2 < 0)
            {
                log.Log(Level.SEVERE, "Column Index for Operator 2 not found - " + _columns[col]);
                continue;
            }
            log.Fine("Column " + col + " = #" + ii_1 + " "
                + _columns[col].GetCalculationType() + " #" + ii_2);
            //	Reverse Range
            if (ii_1 > ii_2 && _columns[col].IsCalculationTypeRange())
            {
                log.Fine("Swap operands from " + ii_1 + " op " + ii_2);
                int temp = ii_1;
                ii_1 = ii_2;
                ii_2 = temp;
            }

            //	+
            if (_columns[col].IsCalculationTypeAdd())
                sb.Append("COALESCE(Col_").Append(ii_1).Append(",0)")
                    .Append("+")
                    .Append("COALESCE(Col_").Append(ii_2).Append(",0)");
            //	-
            else if (_columns[col].IsCalculationTypeSubtract())
                sb.Append("COALESCE(Col_").Append(ii_1).Append(",0)")
                    .Append("-")
                    .Append("COALESCE(Col_").Append(ii_2).Append(",0)");
            //	/
            if (_columns[col].IsCalculationTypePercent())
                sb.Append("CASE WHEN COALESCE(Col_").Append(ii_2)
                    .Append(",0)=0 THEN NULL ELSE ")
                    .Append("COALESCE(Col_").Append(ii_1).Append(",0)")
                    .Append("/")
                    .Append("Col_").Append(ii_2)
                    .Append("*100 END");	//	Zero Divide
            //	Range
            else if (_columns[col].IsCalculationTypeRange())
            {
                sb.Append("COALESCE(Col_").Append(ii_1).Append(",0)");
                for (int ii = ii_1 + 1; ii <= ii_2; ii++)
                    sb.Append("+COALESCE(Col_").Append(ii).Append(",0)");
            }
            //
            sb.Append(" WHERE AD_PInstance_ID=").Append(GetAD_PInstance_ID())
                .Append(" AND ABS(LevelNo)<2");			//	0=Line 1=Acct
            int no = DataBase.DB.ExecuteQuery(sb.ToString(),null, Get_TrxName());
            if (no < 1)
                log.Severe("#=" + no + " for " + _columns[col]
                    + " - " + sb.ToString());
            else
            {
                log.Fine("Col=" + col + " - " + _columns[col]);
                log.Finest(sb.ToString());
            }
        } 	//	for all columns

    }	//	doCalculations
    /// <summary>
    ///	Get List of PA_ReportLine_ID from .. to
    /// </summary>
    /// <param name="fromID">from id</param>
    /// <param name="toID">to id</param>
    /// <returns>comma separated list</returns>
    private String GetLineIDs (int fromID, int toID)
    {
        log.Finest("From=" + fromID + " To=" + toID);
        StringBuilder sb = new StringBuilder();
        sb.Append(fromID);
        bool addToList = false;
        for (int line = 0; line < _lines.Length; line++)
        {
            int PA_ReportLine_ID = _lines[line].GetPA_ReportLine_ID();
            log.Finest("Add=" + addToList 
                + " ID=" + PA_ReportLine_ID + " - " + _lines[line]);
            if (addToList)
            {
                sb.Append (",").Append (PA_ReportLine_ID);
                if (PA_ReportLine_ID == toID)		//	done
                    break;
            }
            else if (PA_ReportLine_ID == fromID)	//	from already added
                addToList = true;
        }
        return sb.ToString();
    }	//	getLineIDs

    /// <summary>
    ///	Get Column Index
    /// </summary>
    /// <param name="PA_ReportColumn_ID">id</param>
    /// <returns>zero based index or if not found</returns>
    private int GetColumnIndex (int PA_ReportColumn_ID)
    {
        for (int i = 0; i < _columns.Length; i++)
        {
            if (_columns[i].GetPA_ReportColumn_ID() == PA_ReportColumn_ID)
                return i;
        }
        return -1;
    }	//	getColumnIndex


    /// <summary>
    ///	Get Financial Reporting Period based on reportong Period and offset.
    /// </summary>
    /// <param name="relativeOffset">offset</param>
    /// <returns> reporting period</returns>
    private FinReportPeriod GetPeriod (Decimal? relativeOffset)
    {
        if (relativeOffset == null)
            return GetPeriod(0);
        return GetPeriod(Utility.Util.GetValueOfInt(relativeOffset));
    }	//	getPeriod

    /// <summary>
    ///	Get Financial Reporting Period based on reporting Period and offset.
    /// </summary>
    /// <param name="relativeOffset">offset</param>
    /// <returns>reporting period</returns>
    private FinReportPeriod GetPeriod (int relativeOffset)
    {
        //	find current reporting period C_Period_ID
        if (_reportPeriod < 0)
        {
            for (int i = 0; i < _periods.Length; i++)
            {
                if (_C_Period_ID == _periods[i].GetC_Period_ID())
                {
                    _reportPeriod = i;
                    break;
                }
            }
        }
        if (_reportPeriod < 0 || _reportPeriod >= _periods.Length)
        {
            throw new Exception("Period index not found - ReportPeriod="
                + _reportPeriod + ", C_Period_ID=" + _C_Period_ID);
        }

        //	Bounds check
        int index = _reportPeriod + relativeOffset;
        if (index < 0)
        {
            log.Log(Level.SEVERE, "Relative Offset(" + relativeOffset 
                + ") not valid for selected Period(" + _reportPeriod + ")");
            index = 0;
        }
        else if (index >= _periods.Length)
        {
            log.Log(Level.SEVERE, "Relative Offset(" + relativeOffset 
                + ") not valid for selected Period(" + _reportPeriod + ")");
            index = _periods.Length - 1;
        }
        //	Get Period
        return _periods[index];
    }	//	getPeriod


    /// <summary>
    ///	Insert Detail Lines if enabled
    /// </summary>
    private void InsertLineDetail()
    {
        if (!_report.IsListSources())
            return;
        log.Info("");

        //	for all source lines
        for (int line = 0; line < _lines.Length; line++)
        {
            //	Line Segment Value (i.e. not calculation)
            if (_lines[line].IsLineTypeSegmentValue ())
                InsertLineSource (line);
        }

        //	Clean up empty rows
        StringBuilder sql = new StringBuilder ("DELETE FROM T_Report WHERE LevelNo<>0")
            .Append(" AND Col_0 IS NULL AND Col_1 IS NULL AND Col_2 IS NULL AND Col_3 IS NULL AND Col_4 IS NULL AND Col_5 IS NULL AND Col_6 IS NULL AND Col_7 IS NULL AND Col_8 IS NULL AND Col_9 IS NULL")
            .Append(" AND Col_10 IS NULL AND Col_11 IS NULL AND Col_12 IS NULL AND Col_13 IS NULL AND Col_14 IS NULL AND Col_15 IS NULL AND Col_16 IS NULL AND Col_17 IS NULL AND Col_18 IS NULL AND Col_19 IS NULL AND Col_20 IS NULL"); 
        int no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
        log.Fine("Deleted empty #=" + no);

        //	Set SeqNo
        sql = new StringBuilder ("UPDATE T_Report r1 "
            + "SET SeqNo = (SELECT SeqNo "
                + "FROM T_Report r2 "
                + "WHERE r1.AD_PInstance_ID=r2.AD_PInstance_ID AND r1.PA_ReportLine_ID=r2.PA_ReportLine_ID"
                + " AND r2.Record_ID=0 AND r2.Fact_Acct_ID=0)"
            + "WHERE SeqNo IS NULL");
        no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
        log.Fine("SeqNo #=" + no);

        if (!_report.IsListTrx())
            return;

        //	Set Name,Description
        String sql_select = "SELECT e.Name, fa.Description "
            + "FROM Fact_Acct fa"
            + " INNER JOIN AD_Table t ON (fa.AD_Table_ID=t.AD_Table_ID)"
            + " INNER JOIN AD_Element e ON (t.TableName||'_ID'=e.ColumnName) "
            + "WHERE r.Fact_Acct_ID=fa.Fact_Acct_ID";
        //	Translated Version ...
        sql = new StringBuilder ("UPDATE T_Report r SET (Name,Description)=(")
            .Append(sql_select).Append(") "
            + "WHERE Fact_Acct_ID <> 0 AND AD_PInstance_ID=")
            .Append(GetAD_PInstance_ID());
        no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
        if (VLogMgt.IsLevelFinest())
            log.Fine("Trx Name #=" + no + " - " + sql.ToString());
    }	//	insertLineDetail

    /// <summary>
    /// Insert Detail Line per Source.	For all columns (in a line) with relative period access
     // 	- AD_PInstance_ID, PA_ReportLine_ID, variable, 0 - Level 1
    /// </summary>
    /// <param name="line"> line</param>
    private void InsertLineSource (int line)
    {
        log.Info("Line=" + line + " - " + _lines[line]);

        //	No source lines
        if (_lines[line] == null || _lines[line].GetSources().Length == 0)
            return;
        String variable = _lines[line].GetSourceColumnName();
        if (variable == null)
            return;
        log.Fine("Variable=" + variable);


        //	Insert
        StringBuilder insert = new StringBuilder("INSERT INTO T_Report "
            + "(AD_PInstance_ID, PA_ReportLine_ID, Record_ID,Fact_Acct_ID,LevelNo ");
        for (int col = 0; col < _columns.Length; col++)
            insert.Append(",Col_").Append(col);
        //	Select
        insert.Append(") SELECT ")
            .Append(GetAD_PInstance_ID()).Append(",")
            .Append(_lines[line].GetPA_ReportLine_ID())
            .Append(",").Append(variable).Append(",0,");	//	Record_ID, Fact_Acct_ID
        if (_DetailsSourceFirst)							//	LevelNo
            insert.Append("-1 ");
        else
            insert.Append("1 ");

        //	for all columns create select statement
        for (int col = 0; col < _columns.Length; col++)
        {
            insert.Append(", ");
            //	No calculation
            if (_columns[col].IsColumnTypeCalculation())
            {
                insert.Append("NULL");
                continue;
            }

            //	SELECT SUM()
            StringBuilder select = new StringBuilder ("SELECT ");
            if (_lines[line].GetAmountType() != null)				//	line amount type overwrites column
                select.Append (_lines[line].GetSelectClause (true));
            else if (_columns[col].GetAmountType() != null)
                select.Append (_columns[col].GetSelectClause (true));
            else
            {
                insert.Append("NULL");
                continue;
            }

            //	Get Period info
            select.Append(" FROM Fact_Acct_Balance fb WHERE DateAcct ");
            FinReportPeriod frp = GetPeriod (_columns[col].GetRelativePeriod());
            if (_lines[line].GetAmountType() != null)			//	line amount type overwrites column
            {
                if (_lines[line].IsPeriod())
                    select.Append(frp.GetPeriodWhere());
                else if (_lines[line].IsYear())
                    select.Append(frp.GetYearWhere());
                else
                    select.Append(frp.GetTotalWhere());
            }
            else if (_columns[col].GetAmountType() != null)
            {
                if (_columns[col].IsPeriod())
                    select.Append(frp.GetPeriodWhere());
                else if (_columns[col].IsYear())
                    select.Append(frp.GetYearWhere());
                else
                    select.Append(frp.GetTotalWhere());
            }
            //	Link
            select.Append(" AND fb.").Append(variable).Append("=x.").Append(variable);
            //	PostingType
            if (!_lines[line].IsPostingType())		//	only if not defined on line
            {
                String PostingType = _columns[col].GetPostingType();
                if (PostingType != null && PostingType.Length > 0)
                    select.Append(" AND fb.PostingType='").Append(PostingType).Append("'");
            }
            //	Report Where
            String s = _report.GetWhereClause();
            if (s != null && s.Length > 0)
                select.Append(" AND ").Append(s);

            //	Limited Segment Values
            if (_columns[col].IsColumnTypeSegmentValue())
                select.Append(_columns[col].GetWhereClause(_PA_Hierarchy_ID));

            //	Parameter Where
            select.Append(m_parameterWhere);
        //	System.out.println("    c=" + col + ", l=" + line + ": " + select);
            //
            insert.Append("(").Append(select).Append(")");
        }
        //	WHERE (sources, posting type)
        StringBuilder where = new StringBuilder(_lines[line].GetWhereClause(_PA_Hierarchy_ID));
        String s1 = _report.GetWhereClause();
        if (s1 != null && s1.Length > 0)
        {
            if (where.Length > 0)
                where.Append(" AND ");
            where.Append(s1);
        }
        if (where.Length > 0)
            where.Append(" AND ");
        where.Append(variable).Append(" IS NOT NULL");

        //	FROM .. WHERE
        insert.Append(" FROM Fact_Acct_Balance x WHERE ").Append(where);	
        //
        insert.Append(m_parameterWhere)
            .Append(" GROUP BY ").Append(variable);

        int no = DataBase.DB.ExecuteQuery(insert.ToString(),null, Get_TrxName());
        if (VLogMgt.IsLevelFinest())
            log.Fine("Source #=" + no + " - " + insert);
        if (no == 0)
            return;

        //	Set Name,Description
        StringBuilder sql = new StringBuilder ("UPDATE T_Report SET (Name,Description)=(")
            .Append(_lines[line].GetSourceValueQuery()).Append("Record_ID) "
            //
            + "WHERE Record_ID <> 0 AND AD_PInstance_ID=").Append(GetAD_PInstance_ID())
            .Append(" AND PA_ReportLine_ID=").Append(_lines[line].GetPA_ReportLine_ID())
            .Append(" AND Fact_Acct_ID=0");
        no = DataBase.DB.ExecuteQuery(sql.ToString(),null, Get_TrxName());
        if (VLogMgt.IsLevelFinest())
            log.Fine("Name #=" + no + " - " + sql.ToString());

        if (_report.IsListTrx())
            InsertLineTrx (line, variable);
    }	//	insertLineSource

    /// <summary>
    ///	Create Trx Line per Source Detail. 	- AD_PInstance_ID, PA_ReportLine_ID, variable, Fact_Acct_ID - Level 2
    /// </summary>
    /// <param name="line">line</param>
    /// <param name="variable">variable, e.g. Account_ID</param>
    private void InsertLineTrx (int line, String variable)
    {
        log.Info("Line=" + line + " - Variable=" + variable);
        MReportLine rLine = _lines[line];

        //	Insert
        StringBuilder insert = new StringBuilder("INSERT INTO T_Report "
            + "(AD_PInstance_ID, PA_ReportLine_ID, Record_ID,Fact_Acct_ID,LevelNo ");
        for (int col = 0; col < _columns.Length; col++)
            insert.Append(",Col_").Append(col);
        //	Select
        insert.Append(") SELECT ")
            .Append(GetAD_PInstance_ID()).Append(",")
            .Append(rLine.GetPA_ReportLine_ID()).Append(",")
            .Append(variable).Append(",Fact_Acct_ID, ");
        if (_DetailsSourceFirst)
            insert.Append("-2 ");
        else
            insert.Append("2 ");

        //	for all columns create select statement
        for (int col = 0; col < _columns.Length; col++)
        {
            insert.Append(", ");
            MReportColumn column = _columns[col]; 

            //	Segment Values
            if (column.IsColumnTypeSegmentValue())
            {
            }
            //	Only relative Period (not calculation or segment value)
            if (!(column.IsColumnTypeRelativePeriod() 
                && column.GetRelativePeriodAsInt() == 0))
            {
                insert.Append("NULL");
                continue;
            }
            //	Amount Type ... Qty
            if (rLine.GetAmountType() != null)				//	line amount type overwrites column
                insert.Append (rLine.GetSelectClause (false));
            else if (column.GetAmountType() != null)
                insert.Append (column.GetSelectClause (false));
            else
            {
                insert.Append("NULL");
                continue;
            }
        }
        //	(sources, posting type)
        StringBuilder where = new StringBuilder(rLine.GetWhereClause(_PA_Hierarchy_ID));
        //	Report Where
        String s = _report.GetWhereClause();
        if (s != null && s.Length > 0)
        {
            if (where.Length > 0)
                where.Append(" AND ");
            where.Append(s);
        }
        //	Period restriction
        FinReportPeriod frp = GetPeriod (0);
        if (where.Length > 0)
            where.Append(" AND ");
        where.Append(" DateAcct ").Append(frp.GetPeriodWhere());
        //	PostingType ??
//		if (!_lines[line].isPostingType())		//	only if not defined on line
//		{
//	      String PostingType = _columns[col].getPostingType();
//  	    if (PostingType != null && PostingType.length() > 0)
//      	  	where.Append(" AND PostingType='").Append(PostingType).Append("'");
//		}
        where.Append(" AND ").Append(variable).Append(" IS NOT NULL");

        //	Final FROM .. Where
        insert.Append(" FROM Fact_Acct WHERE ").Append(where);

        int no = DataBase.DB.ExecuteQuery(insert.ToString(),null, Get_TrxName());
        log.Finest("Trx #=" + no + " - " + insert);
        if (no == 0)
            return;
    }	//	insertLineTrx


    /// <summary>
    ///	Delete Unprinted Lines
    /// </summary>
    private void DeleteUnprintedLines()
    {
        for (int line = 0; line < _lines.Length; line++)
        {
            //	Not Printed - Delete in T
            if (!_lines[line].IsPrinted())
            {
                String sql = "DELETE FROM T_Report WHERE AD_PInstance_ID=" + GetAD_PInstance_ID()
                    + " AND PA_ReportLine_ID=" + _lines[line].GetPA_ReportLine_ID();
                int no = DataBase.DB.ExecuteQuery(sql,null, Get_TrxName());
                if (no > 0)
                    log.Fine(_lines[line].GetName() + " - #" + no);
            }
        }	//	for all lines
    }	//	deleteUnprintedLines


    /// <summary>
    /// Get/Create PrintFormat
    /// </summary>
    /// <returns>print format</returns>
    private MPrintFormat GetPrintFormat()
    {
        int AD_PrintFormat_ID = _report.GetAD_PrintFormat_ID();
        log.Info("AD_PrintFormat_ID=" + AD_PrintFormat_ID);
        MPrintFormat pf = null;
        bool createNew = AD_PrintFormat_ID == 0;

        //	Create New
        if (createNew)
        {
            int AD_Table_ID = 544;		//	T_Report
            pf = MPrintFormat.CreateFromTable(GetCtx(), AD_Table_ID);
            AD_PrintFormat_ID = pf.GetAD_PrintFormat_ID();
            _report.SetAD_PrintFormat_ID(AD_PrintFormat_ID);
            _report.Save();
        }
        else
            pf = MPrintFormat.Get(GetCtx(), AD_PrintFormat_ID, false);	//	use Cache

        //	Print Format Sync
        if (!_report.GetName().Equals(pf.GetName()))
            pf.SetName(_report.GetName());
        if (_report.GetDescription() == null)
        {
            if (pf.GetDescription () != null)
                pf.SetDescription (null);
        }
        else if (!_report.GetDescription().Equals(pf.GetDescription()))
            pf.SetDescription(_report.GetDescription());
        pf.Save();
        log.Fine(pf + " - #" + pf.GetItemCount());

        //	Print Format Item Sync
        int count = pf.GetItemCount();
        for (int i = 0; i < count; i++)
        {
            MPrintFormatItem pfi = pf.GetItem(i);
            String ColumnName = pfi.GetColumnName();
            //
            if (ColumnName == null)
            {
                log.Log(Level.SEVERE, "No ColumnName for #" + i + " - " + pfi);
                if (pfi.IsPrinted())
                    pfi.SetIsPrinted(false);
                if (pfi.IsOrderBy())
                    pfi.SetIsOrderBy(false);
                if (pfi.GetSortNo() != 0)
                    pfi.SetSortNo(0);
            }
            else if (ColumnName.StartsWith("Col"))
            {
                //int index = Integer.parseInt(ColumnName.substring(4));
                int index = Utility.Util.GetValueOfInt(ColumnName.Substring(4));
                if (index < _columns.Length)
                {
                    pfi.SetIsPrinted(_columns[index].IsPrinted());
                    String s = _columns[index].GetName();
                    if (!pfi.GetName().Equals(s))
                    {
                        pfi.SetName(s);
                        pfi.SetPrintName(s);
                    }
                    int seq = 30 + index;
                    if (pfi.GetSeqNo() != seq)
                        pfi.SetSeqNo(seq);
                }
                else	//	not printed
                {
                    if (pfi.IsPrinted())
                        pfi.SetIsPrinted(false);
                }
                //	Not Sorted
                if (pfi.IsOrderBy())
                    pfi.SetIsOrderBy(false);
                if (pfi.GetSortNo() != 0)
                    pfi.SetSortNo(0);
            }
            else if (ColumnName.Equals("SeqNo"))
            {
                if (pfi.IsPrinted())
                    pfi.SetIsPrinted(false);
                if (!pfi.IsOrderBy())
                    pfi.SetIsOrderBy(true);
                if (pfi.GetSortNo() != 10)
                    pfi.SetSortNo(10);
            }
            else if (ColumnName.Equals("LevelNo"))
            {
                if (pfi.IsPrinted())
                    pfi.SetIsPrinted(false);
                if (!pfi.IsOrderBy())
                    pfi.SetIsOrderBy(true);
                if (pfi.GetSortNo() != 20)
                    pfi.SetSortNo(20);
            }
            else if (ColumnName.Equals("Name"))
            {
                if (pfi.GetSeqNo() != 10)
                    pfi.SetSeqNo(10);
                if (!pfi.IsPrinted())
                    pfi.SetIsPrinted(true);
                if (!pfi.IsOrderBy())
                    pfi.SetIsOrderBy(true);
                if (pfi.GetSortNo() != 30)
                    pfi.SetSortNo(30);
            }
            else if (ColumnName.Equals("Description"))
            {
                if (pfi.GetSeqNo() != 20)
                    pfi.SetSeqNo(20);
                if (!pfi.IsPrinted())
                    pfi.SetIsPrinted(true);
                if (pfi.IsOrderBy())
                    pfi.SetIsOrderBy(false);
                if (pfi.GetSortNo() != 0)
                    pfi.SetSortNo(0);
            }
            else	//	Not Printed, No Sort
            {
                if (pfi.IsPrinted())
                    pfi.SetIsPrinted(false);
                if (pfi.IsOrderBy())
                    pfi.SetIsOrderBy(false);
                if (pfi.GetSortNo() != 0)
                    pfi.SetSortNo(0);
            }
            pfi.Save();
            log.Fine(pfi.ToString());
        }
        //	set translated to original
        pf.SetTranslation();
        //	First one is unsorted - just re-load
        if (createNew)
            pf = MPrintFormat.Get(GetCtx(), AD_PrintFormat_ID, false);	//	use Cache
        return pf;
    }	//	getPrintFormat

}	//	FinReport

}
