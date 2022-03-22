using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using System.Data.SqlClient;
using System.Data;
using VAdvantage.WF;
using System.Reflection;
using VAdvantage.DataBase;
using VAdvantage.Utility;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;
using VAdvantage.Print;
using System.Threading;
using System.Windows.Forms;
using VAdvantage.CrystalReport;
using VAdvantage.Logging;
using VAdvantage.Classes;
using System.ComponentModel;
using System.IO;
using VAdvantage.Model;
using Npgsql;
using VAdvantage.BiReport;

namespace VAdvantage.ProcessEngine
{
    public class ProcessCtl
    {
        Ctx _ctx = null;

        public const string ReportType_CSV = "C";
        public const string ReportType_PDF = "P";
        public const string ReportType_RTF = "R";
        public const string ReportType_HTML = "S";//HTML Report of Print Format
        public const string ReportType_BIHTML = "B";//HTML Scripts returned by BI



        private string rptHtml = null;
        private bool isPrintFormat = false;
        // private bool isPrintCsv = false;
        private int AD_PrintFormat_ID = 0;
        private int AD_ReportView_ID = 0;
        //int reportTable_ID = 0;
        // private int ResAD_PrintFormat_ID = -1;
        //private string fileType = "PDF";
        public ProcessCtl() { }

        /// <summary>
        /// Start Workflow.
        /// </summary>
        /// <param name="AD_Workflow_ID">AD_Workflow_ID </param>
        /// <returns>true if started</returns>
        private bool StartWorkflow(int AD_Workflow_ID)
        {
            //Remote process not implemented
            //Only local process is implemented
            //log.Fine(AD_Workflow_ID + " - " + _pi.ToString());
            bool started = false;
            //	Run locally
            if (!started)
            {
                Type type = null;
                System.Reflection.Assembly asm = null;
                try
                {
                    asm = System.Reflection.Assembly.Load(GlobalVariable.PACKAGES[0]);
                    type = asm.GetType("VAdvantage.WF.MWorkflow");

                    ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(Ctx), typeof(int), typeof(Trx) });
                    Object WF = constructor.Invoke(new object[] { _ctx, AD_Workflow_ID, null });

                    MethodInfo mInfo = type.GetMethod("StartWF", new Type[] { typeof(ProcessInfo) });

                    started = Convert.ToBoolean(mInfo.Invoke(WF, new object[] { _pi }));

                }
                catch (Exception ex)
                {
                    log.Fine("Error Starting Workflow: " + ex.Message);
                    //blank
                    return started;
                }
            }

            return started;

        }

        /// <summary>
        /// Start the DB Process
        /// </summary>
        /// <param name="procedureName">name of the process</param>
        /// <returns></returns>
        /// 
