using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using VAdvantage.Classes;
using VAdvantage.Controller;
using VAdvantage.DataBase;
using VAdvantage.Model;
using VAdvantage.Utility;
using Newtonsoft.Json;

using VIS.Helpers;
using VIS.DataContracts;
using VIS.Classes;
using VIS.Models;
using System.Web.Mvc;
using VIS.Filters;
using System.Web.SessionState;
using ViennaAdvantage.Model;
using System.Web.Script.Serialization;
using System.Text;
using System.Threading.Tasks;
using VISLogic.Models;

namespace VIS.Controllers
{
    /// <summary>
    /// common class to handle json data request 
    /// </summary>
    /// 
    [AjaxAuthorizeAttribute] // redirect to login page if reques`t is not Authorized
    [AjaxSessionFilterAttribute] // redirect to Login/Home page if session expire
    [AjaxValidateAntiForgeryToken] // validate antiforgery token 
    //[SessionState(SessionStateBehavior.ReadOnly)]
    // [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]

    public class JsonDataController : Controller
    {

        private static Dictionary<string, string> toastrMessage = new Dictionary<string, string>();
        static readonly object _object = new object();
        public JsonResult UpdateCtx(Dictionary<string, object> dCtx)
        {

            string ip = Request.UserHostAddress;
            string hostName = Request.UserHostName;
            if (Session["Ctx"] != null)
            {
                var ctx = Session["ctx"] as Ctx;
                if (dCtx != null && dCtx.Count > 0)
                {
                    foreach (var i in dCtx)
                    {
                        ctx.SetContext(i.Key, (string)i.Value);
                    }
                }
                ctx.SetContext("IPAddress", ip);
                ctx.SetContext("HostName", hostName);
            }
            else
            {

            }
            return Json(new { result = "" }, JsonRequestBehavior.AllowGet);
        }

