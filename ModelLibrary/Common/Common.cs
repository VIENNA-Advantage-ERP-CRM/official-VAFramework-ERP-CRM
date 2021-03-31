using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.Print;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Common
{
    public class Common
    {
        static public List<string> lstTableName = null;
        static public bool ISTENATRUNNINGFORERP = false;
        public static string NONBUSINESSDAY = "@DateIsInNonBusinessDay@";

        public static string Password_Valid_Upto_Key = "PASSWORD_VALID_UPTO";
        public static string Failed_Login_Count_Key = "FAILED_LOGIN_COUNT";
       
        public static int GetPassword_Valid_Upto
        {
            get { return 3; }
        }
        public static int GetFailed_Login_Count
        {
            get { return 5; }
        }
        public static void GetAllTable()
        {

            lstTableName = new List<string>();
            DataSet ds = DB.ExecuteDataset("select tablename from ad_table where isactive='Y'");

            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 350)
                {
                    ISTENATRUNNINGFORERP = true;
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    lstTableName.Add(Convert.ToString(ds.Tables[0].Rows[i]["TABLENAME"]));
                }

            }

        }


        /// <summary>
        /// Based on Report ID, Generate report and then save it in physcal location
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="_trx"></param>
        /// <param name="AD_TableID"></param>
        /// <param name="recordID"></param>
        /// <param name="AD_UserID"></param>
        /// <param name="AD_ClientID"></param>
        /// <param name="nodeName"></param>
        /// <param name="windowID"></param>
        /// <param name="WFActivity"></param>
        /// <returns>file info.</returns>
        public FileInfo GetPdfReportForMail(Ctx ctx, Trx _trx, int AD_TableID, int recordID, int AD_UserID, int AD_ClientID, string nodeName, int windowID, int WFActivity)
        {
            try
            {

                int reportID = GetDoctypeBasedReport(ctx, AD_TableID, recordID, windowID, WFActivity);

                if (reportID == 0)
                {
                    VLogger.Get().Warning("No Report found on DocType and Window Tab. For Table=" + AD_TableID);
                    return null;
                }
                FileInfo temp = null;
                MProcess process = MProcess.Get(ctx, reportID);

                ProcessInfo pi = new ProcessInfo(nodeName, reportID,
                              AD_TableID, recordID);
                pi.SetAD_User_ID(AD_UserID);
                pi.SetAD_Client_ID(AD_ClientID);
                MPInstance pInstance = new MPInstance(process, recordID);
                //FillParameter(pInstance, trx);
                pi.SetAD_PInstance_ID(pInstance.GetAD_PInstance_ID());

                if (process.GetIsCrystalReport() == "Y")
                {
                    pi.SetIsCrystal(true);
                }
                else
                {
                    pi.SetIsCrystal(false);
                }

                pi.SetAD_ReportFormat_ID(process.GetAD_ReportFormat_ID());
                pi.SetAD_ReportMaster_ID(process.GetAD_ReportMaster_ID());
                process.ProcessIt(pi, _trx);

                IReportEngine repo = ReportRun(pi, ctx, _trx);
                byte[] bytes = repo.GetReportBytes();
                string repPath = repo.GetReportFilePath(true, out bytes);

                repPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + repPath;
                ctx.SetContext("FetchingDocReport", "N");
                if (!string.IsNullOrEmpty(repPath))
                {
                    temp = new FileInfo(repPath);
                    if (temp.Exists)
                    {
                        return temp;
                    }
                    return null;
                }
                return null;
            }
            catch
            {
                ctx.SetContext("FetchingDocReport", "N");
                return null;
            }
        }

        /// <summary>
        /// Based on TableID, RecordID, find the report to be run
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tableID"></param>
        /// <param name="record_ID"></param>
        /// <param name="windowID"></param>
        /// <param name="WFActivity"></param>
        /// <returns></returns>
        private int GetDoctypeBasedReport(Ctx ctx, int tableID, int record_ID, int windowID, int WFActivity)
        {
            #region To Override Default Process With Process Linked To Document Type

            string colName = "C_DocTypeTarget_ID";


            string sql1 = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + tableID + " AND ColumnName   ='C_DocTypeTarget_ID'";
            int id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            if (id < 1)
            {
                colName = "C_DocType_ID";
                sql1 = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + tableID + " AND ColumnName   ='C_DocType_ID'";
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql1));
            }

            if (id > 0)
            {

                string tableName = MTable.GetTableName(ctx, tableID);
                sql1 = "SELECT " + colName + ", AD_Org_ID FROM " + tableName + " WHERE " + tableName + "_ID =" + Util.GetValueOfString(record_ID);
                DataSet ds = DB.ExecuteDataset(sql1);

                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    // Check if Document Sequence has organization Level checked, if yes then get report from there.
                    // If Not, then try to get report from Document Type.
                    sql1 = @"SELECT AD_Sequence_No.Report_ID
                                From Ad_Sequence Ad_Sequence
                                JOIN C_Doctype C_Doctype
                                ON (C_Doctype.Docnosequence_Id =Ad_Sequence.Ad_Sequence_Id 
                                AND C_DocType.ISDOCNOCONTROLLED='Y')  
                                JOIN AD_Sequence_No AD_Sequence_No
                                On (Ad_Sequence_No.Ad_Sequence_Id=Ad_Sequence.Ad_Sequence_Id
                                AND Ad_Sequence_No.AD_Org_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0]["AD_Org_ID"]) + @")
                                JOIN AD_Process ON AD_Process.AD_Process_ID=AD_Sequence_No.Report_ID
                                Where C_Doctype.C_Doctype_Id     = " + Convert.ToInt32(ds.Tables[0].Rows[0][0]) + @"
                                And Ad_Sequence.Isorglevelsequence='Y' AND Ad_Sequence.IsActive='Y' AND AD_Process.IsActive='Y'";

                    object processID = DB.ExecuteScalar(sql1);
                    if (processID == DBNull.Value || processID == null || Convert.ToInt32(processID) == 0)
                    {
                        sql1 = "select Report_ID FRoM C_Doctype WHERE C_Doctype_ID=" + Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                        processID = DB.ExecuteScalar(sql1);
                    }
                    if (processID != DBNull.Value && processID != null && Convert.ToInt32(processID) > 0)
                    {
                        ctx.SetContext("FetchingDocReport", "Y");
                        return Convert.ToInt32(processID);
                    }
                    ctx.SetContext("FetchingDocReport", "N");
                    // If windowID is not available then find windowID in workflow activity
                    if (windowID == 0)
                    {
                        sql1 = @"SELECT  adt.AD_Window_ID, adt.TableName, adt.PO_Window_ID, " +
                                    "case when adwfa.AD_Window_ID is null then (select AD_WINDOW_ID from AD_WF_Activity where AD_WF_Process_ID = (select AD_WF_Process_ID from AD_WF_Activity where AD_WF_Activity_ID = adwfa.AD_WF_Activity_ID) and AD_WINDOW_ID is not null and rownum = 1) else adwfa.AD_Window_ID end as ActivityWindow " +
                                    "FROM AD_Table adt " +
                                    "LEFT JOIN AD_WF_Activity adwfa on adt.AD_Table_ID = adwfa.AD_Table_ID " +
                                    "WHERE adt.AD_Table_ID = " + tableID + " and adwfa.AD_WF_Activity_ID =  " + WFActivity + " ";

                        processID = DB.ExecuteScalar(sql1);
                        if (processID != DBNull.Value && processID != null && Convert.ToInt32(processID) > 0)
                        {
                            windowID = Convert.ToInt32(processID);
                        }
                        else
                        {
                            return 0;
                        }
                    }



                    // Chgeck if Report is linked to TAB of selected report.
                    sql1 = "SELECT AD_Process_ID FROM AD_Tab WHERE AD_Window_ID=" + windowID + " AND AD_Table_ID=" + tableID;
                    processID = DB.ExecuteScalar(sql1);
                    if (processID != DBNull.Value && processID != null && Convert.ToInt32(processID) > 0)
                    {
                        return Convert.ToInt32(processID);
                    }
                }
            }
            return 0;

            #endregion
        }

        /// <summary>
        /// Generate Report based on whatever type of report is selected.
        /// </summary>
        /// <param name="_pi"></param>
        public IReportEngine ReportRun(ProcessInfo _pi, Ctx p_ctx, Trx _trx)
        {
            //  start report code
            ///	Start Report	-----------------------------------------------
            ///	
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;
            string langu = p_ctx.GetAD_Language().Replace("_", "-");
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(langu);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(langu);

            IReportEngine rpe = null;
            bool isDocxFile = false;
            int AD_ReportFormat_ID = _pi.GetAD_ReportFormat_ID();

            //if("Y".Equals(DB.ExecuteScalar("SELECT IsCrystalReport FROM AD_Process WHERE AD_Process_ID = "+_pi.GetAD_Process_ID())))

            //Dynamic Report    
            if (_pi.GetAD_ReportMaster_ID() > 0)
            {
                String fqClassName = "", asmName = "";
                DataSet ds = DB.ExecuteDataset("SELECT ClassName,AssemblyName FROM AD_ReportMaster WHERE IsActive='Y' AND AD_ReportMaster_ID = " + _pi.GetAD_ReportMaster_ID());
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    fqClassName = ds.Tables[0].Rows[0]["ClassName"].ToString();
                    asmName = ds.Tables[0].Rows[0]["AssemblyName"].ToString();
                    rpe = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, _pi, _trx, asmName, fqClassName);
                }
                else
                {
                    VLogger.Get().Warning("Report Engine data not found Error -> InActive record");
                    rpe = null;
                }
            }

            else if (AD_ReportFormat_ID > 0)
            {
                try
                {
                    string lang = p_ctx.GetAD_Language().Replace("_", "-");

                    if ((AD_ReportFormat_ID > 0) && (lang == "ar-IQ"))
                    {
                        isDocxFile = true;
                        rpe = VAdvantage.ReportFormat.ReportFormatEngine.Get(p_ctx, _pi, true);

                    }
                    else
                    {
                        rpe = VAdvantage.ReportFormat.ReportFormatEngine.Get(p_ctx, _pi, false);
                    }
                }
                catch
                {
                    rpe = null;
                }

            }

            else if (_pi.GetIsCrystal())
            {
                //_pi.SetIsCrystal(true);
                try
                {
                    rpe = new VAdvantage.CrystalReport.CrystalReportEngine(p_ctx, _pi);
                }
                catch
                {
                    rpe = null;
                }
            }
            else
            {
                //If user Choose BI Report...................
                if ("B".Equals(DB.ExecuteScalar("SELECT IsCrystalReport FROM AD_Process WHERE AD_Process_ID = " + _pi.GetAD_Process_ID())))
                {
                    rpe = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, _pi, _trx, "VA039", "VA039.Classes.BIReportEngine");
                }
                else if ("J".Equals(DB.ExecuteScalar("SELECT IsCrystalReport FROM AD_Process WHERE AD_Process_ID = " + _pi.GetAD_Process_ID())))
                {
                    //isJasperReport = true;
                    rpe = VAdvanatge.Report.ReportEngine.GetReportEngine(p_ctx, _pi, _trx, "VA039", "VA039.Classes.JasperReportEngine");
                }
                else
                {
                    rpe = ReportCtl.Start(p_ctx, _pi, true);
                }
            }

            _pi.SetSummary("Report", rpe != null);
            System.Threading.Thread.CurrentThread.CurrentCulture = original;
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;
            //ReportCtl.Report = re;
            return rpe;
        }


        // Method added by mohit to get reports as byteArray. 19 April 2019

        /// <summary>
        /// Method to get byte array of report.
        /// </summary>
        /// <param name="ctx">Current Context.</param>
        /// <param name="AD_Process_ID">Process ID.</param>
        /// <param name="Name">Name of process.</param>
        /// <param name="AD_Table_ID">Table id for fetching report.</param>
        /// <param name="Record_ID">Record id.</param>
        /// <param name="WindowNo">Window Number if process fired from window.</param>
        /// <param name="recIDs">String of record id's/</param>
        /// <param name="fileType">Report type.</param>
        /// <param name="report">Out parameter to get byte array.</param>
        /// <returns>Returns Process Info list and report byte array.</returns>
        public Dictionary<string, object> GetReport(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, string recIDs, string fileType, out byte[] report, out string rptFilePath)
        {
            Dictionary<string, object> d = null;
            report = null;
            rptFilePath = null;

            // Create PInstance
            MPInstance instance = null;
            try
            {
                instance = new MPInstance(ctx, AD_Process_ID, Record_ID);
            }
            catch (Exception e)
            {
                VLogger.Get().SaveError("GetReport-Instance Save", e.Message);
                return d;
            }

            if (!instance.Save())
            {
                ValueNamePair vp = VLogger.RetrieveError();
                if (vp != null)
                {
                    VLogger.Get().SaveError("GetReport-Instance Save", vp.Name ?? vp.GetValue());
                }
                else
                {
                    VLogger.Get().SaveError("GetReport-Instance Save", "PInstanceNotSaved");
                }

                return d;
            }

            // Culture.
            string lang = ctx.GetAD_Language().Replace("_", "-");
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);


            // Process info
            ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID, recIDs);
            pi.SetFileType(fileType);
            TryPrintFromDocType(pi);

            try
            {

                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());
                ProcessCtl ctl = new ProcessCtl();
                d = new Dictionary<string, object>();
                d = ctl.Process(pi, ctx, out report, out rptFilePath);
                ctl.ReportString = null;

            }
            catch (Exception e)
            {
                VLogger.Get().SaveError("GetReport", e.Message);
                report = null;
            }
            return d;
        }

        /// <summary>
        /// if Doctype or targetDocType column exist in window, then check print format attached to that Doc type. and open that one.
        /// </summary>
        /// <param name="_pi">Process Info.</param>
        private static void TryPrintFromDocType(ProcessInfo _pi)
        {
            try
            {
                string colName = "C_DocTypeTarget_ID";
                string sql = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + _pi.GetTable_ID() + " AND ColumnName   ='C_DocTypeTarget_ID'";
                int id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id < 1)
                {
                    colName = "C_DocType_ID";
                    sql = "SELECT COUNT(*) FROM AD_Column WHERE AD_Table_ID=" + _pi.GetTable_ID() + " AND ColumnName   ='C_DocType_ID'";
                    id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                    if (id < 1)
                    {
                        return;
                    }
                }
                string tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + _pi.GetTable_ID()));
                sql = "SELECT " + colName + " FROM " + tableName + " WHERE " + tableName + "_ID =" + _pi.GetRecord_ID();
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id < 1)
                {
                    return;
                }
                sql = "SELECT AD_ReportFormat_ID FROM C_DocType WHERE C_DocType_ID=" + id;
                id = Util.GetValueOfInt(DB.ExecuteScalar(sql));
                if (id > 0)
                {
                    _pi.SetAD_ReportFormat_ID(id);
                }

            }
            catch
            {
                return;
            }
        }


        /// <summary>
        ///  If validity is unknown but context  available, then get from context
        ///  if validity and context, both are unknown, the go with static values
        ///  Otherwise supply password validity
        /// </summary>
        /// <param name="newPwd"></param>
        /// <param name="AD_User_ID"></param>
        /// <param name="UpdatedBy"></param>
        /// <param name="passwordValidity"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static bool UpdatePasswordAndValidity(string newPwd, int AD_User_ID, int UpdatedBy, int passwordValidity = -1, Ctx ctx = null)
        {
            //If validity is unknow but context  available, then get from context
            if (passwordValidity == -1 && ctx != null)
            {
                passwordValidity = ctx.GetContextAsInt("#" + Common.Password_Valid_Upto_Key);
            }

            else if (passwordValidity == -1 && ctx == null)// if validity and context, both are unknown, the go with static values
            {
                passwordValidity = GetPassword_Valid_Upto;
            }
            //ELSE
            // Password validity is supllied.
            //


            //Check if User's pwd is to be encrypted or not
            if (DB.ExecuteScalar("SELECT IsEncrypted from AD_Column WHERE AD_Column_ID=" + 417).ToString().Equals("Y"))
                newPwd = SecureEngine.Encrypt(newPwd);

            string newpwdExpireDate = GlobalVariable.TO_DATE(DateTime.Now.AddMonths(passwordValidity), true);

            string sql = "UPDATE AD_User set Updated=Sysdate,UpdatedBy=" + UpdatedBy + ",PasswordExpireOn=" + newpwdExpireDate + ",password='" + newPwd + "' WHERE AD_User_ID=" + AD_User_ID;
            int count = DB.ExecuteQuery(sql);
            if (count > 0)
                return true;
            return false;

        }
        /// <summary>
        /// Validate Password to check if meet all requirements.
        /// if saving new passwqord, then oldpassword value must be null in parameters.
        /// </summary>
        /// <param name="oldPassword"></param>
        /// <param name="NewPassword"></param>
        /// <param name="ConfirmNewPasseword"></param>
        /// <returns></returns>
        public static string ValidatePassword(string oldPassword, string NewPassword, string ConfirmNewPasseword)
        {
            if (NewPassword == null)
            {
                return "mustMatchCriteria";
            }
            if (NewPassword != null && !NewPassword.Equals(ConfirmNewPasseword))
            {
                return "BothPwdNotMatch";
            }
            if (oldPassword != null && oldPassword.Equals(NewPassword))
            {
                return "oldNewSamePwd";
            }
            string regex = @"(^(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9])(?=.*[@$!%*?&])[a-zA-Z][A-Za-z\d@$!%*?& ]{4,}$)";// Start with Alphabet, minimum 4 length
                                                                                                                    //@$!%*#?& allowed only
            Regex re = new Regex(regex);

            // The IsMatch method is used to validate 
            // a string or to ensure that a string 
            // conforms to a particular pattern. 
            if (!re.IsMatch(NewPassword))
            {
                return "mustMatchCriteria";
            }

            return "";
        }

        /// <summary>
        /// Function will check if Action need to save or not. 
        /// If Yes, then save information in table.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="ActionOrigin">Action Origin(Menu, Window, Form)</param>
        /// <param name="OriginName">(Form name or window name)</param>
        /// <param name="AD_Table_ID">AD_Table_ID of table from where action intiated</param>
        /// <param name="Record_ID">Selected Record_ID</param>
        /// <param name="Process_ID">AD_Process_ID with which report is linked</param>
        /// <param name="ProcessName">Name of Process</param>
        /// <param name="fileType">Requested file type(PDF, CSV, RTF)</param>
        /// <param name="description">Desciption like filename or anything else</param>
        /// <param name="ActionType">Action type.(Viewed Or Downloaded)</param>
        public static void SaveActionLog(Ctx ctx, string ActionOrigin, string OriginName, int AD_Table_ID, int Record_ID,
           int processID, string ProcessName, string fileType, string description, string ActionType)
        {
            //Save Action Log key is fetched from System Config window
            string canSave = Util.GetValueOfString(ctx.GetContext("#SAVE_ACTION_LOG"));

            if (!canSave.Equals("Y"))
                return;

            string reportTypeForLog = MActionLog.ACTIONTYPE_View;
            string descriptonForLog = "Report Viewed";

            if (!string.IsNullOrEmpty(description))
            {
                descriptonForLog = description;
            }
            if (!string.IsNullOrEmpty(ActionType))
            {
                reportTypeForLog = ActionType;
            }
            // As PDF viewed in Browser, so PDF report Viewed message will be saved.
            else if (fileType.Equals(ProcessCtl.ReportType_PDF))
            {
                reportTypeForLog = MActionLog.ACTIONTYPE_View;
                descriptonForLog = "PDF Report Viewed";
            }
            else if (fileType.Equals(ProcessCtl.ReportType_CSV))
            {
                reportTypeForLog = MActionLog.ACTIONTYPE_Download;
                descriptonForLog = "CSV Report Downloaded";
            }
            else if (fileType.Equals(ProcessCtl.ReportType_RTF))
            {
                reportTypeForLog = MActionLog.ACTIONTYPE_Download;
                descriptonForLog = "RTF Report Downloaded";
            }
            else if (fileType.Equals(ProcessCtl.ReportType_HTML))
            {
                reportTypeForLog = MActionLog.ACTIONTYPE_View;
                descriptonForLog = "Report Viewed";
            }
            if (processID > 0)
            {
                descriptonForLog += ", Process Name:->" + ProcessName;
            }

            MSession sess = MSession.Get(ctx);
            sess.ActionLog(ctx, sess.GetAD_Session_ID(), ctx.GetAD_Client_ID(), ctx.GetAD_Org_ID(),
               ActionOrigin, reportTypeForLog, OriginName, descriptonForLog, AD_Table_ID, Record_ID);
        }

      


    }

}
