using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;
using System.Data;
using VAdvantage.Model;
using VAdvantage.DataBase;
using System.Diagnostics;
using VAdvantage.Classes;
using VAdvantage.Print;

namespace VAdvantage.Process
{
    public class PrepareModuleSchema : SvrProcess
    {
        private Ctx _ctx = null;

        /* current module Prefix */
        String currentPrefix = "";
        /*other used module Pefix */
        String modulePrefix = "";

        String VIENNA_PREFIX = "VIS_";

        DateTime? RELEASEDDATE = new DateTime(2013, 1, 1);



        //String nameSpace = "";
        int VAF_ModuleInfo_ID = 0;

        const String MODULE_DB_SCHEMA = "VAF_Module_DB_Schema";
        private List<String> prefixList = new List<string>();
        List<String> usedPrefixesList = new List<string>();

        // check for process execution
        public static bool running = false;
        // lock for simultaneous process
        private object _lock = new object();

        protected override void Prepare()
        {
            _ctx = GetCtx();
            VAF_ModuleInfo_ID = GetRecord_ID();
            //prefix = DataBase.DB.ExecuteScalar("SELECT Prefix FROM VAF_ModuleInfo WHERE VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID).ToString();
            IDataReader dr = DB.ExecuteReader("SELECT Prefix,VAF_ModuleInfo_ID FROM VAF_ModuleInfo");
            while (dr.Read())
            {
                string prefix = dr[0].ToString();
                if (string.IsNullOrEmpty(prefix.Trim()))
                {
                    continue;
                }
                prefixList.Add(prefix);
                if (VAF_ModuleInfo_ID == Convert.ToInt32(dr[1]))
                {
                    /* Initially both same */
                    currentPrefix = prefix;
                    modulePrefix = prefix;
                }
            }
            dr.Close();
        }

        protected override string DoIt()
        {
            // lock object
            lock (_lock)
            {
                // check if process is already running for any module, then return with message
                if (running)
                    return "Module process already running, Please wait for some time....";
                if (string.IsNullOrEmpty(currentPrefix))
                    return "Could not find valid prefix";

                if (!DeleteOldSchema(VAF_ModuleInfo_ID))
                    return "could not delete old schema";

                running = true;
            }

            try
            {
                GenerateSchema(VAF_ModuleInfo_ID);
            }
            finally
            {
                running = false;
            }

            return "Schema Generated";
            //throw new NotImplementedException();
        }

