using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web.Hosting;
using VAdvantage.Classes;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Login;
using VAdvantage.Model;
using VAdvantage.Print;
using VAdvantage.ProcessEngine;
using VAdvantage.Report;
using VAdvantage.Utility;

using VIS.Models;

namespace VIS.Helpers
{
    public class ProcessHelper
    {
        private static VLogger _log = VLogger.GetVLogger(typeof(ProcessHelper).FullName);
        private static int msgID = 0;
        public static ProcessDataOut GetProcessInfo(int AD_Process_ID, Ctx ctx)
        {
            ProcessDataOut outt = new ProcessDataOut();

            bool trl = !Env.IsBaseLanguage(ctx, "AD_Process");
            String sql = "SELECT p.Name, p.Description, p.Help, p.IsReport, p.AD_CtxArea_ID, ca.IsSOTrx, p.IsBackgroundProcess, p.AskUserBGProcess, (select COunt(AD_Process_ID) FROM AD_Process_Para where AD_Process_ID=p.AD_Process_ID) as para,p.iSCrystalReport "
                + " ,img.FontName ,    img.ImageURl   AS ImageUrl"
                    + " FROM AD_Process p "
                    + "LEFT OUTER JOIN AD_CtxArea ca ON (p.AD_CtxArea_ID=ca.AD_CtxArea_ID) "
                    + " Left Outer Join Ad_Image Img On  p.AD_Image_ID=img.AD_Image_ID "
                    + "WHERE AD_Process_ID=" + AD_Process_ID;
            if (trl)
                sql = "SELECT t.Name, t.Description, t.Help, p.IsReport, p.AD_CtxArea_ID, ca.IsSOTrx, p.IsBackgroundProcess, p.AskUserBGProcess, (select COunt(AD_Process_ID) FROM AD_Process_Para where AD_Process_ID=p.AD_Process_ID) as para,p.iSCrystalReport "
                    + " ,img.FontName ,   img.ImageURl      AS ImageUrl"
                    + " FROM AD_Process p "
                    + "LEFT OUTER JOIN AD_CtxArea ca ON (p.AD_CtxArea_ID=ca.AD_CtxArea_ID) "
                    + " INNER JOIN AD_Process_Trl t ON (p.AD_Process_ID=t.AD_Process_ID) "
                    + " Left Outer Join Ad_Image Img On  p.AD_Image_ID=img.AD_Image_ID "
                    + "WHERE p.AD_Process_ID=" + AD_Process_ID + " AND t.AD_Language='" + Env.GetAD_Language(ctx) + "'";

            IDataReader dr = null;

            try
            {

                dr = DB.ExecuteReader(sql, null);

                if (dr.Read())
                {
                    outt.Name = Env.TrimModulePrefix(dr.GetString(0));
                    outt.IsReport = dr.GetString(3).Equals("Y");

                    //
                    string msgText = "<b style='display: block;width: calc(100% - 20px);'>";

                    if (dr.IsDBNull(1))
                        msgText += Msg.GetMsg(ctx, "StartProcess?");
                    else
                        msgText += dr.GetString(1);

                    msgText += "</b>";

                    if (!dr.IsDBNull(2))
                        msgText += "<p style='display: inline-block;Max-width: calc(100% - 38px);'>" + dr.GetString(2) + "</p>";
                    //
                    outt.MessageText = msgText;

                    String isSOTrx = dr[5].ToString();
                    if (isSOTrx == "")
                        isSOTrx = "Y";
                    outt.IsSOTrx = isSOTrx;
                    outt.IsBackground = dr["IsBackgroundProcess"].Equals("Y");
                    outt.AskUser = dr["AskUserBGProcess"].Equals("Y");
                    outt.IsCrystal = dr["iSCrystalReport"].Equals("Y");
                    var paraCount = Util.GetValueOfInt(dr["para"]);
                    outt.ImageUrl = Util.GetValueOfString(dr["ImageUrl"]);
                    if (outt.ImageUrl != "" && outt.ImageUrl.Contains("/"))
                    {
                        outt.ImageUrl = outt.ImageUrl.Substring(outt.ImageUrl.LastIndexOf("/") + 1);
                    }
                    outt.FontName = Util.GetValueOfString(dr["FontName"]);

                    if (paraCount > 0)
                    {
                        outt.HasPara = true;
                    }


                }
                dr.Close();
            }
            catch (Exception e)
            {
                if (dr != null)
                    dr.Close();
                outt.IsError = true;
                outt.Message = e.Message;
            }
            return outt;
        }

        //readonly variable for lock process
        private static readonly object _lockObject = new object();
        private static readonly Dictionary<string, object> Locks = new Dictionary<string, object>();
        //private static System.Collections.Concurrent.ConcurrentDictionary<string, object> dictionary = new System.Collections.Concurrent.ConcurrentDictionary<string, object>();