#pragma warning disable 612, 618
        public bool StartDBProcess(String procedureName)
        {
            if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
            {
                NpgsqlConnection conn1 = null;
                try
                {
                    NpgsqlCommand comm = new NpgsqlCommand();
                    conn1 = (NpgsqlConnection)DataBase.DB.GetConnection();
                    conn1.Open();
                    comm.Connection = conn1;
                    comm.CommandText = procedureName;
                    comm.CommandType = CommandType.StoredProcedure;
                    NpgsqlCommandBuilder.DeriveParameters(comm);
                    NpgsqlParameter[] param = new NpgsqlParameter[1];

                    foreach (NpgsqlParameter orp in comm.Parameters)
                    {
                        param[0] = new NpgsqlParameter(orp.ParameterName, _pi.GetAD_PInstance_ID());
                    }

                    //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                    int res = SqlExec.PostgreSql.PostgreHelper.ExecuteNonQuery(conn1, CommandType.StoredProcedure, procedureName, param);
                    conn1.Close();
                    if (res < 0)
                    {
                        ProcessInfoUtil.SetParameterFromDB(_pi, _ctx);
                        return StartDBProcess(procedureName, _pi.GetParameter());
                    }
                }
                catch (Exception e)
                {
                    if (conn1 != null)
                        conn1.Close();
                    //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                    _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessRunError") + " " + e.Message);
                    _pi.SetError(true);
                    return false;

                }
                //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
                return true;
                //DB.ExecuteProcedure(procedureName, null, null);
                //return false;
            }

            //  execute on this thread/connection
            //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
            OracleConnection conn = null;
            try
            {
                //only oracle procedure are supported
                OracleCommand comm = new OracleCommand();
                conn = (OracleConnection)DataBase.DB.GetConnection();
                conn.Open();
                comm.Connection = conn;
                comm.CommandText = procedureName;
                comm.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(comm);
                OracleParameter[] param = new OracleParameter[1];

                foreach (OracleParameter orp in comm.Parameters)
                {
                    param[0] = new OracleParameter(orp.ParameterName, _pi.GetAD_PInstance_ID());
                }

                //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                int res = SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
                conn.Close();
                if (res < 0)
                {
                    ProcessInfoUtil.SetParameterFromDB(_pi, _ctx);
                    return StartDBProcess(procedureName, _pi.GetParameter());
                }

                //DataBase.DB.ExecuteQuery(sql, null);
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();
                //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessRunError") + " " + e.Message);
                _pi.SetError(true);
                return false;

            }
            //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
            return true;
        }


        private bool StartDBProcess(String procedureName, ProcessInfoParameter[] list)
        {

            if (DatabaseType.IsPostgre)  //jz Only DB2 not support stored procedure now
            {
                NpgsqlConnection conn1 = null;
                try
                {
                    NpgsqlCommand comm = new NpgsqlCommand();
                    conn1 = (NpgsqlConnection)DataBase.DB.GetConnection();
                    conn1.Open();
                    comm.Connection = conn1;
                    comm.CommandText = procedureName;
                    comm.CommandType = CommandType.StoredProcedure;
                    NpgsqlCommandBuilder.DeriveParameters(comm);
                    NpgsqlParameter[] param = new NpgsqlParameter[comm.Parameters.Count];
                    int i = 0;
                    StringBuilder orclParams = new StringBuilder();
                    bool isDateTo = false;
                    foreach (NpgsqlParameter orp in comm.Parameters)
                    {
                        if (isDateTo)
                        {
                            isDateTo = false;
                            continue;
                        }
                        Object paramvalue = list[i].GetParameter();
                        if (paramvalue != null)
                        {
                            if (orp.DbType == System.Data.DbType.DateTime)
                            {
                                if (paramvalue.ToString().Length > 0)
                                {
                                    paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                                }
                                param[i] = new NpgsqlParameter(orp.ParameterName, paramvalue);
                                if (list[i].GetParameter_To() != null && list[i].GetParameter_To().ToString().Length > 0)
                                {
                                    paramvalue = list[i].GetParameter_To();
                                    paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                                    param[i + 1] = new NpgsqlParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                    i++;
                                    isDateTo = true;
                                    continue;
                                }
                                else
                                {
                                    if (comm.Parameters.Count > (i + 1))
                                    {
                                        if (comm.Parameters[i + 1].ParameterName.Equals(comm.Parameters[i].ParameterName + "_TO", StringComparison.OrdinalIgnoreCase))
                                        {
                                            param[i + 1] = new NpgsqlParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                            isDateTo = true;
                                            continue;
                                        }
                                    }
                                }
                            }
                            else if (orp.DbType == System.Data.DbType.VarNumeric)
                            {
                                if (paramvalue.ToString().Length > 0)
                                {
                                    //continue;
                                }
                                else
                                    paramvalue = 0;
                            }
                            else
                            {
                                if (paramvalue.ToString().Length > 0)
                                {
                                    paramvalue = GlobalVariable.TO_STRING(paramvalue.ToString());
                                }
                            }

                        }
                        param[i] = new NpgsqlParameter(orp.ParameterName, paramvalue);
                        //orclParams.Append(orp.ParameterName).Append(": ").Append(_curTab.GetValue(list[i]));
                        //if (i < comm.Parameters.Count - 1)
                        //    orclParams.Append(", ");
                        i++;
                    }

                    //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                    int res = SqlExec.PostgreSql.PostgreHelper.ExecuteNonQuery(conn1, CommandType.StoredProcedure, procedureName, param);
                    conn1.Close();
                    if (res < 0)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    if (conn1 != null)
                        conn1.Close();
                    //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                    _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessRunError") + " " + e.Message);
                    _pi.SetError(true);
                    return false;

                }
                //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
                return true;
                //return false;
            }

            //  execute on this thread/connection
            //String sql = "{call " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")}";
            OracleConnection conn = null;
            try
            {
                //only oracle procedure are supported
                OracleCommand comm = new OracleCommand();
                conn = (OracleConnection)VAdvantage.DataBase.DB.GetConnection();
                conn.Open();
                comm.Connection = conn;
                comm.CommandText = procedureName;
                comm.CommandType = CommandType.StoredProcedure;
                OracleCommandBuilder.DeriveParameters(comm);
                OracleParameter[] param = new OracleParameter[comm.Parameters.Count];
                int i = 0;
                StringBuilder orclParams = new StringBuilder();
                bool isDateTo = false;
                foreach (OracleParameter orp in comm.Parameters)
                {
                    if (isDateTo)
                    {
                        isDateTo = false;
                        continue;
                    }
                    Object paramvalue = list[i].GetParameter();
                    if (paramvalue != null)
                    {
                        if (orp.DbType == System.Data.DbType.DateTime)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                            }
                            param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                            if (list[i].GetParameter_To() != null && list[i].GetParameter_To().ToString().Length > 0)
                            {
                                paramvalue = list[i].GetParameter_To();
                                paramvalue = ((DateTime)paramvalue).ToString("dd-MMM-yyyy");
                                param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                i++;
                                isDateTo = true;
                                continue;
                            }
                            else
                            {
                                if (comm.Parameters.Count > (i + 1))
                                {
                                    if (comm.Parameters[i + 1].ParameterName.Equals(comm.Parameters[i].ParameterName + "_TO", StringComparison.OrdinalIgnoreCase))
                                    {
                                        param[i + 1] = new OracleParameter(comm.Parameters[i + 1].ParameterName, paramvalue);
                                        isDateTo = true;
                                        continue;
                                    }
                                }
                            }
                        }
                        else if (orp.DbType == System.Data.DbType.VarNumeric)
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                //continue;
                            }
                            else
                                paramvalue = 0;
                        }
                        else
                        {
                            if (paramvalue.ToString().Length > 0)
                            {
                                paramvalue = GlobalVariable.TO_STRING(paramvalue.ToString());
                            }
                        }

                    }
                    param[i] = new OracleParameter(orp.ParameterName, paramvalue);
                    //orclParams.Append(orp.ParameterName).Append(": ").Append(_curTab.GetValue(list[i]));
                    //if (i < comm.Parameters.Count - 1)
                    //    orclParams.Append(", ");
                    i++;
                }

                //log.Fine("Executing " + procedureName + "(" + _pi.GetAD_PInstance_ID() + ")");
                int res = VAdvantage.SqlExec.Oracle.OracleHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, procedureName, param);
                conn.Close();
                if (res < 0)
                {
                    return false;
                }

                //DataBase.DB.ExecuteQuery(sql, null);
            }
            catch (Exception e)
            {
                if (conn != null)
                    conn.Close();
                VAdvantage.Logging.VLogger.Get().SaveError(e.Message, e);
                //log.Log(Level.SEVERE, "Error executing procedure " + procedureName, e);
                return false;

            }
            //	log.fine(Log.l4_Data, "ProcessCtl.startProcess - done");
            return true;
        }

