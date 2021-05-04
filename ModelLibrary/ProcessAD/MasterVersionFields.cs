/********************************************************
 * Project Name   : ViennaAdvantage
 * Class Name     : MasterVersionFields
 * Class Used     : SvrProcess
 * Chronological    Development
 * Lokesh Chauhan   15-Oct-2019
  ******************************************************/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VAdvantage.DataBase;
using VAdvantage.Logging;
using VAdvantage.Model;
using VAdvantage.ProcessEngine;
using VAdvantage.Utility;

namespace VAdvantage.Process
{
    public class MasterVersionFields : SvrProcess
    {
        #region Private Variables
        private int _AD_Tab_ID = 0;
        public List<String> listDefVerCols = new List<String> { "VersionValidFrom", "IsVersionApproved", "ProcessedVersion", "RecordVersion", "Processed", "Processing", "VersionLog", "OldVersion" };
        public List<int> listDefVerRef = new List<int> { 15, 20, 20, 11, 20, 20, 14, 11 };
        public List<String> listDefVerValues = new List<String> { "Created", "'Y'", "'Y'", "1", "'Y'", "'N'", "''", "''" };
        private List<int> _listDefVerElements = new List<int>();  //{ 617, 351, 1047, 524, 624 };
        private string className = "VIS.verinfo";
        #endregion Private Variables

        /// <summary>
        /// Process Parameters
        /// </summary>
        protected override void Prepare()
        {
            ProcessInfoParameter[] para = GetParameter();
            for (int i = 0; i < para.Length; i++)
            {
                String name = para[i].GetParameterName();
                if (para[i].GetParameter() == null)
                {
                }
                else if (name.Equals("AD_Tab_ID"))
                    _AD_Tab_ID = Utility.Util.GetValueOfInt((Decimal)para[i].GetParameter());
                else
                    log.Log(Level.SEVERE, "Unknown Parameter: " + name);
            }
        }