        /// <summary>
        /// Get locks from lock dictionary
        /// </summary>
        /// <param name="adProcessID">Pass process id e.g. AD_Process_ID to set it into Dictionary</param>
        /// <returns>Object Lock</returns>
        public static object GetLock(string adProcessID)
        {
            lock (_lockObject)
            {
                if (Locks.ContainsKey(adProcessID))
                {
                    //_log.Info("Found in dictionary-------------------->" + adProcessID);
                    return Locks[adProcessID];
                }
                //_log.Info("New process id into dictionary--------->" + adProcessID);
                return Locks[adProcessID] = new object();
            }
        }
        // vinay bhatt window id
        public static ProcessReportInfo ExecuteProcessCommon(Ctx ctx, Dictionary<string, string> processInfo, ProcessPara[] pList)
        {
            ProcessReportInfo ret = new ProcessReportInfo();

            // Get Locks from Dictionary
            string lockedid = processInfo["Process_ID"].ToString() + "_" + processInfo["Record_ID"].ToString();
            //string lockedidlog = lockedid + " -UserID- " + ctx.GetAD_User_ID() + " -SesstionID- " + ctx.GetAD_Session_ID();
            var isBackground = processInfo["IsBackground"].Equals("True", StringComparison.OrdinalIgnoreCase);
            var currentLock = GetLock(lockedid);
            lock (currentLock)
            {
                if (!isBackground)
                {
                    ret = ExecuteProcess(ctx, processInfo, pList);
                }
                else
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(delegate
                    {
                        ret = ExecuteProcess(ctx, processInfo, pList);

                        // Insert Result into Pinstance Result table.
                        X_AD_PInstance_Result pResult = new X_AD_PInstance_Result(ctx, 0, null);
                        pResult.SetAD_PInstance_ID(ret.AD_PInstance_ID);


                        StringBuilder sBuilder = new StringBuilder();
                        string sqlk = "SELECT Log_ID, P_ID, P_Date, P_Number, P_Msg "
                + "FROM AD_PInstance_Log "
                + "WHERE AD_PInstance_ID= " + ret.AD_PInstance_ID
                + " ORDER BY Log_ID";

                        DataSet ds = DB.ExecuteDataset(sqlk);
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                            {
                                sBuilder.Append(Util.GetValueOfString(ds.Tables[0].Rows[i]["P_Number"]) + "  " + Util.GetValueOfString(ds.Tables[0].Rows[i]["P_Msg"]) + " \n ");
                            }
                        }

                        pResult.SetResult(ret.Result + " \n " + sBuilder.ToString());
                        pResult.Save();


                        // Get Message for Notice window from Cache or DB.
                        if (msgID == 0)
                        {
                            object messageID = DB.ExecuteScalar("SELECT AD_Message_ID FROM AD_Message WHERE Value='ProcessResult'");
                            if (messageID != null && messageID != DBNull.Value)
                            {
                                msgID = Convert.ToInt32(messageID);
                            }
                            //cache.Add(10101, msgID);
                        }

                        MProcess pro = MProcess.Get(ctx, Convert.ToInt32(processInfo["Process_ID"]));

                        // Insert Notice
                        MNote note = new MNote(ctx, msgID, ctx.GetAD_User_ID(), null);
                        note.SetTextMsg(Msg.GetMsg(ctx, "ProcessCompleted") + ": " + pro.GetName());

                        note.SetDescription(pro.GetName());
                        note.SetRecord(X_AD_PInstance_Result.Table_ID, pResult.GetAD_PInstance_Result_ID());
                        note.Save();
                        if (ret.Result != null && ret.Result.Length > 100)
                        {
                            ret.Result = ret.Result.Substring(0, 100) + "...";
                        }

                        //VIS.Controllers.JsonDataController.AddMessageForToastr(Convert.ToInt32(processInfo["Process_ID"]) + "_P_" + ctx.GetAD_Session_ID(), pro.GetName() + " " + Msg.GetMsg(ctx, "Completed") + ": " + ret.Result);
                        ModelLibrary.PushNotif.SSEManager.Get().AddMessage(ctx.GetAD_Session_ID(),  pro.GetName() + " " + Msg.GetMsg(ctx, "Completed") + ": " + ret.Result );
                       

                    });
                }
            }
            return ret;
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        // vinay bhatt window id
        public static ProcessReportInfo ExecuteProcess(Ctx ctx, Dictionary<string, string> processInfo, ProcessPara[] pList)
        {
            ProcessInfo pi = new ProcessInfo().FromList(processInfo);

            //Saved Action Log
            if (pi.GetIsReport())
                VAdvantage.Common.Common.SaveActionLog(ctx, pi.GetActionOrigin(), pi.GetOriginName(), pi.GetTable_ID(), pi.GetRecord_ID(), pi.GetAD_Process_ID(), pi.GetTitle(), pi.GetFileType(), "", "");
            pi.SetAD_User_ID(ctx.GetAD_User_ID());
            pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
            if (pi.GetAD_PInstance_ID() == 0)
            {
                MPInstance instance = null;
                instance = new MPInstance(ctx, Util.GetValueOfInt(processInfo["Process_ID"]), Util.GetValueOfInt(processInfo["Record_ID"]));

                if (!instance.Save())
                {

                }
                pi.SetAD_PInstance_ID(instance.Get_ID());
            }

            int vala = 0;
            if (pList != null && pList.Length > 0) //we have process parameter
            {
                for (int i = 0; i < pList.Length; i++)
                {
                    var pp = pList[i];
                    //	Create Parameter
                    MPInstancePara para = new MPInstancePara(ctx, pi.GetAD_PInstance_ID(), i);
                    para.SetParameterName(pp.Name);

                    if (DisplayType.IsDate(pp.DisplayType))
                    {
                        if (pp.DisplayType == DisplayType.DateTime)
                        {
                            if (pp.Result != null)
                            {
                                para.SetP_Date_Time(Convert.ToDateTime(pp.Result));
                            }

                            if (pp.Result2 != null)
                            {
                                para.SetP_Date_Time_To(Convert.ToDateTime(pp.Result2));
                            }
                        }
                        if (pp.DisplayType == DisplayType.Time)
                        {
                            if (pp.Result != null)
                            {
                                para.SetP_Time(Convert.ToDateTime(pp.Result));
                            }

                            if (pp.Result2 != null)
                            {
                                para.SetP_Time_To(Convert.ToDateTime(pp.Result2));
                            }
                        }
                        else
                        {
                            if (pp.Result != null)
                            {
                                para.SetP_Date(Convert.ToDateTime(pp.Result).ToUniversalTime());
                            }

                            if (pp.Result2 != null)
                            {
                                para.SetP_Date_To(Convert.ToDateTime(pp.Result2).ToUniversalTime());
                            }
                        }
                    }

                    else if ((DisplayType.IsID(pp.DisplayType) || DisplayType.Integer == pp.DisplayType || DisplayType.MultiKey == pp.DisplayType))
                    {

                        if (pp.Result != null)
                        {
                            if (DisplayType.IsLookup(pp.DisplayType) && pp.Result.Equals("-1"))
                            {
                                continue;
                            }

                            if (int.TryParse(pp.Result.ToString(), out vala))
                            {
                                para.SetP_Number(Convert.ToInt32(pp.Result));
                            }
                            else
                            {
                                para.SetP_String(pp.Result.ToString());
                            }
                        }
                        if (pp.Result2 != null)
                        {
                            if (DisplayType.IsLookup(pp.DisplayType) && pp.Result2.Equals("-1"))
                            {
                                continue;
                            }
                            if (int.TryParse(pp.Result2.ToString(), out vala))
                            {
                                para.SetP_Number_To(Convert.ToInt32(pp.Result2));
                            }
                            else
                            {
                                para.SetP_String_To(pp.Result2.ToString());
                            }
                        }
                    }
                    else if (DisplayType.IsNumeric(pp.DisplayType))
                    {
                        if (pp.Result != null)
                        {
                            para.SetP_Number(Convert.ToDecimal(pp.Result));
                        }
                        if (pp.Result2 != null)
                        {
                            para.SetP_Number_To(Convert.ToDecimal(pp.Result2));
                        }

                    }
                    else if (DisplayType.YesNo == pp.DisplayType)
                    {
                        Boolean bb = (Boolean)pp.Result;
                        String value = bb ? "Y" : "N";
                        para.SetP_String(value);
                    }

                    else
                    {
                        if (pp.Result != null)
                        {
                            para.SetP_String(pp.Result.ToString());
                        }
                        if (pp.Result2 != null)
                        {
                            para.SetP_String_To(pp.Result.ToString());
                        }
                    }
                    para.SetAD_Process_Para_ID(pp.AD_Column_ID);

                    para.SetInfo(pp.Info);

                    if (pp.Info_To != null)
                        para.SetInfo_To(pp.Info_To);
                    para.Save();
                }
            }

            //string lang = ctx.GetAD_Language().Replace("_", "-");
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;
            //System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);

            byte[] report = null;
            string rptFilePath = null;
            ProcessReportInfo rep = new ProcessReportInfo();
            try
            {
                pi.IsArabicReportFromOutside = false;
                ProcessCtl ctl = new ProcessCtl();
                Dictionary<string, object> d = ctl.Process(pi, ctx, out report, out rptFilePath);
                rep = new ProcessReportInfo();
                rep.ReportProcessInfo = d;
                rep.Report = report;


                rep.ReportString = ctl.ReportString;
                //rep.AD_ReportView_ID = ctl.GetAD_ReportView_ID();
                rep.ReportFilePath = rptFilePath;

                rep.AD_PInstance_ID = pi.GetAD_PInstance_ID();
                rep.Result = pi.GetSummary();
                rep.AD_PrintFormat_ID = pi.Get_AD_PrintFormat_ID();
                ctl.ReportString = null;
                rep.HTML = ctl.GetRptHtml();
                rep.IsBiHTMlReport = pi.GetFileType().Equals(ProcessCtl.ReportType_BIHTML);

                // Change Lokesh Chauhan
                rep.CustomHTML = pi.GetCustomHTML();

            }
            catch (Exception e)
            {
                rep.IsError = true;
                rep.Message = e.Message;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = original;
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;
            VAdvantage.Classes.CleanUp.Get().Start();
            return rep;
        }


        // vinay bhatt window id

        /// <summary>
        /// Excecute Process
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Process_ID"></param>
        /// <param name="Name"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="WindowNo"></param>
        /// <param name="fileType"></param>
        /// <param name="AD_Window_ID"></param>
        /// <returns></returns>
        public static ProcessReportInfo Process(Ctx ctx, Dictionary<string, string> processInfo)
        {
            ProcessReportInfo ret = new ProcessReportInfo();
            MPInstance instance = null;
            try
            {
                instance = new MPInstance(ctx, Util.GetValueOfInt(processInfo["Process_ID"]), Util.GetValueOfInt(processInfo["Record_ID"]));
            }
            catch (Exception e)
            {
                ret.IsError = true;
                ret.Message = e.Message;
                return ret;
            }


            //	Get Parameters (Dialog)
            //Check If Contaon Parameter

            List<GridField> fields = ProcessParameter.GetParametersList(ctx, Util.GetValueOfInt(processInfo["Process_ID"]), Util.GetValueOfInt(processInfo["WindowNo"]));

            if (fields.Count < 1) //no Parameter then save instance and start execution
            {
                if (!instance.Save())
                {
                    ret.IsError = true;
                    ret.Message = Msg.GetMsg(ctx, "ProcessNoInstance");
                    return ret;
                }
                ret.AD_PInstance_ID = instance.GetAD_PInstance_ID();
                processInfo["AD_PInstance_ID"] = ret.AD_PInstance_ID.ToString();
                ret = ExecuteProcessCommon(ctx, processInfo, null);
            }
            else// If  parameter exist, then instance is created after user clciks OK
            {
                ret.ShowParameter = true;
                ret.ProcessFields = fields;
            }
            return ret;
        }











        StringBuilder parentIDs = new StringBuilder();
        private void GetChildNodesID(Ctx ctx, int currentnode, int treeID)
        {
            MTree tree = new MTree(ctx, treeID, null);


            if (parentIDs.Length == 0)
            {
                parentIDs.Append(currentnode);
            }
            else
            {
                parentIDs.Append(",").Append(currentnode);
            }
            string adtableName = MTable.GetTableName(ctx, tree.GetAD_Table_ID());

            string tableName = tree.GetNodeTableName();

            //  string sql = "SELECT node_ID FROM " + tableName + " WHERE AD_Tree_ID=" + treeID + " AND Parent_ID = " + currentnode + " AND NODE_ID IN (SELECT " + adtableName + "_ID FROM " + adtableName + " WHERE ISActive='Y' AND IsSummary='Y')";


            string sql = "SELECT pr.node_ID FROM " + tableName + "   pr JOIN " + adtableName + " mp on pr.Node_ID=mp." + adtableName + "_id  WHERE pr.AD_Tree_ID=" + treeID + " AND pr.Parent_ID = " + currentnode + " AND mp.ISActive='Y' AND mp.IsSummary='Y'";

            DataSet ds = DB.ExecuteDataset(sql);
            if (ds == null || ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                {
                    GetChildNodesID(ctx, Convert.ToInt32(ds.Tables[0].Rows[j]["node_ID"]), treeID);
                }
            }
        }

        private static object _lock = new object();
        private static object _lockcsv = new object();
        private static VLogger log = VLogger.GetVLogger(typeof(ReportEngine_N).FullName);
        public GridReportInfo GenerateReport(Ctx _ctx, int id, List<string> queryInfo, Object code, bool isCreateNew, Dictionary<string, string> nProcessInfo, int pageNo, int AD_PInstance_ID, string fileType, int? nodeID, int? treeID, bool showSummary)
        {
            GridReportInfo res = new GridReportInfo();
            VIS.Models.PageSetting ps = new Models.PageSetting();
            parentIDs.Clear();
            ProcessReportInfo rep = null;
            ReportEngine_N re = null;
            Query _query = null;
            int Record_ID = 0;
            object AD_tab_ID = 0;

            //Saved Action Log
            VAdvantage.Common.Common.SaveActionLog(_ctx, Util.GetValueOfString(nProcessInfo["ActionOrigin"]), Util.GetValueOfString(nProcessInfo["OriginName"]),
                Util.GetValueOfInt(nProcessInfo["AD_Table_ID"]), Util.GetValueOfInt(nProcessInfo["Record_ID"]), Util.GetValueOfInt(nProcessInfo["Process_ID"]),
                MWindow.Get(_ctx, Util.GetValueOfInt(nProcessInfo["Process_ID"])).GetName(), fileType, "", "");


            // _ctx.SetContext("#TimeZoneName", "India Standard Time");
            if (queryInfo.Count > 0 || AD_PInstance_ID > 0)
            {
                string tableName = queryInfo[0];
                if (AD_PInstance_ID > 0)
                {
                    if ((code).GetType() == typeof(int))	//	Form = one record
                        _query = Query.GetEqualQuery(tableName, ((int)code));
                    else
                        _query = Query.Get(_ctx, AD_PInstance_ID, tableName);

                }
                else
                {
                    string wherClause = queryInfo[1];
                    _query = new Query(tableName);

                    if (!string.IsNullOrEmpty(wherClause))
                        _query.AddRestriction(wherClause);
                }

                if (_query.GetRestrictionCount() == 1 && (code).GetType() == typeof(int))
                    Record_ID = ((int)code);

                if (nodeID > 0)
                {
                    GetChildNodesID(_ctx, Convert.ToInt32(nodeID), Convert.ToInt32(treeID));
                    MTree tree = new MTree(_ctx, Convert.ToInt32(treeID), null);
                    string nodeTableName = tree.GetNodeTableName();
                    _query.AddRestriction(" " + tableName + "." + tableName + "_ID  IN (SELECT NODE_ID FROM " + nodeTableName + "  WHERE Parent_ID IN (" + parentIDs.ToString() + ") OR NODE_ID IN (" + parentIDs + ")) ");

                    if (!showSummary)
                    {
                        _query.AddRestriction(" " + tableName + "." + "IsSummary= 'N'");
                    }

                }
            }
            else
            {
                if (nodeID > 0)
                {
                    MTree tree = new MTree(_ctx, Convert.ToInt32(treeID), null);
                    string tableName = MTable.GetTableName(_ctx, tree.GetAD_Table_ID());

                    _query = new Query(tableName);

                    GetChildNodesID(_ctx, Convert.ToInt32(nodeID), Convert.ToInt32(treeID));
                    string nodeTableName = tree.GetNodeTableName();
                    _query.AddRestriction(" " + tableName + "." + tableName + "_ID  IN (SELECT NODE_ID FROM " + nodeTableName + "  WHERE Parent_ID IN (" + parentIDs.ToString() + ")) ");


                    if (!showSummary)
                    {
                        _query.AddRestriction(" " + tableName + "." + "IsSummary= 'N'");
                    }
                }

            }

            if (_query == null)
            {
                _query = new Query();
                _query.AddRestriction(" 1 = 1");
            }



            if (queryInfo.Count > 2)
            {
                if (queryInfo[2] != null && queryInfo[2] != "" && Convert.ToInt32(queryInfo[2]) > 0)
                {
                    AD_tab_ID = Convert.ToInt32(queryInfo[2]);
                }
            }
            //Context _ctx = new Context(ctxDic);
            //Env.SetContext(_ctx);
            string lang = _ctx.GetAD_Language().Replace("_", "-");


            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);



            byte[] b = null;
            try
            {
                MPrintFormat pf = null;

                if (!isCreateNew)
                {
                    bool isPFExist = Util.GetValueOfInt(DB.ExecuteScalar("SELECT Count(*) FROM AD_PrintFormat WHERE AD_PrintFormat_ID=" + id)) > 0;
                    if (isPFExist)
                    {
                        pf = MPrintFormat.Get(_ctx, id, true);
                    }
                    else
                    {
                        rep = new ProcessReportInfo();
                        rep.ErrorText = "NoDocPrintFormat";
                        res.repInfo = rep;
                        return res;
                    }
                }
                else
                {
                    //   pf = MPrintFormat.CreateFromTable(_ctx, id);
                    if (Convert.ToInt32(AD_tab_ID) > 0)
                    {
                        pf = MPrintFormat.CreateFromTable(_ctx, id, AD_tab_ID, true);
                    }
                    else
                    {
                        pf = MPrintFormat.CreateFromTable(_ctx, id, true);
                    }

                }

                pf.IsGridReport = true;
                pf.PageNo = pageNo;



                rep = new ProcessReportInfo();
                pf.SetName(Env.TrimModulePrefix(pf.GetName()));
                if (nProcessInfo == null || pageNo > 0 || (Util.GetValueOfInt(nProcessInfo["Record_ID"]) == 0 && Util.GetValueOfInt(nProcessInfo["Process_ID"]) == 0 &&
                    Util.GetValueOfInt(nProcessInfo["AD_PInstance_ID"]) == 0))
                {
                    PrintInfo info = new PrintInfo(pf.GetName(), pf.GetAD_Table_ID(), Record_ID);
                    info.SetDescription(_query == null ? "" : _query.GetInfo());
                    re = new ReportEngine_N(_ctx, pf, _query, info);
                }
                else
                {
                    ProcessInfo pi = new ProcessInfo().FromList(nProcessInfo);
                    pi.Set_AD_PrintFormat_ID(id);
                    ProcessCtl ctl = new ProcessCtl();
                    ctl.Process(pi, _ctx, out b, out re);
                    re.SetPrintFormat(pf);
                }
                lock (_lock)
                {
                    re.GetView();
                    if (fileType == ProcessCtl.ReportType_PDF)
                    {
                        rep.ReportFilePath = re.GetReportFilePath(true, out b);
                    }
                    else if (fileType == ProcessCtl.ReportType_CSV)
                    {
                        rep.ReportFilePath = re.GetCSVPath(_ctx);
                    }
                    else
                    {
                        rep.ReportFilePath = re.GetReportFilePath(true, out b);
                        rep.HTML = re.GetRptHtml().ToString();
                        rep.AD_PrintFormat_ID = re.GetPrintFormat().GetAD_PrintFormat_ID();
                        rep.ReportProcessInfo = null;
                        rep.Report = re.CreatePDF();
                    }
                }

                ps.TotalPage = pf.TotalPage;
                ps.CurrentPage = pageNo;
                // b = re.CreatePDF();
                //rep.Report = b;
                //rep.HTML = re.GetRptHtml().ToString();
                //rep.AD_PrintFormat_ID = re.GetPrintFormat().GetAD_PrintFormat_ID();
                //rep.ReportProcessInfo = null;
            }
            catch (Exception ex)
            {
                rep.IsError = true;
                rep.ErrorText = ex.Message;
                log.Severe("ReportEngine_N_CreatePDF_" + ex.ToString());
            }
            //      VAdvantage.Classes.CleanUp.Get().Start();
            res.repInfo = rep;
            res.pSetting = ps;
            return res;
        }