#pragma warning restore 612, 618

        ProcessInfo _pi = null;

        volatile IReportEngine re = null;

        private void Unlock()
        {
            String summary = _pi.GetSummary();
            if (summary != null && summary.IndexOf("@") != -1)
                _pi.SetSummary(Utility.Msg.ParseTranslation(_ctx, summary));
            if (_parent != null)
            {
                Thread t = new Thread(delegate () { _parent.UnlockUI(_pi); });
                t.CurrentCulture = Utility.Env.GetLanguage(_ctx).GetCulture(Utility.Env.GetLoginLanguage(_ctx).GetAD_Language());
                t.CurrentUICulture = Utility.Env.GetLanguage(_ctx).GetCulture(Utility.Env.GetLoginLanguage(_ctx).GetAD_Language());
                t.Start();
            }
        }





        public Dictionary<string, object> Process(ProcessInfo pi, Ctx ctx, out byte[] report, out ReportEngine_N _rep)
        {
            _rep = null;

            _ctx = ctx;
            _pi = pi;
            report = null;

            MPInstance instance = null;

            if (_pi.GetAD_PInstance_ID() < 1)
            {
                try
                {
                    instance = new MPInstance(ctx, _pi.GetAD_Process_ID(), _pi.GetRecord_ID());

                }
                catch (Exception e)
                {
                    _pi.SetSummary(e.Message);
                    _pi.SetError(true);
                    return _pi.ToList();
                }

                if (!instance.Save())
                {
                    _pi.SetSummary(Msg.GetMsg(ctx, "ProcessNoInstance", true));
                    _pi.SetError(true);
                    return _pi.ToList();
                }

                _pi.SetAD_PInstance_ID(instance.Get_ID());
            }



            String procedureName = "";
            int AD_ReportViews_ID = 0;
            int AD_ReportFormat_ID = 0;
            int AD_Workflow_ID = 0;
            bool IsReport = false;

            bool IsDirectPrint = false;

            bool IsCrystalReport = false;

            String sql = "SELECT p.Name, p.procedureName,p.Classname, p.AD_Process_ID,"		//	1..4  
                + " p.IsReport,p.IsDirectPrint,p.AD_ReportView_ID,p.AD_Workflow_ID,"		//	5..8
                + " CASE WHEN COALESCE(p.Statistic_Count,0)=0 THEN 0 ELSE p.Statistic_Seconds/p.Statistic_Count END," //9
                + " p.IsServerProcess, " //10
                + " p.IsCrystalReport, "         // crystal  11...12
                + " p.AD_ReportFormat_ID " //12
                + " FROM AD_Process p"
                + " INNER JOIN AD_PInstance i ON (p.AD_Process_ID=i.AD_Process_ID) "
                + " WHERE p.IsActive='Y'"
                + " AND i.AD_PInstance_ID=@pinstanceid";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", _pi.GetAD_PInstance_ID());
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    _pi.SetTitle(dr[0].ToString());
                    //if (m_waiting != null)
                    //    m_waiting.setTitle(_pi.getTitle());
                    procedureName = dr[1].ToString();

                    _pi.SetClassName(dr[2].ToString());

                    _pi.SetAD_Process_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    //	Report
                    if ("Y".Equals(dr[4].ToString()))
                    {
                        IsReport = true;
                        IsCrystalReport = "Y".Equals(dr[10].ToString());
                        AD_ReportFormat_ID = dr[11].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[11].ToString());
                        //later
                    }
                    AD_ReportView_ID = dr[6].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[6].ToString());
                    AD_Workflow_ID = dr[7].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[7].ToString());
                    //
                    //_IsServerProcess = "Y".Equals(dr[9].ToString());




                }
                dr.Close();
            }
            catch (Exception ex)
            {
                if (dr != null)
                    dr.Close();
                _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessNoProcedure") + " " + ex.Message, true);
                Unlock();

            }

            if (procedureName == null)
                procedureName = "";

            if (AD_Workflow_ID > 0)
            {
                StartWorkflow(AD_Workflow_ID);

                //Updated by raghu to open reports from work flow

                byte[] repByt = null;
                re = ReportCtl.Report;
                if (re != null)
                {
                    // int reportTable_ID = 0;
                    if (re is IReportView)
                    {
                        IReportView irv = re as IReportView;
                        //  reportTable_ID = irv.GetPrintFormat().GetAD_Table_ID();
                        irv.GetView();
                        _pi.Set_AD_PrintFormat_Table_ID(irv.GetPrintFormat().GetAD_Table_ID());
                    }
                    _pi.SetSummary("Report", re != null);
                    Unlock();
                    if (re != null)
                    {
                        repByt = re.GetReportBytes();
                    }
                }
                else
                {
                    repByt = null;
                }
                report = repByt;
                return _pi.ToList();
            }

            if (_pi.GetClassName() != null)
            {
                if (!StartProcess())
                {
                    report = null;
                    return _pi.ToList();
                }

                if (!IsReport && procedureName.Length == 0)
                {
                    report = null;
                    return _pi.ToList();
                }
            }
            report = null;
            if (IsReport)
            {
                if (procedureName.Length > 0)
                {
                    if (!StartDBProcess(procedureName))
                    {

                    }
                }


                if (AD_ReportFormat_ID > 0)             // For Report Formats
                {
                    _pi.SetIsReportFormat(true);
                    int totalRecords = 0;
                    re = VAdvantage.ReportFormat.ReportFormatEngine.Get(_ctx, _pi, out totalRecords, _pi.IsArabicReportFromOutside);
                    Unlock();
                    _pi.SetSummary("Report", re != null);
                    _pi.SetTotalRecords(totalRecords);
                    if (re != null)
                    {
                        report = re.GetReportBytes();
                        ReportString = re.GetReportString();
                    }
                }


                else if (!IsCrystalReport)
                {
                    //start report code
                    //	Start Report	-----------------------------------------------
                    re = ReportCtl.Start(_ctx, _pi, IsDirectPrint);
                    _rep = (ReportEngine_N)re;
                    // int reportTable_ID = _rep.GetPrintFormat().GetAD_Table_ID();
                    _rep.GetView();
                    _pi.Set_AD_PrintFormat_Table_ID(_rep.GetPrintFormat().GetAD_Table_ID());
                    _pi.Set_AD_PrintFormat_ID(_rep.GetPrintFormat().GetAD_PrintFormat_ID());
                    _pi.SetSummary("Report", re != null);
                    Unlock();
                    if (re != null)
                    {
                        report = re.GetReportBytes();
                    }
                }
                else
                {
                    _pi.SetIsCrystal(true);
                    string errorMsg = null;
                    IReportEngine en = null;
                    try
                    {
                        en = new CrystalReportEngine();
                        en.StartReport(_ctx, _pi, null);
                        report = en.GetReportBytes();
                    }
                    catch (Exception err)
                    {
                        errorMsg = err.Message;
                    }

                    _pi.SetSummary(errorMsg ?? "Done", errorMsg != null);
                }
            }
            else
            {
                if (!StartDBProcess(procedureName))
                {
                    _pi.SetSummary("procedure ERROR");
                    return _pi.ToList();
                }
                ProcessInfoUtil.SetSummaryFromDB(_pi);
            }

            return _pi.ToList();
        }

        public Dictionary<string, object> Process(ProcessInfo pi, Ctx ctx, out byte[] report, out string reportFilePath)

        {
            reportFilePath = null;

            _ctx = ctx;
            _pi = pi;
            report = null;

            MPInstance instance = null;

            if (_pi.GetAD_PInstance_ID() < 1)
            {
                try
                {
                    instance = new MPInstance(ctx, _pi.GetAD_Process_ID(), _pi.GetRecord_ID());

                }
                catch (Exception e)
                {
                    _pi.SetSummary(e.Message);
                    _pi.SetError(true);
                    return _pi.ToList();
                }

                if (!instance.Save())
                {
                    _pi.SetSummary(Msg.GetMsg(ctx, "ProcessNoInstance", true));
                    _pi.SetError(true);
                    return _pi.ToList();
                }

                _pi.SetAD_PInstance_ID(instance.Get_ID());
            }



            String procedureName = "";
            //int AD_ReportView_ID = 0;
            int AD_ReportFormat_ID = 0;
            int AD_Workflow_ID = 0;
            bool IsReport = false;

            bool IsDirectPrint = false;

            bool IsCrystalReport = false;
            bool IsTelerikReport = false;
            bool IsBIReport = false;
            bool IsJasperReport = false;
            int AD_ReportMaster_ID = 0;

            String sql = "SELECT p.Name, p.procedureName,p.Classname, p.AD_Process_ID,"		//	1..4  
                + " p.IsReport,p.IsDirectPrint,p.AD_ReportView_ID,p.AD_Workflow_ID,"		//	5..8
                + " CASE WHEN COALESCE(p.Statistic_Count,0)=0 THEN 0 ELSE p.Statistic_Seconds/p.Statistic_Count END," //9
                + " p.IsServerProcess, " //10
                + " p.IsCrystalReport, "         // crystal  11...12
                + " p.AD_ReportFormat_ID,  " //12
                + "  p.AD_ReportMaster_ID  "  //13
                + " FROM AD_Process p"
                + " INNER JOIN AD_PInstance i ON (p.AD_Process_ID=i.AD_Process_ID) "


                + " WHERE p.IsActive='Y'"
                + " AND i.AD_PInstance_ID=@pinstanceid";

            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", _pi.GetAD_PInstance_ID());
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    _pi.SetTitle(dr[0].ToString());
                    //if (m_waiting != null)
                    //    m_waiting.setTitle(_pi.getTitle());
                    procedureName = dr[1].ToString();

                    _pi.SetClassName(dr[2].ToString());

                    _pi.SetAD_Process_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    //	Report
                    if ("Y".Equals(dr[4].ToString()))
                    {
                        IsReport = true;
                        pi.SetIsReport(IsReport);
                        IsCrystalReport = "Y".Equals(dr[10].ToString());
                        AD_ReportFormat_ID = dr[11].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[11].ToString());
                        IsTelerikReport = "T".Equals(dr[10].ToString());
                        //later
                        IsBIReport = "B".Equals(dr[10].ToString());
                        IsJasperReport = "J".Equals(dr[10].ToString());

                        //New Column 
                        AD_ReportMaster_ID = Utility.Util.GetValueOfInt(dr[12].ToString());
                    }
                    AD_ReportView_ID = dr[6].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[6].ToString());
                    AD_Workflow_ID = dr[7].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[7].ToString());
                    //
                    //_IsServerProcess = "Y".Equals(dr[9].ToString());




                }
                dr.Close();
            }
            catch (Exception ex)
            {
                if (dr != null)
                    dr.Close();
                _pi.SetSummary(Msg.GetMsg(ctx, "ProcessNoProcedure") + " " + ex.Message, true);
                Unlock();

            }

            _pi.SetUseCrystalReportViewer(ctx.GetUseCrystalReportViewer().Equals("Y"));

            if (procedureName == null)
                procedureName = "";

            if (AD_Workflow_ID > 0)
            {
                StartWorkflow(AD_Workflow_ID);

                //Updated by raghu to open reports from work flow

                byte[] repByt = null;
                re = ReportCtl.Report;
                if (re != null)
                {
                    // int reportTable_ID = 0;
                    if (re is IReportView)
                    {
                        IReportView irv = re as IReportView;
                        // reportTable_ID = irv.GetPrintFormat().GetAD_Table_ID();
                        irv.GetView();
                        _pi.Set_AD_PrintFormat_Table_ID(irv.GetPrintFormat().GetAD_Table_ID());
                    }
                    _pi.SetSummary("Report", re != null);
                    Unlock();
                    if (re != null)
                    {
                        reportFilePath = re.GetReportFilePath(true, out repByt);
                    }
                }
                else
                {
                    repByt = null;
                }
                report = repByt;
                return _pi.ToList();
            }

            if (_pi.GetClassName() != null)
            {
                if (!StartProcess())
                {
                    report = null;
                    return _pi.ToList();
                }

                if (!IsReport && procedureName.Length == 0)
                {
                    report = null;
                    return _pi.ToList();
                }
            }
            report = null;
            if (IsReport)
            {

                string lang = ctx.GetAD_Language().Replace("_", "-");
                // Set Report Language -VIS0228
                if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang")))
                {
                    lang = ctx.GetContext("Report_Lang").Replace("_", "-");
                }
                System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

                if (procedureName.Length > 0)
                {
                    if (!StartDBProcess(procedureName))
                    {

                    }
                }

                //Check Dynamically created report 
                if (AD_ReportMaster_ID > 0)
                {
                    IDataReader drRep = null;
                    // bool foundEngine = false;
                    try
                    {
                        drRep = DB.ExecuteReader(@"SELECT Name, ClassName,Assemblyname,IssupportPaging,dynamicAction,PageSize FROM AD_ReportMaster WHERE
                                                   IsActive='Y' AND AD_ReportMaster_ID = " + AD_ReportMaster_ID);
                        string cName = "", assemblyName = "", dynamicAction = "";
                        int pageSize = 0;
                        bool isSupportPaging = false;
                        if (drRep.Read())
                        {
                            // foundEngine = true;
                            cName = Util.GetValueOfString(drRep["ClassName"]);
                            assemblyName = Util.GetValueOfString(drRep["AssemblyName"]);
                            isSupportPaging = Util.GetValueOfString(drRep["IsSupportPaging"]) == "Y";
                            dynamicAction = Util.GetValueOfString(drRep["DynamicAction"]);
                            pageSize = Util.GetValueOfInt(drRep["PageSize"]);
                        }
                        drRep.Close();

                        if (cName == "" || assemblyName == "")
                        {
                            _pi.SetSummary("parameters[assemblyname,ClassName] having null or empty values");
                        }
                        else
                        {
                            if (pageSize > 0)
                                _pi.SetPageSize(pageSize);

                            _pi.SetIsSupportPaging(isSupportPaging);
                            _pi.SetDynamicAction(dynamicAction);

                            re = VAdvanatge.Report.ReportEngine.GetReportEngine(_ctx, pi, _trx, assemblyName, cName);
                        }
                    }
                    catch (Exception e)
                    {
                        _pi.SetSummary(e.Message);
                    }
                    finally
                    {
                        if (drRep != null && !drRep.IsClosed)
                            drRep.Close();
                    }
                }

                else if (AD_ReportFormat_ID > 0)             // For Report Formats
                {
                    _pi.SetIsReportFormat(true);
                    _pi.SetPrintAllPages(false);
                    re = VAdvanatge.Report.ReportEngine.GetReportEngine(_ctx, pi, _trx, "VARCOMSvc", "ViennaAdvantage.Classes.ReportFromatWrapper");
                    Unlock();

                    // "#REPORT_PAGE_SIZE"
                    int pageSize = Util.GetValueOfInt(ctx.GetContext("#REPORT_PAGE_SIZE")); //500;
                    _pi.SetTotalPage(_pi.GetTotalPage());
                    _pi.SetIsSupportPaging(true);

                    _pi.SetSummary("Report", re != null);
                }

                else if (IsBIReport)
                {
                    re = VAdvanatge.Report.ReportEngine.GetReportEngine(_ctx, pi, _trx, "VA039", "VA039.Classes.BIReportEngine");
                }

                else if (IsJasperReport)
                {
                    _pi.SetIsJasperReport(true);

                    re = VAdvanatge.Report.ReportEngine.GetReportEngine(_ctx, pi, _trx, "VA039", "VA039.Classes.JasperReportEngine");
                }
                else if (!IsCrystalReport)
                {
                    //start report code
                    //	Start Report	-----------------------------------------------
                    re = ReportCtl.Start(_ctx, _pi, IsDirectPrint);
                    ReportEngine_N _rep = (ReportEngine_N)re;
                    //null check Implemented by raghu 22-May-2015
                    if (_rep != null)
                    {
                        //reportTable_ID = _rep.GetPrintFormat().GetAD_Table_ID();
                        _rep.GetView();

                        _pi.Set_AD_PrintFormat_Table_ID(_rep.GetPrintFormat().GetAD_Table_ID());
                        _pi.Set_AD_PrintFormat_ID(_rep.GetPrintFormat().GetAD_PrintFormat_ID());
                        //_pi.SetTotalRecords(_rep.GetPrintFormat().TotalPage);
                        _pi.Set_AD_ReportView_ID(AD_ReportView_ID);
                        _pi.SetSummary("Report", re != null);

                        // "#REPORT_PAGE_SIZE"
                        int pageSize = Util.GetValueOfInt(ctx.GetContext("#REPORT_PAGE_SIZE")); //500;
                        int totalRecords = _rep.GetPrintFormat().TotalPage;
                        _pi.SetTotalPage((totalRecords % pageSize) == 0 ? (totalRecords / pageSize) : ((totalRecords / pageSize) + 1));
                        _pi.SetIsSupportPaging(true);
                        Unlock();


                        if (_pi.GetFileType() == ReportType_CSV)
                        {
                            report = _rep.CreateCSV(_ctx);
                            string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload";

                            if (!Directory.Exists(filePath))
                                Directory.CreateDirectory(filePath);

                            filePath = filePath + "\\temp_" + CommonFunctions.CurrentTimeMillis() + ".csv";
                            File.WriteAllBytes(filePath, report);
                            reportFilePath = filePath.Substring(filePath.IndexOf("TempDownload"));
                            re = null;
                        }
                        else if (_pi.GetFileType() == ReportType_PDF)
                        {
                            reportFilePath = re.GetReportFilePath(true, out report);
                        }
                        else if (_pi.GetFileType() == ReportType_RTF)
                        {
                            reportFilePath = re.GetRtfReportFilePath("");
                            if (GetIsPrintFormat())
                            {
                                _pi.SetSummary("RTF not implemented for AD reports");
                            }
                        }
                        else
                        {
                            reportFilePath = re.GetReportFilePath(true, out report);
                            rptHtml = _rep.GetRptHtml().ToString();
                        }
                    }
                }
                else if (IsTelerikReport)
                {
                    _pi.SetIsTelerik(true);
                }
                // Execute Crystal report only if user don't want to use crystal report viewer.
                else if (!_pi.GetUseCrystalReportViewer())
                //else
                {
                    _pi.SetIsCrystal(true);
                    string errorMsg = null;
                    try
                    {
                        re = new CrystalReportEngine();
                        re.StartReport(_ctx, _pi, null);
                    }
                    catch (Exception err)
                    {
                        errorMsg = err.Message;
                    }

                    _pi.SetSummary(errorMsg ?? "Done", errorMsg != null);
                }


                if (re != null)
                {

                    if (_pi.GetFileType() == ReportType_CSV)
                    {
                        reportFilePath = re.GetCsvReportFilePath(reportFilePath);
                    }
                    else if (_pi.GetFileType() == ReportType_PDF || _pi.GetFileType() == ReportType_HTML)
                    {
                        reportFilePath = re.GetReportFilePath(true, out report);
                    }
                    else if (_pi.GetFileType() == ReportType_RTF)
                    {
                        reportFilePath = re.GetRtfReportFilePath(reportFilePath);
                    }
                    else if (_pi.GetFileType() == ReportType_BIHTML)
                    {
                        BiReportEngine com = new BiReportEngine();
                        reportFilePath = com.GetReportString(ctx, log, pi);
                        //reportFilePath = re.GetHTMLScript(reportFilePath);
                    }

                }
            }
            else
            {
                if (!StartDBProcess(procedureName))
                {
                    _pi.SetSummary("procedure ERROR");
                    return _pi.ToList();
                }
                ProcessInfoUtil.SetSummaryFromDB(_pi);
            }

            return _pi.ToList();
        }





        public string GetRptHtml()
        {
            return rptHtml;
        }
        bool isRCReport = false;
        public bool IsRCReport()
        {
            return isRCReport;
        }
        public int GetAD_PrintFormat_ID()
        {
            return AD_PrintFormat_ID;
        }


        public int GetAD_ReportView_ID()
        {
            return AD_ReportView_ID;
        }

        //public Dictionary<string, object> Process(ProcessInfo pi, Ctx ctx, out byte[] report, out IReportEngine _rep)
        //{
        //    _rep = null;

        //    _ctx = ctx;
        //    _pi = pi;
        //    report = null; 

        //    MPInstance instance = null;

        //    if (_pi.GetAD_PInstance_ID() < 1)
        //    {
        //        try
        //        {
        //            instance = new MPInstance(_ctx, _pi.GetAD_Process_ID(), _pi.GetRecord_ID());
        //        }
        //        catch (Exception e)
        //        {
        //            _pi.SetSummary(e.Message);
        //            _pi.SetError(true);
        //            return _pi.ToList();
        //        }

        //        if (!instance.Save())
        //        {
        //            _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessNoInstance", true));
        //            _pi.SetError(true);
        //            return _pi.ToList();
        //        }

        //        _pi.SetAD_PInstance_ID(instance.Get_ID());
        //    }



        //    String procedureName = "";
        //    int AD_ReportView_ID = 0;
        //    int AD_Workflow_ID = 0;
        //    bool IsReport = false;

        //    bool IsDirectPrint = false;

        //    bool IsCrystalReport = false;

        //    String sql = "SELECT p.Name, p.procedureName,p.Classname, p.AD_Process_ID,"		//	1..4  
        //        + " p.IsReport,p.IsDirectPrint,p.AD_ReportView_ID,p.AD_Workflow_ID,"		//	5..8
        //        + " CASE WHEN COALESCE(p.Statistic_Count,0)=0 THEN 0 ELSE p.Statistic_Seconds/p.Statistic_Count END," //9
        //        + " p.IsServerProcess, " //10
        //        + " p.IsCrystalReport "         // crystal  11...12
        //        + " FROM AD_Process p"
        //        + " INNER JOIN AD_PInstance i ON (p.AD_Process_ID=i.AD_Process_ID) "
        //        + " WHERE p.IsActive='Y'"
        //        + " AND i.AD_PInstance_ID=@pinstanceid";

        //    IDataReader dr = null;
        //    try
        //    {
        //        SqlParameter[] param = new SqlParameter[1];
        //        param[0] = new SqlParameter("@pinstanceid", _pi.GetAD_PInstance_ID());
        //        dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
        //        while (dr.Read())
        //        {
        //            _pi.SetTitle(dr[0].ToString());
        //            //if (m_waiting != null)
        //            //    m_waiting.setTitle(_pi.getTitle());
        //            procedureName = dr[1].ToString();

        //            _pi.SetClassName(dr[2].ToString());

        //            _pi.SetAD_Process_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
        //            //	Report
        //            if ("Y".Equals(dr[4].ToString()))
        //            {
        //                IsReport = true;

        //                IsCrystalReport = "Y".Equals(dr[10].ToString());
        //                //later
        //            }
        //            AD_ReportView_ID = dr[6].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[6].ToString());
        //            AD_Workflow_ID = dr[7].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[7].ToString());
        //            //
        //            //_IsServerProcess = "Y".Equals(dr[9].ToString());




        //        }
        //        dr.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessNoProcedure") + " " + ex.Message, true);
        //        Unlock();

        //    }

        //    if (procedureName == null)
        //        procedureName = "";

        //    if (AD_Workflow_ID > 0)
        //    {
        //        StartWorkflow(AD_Workflow_ID);

        //        //Updated by raghu to open reports from work flow

        //        byte[] repByt = null;
        //        re = ReportCtl.Report;
        //        if (re != null)
        //        {
        //            int reportTable_ID = re.GetPrintFormat().GetAD_Table_ID();
        //            re.GetView();
        //            _pi.Set_AD_PrintFormat_Table_ID(re.GetPrintFormat().GetAD_Table_ID());
        //            _pi.SetSummary("Report", re != null);
        //            Unlock();
        //            if (re != null)
        //            {
        //                repByt = re.CreatePDF();
        //            }
        //        }
        //        else
        //        {
        //            repByt = null;
        //        }
        //        report = repByt;
        //        return _pi.ToList();
        //    }

        //    if (_pi.GetClassName() != null)
        //    {
        //        if (!StartProcess())
        //        {
        //            report = null;
        //            return _pi.ToList();
        //        }

        //        if (!IsReport && procedureName.Length == 0)
        //        {
        //            report = null;
        //            return _pi.ToList();
        //        }
        //    }
        //    report = null;
        //    if (IsReport)
        //    {
        //        if (procedureName.Length > 0)
        //        {
        //            if (!StartDBProcess(procedureName))
        //            {

        //            }
        //        }

        //        if (!IsCrystalReport)
        //        {
        //            //start report code
        //            //	Start Report	-----------------------------------------------
        //            re = ReportCtl.Start(_ctx, _pi, IsDirectPrint);
        //            _rep = re;
        //            int reportTable_ID = re.GetPrintFormat().GetAD_Table_ID();
        //            re.GetView();
        //            _pi.Set_AD_PrintFormat_Table_ID(re.GetPrintFormat().GetAD_Table_ID());
        //            _pi.Set_AD_PrintFormat_ID(re.GetPrintFormat().GetAD_PrintFormat_ID());
        //            _pi.SetSummary("Report", re != null);
        //            Unlock();
        //            if (re != null)
        //            {
        //                report = re.CreatePDF();
        //            }
        //        }
        //        else
        //        {
        //            _pi.SetIsCrystal(true);
        //            string errorMsg = null;
        //            CrystalReportEngine en = null;
        //            try
        //            {
        //                en = new CrystalReportEngine(_ctx, _pi);
        //               report = en.GenerateCrystalReport();
        //            }
        //            catch (Exception err)
        //            {
        //                errorMsg = err.Message;
        //            }

        //            _pi.SetSummary(errorMsg??"Done", errorMsg != null);
        //        }
        //    }
        //    else
        //    {
        //        if (!StartDBProcess(procedureName))
        //        {
        //            _pi.SetSummary("procedure ERROR");
        //            return _pi.ToList();
        //        }
        //        ProcessInfoUtil.SetSummaryFromDB(_pi);
        //    }

        //    return _pi.ToList();
        //}


        public const string ASSEMBLY_NAME = "ModelLibrary";

        Trx _trx;

        /// <summary>
        /// Starts the process by calling the required class at run time.
        /// </summary>
        /// <returns>Returs ture or false on the successfull calling of the proecss</returns>
        private bool StartProcess()
        {
            //_trx = Trx.Get("ServerProcess", true);
            //log.Fine(_pi.ToString());
            try
            {
                string className = _pi.GetClassName();

                /*Customization Process */

                Type type = null;
                string cName = _pi.GetTitle();

                type = Utility.ModuleTypeConatiner.GetClassType(className, cName);

                if (type == null)
                {
                    //MessageBox.Show("no Type");
                }

                if (type.IsClass)
                {
                    ProcessEngine.ProcessCall oClass = (ProcessEngine.ProcessCall)Activator.CreateInstance(type);
                    if (oClass == null)
                        return false;
                    else
                        //oClass.StartProcess(_ctx, _pi, _trx);
                        oClass.StartProcess(_ctx, _pi, _trx);

                    if (_trx != null)
                    {
                        _trx.Commit();
                        //log.Fine("Commit " + _trx.ToString());
                        _trx.Close();
                    }

                }
            }
            catch
            {
                if (_trx != null)
                {
                    _trx.Rollback();
                    //log.Fine("Rollback " + _trx.ToString());
                    _trx.Close();
                }
                _pi.SetSummary("Error starting Class " + _pi.GetClassName(), true);

                //log.Log(Level.SEVERE, _pi.GetClassName(), ex);
            }
            return !_pi.IsError();
        }

        public string ReportString
        {
            get;
            set;
        }

        ////public bool IsArabicReportFromOutside
        ////{
        ////    get;
        ////    set;
        ////}
        public bool GetIsPrintFormat()
        {
            return isPrintFormat;
        }
        //public void SetIsPrintFormat(bool _isPrintFormat)
        //{
        //    isPrintFormat = _isPrintFormat;
        //}
        //public int GetReprortTableID()
        //{
        //    return reportTable_ID;
        //}

        //public void SetAD_PrintFormat_ID(int _AD_PrintFormat_ID)
        //{
        //    ResAD_PrintFormat_ID = _AD_PrintFormat_ID;
        //}
        //public bool GetIsPrintCsv()
        //{
        //    return isPrintCsv;
        //}
        //public void SetIsPrintCsv(bool _isPrintCsv)
        //{
        //    isPrintCsv = _isPrintCsv;
        //}
        #region // 19-07-2014 Call process at run time



        /**	Parenr				*/
        ASyncProcess _parent;
        //private BackgroundWorker backgroundWorker1 = new BackgroundWorker();
        //private System.Windows.Forms.Timer tmrProcess = new System.Windows.Forms.Timer();
        static VLogger log = VLogger.GetVLogger(typeof(ProcessCtl).FullName);
        // bool showProgress = true;
        private bool _IsServerProcess = false;

        public ProcessCtl(Ctx ctx, ASyncProcess parent, ProcessInfo pi, Trx trx)
        {
            _ctx = ctx;
            _parent = parent;
            _pi = pi;
            _trx = trx;
            //this.Run();
            //tmrProcess.Interval = 100;
            //tmrProcess.Tick += new System.EventHandler(this.tmrProcess_Tick);
            //InitializeBackgoundWorker();
        }







        private void Lock()
        {
            if (_parent == null)
                return;
            else
            {
                Thread t = new Thread(delegate () { _parent.LockUI(_pi); });
                t.CurrentCulture = Utility.Env.GetLanguage(_ctx).GetCulture(Utility.Env.GetLoginLanguage(_ctx).GetAD_Language());
                t.CurrentUICulture = Utility.Env.GetLanguage(_ctx).GetCulture(Utility.Env.GetLoginLanguage(_ctx).GetAD_Language());
                t.Start();
            }

        }

        public void Run()
        {
            log.Fine("AD_PInstance_ID=" + _pi.GetAD_PInstance_ID() + ", Record_ID=" + _pi.GetRecord_ID());
            Lock();

            //	Get Process Information: Name, Procedure Name, ClassName, IsReport, IsDirectPrint
            String procedureName = "";
            int AD_ReportView_ID = 0;
            int AD_Workflow_ID = 0;
            bool IsReport = false;
            bool IsDirectPrint = false;
            //
            String sql = "SELECT p.Name, p.procedureName,p.Classname, p.AD_Process_ID,"		//	1..4  
                + " p.IsReport,p.IsDirectPrint,p.AD_ReportView_ID,p.AD_Workflow_ID,"		//	5..8
                + " CASE WHEN COALESCE(p.Statistic_Count,0)=0 THEN 0 ELSE p.Statistic_Seconds/p.Statistic_Count END,"
                + " p.IsServerProcess "
                + "FROM AD_Process p"
                + " INNER JOIN AD_PInstance i ON (p.AD_Process_ID=i.AD_Process_ID) "
                + "WHERE p.IsActive='Y'"
                + " AND i.AD_PInstance_ID=@pinstanceid";

            if (!Utility.Env.IsBaseLanguage(_ctx, "AD_Process"))//   GlobalVariable.IsBaseLanguage())
                sql = "SELECT t.Name, p.procedureName,p.Classname, p.AD_Process_ID,"		//	1..4  
                    + " p.IsReport, p.IsDirectPrint,p.AD_ReportView_ID,p.AD_Workflow_ID,"	//	5..8
                    + " CASE WHEN COALESCE(p.Statistic_Count,0)=0 THEN 0 ELSE p.Statistic_Seconds/p.Statistic_Count END CASE,"
                    + " p.IsServerProcess "
                    + "FROM AD_Process p"
                    + " INNER JOIN AD_PInstance i ON (p.AD_Process_ID=i.AD_Process_ID) "
                    + " INNER JOIN AD_Process_Trl t ON (p.AD_Process_ID=t.AD_Process_ID"
                        + " AND t.AD_Language='" + Utility.Env.GetAD_Language(_ctx) + "') "
                    + "WHERE p.IsActive='Y'"
                    + " AND i.AD_PInstance_ID=@pinstanceid";
            //
            IDataReader dr = null;
            try
            {
                SqlParameter[] param = new SqlParameter[1];
                param[0] = new SqlParameter("@pinstanceid", _pi.GetAD_PInstance_ID());
                dr = SqlExec.ExecuteQuery.ExecuteReader(sql, param);
                while (dr.Read())
                {
                    _pi.SetTitle(dr[0].ToString());
                    //if (m_waiting != null)
                    //    m_waiting.setTitle(_pi.getTitle());
                    procedureName = dr[1].ToString();

                    _pi.SetClassName(dr[2].ToString());

                    _pi.SetAD_Process_ID(Utility.Util.GetValueOfInt(dr[3].ToString()));
                    //	Report
                    if ("Y".Equals(dr[4].ToString()))
                    {
                        IsReport = true;
                        if ("Y".Equals(dr[5].ToString()) && !Ini.IsPropertyBool(Ini.P_PRINTPREVIEW))
                            IsDirectPrint = true;
                    }
                    AD_ReportView_ID = dr[6].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[6].ToString());
                    AD_Workflow_ID = dr[7].ToString() == "" ? 0 : Utility.Util.GetValueOfInt(dr[7].ToString());
                    //
                    _IsServerProcess = "Y".Equals(dr[9].ToString());
                }
                dr.Close();

            }
            catch (SqlException ex)
            {
                if (dr != null)
                    dr.Close();
                _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessNoProcedure") + " " + ex.Message, true);
                Unlock();
                log.Log(Level.SEVERE, "run", ex);
                return;
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                }
            }

            if (procedureName == null)
                procedureName = "";

            if (AD_Workflow_ID > 0)
            {
                StartWorkflow(AD_Workflow_ID);
                re = ReportCtl.Report;

                Unlock();
                return;
            }

            if (_pi.GetClassName() != null)
            {
                if (!StartProcess())
                {
                    Unlock();
                    return;
                }

                if (!IsReport && procedureName.Length == 0)
                {
                    Unlock();
                    return;
                }

                if (IsReport && AD_ReportView_ID == 0)
                {
                    Unlock();
                    return;
                }

            }

            //  If not a report, we need a prodedure name
            if (!IsReport && procedureName.Length == 0)
            {
                _pi.SetSummary(Msg.GetMsg(_ctx, "ProcessNoProcedure", true));
                Unlock();
                return;
            }

            if (IsReport)
            {
                if (procedureName.Length > 0)
                {
                    if (!StartDBProcess(procedureName))
                    {
                        Unlock();
                        return;
                    }
                }

                //start report code
                //	Start Report	-----------------------------------------------
                re = ReportCtl.Start(_ctx, _pi, IsDirectPrint);

                _pi.SetSummary("Report", re != null);
                Unlock();
                if (re != null && IsDirectPrint)
                {
                    re = null;
                    //ve =  new Viewer(re);
                    //new ReportDialog(re);
                }
            }
            else
            {
                if (!StartDBProcess(procedureName))
                {
                    Unlock();
                    return;
                }

                ProcessInfoUtil.SetSummaryFromDB(_pi);
                Unlock();
            }

        }

        #endregion

       
    }
}

namespace VAdvantage.Classes
{
    public interface ASyncProcess
    {
        /// <summary>
        /// Lock User Interface.
        /// Called from the Worker before processing
        /// @param pi process info
        /// </summary>
        /// <param name="pi"></param>
        void LockUI(ProcessInfo pi);


        /// <summary>
        /// Unlock User Interface.
        /// Called from the Worker when processing is done
        /// @param pi result of execute ASync call
        /// </summary>
        /// <param name="pi"></param>
        void UnlockUI(ProcessInfo pi);


        /// <summary>
        /// Is the UI locked (Internal method)
        ///  @return true, if UI is locked
        /// </summary>
        /// <returns></returns>
        bool IsUILocked();

        /// <summary>
        /// Method to be executed async.
        /// Called from the Worker
        /// @param pi ProcessInfo
        /// </summary>
        /// <param name="pi"></param>
        void ExecuteASync(ProcessInfo pi);


    }

    /// <summary>
    /// Refresh UI
    /// </summary>
    public interface ASyncRefreshUI
    {
        void RefreshUI(ProcessInfo pi);
    }
}