        private bool DeleteOldSchema(int VAF_ModuleInfo_ID)
        {
            int count = DB.ExecuteQuery("DELETE FROM VAF_Module_DB_Schema WHERE VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID, null, Get_TrxName());
            if (count < 0)
            {
                return false;
            }
            return true;
        }



        #region Function

        private bool IsRecordExistInDBSchema(int VAF_TableView_ID, int Record_ID)
        {
            int count = Convert.ToInt32(DB.ExecuteScalar("SELECT Count(*) FROM " + MODULE_DB_SCHEMA + " WHERE VAF_TableView_ID =" + VAF_TableView_ID
                                          + " AND Record_ID = " + Record_ID + " AND VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID, null, Get_TrxName()));
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        private bool InsertIntoDBSchema(int VAF_TableView_ID, int Record_ID, string TableName, String RName, string whereClause)
        {
            //Check Existence

            int count = Convert.ToInt32(DB.ExecuteScalar("SELECT Count(*) FROM " + MODULE_DB_SCHEMA + " WHERE VAF_TableView_ID =" + VAF_TableView_ID
                                          + " AND Record_ID = " + Record_ID + " AND VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID, null, Get_TrxName()));

            if (count == 0)
            {
                MModuleDBSchema schema = new MModuleDBSchema(_ctx, 0, Get_TrxName());
                schema.SetVAF_TableView_ID(VAF_TableView_ID);
                schema.SetRecord_ID(Record_ID);
                schema.SetName(RName);
                schema.SetTableName(TableName);
                schema.SetIsActive(true);
                schema.SetVAF_ModuleInfo_ID(VAF_ModuleInfo_ID);

                try
                {
                    if (schema.Save())
                    {
                        // Check For Export ID
                        object Export_ID = DB.ExecuteScalar("SELECT Export_ID FROM " + TableName + " WHERE " + whereClause);
                        if (Export_ID == null || Export_ID.ToString() == "")
                        {
                            int expID = MSequence.GetNextExportID(GetCtx().GetVAF_Client_ID(), TableName, null);
                            if (expID == -1)
                            {
                                throw new InvalidConstraintException("ExportID -1 for TableName: " + TableName);
                            }

                            string sql = "UPDATE " + TableName + " SET Export_ID = '" + modulePrefix + expID + "' WHERE " +
                                                    whereClause;
                            // insert Export Id
                            if (DB.ExecuteQuery(sql, null, Get_TrxName()) != 1)
                            {
                                throw new InvalidConstraintException("error =>  " + sql);
                            }
                        }
                        else
                        {
                            //    ;
                            //    //Unchanged ;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                finally
                {
                    //Reset to current Prefix
                    modulePrefix = currentPrefix;
                }
            }
            return true;
        }


        private List<int> GetIDs(String TableName, String IdColumn, String WhereClause)
        {
            return GetIDs(TableName, IdColumn, WhereClause, "");
        }

        private List<int> GetIDs(String TableName, String IdColumn, String WhereClause, string OrderBy)
        {
            List<int> list = new List<int>();

            IDataReader dr = DB.ExecuteReader("SELECT " + IdColumn + " FROM " + TableName + " WHERE " + WhereClause + " AND IsActive='Y' " + OrderBy);
            while (dr.Read())
            {
                list.Add(Utility.Util.GetValueOfInt(dr[0]));
            }
            dr.Close();
            return list;
        }

        private int GetID(String TableName, String IdColumn, String WhereClause)
        {
            int id = 0;
            id = Util.GetValueOfInt(DB.ExecuteScalar("SELECT " + IdColumn + " FROM " + TableName + " WHERE " + WhereClause));
            return id;
        }

        private bool HasModulePrefix(string nameCol, String tableName, string whereClause, out string name)
        {
            name = DB.ExecuteScalar("SELECT " + nameCol + " FROM " + tableName + " WHERE " + whereClause).ToString();

            Debug.Write(tableName + name);

            modulePrefix = currentPrefix;

            //Check Against All Module Prefix
            for (int i = 0; i < prefixList.Count; i++)
            {
                if (name == prefixList[i])
                    return false;
                if (name.StartsWith(prefixList[i], StringComparison.Ordinal))
                {
                    if (!usedPrefixesList.Contains(prefixList[i]))
                    {
                        usedPrefixesList.Add(prefixList[i]);
                    }
                    /* May Use Other Module data*/
                    // modulePrefix = prefixList[i];
                    return true;
                }

            }

            //Special Check for ViennaPrefix
            if (currentPrefix == VIENNA_PREFIX)
            {
                /* Vienna Prefix */
                modulePrefix = VIENNA_PREFIX;
                return true;
            }
            else
            {
                DateTime? updatedDate = Util.GetValueOfDateTime(DB.ExecuteScalar("SELECT Updated FROM " + tableName + " WHERE " + whereClause));

                if (updatedDate != null && updatedDate > RELEASEDDATE)
                {
                    return true;
                }

            }


            return false;
        }

        #endregion

        private void GenerateSchema(int VAF_ModuleInfo_ID)
        {
            GetModuleInfo(VAF_ModuleInfo_ID);
            GetModuleMenus(VAF_ModuleInfo_ID);
            GetForms(VAF_ModuleInfo_ID);
            GetWindowTabField(VAF_ModuleInfo_ID);
            GetProcesses(VAF_ModuleInfo_ID);
            GetTables(VAF_ModuleInfo_ID);
        }

        /// <summary>
        ///Insert Module Info Folder menu ids in DB Schema
        /// </summary>
        /// <param name="VAF_ModuleInfo_ID"></param>
        private void GetModuleMenus(int VAF_ModuleInfo_ID)
        {
            List<Int32> ids = GetIDs("VAF_ModuleMenuFolder", "VAF_ModuleMenuFolder_ID", " VAF_ModuleInfo_Id = " + VAF_ModuleInfo_ID);
            for (int i = 0; i < ids.Count; i++)
            {
                InsertIntoDBSchema(X_VAF_ModuleMenuFolder.Table_ID, ids[i], X_VAF_ModuleMenuFolder.Table_Name, "_MenuFolder" + i, "VAF_ModuleMenuFolder_ID = " + ids[i]);
            }
        }

        /// <summary>
        /// Insert VAF_Module_Info table entry
        /// </summary>
        /// <param name="VAF_ModuleInfo_ID"></param>
        private void GetModuleInfo(int VAF_ModuleInfo_ID)
        {
            string name;
            HasModulePrefix("Name", "VAF_ModuleInfo", "VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID, out name);
            // {
            InsertIntoDBSchema(X_VAF_ModuleInfo.Table_ID, VAF_ModuleInfo_ID, X_VAF_ModuleInfo.Table_Name, name, "VAF_ModuleInfo_ID =" + VAF_ModuleInfo_ID);
            //}
        }

        /// <summary>
        /// Mark Module Process
        /// </summary>
        /// <param name="VAF_ModuleInfo_ID">ModuleInfo ID</param>
        private void GetProcesses(int VAF_ModuleInfo_ID)
        {
            List<int> lstProcessIds = GetIDs("VAF_ModuleProcess", "VAF_ModuleProcess_ID", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID);
            for (int p = 0; p < lstProcessIds.Count; p++)
            {
                int id = GetID("VAF_ModuleProcess", "VAF_Job_ID", "VAF_ModuleProcess_ID = " + lstProcessIds[p]);
                GetProcess(id);
                InsertIntoDBSchema(X_VAF_ModuleProcess.Table_ID, lstProcessIds[p], X_VAF_ModuleProcess.Table_Name, "_ModuleProcess" + p, "VAF_ModuleProcess_ID =" + lstProcessIds[p]);
            }
        }


        #region Table

        /// <summary>
        /// Market Tables of Module
        /// </summary>
        /// <param name="VAF_ModuleInfo_ID">id of module</param>
        private void GetTables(int VAF_ModuleInfo_ID)
        {
            List<int> tableIds = GetIDs("VAF_ModuleTable", "VAF_ModuleTable_ID", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID);
            //string name;
            for (int t = 0; t < tableIds.Count; t++)
            {
                int VAF_TableView_ID = GetID("VAF_ModuleTable", "VAF_TableView_ID", "VAF_ModuleTable_ID = " + tableIds[t]);
                if (GetTable(VAF_TableView_ID))
                {
                    List<int> columnIds = GetIDs("VAF_Column", "VAF_Column_ID", "VAF_TableView_ID=" + VAF_TableView_ID);
                    for (int c = 0; c < columnIds.Count; c++)
                    {
                        GetColumn(columnIds[c], true, false);
                    }
                }
                InsertIntoDBSchema(X_VAF_ModuleTable.Table_ID, tableIds[t], X_VAF_ModuleTable.Table_Name, "ModuleTable_" + t, "VAF_ModuleTable_ID=" + tableIds[t]);
            }
        }

        #endregion

        #region Forms

        /// <summary>
        /// Insert Module Forms
        /// </summary>
        /// <param name="VAF_ModuleInfo_ID"></param>
        private void GetForms(int VAF_ModuleInfo_ID)
        {
            List<int> lstModuleFormIds = GetIDs("VAF_ModuleForm", "VAF_ModuleForm_ID", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID);

            for (int i = 0; i < lstModuleFormIds.Count; i++)
            {
                int id = GetID("VAF_ModuleForm", "VAF_Page_ID", "VAF_ModuleForm_ID = " + lstModuleFormIds[i]);
                GetForm(id);
                InsertIntoDBSchema(X_VAF_ModuleForm.Table_ID, lstModuleFormIds[i], X_VAF_ModuleForm.Table_Name, "_ModuleFrom" + i, "VAF_ModuleForm_ID =" + lstModuleFormIds[i]);
            }
        }

        /// <summary>
        /// Mark Module Form
        /// </summary>
        /// <param name="sVAF_Page_ID">id of form</param>
        void GetForm(int sVAF_Page_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Page", "VAF_Page_ID =" + sVAF_Page_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Page.Table_ID, sVAF_Page_ID, X_VAF_Page.Table_Name, name, "VAF_Page_ID =" + sVAF_Page_ID);

                // 1000175
                DataSet ds = DB.ExecuteDataset("SELECT * FROM VAF_CrystalReport_Para WHERE VAF_Page_ID = " + sVAF_Page_ID);


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    name = "";
                    X_VAF_CrystalReport_Para cPara = new X_VAF_CrystalReport_Para(_ctx, ds.Tables[0].Rows[i], null);

                    if (cPara.GetVAF_ColumnDic_ID() > 0)
                    {
                        if (HasModulePrefix("ColumnName", "VAF_ColumnDic", "VAF_ColumnDic_ID =" + cPara.GetVAF_ColumnDic_ID(), out name))
                        {
                            InsertIntoDBSchema(X_VAF_ColumnDic.Table_ID, cPara.GetVAF_ColumnDic_ID(), X_VAF_ColumnDic.Table_Name, name, "VAF_ColumnDic_ID = " + cPara.GetVAF_ColumnDic_ID());
                        }
                    }

                    if (cPara.GetVAF_Control_Ref_Value_ID() > 0)
                    {
                        GetReference(cPara.GetVAF_Control_Ref_Value_ID());//mark reference record [ reference key ]
                    }

                    if (cPara.GetVAF_DataVal_Rule_ID() > 0)
                    {
                        GetValRule(cPara.GetVAF_DataVal_Rule_ID()); //Mark Val_rule recored [ dynamic validation ]
                    }

                    InsertIntoDBSchema(X_VAF_CrystalReport_Para.Table_ID, cPara.GetVAF_CrystalReport_Para_ID(), X_VAF_CrystalReport_Para.Table_Name, cPara.GetColumnName(), "VAF_CrystalReport_Para_ID = " + cPara.GetVAF_CrystalReport_Para_ID());

                }
            }
        }

        #endregion

        #region Messages

        private String GetError()
        {

            //String msg = "SaveError";
            String info = "";
            //ValueNamePair ppE = VLogger.RetrieveError();
            //if (ppE != null)
            //{
            //    msg = ppE.GetValue();
            //    info = ppE.GetName();
            //    //	Unique Constraint
            //    Exception ex = VLogger.RetrieveException();
            //    if (ex != null
            //        && ex is System.Data.Common.DbException
            //        && ((System.Data.Common.DbException)ex).ErrorCode == -2146232008)
            //        msg = "SaveErrorNotUnique";
            //}

            StringBuilder sb = new StringBuilder("");
            // String msg = e.GetMessage();
            //if (msg != null && msg.Length > 0)
            // sb.Append(VAdvantage.Utility.Msg.GetMsg(_ctx, msg));
            //String info = e.GetInfo();
            if (info != null && info.Length > 0)
            {
                if (sb.Length > 0 && !sb.ToString().Trim().EndsWith(":"))
                    sb.Append(": ");
                sb.Append(info);
            }
            if (sb.Length > 0)
            {
                int pos = sb.ToString().IndexOf("\n");
                if (pos != -1)  // replace CR/NL
                    sb.Replace("\n", " - ", pos, 1);
                //statusBar.SetStatusLine(sb.ToString(), e.IsError());
                //CommonFunctions.ShowMessage(sb.ToString(), statusBar, e.IsError());
            }
            return sb.ToString();
        }

        private void DoReportInfo(string msg)
        {

            DoReport(msg, "", 3, "", false);
        }

        private void DoReportInfo(string msg, string msg2)
        {
            DoReport(msg, msg2, 3, "", false);
        }


        private void DoReport(string msg)
        {
            DoReport(msg, "", 3, "", false);
        }

        private void DoReport(string msg, string msg2)
        {
            DoReport(msg, msg2, 3, "", false);
        }

        private void DoReportBold(string msg)
        {
            DoReport(msg, "", 3, "", true);
        }

        private void DoReportBold(string msg, string msg2)
        {
            DoReport(msg, msg2, 3, "", true);
        }

        private void DoReportWarn(string msg, string msg2, string detail)
        {
            DoReport(msg, msg2, 2, detail, false);
        }

        private void DoReportOk(string msg, string msg2, string detail)
        {
            DoReport(msg, msg2, 0, detail, false);
        }

        private void DoReportError(string msg, string msg2, string detail)
        {
            DoReport(msg, msg2, 1, detail, false);
        }

        private void DoReport(string msg, string msg2, int imgIndex, string detail, bool isBold)
        {
            //msg = Msg.GetMsg(_ctx, msg);
            //if (msg2 == null)
            //{
            //    msg2 = "Null Value";

            //}
            //if (msg != null && msg2.Length > 0)
            //{
            //    msg = msg + " -> " + msg2;
            //}
            //if (detail.Length > 0 && imgIndex != 1) //error message already changed
            //{
            //    detail = Msg.GetMsg(_ctx, detail);
            //}

            //System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem("", imgIndex);
            //if (imgIndex == 1)
            //{
            //    item.ForeColor = System.Drawing.Color.Red;
            //}
            //else if (imgIndex == 2)
            //{
            //    item.ForeColor = System.Drawing.Color.Blue;
            //}
            //else if (imgIndex == 3 && isBold)
            //{
            //    item.Font = new System.Drawing.Font(item.Font, System.Drawing.FontStyle.Bold);
            //}

            //item.SubItems.Add(msg);
            //item.SubItems.Add(detail);
            //_item.Add(item);
            //_worker.ReportProgress(0);
        }

        #endregion

        #region "Common"

        private void CheckCtxArea(int sVAF_ContextScope_ID)
        {
            if (!IsRecordExistInDBSchema(X_VAF_ContextScope.Table_ID, sVAF_ContextScope_ID))
            {
                string name;
                if (HasModulePrefix("Name", "VAF_ContextScope", "VAF_ContextScope_ID = " + sVAF_ContextScope_ID, out name))
                {
                    InsertIntoDBSchema(X_VAF_ContextScope.Table_ID, sVAF_ContextScope_ID, X_VAF_ContextScope.Table_Name, name, " VAF_ContextScope_ID = " + sVAF_ContextScope_ID);
                }
            }
        }

        private void CheckImage(int sVAF_Image_ID)
        {
            if (!IsRecordExistInDBSchema(X_VAF_Image.Table_ID, sVAF_Image_ID))
            {
                string name;
                if (HasModulePrefix("Name", "VAF_Image", "VAF_Image_ID = " + sVAF_Image_ID, out name))
                {
                    InsertIntoDBSchema(X_VAF_Image.Table_ID, sVAF_Image_ID, X_VAF_Image.Table_Name, name, " VAF_Image_ID = " + sVAF_Image_ID);
                }
            }
        }


        #endregion

        #region Window Tab Field

        private void GetWindowTabField(int VAF_ModuleInfo_ID)
        {
            List<int> windowIds = GetIDs("VAF_ModuleWindow", " VAF_ModuleWindow_ID ", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID);

            for (int i = 0; i < windowIds.Count; i++)
            {
                int sAD_Window_ID = GetID("VAF_ModuleWindow", "AD_Window_ID", "VAF_ModuleWindow_ID = " + windowIds[i]);
                GetWindow(sAD_Window_ID, windowIds[i]);

                //int Id = GetID("VAF_ModuleWindow", "VAF_ModuleWindow_ID", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID + " AND  AD_Window_ID =" + sAD_Window_ID);
                //InsertIntoDBSchema(X_VAF_ModuleWindow.Table_ID, windowIds[i], X_VAF_ModuleWindow.Table_Name, "ModuleWindow" + i, "VAF_ModuleWindow_ID =" + windowIds[i]);

            }
        }

        void GetWindow(int sAD_Window_ID, int sVAF_ModuleWindow_ID)
        {
            // if (IsRecordExistInDBSchema(X_AD_Window.Table_ID, sAD_Window_ID))
            //  return;
            string name;
            if (HasModulePrefix("Name", "AD_Window", "AD_WINDOW_ID =" + sAD_Window_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Window.Table_ID, sAD_Window_ID, X_AD_Window.Table_Name, name, "AD_WINDOW_ID =" + sAD_Window_ID);
            }


            if (sVAF_ModuleWindow_ID > 0)
            {

                IDataReader dr = DB.ExecuteReader("SELECT AD_WindowAction_ID FROM AD_WindowAction WHERE AD_Window_ID =" + sAD_Window_ID);
                try
                {
                    if (dr.Read())
                    {
                        int id = dr.GetInt32(0);
                        InsertIntoDBSchema(1000331, id, "AD_WindowAction", "windowAction_" + id, "AD_WindowAction_ID = " + id);
                    }
                    dr.Close();
                }
                catch
                {
                    dr.Close();
                }

                InsertIntoDBSchema(X_VAF_ModuleWindow.Table_ID, sVAF_ModuleWindow_ID, X_VAF_ModuleWindow.Table_Name, "_ModuleWindow" + sVAF_ModuleWindow_ID, "VAF_ModuleWindow_ID =" + sVAF_ModuleWindow_ID);

                List<int> lstModuleTab = GetIDs("VAF_ModuleTab", "VAF_ModuleTab_ID", "VAF_ModuleWindow_ID = " + sVAF_ModuleWindow_ID);//+ " ORDER BY SeqNo");
                GetTabsFields(sAD_Window_ID, lstModuleTab);
            }
        }

        private void GetTabsFields(int sAD_Window_ID, List<int> lstModuleTab)
        {
            //DataTable dt = GetTable("SELECT * FROM VAF_Tab WHERE AD_Window_ID=" + sWindowObj.GetAD_Window_ID() + " ORDER BY SeqNo");
            MTab sTab = null;
            int sVAF_Tab_ID = 0;
            for (int i = 0; i < lstModuleTab.Count; i++)
            {
                sVAF_Tab_ID = GetID("VAF_ModuleTab", "VAF_Tab_ID", "VAF_ModuleTab_ID = " + lstModuleTab[i]);

                bool newTable = false;
                sTab = new MTab(GetCtx(), sVAF_Tab_ID, null);
                string name;

                if (HasModulePrefix("TableName", "VAF_TableView", " VAF_TableView_ID = " + sTab.GetVAF_TableView_ID(), out name))
                {

                    MTable sTable = new MTable(_ctx, sTab.GetVAF_TableView_ID(), null);

                    if (sTable.GetAD_Window_ID() > 0 && sAD_Window_ID != sTable.GetAD_Window_ID())
                    {
                        string name2;
                        if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sTable.GetAD_Window_ID(), out name2))
                        {
                            InsertIntoDBSchema(X_AD_Window.Table_ID, sTable.GetAD_Window_ID(), X_AD_Window.Table_Name, name2, "AD_Window_ID =" + sTable.GetAD_Window_ID());
                        }
                    }

                    if (sTable.GetPO_Window_ID() > 0 && sAD_Window_ID != sTable.GetPO_Window_ID())
                    {

                        string name3;
                        if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sTable.GetPO_Window_ID(), out name3))
                        {
                            InsertIntoDBSchema(X_AD_Window.Table_ID, sTable.GetPO_Window_ID(), X_AD_Window.Table_Name, name3, "AD_Window_ID =" + sTable.GetPO_Window_ID());
                        }
                    }

                    if (sTable.GetReferenced_Table_ID() > 0 && sTable.GetVAF_TableView_ID() != sTable.GetReferenced_Table_ID())
                    {
                        string name4;
                        if (HasModulePrefix("TableName", "VAF_TableView", "VAF_TableView_ID = " + sTable.GetReferenced_Table_ID(), out name4))
                        {
                            InsertIntoDBSchema(X_VAF_TableView.Table_ID, sTable.GetReferenced_Table_ID(), X_VAF_TableView.Table_Name, name4, "VAF_TableView_ID =" + sTable.GetReferenced_Table_ID());
                        }
                    }

                    InsertIntoDBSchema(X_VAF_TableView.Table_ID, sTab.GetVAF_TableView_ID(), X_VAF_TableView.Table_Name, name, "VAF_TableView_ID = " + sTab.GetVAF_TableView_ID());
                    InsertDefaultColumn(sTab.GetVAF_TableView_ID());
                    newTable = true;
                }



                if (sTab.GetVAF_Column_ID() != 0)
                {
                    //  if (HasModulePrefix("ColumnName", "VAF_Column", "VAF_Column_ID = " + sTab.GetVAF_Column_ID(), out name))
                    //{
                    GetColumn(sTab.GetVAF_Column_ID(), true, false);
                    //InsertIntoDBSchema(X_VAF_Column.Table_ID, sTab.GetVAF_Column_ID(), X_VAF_Column.Table_Name, name, " VAF_Column_ID = " + sTab.GetVAF_Column_ID());
                    //}
                }

                if (sTab.GetVAF_ContextScope_ID() != 0)
                {
                    CheckCtxArea(sTab.GetVAF_ContextScope_ID());
                }

                //ImageID
                if (sTab.GetVAF_Image_ID() != 0)
                {
                    CheckImage(sTab.GetVAF_Image_ID());
                }

                if (sTab.GetReferenced_Tab_ID() != 0)
                {
                    MTab rTab = new MTab(GetCtx(), sTab.GetReferenced_Tab_ID(), null);

                    if (HasModulePrefix("TableName", "VAF_TableView", " VAF_TableView_ID = " + rTab.GetVAF_TableView_ID(), out name))
                    {
                        InsertIntoDBSchema(X_VAF_TableView.Table_ID, rTab.GetVAF_TableView_ID(), X_VAF_TableView.Table_Name, name, "VAF_TableView_ID = " + rTab.GetVAF_TableView_ID());
                    }
                    InsertIntoDBSchema(MTab.Table_ID, rTab.GetVAF_Tab_ID(), MTab.Table_Name, rTab.GetName(), " VAF_Tab_ID = " + rTab.GetVAF_Tab_ID());
                }

                if (sTab.GetVAF_Job_ID() != 0)
                {
                    GetProcess(sTab.GetVAF_Job_ID());
                }

                //New Table HeaderLayout
                if (sTab.GetVAF_HeaderLayout_ID() > 0)
                {
                    GetHeaderLayout(sTab.GetVAF_HeaderLayout_ID());
                }

                InsertIntoDBSchema(MTab.Table_ID, sVAF_Tab_ID, MTab.Table_Name, sTab.GetName(), " VAF_Tab_ID = " + sVAF_Tab_ID);


                GetTabPanel(sVAF_Tab_ID);


                //Insert VAF_ModuleTab Info

                //int Id = GetID("VAF_ModuleTab", "VAF_ModuleTab_ID", "VAF_ModuleInfo_ID = " + VAF_ModuleInfo_ID + " AND  AD_Window_ID =" + windowIds[i]);
                InsertIntoDBSchema(X_VAF_ModuleTab.Table_ID, lstModuleTab[i], X_VAF_ModuleTab.Table_Name, "_ModuleTab" + i, "VAF_ModuleTab_ID =" + lstModuleTab[i]);


                List<int> lstModuleField = GetIDs("VAF_ModuleField", "VAF_ModuleField_ID", "VAF_ModuleTab_ID = " + lstModuleTab[i], " ORDER BY SeqNo");

                int sVAF_Field_ID = 0;
                List<int> fields = new List<int>();
                for (int f = 0; f < lstModuleField.Count; f++)
                {

                    sVAF_Field_ID = GetID("VAF_ModuleField", "VAF_Field_ID", "VAF_ModuleField_ID =" + lstModuleField[f]);


                    name = InsertField(newTable, name, sVAF_Field_ID);

                    fields.Add(sVAF_Field_ID);

                    InsertIntoDBSchema(X_VAF_ModuleField.Table_ID, lstModuleField[f], X_VAF_ModuleField.Table_Name, "_ModuleField" + i, "VAF_ModuleField_ID =" + lstModuleField[f]);
                }

                ///Insert default Field
                ///
                if (lstModuleField.Count > 0) // run only if at least one field is binded in modulefield and new tab created
                {
                    IDataReader dr = DB.ExecuteReader(@" SELECT Name, VAF_Field_ID FROM VAF_Field WHERE VAF_Column_ID IN
                                                        ( SELECT VAF_Column_ID FROM VAF_Column WHERE ColumnName IN ('IsActive','VAF_Client_ID', 'VAF_Org_ID','" + MTable.Get(_ctx, sTab.GetVAF_TableView_ID()).GetTableName() + @"_ID',
                                                                                                                 'Export_ID')
                                                        AND VAF_TableView_ID = " + sTab.GetVAF_TableView_ID() + @") AND VAF_Tab_ID =" + sTab.GetVAF_Tab_ID());
                    while (dr.Read())
                    {
                        if (!fields.Contains(dr.GetInt32(1)))
                        {
                            InsertField(newTable, dr[0].ToString(), dr.GetInt32(1));
                        }
                    }
                    dr.Close();
                }
                fields.Clear();
                fields = null;
            }
        }

        /// <summary>
        ///  Mark Tab Panel record if any
        /// </summary>
        /// <param name="sVAF_Tab_ID"> Tab ID</param>
        private void GetTabPanel(int sVAF_Tab_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT * FROM VAF_TabPanel WHERE VAF_Tab_ID=" + sVAF_Tab_ID);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    InsertIntoDBSchema(X_VAF_TabPanel.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_TabPanel_ID"]),
                        X_VAF_TabPanel.Table_Name, ds.Tables[0].Rows[i]["Name"].ToString(), "VAF_TabPanel_ID =" + ds.Tables[0].Rows[i]["VAF_TabPanel_ID"]);
                }
            }
        }

