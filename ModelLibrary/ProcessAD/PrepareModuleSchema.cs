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
        int AD_ModuleInfo_ID = 0;

        const String MODULE_DB_SCHEMA = "AD_Module_DB_Schema";
        private List<String> prefixList = new List<string>();
        List<String> usedPrefixesList = new List<string>();

        // check for process execution
        public static bool running = false;
        // lock for simultaneous process
        private object _lock = new object();

        protected override void Prepare()
        {
            _ctx = GetCtx();
            AD_ModuleInfo_ID = GetRecord_ID();
            //prefix = DataBase.DB.ExecuteScalar("SELECT Prefix FROM AD_ModuleInfo WHERE AD_ModuleInfo_ID =" + AD_ModuleInfo_ID).ToString();
            IDataReader dr = DB.ExecuteReader("SELECT Prefix,AD_ModuleInfo_ID FROM AD_ModuleInfo");
            while (dr.Read())
            {
                string prefix = dr[0].ToString();
                if (string.IsNullOrEmpty(prefix.Trim()))
                {
                    continue;
                }
                prefixList.Add(prefix);
                if (AD_ModuleInfo_ID == Convert.ToInt32(dr[1]))
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

                if (!DeleteOldSchema(AD_ModuleInfo_ID))
                    return "could not delete old schema";

                running = true;
            }

            try
            {
                GenerateSchema(AD_ModuleInfo_ID);
            }
            finally
            {
                running = false;
            }

            return "Schema Generated";
            //throw new NotImplementedException();
        }

        private bool DeleteOldSchema(int AD_ModuleInfo_ID)
        {
            int count = DB.ExecuteQuery("DELETE FROM AD_Module_DB_Schema WHERE AD_ModuleInfo_ID = " + AD_ModuleInfo_ID, null, Get_TrxName());
            if (count < 0)
            {
                return false;
            }
            return true;
        }



        #region Function

        private bool IsRecordExistInDBSchema(int AD_Table_ID, int Record_ID)
        {
            int count = Convert.ToInt32(DB.ExecuteScalar("SELECT Count(*) FROM " + MODULE_DB_SCHEMA + " WHERE AD_Table_ID =" + AD_Table_ID
                                          + " AND Record_ID = " + Record_ID + " AND AD_ModuleInfo_ID = " + AD_ModuleInfo_ID, null, Get_TrxName()));
            if (count > 0)
            {
                return true;
            }
            return false;
        }

        private bool InsertIntoDBSchema(int AD_Table_ID, int Record_ID, string TableName, String RName, string whereClause)
        {
            //Check Existence

            int count = Convert.ToInt32(DB.ExecuteScalar("SELECT Count(*) FROM " + MODULE_DB_SCHEMA + " WHERE AD_Table_ID =" + AD_Table_ID
                                          + " AND Record_ID = " + Record_ID + " AND AD_ModuleInfo_ID = " + AD_ModuleInfo_ID, null, Get_TrxName()));

            if (count == 0)
            {
                MModuleDBSchema schema = new MModuleDBSchema(_ctx, 0, Get_TrxName());
                schema.SetAD_Table_ID(AD_Table_ID);
                schema.SetRecord_ID(Record_ID);
                schema.SetName(RName);
                schema.SetTableName(TableName);
                schema.SetIsActive(true);
                schema.SetAD_ModuleInfo_ID(AD_ModuleInfo_ID);

                try
                {
                    if (schema.Save())
                    {
                        // Check For Export ID
                        object Export_ID = DB.ExecuteScalar("SELECT Export_ID FROM " + TableName + " WHERE " + whereClause);
                        if (Export_ID == null || Export_ID.ToString() == "")
                        {
                            int expID = MSequence.GetNextExportID(GetCtx().GetAD_Client_ID(), TableName, null);
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

        private void GenerateSchema(int AD_ModuleInfo_ID)
        {
            GetModuleInfo(AD_ModuleInfo_ID);
            GetModuleMenus(AD_ModuleInfo_ID);
            GetForms(AD_ModuleInfo_ID);
            GetWindowTabField(AD_ModuleInfo_ID);
            GetProcesses(AD_ModuleInfo_ID);
            GetTables(AD_ModuleInfo_ID);
        }

        /// <summary>
        ///Insert Module Info Folder menu ids in DB Schema
        /// </summary>
        /// <param name="AD_ModuleInfo_ID"></param>
        private void GetModuleMenus(int AD_ModuleInfo_ID)
        {
            List<Int32> ids = GetIDs("AD_ModuleMenuFolder", "AD_ModuleMenuFolder_ID", " AD_ModuleInfo_Id = " + AD_ModuleInfo_ID);
            for (int i = 0; i < ids.Count; i++)
            {
                InsertIntoDBSchema(X_AD_ModuleMenuFolder.Table_ID, ids[i], X_AD_ModuleMenuFolder.Table_Name, "_MenuFolder" + i, "AD_ModuleMenuFolder_ID = " + ids[i]);
            }
        }

        /// <summary>
        /// Insert AD_Module_Info table entry
        /// </summary>
        /// <param name="AD_ModuleInfo_ID"></param>
        private void GetModuleInfo(int AD_ModuleInfo_ID)
        {
            string name;
            HasModulePrefix("Name", "AD_ModuleInfo", "AD_ModuleInfo_ID =" + AD_ModuleInfo_ID, out name);
            // {
            InsertIntoDBSchema(X_AD_ModuleInfo.Table_ID, AD_ModuleInfo_ID, X_AD_ModuleInfo.Table_Name, name, "AD_ModuleInfo_ID =" + AD_ModuleInfo_ID);
            //}
        }

        /// <summary>
        /// Mark Module Process
        /// </summary>
        /// <param name="AD_ModuleInfo_ID">ModuleInfo ID</param>
        private void GetProcesses(int AD_ModuleInfo_ID)
        {
            List<int> lstProcessIds = GetIDs("AD_ModuleProcess", "AD_ModuleProcess_ID", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID);
            for (int p = 0; p < lstProcessIds.Count; p++)
            {
                int id = GetID("AD_ModuleProcess", "AD_Process_ID", "AD_ModuleProcess_ID = " + lstProcessIds[p]);
                GetProcess(id);
                InsertIntoDBSchema(X_AD_ModuleProcess.Table_ID, lstProcessIds[p], X_AD_ModuleProcess.Table_Name, "_ModuleProcess" + p, "AD_ModuleProcess_ID =" + lstProcessIds[p]);
            }
        }


        #region Table

        /// <summary>
        /// Market Tables of Module
        /// </summary>
        /// <param name="AD_ModuleInfo_ID">id of module</param>
        private void GetTables(int AD_ModuleInfo_ID)
        {
            List<int> tableIds = GetIDs("AD_ModuleTable", "AD_ModuleTable_ID", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID);
            //string name;
            for (int t = 0; t < tableIds.Count; t++)
            {
                int AD_Table_ID = GetID("AD_ModuleTable", "AD_Table_ID", "AD_ModuleTable_ID = " + tableIds[t]);
                if (GetTable(AD_Table_ID))
                {
                    List<int> columnIds = GetIDs("AD_Column", "AD_Column_ID", "AD_Table_ID=" + AD_Table_ID);
                    for (int c = 0; c < columnIds.Count; c++)
                    {
                        GetColumn(columnIds[c], true, false);
                    }
                }
                InsertIntoDBSchema(X_AD_ModuleTable.Table_ID, tableIds[t], X_AD_ModuleTable.Table_Name, "ModuleTable_" + t, "AD_ModuleTable_ID=" + tableIds[t]);
            }
        }

        #endregion

        #region Forms

        /// <summary>
        /// Insert Module Forms
        /// </summary>
        /// <param name="AD_ModuleInfo_ID"></param>
        private void GetForms(int AD_ModuleInfo_ID)
        {
            List<int> lstModuleFormIds = GetIDs("AD_ModuleForm", "AD_ModuleForm_ID", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID);

            for (int i = 0; i < lstModuleFormIds.Count; i++)
            {
                int id = GetID("AD_ModuleForm", "AD_Form_ID", "AD_ModuleForm_ID = " + lstModuleFormIds[i]);
                GetForm(id);
                InsertIntoDBSchema(X_AD_ModuleForm.Table_ID, lstModuleFormIds[i], X_AD_ModuleForm.Table_Name, "_ModuleFrom" + i, "AD_ModuleForm_ID =" + lstModuleFormIds[i]);
            }
        }

        /// <summary>
        /// Mark Module Form
        /// </summary>
        /// <param name="sAD_Form_ID">id of form</param>
        void GetForm(int sAD_Form_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_Form", "AD_Form_ID =" + sAD_Form_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Form.Table_ID, sAD_Form_ID, X_AD_Form.Table_Name, name, "AD_Form_ID =" + sAD_Form_ID);

                // 1000175
                DataSet ds = DB.ExecuteDataset("SELECT * FROM AD_CrystalReport_Para WHERE AD_Form_ID = " + sAD_Form_ID);


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    name = "";
                    X_AD_CrystalReport_Para cPara = new X_AD_CrystalReport_Para(_ctx, ds.Tables[0].Rows[i], null);

                    if (cPara.GetAD_Element_ID() > 0)
                    {
                        if (HasModulePrefix("ColumnName", "AD_Element", "AD_Element_ID =" + cPara.GetAD_Element_ID(), out name))
                        {
                            InsertIntoDBSchema(X_AD_Element.Table_ID, cPara.GetAD_Element_ID(), X_AD_Element.Table_Name, name, "AD_Element_ID = " + cPara.GetAD_Element_ID());
                        }
                    }

                    if (cPara.GetAD_Reference_Value_ID() > 0)
                    {
                        GetReference(cPara.GetAD_Reference_Value_ID());//mark reference record [ reference key ]
                    }

                    if (cPara.GetAD_Val_Rule_ID() > 0)
                    {
                        GetValRule(cPara.GetAD_Val_Rule_ID()); //Mark Val_rule recored [ dynamic validation ]
                    }

                    InsertIntoDBSchema(X_AD_CrystalReport_Para.Table_ID, cPara.GetAD_CrystalReport_Para_ID(), X_AD_CrystalReport_Para.Table_Name, cPara.GetColumnName(), "AD_CrystalReport_Para_ID = " + cPara.GetAD_CrystalReport_Para_ID());

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

        private void CheckCtxArea(int sAD_CtxArea_ID)
        {
            if (!IsRecordExistInDBSchema(X_AD_CtxArea.Table_ID, sAD_CtxArea_ID))
            {
                string name;
                if (HasModulePrefix("Name", "AD_CtxArea", "AD_CtxArea_ID = " + sAD_CtxArea_ID, out name))
                {
                    InsertIntoDBSchema(X_AD_CtxArea.Table_ID, sAD_CtxArea_ID, X_AD_CtxArea.Table_Name, name, " AD_CtxArea_ID = " + sAD_CtxArea_ID);
                }
            }
        }

        private void CheckImage(int sAD_Image_ID)
        {
            if (!IsRecordExistInDBSchema(X_AD_Image.Table_ID, sAD_Image_ID))
            {
                string name;
                if (HasModulePrefix("Name", "AD_Image", "AD_Image_ID = " + sAD_Image_ID, out name))
                {
                    InsertIntoDBSchema(X_AD_Image.Table_ID, sAD_Image_ID, X_AD_Image.Table_Name, name, " AD_Image_ID = " + sAD_Image_ID);
                }
            }
        }


        #endregion

        #region Window Tab Field

        private void GetWindowTabField(int AD_ModuleInfo_ID)
        {
            List<int> windowIds = GetIDs("AD_ModuleWindow", " AD_ModuleWindow_ID ", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID);

            for (int i = 0; i < windowIds.Count; i++)
            {
                int sAD_Window_ID = GetID("AD_ModuleWindow", "AD_Window_ID", "AD_ModuleWindow_ID = " + windowIds[i]);
                GetWindow(sAD_Window_ID, windowIds[i]);

                //int Id = GetID("AD_ModuleWindow", "AD_ModuleWindow_ID", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID + " AND  AD_Window_ID =" + sAD_Window_ID);
                //InsertIntoDBSchema(X_AD_ModuleWindow.Table_ID, windowIds[i], X_AD_ModuleWindow.Table_Name, "ModuleWindow" + i, "AD_ModuleWindow_ID =" + windowIds[i]);

            }
        }

        void GetWindow(int sAD_Window_ID, int sAD_ModuleWindow_ID)
        {
            // if (IsRecordExistInDBSchema(X_AD_Window.Table_ID, sAD_Window_ID))
            //  return;
            string name;
            if (HasModulePrefix("Name", "AD_Window", "AD_WINDOW_ID =" + sAD_Window_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Window.Table_ID, sAD_Window_ID, X_AD_Window.Table_Name, name, "AD_WINDOW_ID =" + sAD_Window_ID);
            }


            if (sAD_ModuleWindow_ID > 0)
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

                InsertIntoDBSchema(X_AD_ModuleWindow.Table_ID, sAD_ModuleWindow_ID, X_AD_ModuleWindow.Table_Name, "_ModuleWindow" + sAD_ModuleWindow_ID, "AD_ModuleWindow_ID =" + sAD_ModuleWindow_ID);

                List<int> lstModuleTab = GetIDs("AD_ModuleTab", "AD_ModuleTab_ID", "AD_ModuleWindow_ID = " + sAD_ModuleWindow_ID);//+ " ORDER BY SeqNo");
                GetTabsFields(sAD_Window_ID, lstModuleTab);
            }
        }

        private void GetTabsFields(int sAD_Window_ID, List<int> lstModuleTab)
        {
            //DataTable dt = GetTable("SELECT * FROM AD_Tab WHERE AD_Window_ID=" + sWindowObj.GetAD_Window_ID() + " ORDER BY SeqNo");
            MTab sTab = null;
            int sAD_Tab_ID = 0;
            for (int i = 0; i < lstModuleTab.Count; i++)
            {
                sAD_Tab_ID = GetID("AD_ModuleTab", "AD_Tab_ID", "AD_ModuleTab_ID = " + lstModuleTab[i]);

                bool newTable = false;
                sTab = new MTab(GetCtx(), sAD_Tab_ID, null);
                string name;

                if (HasModulePrefix("TableName", "AD_Table", " AD_Table_ID = " + sTab.GetAD_Table_ID(), out name))
                {

                    MTable sTable = new MTable(_ctx, sTab.GetAD_Table_ID(), null);

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

                    if (sTable.GetReferenced_Table_ID() > 0 && sTable.GetAD_Table_ID() != sTable.GetReferenced_Table_ID())
                    {
                        string name4;
                        if (HasModulePrefix("TableName", "AD_Table", "AD_Table_ID = " + sTable.GetReferenced_Table_ID(), out name4))
                        {
                            InsertIntoDBSchema(X_AD_Table.Table_ID, sTable.GetReferenced_Table_ID(), X_AD_Table.Table_Name, name4, "AD_Table_ID =" + sTable.GetReferenced_Table_ID());
                        }
                    }

                    InsertIntoDBSchema(X_AD_Table.Table_ID, sTab.GetAD_Table_ID(), X_AD_Table.Table_Name, name, "AD_Table_ID = " + sTab.GetAD_Table_ID());
                    InsertDefaultColumn(sTab.GetAD_Table_ID());
                    newTable = true;
                }

                if (sTab.GetAD_Column_ID() != 0)
                {
                    //  if (HasModulePrefix("ColumnName", "AD_Column", "AD_Column_ID = " + sTab.GetAD_Column_ID(), out name))
                    //{
                    GetColumn(sTab.GetAD_Column_ID(), true, false);
                    //InsertIntoDBSchema(X_AD_Column.Table_ID, sTab.GetAD_Column_ID(), X_AD_Column.Table_Name, name, " AD_Column_ID = " + sTab.GetAD_Column_ID());
                    //}
                }

                if (sTab.GetAD_CtxArea_ID() != 0)
                {
                    CheckCtxArea(sTab.GetAD_CtxArea_ID());
                }

                //ImageID
                if (sTab.GetAD_Image_ID() != 0)
                {
                    CheckImage(sTab.GetAD_Image_ID());
                }

                if (sTab.GetReferenced_Tab_ID() != 0)
                {
                    MTab rTab = new MTab(GetCtx(), sTab.GetReferenced_Tab_ID(), null);

                    if (HasModulePrefix("TableName", "AD_Table", " AD_Table_ID = " + rTab.GetAD_Table_ID(), out name))
                    {
                        InsertIntoDBSchema(X_AD_Table.Table_ID, rTab.GetAD_Table_ID(), X_AD_Table.Table_Name, name, "AD_Table_ID = " + rTab.GetAD_Table_ID());
                    }
                    InsertIntoDBSchema(MTab.Table_ID, rTab.GetAD_Tab_ID(), MTab.Table_Name, rTab.GetName(), " AD_Tab_ID = " + rTab.GetAD_Tab_ID());
                }

                if (sTab.GetAD_Process_ID() != 0)
                {
                    GetProcess(sTab.GetAD_Process_ID());
                }

                //New Table HeaderLayout
                if (sTab.GetAD_HeaderLayout_ID() > 0)
                {
                    GetHeaderLayout(sTab.GetAD_HeaderLayout_ID());
                }

                InsertIntoDBSchema(MTab.Table_ID, sAD_Tab_ID, MTab.Table_Name, sTab.GetName(), " AD_Tab_ID = " + sAD_Tab_ID);
                GetTabPanel(sAD_Tab_ID);
                //Insert AD_ModuleTab Info
                //int Id = GetID("AD_ModuleTab", "AD_ModuleTab_ID", "AD_ModuleInfo_ID = " + AD_ModuleInfo_ID + " AND  AD_Window_ID =" + windowIds[i]);
                InsertIntoDBSchema(X_AD_ModuleTab.Table_ID, lstModuleTab[i], X_AD_ModuleTab.Table_Name, "_ModuleTab" + i, "AD_ModuleTab_ID =" + lstModuleTab[i]);
                List<int> lstModuleField = GetIDs("AD_ModuleField", "AD_ModuleField_ID", "AD_ModuleTab_ID = " + lstModuleTab[i], " ORDER BY SeqNo");

                int sAD_Field_ID = 0;
                List<int> fields = new List<int>();
                for (int f = 0; f < lstModuleField.Count; f++)
                {
                    sAD_Field_ID = GetID("AD_ModuleField", "AD_Field_ID", "AD_ModuleField_ID =" + lstModuleField[f]);
                    name = InsertField(newTable, name, sAD_Field_ID);
                    fields.Add(sAD_Field_ID);
                    InsertIntoDBSchema(X_AD_ModuleField.Table_ID, lstModuleField[f], X_AD_ModuleField.Table_Name, "_ModuleField" + i, "AD_ModuleField_ID =" + lstModuleField[f]);
                }

                ///Insert default Field
                ///
                if (lstModuleField.Count > 0) // run only if at least one field is binded in modulefield and new tab created
                {
                    IDataReader dr = DB.ExecuteReader(@" SELECT Name, AD_Field_ID FROM AD_Field WHERE AD_Column_ID IN
                                                        ( SELECT AD_Column_ID FROM AD_Column WHERE ColumnName IN ('IsActive','AD_Client_ID', 'AD_Org_ID','" + MTable.Get(_ctx, sTab.GetAD_Table_ID()).GetTableName() + @"_ID',
                                                                                                                 'Export_ID')
                                                        AND AD_Table_ID = " + sTab.GetAD_Table_ID() + @") AND AD_Tab_ID =" + sTab.GetAD_Tab_ID());
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

                //Insert Card view
                GetCardView(sAD_Window_ID, sAD_Tab_ID);
            }
        }

        /// <summary>
        ///  Mark Tab Panel record if any
        /// </summary>
        /// <param name="sAD_Tab_ID"> Tab ID</param>
        private void GetTabPanel(int sAD_Tab_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT * FROM AD_TabPanel WHERE AD_Tab_ID=" + sAD_Tab_ID);
            if (ds != null)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    InsertIntoDBSchema(X_AD_TabPanel.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_TabPanel_ID"]),
                        X_AD_TabPanel.Table_Name, ds.Tables[0].Rows[i]["Name"].ToString(), "AD_TabPanel_ID =" + ds.Tables[0].Rows[i]["AD_TabPanel_ID"]);
                }
            }
        }


        /// <summary>
        /// Mark card view with tab and window
        /// </summary>
        /// <param name="sAD_Window_ID">window id</param>
        /// <param name="sAD_Tab_ID">tab id</param>
        private void GetCardView(int sAD_Window_ID, int sAD_Tab_ID)
        {
            DataSet ds = DB.ExecuteDataset("SELECT * FROM AD_CardView WHERE AD_Window_ID=" + sAD_Window_ID
                                            + " AND AD_Tab_ID=" + sAD_Tab_ID + " AND AD_Client_ID = 0 AND IsActive='Y'");
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++) //Grid View
            {
                DataRow dr = ds.Tables[0].Rows[0];
                if (Util.GetValueOfInt(dr["AD_HeaderLayout_ID"]) > 0)
                {
                    //insert
                    GetHeaderLayout(Util.GetValueOfInt(dr["AD_HeaderLayout_ID"]));
                }

                int sAD_CardView_ID = Convert.ToInt32(dr["AD_CardView_ID"]);

                string name = "";
                if (HasModulePrefix("Name", "AD_CardView", "AD_CardView_ID=" + sAD_CardView_ID, out name))
                {
                    InsertIntoDBSchema(X_AD_CardView.Table_ID, sAD_CardView_ID, X_AD_CardView.Table_Name, name, "AD_CardView_ID =" + sAD_CardView_ID);
                } //Header

                // Card view column
                List<int> lstCardCol = GetIDs("AD_CardView_Column", "AD_CardView_Column_ID", "AD_CardView_ID=" + sAD_CardView_ID);

                for (int col = 0; col < lstCardCol.Count; col++)
                {
                    if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_ModuleField.AD_Field_ID FROM AD_ModuleField "
                                         +" WHERE AD_ModuleField.AD_Field_ID = (SELECT AD_CardView_Column.AD_Field_ID "
                                         +" FROM AD_CardView_Column WHERE AD_CardView_Column_ID=" + lstCardCol[col] + ")"))>0)
                    // if (HasModulePrefix("Sequence", "AD_CardView_Column", "AD_CardView_Column_ID=" + lstCardCol[col], out name))
                    {
                        InsertIntoDBSchema(X_AD_CardView_Column.Table_ID, lstCardCol[col], X_AD_CardView_Column.Table_Name, name, "AD_CardView_Column_ID=" + lstCardCol[col]);
                    }
                   
                }
                List<int> lstCardCondition = GetIDs("AD_CardView_Condition", "AD_CardView_Condition_ID", "AD_CardView_ID=" + sAD_CardView_ID);
                for (int cnd = 0; cnd < lstCardCondition.Count; cnd++)
                {
                    InsertIntoDBSchema(X_AD_CardView_Condition.Table_ID, lstCardCondition[cnd], X_AD_CardView_Condition.Table_Name, name, "AD_CardView_Condition_ID=" + lstCardCondition[cnd]);
                }

                //mark default card 
                List<int> dcids = GetIDs("AD_DefaultCardView", "AD_DefaultCardView_ID","AD_CardView_ID=" + sAD_CardView_ID);
                for (int crd = 0; crd < dcids.Count; crd++)
                {
                    InsertIntoDBSchema(X_AD_DefaultCardView.Table_ID, dcids[crd], X_AD_DefaultCardView.Table_Name, name, "AD_DefaultCardView_ID =" + dcids[crd]);
                }
            }
        }


        private void GetHeaderLayout(int sAD_HeaderLayout_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_HeaderLayout", "AD_HeaderLayout_ID=" + sAD_HeaderLayout_ID, out name))
            {
                InsertIntoDBSchema(X_AD_HeaderLayout.Table_ID, sAD_HeaderLayout_ID, X_AD_HeaderLayout.Table_Name, name, "AD_HeaderLayout_ID =" + sAD_HeaderLayout_ID);
            } //Header

            DataSet ds = DB.ExecuteDataset("SELECT * FROM AD_GridLayout WHERE AD_HeaderLayout_ID=" + sAD_HeaderLayout_ID);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++) //Grid header
            {
                if (HasModulePrefix("Name", "AD_GridLayout", "AD_GridLayout_ID = " + ds.Tables[0].Rows[i]["AD_GridLayout_ID"], out name))
                {
                    InsertIntoDBSchema(X_AD_GridLayout.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_GridLayout_ID"]),
                        X_AD_GridLayout.Table_Name, name, "AD_GridLayout_ID=" + ds.Tables[0].Rows[i]["AD_GridLayout_ID"]);

                    DataSet dsItem = DB.ExecuteDataset("SELECT * FROM AD_GridLayoutItems WHERE AD_GridLayout_ID=" + ds.Tables[0].Rows[i]["AD_GridLayout_ID"]);
                    for (int j = 0; j < dsItem.Tables[0].Rows.Count; j++) //GridItems
                    {
                        //if (HasModulePrefix("Name", "AD_GridLayoutItems", "AD_GridLayoutItems_ID = " + dsItem.Tables[0].Rows[j]["AD_GridLayoutItems_ID"], out name))
                        // {
                        InsertIntoDBSchema(X_AD_GridLayoutItems.Table_ID, Util.GetValueOfInt(dsItem.Tables[0].Rows[j]["AD_GridLayoutItems_ID"]),
                            X_AD_GridLayoutItems.Table_Name, "item" + j, "AD_GridLayoutItems_ID=" + dsItem.Tables[0].Rows[j]["AD_GridLayoutItems_ID"]);
                        //}
                    }
                }
            }
        }

        private string InsertField(bool newTable, string name, int sAD_Field_ID)
        {
            MField sField = new MField(_ctx, sAD_Field_ID, null);

            GetColumn(sField.GetAD_Column_ID(), true, !newTable);

            if (sField.GetAD_FieldGroup_ID() > 0)
            {
                InsertIntoDBSchema(X_AD_FieldGroup.Table_ID, sField.GetAD_FieldGroup_ID(), X_AD_FieldGroup.Table_Name, "FieldGroup_" + sField.GetAD_FieldGroup_ID(), "AD_FieldGroup_ID = " + sField.GetAD_FieldGroup_ID());
            }

            if (sField.GetZoomWindow_ID() > 0)
            {
                if (HasModulePrefix("Name", "AD_Window", "AD_Window_ID = " + sField.GetZoomWindow_ID(), out name))
                {
                    InsertIntoDBSchema(X_AD_Window.Table_ID, sField.GetZoomWindow_ID(), X_AD_Window.Table_Name, name, " AD_Window_ID = " + sField.GetZoomWindow_ID());
                }
            }
            if (sField.GetAD_InfoWindow_ID() > 0)
            {
                if (HasModulePrefix("Name", "AD_InfoWindow", "AD_InfoWindow_ID = " + sField.GetAD_InfoWindow_ID(), out name))
                {
                    InsertIntoDBSchema(X_AD_InfoWindow.Table_ID, sField.GetAD_InfoWindow_ID(), X_AD_InfoWindow.Table_Name, name, " AD_InfoWindow_ID = " + sField.GetAD_InfoWindow_ID());
                }

                DataSet ds = DB.ExecuteDataset("SELECT * FROM AD_InfoColumn WHERE AD_InfoWindow_ID=" + sField.GetAD_InfoWindow_ID());
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    if (HasModulePrefix("Name", "AD_InfoColumn", "AD_InfoColumn_ID = " + ds.Tables[0].Rows[i]["AD_InfoColumn_ID"], out name))
                    {

                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_REFERENCE_VALUE_ID"]) > 0)
                        {
                            InsertIntoDBSchema(X_AD_Reference.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_REFERENCE_VALUE_ID"]), X_AD_Reference.Table_Name, name,
                           " AD_Reference_ID = " + ds.Tables[0].Rows[i]["AD_REFERENCE_VALUE_ID"]);
                        }
                        if (Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_ELEMENT_ID"]) > 0)
                        {
                            InsertIntoDBSchema(X_AD_Element.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_ELEMENT_ID"]), X_AD_Element.Table_Name, name,
                           " AD_Element_ID = " + ds.Tables[0].Rows[i]["AD_ELEMENT_ID"]);
                        }

                        InsertIntoDBSchema(X_AD_InfoColumn.Table_ID, Util.GetValueOfInt(ds.Tables[0].Rows[i]["AD_InfoColumn_ID"]), X_AD_InfoColumn.Table_Name, name,
                            " AD_InfoColumn_ID = " + ds.Tables[0].Rows[i]["AD_InfoColumn_ID"]);
                    }
                }
            }

            if (sField.GetAD_Image_ID() > 0)
            {
                CheckImage(sField.GetAD_Image_ID());
            }

            InsertIntoDBSchema(X_AD_Field.Table_ID, sAD_Field_ID, X_AD_Field.Table_Name, sField.GetName(), " AD_Field_ID = " + sAD_Field_ID);
            return name;
        }

        /// <summary>
        /// If New Table bound to Tab
        /// </summary>
        /// <param name="sAD_Table_ID"></param>
        private void InsertDefaultColumn(int sAD_Table_ID)
        {
            string tableName = DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + sAD_Table_ID).ToString();

            string stdWhere = " ColumnName IN ('IsActive','AD_Client_ID', 'AD_Org_ID','Created','CreatedBy','Updated','UpdatedBy','"
                              + tableName + "_ID','Export_ID')";

            IDataReader dr = DB.ExecuteReader("SELECT AD_Column_ID,Name,ColumnName,AD_Val_Rule_ID FROM AD_Column WHERE AD_Table_ID = " + sAD_Table_ID + " AND  " + stdWhere);
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
                if (Util.GetValueOfInt(dr["AD_Val_Rule_ID"]) != 0)
                {
                    GetValRule(Util.GetValueOfInt(dr["AD_Val_Rule_ID"]));
                }
                InsertIntoDBSchema(X_AD_Column.Table_ID, id, X_AD_Column.Table_Name, Util.GetValueOfString(dr[1]), " AD_Column_ID = " + id);
            }
            dr.Close();
        }

        private bool GetTable(int sAD_Table_ID)
        {
            string name;
            if (HasModulePrefix("TableName", "AD_Table", "AD_Table_ID=" + sAD_Table_ID, out name))
            {
                MTable sTable = new MTable(_ctx, sAD_Table_ID, null);

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
                    if (HasModulePrefix("TableName", "AD_Table", "AD_Table_ID = " + sTable.GetReferenced_Table_ID(), out name4))
                    {
                        InsertIntoDBSchema(X_AD_Table.Table_ID, sTable.GetReferenced_Table_ID(), X_AD_Table.Table_Name, name4, "AD_Table_ID =" + sTable.GetReferenced_Table_ID());
                    }
                }


                InsertIntoDBSchema(X_AD_Table.Table_ID, sAD_Table_ID, X_AD_Table.Table_Name, name, "AD_Table_ID =" + sAD_Table_ID);
                InsertDefaultColumn(sAD_Table_ID);
                return true;
            }
            return false;
        }

        private void GetColumn(int sAD_Column_ID)
        {
            GetColumn(sAD_Column_ID, false, false);
        }
        private void GetColumn(int sAD_Column_ID, bool checkRP)
        {
            GetColumn(sAD_Column_ID, checkRP, false);
        }
        private void GetColumn(int sAd_Column_ID, bool checkRP, bool checkModulePrefix)
        {
            string name;
            if (HasModulePrefix("ColumnName", "AD_Column", "AD_Column_ID=" + sAd_Column_ID, out name))
            {
                int sAd_Element_ID = GetID("AD_Column", "AD_Element_ID", "AD_Column_ID=" + sAd_Column_ID);

                GetElement(sAd_Element_ID);

                if (checkRP)
                {
                    CheckReferenceAndProcess(sAd_Column_ID);
                }
                InsertIntoDBSchema(X_AD_Column.Table_ID, sAd_Column_ID, X_AD_Column.Table_Name, name, "AD_Column_ID =" + sAd_Column_ID);
            }
            else if (!checkModulePrefix)
            {
                if (checkRP)
                {
                    CheckReferenceAndProcess(sAd_Column_ID);
                }
                InsertIntoDBSchema(X_AD_Column.Table_ID, sAd_Column_ID, X_AD_Column.Table_Name, name, "AD_Column_ID =" + sAd_Column_ID);
            }
        }

        private void GetElement(int sAd_Element_ID)
        {
            string name;
            if (HasModulePrefix("ColumnName", "AD_Element", "AD_Element_ID=" + sAd_Element_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Element.Table_ID, sAd_Element_ID, X_AD_Element.Table_Name, name, "AD_Element_ID =" + sAd_Element_ID);
            }
        }

        private void CheckReferenceAndProcess(int sAd_Column_id)
        {
            MColumn sColumn = new MColumn(_ctx, sAd_Column_id, null);
            if (DisplayType.IsLookup(sColumn.GetAD_Reference_ID()) || DisplayType.Button == sColumn.GetAD_Reference_ID())
            {

                if (DisplayType.TableDir != sColumn.GetAD_Reference_ID())
                {
                    if (sColumn.GetAD_Reference_Value_ID() != 0)
                    {
                        GetReference(sColumn.GetAD_Reference_Value_ID());

                    }
                }

                if (sColumn.GetAD_Val_Rule_ID() != 0)
                {
                    GetValRule(sColumn.GetAD_Val_Rule_ID());
                }

                if (sColumn.GetAD_Process_ID() != 0)
                {
                    GetProcess(sColumn.GetAD_Process_ID());
                }

                if (sColumn.GetAD_Form_ID() > 0)
                {
                    GetForm(sColumn.GetAD_Form_ID());
                }

            }
        }

        private void GetValRule(int sAD_Val_Rule_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_Val_Rule", "AD_Val_Rule_ID =" + sAD_Val_Rule_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Val_Rule.Table_ID, sAD_Val_Rule_ID, X_AD_Val_Rule.Table_Name, name, "AD_Val_Rule_ID =" + sAD_Val_Rule_ID);
            }
        }

        private void GetReference(int sAD_Reference_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_Reference", "AD_Reference_ID =" + sAD_Reference_ID, out name))
            {
                InsertIntoDBSchema(X_AD_Reference.Table_ID, sAD_Reference_ID, X_AD_Reference.Table_Name, name, "AD_Reference_ID =" + sAD_Reference_ID);

                MReference sReference = new MReference(_ctx, sAD_Reference_ID, null);

                if (sReference.GetValidationType() == X_AD_Reference.VALIDATIONTYPE_ListValidation)
                {
                    IDataReader dr = DataBase.DB.ExecuteReader("Select AD_Ref_List_ID,Name,AD_Image_ID From AD_Ref_List Where AD_Reference_ID =" + sReference.GetAD_Reference_ID());
                    while (dr.Read())
                    {
                        int ad_image_ID = Util.GetValueOfInt(dr[2]);
                        if (ad_image_ID > 0)
                        {
                            CheckImage(ad_image_ID); 
                        }
                        InsertIntoDBSchema(X_AD_Ref_List.Table_ID, Convert.ToInt32(dr[0]), X_AD_Ref_List.Table_Name, dr[1].ToString(), "AD_Ref_List_ID = " + Convert.ToInt32(dr[0]));
                    }
                    dr.Close();
                }
                else if (sReference.GetValidationType() == X_AD_Reference.VALIDATIONTYPE_TableValidation)
                {
                    MRefTable sRefTable = null;

                    DataSet ds = DataBase.DB.ExecuteDataset("Select * From AD_Ref_Table Where AD_Reference_ID =" + sReference.GetAD_Reference_ID());

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        sRefTable = new MRefTable(_ctx, dr, Get_TrxName());

                        GetTable(sRefTable.GetAD_Table_ID());
                        GetColumn(sRefTable.GetColumn_Key_ID());
                        GetColumn(sRefTable.GetColumn_Display_ID());
                        InsertIntoDBSchema(X_AD_Ref_Table.Table_ID, sAD_Reference_ID, X_AD_Ref_Table.Table_Name, sRefTable.GetDisplayColumnName(), "AD_Reference_ID = " + sReference.GetAD_Reference_ID());
                    }
                }
            }
        }

        private void GetProcess(int sAD_Process_ID)
        {
            //if (IsRecordExistInDBSchema(X_AD_Process.Table_ID, sAD_Process_ID))
            //    return;

            string name;
            if (HasModulePrefix("Name", "AD_Process", "AD_Process_ID=" + sAD_Process_ID, out name))
            {
                MProcess sProcess = new MProcess(_ctx, sAD_Process_ID, null);

                if (sProcess.GetAD_CtxArea_ID() != 0)
                {
                    CheckCtxArea(sProcess.GetAD_CtxArea_ID());
                }

                if (sProcess.GetAD_Workflow_ID() != 0)
                {
                    GetWorkflow(sProcess.GetAD_Workflow_ID());
                }

                if (sProcess.IsReport())
                {
                    if (sProcess.GetAD_ReportView_ID() != 0)
                    {
                        GetReportView(sProcess.GetAD_ReportView_ID());
                    }

                    if (sProcess.GetAD_PrintFormat_ID() != 0)
                    {
                        //Set Print Format
                        GetPrintFormat(sProcess.GetAD_PrintFormat_ID());
                    }

                    if (sProcess.GetAD_ReportFormat_ID() > 0)
                    {
                        GetReportFormat(sProcess.GetAD_ReportFormat_ID());
                    }
                }

                InsertIntoDBSchema(X_AD_Process.Table_ID, sAD_Process_ID, X_AD_Process.Table_Name, name, "AD_Process_ID =" + sAD_Process_ID);

                //ProcessParameter
                DataSet ds = DataBase.DB.ExecuteDataset("Select * From AD_Process_Para where AD_Process_ID = " + sAD_Process_ID);

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    MProcessPara sPara = new MProcessPara(_ctx, dr, Get_TrxName());
                    if (sPara.GetAD_Element_ID() != 0)
                    {
                        GetElement(sPara.GetAD_Element_ID());
                    }

                    if (sPara.GetAD_Reference_Value_ID() != 0)
                    {
                        GetReference(sPara.GetAD_Reference_Value_ID());
                    }
                    if (sPara.GetAD_Val_Rule_ID() != 0)
                    {
                        GetValRule(sPara.GetAD_Val_Rule_ID());
                    }
                    InsertIntoDBSchema(X_AD_Process_Para.Table_ID, sPara.GetAD_Process_Para_ID(), X_AD_Process_Para.Table_Name, sPara.GetName(), "AD_Process_Para_ID=" + sPara.GetAD_Process_Para_ID());
                }
            }
        }

        /// <summary>
        /// New Report Marking(22/8/13)
        /// </summary>
        /// <param name="p"></param>
        private void GetReportFormat(int sAD_ReportFormat_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_ReportFormat", "AD_ReportFormat_ID=" + sAD_ReportFormat_ID, out name))
            {
                X_AD_ReportFormat sReportFromat = new X_AD_ReportFormat(_ctx, sAD_ReportFormat_ID, null);
                InsertIntoDBSchema(X_AD_ReportFormat.Table_ID, sAD_ReportFormat_ID, X_AD_ReportFormat.Table_Name, name, "AD_ReportFormat_ID=" + sAD_ReportFormat_ID);

                //Get Report Tables
                DataSet dsRTable = DB.ExecuteDataset("SELECT * FROM AD_ReportTable WHERE AD_ReportFormat_ID =" + sAD_ReportFormat_ID);
                DataSet dsRItems = null;
                foreach (DataRow drTable in dsRTable.Tables[0].Rows)
                {

                    X_AD_ReportTable rTable = new X_AD_ReportTable(_ctx, drTable, null);

                    if (rTable.GetAD_Table_ID() > 0)
                    {
                        GetTable(rTable.GetAD_Table_ID());
                    }

                    if (rTable.GetAD_Table_ID_1() > 0)
                    {
                        GetTable(rTable.GetAD_Table_ID_1());
                    }

                    if (rTable.GetAD_Column_ID() > 0)
                    {
                        GetColumn(rTable.GetAD_Column_ID(), true, false);
                    }

                    InsertIntoDBSchema(X_AD_ReportTable.Table_ID, rTable.GetAD_ReportTable_ID(), X_AD_ReportTable.Table_Name, "ReportTable[" + rTable.GetAD_ReportTable_ID() + "]", "AD_ReportTable_ID = " + rTable.GetAD_ReportTable_ID());

                    //Get Report Items
                    dsRItems = DB.ExecuteDataset("SELECT * FROM AD_REPORTITEM WHERE AD_ReportTable_ID = " + rTable.GetAD_ReportTable_ID());
                    foreach (DataRow drItem in dsRItems.Tables[0].Rows)
                    {
                        X_AD_ReportItem rItem = new X_AD_ReportItem(_ctx, drItem, null);

                        if (rItem.GetAD_Column_ID() > 0)
                        {
                            GetColumn(rItem.GetAD_Column_ID(), true, false);
                        }
                        InsertIntoDBSchema(X_AD_ReportItem.Table_ID, rItem.GetAD_ReportItem_ID(), X_AD_ReportItem.Table_Name, "ReportItem[" + rItem.GetAD_ReportItem_ID() + "]", "AD_ReportItem_ID=" + rItem.GetAD_ReportItem_ID());
                    }
                    //end Item loop
                }
                //end table loop
            }
        }

        private void GetReportView(int sAD_Reportview_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_ReportView", "AD_ReportView_ID=" + sAD_Reportview_ID, out name))
            {
                X_AD_ReportView sReportView = new X_AD_ReportView(_ctx, sAD_Reportview_ID, null);

                int rAD_Table_ID = GetID("AD_ReportView", "AD_Table_ID", "AD_ReportView_ID=" + sAD_Reportview_ID);

                GetTable(rAD_Table_ID);

                InsertIntoDBSchema(X_AD_ReportView.Table_ID, sAD_Reportview_ID, X_AD_ReportView.Table_Name, name, "AD_ReportView_ID =" + sAD_Reportview_ID);

                //Check for report view column
                IDataReader dr = DataBase.DB.ExecuteReader("SELECT AD_ReportView_Col_ID,AD_Column_ID,FunctionColumn FROM AD_ReportView_Col WHERE AD_ReportView_ID = " + sReportView.GetAD_ReportView_ID());

                while (dr.Read())
                {
                    int sAD_ReportView_Col_ID = Utility.Util.GetValueOfInt(dr[0]);
                    if (Utility.Util.GetValueOfInt(dr[1]) != 0)
                    {
                        GetColumn(Utility.Util.GetValueOfInt(dr[1]), true, false);
                    }
                    InsertIntoDBSchema(X_AD_ReportView_Col.Table_ID, sAD_ReportView_Col_ID, X_AD_ReportView_Col.Table_Name, dr[2].ToString(), "AD_ReportView_Col_ID =" + sAD_ReportView_Col_ID);
                }
                dr.Close();
            }
        }

        private void GetPrintFormat(int sAD_PrintFormat_ID)
        {
            GetPrintFormat(sAD_PrintFormat_ID, true);
        }

        private void GetPrintFormat(int sAD_PrintFormat_ID, bool checkchild)
        {
            string name;
            if (HasModulePrefix("Name", "AD_PrintFormat", "AD_PrintFormat_ID=" + sAD_PrintFormat_ID, out name))
            {
                X_AD_PrintFormat sPF = new X_AD_PrintFormat(_ctx, sAD_PrintFormat_ID, null);

                if (sPF.GetAD_ReportView_ID() != 0)
                {
                    GetReportView(sPF.GetAD_ReportView_ID());
                }

                if (sPF.GetAD_PrintPaper_ID() != 0)
                {
                    GetPrintPaper(sPF.GetAD_PrintPaper_ID());
                }

                if (sPF.GetAD_PrintColor_ID() != 0)
                {
                    GetPrintColor(sPF.GetAD_PrintColor_ID());
                }

                if (sPF.GetAD_PrintFont_ID() != 0)
                {
                    GetPrintFont(sPF.GetAD_PrintFont_ID());
                }

                if (sPF.GetAD_PrintTableFormat_ID() != 0)
                {
                    GetPrintTableFormat(sPF.GetAD_PrintTableFormat_ID());
                }

                GetTable(sPF.GetAD_Table_ID());

                InsertIntoDBSchema(X_AD_PrintFormat.Table_ID, sAD_PrintFormat_ID, X_AD_PrintFormat.Table_Name, name, "AD_PrintFormat_ID =" + sAD_PrintFormat_ID);
                //Insert PrintItem
                if (!checkchild)
                    return;
                DataSet ds = DataBase.DB.ExecuteDataset("SELECT * FROM AD_PrintFormatItem WHERE AD_PrintFormat_ID = " + sPF.GetAD_PrintFormat_ID());

                X_AD_PrintFormatItem sItem = null;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    sItem = new X_AD_PrintFormatItem(_ctx, dr, null);

                    if (sItem.GetAD_Column_ID() != 0)
                    {
                        GetColumn(sItem.GetAD_Column_ID(), true, false); // print format not inserted , process binded to tab;
                    }
                    if (sItem.GetAD_PrintColor_ID() != 0)
                    {
                        GetPrintColor(sItem.GetAD_PrintColor_ID());
                    }
                    if (sItem.GetAD_PrintFont_ID() != 0)
                    {
                        GetPrintFont(sItem.GetAD_PrintFont_ID());
                    }

                    if (sItem.GetPrintFormatType().Equals(X_AD_PrintFormatItem.PRINTFORMATTYPE_PrintFormat) && sItem.GetAD_PrintFormatChild_ID() != 0)
                    {
                        GetPrintFormat(sItem.GetAD_PrintFormatChild_ID(), true);
                    }

                    InsertIntoDBSchema(X_AD_PrintFormatItem.Table_ID, sItem.Get_ID(), X_AD_PrintFormatItem.Table_Name, sItem.GetName(), "AD_PrintFormatItem_ID= " + sItem.Get_ID());
                }
            }
        }

        private void GetPrintTableFormat(int sAD_PrintTableFormat_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_PrintTableFormat", "AD_PrintTableFormat_ID =" + sAD_PrintTableFormat_ID, out name))
            {
                InsertIntoDBSchema(X_AD_PrintTableFormat.Table_ID, sAD_PrintTableFormat_ID, X_AD_PrintTableFormat.Table_Name, name, "AD_PrintTableFormat_ID =" + sAD_PrintTableFormat_ID);
            }
        }

        private void GetPrintFont(int sAD_PrintFont_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_PrintFont", "AD_PrintFont_ID =" + sAD_PrintFont_ID, out name))
            {
                InsertIntoDBSchema(X_AD_PrintFont.Table_ID, sAD_PrintFont_ID, X_AD_PrintFont.Table_Name, name, "AD_PrintFont_ID =" + sAD_PrintFont_ID);
            }
        }

        private void GetPrintColor(int sAD_PrintColor_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_PrintColor", "AD_PrintColor_ID =" + sAD_PrintColor_ID, out name))
            {
                InsertIntoDBSchema(X_AD_PrintColor.Table_ID, sAD_PrintColor_ID, X_AD_PrintColor.Table_Name, name, "AD_PrintColor_ID =" + sAD_PrintColor_ID);
            }
        }

        private void GetPrintPaper(int sAD_PrintPaper_ID)
        {
            string name;
            if (HasModulePrefix("Name", "AD_PrintPaper", "AD_PrintPaper_ID =" + sAD_PrintPaper_ID, out name))
            {
                InsertIntoDBSchema(X_AD_PrintPaper.Table_ID, sAD_PrintPaper_ID, X_AD_PrintPaper.Table_Name, name, "AD_PrintPaper_ID =" + sAD_PrintPaper_ID);
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

                if (sWFlow.GetAD_Table_ID() != 0)
                {
                    GetTable(sWFlow.GetAD_Table_ID());
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
                    if (sNode.GetAD_Image_ID() != 0)
                    {
                        CheckImage(sNode.GetAD_Image_ID());
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
                        if (sNode.GetAD_Column_ID() != 0)
                        {
                            GetColumn(sNode.GetAD_Column_ID(), false);
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
                        if (sNode.GetAD_Form_ID() != 0)
                        {
                            GetForm(sNode.GetAD_Form_ID());
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
                        if (sNode.GetAD_Process_ID() != 0)
                        {
                            GetProcess(sNode.GetAD_Process_ID());
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
                    if (next.GetAD_Column_ID() > 0)
                    {
                        if (IsRecordExistInDBSchema(X_AD_Column.Table_ID, next.GetAD_Column_ID()))
                        {
                            ;
                        }
                        else
                        {
                            GetColumn(next.GetAD_Column_ID(), true, false);
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
            IDataReader dr = DB.ExecuteReader("SELECT AD_WF_Node_Para_ID, AttributeName, AD_Process_Para_ID FROM AD_WF_Node_Para WHERE AD_WF_Node_ID = " + sAD_WF_Node_ID);
            while (dr.Read())
            {
                int AD_WF_Node_Para_ID = Util.GetValueOfInt(dr[0]);
                int AD_Process_Para_ID = Util.GetValueOfInt(dr[2]);
                if (AD_Process_Para_ID > 0)
                {

                    X_AD_Process_Para para = new X_AD_Process_Para(GetCtx(), AD_Process_Para_ID, null);
                    //CheckCtxArea For Process ID
                    if (!IsRecordExistInDBSchema(X_AD_Process.Table_ID, para.GetAD_Process_ID()))
                    {
                        GetProcess(para.GetAD_Process_ID());
                    }
                }

                InsertIntoDBSchema(X_AD_WF_Node_Para.Table_ID, AD_WF_Node_Para_ID, X_AD_WF_Node_Para.Table_Name, dr[1].ToString(), "AD_WF_Node_Para_ID = " + AD_WF_Node_Para_ID);
            }
            dr.Close();
        }

        #endregion
    }
}