        #region Window

        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetGridWindow(int windowNo, int AD_Window_ID)
        {
            GridWindow wVo = null;
            string retJSON = "";
            string retError = null;
            string windowCtx = "";

            if (Session["ctx"] != null)
            {
                Ctx lctx = Session["ctx"] as Ctx;
                Ctx ctx = new Ctx(lctx.GetMap());
                GridWindowVO vo = AEnv.GetMWindowVO(ctx, windowNo, AD_Window_ID, 0);
                if (vo != null)
                {
                    try
                    {
                        wVo = new GridWindow(vo);
                        retJSON = JsonConvert.SerializeObject(wVo, Formatting.None);
                    }
                    catch (Exception ex)
                    {
                        retError = ex.Message;
                    }
                }
                else
                {
                    retError = "AccessTableNoView";
                }
                windowCtx = JsonConvert.SerializeObject(ctx.GetMap(windowNo));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError, wCtx = windowCtx }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetWindowRecords(List<string> fields, SqlParamsIn sqlIn, int rowCount, string sqlCount, int AD_Table_ID, List<string> obscureFields)
        {
            object data = null;
            if (Session["ctx"] == null)
            {

            }
            else
            {
                using (var w = new WindowHelper())
                {
                    Ctx ctx = Session["ctx"] as Ctx;
                    sqlIn.sql = SecureEngineBridge.DecryptByClientKey(sqlIn.sql, ctx.GetSecureKey());
                    sqlIn.sqlDirect = SecureEngineBridge.DecryptByClientKey(sqlIn.sqlDirect, ctx.GetSecureKey());
                    sqlCount = SecureEngineBridge.DecryptByClientKey(sqlCount, ctx.GetSecureKey());
                    sqlIn.sql = Server.HtmlDecode(sqlIn.sql);
                    sqlIn.sqlDirect = Server.HtmlDecode(sqlIn.sqlDirect);
                    data = w.GetWindowRecords(sqlIn, fields, ctx, rowCount, sqlCount, AD_Table_ID, obscureFields);

                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Total card record count 
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public int GetRecordCountWithCard(string sql, int cardID) {
            int count = 0;
            using (var w = new WindowHelper())
            {
                Ctx ctx = Session["ctx"] as Ctx;
                sql = SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
                count= w.GetRecordCountWithCard(sql, cardID);
            }
                return count;
        }

        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public JsonResult GetWindowRecord(string sql, List<string> fields, List<string> obscureFields)
        {
            object data = null;
            if (Session["ctx"] == null)
            {
            }
            else
            {
                using (var w = new WindowHelper())
                {
                    Ctx ctx = Session["ctx"] as Ctx;
                    sql = Server.HtmlDecode(sql);
                    sql = SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
                    data = w.GetWindowRecord(sql, fields, ctx, obscureFields);
                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetWindowRecordsForTreeNode(List<string> fields, SqlParamsIn sqlIn, int rowCount, string sqlCount, int AD_Table_ID, int treeID, int treeNodeID, List<string> obscureFields)
        {
            object data = null;
            if (Session["ctx"] == null)
            {

            }
            else
            {
                using (var w = new WindowHelper())
                {
                    Ctx ctx = Session["ctx"] as Ctx;
                    sqlIn.sql = SecureEngineBridge.DecryptByClientKey(sqlIn.sql, ctx.GetSecureKey());
                    sqlIn.sql = Server.HtmlDecode(sqlIn.sql);
                    sqlCount = SecureEngineBridge.DecryptByClientKey(sqlCount, ctx.GetSecureKey());
                    data = w.GetWindowRecordsForTreeNode(sqlIn, fields, ctx, rowCount, sqlCount, AD_Table_ID, treeID, treeNodeID, obscureFields);
                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// retrun Grid window Model json object against window Id
        /// </summary>
        /// <param name="windowNo">window number</param>
        /// <param name="AD_Window_ID">window Id</param>
        /// <returns>grid window json result</returns>
        public JsonResult GetRecordCountForTreeNode(string sqlIn, int AD_Table_ID, int treeID, int treeNodeID, int windowNo, bool summaryOnly)
        {
            object data = null;
            if (Session["ctx"] == null)
            {

            }
            else
            {
                using (var w = new WindowHelper())
                {
                    sqlIn = Server.HtmlDecode(sqlIn);
                    data = w.GetRecordCountForTreeNode(sqlIn, Session["ctx"] as Ctx, AD_Table_ID, treeID, treeNodeID, windowNo, summaryOnly);
                }
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save or Update Window Record
        /// </summary>
        /// <param name="gridTable">Data Contract </param>
        /// <returns>json result </returns>
        public JsonResult InsertOrUpdateWRecords(SaveRecordIn gridTable)
        {
            try
            {
                SaveRecordOut gOut = null;
                if (Session["ctx"] == null)
                {
                    gOut = new SaveRecordOut();
                    gOut.IsError = true;
                    gOut.ErrorMsg = "Session Expired";
                }
                else
                {
                    using (var w = new WindowHelper())
                    {
                        Ctx ctx = Session["ctx"] as Ctx;
                        gridTable.SelectSQL = SecureEngineBridge.DecryptByClientKey(gridTable.SelectSQL, ctx.GetSecureKey());

                        gOut = w.InsertOrUpdateRecord(gridTable, Session["ctx"] as Ctx);
                    }
                }
                return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                SaveRecordOut o = new SaveRecordOut();
                o.IsError = true;
                o.ErrorMsg = e.Message;
                return Json(JsonConvert.SerializeObject(o), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Delete Window Record(s) 
        ///  - handle error or restriction 
        ///  - delete multiple or single record
        /// </summary>
        /// <param name="dInn">data contract</param>
        /// <returns>json result</returns>
        public JsonResult DeleteWRecords(DeleteRecordIn dInn)
        {
            DeleteRecordOut gOut = null;
            if (Session["ctx"] == null)
            {
                gOut = new DeleteRecordOut();
                gOut.IsError = true;
                gOut.ErrorMsg = "Session Expired";
            }
            else
            {
                using (var w = new WindowHelper())
                {
                    gOut = w.DeleteRecord(dInn, Session["ctx"] as Ctx);
                }
            }
            return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRecordInfo(RecordInfoIn dse)
        {
            RecordInfoOut gOut = null;
            using (var w = new WindowHelper())
            {
                gOut = w.GetRecordInfo(dse, Session["ctx"] as Ctx);
            }
            return Json(JsonConvert.SerializeObject(gOut), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateOrInsertPersonalLock(int AD_User_ID, int AD_Table_ID, int Record_ID, bool locked)
        {
            MPrivateAccess access = MPrivateAccess.Get(Session["ctx"] as Ctx, AD_User_ID, AD_Table_ID, Record_ID);
            if (access == null)
            {
                access = new MPrivateAccess(Session["ctx"] as Ctx, AD_User_ID, AD_Table_ID, Record_ID);
            }
            access.SetIsActive(locked);
            bool ret = access.Save();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Form

        /// <summary>
        /// get form Info agianst form Id 
        /// - call from menu form item
        /// </summary>
        /// <param name="AD_Form_ID">id of form</param>
        /// <returns>json result</returns>
        public JsonResult GetFormInfo(int AD_Form_ID)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                retJSON = JsonConvert.SerializeObject(FormHelper.GetFormInfo(AD_Form_ID, Session["ctx"] as Ctx));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Process

        /// <summary>
        /// Get Process Info
        /// - call from process menu Item 
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <returns>json result</returns>
        public JsonResult GetProcessInfo(int AD_Process_ID)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                retJSON = JsonConvert.SerializeObject(ProcessHelper.GetProcessInfo(AD_Process_ID, Session["ctx"] as Ctx));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// execute process 
        /// - return process parameter fields if has any, halt processing
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <param name="Name">name of process</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record  id of table</param>
        /// <param name="WindowNo">window number</param>
        /// <returns>json result
        /// - either complete or uncomplete message 
        /// - or list process fields
        /// </returns>
        /// vinay bhatt window id
        public JsonResult Process(Dictionary<string, string> processInfo)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                //int pID = GetDoctypeBasedReport(Session["ctx"] as Ctx, Convert.ToInt32(processInfo["AD_Table_ID"]), Convert.ToInt32(processInfo["Record_ID"]));
                //if (pID > 0)
                //{
                //    processInfo["Process_ID"] = pID.ToString();
                //}
                retJSON = JsonConvert.SerializeObject(ProcessHelper.Process(Session["ctx"] as Ctx, processInfo));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// execute process   
        /// - call after parameter dialog window
        /// - first save parameter for Process then execute
        /// </summary>
        /// <param name="AD_Process_ID">id of process</param>
        /// <param name="Name">name of process</param>
        /// <param name="AD_PInstance_ID">process instance id</param>
        /// <param name="AD_Table_ID">table id</param>
        /// <param name="Record_ID">record id</param>
        /// <param name="ParameterList">process parameter list</param>
        /// <returns>json result</returns>
        /// 
        public async Task<JsonResult> ExecuteProcess(Dictionary<string, string> processInfo, ProcessPara[] parameterList)
        {
            return await System.Threading.Tasks.Task.Run(() => ExecuteProcesAsync(processInfo, parameterList));
        }

        public JsonResult ExecuteProcesAsync(Dictionary<string, string> processInfo, ProcessPara[] parameterList)
        {
            string retJSON = "";
            string retError = null;

            if (Session["ctx"] != null)
            {
                //int pID = GetDoctypeBasedReport(Session["ctx"] as Ctx, Convert.ToInt32(processInfo["AD_Table_ID"]), Convert.ToInt32(processInfo["Record_ID"]));
                //if (pID > 0)
                //{
                //    processInfo["Process_ID"] = pID.ToString();
                //}
                //csv = true;
                //pdf = false;
                ProcessReportInfo rep = ProcessHelper.ExecuteProcessCommon(Session["ctx"] as Ctx, processInfo, parameterList);
                //rep.HTML=GetHtml(rep.ReportFilePath);
                //if (rep.Report != null && (rep.Report.Length > 1048576 || Record_ID > 0) && !csv && !pdf)
                if (rep.Report != null && (rep.Report.Length > 1048576 || Util.GetValueOfInt(rep.ReportProcessInfo["Record_ID"]) > 0))
                {
                    rep.AskForNewTab = true;
                    string filePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload" + "\\temp_" + DateTime.Now.Ticks;
                    if (processInfo["FileType"] == "C")
                    {
                        filePath += ".csv";
                    }
                    else
                    {
                        filePath += ".pdf";
                    }

                    System.IO.File.WriteAllBytes(filePath, rep.Report);
                    rep.ReportFilePath = filePath.Substring(filePath.IndexOf("TempDownload"));
                    rep.HTML = null;
                    rep.Report = null;
                }
                //if (rep.Report != null)
                //{
                //    rep.byteString = Convert.ToBase64String(rep.Report);
                //}

                retJSON = JsonConvert.SerializeObject(rep);
                //retJSON = JsonConvert.SerializeObject(ProcessHelper.ExecuteProcess(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_PInstance_ID, AD_Table_ID, Record_ID, ParameterList));
            }
            else
            {
                retError = "SessionExpired";
            }
            return Json(new { result = retJSON, error = retError }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// Select report info based on Document type selected in that particular record.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tableID"></param>
        /// <param name="record_ID"></param>
        /// <returns></returns>
        private int GetDoctypeBasedReport(Ctx ctx, int tableID, int record_ID)
        {
            string tableName = MTable.GetTableName(ctx, tableID);
            #region To Override Default Process With Process Linked To Document Type

            string colName = "C_DocTypeTarget_ID";
            int invoiceReportID = VAdvantage.Common.Common.GetBusinessInvoiceReportID(ctx, tableID, record_ID);
            if (invoiceReportID > 0)
            {
                string lang = VAdvantage.Common.Common.GetCustomerLanguage(ctx, tableID, record_ID);
                if (lang != "" && ctx.GetContext("#AD_Language") != lang)
                {
                    ctx.SetContext("Report_Lang", lang);
                }
                return invoiceReportID;
            }

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
                        return Convert.ToInt32(processID);
                    }
                }
            }
            return 0;

            #endregion
        }

        /// <summary>
        /// Get Report Language from Coustomer Master
        /// </summary>
        /// <param name="tableID"></param>
        /// <param name="record_ID"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private string GetCustomerLanguage(int tableID, int record_ID, string tableName)
        {
            string lang = "";
            try
            {
                lang = Util.GetValueOfString(DB.ExecuteScalar("SELECT AD_Language FROM C_BPartner WHERE C_BPartner_ID=(SELECT C_BPartner_ID FROM " + tableName + " WHERE " + tableName + "_ID=" + record_ID + ")"));
            }
            catch (Exception ex)
            {

            }

            return lang;
        }

        #endregion

        #region "Dataset"

        //private string GetHtml(string pdfFilePath)
        //{


        //    string pathToPdf = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + pdfFilePath;
        //    string pathToHtml = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "TempDownload\\temp" + DateTime.Now.Ticks + ".html";

        //    // Convert PDF file to HTML file
        //    SautinSoft.PdfFocus f = new SautinSoft.PdfFocus();
        //    // Let's force the component to store images inside HTML document
        //    // using base-64 encoding
        //    f.HtmlOptions.IncludeImageInHtml = true;
        //    f.HtmlOptions.Title = "Simple text";

        //    // This property is necessary only for registered version
        //    //f.Serial = "XXXXXXXXXXX";

        //    f.OpenPdf(pathToPdf);

        //    if (f.PageCount > 0)
        //    {
        //      //  f.ToHtml(pathToHtml);
        //        return f.ToHtml();

        //    }
        //    return null;
        //}
        /// <summary>
        /// json object representation of dataset
        /// --has table or tables -
        /// -- 
        /// </summary>
        /// <param name="sqlIn">sql param datacontract </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult JDataSet(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            Ctx ctx = Session["ctx"] as Ctx;

            h.SetContext(ctx);

            /// sqlIn.sql = SecureEngineBridge.DecryptByClientKey(sqlIn.sql, ctx.GetSecureKey());

            //sqlIn.sql = VAdvantage.Utility.SecureEngine.Decrypt(sqlIn.sql);

            sqlIn.sql = Server.HtmlDecode(sqlIn.sql);
            object data = h.ExecuteJDataSet(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// json object representation of dataset
        /// --has table or tables -
        /// -- 
        /// </summary>
        /// <param name="sqlIn">sql param datacontract </param>
        /// <returns>
        /// json dataset</returns>
        /// 

        public async System.Threading.Tasks.Task<JsonResult> JDataSetWithCode(SqlParamsIn sqlIn)
        {
            return await System.Threading.Tasks.Task.Run(() => JDataSetWithCodeAsync(sqlIn));
        }
        public JsonResult JDataSetWithCodeAsync(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            Ctx ctx = Session["ctx"] as Ctx;

            h.SetContext(ctx);
            if (sqlIn.sql.IndexOf("~") > -1)
            {
                string[] sql = sqlIn.sql.Split('~');
                for (int i = 0; i < sql.Length; i++)
                {
                    if (i == 0)
                    {
                        sqlIn.sql = QueryCollection.GetQuery(sql[i], ctx);
                    }
                    else
                    {
                        sqlIn.sql += "~" + QueryCollection.GetQuery(sql[i], ctx);
                    }
                }

            }
            else
            {
                sqlIn.sql = QueryCollection.GetQuery(sqlIn.sql, ctx);
            }
            object data = h.ExecuteJDataSet(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQueriesWithCode(string sql, List<List<SqlParams>> param)
        {
            SqlHelper h = new SqlHelper();
            Ctx ctx = Session["ctx"] as Ctx;
            h.SetContext(ctx);
            //  sql = SecureEngineBridge.DecryptByClientKey(sql, ctx.GetSecureKey());
            sql = QueryCollection.GetQuery(sql, ctx);
            object data = h.ExecuteNonQueries(sql, param);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQueryWithCode(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            Ctx ctx = Session["ctx"] as Ctx;
            h.SetContext(ctx);
            //sqlIn.sql = SecureEngineBridge.DecryptByClientKey(sqlIn.sql, ctx.GetSecureKey());
            sqlIn.sql = QueryCollection.GetQuery(sqlIn.sql, ctx);
            object data = h.ExecuteNonQuery(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQueries(string sql, List<List<SqlParams>> param)
        {
            SqlHelper h = new SqlHelper();
            object data = h.ExecuteNonQueries(sql, param);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 
        /// Execute Multiple queries and return result of each query in a list
        /// 
        /// </summary>
        /// <param name="sql">sql splitted by '/'</param>
        /// <param name="lstParams">Parameters for each query </param>
        /// <returns>
        /// json dataset</returns>
        public JsonResult ExecuteNonQuery(SqlParamsIn sqlIn)
        {
            SqlHelper h = new SqlHelper();
            object data = h.ExecuteNonQuery(sqlIn);
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Others"
        public JsonResult GetLookup(Dictionary<string, object> ctx, int windowNo, int column_ID, int AD_Reference_ID, string columnName,
            int AD_Reference_Value_ID, bool isParent, string validationCode)
        {
            Ctx _ctx = new Ctx(ctx);
            //Ctx _ctx = null;//(ctx) as Ctx;
            Lookup res = LookupHelper.GetLookup(_ctx, windowNo, column_ID, AD_Reference_ID, columnName,
                AD_Reference_Value_ID, isParent, validationCode);
            return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
        }


        public async System.Threading.Tasks.Task<JsonResult> GenerateReport(Dictionary<string, string> processInfo, string queryInfo, Object code, bool isCreateNew, int? treeID, int? node_ID, bool IsSummary)
        {
            return await System.Threading.Tasks.Task.Run(() => GenerateReportAsync(processInfo, queryInfo, code, isCreateNew, treeID, node_ID, IsSummary));
        }

        [HttpPost]
        public JsonResult GenerateReportAsync(Dictionary<string, string> processInfo, string queryInfo, Object code, bool isCreateNew, int? treeID, int? node_ID, bool IsSummary)
        {
            if (Session["ctx"] != null)
            {
                //int pID = GetDoctypeBasedReport(Session["ctx"] as Ctx, Convert.ToInt32(processInfo["AD_Table_ID"]), Convert.ToInt32(processInfo["Record_ID"]));
                //if (pID > 0)
                //{
                //    processInfo["Process_ID"] = pID.ToString();
                //}
                List<string> qryInfo = JsonConvert.DeserializeObject<List<string>>(queryInfo);
                ProcessHelper pHe = new ProcessHelper();
                GridReportInfo rep = (pHe.GenerateReport(Session["ctx"] as Ctx, Util.GetValueOfInt(processInfo["AD_PrintFormat_ID"]), qryInfo, code, isCreateNew, processInfo, Util.GetValueOfInt(processInfo["PageNo"]), Util.GetValueOfInt(processInfo["AD_PInstance_ID"]), Util.GetValueOfString(processInfo["FileType"]), node_ID, treeID, IsSummary));
                return Json(JsonConvert.SerializeObject(rep), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }
        }

        public async System.Threading.Tasks.Task<JsonResult> GeneratePrint(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, string filetype, string actionOrigin, string originName)
        {
            return await System.Threading.Tasks.Task.Run(() => GeneratePrintAsync(AD_Process_ID, Name, AD_Table_ID, Record_ID, WindowNo, filetype, actionOrigin, originName));
        }
        public JsonResult GeneratePrintAsync(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, string filetype, string actionOrigin, string originName)
        {
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                int pID = GetDoctypeBasedReport(ctx, AD_Table_ID, Record_ID);
                if (pID > 0)
                {
                    ctx.SetContext("FetchingDocReport", "Y");
                    AD_Process_ID = pID;
                }
                ProcessReportInfo rep = (ProcessHelper.GeneratePrint(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_Table_ID, Record_ID, WindowNo, "", filetype, actionOrigin, originName));
                ctx.SetContext("FetchingDocReport", "N");
                ctx.SetContext("Report_Lang", "");
                return Json(JsonConvert.SerializeObject(rep), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }

        /// <summary>
        /// This function executes when user select multiple records from window and click print button
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <param name="Name"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="RecIDs"></param>
        /// <param name="WindowNo"></param>
        /// <param name="filetype"></param>
        /// <returns></returns>
        public JsonResult GenerateMultiPrint(int AD_Process_ID, string Name, int AD_Table_ID, string RecIDs, int WindowNo, string filetype, string actionOrigin, string originName)
        {
            if (Session["ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                ctx.SetContext("FetchingDocReport", "Y");
                int Record_ID = Convert.ToInt32(RecIDs.Split(',')[0]);
                int pID = GetDoctypeBasedReport(ctx, AD_Table_ID, Record_ID);
                if (pID > 0)
                {
                    AD_Process_ID = pID;
                }
                ProcessReportInfo rep = (ProcessHelper.GeneratePrint(ctx, AD_Process_ID, Name, AD_Table_ID, 0, WindowNo, RecIDs, filetype, actionOrigin, originName));
                ctx.SetContext("FetchingDocReport", "N");
                return Json(JsonConvert.SerializeObject(rep), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }




        public JsonResult ArchiveDoc(int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int C_BPartner_ID, bool isReport, byte[] binaryData, string reportPath)
        {
            if (Session["ctx"] != null)
            {
                return Json(JsonConvert.SerializeObject(ProcessHelper.ArchiveDoc(Session["ctx"] as Ctx, AD_Process_ID, Name, AD_Table_ID, Record_ID, C_BPartner_ID, isReport, binaryData, reportPath)), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AttachFrom(string docID, int winID, int tableID, int recID)
        {
            bool isSuccessFullAttach = false;
            bool isAlreadyAttach = false;
            if (Session["ctx"] != null)
            {
                string[] strDocIds = docID.Split(',');
                string[] strMetaId = null;
                if (strDocIds.Count() > 0)
                {
                    for (int j = 0; j < strDocIds.Count(); j++)
                    {
                        strMetaId = strDocIds[j].Split('-');
                        Ctx ctx = Session["ctx"] as Ctx;
                        string sql1 = "Select count(dlink.VADMS_WindowDocLink_ID) FROM VADMS_WindowDocLink dlink INNER JOIN vadms_attachmetadata amd ON amd.VADMS_WindowDocLink_ID = dlink.VADMS_WindowDocLink_ID where dlink.AD_Table_ID=" + tableID + " AND dlink.record_ID=" + recID + " AND dlink.AD_Window_ID=" + winID + " AND dlink.VADMS_Document_ID=" + strMetaId[0] + " AND amd.VADMS_MetaData_ID=" + strMetaId[1];
                        int count = Convert.ToInt32(DB.ExecuteScalar(sql1));
                        if (count > 0)
                        {
                            return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                        }
                        string sql = "Select VADMS_WindowDocLink_ID from VADMS_WindowDocLink where AD_Table_ID=" + tableID + " AND record_ID=" + recID + " AND AD_Window_ID=" + winID + " AND VADMS_Document_ID=" + strMetaId[0];
                        int ID = Convert.ToInt32(DB.ExecuteScalar(sql));
                        if (ID > 0)
                        {
                            //return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                            isAlreadyAttach = true;
                        }
                        VAdvantage.Model.X_VADMS_WindowDocLink wlink = null;
                        if (!isAlreadyAttach)
                        {
                            wlink = new VAdvantage.Model.X_VADMS_WindowDocLink(ctx, 0, null);
                        }
                        else
                        {
                            wlink = new VAdvantage.Model.X_VADMS_WindowDocLink(ctx, ID, null);
                        }
                        wlink.SetAD_Client_ID(ctx.GetAD_Client_ID());
                        wlink.SetAD_Org_ID(ctx.GetAD_Org_ID());
                        wlink.SetAD_Table_ID(tableID);
                        wlink.SetAD_Window_ID(winID);
                        wlink.SetRecord_ID(recID);
                        if (strDocIds[j].Trim() != string.Empty)
                        {
                            wlink.SetVADMS_Document_ID(Convert.ToInt32(strMetaId[0]));
                        }
                        if (wlink.Save())
                        {

                            X_VADMS_AttachMetaData objAttachMetaData = new X_VADMS_AttachMetaData(ctx, 0, null);
                            objAttachMetaData.SetVADMS_WindowDocLink_ID(wlink.Get_ID());
                            objAttachMetaData.SetVADMS_Document_ID(wlink.GetVADMS_Document_ID());
                            objAttachMetaData.SetVADMS_MetaData_ID(Convert.ToInt32(strMetaId[1]));
                            //objAttachMetaData.SetRecord_ID(wlink.GetRecord_ID());
                            if (objAttachMetaData.Save())
                            {
                                isSuccessFullAttach = true;
                            }
                            else
                            {
                                return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(JsonConvert.SerializeObject("NotSaved"), JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                if (!isSuccessFullAttach)
                {
                    return Json(JsonConvert.SerializeObject("DocumentNotAttach"), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(JsonConvert.SerializeObject("OK"), JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }


        public JsonResult GetKeyColumns(int AD_Table_ID)
        {
            if (Session["ctx"] != null)
            {
                Ctx _ctx = Session["ctx"] as Ctx;
                string[] res = LookupHelper.GetKeyColumns(AD_Table_ID, _ctx);
                return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetTranslatedText(List<string> Columns, string AD_Language)
        {
            if (Session["ctx"] != null)
            {
                Ctx _ctx = Session["ctx"] as Ctx;
                FormModel fm = new FormModel(_ctx);
                var res = fm.GetTranslatedText(_ctx, Columns, AD_Language);
                return Json(JsonConvert.SerializeObject(res), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(JsonConvert.SerializeObject("SessionExpired"), JsonRequestBehavior.AllowGet);
            }

        }

        #endregion


        //Card View

        public JsonResult GetCardViewDetail(int AD_Window_ID, int AD_Tab_ID,int AD_CardView_ID,string SQL)
        {
            return Json(JsonConvert.SerializeObject(WindowHelper.GetCardViewDetail(AD_Window_ID, AD_Tab_ID, Session["ctx"] as Ctx, AD_CardView_ID,SQL)), JsonRequestBehavior.AllowGet);
        }

        public JsonResult InsertUpdateDefaultSearch(int AD_Tab_ID, int AD_Table_ID, int AD_User_ID, int? AD_UserQuery_ID)
        {
            Ctx _ctx = Session["ctx"] as Ctx;

            WindowHelper wHelper = new WindowHelper();
            wHelper.InsertUpdateDefaultSearch(_ctx, AD_Tab_ID, AD_Table_ID, AD_User_ID, AD_UserQuery_ID);
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTreeNodePath(int nodeID, int treeID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            object data = null;
            using (var w = new WindowHelper())
            {
                data = w.GetTreeNodePath(ctx, nodeID, treeID);
            }
            return Json(JsonConvert.SerializeObject(data), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetReportFileTypes(int AD_Process_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            return Json(JsonConvert.SerializeObject(ProcessHelper.GetReportFileTypes(ctx, AD_Process_ID)), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Method to get parent tab records ID.
        /// </summary>
        /// <param name="SelectColumn">Column  to be selected</param>
        /// <param name="SelectTable">From table</param>
        /// <param name="WhereColumn">Where column</param>
        /// <param name="WhereValue">ID of child column</param>
        /// <returns></returns>
        public ActionResult GetZoomParentRec(string SelectColumn, string SelectTable, string WhereColumn, string WhereValue)
        {

            WindowHelper obj = new WindowHelper();
            return Json(JsonConvert.SerializeObject(obj.GetZoomParentRecord(SelectColumn, SelectTable, WhereColumn, WhereValue)), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRecordForFilter(string keyCol, string displayCol, string validationCode, string tableName,
            string AD_Referencevalue_ID, string pTableName, string pColumnName, string whereClause)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string sql = null;
            if (keyCol == "")
            {
                sql = "SELECT " + pColumnName + "," + pColumnName + " as Name, count(" + pColumnName + ") FROM " + pTableName;
                sql = "SELECT * FROM (" + MRole.GetDefault(ctx).AddAccessSQL(sql, pTableName, true, false);
                if (!string.IsNullOrEmpty(validationCode))
                    sql += " AND " + validationCode;
                if (!string.IsNullOrEmpty(whereClause))
                    sql += " AND " + whereClause;

                sql += " AND " + pColumnName + " IS NOT NULL ";

                sql += " GROUP BY " + pColumnName +
                         " ORDER BY COUNT(" + pColumnName + ") DESC) ";
            }
            else
            {
                if (tableName.Equals("AD_Ref_List"))
                {
                    //sql = "SELECT " + keyCol + ", " + displayCol + " || '('|| count(" + keyCol + ") || ')' FROM " + tableName + " WHERE IsActive='Y'";
                    sql = "SELECT " + pColumnName + ", (Select Name from AD_REf_List where Value= " + pColumnName + " AND AD_Reference_ID=" + AD_Referencevalue_ID + ")  as name ,count(" + pColumnName + ")"
                        + " FROM " + pTableName;// + " WHERE " + pTableName + ".IsActive='Y'";
                    sql = "SELECT * FROM (" + MRole.GetDefault(ctx).AddAccessSQL(sql, pTableName, true, false);
                    if (!string.IsNullOrEmpty(validationCode))
                        sql += " AND " + validationCode;
                    if (!string.IsNullOrEmpty(whereClause))
                        sql += " AND " + whereClause;
                    sql += " GROUP BY " + pColumnName +
                             " ORDER BY COUNT(" + pColumnName + ") DESC)";
                }
                else
                {
                    sql = "SELECT " + keyCol + ", " + displayCol + " , count(" + keyCol + ")  FROM " + pTableName + " " + pTableName + " JOIN " + tableName + " " + tableName
                        + " ON " + tableName + "." + tableName + "_ID =" + pTableName + "." + pColumnName
                        + " ";// WHERE " + pTableName + ".IsActive='Y'";
                    sql = "SELECT * FROM (" + MRole.GetDefault(ctx).AddAccessSQL(sql, tableName, true, false);
                    if (!string.IsNullOrEmpty(validationCode))
                        sql += " AND " + validationCode;
                    if (!string.IsNullOrEmpty(whereClause))
                        sql += " AND " + whereClause;
                    sql += " GROUP BY " + keyCol + ", " + displayCol
                        + " ORDER BY COUNT(" + keyCol + ") DESC) ";

                }
            }

            //If DB is postgre, then append foo at end of subquery
            if (DB.IsPostgreSQL())
                sql += " AS foo ";

            Dictionary<string, object> result = new Dictionary<string, object>();
            List<FilterDataContract> keyva = new List<FilterDataContract>();
            DataSet ds = VIS.DBase.DB.ExecuteDatasetPaging(sql, 1, 10);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    FilterDataContract val = new FilterDataContract();
                    if (ds.Tables[0].Rows[i][0] == null || ds.Tables[0].Rows[i][0] == DBNull.Value)
                        continue;
                    val.ID = Convert.ToString(ds.Tables[0].Rows[i][0]);
                    val.Name = Convert.ToString(ds.Tables[0].Rows[i][1]);
                    val.Count = Convert.ToInt32(ds.Tables[0].Rows[i][2]);
                    keyva.Add(val);
                }
            }
            result["keyCol"] = pColumnName;
            result["list"] = keyva;
            return Json(JsonConvert.SerializeObject(result), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Controller function to fetch Version details
        /// </summary>
        /// <param name="RowData"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetVerInfo(string RowData)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            dynamic od = JsonConvert.DeserializeObject(RowData);
            CommonModel cm = new CommonModel();
            var retRes = cm.GetVerDetails(ctx, od);
            return Json(JsonConvert.SerializeObject(retRes), JsonRequestBehavior.AllowGet);
        }


        public JsonResult CheckVersions(SaveRecordIn RowData)
        {
            bool hasRecords = false;
            if (Session["Ctx"] != null)
            {
                Ctx ctx = Session["ctx"] as Ctx;
                CommonModel cmm = new CommonModel();
                hasRecords = cmm.HasVersions(ctx, RowData);
            }
            return Json(new { result = hasRecords }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Function to get data based on the query generated for table
        /// </summary>
        /// <param name="fields"></param>
        /// <returns></returns>
        public JsonResult GetIDTextData(string fields)
        {
            if (Session["Ctx"] != null)
            {
                string retJSON = "";
                Ctx ctx = Session["ctx"] as Ctx;

                // get parameters and split with comma
                string[] paramValue = !string.IsNullOrEmpty(fields) ? fields.Split(',') : null;
                // get table name from first index
                string TableName = Util.GetValueOfString(paramValue[0]);

                // check if second parameter is true for Version Table
                if (Util.GetValueOfBool(paramValue[1]))
                    TableName = TableName + "_Ver";

                int AD_Table_ID = MTable.Get_Table_ID(TableName);
                CommonModel objCommonModel = new CommonModel();
                POInfo inf = POInfo.GetPOInfo(ctx, AD_Table_ID);
                // Get SQL Query from PO Info for selected table
                string sqlCol = objCommonModel.GetSQLQuery(ctx,AD_Table_ID,inf.GetPoInfoColumns());

                // Append where Clause, passed in the parameter
                string whClause = Util.GetValueOfString(paramValue[2]);
                if (whClause != "")
                    sqlCol += " WHERE " + whClause;

                // Apply Role check
                if (sqlCol.Trim() != "")
                    sqlCol = MRole.GetDefault(ctx).AddAccessSQL(sqlCol, TableName, MRole.SQL_NOTQUALIFIED, MRole.SQL_RO);

                // if SQL is being generated for Version table
                if (Util.GetValueOfBool(paramValue[1]))
                {
                    if (sqlCol.Contains("WHERE"))
                    {
                        sqlCol += " AND " + TableName + ".ProcessedVersion = 'N' ";
                    }
                    sqlCol += " ORDER BY " + TableName + ".VERSIONVALIDFROM DESC, " + TableName + ".RecordVersion DESC";
                }
               
                retJSON = JsonConvert.SerializeObject(objCommonModel.GetIDTextData(ctx, sqlCol));
                return Json(retJSON, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Themes list 
        /// </summary>
        /// <returns></returns>
        public JsonResult GetThemes()
        {
            CommonModel cm = new CommonModel();
            var retRes = cm.GetThemes();
            return Json(JsonConvert.SerializeObject(retRes), JsonRequestBehavior.AllowGet);
        }


        #region Toaster notification
       [Obsolete]
        [NonAction]
        public static void AddMessageForToastr(string key, string value)
        {
            try
            {
                if (key.Contains("_"))
                {
                    key = key.Substring(key.IndexOf("_") + 1);
                }
                int val = 0;
                if (int.TryParse(key, out val))
                {
                    ModelLibrary.PushNotif.SSEManager.Get().AddMessage(val, value);
                }
            }
            catch
            {
                // blank
            }
        }

        public JsonResult GetCardsInfo(int AD_Tab_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            WindowHelper help = new WindowHelper();
            List<CardsInfo> cards = help.GetCards(ctx, AD_Tab_ID);
            return Json(JsonConvert.SerializeObject(cards), JsonRequestBehavior.AllowGet);
        }

        public void InsertUpdateDefaultCard(int AD_Tab_ID, int AD_Card_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            CardViewModel objCardViewModel = new CardViewModel();
            objCardViewModel.SetDefaultCardView(ctx, AD_Card_ID, AD_Tab_ID);
        }

        public ContentResult MsgForToastr()
        {
            Ctx ctx = Session["ctx"] as Ctx;
            string serializedObject = null;
            if (ctx != null)
            {
                string sessionID = ctx.GetAD_Session_ID().ToString();
                JavaScriptSerializer ser = new JavaScriptSerializer();

                //IEnumerable<KeyValuePair<string, string>> Dic = toastrMessage.Where(kvp => kvp.Key.Contains(sessionID));
                var newDic = ModelLibrary.PushNotif.SSEManager.Get().GetMessages(ctx.GetAD_Session_ID());
                if (newDic != null && newDic.Count() > 0)
                {
                   /// for (int i = 0; i < newDic.Count();)
                   // {
                    //    KeyValuePair<string, string> keyVal = newDic.ElementAt(i);
                    //    toastrMessage.Remove(keyVal.Key);
                        serializedObject = ser.Serialize(newDic);
                        return Content(string.Format("data: {0}\n\n", serializedObject), "text/event-stream");
                    //}
                }
            }
            JavaScriptSerializer se1r = new JavaScriptSerializer();
            serializedObject = se1r.Serialize(new { item = 1, message = "" });
            return Content(string.Format("data: {0}\n\n", serializedObject), "text/event-stream");
        }
        #endregion

    }

    public class FilterDataContract
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// handle Tree Creation Request for AD_Window
    /// </summary>
    public class TreeController : Controller
    {
        public ActionResult GetTreeAsString(int AD_Tree_ID, bool editable, int windowNo, bool onDemandTree, int AD_Tab_ID)
        {
            var html = "";
            if (Session["ctx"] != null)
            {
                var m = new MenuHelper(Session["ctx"] as Ctx);
                var tree = m.GetMenuTree(AD_Tree_ID, editable, onDemandTree, 0, AD_Tab_ID, windowNo);
                html = m.GetMenuTreeUI(tree.GetRootNode(), @Url.Content("~/"), windowNo.ToString(), tree.GetNodeTableName());
                m.dispose();
            }
            return Content(html);
        }

        public ActionResult UpdateTree(string nodeID, int oldParentID, int newParentID, int AD_Tree_ID)
        {
            Ctx ctx = Session["ctx"] as Ctx;
            var m = new MenuHelper(ctx);
            return Json(JsonConvert.SerializeObject(m.updateTree(ctx, nodeID, oldParentID, newParentID, AD_Tree_ID)), JsonRequestBehavior.AllowGet);
        }


    }


}