        private void GetHeaderLayout(int sVAF_HeaderLayout_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_HeaderLayout", "VAF_HeaderLayout_ID=" + sVAF_HeaderLayout_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_HeaderLayout.Table_ID, sVAF_HeaderLayout_ID, X_VAF_HeaderLayout.Table_Name, name, "VAF_HeaderLayout_ID =" + sVAF_HeaderLayout_ID);
            } //Header

            DataSet ds = DB.ExecuteDataset("SELECT * FROM VAF_GridLayout WHERE VAF_HeaderLayout_ID=" + sVAF_HeaderLayout_ID);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++) //Grid header
            {
                if (HasModulePrefix("Name", "VAF_GridLayout", "VAF_GridLayout_ID = " + ds.Tables[0].Rows[i]["VAF_GridLayout_ID"], out name))
                {
                    InsertIntoDBSchema(X_VAF_GridLayout.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_GridLayout_ID"]),
                        X_VAF_GridLayout.Table_Name, name, "VAF_GridLayout_ID=" + ds.Tables[0].Rows[i]["VAF_GridLayout_ID"]);

                    DataSet dsItem = DB.ExecuteDataset("SELECT * FROM VAF_GridLayoutItems WHERE VAF_GridLayout_ID=" + ds.Tables[0].Rows[i]["VAF_GridLayout_ID"]);
                    for (int j = 0; j < dsItem.Tables[0].Rows.Count; j++) //GridItems
                    {
                        //if (HasModulePrefix("Name", "VAF_GridLayoutItems", "VAF_GridLayoutItems_ID = " + dsItem.Tables[0].Rows[j]["VAF_GridLayoutItems_ID"], out name))
                        // {
                        InsertIntoDBSchema(X_VAF_GridLayoutItems.Table_ID, Util.GetValueOfInt(dsItem.Tables[0].Rows[j]["VAF_GridLayoutItems_ID"]),
                            X_VAF_GridLayoutItems.Table_Name, "item" + j, "VAF_GridLayoutItems_ID=" + dsItem.Tables[0].Rows[j]["VAF_GridLayoutItems_ID"]);
                        //}
                    }
                }
            }
        }

        private string InsertField(bool newTable, string name, int sVAF_Field_ID)
        {
            MField sField = new MField(_ctx, sVAF_Field_ID, null);

            GetColumn(sField.GetVAF_Column_ID(), true, !newTable);

            if (sField.GetVAF_FieldSection_ID() > 0)
            {
                InsertIntoDBSchema(X_VAF_FieldSection.Table_ID, sField.GetVAF_FieldSection_ID(), X_VAF_FieldSection.Table_Name, "FieldGroup_" + sField.GetVAF_FieldSection_ID(), "VAF_FieldSection_ID = " + sField.GetVAF_FieldSection_ID());
            }

            if (sField.GetZoomWindow_ID() > 0)
            {
                if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sField.GetZoomWindow_ID(), out name))
                {
                    InsertIntoDBSchema(X_AD_Window.Table_ID, sField.GetZoomWindow_ID(), X_AD_Window.Table_Name, name, " AD_Window_ID = " + sField.GetZoomWindow_ID());
                }
            }
            if (sField.GetVAF_QuickSearchWindow_ID() > 0)
            {
                if (HasModulePrefix("Name", "VAF_QuickSearchWindow", "VAF_QuickSearchWindow_ID = " + sField.GetVAF_QuickSearchWindow_ID(), out name))
                {
                    InsertIntoDBSchema(X_VAF_QuickSearchWindow.Table_ID, sField.GetVAF_QuickSearchWindow_ID(), X_VAF_QuickSearchWindow.Table_Name, name, " VAF_QuickSearchWindow_ID = " + sField.GetVAF_QuickSearchWindow_ID());
                }

                DataSet ds = DB.ExecuteDataset("SELECT * FROM VAF_QuickSearchColumn WHERE VAF_QuickSearchWindow_ID=" + sField.GetVAF_QuickSearchWindow_ID());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (HasModulePrefix("Name", "VAF_QuickSearchColumn", "VAF_QuickSearchColumn_ID = " + ds.Tables[0].Rows[i]["VAF_QuickSearchColumn_ID"], out name))
                    {

                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_CONTROL_REF_VALUE_ID"]) > 0)
                        {
                            InsertIntoDBSchema(X_VAF_Control_Ref.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_CONTROL_REF_VALUE_ID"]), X_VAF_Control_Ref.Table_Name, name,
                           " VAF_Control_Ref_ID = " + ds.Tables[0].Rows[i]["VAF_CONTROL_REF_VALUE_ID"]);
                        }
                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_COLUMNDIC_ID"]) > 0)
                        {
                            InsertIntoDBSchema(X_VAF_ColumnDic.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_COLUMNDIC_ID"]), X_VAF_ColumnDic.Table_Name, name,
                           " VAF_ColumnDic_ID = " + ds.Tables[0].Rows[i]["VAF_COLUMNDIC_ID"]);
                        }

                        InsertIntoDBSchema(X_VAF_QuickSearchColumn.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["VAF_QuickSearchColumn_ID"]), X_VAF_QuickSearchColumn.Table_Name, name,
                            " VAF_QuickSearchColumn_ID = " + ds.Tables[0].Rows[i]["VAF_QuickSearchColumn_ID"]);
                    }
                }
            }

            if (sField.GetVAF_Image_ID() > 0)
            {
                CheckImage(sField.GetVAF_Image_ID());
            }

            InsertIntoDBSchema(X_VAF_Field.Table_ID, sVAF_Field_ID, X_VAF_Field.Table_Name, sField.GetName(), " VAF_Field_ID = " + sVAF_Field_ID);
            return name;
        }

        /// <summary>
        /// If New Table bound to Tab
        /// </summary>
        /// <param name="sVAF_TableView_ID"></param>
        private void InsertDefaultColumn(int sVAF_TableView_ID)
        {
            string tableName = DB.ExecuteScalar("SELECT TableName FROM VAF_TableView WHERE VAF_TableView_ID = " + sVAF_TableView_ID).ToString();

            string stdWhere = " ColumnName IN ('IsActive','VAF_Client_ID', 'VAF_Org_ID','Created','CreatedBy','Updated','UpdatedBy','"
                              + tableName + "_ID','Export_ID')";

            IDataReader dr = DB.ExecuteReader("SELECT VAF_Column_ID,Name,ColumnName FROM VAF_Column WHERE VAF_TableView_ID = " + sVAF_TableView_ID + " AND  " + stdWhere);
            int id = 0;
            // string colName = "";
            while (dr.Read())
            {
                id = Util.GetValueOfInt(dr[0]);

                /* Get System Element for ID Column*/

                if (dr[2].ToString() == tableName + "_ID")
                {
                    GetColumn(id, false, false);
                }
                InsertIntoDBSchema(X_VAF_Column.Table_ID, id, X_VAF_Column.Table_Name, Util.GetValueOfString(dr[1]), " VAF_Column_ID = " + id);
            }
            dr.Close();
        }

        private bool GetTable(int sVAF_TableView_ID)
        {
            string name;
            if (HasModulePrefix("TableName", "VAF_TableView", "VAF_TableView_ID=" + sVAF_TableView_ID, out name))
            {
                MTable sTable = new MTable(_ctx, sVAF_TableView_ID, null);

                if (sTable.GetAD_Window_ID() > 0)
                {
                    string name2;
                    if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sTable.GetAD_Window_ID(), out name2))
                    {
                        InsertIntoDBSchema(X_AD_Window.Table_ID, sTable.GetAD_Window_ID(), X_AD_Window.Table_Name, name2, "AD_Window_ID =" + sTable.GetAD_Window_ID());
                    }
                }

                if (sTable.GetPO_Window_ID() > 0)
                {

                    string name3;
                    if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sTable.GetPO_Window_ID(), out name3))
                    {
                        InsertIntoDBSchema(X_AD_Window.Table_ID, sTable.GetPO_Window_ID(), X_AD_Window.Table_Name, name3, "AD_Window_ID =" + sTable.GetPO_Window_ID());
                    }
                }

                if (sTable.GetReferenced_Table_ID() > 0)
                {
                    string name4;
                    if (HasModulePrefix("TableName", "VAF_TableView", "VAF_TableView_ID = " + sTable.GetReferenced_Table_ID(), out name4))
                    {
                        InsertIntoDBSchema(X_VAF_TableView.Table_ID, sTable.GetReferenced_Table_ID(), X_VAF_TableView.Table_Name, name4, "VAF_TableView_ID =" + sTable.GetReferenced_Table_ID());
                    }
                }


                InsertIntoDBSchema(X_VAF_TableView.Table_ID, sVAF_TableView_ID, X_VAF_TableView.Table_Name, name, "VAF_TableView_ID =" + sVAF_TableView_ID);
                InsertDefaultColumn(sVAF_TableView_ID);
                return true;
            }
            return false;
        }

        private void GetColumn(int sVAF_Column_ID)
        {
            GetColumn(sVAF_Column_ID, false, false);
        }
        private void GetColumn(int sVAF_Column_ID, bool checkRP)
        {
            GetColumn(sVAF_Column_ID, checkRP, false);
        }
        private void GetColumn(int svaf_column_ID, bool checkRP, bool checkModulePrefix)
        {
            string name;
            if (HasModulePrefix("ColumnName", "VAF_Column", "VAF_Column_ID=" + svaf_column_ID, out name))
            {
                int sVAF_ColumnDic_ID = GetID("VAF_Column", "VAF_ColumnDic_ID", "VAF_Column_ID=" + svaf_column_ID);

                GetElement(sVAF_ColumnDic_ID);

                if (checkRP)
                {
                    CheckReferenceAndProcess(svaf_column_ID);
                }
                InsertIntoDBSchema(X_VAF_Column.Table_ID, svaf_column_ID, X_VAF_Column.Table_Name, name, "VAF_Column_ID =" + svaf_column_ID);
            }
            else if (!checkModulePrefix)
            {
                if (checkRP)
                {
                    CheckReferenceAndProcess(svaf_column_ID);
                }
                InsertIntoDBSchema(X_VAF_Column.Table_ID, svaf_column_ID, X_VAF_Column.Table_Name, name, "VAF_Column_ID =" + svaf_column_ID);
            }
        }

        private void GetElement(int sVAF_ColumnDic_ID)
        {
            string name;
            if (HasModulePrefix("ColumnName", "VAF_ColumnDic", "VAF_ColumnDic_ID=" + sVAF_ColumnDic_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_ColumnDic.Table_ID, sVAF_ColumnDic_ID, X_VAF_ColumnDic.Table_Name, name, "VAF_ColumnDic_ID =" + sVAF_ColumnDic_ID);
            }
        }

        private void CheckReferenceAndProcess(int svaf_column_id)
        {
            MColumn sColumn = new MColumn(_ctx, svaf_column_id, null);
            if (DisplayType.IsLookup(sColumn.GetVAF_Control_Ref_ID()) || DisplayType.Button == sColumn.GetVAF_Control_Ref_ID())
            {

                if (DisplayType.TableDir != sColumn.GetVAF_Control_Ref_ID())
                {
                    if (sColumn.GetVAF_Control_Ref_Value_ID() != 0)
                    {
                        GetReference(sColumn.GetVAF_Control_Ref_Value_ID());

                    }
                }

                if (sColumn.GetVAF_DataVal_Rule_ID() != 0)
                {
                    GetValRule(sColumn.GetVAF_DataVal_Rule_ID());
                }

                if (sColumn.GetVAF_Job_ID() != 0)
                {
                    GetProcess(sColumn.GetVAF_Job_ID());
                }

                if (sColumn.GetVAF_Page_ID() > 0)
                {
                    GetForm(sColumn.GetVAF_Page_ID());
                }

            }
        }

        private void GetValRule(int sVAF_DataVal_Rule_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_DataVal_Rule", "VAF_DataVal_Rule_ID =" + sVAF_DataVal_Rule_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_DataVal_Rule.Table_ID, sVAF_DataVal_Rule_ID, X_VAF_DataVal_Rule.Table_Name, name, "VAF_DataVal_Rule_ID =" + sVAF_DataVal_Rule_ID);
            }
        }

        private void GetReference(int sVAF_Control_Ref_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Control_Ref", "VAF_Control_Ref_ID =" + sVAF_Control_Ref_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Control_Ref.Table_ID, sVAF_Control_Ref_ID, X_VAF_Control_Ref.Table_Name, name, "VAF_Control_Ref_ID =" + sVAF_Control_Ref_ID);

                MReference sReference = new MReference(_ctx, sVAF_Control_Ref_ID, null);

                if (sReference.GetValidationType() == X_VAF_Control_Ref.VALIDATIONTYPE_ListValidation)
                {
                    IDataReader dr = DataBase.DB.ExecuteReader("Select VAF_CtrlRef_List_ID,Name From VAF_CtrlRef_List Where VAF_Control_Ref_ID =" + sReference.GetVAF_Control_Ref_ID());
                    while (dr.Read())
                    {
                        InsertIntoDBSchema(X_VAF_CtrlRef_List.Table_ID, Convert.ToInt32(dr[0]), X_VAF_CtrlRef_List.Table_Name, dr[1].ToString(), "VAF_CtrlRef_List_ID = " + Convert.ToInt32(dr[0]));
                    }
                    dr.Close();
                }
                else if (sReference.GetValidationType() == X_VAF_Control_Ref.VALIDATIONTYPE_TableValidation)
                {
                    MRefTable sRefTable = null;

                    DataSet ds = DataBase.DB.ExecuteDataset("Select * From VAF_CtrlRef_Table Where VAF_Control_Ref_ID =" + sReference.GetVAF_Control_Ref_ID());

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        sRefTable = new MRefTable(_ctx, dr, Get_TrxName());

                        GetTable(sRefTable.GetVAF_TableView_ID());
                        GetColumn(sRefTable.GetColumn_Key_ID());
                        GetColumn(sRefTable.GetColumn_Display_ID());
                        InsertIntoDBSchema(X_VAF_CtrlRef_Table.Table_ID, sVAF_Control_Ref_ID, X_VAF_CtrlRef_Table.Table_Name, sRefTable.GetDisplayColumnName(), "VAF_Control_Ref_ID = " + sReference.GetVAF_Control_Ref_ID());
                    }
                }
            }
        }

        private void GetProcess(int sVAF_Job_ID)
        {
            //if (IsRecordExistInDBSchema(X_VAF_Job.Table_ID, sVAF_Job_ID))
            //    return;

            string name;
            if (HasModulePrefix("Name", "VAF_Job", "VAF_Job_ID=" + sVAF_Job_ID, out name))
            {
                MProcess sProcess = new MProcess(_ctx, sVAF_Job_ID, null);

                if (sProcess.GetVAF_ContextScope_ID() != 0)
                {
                    CheckCtxArea(sProcess.GetVAF_ContextScope_ID());
                }

                if (sProcess.GetAD_Workflow_ID() != 0)
                {
                    GetWorkflow(sProcess.GetAD_Workflow_ID());
                }

                if (sProcess.IsReport())
                {
                    if (sProcess.GetVAF_ReportView_ID() != 0)
                    {
                        GetReportView(sProcess.GetVAF_ReportView_ID());
                    }

                    if (sProcess.GetVAF_Print_Rpt_Layout_ID() != 0)
                    {
                        //Set Print Format
                        GetPrintFormat(sProcess.GetVAF_Print_Rpt_Layout_ID());
                    }

                    if (sProcess.GetVAF_ReportLayout_ID() > 0)
                    {
                        GetReportFormat(sProcess.GetVAF_ReportLayout_ID());
                    }
                }

                InsertIntoDBSchema(X_VAF_Job.Table_ID, sVAF_Job_ID, X_VAF_Job.Table_Name, name, "VAF_Job_ID =" + sVAF_Job_ID);

                //ProcessParameter
                DataSet ds = DataBase.DB.ExecuteDataset("Select * From VAF_Job_Para where VAF_Job_ID = " + sVAF_Job_ID);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MProcessPara sPara = new MProcessPara(_ctx, dr, Get_TrxName());
                    if (sPara.GetVAF_ColumnDic_ID() != 0)
                    {
                        GetElement(sPara.GetVAF_ColumnDic_ID());
                    }

                    if (sPara.GetVAF_Control_Ref_Value_ID() != 0)
                    {
                        GetReference(sPara.GetVAF_Control_Ref_Value_ID());
                    }
                    if (sPara.GetVAF_DataVal_Rule_ID() != 0)
                    {
                        GetValRule(sPara.GetVAF_DataVal_Rule_ID());
                    }
                    InsertIntoDBSchema(X_VAF_Job_Para.Table_ID, sPara.GetVAF_Job_Para_ID(), X_VAF_Job_Para.Table_Name, sPara.GetName(), "VAF_Job_Para_ID=" + sPara.GetVAF_Job_Para_ID());
                }
            }
        }

        /// <summary>
        /// New Report Marking(22/8/13)
        /// </summary>
        /// <param name="p"></param>
        private void GetReportFormat(int sVAF_ReportLayout_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_ReportLayout", "VAF_ReportLayout_ID=" + sVAF_ReportLayout_ID, out name))
            {
                X_VAF_ReportLayout sReportFromat = new X_VAF_ReportLayout(_ctx, sVAF_ReportLayout_ID, null);
                InsertIntoDBSchema(X_VAF_ReportLayout.Table_ID, sVAF_ReportLayout_ID, X_VAF_ReportLayout.Table_Name, name, "VAF_ReportLayout_ID=" + sVAF_ReportLayout_ID);

                //Get Report Tables
                DataSet dsRTable = DB.ExecuteDataset("SELECT * FROM VAF_ReportTable WHERE VAF_ReportLayout_ID =" + sVAF_ReportLayout_ID);
                DataSet dsRItems = null;
                foreach (DataRow drTable in dsRTable.Tables[0].Rows)
                {

                    X_VAF_ReportTable rTable = new X_VAF_ReportTable(_ctx, drTable, null);

                    if (rTable.GetVAF_TableView_ID() > 0)
                    {
                        GetTable(rTable.GetVAF_TableView_ID());
                    }

                    if (rTable.GetVAF_TableView_ID_1() > 0)
                    {
                        GetTable(rTable.GetVAF_TableView_ID_1());
                    }

                    if (rTable.GetVAF_Column_ID() > 0)
                    {
                        GetColumn(rTable.GetVAF_Column_ID(), true, false);
                    }

                    InsertIntoDBSchema(X_VAF_ReportTable.Table_ID, rTable.GetVAF_ReportTable_ID(), X_VAF_ReportTable.Table_Name, "ReportTable[" + rTable.GetVAF_ReportTable_ID() + "]", "VAF_ReportTable_ID = " + rTable.GetVAF_ReportTable_ID());

                    //Get Report Items
                    dsRItems = DB.ExecuteDataset("SELECT * FROM VAF_ReportItem WHERE VAF_ReportTable_ID = " + rTable.GetVAF_ReportTable_ID());
                    foreach (DataRow drItem in dsRItems.Tables[0].Rows)
                    {
                        X_VAF_ReportItem rItem = new X_VAF_ReportItem(_ctx, drItem, null);

                        if (rItem.GetVAF_Column_ID() > 0)
                        {
                            GetColumn(rItem.GetVAF_Column_ID(), true, false);
                        }
                        InsertIntoDBSchema(X_VAF_ReportItem.Table_ID, rItem.GetVAF_ReportItem_ID(), X_VAF_ReportItem.Table_Name, "ReportItem[" + rItem.GetVAF_ReportItem_ID() + "]", "VAF_ReportItem_ID=" + rItem.GetVAF_ReportItem_ID());
                    }
                    //end Item loop
                }
                //end table loop
            }
        }

        private void GetReportView(int sVAF_ReportView_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_ReportView", "VAF_ReportView_ID=" + sVAF_ReportView_ID, out name))
            {
                X_VAF_ReportView sReportView = new X_VAF_ReportView(_ctx, sVAF_ReportView_ID, null);

                int rVAF_TableView_ID = GetID("VAF_ReportView", "VAF_TableView_ID", "VAF_ReportView_ID=" + sVAF_ReportView_ID);

                GetTable(rVAF_TableView_ID);

                InsertIntoDBSchema(X_VAF_ReportView.Table_ID, sVAF_ReportView_ID, X_VAF_ReportView.Table_Name, name, "VAF_ReportView_ID =" + sVAF_ReportView_ID);

                //Check for report view column
                IDataReader dr = DataBase.DB.ExecuteReader("SELECT VAF_ReportView_Col_ID,VAF_Column_ID,FunctionColumn FROM VAF_ReportView_Col WHERE VAF_ReportView_ID = " + sReportView.GetVAF_ReportView_ID());

                while (dr.Read())
                {
                    int sVAF_ReportView_Col_ID = Utility.Util.GetValueOfInt(dr[0]);
                    if (Utility.Util.GetValueOfInt(dr[1]) != 0)
                    {
                        GetColumn(Utility.Util.GetValueOfInt(dr[1]), true, false);
                    }
                    InsertIntoDBSchema(X_VAF_ReportView_Col.Table_ID, sVAF_ReportView_Col_ID, X_VAF_ReportView_Col.Table_Name, dr[2].ToString(), "VAF_ReportView_Col_ID =" + sVAF_ReportView_Col_ID);
                }
                dr.Close();
            }
        }

        private void GetPrintFormat(int sVAF_Print_Rpt_Layout_ID)
        {
            GetPrintFormat(sVAF_Print_Rpt_Layout_ID, true);
        }

        private void GetPrintFormat(int sVAF_Print_Rpt_Layout_ID, bool checkchild)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Print_Rpt_Layout", "VAF_Print_Rpt_Layout_ID=" + sVAF_Print_Rpt_Layout_ID, out name))
            {
                X_VAF_Print_Rpt_Layout sPF = new X_VAF_Print_Rpt_Layout(_ctx, sVAF_Print_Rpt_Layout_ID, null);

                if (sPF.GetVAF_ReportView_ID() != 0)
                {
                    GetReportView(sPF.GetVAF_ReportView_ID());
                }

                if (sPF.GetVAF_Print_Rpt_Paper_ID() != 0)
                {
                    GetPrintPaper(sPF.GetVAF_Print_Rpt_Paper_ID());
                }

                if (sPF.GetVAF_Print_Rpt_Colour_ID() != 0)
                {
                    GetPrintColor(sPF.GetVAF_Print_Rpt_Colour_ID());
                }

                if (sPF.GetVAF_Print_Rpt_Font_ID() != 0)
                {
                    GetPrintFont(sPF.GetVAF_Print_Rpt_Font_ID());
                }

                if (sPF.GetVAF_Print_Rpt_TblLayout_ID() != 0)
                {
                    GetPrintTableFormat(sPF.GetVAF_Print_Rpt_TblLayout_ID());
                }

                GetTable(sPF.GetVAF_TableView_ID());

                InsertIntoDBSchema(X_VAF_Print_Rpt_Layout.Table_ID, sVAF_Print_Rpt_Layout_ID, X_VAF_Print_Rpt_Layout.Table_Name, name, "VAF_Print_Rpt_Layout_ID =" + sVAF_Print_Rpt_Layout_ID);
                //Insert PrintItem
                if (!checkchild)
                    return;
                DataSet ds = DataBase.DB.ExecuteDataset("SELECT * FROM VAF_Print_Rpt_LItem WHERE VAF_Print_Rpt_Layout_ID = " + sPF.GetVAF_Print_Rpt_Layout_ID());

                X_VAF_Print_Rpt_LItem sItem = null;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sItem = new X_VAF_Print_Rpt_LItem(_ctx, dr, null);

                    if (sItem.GetVAF_Column_ID() != 0)
                    {
                        GetColumn(sItem.GetVAF_Column_ID(), true, false); // print format not inserted , process binded to tab;
                    }
                    if (sItem.GetVAF_Print_Rpt_Colour_ID() != 0)
                    {
                        GetPrintColor(sItem.GetVAF_Print_Rpt_Colour_ID());
                    }
                    if (sItem.GetVAF_Print_Rpt_Font_ID() != 0)
                    {
                        GetPrintFont(sItem.GetVAF_Print_Rpt_Font_ID());
                    }

                    if (sItem.GetPrintFormatType().Equals(X_VAF_Print_Rpt_LItem.PRINTFORMATTYPE_PrintFormat) && sItem.GetVAF_Print_Rpt_LayoutChild_ID() != 0)
                    {
                        GetPrintFormat(sItem.GetVAF_Print_Rpt_LayoutChild_ID(), true);
                    }

                    InsertIntoDBSchema(X_VAF_Print_Rpt_LItem.Table_ID, sItem.Get_ID(), X_VAF_Print_Rpt_LItem.Table_Name, sItem.GetName(), "VAF_Print_Rpt_LItem_ID= " + sItem.Get_ID());
                }
            }
        }

        private void GetPrintTableFormat(int sVAF_Print_Rpt_TblLayout_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Print_Rpt_TblLayout", "VAF_Print_Rpt_TblLayout_ID =" + sVAF_Print_Rpt_TblLayout_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Print_Rpt_TblLayout.Table_ID, sVAF_Print_Rpt_TblLayout_ID, X_VAF_Print_Rpt_TblLayout.Table_Name, name, "VAF_Print_Rpt_TblLayout_ID =" + sVAF_Print_Rpt_TblLayout_ID);
            }
        }

        private void GetPrintFont(int sVAF_Print_Rpt_Font_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Print_Rpt_Font", "VAF_Print_Rpt_Font_ID =" + sVAF_Print_Rpt_Font_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Print_Rpt_Font.Table_ID, sVAF_Print_Rpt_Font_ID, X_VAF_Print_Rpt_Font.Table_Name, name, "VAF_Print_Rpt_Font_ID =" + sVAF_Print_Rpt_Font_ID);
            }
        }

        private void GetPrintColor(int sVAF_Print_Rpt_Colour_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Print_Rpt_Colour", "VAF_Print_Rpt_Colour_ID =" + sVAF_Print_Rpt_Colour_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Print_Rpt_Colour.Table_ID, sVAF_Print_Rpt_Colour_ID, X_VAF_Print_Rpt_Colour.Table_Name, name, "VAF_Print_Rpt_Colour_ID =" + sVAF_Print_Rpt_Colour_ID);
            }
        }

        private void GetPrintPaper(int sVAF_Print_Rpt_Paper_ID)
        {
            string name;
            if (HasModulePrefix("Name", "VAF_Print_Rpt_Paper", "VAF_Print_Rpt_Paper_ID =" + sVAF_Print_Rpt_Paper_ID, out name))
            {
                InsertIntoDBSchema(X_VAF_Print_Rpt_Paper.Table_ID, sVAF_Print_Rpt_Paper_ID, X_VAF_Print_Rpt_Paper.Table_Name, name, "VAF_Print_Rpt_Paper_ID =" + sVAF_Print_Rpt_Paper_ID);
            }
        }



        #endregion

        #region Workflow

        private void GetWorkflow(int sAD_Workflow_ID)
        {
            //if (IsRecordExistInDBSchema(X_AD_Workflow.Table_ID, sAD_Workflow_ID))
            //{
            //    return;
            //}

            string name;
            if (HasModulePrefix("Name", "AD_Workflow", "AD_Workflow_ID=" + sAD_Workflow_ID, out name))
            {
                X_AD_Workflow sWFlow = new X_AD_Workflow(_ctx, sAD_Workflow_ID, null);

                if (sWFlow.GetAD_WF_Responsible_ID() != 0)
                {
                    GetWorkflowResponsible(sWFlow.GetAD_WF_Responsible_ID());
                }
                if (sWFlow.GetAD_WorkflowProcessor_ID() != 0)
                {
                    GetWorkflowProcessor(sWFlow.GetAD_WorkflowProcessor_ID());
                }

                if (sWFlow.GetVAF_TableView_ID() != 0)
                {
                    GetTable(sWFlow.GetVAF_TableView_ID());
                }

                if (sWFlow.GetAD_WF_Node_ID() != 0)
                {
                    //GetWFNode(sWFlow.GetAD_WF_Node_ID());
                }

                InsertIntoDBSchema(X_AD_Workflow.Table_ID, sAD_Workflow_ID, X_AD_Workflow.Table_Name, name, "AD_Workflow_ID=" + sAD_Workflow_ID);

                //Check Workflow Nodes

                DataSet ds = DataBase.DB.ExecuteDataset("Select * From AD_WF_Node Where AD_WorkFlow_ID = " + sAD_Workflow_ID);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    X_AD_WF_Node sNode = new X_AD_WF_Node(_ctx, dr, null);

                    if (sNode.GetAD_WF_Responsible_ID() != 0)
                    {
                        GetWorkflowResponsible(sNode.GetAD_WF_Responsible_ID());
                    }
                    if (sNode.GetVAF_Image_ID() != 0)
                    {
                        CheckImage(sNode.GetVAF_Image_ID());
                    }

                    if (sNode.GetR_MailText_ID() != 0)
                    {
                        if (HasModulePrefix("Name", "R_MailText", "R_MailText_ID =" + sNode.GetR_MailText_ID(), out name))
                        {
                            InsertIntoDBSchema(X_R_MailText.Table_ID, sNode.GetR_MailText_ID(), X_R_MailText.Table_Name, name, "R_MailText_ID =" + sNode.GetR_MailText_ID());
                        }
                    }

                    if (sNode.GetAction() == X_AD_WF_Node.ACTION_UserChoice || sNode.GetAction() == X_AD_WF_Node.ACTION_SetVariable)
                    {
                        if (sNode.GetVAF_Column_ID() != 0)
                        {
                            GetColumn(sNode.GetVAF_Column_ID(), false);
                        }
                    }
                    //else if (sNode.GetAction() == X_AD_WF_Node.ACTION_EMail)
                    //{
                    //    if (sNode.GetR_MailText_ID() != 0)
                    //    {
                    //        if (HasModulePrefix("Name", "R_MailText", "R_MailText_ID =" + sNode.GetR_MailText_ID(), out name))
                    //        {
                    //            InsertIntoDBSchema(X_R_MailText.Table_ID, sNode.GetR_MailText_ID(), X_R_MailText.Table_Name, name, "R_MailText_ID =" + sNode.GetR_MailText_ID());
                    //        }
                    //    }
                    //}

                    else if (sNode.GetAction() == X_AD_WF_Node.ACTION_UserForm)
                    {
                        if (sNode.GetVAF_Page_ID() != 0)
                        {
                            GetForm(sNode.GetVAF_Page_ID());
                        }
                    }
                    else if (sNode.GetAction() == X_AD_WF_Node.ACTION_AppsTask)
                    {
                        if (sNode.GetAD_Task_ID() != 0)
                        {
                            GetTask(sNode.GetAD_Task_ID());
                        }
                    }

                    else if (sNode.GetAction() == X_AD_WF_Node.ACTION_AppsReport || sNode.GetAction() == X_AD_WF_Node.ACTION_AppsProcess)
                    {
                        if (sNode.GetVAF_Job_ID() != 0)
                        {
                            GetProcess(sNode.GetVAF_Job_ID());
                        }
                    }

                    else if (sNode.GetAction() == X_AD_WF_Node.ACTION_SubWorkflow)
                    {
                        if (sNode.GetAD_Workflow_ID() != 0)
                        {
                            GetWorkflow(sNode.GetAD_Workflow_ID());
                        }
                    }

                    else if (sNode.GetAction() == X_AD_WF_Node.ACTION_UserWindow)
                    {
                        if (sNode.GetAD_Window_ID() != 0)
                        {
                            GetWindow(sNode.GetAD_Window_ID(), 0);
                        }
                    }
                    InsertIntoDBSchema(X_AD_WF_Node.Table_ID, sNode.GetAD_WF_Node_ID(), X_AD_WF_Node.Table_Name, name, "AD_WF_Node_ID=" + sNode.GetAD_WF_Node_ID());
                }

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    GetNodePara(Convert.ToInt32(dr["AD_WF_Node_ID"]));
                    GetNodeTransition(Convert.ToInt32(dr["AD_WF_Node_ID"]));
                }
            }
        }



        private void GetTask(int sAD_Task_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_Task", "AD_Task_ID =" + sAD_Task_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Task.Table_ID, sAD_Task_ID, X_AD_Task.Table_Name, name, "AD_Task_ID =" + sAD_Task_ID);
            }
        }

        private void GetWorkflowResponsible(int sAD_WF_Responsible_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_WF_Responsible", "AD_WF_Responsible_ID =" + sAD_WF_Responsible_ID, out name))
            {
                InsertIntoDBSchema(X_AD_WF_Responsible.Table_ID, sAD_WF_Responsible_ID, X_AD_WF_Responsible.Table_Name, name, "AD_WF_Responsible_ID =" + sAD_WF_Responsible_ID);
            }
        }

        private void GetWorkflowProcessor(int sAD_WorkflowProcessor_ID)
        {
            //Select AD_WorkflowProcessor_ID From AD_WorkflowProcessor Where Name = '" + sWFProcessor.GetName() + "'"));
            string name;
            if (HasModulePrefix("Name", "AD_WorkflowProcessor", "AD_WorkflowProcessor_ID =" + sAD_WorkflowProcessor_ID, out name))
            {

                int sAD_Scheduler_ID = GetID("AD_WorkflowProcessor", "AD_Schedule_ID", "AD_WorkflowProcessor_ID =" + sAD_WorkflowProcessor_ID);
                if (sAD_Scheduler_ID != 0)
                {
                    //Select AD_Schedule_ID Form AD_Schedule Where Name = '" + sch.GetName() + "'"));
                    if (HasModulePrefix("Name", "AD_Schedule", "AD_Schedule_ID=" + sAD_Scheduler_ID, out name))
                    {
                        InsertIntoDBSchema(X_AD_Schedule.Table_ID, sAD_Scheduler_ID, X_AD_Schedule.Table_Name, name, "AD_Schedule_ID=" + sAD_Scheduler_ID);
                    }
                }
                InsertIntoDBSchema(X_AD_WorkflowProcessor.Table_ID, sAD_WorkflowProcessor_ID, X_AD_WorkflowProcessor.Table_Name, name, "AD_WorkflowProcessor_ID =" + sAD_WorkflowProcessor_ID);
            }
        }

        private void GetNodeTransition(int sAD_WF_Node_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT AD_WF_NodeNext_ID,Description FROM AD_WF_NodeNext WHERE AD_WF_Node_ID = " + sAD_WF_Node_ID);
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                int sAD_WF_NodeNext_ID = Util.GetValueOfInt(dr[0]);

                if (IsRecordExistInDBSchema(X_AD_WF_NodeNext.Table_ID, sAD_WF_NodeNext_ID))
                    continue;

                InsertIntoDBSchema(X_AD_WF_NodeNext.Table_ID, sAD_WF_NodeNext_ID, X_AD_WF_NodeNext.Table_Name, dr[1].ToString(), "AD_WF_NodeNext_ID = " + sAD_WF_NodeNext_ID);

                DataSet ds1 = DB.ExecuteDataset("SELECT AD_WF_NextCondition_ID,Value FROM AD_WF_NextCondition WHERE AD_WF_NodeNext_ID = " + sAD_WF_NodeNext_ID);

                foreach (DataRow dr1 in ds1.Tables[0].Rows)
                {
                    int sAD_WF_NextCondition_ID = Util.GetValueOfInt(dr1[0]);

                    X_AD_WF_NextCondition next = new X_AD_WF_NextCondition(_ctx, sAD_WF_NextCondition_ID, null);
                    if (next.GetVAF_Column_ID() > 0)
                    {
                        if (IsRecordExistInDBSchema(X_VAF_Column.Table_ID, next.GetVAF_Column_ID()))
                        {
                            ;
                        }
                        else
                        {
                            GetColumn(next.GetVAF_Column_ID(), true, false);
                        }
                    }
                    InsertIntoDBSchema(X_AD_WF_NextCondition.Table_ID, sAD_WF_NextCondition_ID, X_AD_WF_NextCondition.Table_Name, dr[1].ToString(), "AD_WF_NextCondition_ID = " + sAD_WF_NextCondition_ID);
                }
                //dr1.Close();
            }
            //dr.Close();
        }

        private void GetNodePara(int sAD_WF_Node_ID)
        {
            //IDataReader dr = DB.ExecuteReader("SELECT AD_WF_Node_Para_ID,AttributeName FROM AD_WF_Node_Para WHERE AD_WF_Node_Para_ID = " + sAD_WF_Node_ID);

            //fixed - find parameter against workflow node
            IDataReader dr = DB.ExecuteReader("SELECT AD_WF_Node_Para_ID, AttributeName, VAF_Job_Para_ID FROM AD_WF_Node_Para WHERE AD_WF_Node_ID = " + sAD_WF_Node_ID);
            while (dr.Read())
            {
                int AD_WF_Node_Para_ID = Util.GetValueOfInt(dr[0]);
                int VAF_Job_Para_ID = Util.GetValueOfInt(dr[2]);
                if (VAF_Job_Para_ID > 0)
                {

                    X_VAF_Job_Para para = new X_VAF_Job_Para(GetCtx(), VAF_Job_Para_ID, null);
                    //CheckCtxArea For Process ID
                    if (!IsRecordExistInDBSchema(X_VAF_Job.Table_ID, para.GetVAF_Job_ID()))
                    {
                        GetProcess(para.GetVAF_Job_ID());
                    }
                }

                InsertIntoDBSchema(X_AD_WF_Node_Para.Table_ID, AD_WF_Node_Para_ID, X_AD_WF_Node_Para.Table_Name, dr[1].ToString(), "AD_WF_Node_Para_ID = " + AD_WF_Node_Para_ID);
            }
            dr.Close();
        }

        #endregion
    }
}