        /// <summary>
        /// This function executes when user select record (one OR more) from window and click print button.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Process_ID"></param>
        /// <param name="Name"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="WindowNo"></param>
        /// <param name="recIDs"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public static ProcessReportInfo GeneratePrint(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int WindowNo, string recIDs, string fileType, string actionOrigin, string originName)
        {
            ProcessReportInfo ret = new ProcessReportInfo();
            MPInstance instance = null;
            //Saved Action Log
            VAdvantage.Common.Common.SaveActionLog(ctx, actionOrigin, originName, AD_Table_ID, Record_ID, AD_Process_ID, MProcess.Get(ctx, AD_Process_ID).GetName(), fileType, "", "");

            try
            {
                instance = new MPInstance(ctx, AD_Process_ID, Record_ID);
            }
            catch (Exception e)
            {
                ret.IsError = true;
                ret.Message = e.Message;
                return ret;
            }

            if (!instance.Save())
            {
                ret.IsError = true;
                ret.Message = Msg.GetMsg(ctx, "ProcessNoInstance");
                return ret;
            }
            ret.AD_PInstance_ID = instance.GetAD_PInstance_ID();




            //ReportEngine_N re = null;

            string lang = ctx.GetAD_Language().Replace("_", "-");
            // Set Report Language -VIS0228
            if (!string.IsNullOrEmpty(ctx.GetContext("Report_Lang"))) {
                lang = ctx.GetContext("Report_Lang").Replace("_", "-");
            }
            System.Globalization.CultureInfo original = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);