        /// <summary>
        /// process logic (to create master version window and Tab)
        /// </summary>
        /// <returns></returns>
        protected override string DoIt()
        {
            if (_AD_Tab_ID <= 0)
                _AD_Tab_ID = GetRecord_ID();

            if (_AD_Tab_ID <= 0)
                return Msg.GetMsg(GetCtx(), "TabNotFound");

            int Ver_AD_Window_ID = 0;
            int Ver_AD_Tab_ID = 0;

            // Created object of MTab with Tab ID (either selected from parameter or from Record ID)
            MTab tab = new MTab(GetCtx(), _AD_Tab_ID, Get_TrxName());

            if (!(Util.GetValueOfString(DB.ExecuteScalar("SELECT IsMaintainVersions FROM AD_Table WHERE AD_Table_ID = " + tab.GetAD_Table_ID(), null, Get_TrxName())) == "Y"))
            {
                // Checked whether there are any columns with the table which is marked as "Maintain Versions"
                // if no such column found then return message
                if (Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Column_ID) FROM AD_Column WHERE AD_Table_ID = " + tab.GetAD_Table_ID() + " AND IsMaintainVersions = 'Y'", null, Get_TrxName())) <= 0)
                    return Msg.GetMsg(GetCtx(), "NoVerColFound");
            }

            // Always called this function so that in case if new column is added
            // to table this function creates new columns to table
            CreateMasterVersionTable(0, tab.GetAD_Table_ID());
            int Ver_AD_Table_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Table_ID FROM AD_Table WHERE TableName = ((SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + tab.GetAD_Table_ID() + ") || '_Ver')"));
            if (Ver_AD_Table_ID <= 0)
                return Msg.GetMsg(GetCtx(), "VerTableNotFound");

            // Get Display Name of Window linked with the tab
            string displayName = Util.GetValueOfString(DB.ExecuteScalar("SELECT Name FROM AD_Window WHERE AD_Window_ID = " + tab.GetAD_Window_ID(), null, Get_TrxName())) + " Version";

            Ver_AD_Window_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Window_ID FROM AD_Window WHERE Name = '" + displayName + "_" + tab.GetName() + "'", null, Get_TrxName()));
            // Check if Version window do not exist, then create new version window
            if (Ver_AD_Window_ID <= 0)
            {
                Ver_AD_Window_ID = CreateVerWindow(displayName, tab.GetName());
                // if window not created then return message
                if (Ver_AD_Window_ID <= 0)
                    return Msg.GetMsg(GetCtx(), "VersionWinNotCreated");
                else
                    // Update Version Window ID on Version Table
                    DB.ExecuteQuery("UPDATE AD_Table SET AD_Window_ID = " + Ver_AD_Window_ID + " WHERE  AD_Table_ID = " + Ver_AD_Table_ID, null, Get_Trx());
            }

            // check access for Version window
            int countAcc = Util.GetValueOfInt(DB.ExecuteScalar("SELECT COUNT(AD_Role_ID) FROM AD_Window_Access WHERE AD_Window_ID = " + Ver_AD_Window_ID + " AND IsReadWrite = 'Y'", null, Get_Trx()));
            if (countAcc <= 0)
            {
                // Provide access to Version window, for the roles which parent (Master window) has
                countAcc = Util.GetValueOfInt(DB.ExecuteQuery(@"UPDATE AD_Window_Access SET IsReadWrite = 'Y' WHERE AD_Window_ID = " + Ver_AD_Window_ID + @" 
                            AND AD_Role_ID IN (SELECT AD_Role_ID FROM AD_Window_Access WHERE AD_Window_ID = " + tab.GetAD_Window_ID() + @"  AND IsReadWrite = 'Y')", null, Get_Trx()));
            }

            // check for version tab
            Ver_AD_Tab_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Tab_ID FROM AD_Tab WHERE AD_Window_ID = " + Ver_AD_Window_ID, null, Get_TrxName()));
            // if version tab do not exist, then create new version tab
            if (Ver_AD_Tab_ID <= 0)
                Ver_AD_Tab_ID = CreateVerTab(tab, Ver_AD_Window_ID, Ver_AD_Table_ID);
            else
            {
                MTab verTab = new MTab(GetCtx(), Ver_AD_Tab_ID, Get_TrxName());
                CreateVerTabPanel(verTab);
            }

            // if version tab not found then return message
            if (Ver_AD_Tab_ID <= 0)
                return Msg.GetMsg(GetCtx(), "VersionTabNotCreated");

            // Create fields against version Tab
            string retMsg = CreateVerFields(tab, Ver_AD_Tab_ID, Ver_AD_Table_ID);
            if (retMsg != "")
                return retMsg;

            return Msg.GetMsg(GetCtx(), "VersionWindowCreated");
        }

        /// <summary>
        /// function to create window
        /// </summary>
        /// <param name="DisplayName">name of window</param>
        /// <returns>int (Window ID)</returns>
        private int CreateVerWindow(string DisplayName, string TabName)
        {
            // create new version window
            MWindow verWnd = new MWindow(GetCtx(), 0, Get_TrxName());
            verWnd.SetAD_Client_ID(0);
            verWnd.SetAD_Org_ID(0);
            verWnd.SetName(DisplayName + "_" + TabName);
            verWnd.SetDisplayName(DisplayName);
            // set window as Query Only
            verWnd.SetWindowType("M");
            verWnd.SetDescription("Display version data");
            verWnd.SetHelp("The window allows you to view past data versioning and future updation versions (if any).");
            if (!verWnd.Save())
            {
                ValueNamePair vnp = VLogger.RetrieveError();
                string error = "";
                if (vnp != null)
                {
                    error = vnp.GetName();
                    if (error == "" && vnp.GetValue() != null)
                        error = vnp.GetValue();
                }
                if (error == "")
                    error = "Error in creating Version Window";
                log.Log(Level.SEVERE, "Version Window not Created :: " + DisplayName + " :: " + error);
                Get_TrxName().Rollback();
                return 0;
            }
            // Return Window ID
            return verWnd.GetAD_Window_ID();
        }

        /// <summary>
        /// function to create tab against Version Window
        /// </summary>
        /// <param name="tab">Object of MTab</param>
        /// <param name="ver_AD_Window_ID">Version Window ID</param>
        /// <param name="Ver_AD_Table_ID">Version Table ID</param>
        /// <returns>int (Version Tab ID)</returns>
        private int CreateVerTab(MTab tab, int ver_AD_Window_ID, int Ver_AD_Table_ID)
        {
            MTab verTab = new MTab(GetCtx(), 0, Get_TrxName());
            // copy Master table tab to Version tab
            tab.CopyTo(verTab);
            verTab.SetAD_Window_ID(ver_AD_Window_ID);
            verTab.SetAD_Table_ID(Ver_AD_Table_ID);
            verTab.SetIsReadOnly(false);
            verTab.SetIsInsertRecord(false);
            verTab.SetIsSingleRow(true);
            verTab.SetWhereClause(null);
            verTab.SetSeqNo(10);
            verTab.SetAD_Column_ID(0);
            verTab.SetTabLevel(0);
            verTab.SetExport_ID(null);
            verTab.SetDescription("Version tab for " + tab.GetName());
            verTab.SetHelp("Version tab for " + tab.GetName() + ", to display versions for current record");
            // set order by on Version Window's tab on "Version Valid From column"
            verTab.SetOrderByClause("VersionValidFrom DESC, RecordVersion DESC");
            if (!verTab.Save())
            {
                ValueNamePair vnp = VLogger.RetrieveError();
                string error = "";
                if (vnp != null)
                {
                    error = vnp.GetName();
                    if (error == "" && vnp.GetValue() != null)
                        error = vnp.GetValue();
                }
                if (error == "")
                    error = "Error in creating Version Tab";
                log.Log(Level.SEVERE, "Version Tab not Created :: " + tab.GetName() + " :: " + error);
                Get_TrxName().Rollback();
                return 0;
            }
            else
                CreateVerTabPanel(verTab);

            return verTab.GetAD_Tab_ID();
        }

        /// <summary>
        /// Create tab panel for Version window's Tab
        /// </summary>
        /// <param name="tab"></param>
        public void CreateVerTabPanel(MTab tab)
        {
            int AD_TabPanel_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_TabPanel_ID FROM AD_TabPanel WHERE Classname = '" + className + "' AND AD_Tab_ID = " + tab.GetAD_Tab_ID(), null, null));
            if (AD_TabPanel_ID <= 0)
            {
                MTabPanel tbPnl = new MTabPanel(GetCtx(), 0, Get_TrxName());
                tbPnl.SetAD_Client_ID(tab.GetAD_Client_ID());
                tbPnl.SetAD_Org_ID(tab.GetAD_Org_ID());
                tbPnl.SetAD_Tab_ID(tab.GetAD_Tab_ID());
                tbPnl.SetAD_Window_ID(tab.GetAD_Window_ID());
                tbPnl.SetClassname(className);
                tbPnl.SetName(tab.GetName() + " " + Msg.GetMsg(GetCtx(), "VersionHistory"));
                tbPnl.SetIsDefault(true);
                tbPnl.SetSeqNo(Util.GetValueOfInt(DB.ExecuteScalar("SELECT (NVL(MAX(SeqNo), 0) + 10) FROM AD_TabPanel WHERE AD_Tab_ID = " + tab.GetAD_Tab_ID(), null, null)));
                if (!tbPnl.Save())
                {
                    log.SaveError("MasterVerField", "Tab Panel data not created");
                }
            }
        }

        /// <summary>
        /// Create version fields against version tab
        /// </summary>
        /// <param name="tab"> Object of MTab </param>
        /// <param name="Ver_AD_Tab_ID"> Tab ID of Version window </param>
        /// <param name="Ver_AD_Table_ID"> Table ID of Version </param>
        /// <returns>string (Message)</returns>
        private string CreateVerFields(MTab tab, int Ver_AD_Tab_ID, int Ver_AD_Table_ID)
        {
            // Get all fields from Master Tab
            int[] fields = MTable.GetAllIDs("AD_Field", "AD_Tab_ID = " + tab.GetAD_Tab_ID(), Get_TrxName());

            bool hasOrigCols = false;
            bool hasVerFields = false;
            bool hasVerCols = false;

            // Get columns from Master table
            DataSet origColDS = DB.ExecuteDataset("SELECT AD_Column_ID, Name,ColumnSql, ColumnName FROM AD_Column WHERE AD_Table_ID = " + tab.GetAD_Table_ID(), null, Get_TrxName());
            if (origColDS != null && origColDS.Tables[0].Rows.Count > 0)
                hasOrigCols = true;

            // Get fields from Version Tab
            DataSet verFieldDS = DB.ExecuteDataset("SELECT f.AD_Column_ID, f.Name, f.AD_Field_ID, (SELECT c.AD_Element_ID FROM AD_Column c  WHERE c.AD_Column_ID = f.AD_Column_ID) AS AD_Element_ID FROM AD_Field f WHERE f.AD_Tab_ID = " + Ver_AD_Tab_ID, null, Get_TrxName());
            if (verFieldDS != null && verFieldDS.Tables[0].Rows.Count > 0)
                hasVerFields = true;

            // Get Columns from Version Table
            DataSet verColumnsDS = DB.ExecuteDataset("SELECT AD_Column_ID, Name, ColumnName FROM AD_Column WHERE AD_Table_ID = " + Ver_AD_Table_ID, null, Get_TrxName());
            if (verColumnsDS != null && verColumnsDS.Tables[0].Rows.Count > 0)
                hasVerCols = true;

            StringBuilder sbColName = new StringBuilder("");
            foreach (int fld in fields)
            {
                bool createNew = true;
                MField origFld = new MField(GetCtx(), fld, Get_TrxName());

                // check if Field already exist for Version Tab else create new
                if (hasVerFields)
                {
                    DataRow[] drFld = verFieldDS.Tables[0].Select("Name = '" + origFld.GetName() + "'");
                    if (drFld.Length > 0)
                        createNew = false;
                }

                // if Field do not exist on Version tab
                if (createNew)
                {
                    sbColName.Clear();
                    int VerColID = 0;
                    // Get column Info from Column ID of Master Table
                    DataRow[] drOrigColName = origColDS.Tables[0].Select("AD_Column_ID = " + origFld.GetAD_Column_ID());
                    if (drOrigColName.Length > 0)
                    {
                        if (Util.GetValueOfString(drOrigColName[0]["ColumnSQL"]).Trim() != "")
                            continue;
                        sbColName.Append(Util.GetValueOfString(drOrigColName[0]["ColumnName"]));
                        // check whether  Column exist in Version table with column name of Master Table
                        // if column not found return with Message
                        DataRow[] drVerCol = verColumnsDS.Tables[0].Select("ColumnName = '" + sbColName.ToString() + "'");
                        if (drVerCol.Length > 0)
                            VerColID = Util.GetValueOfInt(drVerCol[0]["AD_Column_ID"]);
                        else
                        {
                            log.Log(Level.SEVERE, "Version Column Not Found :: " + sbColName.ToString());
                            Get_TrxName().Rollback();
                            return Msg.GetMsg(GetCtx(), "ColumnNameNotFound") + " :: " + sbColName.ToString();
                        }
                    }
                    else
                    {
                        log.Log(Level.SEVERE, "Column ID not found in Original table :: " + origFld.GetAD_Column_ID());
                        Get_TrxName().Rollback();
                        return Msg.GetMsg(GetCtx(), "ColumnNameNotFound") + " :: " + origFld.GetAD_Column_ID();
                    }

                    // Only create Version Field if Version Column found
                    if (VerColID > 0)
                    {
                        // check if field is already created with column
                        // else skip creating field
                        int fldID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Field_ID FROM AD_Field WHERE AD_Column_ID = " + VerColID + " AND AD_Tab_ID = " + Ver_AD_Tab_ID, null, Get_TrxName()));
                        if (fldID <= 0)
                        {
                            // Create Field for Column, copy field from Master tables Field tab on Version Field against Version Tab
                            MField verFld = new MField(GetCtx(), 0, Get_TrxName());
                            origFld.CopyTo(verFld);
                            verFld.SetAD_Tab_ID(Ver_AD_Tab_ID);
                            verFld.SetAD_Column_ID(VerColID);
                            verFld.SetExport_ID(null);
                            verFld.SetIsReadOnly(true);
                            if (!verFld.Save())
                            {
                                ValueNamePair vnp = VLogger.RetrieveError();
                                string error = "";
                                if (vnp != null)
                                {
                                    error = vnp.GetName();
                                    if (error == "" && vnp.GetValue() != null)
                                        error = vnp.GetValue();
                                }
                                if (error == "")
                                    error = "Error in creating Version Field";
                                log.Log(Level.SEVERE, "Version Field not saved :: " + verFld.GetName() + " :: " + error);
                                Get_TrxName().Rollback();
                                return Msg.GetMsg(GetCtx(), "FieldNotSaved") + " :: " + verFld.GetName();
                            }
                        }
                    }
                }
            }

            // Fill Dataset again from Version Field
            verFieldDS = DB.ExecuteDataset("SELECT f.AD_Column_ID, f.Name, f.AD_Field_ID, (SELECT c.AD_Element_ID FROM AD_Column c  WHERE c.AD_Column_ID = f.AD_Column_ID) AS AD_Element_ID FROM AD_Field f WHERE f.AD_Tab_ID = " + Ver_AD_Tab_ID, null, Get_TrxName());

            // Create Default Fields against default Columns for Version tab
            string retMsg = CreateDefaultFields(Ver_AD_Tab_ID, verFieldDS, hasVerFields, verColumnsDS, Ver_AD_Table_ID);
            if (retMsg != "")
            {
                Get_TrxName().Rollback();
                return retMsg;
            }

            return "";
        }

        /// <summary>
        /// Function to Create Version Fields (Defaults for Version tab) against version tab
        /// </summary>
        /// <param name="ver_AD_Tab_ID"> Version TabID</param>
        /// <param name="verFieldDS">Dataset of Version Fields</param>
        /// <param name="hasVerFlds">check of Version Field</param>
        /// <param name="verColumnsDS"> Dataset of Version Columns</param>
        /// <param name="Ver_TableID"> Version TableID</param>
        /// <returns>String (Message)</returns>
        private string CreateDefaultFields(int ver_AD_Tab_ID, DataSet verFieldDS, bool hasVerFlds, DataSet verColumnsDS, int Ver_TableID)
        {
            // Get system elements against Version Table's Columns
            string VerTableName = Util.GetValueOfString(DB.ExecuteScalar("SELECT TableName FROM AD_Table WHERE AD_Table_ID = " + Ver_TableID, null, Get_Trx()));
            GetSystemElements(VerTableName);

            // Get field Group ID for Versioning
            int AD_FieldGroup_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_FieldGroup_ID FROM AD_FieldGroup WHERE Name = 'Versioning'", null, Get_Trx()));

            // get max seqno + 500 to set seqnumbers for new fields 
            int SeqNo = Util.GetValueOfInt(DB.ExecuteScalar("SELECT MAX(SeqNo) + 500 FROM AD_Field WHERE AD_Tab_ID = " + ver_AD_Tab_ID, null, Get_Trx()));

            for (int i = 0; i < listDefVerCols.Count; i++)
            {
                // check if system element exist 
                DataRow[] drVerFld = verFieldDS.Tables[0].Select("AD_Element_ID = " + _listDefVerElements[i]);
                if (drVerFld.Length <= 0)
                {
                    int VerColID = 0;
                    MField verFld = new MField(GetCtx(), 0, Get_TrxName());
                    verFld.SetAD_Tab_ID(ver_AD_Tab_ID);

                    DataRow[] drVerCol = verColumnsDS.Tables[0].Select("ColumnName = '" + listDefVerCols[i].ToString() + "'");
                    if (drVerCol.Length > 0)
                        VerColID = Util.GetValueOfInt(drVerCol[0]["AD_Column_ID"]);
                    else
                    {
                        log.Log(Level.SEVERE, "Version Column Not Found :: " + listDefVerCols[i].ToString());
                        Get_TrxName().Rollback();
                        return Msg.GetMsg(GetCtx(), "ColumnNameNotFound") + " :: " + listDefVerCols[i].ToString();
                    }

                    verFld.SetAD_Column_ID(VerColID);

                    if (listDefVerCols[i].ToString() == "Processing" || listDefVerCols[i].ToString() == "Processed" || listDefVerCols[i].ToString() == VerTableName + "_ID")
                        verFld.SetIsDisplayed(false);

                    if (listDefVerCols[i].ToString() == "VersionLog")
                        verFld.SetDisplayLength(100);

                    //if (listDefVerCols[i].ToString() == "ProcessedVersion" || listDefVerCols[i].ToString() == "RecordVersion")
                    verFld.SetIsSameLine(true);

                    // Set Field Group for Versioning field
                    if (AD_FieldGroup_ID > 0)
                        verFld.SetAD_FieldGroup_ID(AD_FieldGroup_ID);
                    verFld.SetSeqNo(SeqNo);
                    //"VersionValidFrom", "IsVersionApproved", "ProcessedVersion", "RecordVersion", "Processed", "Processing", "VersionLog", "OldVersion"
                    if (listDefVerCols[i].ToString() == "VersionValidFrom")
                    {
                        verFld.SetMRIsDisplayed("Y");
                        verFld.SetMRSeqNo(5);
                    }
                    if (listDefVerCols[i].ToString() == "RecordVersion")
                    {
                        verFld.SetMRIsDisplayed("Y");
                        verFld.SetMRSeqNo(2);
                    }
                    verFld.SetIsReadOnly(true);
                    if (!verFld.Save())
                    {
                        log.Log(Level.SEVERE, "Version Field not saved :: " + verFld.GetName());
                        Get_TrxName().Rollback();
                        return Msg.GetMsg(GetCtx(), "FieldNotSaved") + " :: " + verFld.GetName();
                    }
                    else
                        SeqNo = SeqNo + 10;
                }
            }
            return "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AD_Column_ID"></param>
        /// <param name="AD_Table_ID"></param>
        /// <returns></returns>
        public string CreateMasterVersionTable(int AD_Column_ID, int AD_Table_ID)
        {
            int AD_Process_ID = Util.GetValueOfInt(DB.ExecuteScalar("SELECT AD_Process_ID FROM AD_Process WHERE Value = 'MasterDataVersions'", null, Get_Trx()));
            MPInstance instance = new MPInstance(GetCtx(), AD_Process_ID, 0);
            if (!instance.Save())
            {
                log.Info(Msg.GetMsg(GetCtx(), "ProcessNoInstance"));
                return Msg.GetMsg(GetCtx(), "ProcessNoInstance");
            }

            ProcessInfo pi = new ProcessInfo("", AD_Process_ID);
            pi.SetAD_PInstance_ID(instance.GetAD_PInstance_ID());
            pi.SetAD_Client_ID(GetCtx().GetAD_Client_ID());

            //	Add Parameters
            MPInstancePara para = new MPInstancePara(instance, 10);
            //para.setParameter("Selection", "Y");
            para.setParameter("AD_Table_ID", AD_Table_ID);
            if (!para.Save())
            {
                String msg = "Table parameter not found";
                log.Info(msg.ToString());
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            para = new MPInstancePara(instance, 20);
            para.setParameter("AD_Column_ID", AD_Column_ID);
            if (!para.Save())
            {
                String msg = "Column parameter not found";
                log.Info(msg.ToString());
                //log.Log(Level.SEVERE, msg);
                return msg.ToString();
            }

            ProcessCtl worker = new ProcessCtl(GetCtx(), null, pi, Get_Trx());
            worker.Run();
            string retMsg = pi.GetSummary();
            return "Test";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="VerTblName"></param>
        public void GetSystemElements(string VerTblName)
        {
            if (_listDefVerElements.Count == listDefVerCols.Count)
                return;

            _listDefVerElements.Clear();

            if (!listDefVerCols.Contains(VerTblName + "_ID"))
                listDefVerCols.Add(VerTblName + "_ID");

            string DefSysEle = string.Join(",", listDefVerCols
                                            .Select(x => string.Format("'{0}'", x)));

            DataSet dsDefVerCols = DB.ExecuteDataset("SELECT AD_Element_ID, ColumnName FROM AD_Element WHERE ColumnName IN (" + DefSysEle + ")", null, Get_TrxName());

            if (dsDefVerCols != null && dsDefVerCols.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < listDefVerCols.Count; i++)
                {
                    DataRow[] drSysEle = dsDefVerCols.Tables[0].Select("ColumnName='" + listDefVerCols[i] + "'");
                    if (drSysEle.Length > 0)
                    {
                        if (!_listDefVerElements.Contains(Util.GetValueOfInt(drSysEle[0]["AD_Element_ID"])))
                            _listDefVerElements.Add(Util.GetValueOfInt(drSysEle[0]["AD_Element_ID"]));
                    }
                    else
                    {
                        M_Element ele = new M_Element(GetCtx(), 0, Get_Trx());
                        ele.SetAD_Client_ID(0);
                        ele.SetAD_Org_ID(0);
                        ele.SetName(listDefVerCols[i]);
                        ele.SetColumnName(listDefVerCols[i]);
                        ele.SetPrintName(listDefVerCols[i]);
                        if (!ele.Save())
                        {

                        }
                        else
                        {
                            _listDefVerElements.Add(ele.GetAD_Element_ID());
                            if (ele.GetColumnName() == VerTblName + "_ID")
                            {
                                listDefVerRef.Add(13);
                            }
                        }
                    }
                }
            }
        }
    }
}