            ////////log/////
            //string clientName = ctx.GetAD_Org_Name() + "_" + ctx.GetAD_User_Name();
            //string storedPath = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "");
            //storedPath += clientName;
            //VLogMgt.Initialize(true, storedPath);
            ////////////////
            ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID, recIDs);
            pi.SetFileType(fileType);
            TryPrintFromDocType(pi);
            //if (ret != null)
            //{
            //    return ret;
            //}


            ret = new ProcessReportInfo();
            ret.AD_PInstance_ID = instance.GetAD_PInstance_ID();
            byte[] report = null;
            string rptFilePath = null;
            // ProcessReportInfo rep = new ProcessReportInfo();
            try
            {

                pi.SetAD_User_ID(ctx.GetAD_User_ID());
                pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                pi.SetAD_PInstance_ID(ret.AD_PInstance_ID);

                //report = null;
                ProcessCtl ctl = new ProcessCtl();
                //ctl.SetIsPrintFormat(true);
                //ctl.IsArabicReportFromOutside = false;
                //ctl.SetIsPrintCsv(csv);
                //ctl.SetFileType(fileType);
                Dictionary<string, object> d = ctl.Process(pi, ctx, out report, out rptFilePath);
                //rep = new ProcessReportInfo();
                ret.ReportProcessInfo = d;
                ret.Report = report;
                ret.ReportString = ctl.ReportString;
                ret.ReportFilePath = rptFilePath;
                ret.HTML = ctl.GetRptHtml();
                ret.IsRCReport = ctl.IsRCReport();
                ret.TotalRecords = pi.GetTotalRecords();
                ret.IsReportFormat = pi.GetIsReportFormat();
                ret.IsTelerikReport = pi.GetIsTelerik();
                ret.AD_Table_ID = pi.GetTable_ID();
                ret.RecordID = pi.GetRecord_ID();
                ret.AD_Process_ID = pi.GetAD_Process_ID();
                ret.RecordIDs = pi.GetRecIds();
                ctl.ReportString = null;


                //Env.GetCtx().Clear();
            }
            catch (Exception e)
            {
                ret.IsError = true;
                ret.Message = e.Message;
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = original;
            System.Threading.Thread.CurrentThread.CurrentUICulture = original;

            //return ret;
            //ret = ExecuteProcess(ctx, AD_Process_ID, Name, ret.AD_PInstance_ID, AD_Table_ID, Record_ID, null,true);
            // VAdvantage.Classes.CleanUp.Get().Start();
            return ret;
        }


        /// <summary>
        /// if Doctype or targetDocType column exist in window, then check print format attached to that Doc type. and open that one.
        /// </summary>
        /// <param name="_pi"></param>
        private static void TryPrintFromDocType(ProcessInfo _pi)
        {
            try
            {

                string tableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID=" + _pi.GetTable_ID()));

                if (tableName.ToLower() == "c_invoice")
                {

                    int Report_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT InvoiceReport_ID FROM C_BPartner WHERE C_BPartner_ID=(SELECT C_BPartner_ID FROM " + tableName + " WHERE " + tableName + "_ID=" + _pi.GetRecord_ID() + ")"));

                    if (Report_ID > 0)
                    {                       
                        return;
                    }
                }

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
                //ProcessReportInfo ret = new ProcessReportInfo();
                //string reportFilePath = null;
                //byte[] report = null;
                //string ReportString = null;
                //ProcessInfo pi = new ProcessInfo(Name, AD_Process_ID, AD_Table_ID, Record_ID, recIDs);
                //pi.SetAD_User_ID(ctx.GetAD_User_ID());
                //pi.SetAD_Client_ID(ctx.GetAD_Client_ID());
                //pi.SetAD_PInstance_ID(AD_PInstance_ID);
                //pi.IsArabicReportFromOutside = false;
                //pi.SetFileType(fileType);

                //ProcessCtl ctl = new ProcessCtl();

                //pi.SetIsReportFormat(true);
                //int totalRecords = 0;
                //IReportEngine re = VAdvantage.ReportFormat.ReportFormatEngine.Get(ctx, pi, out totalRecords, false);// VAdvantage.ReportFormat.ReportFormatEngine.Get(ctx, pi, out totalRecords, false);           
                //pi.SetSummary("Report", re != null);
                //pi.SetTotalRecords(totalRecords);
                //if (re != null)
                //{
                //    //isRCReport = true;
                //    if (fileType == "C")//csv
                //    {
                //        string s = "";
                //        reportFilePath = re.GetCsvReportFilePath(s);
                //    }
                //    else if (fileType == "P")
                //    {
                //        report = re.GetReportBytes();
                //        reportFilePath = re.GetReportFilePath(true, out report);
                //    }
                //    else if (fileType == "R")
                //    {
                //        reportFilePath = re.GetRtfReportFilePath("");
                //    }
                //    ReportString = re.GetReportString();
                //}
                //ret.ReportProcessInfo = pi.ToList();
                //ret.Report = report;
                //ret.ReportString = ReportString;
                //ret.ReportFilePath = reportFilePath;
                //ret.HTML = ctl.GetRptHtml();
                //ret.IsRCReport = ctl.IsRCReport();
                //ret.TotalRecords = pi.GetTotalRecords();
                //ret.IsReportFormat = pi.GetIsReportFormat();
                //ctl.ReportString = null;
                //return ret;
            }
            catch
            {
                return;
            }
        }

        /// <summary>
        /// Archieve Reports data
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Process_ID"></param>
        /// <param name="Name"></param>
        /// <param name="AD_Table_ID"></param>
        /// <param name="Record_ID"></param>
        /// <param name="C_BPartner_ID"></param>
        /// <param name="isReport"></param>
        /// <param name="binaryData"></param>
        /// <param name="reportPath"></param>
        /// <returns></returns>
        public static bool ArchiveDoc(Ctx ctx, int AD_Process_ID, string Name, int AD_Table_ID, int Record_ID, int C_BPartner_ID, bool isReport, byte[] binaryData, string reportPath)
        {
            MArchive archive = new MArchive(ctx, 0, null);
            archive.SetName(Name);
            archive.SetIsReport(isReport);
            archive.SetAD_Process_ID(AD_Process_ID);
            archive.SetAD_Table_ID(AD_Table_ID);
            archive.SetRecord_ID(Record_ID);
            archive.SetC_BPartner_ID(C_BPartner_ID);

            if (!string.IsNullOrEmpty(reportPath))
            {
                string path = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + reportPath;
                if (File.Exists(path))
                {
                    binaryData = File.ReadAllBytes(path);
                    archive.SetBinaryData(binaryData);
                }
            }

            if (binaryData != null)
            {
                archive.SetBinaryData(binaryData);
            }

            bool res = archive.Save();
            return res;

        }

        /// <summary>
        /// Get Report Type list 
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="AD_Process_ID"></param>
        /// <returns></returns>
        public static List<ValueNamePair> GetReportFileTypes(Ctx ctx, int AD_Process_ID)
        {
            List<ValueNamePair> list = new List<ValueNamePair>();
            string sql = "";

            if (ctx.GetUseCrystalReportViewer().Equals("Y"))
            {
                object isCrystal = DB.ExecuteScalar("SELECT  IsCrystalReport FROM AD_Process WHERE IsActive='Y' AND AD_Process_ID=" + AD_Process_ID);
                if (isCrystal.Equals("Y"))
                {
                    return list;
                }
            }

            int refListID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Reference_ID FROM AD_Reference WHERE Export_ID='VIS_1000195'"));
            bool showRTF = IsShowRTF(AD_Process_ID);

            bool isBaseLanguage = VAdvantage.Utility.Env.IsBaseLanguage(ctx, "");
            if (isBaseLanguage)
            {
                sql = "SELECT Value, Name FROM AD_Ref_List "
                 + "WHERE AD_Reference_ID=" + refListID + " AND IsActive='Y' ORDER BY 1";
            }
            else
            {
                sql = @"SELECT r.Value, rt.Name FROM AD_Ref_List r
                      inner join ad_ref_list_trl rt on (r.AD_Ref_List_ID=rt.AD_Ref_List_ID)
                      WHERE r.AD_Reference_ID=" + refListID + " AND r.IsActive='Y' AND rt.AD_language='" + ctx.GetAD_Language() + "' ORDER BY 1";
            }
            DataSet ds = null;
            try
            {
                ds = DB.ExecuteDataset(sql, null, null);
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow rs = ds.Tables[0].Rows[i];
                    list.Add(new ValueNamePair(rs[0].ToString(), rs[1].ToString()));
                }
                ds = null;
            }
            catch (Exception e)
            {
            }

            if (!showRTF)
            {
                foreach (ValueNamePair v in list)
                {
                    if (v.Key.Equals("R"))
                    {
                        list.Remove(v);
                        break;
                    }
                }

            }

            bool isHtmlSupported = IsHtmlSupported(AD_Process_ID);

            if (!isHtmlSupported)
            {
                foreach (ValueNamePair v in list)
                {
                    if (v.Key.Equals("B"))//if HTML not supported then remove this key from list.
                    {
                        list.Remove(v);
                        break;
                    }
                }
            }

            return list;

        }

        /// <summary>
        /// Check if report Type Support HTML reposonse(Only Supprted in BI Reports for now)
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <returns></returns>
        private static bool IsHtmlSupported(int AD_Process_ID)
        {
            object val = DB.ExecuteScalar("SELECT IsCrystalReport FROM AD_Process WHERE AD_Process_ID=" + AD_Process_ID);
            if (val != DBNull.Value && val != null && val.Equals("B"))
                return true;
            return false;
        }

        /// <summary>
        /// Is RTF is supported by Report
        /// </summary>
        /// <param name="AD_Process_ID"></param>
        /// <returns></returns>
        private static bool IsShowRTF(int AD_Process_ID)
        {
            int rfID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_ReportFormat_ID FROM AD_Process WHERE IsActive='Y' AND AD_Process_ID=" + AD_Process_ID));
            if (rfID > 0)
            {
                return true;
            }
            DataSet ds = DB.ExecuteDataset("SELECT  IsReport,IsCrystalReport,ReportPath FROM AD_Process WHERE IsActive='Y' AND AD_Process_ID=" + AD_Process_ID);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                if (Util.GetValueOfString(ds.Tables[0].Rows[0]["IsReport"]) == "Y"
                    && Util.GetValueOfString(ds.Tables[0].Rows[0]["IsCrystalReport"]) == "Y"
                    && ds.Tables[0].Rows[0]["ReportPath"] != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// function will accept columnName and Ids selected. Will Fetch information from Default tree hierarchy and get child records accordingly.
        /// </summary>
        /// <param name="_ctx"></param>
        /// <param name="columnName"></param>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns>Name of orgs separated bY commas, and IDS in Reference Object</returns>
        private static string GetRecursiveParameterValue(Ctx _ctx, string columnName, string value, ref string result, bool ShowChildOfSelected)
        {

            string tableName = columnName.Substring(0, columnName.Length - 3);

            string[] values = value.Split(',');
            String eSql = "";
            string result1 = "";
            StringBuilder finalResult = new StringBuilder();
            StringBuilder nonSummaryResult = new StringBuilder();
            if (values.Length > 0)
            {

                // Get Default Heirarchy
                string sqla = @"SELECT PA_HIERARCHY_id FROM PA_Hierarchy WHERE ISACTIVE ='Y' 
                       ORDER BY ISDEFAULT DESC ,PA_HIERARCHY_id ASC";
                sqla = MRole.GetDefault(_ctx).AddAccessSQL(sqla, "PA_Hierarchy", true, true);
                object ID = DB.ExecuteScalar(sqla);
                int _PA_Hierarchy_ID = 0;
                if (ID != null && ID != DBNull.Value)
                {
                    _PA_Hierarchy_ID = Util.GetValueOfInt(ID);
                }
                Language _language = Language.GetLanguage(_ctx.GetAD_Language());

                //Get Query to fetch identifier value from table based on column selected. it will be used to display identifires on for parameter in report.
                eSql = VLookUpFactory.GetLookup_TableDirEmbed(_language, columnName, columnName.Substring(0, columnName.Length - 3));

                for (int i = 0; i < values.Length; i++)
                {
                    if (!string.IsNullOrEmpty(values[i]))
                    {
                        //try
                        //{
                        string sqlCheckSummary = "SELECT IsSummary FROM " + tableName + " WHERE " + columnName + "=" + values[i];
                        object val = DB.ExecuteScalar(sqlCheckSummary);
                        if (val != null && val != DBNull.Value)
                        {

                            if (val.ToString().Equals("N"))     // If non-summary is selected then add it string and continue to next ID
                            {
                                if (nonSummaryResult.Length > 0)
                                {
                                    nonSummaryResult.Append("," + values[i]);
                                }
                                else
                                {
                                    nonSummaryResult.Append(values[i]);
                                }
                                continue;
                            }
                        }
                        //}
                        //catch
                        //{
                        //    result = "";
                        //    continue;
                        //}

                        // Fetch child records from tree hierarchy based on ID selected.
                        if (columnName.Equals("AD_Org_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Organization, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("C_BPartner_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_BPartner, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("M_Product_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Product, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("C_Project_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Project, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("AD_OrgTrx_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_OrgTrx, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("Account_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Account, Convert.ToInt32(values[i]));
                        }
                        else if (columnName.Equals("C_Campaign_ID", StringComparison.OrdinalIgnoreCase))
                        {
                            result1 = MReportTree.GetWhereClause(_ctx, _PA_Hierarchy_ID, MAcctSchemaElement.ELEMENTTYPE_Campaign, Convert.ToInt32(values[i]));
                        }


                        if (result1.IndexOf("(") > -1)
                        {
                            result1 = result1.Substring(result1.IndexOf("(") + 1);
                            result1 = result1.Substring(0, result1.IndexOf(")"));
                        }
                        else
                        {
                            result1 = result1.Substring(result1.IndexOf("=") + 1);
                        }

                        //create list of sleected IDs in stringbuilder
                        if (result1 != values[i] && result1.Length > 0)
                        {
                            if (finalResult.Length > 0)
                            {
                                finalResult.Append("," + result1);
                            }
                            else
                            {
                                finalResult.Append(result1);
                            }
                        }
                    }
                }
            }




            StringBuilder identifiedsName = new StringBuilder();
            if (finalResult.Length > 0)
            {
                if (finalResult.ToString().IndexOf(",") > -1)
                {
                    eSql = eSql + " AND " + columnName + " IN (" + finalResult.ToString() + ")";
                }
                else
                {
                    eSql = eSql + " AND " + columnName + " = " + finalResult.ToString();
                }
                //eSql = eSql + " AND " + result1;


                //if (!string.IsNullOrEmpty(finalResult.ToString()))
                //{
                //    result = result + "," + finalResult.ToString();
                //}

                DataSet dsIdeintifiers = DB.ExecuteDataset(eSql);



                if (ShowChildOfSelected && (dsIdeintifiers != null && dsIdeintifiers.Tables[0].Rows.Count > 0))
                {
                    for (int s = 0; s < dsIdeintifiers.Tables[0].Rows.Count; s++)
                    {
                        if (identifiedsName.Length > 0)
                        {
                            identifiedsName.Append(",");
                        }
                        identifiedsName.Append(dsIdeintifiers.Tables[0].Rows[s][0]);
                    }
                }

            }
            if (nonSummaryResult.Length > 0 || finalResult.Length > 0)
            {
                if (nonSummaryResult.Length > 0)
                {
                    result = nonSummaryResult.ToString();
                }

                if (finalResult.Length > 0)
                {
                    if (result.Length > 0)
                    {
                        result += ",";
                    }
                    result += finalResult.ToString();
                }

            }



            if (identifiedsName != null)
            {
                return identifiedsName.ToString();
            }

            return "";
        }
    }
}